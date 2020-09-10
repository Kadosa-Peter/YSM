using System;
using System.IO;
using System.Reflection;
using Chromium;
using Chromium.WebBrowser;
using Ysm.Core;

namespace Ysm.Controls
{
    internal static class ChromiumFx
    {
        public static void Setup()
        {
            try
            {
                CfxRuntime.LibCefDirPath = Path.GetFullPath("cef");

                ChromiumWebBrowser.OnBeforeCommandLineProcessing += e =>
                {
                    e.CommandLine.AppendSwitch("--disable-web-security");
                };

                ChromiumWebBrowser.OnBeforeCfxInitialize += e =>
                {
                    e.Settings.ResourcesDirPath = Path.GetFullPath(@"cef\Resources");
                    e.Settings.LocalesDirPath = Path.GetFullPath(@"cef\Resources\locales");
                    e.Settings.CachePath = Path.GetFullPath(@"cef\Resources\cache");
                    e.Settings.FrameworkDirPath = Path.GetFullPath(@"cef\");

                    e.Settings.LogSeverity = CfxLogSeverity.Disable;
                };

                ChromiumWebBrowser.Initialize();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ChromiumFx).Assembly.FullName,
                    ClassName = typeof(ChromiumFx).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
           
        }
    }
}
