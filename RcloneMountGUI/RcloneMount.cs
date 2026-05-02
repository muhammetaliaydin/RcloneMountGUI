using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RcloneMountGUI
{
    public partial class RcloneMount : Form
    {
        public RcloneMount()
        {
            InitializeComponent();
            TrayMenuContext();

            // Default selections
            populateDriveLetters();
            comboBoxMount.SelectedIndex = 2;

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false);

                if (key != null && key.GetValue("RcloneMountGUI") != null)
                {
                    string regValue = key.GetValue("RcloneMountGUI").ToString();
                    if (regValue == "\"" + Application.ExecutablePath + "\"" + " -tray")
                    {
                        checkBoxRAAS.Checked = true;
                    }
                }
            }
            catch { }

            loadSettings();
            loadTheme();
            updateStatusLabel(false);
            updateButtonStates(false);
        }

        string[] args = Environment.GetCommandLineArgs();

        #region Variables

        string rclonePath, rcloneFileName;
        bool isDarkMode;

        // GitHub repository for updates
        const string GitHubOwner = "muhammetaliaydin";
        const string GitHubRepo = "RcloneMountGUI";
        const string CurrentVersion = "2.0.1";

        // Theme colors
        static readonly Color DarkBackground = Color.FromArgb(30, 30, 46);
        static readonly Color DarkSurface = Color.FromArgb(45, 45, 65);
        static readonly Color DarkForeground = Color.FromArgb(205, 214, 244);
        static readonly Color DarkAccent = Color.FromArgb(137, 180, 250);
        static readonly Color DarkBorder = Color.FromArgb(88, 91, 112);

        static readonly Color LightBackground = Color.FromArgb(239, 241, 245);
        static readonly Color LightSurface = Color.White;
        static readonly Color LightForeground = Color.FromArgb(30, 30, 46);
        static readonly Color LightAccent = Color.FromArgb(30, 102, 245);
        static readonly Color LightBorder = Color.FromArgb(172, 176, 190);

        static readonly Color MountedColor = Color.FromArgb(64, 190, 100);
        static readonly Color UnmountedColor = Color.FromArgb(180, 180, 190);

        #endregion

        #region Methods

        private void loadSettings()
        {
            // Loads settings from registry

            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RcloneMountGUI", true);

            if (key != null)
            {
                try
                {
                    if (!String.IsNullOrEmpty(key.GetValue("RcloneLocation")?.ToString()))
                    {
                        txtRLocation.Text = key.GetValue("RcloneLocation").ToString();

                        rclonePath = Path.GetDirectoryName(txtRLocation.Text);
                        rcloneFileName = Path.GetFileNameWithoutExtension(txtRLocation.Text);
                    }
                }
                catch { }

                try
                {
                    if (!String.IsNullOrEmpty(key.GetValue("RemoteName")?.ToString()))
                    {
                        txtRemoteName.Text = key.GetValue("RemoteName").ToString();
                    }
                }
                catch { }

                try
                {
                    string savedLetter = key.GetValue("DriveLetter")?.ToString();
                    if (!String.IsNullOrEmpty(savedLetter))
                    {
                        int index = txtDriveLetter.FindStringExact(savedLetter);
                        if (index >= 0)
                            txtDriveLetter.SelectedIndex = index;
                    }
                }
                catch { }

                try
                {
                    string savedOption = key.GetValue("MountOptions")?.ToString();
                    if (!String.IsNullOrEmpty(savedOption))
                    {
                        int index = comboBoxMount.FindStringExact(savedOption);
                        if (index >= 0)
                            comboBoxMount.SelectedIndex = index;
                    }
                }
                catch { }
            }
            else
                Registry.CurrentUser.CreateSubKey("SOFTWARE\\RcloneMountGUI");
        }

        private void populateDriveLetters()
        {
            // Populates the drive letter combo with available (unused) letters A-Z

            txtDriveLetter.Items.Clear();

            string[] usedDrives = Environment.GetLogicalDrives();

            for (char letter = 'A'; letter <= 'Z'; letter++)
            {
                string drive = letter + ":\\";
                bool inUse = false;

                foreach (string used in usedDrives)
                {
                    if (used.Equals(drive, StringComparison.OrdinalIgnoreCase))
                    {
                        inUse = true;
                        break;
                    }
                }

                if (!inUse)
                    txtDriveLetter.Items.Add(letter.ToString());
            }

            // Default to last available letter
            if (txtDriveLetter.Items.Count > 0)
                txtDriveLetter.SelectedIndex = txtDriveLetter.Items.Count - 1;
        }

        private void loadTheme()
        {
            // Loads theme preference from registry, defaults to system setting

            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RcloneMountGUI", false);

            if (key != null && key.GetValue("DarkMode") != null)
            {
                isDarkMode = key.GetValue("DarkMode").ToString() == "1";
            }
            else
            {
                // Detect Windows system theme
                isDarkMode = isWindowsDarkMode();
            }

            checkBoxDarkMode.Checked = isDarkMode;
            applyTheme();
        }

        private bool isWindowsDarkMode()
        {
            // Checks if Windows is using dark mode
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", false);

                if (key != null)
                {
                    object value = key.GetValue("AppsUseLightTheme");
                    if (value != null)
                        return (int)value == 0;
                }
            }
            catch { }

            return false;
        }

        private void applyTheme()
        {
            // Applies dark or light theme to all controls

            Color bg = isDarkMode ? DarkBackground : LightBackground;
            Color surface = isDarkMode ? DarkSurface : LightSurface;
            Color fg = isDarkMode ? DarkForeground : LightForeground;
            Color accent = isDarkMode ? DarkAccent : LightAccent;
            Color border = isDarkMode ? DarkBorder : LightBorder;

            this.BackColor = bg;
            this.ForeColor = fg;

            // Text boxes
            txtRLocation.BackColor = surface;
            txtRLocation.ForeColor = fg;
            txtRLocation.BorderStyle = BorderStyle.FixedSingle;

            txtRemoteName.BackColor = surface;
            txtRemoteName.ForeColor = fg;
            txtRemoteName.BorderStyle = BorderStyle.FixedSingle;

            txtDriveLetter.BackColor = surface;
            txtDriveLetter.ForeColor = fg;

            // Combo box
            comboBoxMount.BackColor = surface;
            comboBoxMount.ForeColor = fg;

            // Buttons
            applyButtonTheme(btnSelect, fg, bg, border);
            applyButtonTheme(btnConfig, fg, bg, border);
            applyButtonTheme(btnMount, accent, isDarkMode ? DarkBackground : Color.White, accent);
            applyButtonTheme(btnUnmount, fg, bg, border);

            // Labels
            lblRcloneLocation.ForeColor = fg;
            lblRemoteName.ForeColor = fg;
            lblDriveLetter.ForeColor = fg;
            lblMountOptions.ForeColor = fg;
            lblStatus.ForeColor = fg;

            // Link label
            linklblDocument.LinkColor = accent;
            linklblDocument.ActiveLinkColor = accent;

            // Checkboxes
            checkBoxRAAS.ForeColor = fg;
            checkBoxDarkMode.ForeColor = fg;
        }

        private void applyButtonTheme(Button btn, Color fg, Color bg, Color border)
        {
            btn.ForeColor = fg;
            btn.BackColor = bg;
            btn.FlatAppearance.BorderColor = border;
            btn.FlatAppearance.MouseOverBackColor = isDarkMode ? DarkSurface : Color.FromArgb(220, 224, 232);
        }

        private void updateStatusLabel(bool mounted)
        {
            // Updates the mount status indicator

            if (mounted)
            {
                lblStatus.Text = "Mounted";
                lblStatus.ForeColor = MountedColor;
                panelStatus.BackColor = MountedColor;
            }
            else
            {
                lblStatus.Text = "Not Mounted";
                lblStatus.ForeColor = UnmountedColor;
                panelStatus.BackColor = UnmountedColor;
            }

            // Position the dot right next to the text
            int textWidth = TextRenderer.MeasureText(lblStatus.Text, lblStatus.Font).Width;
            int dotX = lblStatus.Right - textWidth - panelStatus.Width - 4;
            int dotY = lblStatus.Top + (lblStatus.Height - panelStatus.Height) / 2;
            panelStatus.Location = new System.Drawing.Point(dotX, dotY);
        }

        private void updateButtonStates(bool mounted)
        {
            // Enables/disables mount and unmount buttons based on current state
            btnMount.Enabled = !mounted;
            btnUnmount.Enabled = mounted;
        }

        private bool isRcloneRunning()
        {
            // Checks if rclone process is running

            if (String.IsNullOrEmpty(rcloneFileName))
                return false;

            Process[] processes = Process.GetProcessesByName(rcloneFileName);
            return processes.Length > 0;
        }

        private void runTray()
        {
            // Minimizes to tray
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIconRclone.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        private void RcloneStart()
        {
            // Starts the mounting process
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/C " + rcloneFileName + " mount " + txtRemoteName.Text + ": " + txtDriveLetter.Text + ": " + comboBoxMount.Text;
            process.StartInfo.WorkingDirectory = rclonePath;
            process.Start();

            System.Threading.Thread.Sleep(1000);

            // Check if mounted
            if (isRcloneRunning())
            {
                updateStatusLabel(true);
                updateButtonStates(true);

                if (this.WindowState != FormWindowState.Minimized)
                {
                    MessageBox.Show("Mounted successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                updateStatusLabel(false);
                updateButtonStates(false);

                if (this.WindowState != FormWindowState.Minimized)
                {
                    MessageBox.Show("Mount failed. Check your settings.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RcloneKill(bool silent = false)
        {
            // Kills the rclone process

            if (String.IsNullOrEmpty(rcloneFileName))
                return;

            Process[] processes = Process.GetProcessesByName(rcloneFileName);

            if (processes.Length > 0)
            {
                foreach (Process process in processes)
                {
                    try { process.Kill(); }
                    catch { }
                }

                updateStatusLabel(false);
                updateButtonStates(false);

                if (!silent)
                    MessageBox.Show("Unmounted successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (!silent)
            {
                MessageBox.Show("No mounted drive found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void TrayMenuContext()
        {
            this.notifyIconRclone.ContextMenuStrip = new ContextMenuStrip();
            this.notifyIconRclone.ContextMenuStrip.Items.Add("Show", null, this.Show_Click);
            this.notifyIconRclone.ContextMenuStrip.Items.Add("Exit", null, this.Exit_Click);
        }

        private void checkForUpdate()
        {
            // Checks GitHub releases for a newer version

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RcloneMountGUI");
                    client.Timeout = TimeSpan.FromSeconds(10);

                    string url = "https://api.github.com/repos/" + GitHubOwner + "/" + GitHubRepo + "/releases/latest";
                    string response = client.GetStringAsync(url).Result;

                    // Simple JSON parsing for tag_name
                    string latestVersion = parseJsonValue(response, "tag_name");

                    if (!String.IsNullOrEmpty(latestVersion))
                    {
                        // Remove 'v' prefix if present
                        latestVersion = latestVersion.TrimStart('v');

                        if (isNewerVersion(latestVersion, CurrentVersion))
                        {
                            // Find download URL for the exe asset
                            string downloadUrl = parseDownloadUrl(response);

                            if (MessageBox.Show(
                                "A new version (v" + latestVersion + ") is available.\nCurrent version: v" + CurrentVersion + "\n\nWould you like to update?",
                                "Rclone Mount Update",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information) == DialogResult.Yes)
                            {
                                if (!String.IsNullOrEmpty(downloadUrl))
                                {
                                    updateApp(downloadUrl);
                                    Application.Restart();
                                }
                                else
                                {
                                    // Fallback: open releases page in browser
                                    Process.Start("https://github.com/" + GitHubOwner + "/" + GitHubRepo + "/releases/latest");
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        private bool isNewerVersion(string latest, string current)
        {
            // Compares version strings (e.g., "2.1.0" > "2.0.0")
            try
            {
                Version latestVer = new Version(latest);
                Version currentVer = new Version(current);
                return latestVer > currentVer;
            }
            catch
            {
                return false;
            }
        }

        private string parseJsonValue(string json, string key)
        {
            // Simple JSON value parser (avoids adding Newtonsoft dependency)
            string pattern = "\"" + key + "\"\\s*:\\s*\"([^\"]+)\"";
            Match match = Regex.Match(json, pattern);
            return match.Success ? match.Groups[1].Value : null;
        }

        private string parseDownloadUrl(string json)
        {
            // Finds the .exe download URL from GitHub release assets
            string pattern = "\"browser_download_url\"\\s*:\\s*\"([^\"]+\\.exe)\"";
            Match match = Regex.Match(json, pattern);
            return match.Success ? match.Groups[1].Value : null;
        }

        public void updateApp(string downloadUrl)
        {
            // Downloads and replaces the application

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "RcloneMountGUI");

                    string appPath = Application.ExecutablePath;
                    string appDir = Path.GetDirectoryName(appPath);
                    string appName = Path.GetFileName(appPath);
                    string tempPath = Path.Combine(Path.GetTempPath(), appName);
                    string backupPath = appPath + ".bak";

                    // Download new version to temp
                    byte[] data = client.GetByteArrayAsync(downloadUrl).Result;
                    File.WriteAllBytes(tempPath, data);

                    // Backup current and replace
                    if (File.Exists(backupPath))
                        File.Delete(backupPath);

                    File.Move(appPath, backupPath);
                    File.Move(tempPath, appPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Update failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void clearOldVersion()
        {
            // Cleans up backup files from previous update
            string appPath = Application.ExecutablePath;
            string backupPath = appPath + ".bak";

            try
            {
                if (File.Exists(backupPath))
                    File.Delete(backupPath);
            }
            catch { }
        }

        #endregion

        #region Form tools

        private void RcloneMount_Load(object sender, EventArgs e)
        {
            backgroundWorkerRclone.RunWorkerAsync();

            // Starts the application according to the argument
            if (args.Length > 1)
            {
                if (!String.IsNullOrEmpty(args[1]))
                {
                    if (args[1] == "-tray")
                    {
                        this.WindowState = FormWindowState.Minimized;

                        runTray();
                        RcloneStart();
                    }
                }
            }
        }

        private void backgroundWorkerRclone_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            clearOldVersion();

            System.Threading.Thread.Sleep(2500);

            // If there is a new version of the application, it will ask for an update
            if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                checkForUpdate();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            // Saves requirement information
            if (openFileDialogRclone.ShowDialog() == DialogResult.OK)
            {
                txtRLocation.Text = openFileDialogRclone.FileName;
                rclonePath = Path.GetDirectoryName(openFileDialogRclone.FileName);
                rcloneFileName = Path.GetFileNameWithoutExtension(openFileDialogRclone.SafeFileName);
            }
        }

        private void btnMount_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRLocation.Text) || String.IsNullOrEmpty(txtRemoteName.Text) || txtDriveLetter.SelectedItem == null)
            {
                MessageBox.Show("Please fill in the required information.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Saves settings
                RegistryKey key;
                key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RcloneMountGUI", true);

                key.SetValue("RcloneLocation", txtRLocation.Text);
                key.SetValue("RemoteName", txtRemoteName.Text);
                key.SetValue("DriveLetter", txtDriveLetter.Text);
                key.SetValue("MountOptions", comboBoxMount.Text);

                RcloneStart();
            }
        }

        private void btnUnmount_Click(object sender, EventArgs e)
        {
            RcloneKill();
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtRLocation.Text))
            {
                MessageBox.Show("Please fill the 'Rclone Location'.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // Opens rclone config settings

                Hide();

                rclonePath = Path.GetDirectoryName(txtRLocation.Text);
                rcloneFileName = Path.GetFileNameWithoutExtension(txtRLocation.Text);

                Process process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C " + rcloneFileName + " config";
                process.StartInfo.WorkingDirectory = rclonePath;
                process.Start();
                process.WaitForExit();

                Show();
            }
        }

        private void checkBoxRAAS_CheckedChanged(object sender, EventArgs e)
        {
            // Run automatically at startup settings
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                if (checkBoxRAAS.Checked)
                {
                    key.SetValue("RcloneMountGUI", "\"" + Application.ExecutablePath + "\"" + " -tray");
                }
                else
                {
                    key.DeleteValue("RcloneMountGUI", false);
                }
            }
            catch { }
        }

        private void checkBoxDarkMode_CheckedChanged(object sender, EventArgs e)
        {
            // Saves theme preference and applies it
            isDarkMode = checkBoxDarkMode.Checked;
            applyTheme();

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\RcloneMountGUI", true);
                if (key != null)
                    key.SetValue("DarkMode", isDarkMode ? "1" : "0");
            }
            catch { }
        }

        private void linklblDocument_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://rclone.org/commands/rclone_mount/#vfs-file-caching");
        }

        private void RcloneMount_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIconRclone.Visible = true;
            }
        }

        private void notifyIconRclone_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIconRclone.Visible = false;

            // Refresh status when restoring from tray
            bool running = isRcloneRunning();
            updateStatusLabel(running);
            updateButtonStates(running);
        }

        private void Show_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notifyIconRclone.Visible = false;
            bool running = isRcloneRunning();
            updateStatusLabel(running);
            updateButtonStates(running);
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RcloneMount_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ask for confirmation if a remote is currently mounted
            if (isRcloneRunning())
            {
                if (MessageBox.Show("A remote drive is currently mounted.\nDo you want to unmount and exit?",
                    "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            RcloneKill(true);
        }

        #endregion
    }
}