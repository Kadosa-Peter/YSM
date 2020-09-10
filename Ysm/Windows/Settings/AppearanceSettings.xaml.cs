using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Ysm.Assets;
using Ysm.Core;

namespace Ysm.Windows
{
    public partial class AppearanceSettings
    {
        private bool _isLoaded;

        public AppearanceSettings()
        {
            InitializeComponent();

            Loaded += (s, e) => _isLoaded = true;
        }

        private void CloseSystemTray_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
                cbMinimizeSystemTray.IsChecked = false;
        }

        private void MinimizeSystemTray_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isLoaded)
                cbCloseSystemTray.IsChecked = false;
        }

        private void StartOnSystemStartup_OnClick(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.StartOnSystemStartup)
            {
                CreateShortcut();
            }
            else
            {
                DeleteShortcut();
            }
        }

        private void CreateShortcut()
        {
            try
            {
                string startupFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                string target = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

                string source = Path.Combine(startupFolder, "Ysm.exe");

                string icon = Path.Combine(startupFolder, "Resources", "yt.ico");

                Shortcut.Create(source, target, "-on-system-startup", icon, null, null);
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = GetType().Assembly.FullName,
                    ClassName = GetType().FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }

        private void DeleteShortcut()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            path = Path.Combine(path, "Ysm.lnk");

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private void StartNotification_OnChecked(object sender, RoutedEventArgs e)
        {
            cbTaskbar.IsChecked = false;
        }

        private void StartTaskbar_OnChecked(object sender, RoutedEventArgs e)
        {
            cbNotification.IsChecked = false;
        }
    }
}
