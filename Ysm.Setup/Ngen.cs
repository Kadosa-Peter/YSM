using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Ysm.Setup
{
    public class Ngen
    {
        public static void Install()
        {
            string ngen = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");
            string app = GetApp();

            if (File.Exists(ngen))
            {
                Process process = new Process();

                process.StartInfo.FileName = Path.GetFullPath(ngen);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = $"install \"{app}\"";
                process.Start();
            }
        }

        public static void UnInstall()
        {
            string ngen = Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "ngen.exe");
            string app = GetApp();

            if (File.Exists(ngen))
            {
                Process process = new Process();

                process.StartInfo.FileName = Path.GetFullPath(ngen);
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.Arguments = $"uninstall \"{app}\"";
                process.Start();
            }
        }

        private static string GetApp()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            path = Path.Combine(path, "Ysm.exe");
            return path;
        }

    
    }
}
