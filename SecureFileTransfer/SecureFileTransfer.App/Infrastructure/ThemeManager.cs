using System.Drawing;
using System.Windows.Forms;

namespace SecureFileTransfer.App.Infrastructure;

/// <summary>
/// Управляет визуальной темой приложения (тёмная/светлая)
/// </summary>
public class ThemeManager
{
    // Цвета для тёмной темы
    private static readonly Color DarkBg = Color.FromArgb(30, 30, 30);
    private static readonly Color DarkFg = Color.FromArgb(240, 240, 240);
    private static readonly Color DarkControlBg = Color.FromArgb(45, 45, 48);
    private static readonly Color DarkBorder = Color.FromArgb(60, 60, 65);
    private static readonly Color DarkButton = Color.FromArgb(63, 63, 70);
    private static readonly Color DarkButtonHover = Color.FromArgb(75, 75, 82);
    private static readonly Color DarkAccent = Color.FromArgb(0, 122, 204);

    public static void ApplyDarkTheme(Form form)
    {
        ApplyThemeToControl(form, isDark: true);
    }

    public static void ApplyLightTheme(Form form)
    {
        ApplyThemeToControl(form, isDark: false);
    }

    private static void ApplyThemeToControl(Control control, bool isDark)
    {
        if (isDark)
        {
            control.BackColor = DarkBg;
            control.ForeColor = DarkFg;
        }
        else
        {
            control.BackColor = SystemColors.Control;
            control.ForeColor = SystemColors.ControlText;
        }

        // Применяем тему к элементам управления
        foreach (Control child in control.Controls)
        {
            if (child is Button button)
            {
                if (isDark)
                {
                    button.BackColor = DarkButton;
                    button.ForeColor = DarkFg;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = DarkBorder;
                }
                else
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = SystemColors.ControlText;
                    button.FlatStyle = FlatStyle.Standard;
                }
            }
            else if (child is TextBox textbox)
            {
                if (isDark)
                {
                    textbox.BackColor = DarkControlBg;
                    textbox.ForeColor = DarkFg;
                }
            }
            else if (child is DataGridView dgv)
            {
                if (isDark)
                {
                    dgv.BackgroundColor = DarkBg;
                    dgv.GridColor = DarkBorder;
                    dgv.DefaultCellStyle.BackColor = DarkControlBg;
                    dgv.DefaultCellStyle.ForeColor = DarkFg;
                    dgv.DefaultCellStyle.SelectionBackColor = DarkAccent;
                    dgv.DefaultCellStyle.SelectionForeColor = DarkFg;
                    dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkButton;
                    dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkFg;
                    dgv.RowHeadersDefaultCellStyle.BackColor = DarkButton;
                }
            }
            else if (child is ListBox listbox)
            {
                if (isDark)
                {
                    listbox.BackColor = DarkControlBg;
                    listbox.ForeColor = DarkFg;
                }
            }
            else if (child is ProgressBar progressbar)
            {
                // ProgressBar doesn't support direct color changes in easy way
                // But we can style it through FlatStyle
            }
            else if (child is Label label)
            {
                if (isDark)
                {
                    label.BackColor = DarkBg;
                    label.ForeColor = DarkFg;
                }
            }
            else if (child is Panel || child is TableLayoutPanel || child is FlowLayoutPanel)
            {
                if (isDark)
                {
                    child.BackColor = DarkBg;
                }
                // Рекурсивно применяем тему к дочерним элементам
                ApplyThemeToControl(child, isDark);
                continue;
            }

            // Рекурсивно применяем тему к дочерним элементам
            if (child.HasChildren)
                ApplyThemeToControl(child, isDark);
        }
    }
}
