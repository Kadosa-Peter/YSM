using System;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;

namespace Ysm.Core.Update
{
    public static class LogFactory
    {
        public static string CreateLog(string versionString, string htmlLog)
        {
            UpdateLog log = new UpdateLog
            {
                VersionString = versionString,
                ReleaseDate = DateTime.Now,
                HtmlLog = htmlLog
            };

            string json = JsonConvert.SerializeObject(log);

            return json;
        }

        public static UpdateLog ReadLog(string json)
        {
            UpdateLog log = JsonConvert.DeserializeObject<UpdateLog>(json);

            return log;
        }

        public static void SaveLog(string versionString, string htmlLog, string path)
        {
            try
            {
                string json = CreateLog(versionString, htmlLog);

                using (StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8))
                {
                    writer.Write(json);
                }
            }
            catch (IOException ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(LogFactory).Assembly.FullName,
                    ClassName = typeof(LogFactory).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(LogFactory).Assembly.FullName,
                    ClassName = typeof(LogFactory).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }
        }
    }
}
