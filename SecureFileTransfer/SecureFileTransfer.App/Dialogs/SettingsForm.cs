using SecureFileTransfer.App.Infrastructure;

namespace SecureFileTransfer.App.Dialogs;

public sealed class SettingsForm : Form
{
    private readonly TextBox _txtNodeName = new();
    private readonly TextBox _txtSharedSecret = new();
    private readonly NumericUpDown _numPort = new() { Minimum = 1024, Maximum = 65535 };
    private readonly NumericUpDown _numChunkKb = new() { Minimum = 16, Maximum = 1024 };
    private readonly NumericUpDown _numRate = new() { Minimum = 1, Maximum = 100 };
    private readonly TextBox _txtReceivedDir = new();

    public AppSettings? Result { get; private set; }

    public SettingsForm(AppSettings current)
    {
        Text = "Settings";
        Width = 600;
        Height = 340;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        _txtNodeName.Text = current.NodeName;
        _txtSharedSecret.Text = current.SharedSecret;
        _numPort.Value = current.ListenPort;
        _numChunkKb.Value = current.ChunkSizeKb;
        _numRate.Value = current.MaxRequestsPerSecond;
        _txtReceivedDir.Text = current.ReceivedDirectory;

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 7,
            Padding = new Padding(10)
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        table.Controls.Add(new Label { Text = "Node name", AutoSize = true }, 0, 0);
        table.Controls.Add(_txtNodeName, 1, 0);

        table.Controls.Add(new Label { Text = "Listen port", AutoSize = true }, 0, 1);
        table.Controls.Add(_numPort, 1, 1);

        table.Controls.Add(new Label { Text = "Shared secret", AutoSize = true }, 0, 2);
        table.Controls.Add(_txtSharedSecret, 1, 2);

        table.Controls.Add(new Label { Text = "Chunk size (KB)", AutoSize = true }, 0, 3);
        table.Controls.Add(_numChunkKb, 1, 3);

        table.Controls.Add(new Label { Text = "Rate limit (req/s)", AutoSize = true }, 0, 4);
        table.Controls.Add(_numRate, 1, 4);

        table.Controls.Add(new Label { Text = "Received folder", AutoSize = true }, 0, 5);
        table.Controls.Add(_txtReceivedDir, 1, 5);
        var btnBrowse = new Button { Text = "Browse...", Width = 90 };
        btnBrowse.Click += (_, _) => BrowseFolder();
        table.Controls.Add(btnBrowse, 2, 5);

        var panelButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnOk = new Button { Text = "OK", Width = 90 };
        var btnCancel = new Button { Text = "Cancel", Width = 90 };
        btnOk.Click += (_, _) => SaveAndClose(current);
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        panelButtons.Controls.Add(btnOk);
        panelButtons.Controls.Add(btnCancel);
        table.Controls.Add(panelButtons, 1, 6);

        Controls.Add(table);
    }

    private void BrowseFolder()
    {
        using var dialog = new FolderBrowserDialog();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtReceivedDir.Text = dialog.SelectedPath;
        }
    }

    private void SaveAndClose(AppSettings current)
    {
        if (string.IsNullOrWhiteSpace(_txtNodeName.Text) || string.IsNullOrWhiteSpace(_txtSharedSecret.Text))
        {
            MessageBox.Show("Node name and shared secret are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new AppSettings
        {
            NodeId = current.NodeId,
            DiscoveryPort = current.DiscoveryPort,
            NodeName = _txtNodeName.Text.Trim(),
            SharedSecret = _txtSharedSecret.Text,
            ListenPort = (int)_numPort.Value,
            ChunkSizeKb = (int)_numChunkKb.Value,
            MaxRequestsPerSecond = (int)_numRate.Value,
            ReceivedDirectory = _txtReceivedDir.Text.Trim()
        };

        DialogResult = DialogResult.OK;
    }
}
