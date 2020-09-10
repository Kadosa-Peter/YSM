using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ysm.Core;

namespace Ysm.Downloader
{
    public class App : Application
    {
        public MainWindow Window { get; private set; }

        public App()
        {
            Current.Resources.BeginInit();

            Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Ysm.Controls;component/Themes/Generic.xaml", UriKind.RelativeOrAbsolute)
            });

            Current.Resources.EndInit();

            ShutdownMode = ShutdownMode.OnMainWindowClose;

            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Window = new MainWindow();

            Window.Show();

            ProcessArgs(e.Args, true);
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(MethodBase.GetCurrentMethod(), e.Exception);
        }

        public void ProcessArgs(string[] args, bool firstInstance)
        {
            if (args.Length == 1)
            {
                Window?.CreateDownload(args[0]);
            }
        }
    }
}
