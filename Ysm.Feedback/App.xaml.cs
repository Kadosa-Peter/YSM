using System.Reflection;
using System.Windows.Threading;
using Ysm.Core;

namespace Ysm.Feedback
{
    public partial class App
    {
        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.Log(MethodBase.GetCurrentMethod(), e.Exception.Message);
        }
    }
}
