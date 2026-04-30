namespace RcloneMountGUI
{
    partial class RcloneMount
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RcloneMount));
            this.txtRLocation = new System.Windows.Forms.TextBox();
            this.lblRcloneLocation = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.openFileDialogRclone = new System.Windows.Forms.OpenFileDialog();
            this.btnMount = new System.Windows.Forms.Button();
            this.lblMountOptions = new System.Windows.Forms.Label();
            this.comboBoxMount = new System.Windows.Forms.ComboBox();
            this.lblRemoteName = new System.Windows.Forms.Label();
            this.txtRemoteName = new System.Windows.Forms.TextBox();
            this.lblDriveLetter = new System.Windows.Forms.Label();
            this.txtDriveLetter = new System.Windows.Forms.ComboBox();
            this.btnUnmount = new System.Windows.Forms.Button();
            this.linklblDocument = new System.Windows.Forms.LinkLabel();
            this.checkBoxRAAS = new System.Windows.Forms.CheckBox();
            this.notifyIconRclone = new System.Windows.Forms.NotifyIcon(this.components);
            this.backgroundWorkerRclone = new System.ComponentModel.BackgroundWorker();
            this.btnConfig = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.checkBoxDarkMode = new System.Windows.Forms.CheckBox();
            this.toolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.panelStatus = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // txtRLocation
            // 
            this.txtRLocation.Location = new System.Drawing.Point(214, 24);
            this.txtRLocation.Name = "txtRLocation";
            this.txtRLocation.ReadOnly = true;
            this.txtRLocation.Size = new System.Drawing.Size(385, 34);
            this.txtRLocation.TabIndex = 0;
            this.txtRLocation.TabStop = false;
            this.toolTipMain.SetToolTip(this.txtRLocation, "Path to rclone.exe");
            // 
            // lblRcloneLocation
            // 
            this.lblRcloneLocation.AutoSize = true;
            this.lblRcloneLocation.Location = new System.Drawing.Point(30, 27);
            this.lblRcloneLocation.Name = "lblRcloneLocation";
            this.lblRcloneLocation.Size = new System.Drawing.Size(154, 28);
            this.lblRcloneLocation.TabIndex = 100;
            this.lblRcloneLocation.Text = "Rclone Location:";
            // 
            // btnSelect
            // 
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Location = new System.Drawing.Point(608, 21);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(112, 45);
            this.btnSelect.TabIndex = 10;
            this.btnSelect.TabStop = false;
            this.btnSelect.Text = "Select";
            this.toolTipMain.SetToolTip(this.btnSelect, "Browse for rclone.exe");
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // openFileDialogRclone
            // 
            this.openFileDialogRclone.FileName = "rclone";
            this.openFileDialogRclone.Filter = "Exe Files (*.exe)|*.exe";
            // 
            // btnMount
            // 
            this.btnMount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnMount.Location = new System.Drawing.Point(214, 232);
            this.btnMount.Name = "btnMount";
            this.btnMount.Size = new System.Drawing.Size(232, 57);
            this.btnMount.TabIndex = 11;
            this.btnMount.TabStop = false;
            this.btnMount.Text = "Mount";
            this.toolTipMain.SetToolTip(this.btnMount, "Mount the remote drive");
            this.btnMount.UseVisualStyleBackColor = true;
            this.btnMount.Click += new System.EventHandler(this.btnMount_Click);
            // 
            // lblMountOptions
            // 
            this.lblMountOptions.AutoSize = true;
            this.lblMountOptions.Location = new System.Drawing.Point(34, 162);
            this.lblMountOptions.Name = "lblMountOptions";
            this.lblMountOptions.Size = new System.Drawing.Size(150, 28);
            this.lblMountOptions.TabIndex = 103;
            this.lblMountOptions.Text = "Mount Options:";
            // 
            // comboBoxMount
            // 
            this.comboBoxMount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxMount.Items.AddRange(new object[] {
            "--vfs-cache-mode off",
            "--vfs-cache-mode minimal",
            "--vfs-cache-mode writes",
            "--vfs-cache-mode full"});
            this.comboBoxMount.Location = new System.Drawing.Point(214, 159);
            this.comboBoxMount.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBoxMount.Name = "comboBoxMount";
            this.comboBoxMount.Size = new System.Drawing.Size(504, 36);
            this.comboBoxMount.TabIndex = 3;
            this.comboBoxMount.TabStop = false;
            this.toolTipMain.SetToolTip(this.comboBoxMount, "VFS cache mode for rclone mount");
            // 
            // lblRemoteName
            // 
            this.lblRemoteName.AutoSize = true;
            this.lblRemoteName.Location = new System.Drawing.Point(44, 72);
            this.lblRemoteName.Name = "lblRemoteName";
            this.lblRemoteName.Size = new System.Drawing.Size(140, 28);
            this.lblRemoteName.TabIndex = 101;
            this.lblRemoteName.Text = "Remote Name:";
            // 
            // txtRemoteName
            // 
            this.txtRemoteName.Location = new System.Drawing.Point(214, 69);
            this.txtRemoteName.Name = "txtRemoteName";
            this.txtRemoteName.Size = new System.Drawing.Size(504, 34);
            this.txtRemoteName.TabIndex = 1;
            this.txtRemoteName.TabStop = false;
            this.toolTipMain.SetToolTip(this.txtRemoteName, "Name of the configured rclone remote");
            // 
            // lblDriveLetter
            // 
            this.lblDriveLetter.AutoSize = true;
            this.lblDriveLetter.Location = new System.Drawing.Point(67, 117);
            this.lblDriveLetter.Name = "lblDriveLetter";
            this.lblDriveLetter.Size = new System.Drawing.Size(117, 28);
            this.lblDriveLetter.TabIndex = 102;
            this.lblDriveLetter.Text = "Drive Letter:";
            // 
            // txtDriveLetter
            // 
            this.txtDriveLetter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.txtDriveLetter.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.txtDriveLetter.Location = new System.Drawing.Point(214, 114);
            this.txtDriveLetter.Name = "txtDriveLetter";
            this.txtDriveLetter.Size = new System.Drawing.Size(88, 36);
            this.txtDriveLetter.TabIndex = 2;
            this.txtDriveLetter.TabStop = false;
            this.toolTipMain.SetToolTip(this.txtDriveLetter, "Available drive letter to mount");
            // 
            // btnUnmount
            // 
            this.btnUnmount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUnmount.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnUnmount.Location = new System.Drawing.Point(478, 232);
            this.btnUnmount.Name = "btnUnmount";
            this.btnUnmount.Size = new System.Drawing.Size(242, 57);
            this.btnUnmount.TabIndex = 12;
            this.btnUnmount.TabStop = false;
            this.btnUnmount.Text = "Unmount";
            this.toolTipMain.SetToolTip(this.btnUnmount, "Unmount the remote drive");
            this.btnUnmount.UseVisualStyleBackColor = true;
            this.btnUnmount.Click += new System.EventHandler(this.btnUnmount_Click);
            // 
            // linklblDocument
            // 
            this.linklblDocument.AutoSize = true;
            this.linklblDocument.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.linklblDocument.Location = new System.Drawing.Point(592, 135);
            this.linklblDocument.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linklblDocument.Name = "linklblDocument";
            this.linklblDocument.Size = new System.Drawing.Size(129, 23);
            this.linklblDocument.TabIndex = 104;
            this.linklblDocument.TabStop = true;
            this.linklblDocument.Text = "Documentation";
            this.linklblDocument.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linklblDocument_LinkClicked);
            // 
            // checkBoxRAAS
            // 
            this.checkBoxRAAS.AutoSize = true;
            this.checkBoxRAAS.Location = new System.Drawing.Point(22, 315);
            this.checkBoxRAAS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxRAAS.Name = "checkBoxRAAS";
            this.checkBoxRAAS.Size = new System.Drawing.Size(236, 32);
            this.checkBoxRAAS.TabIndex = 105;
            this.checkBoxRAAS.Text = "Auto Mount at Startup";
            this.toolTipMain.SetToolTip(this.checkBoxRAAS, "Automatically mount when Windows starts");
            this.checkBoxRAAS.UseVisualStyleBackColor = true;
            this.checkBoxRAAS.CheckedChanged += new System.EventHandler(this.checkBoxRAAS_CheckedChanged);
            // 
            // notifyIconRclone
            // 
            this.notifyIconRclone.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIconRclone.BalloonTipText = "Mounted Successfully";
            this.notifyIconRclone.BalloonTipTitle = "Rclone Mount";
            this.notifyIconRclone.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconRclone.Icon")));
            this.notifyIconRclone.Text = "Rclone Mount";
            this.notifyIconRclone.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconRclone_MouseDoubleClick);
            // 
            // backgroundWorkerRclone
            // 
            this.backgroundWorkerRclone.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorkerRclone_DoWork);
            // 
            // btnConfig
            // 
            this.btnConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConfig.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.btnConfig.Location = new System.Drawing.Point(22, 232);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(162, 57);
            this.btnConfig.TabIndex = 106;
            this.btnConfig.TabStop = false;
            this.btnConfig.Text = "Config";
            this.toolTipMain.SetToolTip(this.btnConfig, "Open rclone config in terminal");
            this.btnConfig.UseVisualStyleBackColor = true;
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(345, 320);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(375, 30);
            this.lblStatus.TabIndex = 107;
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBoxDarkMode
            // 
            this.checkBoxDarkMode.AutoSize = true;
            this.checkBoxDarkMode.Location = new System.Drawing.Point(22, 360);
            this.checkBoxDarkMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.checkBoxDarkMode.Name = "checkBoxDarkMode";
            this.checkBoxDarkMode.Size = new System.Drawing.Size(136, 32);
            this.checkBoxDarkMode.TabIndex = 108;
            this.checkBoxDarkMode.Text = "Dark Mode";
            this.toolTipMain.SetToolTip(this.checkBoxDarkMode, "Toggle dark/light theme");
            this.checkBoxDarkMode.UseVisualStyleBackColor = true;
            this.checkBoxDarkMode.CheckedChanged += new System.EventHandler(this.checkBoxDarkMode_CheckedChanged);
            // 
            // toolTipMain
            // 
            this.toolTipMain.AutoPopDelay = 5000;
            this.toolTipMain.InitialDelay = 400;
            this.toolTipMain.ReshowDelay = 100;
            // 
            // panelStatus
            // 
            this.panelStatus.Location = new System.Drawing.Point(320, 324);
            this.panelStatus.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(18, 18);
            this.panelStatus.TabIndex = 109;
            // 
            // RcloneMount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(144F, 144F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(742, 405);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.checkBoxDarkMode);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.checkBoxRAAS);
            this.Controls.Add(this.linklblDocument);
            this.Controls.Add(this.btnUnmount);
            this.Controls.Add(this.txtDriveLetter);
            this.Controls.Add(this.lblDriveLetter);
            this.Controls.Add(this.txtRemoteName);
            this.Controls.Add(this.lblRemoteName);
            this.Controls.Add(this.comboBoxMount);
            this.Controls.Add(this.lblMountOptions);
            this.Controls.Add(this.btnMount);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblRcloneLocation);
            this.Controls.Add(this.txtRLocation);
            this.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.MaximizeBox = false;
            this.Name = "RcloneMount";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Rclone Mount";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RcloneMount_FormClosing);
            this.Load += new System.EventHandler(this.RcloneMount_Load);
            this.Resize += new System.EventHandler(this.RcloneMount_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRLocation;
        private System.Windows.Forms.Label lblRcloneLocation;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.OpenFileDialog openFileDialogRclone;
        private System.Windows.Forms.Button btnMount;
        private System.Windows.Forms.Label lblMountOptions;
        private System.Windows.Forms.ComboBox comboBoxMount;
        private System.Windows.Forms.Label lblRemoteName;
        private System.Windows.Forms.TextBox txtRemoteName;
        private System.Windows.Forms.Label lblDriveLetter;
        private System.Windows.Forms.ComboBox txtDriveLetter;
        private System.Windows.Forms.Button btnUnmount;
        private System.Windows.Forms.LinkLabel linklblDocument;
        private System.Windows.Forms.CheckBox checkBoxRAAS;
        private System.Windows.Forms.NotifyIcon notifyIconRclone;
        private System.ComponentModel.BackgroundWorker backgroundWorkerRclone;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.CheckBox checkBoxDarkMode;
        private System.Windows.Forms.ToolTip toolTipMain;
        private System.Windows.Forms.Panel panelStatus;
    }
}
