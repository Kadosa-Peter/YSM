using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Ysm.Core;

namespace Ysm
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");

            base.OnStartup(e);
        }

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(MethodBase.GetCurrentMethod(), e.Exception.Message);
            Logger.Log(MethodBase.GetCurrentMethod(), e.Exception.StackTrace);

            Logger.Save();
        }
    }
}
