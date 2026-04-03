namespace SecureFileTransfer.App;

using System.ComponentModel;
using SecureFileTransfer.App.Dialogs;
using SecureFileTransfer.App.Infrastructure;
using SecureFileTransfer.App.Networking;
using SecureFileTransfer.App.ViewModels;
using SecureFileTransfer.Core.Models;

public partial class Form1 : Form
{
    private readonly BindingList<NodeViewModel> _nodes = new();
    private readonly string _appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
    private readonly SequenceGenerator _sequenceGenerator = new();

    private SettingsStore? _settingsStore;
    private TrustedNodeStore? _trustedNodeStore;
    private AppSettings? _settings;
    private List<TrustedNodeConfig> _trustedNodes = new();
    private LocalServerHost? _localServer;
    private NodeDiscoveryService? _discovery;
    private string? _selectedFilePath;
    private readonly CancellationTokenSource _cts = new();
    private readonly System.Windows.Forms.Timer _heartbeatTimer = new();

    public Form1()
    {
        InitializeComponent();
        gridNodes.DataSource = _nodes;
        _heartbeatTimer.Interval = 5000;
        _heartbeatTimer.Tick += (_, _) => RefreshNodeOnlineStatus();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        try
        {
            // Применяем тёмную тему
            ThemeManager.ApplyDarkTheme(this);

            _settingsStore = new SettingsStore(_appDataPath);
            _trustedNodeStore = new TrustedNodeStore(_appDataPath);

            _settings = _settingsStore.Load();
            _trustedNodes = _trustedNodeStore.Load();
            PopulateTrustedNodesToGrid();

            _localServer = new LocalServerHost(
                _settings,
                () => _trustedNodes,
                Log,
                UpdateReceiveProgress);
            await _localServer.StartAsync(_cts.Token);

            _discovery = new NodeDiscoveryService(
                _settings.NodeId,
                _settings.NodeName,
                _settings.DiscoveryPort,
                () => _settings.ListenPort,
                Log);
            _discovery.NodeSeen += OnNodeSeen;
            _discovery.Start();

            _heartbeatTimer.Start();
            lblStatus.Text = $"Статус: готово. Узел={_settings.NodeId}";
            Log("Приложение SafeLane запущено.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Ошибка запуска", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }

    private void PopulateTrustedNodesToGrid()
    {
        _nodes.Clear();
        foreach (var node in _trustedNodes)
        {
            _nodes.Add(new NodeViewModel
            {
                NodeId = node.NodeId,
                Name = node.DisplayName,
                BaseUrl = node.BaseUrl,
                IsTrusted = true,
                IsOnline = false,
                LastSeenUtc = DateTime.MinValue
            });
        }
    }

    private void OnNodeSeen(NodeAnnouncement ann, System.Net.IPEndPoint remoteEndPoint)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => OnNodeSeen(ann, remoteEndPoint));
            return;
        }

        var existing = _nodes.FirstOrDefault(n => n.NodeId == ann.NodeId);
        if (existing is null)
        {
            var trusted = _trustedNodes.FirstOrDefault(t => t.NodeId == ann.NodeId);
            _nodes.Add(new NodeViewModel
            {
                NodeId = ann.NodeId,
                Name = trusted?.DisplayName ?? ann.NodeName,
                BaseUrl = trusted?.BaseUrl ?? ann.BaseUrl,
                IsTrusted = trusted is not null,
                IsOnline = true,
                LastSeenUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.IsOnline = true;
            existing.LastSeenUtc = DateTime.UtcNow;
            existing.BaseUrl = ann.BaseUrl;
            if (!existing.IsTrusted)
            {
                existing.Name = ann.NodeName;
            }
        }

        gridNodes.Refresh();
    }

    private void RefreshNodeOnlineStatus()
    {
        var threshold = DateTime.UtcNow.AddSeconds(-15);
        foreach (var node in _nodes)
        {
            node.IsOnline = node.LastSeenUtc >= threshold;
        }

        gridNodes.Refresh();
    }

    private void btnChooseFile_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Выберите файл для отправки",
            CheckFileExists = true,
            Multiselect = false
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _selectedFilePath = dialog.FileName;
        txtSelectedFile.Text = _selectedFilePath;
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        if (_settings is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(_selectedFilePath) || !File.Exists(_selectedFilePath))
        {
            MessageBox.Show(this, "Выберите корректный файл перед отправкой.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var selected = gridNodes.CurrentRow?.DataBoundItem as NodeViewModel;
        if (selected is null)
        {
            MessageBox.Show(this, "Выберите целевой узел.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var trusted = _trustedNodes.FirstOrDefault(n => n.NodeId == selected.NodeId);
        if (trusted is null)
        {
            MessageBox.Show(this, "Целевой узел не в списке доверенных. Добавьте его сначала.", "Безопасность", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnSend.Enabled = false;
        progressSend.Value = 0;
        lblStatus.Text = "Статус: отправка...";

        try
        {
            var senderService = new FileSenderService(_settings, _sequenceGenerator, Log);
            await senderService.SendFileAsync(_selectedFilePath, trusted, _cts.Token, (done, total) =>
            {
                if (InvokeRequired)
                {
                    BeginInvoke(() => UpdateSendProgress(done, total));
                }
                else
                {
                    UpdateSendProgress(done, total);
                }
            });

            lblStatus.Text = "Статус: отправка завершена";
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Статус: ошибка отправки";
            Log($"Ошибка отправки: {ex.Message}");
            MessageBox.Show(this, ex.Message, "Ошибка отправки", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnSend.Enabled = true;
        }
    }

    private void UpdateSendProgress(int done, int total)
    {
        var percent = total == 0 ? 0 : (int)((double)done / total * 100);
        progressSend.Value = Math.Max(0, Math.Min(100, percent));
        lblStatus.Text = $"Статус: отправка {done}/{total}";
    }

    private void UpdateReceiveProgress(int done, int total)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => UpdateReceiveProgress(done, total));
            return;
        }

        var percent = total == 0 ? 0 : (int)((double)done / total * 100);
        progressReceive.Value = Math.Max(0, Math.Min(100, percent));
        lblStatus.Text = $"Статус: получение {done}/{total}";
    }

    private void btnAddNode_Click(object sender, EventArgs e)
    {
        if (_trustedNodeStore is null)
        {
            return;
        }

        using var dialog = new AddNodeForm();
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        _trustedNodes.RemoveAll(n => n.NodeId == dialog.Result.NodeId);
        _trustedNodes.Add(dialog.Result);
        _trustedNodeStore.Save(_trustedNodes);
        PopulateTrustedNodesToGrid();
        Log($"Доверенный узел добавлен: {dialog.Result.DisplayName} ({dialog.Result.NodeId})");
    }

    private void btnRemoveNode_Click(object sender, EventArgs e)
    {
        if (_trustedNodeStore is null)
        {
            return;
        }

        var selected = gridNodes.CurrentRow?.DataBoundItem as NodeViewModel;
        if (selected is null)
        {
            return;
        }

        _trustedNodes.RemoveAll(n => n.NodeId == selected.NodeId);
        _trustedNodeStore.Save(_trustedNodes);
        PopulateTrustedNodesToGrid();
        Log($"Доверенный узел удален: {selected.NodeId}");
    }

    private void btnSettings_Click(object sender, EventArgs e)
    {
        if (_settingsStore is null || _settings is null)
        {
            return;
        }

        using var dialog = new SettingsForm(_settings);
        if (dialog.ShowDialog(this) != DialogResult.OK || dialog.Result is null)
        {
            return;
        }

        _settings = dialog.Result;
        _settingsStore.Save(_settings);
        Log("Настройки сохранены. Перезагрузите приложение для применения изменений порта прослушивания.");
    }

    private void Log(string message)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => Log(message));
            return;
        }

        var line = $"[{DateTime.Now:HH:mm:ss}] {message}";
        listLogs.Items.Insert(0, line);
        while (listLogs.Items.Count > 500)
        {
            listLogs.Items.RemoveAt(listLogs.Items.Count - 1);
        }
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _heartbeatTimer.Stop();
        _cts.Cancel();

        _discovery?.Dispose();
        if (_localServer is not null)
        {
            await _localServer.DisposeAsync();
        }

        _cts.Dispose();
    }
}
