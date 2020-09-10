using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Ysm.Core
{
    public static class Logger
    {
        private static readonly List<string> Logs;

        static Logger()
        {
            Logs = new List<string>();
        }

        public static void Log(MethodBase methodBase, string message)
        {
            string log = $"{methodBase.DeclaringType?.FullName}.{methodBase.Name} => {message}";

            Debug.WriteLine(log);
            Logs.Add(log);
        }

        public static void Log(MethodBase methodBase, Exception exception)
        {
            string log = $"{methodBase.DeclaringType?.FullName}.{methodBase.Name} => {exception}";

            Debug.WriteLine(log);
            Logs.Add(log);
        }

        public static void Log(string method, string message)
        {
            string log = $"{method} => {message}";

            Debug.WriteLine(log);
            Logs.Add(log);
        }

        public static void Log(string method, Exception exception)
        {
            string log = $"{method} => {exception}";

            Debug.WriteLine(log);
            Logs.Add(log);
        }

        public static void Log(string message)
        {
            Debug.WriteLine(message);
            Logs.Add(message);
        }

        public static void Log(Error message)
        {
            Debug.WriteLine(message);

            Logs.Add(message.ToString());
        }

        public static void Save()
        {
            if (Logs.Count > 0)
            {
                string file = Path.Combine(FileSystem.Logs, DateTime.Now.ToString("yyyy-M-dd HH-mm-ss"));
                file = file.InsertEnd(".txt");

                using (StreamWriter writer = new StreamWriter(file, false, Encoding.UTF8))
                {
                    foreach (string log in Logs)
                    {
                        writer.WriteLine(log);
                    }
                }

                CleanupLogs();
            }
        }

        private static void CleanupLogs()
        {
            DateTime date = DateTime.Now.AddDays(-3).Date;

            foreach (FileInfo file in new DirectoryInfo(FileSystem.Logs).GetFiles())
            {
                if (file.CreationTime < date)
                {
                    File.Delete(file.FullName);
                }
            }
        }
    }
}
