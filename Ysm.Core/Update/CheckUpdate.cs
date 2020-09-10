using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Ysm.Core.Update
{
    public class CheckUpdate
    {
        public static UpdateLog GetUpdateLog()
        {
            try
            {
                UpdateLog log = GetVersion();

                VersionInfo newestVersion = new VersionInfo(log.VersionString);
                VersionInfo currentVersion;

                List<string> skipVersions = GetSkipVersions();

                if (DebugMode.IsDebugMode)
                {
                    currentVersion = new VersionInfo("0.8.0");
                }
                else
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo("Ysm.exe");
                    currentVersion = new VersionInfo(fileVersionInfo);
                }

                if (skipVersions.Contains(newestVersion.ToString()) == false)
                {
                    if (newestVersion > currentVersion)
                    {
                        return log;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(CheckUpdate).Assembly.FullName,
                    ClassName = typeof(CheckUpdate).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }

        public static bool Check()
        {
            try
            {
                UpdateLog log = GetVersion();

                VersionInfo newestVersion = new VersionInfo(log.VersionString);
                VersionInfo currentVersion;

                List<string> skipVersions = GetSkipVersions();

                if (DebugMode.IsDebugMode)
                {
                    currentVersion = new VersionInfo("0.8.0");
                }
                else
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo("Ysm.exe");
                    currentVersion = new VersionInfo(fileVersionInfo);
                }

                if (skipVersions.Contains(newestVersion.ToString()) == false)
                {
                    if (newestVersion > currentVersion)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(CheckUpdate).Assembly.FullName,
                    ClassName = typeof(CheckUpdate).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return false;
        }

        private static List<string> GetSkipVersions()
        {
            List<string> versionInfos = new List<string>();

            string path = FileSystem.Update;

            if (File.Exists(path))
            {
                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        VersionInfo info = new VersionInfo(line);

                        versionInfos.Add(info.ToString());
                    }
                }
            }

            return versionInfos;
        }

        private static UpdateLog GetVersion()
        {
            string json;

            using (WebClient client = new WebClient())
            {
                json = client.DownloadString(new Uri("http://yosuma.com/update/v1/update.json"));
            }

            if (json.NotNull())
            {
                return JsonConvert.DeserializeObject<UpdateLog>(json);
            }

            return null;
        }
    }
}
