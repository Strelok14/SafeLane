using SecureFileTransfer.Core.Models;

namespace SecureFileTransfer.App.Dialogs;

public sealed class AddNodeForm : Form
{
    private readonly TextBox _txtNodeId = new() { PlaceholderText = "идентификатор-узла" };
    private readonly TextBox _txtName = new() { PlaceholderText = "Отображаемое имя" };
    private readonly TextBox _txtBaseUrl = new() { PlaceholderText = "http://192.168.0.20:5077" };
    private readonly TextBox _txtSecret = new() { PlaceholderText = "Свой секрет (необязательно)" };

    public TrustedNodeConfig? Result { get; private set; }

    public AddNodeForm()
    {
        Text = "Добавить доверенный узел";
        Width = 520;
        Height = 250;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 5,
            Padding = new Padding(10)
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        table.Controls.Add(new Label { Text = "ID узла", AutoSize = true }, 0, 0);
        table.Controls.Add(_txtNodeId, 1, 0);
        table.Controls.Add(new Label { Text = "Имя", AutoSize = true }, 0, 1);
        table.Controls.Add(_txtName, 1, 1);
        table.Controls.Add(new Label { Text = "Адрес (URL)", AutoSize = true }, 0, 2);
        table.Controls.Add(_txtBaseUrl, 1, 2);
        table.Controls.Add(new Label { Text = "Свой секрет", AutoSize = true }, 0, 3);
        table.Controls.Add(_txtSecret, 1, 3);

        var panelButtons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var btnOk = new Button { Text = "ОК", Width = 90 };
        var btnCancel = new Button { Text = "Отмена", Width = 90 };
        btnOk.Click += (_, _) => SaveAndClose();
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        panelButtons.Controls.Add(btnOk);
        panelButtons.Controls.Add(btnCancel);

        table.Controls.Add(panelButtons, 1, 4);
        Controls.Add(table);
    }

    private void SaveAndClose()
    {
        if (string.IsNullOrWhiteSpace(_txtNodeId.Text) ||
            string.IsNullOrWhiteSpace(_txtName.Text) ||
            string.IsNullOrWhiteSpace(_txtBaseUrl.Text))
        {
            MessageBox.Show("Заполните ID узла, Имя и Адрес.", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        Result = new TrustedNodeConfig
        {
            NodeId = _txtNodeId.Text.Trim(),
            DisplayName = _txtName.Text.Trim(),
            BaseUrl = _txtBaseUrl.Text.Trim(),
            SharedSecretOverride = string.IsNullOrWhiteSpace(_txtSecret.Text) ? null : _txtSecret.Text.Trim()
        };

        DialogResult = DialogResult.OK;
    }
}
