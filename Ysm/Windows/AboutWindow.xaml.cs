using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ysm.Assets;
using Ysm.Core;
using Ysm.Core.Update;

namespace Ysm.Windows
{
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();

            DisplayUpdate();

            DisplayVersion();

            DisplayLicense();
        }

        private void DisplayLicense()
        {
            if (Settings.Default.Validated.IsNull() ||
                StringEncryption.Decrypt(Settings.Default.Validated, Kernel.Default.EncryptionKey.ConvertToString()) !=
                "Validated")
            {
                LicencLabel.Text = "Unregistered evaluation version.";
            }
            else
            {
                LicencLabel.Text = "Registered version.";
            }
        }

        private void DisplayUpdate()
        {
            Task<bool> task = Task.Run(() => CheckUpdate.Check());

            task.ContinueWith(t =>
            {
                if (t.Result)
                {
                    UpdateLabel.Text = Properties.Resources.About_Update2;
                    UpdateButton.IsEnabled = true;
                }
                else
                {
                    UpdateLabel.Text = Properties.Resources.About_Update1;
                }

            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void DisplayVersion()
        {
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo("Ysm.exe");
            VersionInfo versionInfo = new VersionInfo(fileVersionInfo);

            VersionLabel.Text = $"Version: {versionInfo}";
        }

        private void Website_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Kernel.Default.Website);
            }
            catch (Exception exception)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
            }
        }

        private void Faq_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Kernel.Default.Faq);
            }
            catch (Exception exception)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
            }
        }

        private void UserGuide_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start(Kernel.Default.UserGuide);
            }
            catch (Exception exception)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("Ysm.Update.exe");

                Close();
            }
            catch (Exception exception)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), exception.Message);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AboutWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Header_OnMouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
