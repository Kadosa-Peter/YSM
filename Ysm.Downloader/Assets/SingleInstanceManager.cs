using System;
using System.Linq;
using Microsoft.VisualBasic.ApplicationServices;

// ReSharper disable once CheckNamespace
namespace Ysm.Downloader
{
    public sealed class SingleInstanceManager : WindowsFormsApplicationBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new SingleInstanceManager().Run(args);
        }

        public SingleInstanceManager()
        {
            IsSingleInstance = true;
        }

        public App App { get; private set; }

        protected override bool OnStartup(StartupEventArgs e)
        {
            App = new App();
            App.Run();
            return false;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            base.OnStartupNextInstance(eventArgs);
            App.Window.Activate();
            App.ProcessArgs(eventArgs.CommandLine.ToArray(), false);
        }
    }
}
