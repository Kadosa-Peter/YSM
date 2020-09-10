using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using Chromium;
using Chromium.WebBrowser;
using Chromium.WebBrowser.Event;
using Ysm.Core;
using Ysm.Core.Update;

namespace Ysm.Update
{
    public partial class MainWindow
    {
        private string _fileName;

        private UpdateLog _log;
        private readonly ChromiumWebBrowser _webBrowser;

        public MainWindow()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;
            ShowInTaskbar = false;

            SetupFx();

            _webBrowser = new ChromiumWebBrowser();
            _webBrowser.Dock = DockStyle.Fill;

            FormHost.Child = _webBrowser;
        }

        private void SetupFx()
        {
            CfxRuntime.LibCefDirPath = Path.GetFullPath(@"cef");

            ChromiumWebBrowser.OnBeforeCfxInitialize += ChromiumWebBrowser_OnBeforeCfxInitialize;
            ChromiumWebBrowser.Initialize();
        }

        private void ChromiumWebBrowser_OnBeforeCfxInitialize(OnBeforeCfxInitializeEventArgs e)
        {
            e.Settings.ResourcesDirPath = Path.GetFullPath(@"cef\Resources");
            e.Settings.LocalesDirPath = Path.GetFullPath(@"cef\Resources\locales");
            e.Settings.CachePath = Path.GetFullPath(@"cef\Resources\cache");
            e.Settings.FrameworkDirPath = Path.GetFullPath(@"cef\");

            e.Settings.LogSeverity = CfxLogSeverity.Disable;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _log = CheckUpdate.GetUpdateLog();

            if (_log != null)
            {
                Visibility = Visibility.Visible;
                ShowInTaskbar = true;

                string currentVersion = GetCurrentVersion();

                _fileName = $"Ysm-{_log.VersionString}.exe";

                txtCurrentVersion.Text = currentVersion;
                txtNewVersion.Text = _log.VersionString;

                _webBrowser.LoadString(_log.HtmlLog, " http://localhost");
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private string GetCurrentVersion()
        {
            if (DebugMode.IsDebugMode)
            {
                return "0.8.0";
            }

            string path = "Ermirage.exe";

            if (File.Exists(path))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);
                string versionString = versionInfo.ProductVersion;

                return versionString.Substring(0, 5);
            }

            return string.Empty;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Download_Click(object sender, RoutedEventArgs e)
        {
            ProgressWindow window = new ProgressWindow(false, _fileName);
            window.Show();

            Close();
        }

        private void Instal_Click(object sender, RoutedEventArgs e)
        {
            ProgressWindow window = new ProgressWindow(true, _fileName);
            window.Show();

            Close();
        }

        private void Skip_Click(object sender, RoutedEventArgs e)
        {
            string path = FileSystem.Update;

            using (StreamWriter writer = new StreamWriter(path, true, Encoding.UTF8))
            {
                writer.WriteLine(_log.VersionString);
            }

            Close();
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            CfxRuntime.Shutdown();
        }
    }
}
