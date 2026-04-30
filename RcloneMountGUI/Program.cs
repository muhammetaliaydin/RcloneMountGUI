using System;
using System.Windows.Forms;

namespace RcloneMountGUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// DPI awareness is handled via app.manifest and App.config
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RcloneMount());
        }
    }
}
