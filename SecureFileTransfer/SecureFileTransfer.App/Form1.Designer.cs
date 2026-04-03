namespace SecureFileTransfer.App;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.tableRoot = new System.Windows.Forms.TableLayoutPanel();
        this.panelTopButtons = new System.Windows.Forms.FlowLayoutPanel();
        this.btnAddNode = new System.Windows.Forms.Button();
        this.btnRemoveNode = new System.Windows.Forms.Button();
        this.btnSettings = new System.Windows.Forms.Button();
        this.gridNodes = new System.Windows.Forms.DataGridView();
        this.panelSend = new System.Windows.Forms.TableLayoutPanel();
        this.txtSelectedFile = new System.Windows.Forms.TextBox();
        this.btnChooseFile = new System.Windows.Forms.Button();
        this.btnSend = new System.Windows.Forms.Button();
        this.progressSend = new System.Windows.Forms.ProgressBar();
        this.progressReceive = new System.Windows.Forms.ProgressBar();
        this.lblStatus = new System.Windows.Forms.Label();
        this.listLogs = new System.Windows.Forms.ListBox();
        this.tableRoot.SuspendLayout();
        this.panelTopButtons.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.gridNodes)).BeginInit();
        this.panelSend.SuspendLayout();
        this.SuspendLayout();
        // 
        // tableRoot
        // 
        this.tableRoot.ColumnCount = 1;
        this.tableRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableRoot.Controls.Add(this.panelTopButtons, 0, 0);
        this.tableRoot.Controls.Add(this.gridNodes, 0, 1);
        this.tableRoot.Controls.Add(this.panelSend, 0, 2);
        this.tableRoot.Controls.Add(this.progressSend, 0, 3);
        this.tableRoot.Controls.Add(this.progressReceive, 0, 4);
        this.tableRoot.Controls.Add(this.lblStatus, 0, 5);
        this.tableRoot.Controls.Add(this.listLogs, 0, 6);
        this.tableRoot.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableRoot.Location = new System.Drawing.Point(0, 0);
        this.tableRoot.Name = "tableRoot";
        this.tableRoot.RowCount = 7;
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
        this.tableRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
        this.tableRoot.Size = new System.Drawing.Size(1100, 720);
        this.tableRoot.TabIndex = 0;
        // 
        // panelTopButtons
        // 
        this.panelTopButtons.Controls.Add(this.btnAddNode);
        this.panelTopButtons.Controls.Add(this.btnRemoveNode);
        this.panelTopButtons.Controls.Add(this.btnSettings);
        this.panelTopButtons.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelTopButtons.Location = new System.Drawing.Point(3, 3);
        this.panelTopButtons.Name = "panelTopButtons";
        this.panelTopButtons.Size = new System.Drawing.Size(1094, 38);
        this.panelTopButtons.TabIndex = 0;
        // 
        // btnAddNode
        // 
        this.btnAddNode.Location = new System.Drawing.Point(3, 3);
        this.btnAddNode.Name = "btnAddNode";
        this.btnAddNode.Size = new System.Drawing.Size(122, 30);
        this.btnAddNode.TabIndex = 0;
        this.btnAddNode.Text = "Добавить узел";
        this.btnAddNode.UseVisualStyleBackColor = true;
        this.btnAddNode.Click += new System.EventHandler(this.btnAddNode_Click);
        // 
        // btnRemoveNode
        // 
        this.btnRemoveNode.Location = new System.Drawing.Point(131, 3);
        this.btnRemoveNode.Name = "btnRemoveNode";
        this.btnRemoveNode.Size = new System.Drawing.Size(122, 30);
        this.btnRemoveNode.TabIndex = 1;
        this.btnRemoveNode.Text = "Удалить узел";
        this.btnRemoveNode.UseVisualStyleBackColor = true;
        this.btnRemoveNode.Click += new System.EventHandler(this.btnRemoveNode_Click);
        // 
        // btnSettings
        // 
        this.btnSettings.Location = new System.Drawing.Point(259, 3);
        this.btnSettings.Name = "btnSettings";
        this.btnSettings.Size = new System.Drawing.Size(122, 30);
        this.btnSettings.TabIndex = 2;
        this.btnSettings.Text = "Настройки";
        this.btnSettings.UseVisualStyleBackColor = true;
        this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
        // 
        // gridNodes
        // 
        this.gridNodes.AllowUserToAddRows = false;
        this.gridNodes.AllowUserToDeleteRows = false;
        this.gridNodes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.gridNodes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.gridNodes.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gridNodes.Location = new System.Drawing.Point(3, 47);
        this.gridNodes.MultiSelect = false;
        this.gridNodes.Name = "gridNodes";
        this.gridNodes.ReadOnly = true;
        this.gridNodes.RowHeadersVisible = false;
        this.gridNodes.RowTemplate.Height = 25;
        this.gridNodes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.gridNodes.Size = new System.Drawing.Size(1094, 218);
        this.gridNodes.TabIndex = 1;
        // 
        // panelSend
        // 
        this.panelSend.ColumnCount = 3;
        this.panelSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.panelSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
        this.panelSend.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
        this.panelSend.Controls.Add(this.txtSelectedFile, 0, 0);
        this.panelSend.Controls.Add(this.btnChooseFile, 1, 0);
        this.panelSend.Controls.Add(this.btnSend, 2, 0);
        this.panelSend.Dock = System.Windows.Forms.DockStyle.Fill;
        this.panelSend.Location = new System.Drawing.Point(3, 271);
        this.panelSend.Name = "panelSend";
        this.panelSend.RowCount = 1;
        this.panelSend.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.panelSend.Size = new System.Drawing.Size(1094, 46);
        this.panelSend.TabIndex = 2;
        // 
        // txtSelectedFile
        // 
        this.txtSelectedFile.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtSelectedFile.Location = new System.Drawing.Point(3, 10);
        this.txtSelectedFile.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
        this.txtSelectedFile.Name = "txtSelectedFile";
        this.txtSelectedFile.PlaceholderText = "Выберите файл для отправки";
        this.txtSelectedFile.ReadOnly = true;
        this.txtSelectedFile.Size = new System.Drawing.Size(808, 23);
        this.txtSelectedFile.TabIndex = 0;
        // 
        // btnChooseFile
        // 
        this.btnChooseFile.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnChooseFile.Location = new System.Drawing.Point(817, 3);
        this.btnChooseFile.Name = "btnChooseFile";
        this.btnChooseFile.Size = new System.Drawing.Size(134, 40);
        this.btnChooseFile.TabIndex = 1;
        this.btnChooseFile.Text = "Выбрать файл";
        this.btnChooseFile.UseVisualStyleBackColor = true;
        this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
        // 
        // btnSend
        // 
        this.btnSend.Dock = System.Windows.Forms.DockStyle.Fill;
        this.btnSend.Location = new System.Drawing.Point(957, 3);
        this.btnSend.Name = "btnSend";
        this.btnSend.Size = new System.Drawing.Size(134, 40);
        this.btnSend.TabIndex = 2;
        this.btnSend.Text = "Отправить";
        this.btnSend.UseVisualStyleBackColor = true;
        this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
        // 
        // progressSend
        // 
        this.progressSend.Dock = System.Windows.Forms.DockStyle.Fill;
        this.progressSend.Location = new System.Drawing.Point(3, 323);
        this.progressSend.Name = "progressSend";
        this.progressSend.Size = new System.Drawing.Size(1094, 20);
        this.progressSend.TabIndex = 3;
        // 
        // progressReceive
        // 
        this.progressReceive.Dock = System.Windows.Forms.DockStyle.Fill;
        this.progressReceive.Location = new System.Drawing.Point(3, 349);
        this.progressReceive.Name = "progressReceive";
        this.progressReceive.Size = new System.Drawing.Size(1094, 20);
        this.progressReceive.TabIndex = 4;
        // 
        // lblStatus
        // 
        this.lblStatus.AutoSize = true;
        this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
        this.lblStatus.Location = new System.Drawing.Point(3, 372);
        this.lblStatus.Name = "lblStatus";
        this.lblStatus.Size = new System.Drawing.Size(1094, 24);
        this.lblStatus.TabIndex = 5;
        this.lblStatus.Text = "Статус: ожидание";
        this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
        // 
        // listLogs
        // 
        this.listLogs.Dock = System.Windows.Forms.DockStyle.Fill;
        this.listLogs.FormattingEnabled = true;
        this.listLogs.HorizontalScrollbar = true;
        this.listLogs.ItemHeight = 15;
        this.listLogs.Location = new System.Drawing.Point(3, 399);
        this.listLogs.Name = "listLogs";
        this.listLogs.Size = new System.Drawing.Size(1094, 318);
        this.listLogs.TabIndex = 6;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1100, 720);
        this.Controls.Add(this.tableRoot);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "SafeLane - Безопасная передача файлов";
        this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
        this.Load += new System.EventHandler(this.Form1_Load);
        this.tableRoot.ResumeLayout(false);
        this.tableRoot.PerformLayout();
        this.panelTopButtons.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.gridNodes)).EndInit();
        this.panelSend.ResumeLayout(false);
        this.panelSend.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableRoot;
    private System.Windows.Forms.FlowLayoutPanel panelTopButtons;
    private System.Windows.Forms.Button btnAddNode;
    private System.Windows.Forms.Button btnRemoveNode;
    private System.Windows.Forms.Button btnSettings;
    private System.Windows.Forms.DataGridView gridNodes;
    private System.Windows.Forms.TableLayoutPanel panelSend;
    private System.Windows.Forms.TextBox txtSelectedFile;
    private System.Windows.Forms.Button btnChooseFile;
    private System.Windows.Forms.Button btnSend;
    private System.Windows.Forms.ProgressBar progressSend;
    private System.Windows.Forms.ProgressBar progressReceive;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.ListBox listLogs;
}

