using System;
using System.Configuration;
using System.Reflection;
using Ysm.Core;
using Ysm.Data;

namespace Ysm.Assets
{
    internal static class AppSettings
    {
        internal static void Read()
        {
            try
            {
                Configuration configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConfigurationSection appSettings = configFile.GetSection("appSettings");

                if (appSettings != null && appSettings.SectionInformation.IsProtected == false)
                {
                    appSettings.SectionInformation.ProtectSection("RsaProtectedConfigurationProvider");
                    appSettings.SectionInformation.ForceSave = true;
                    configFile.Save(ConfigurationSaveMode.Modified);
                }

                Kernel.Default.ClientId = ConfigurationManager.AppSettings["ClientId"].ConvertToSecureString();
                Kernel.Default.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ConvertToSecureString();
                Kernel.Default.EncryptionKey = ConfigurationManager.AppSettings["EncryptionKey"].ConvertToSecureString();
                Kernel.Default.ConnectionString = ConfigurationManager.AppSettings["ConnectionStrings"].ConvertToSecureString();
                Kernel.Default.IsBeta = ConfigurationManager.AppSettings["IsBeta"].ToBool();
                Kernel.Default.Beta = ConfigurationManager.AppSettings["Beta"].ToDateTime();
               

                AuthenticationService.Default.ClientId = ConfigurationManager.AppSettings["ClientId"].ConvertToSecureString();
                AuthenticationService.Default.ClientSecret = ConfigurationManager.AppSettings["ClientSecret"].ConvertToSecureString();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(AppSettings).Assembly.FullName,
                    ClassName = typeof(AppSettings).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }
    }
}
