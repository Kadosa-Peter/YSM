using System;
using System.IO;
using System.Reflection;
using IWshRuntimeLibrary;
using Ysm.Core;

namespace Ysm.Assets
{
    public static class Shortcut
    {
        public static void Create(string source, string target, string arguments, string icon, string description, string hotkey)
        {
            try
            {
                string shortcutLocation;

                if (Directory.Exists(target))
                {
                    shortcutLocation = Path.Combine(target, Path.GetFileNameWithoutExtension(source) + ".lnk");
                }
                else
                {
                    shortcutLocation = target;

                }

                WshShell shell = new WshShell();

                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = description; // The description of the shortcut

                if (System.IO.File.Exists(icon))
                    shortcut.IconLocation = icon; // The icon of the shortcut

                shortcut.TargetPath = source; // The path of the file that will launch when the shortcut is run

                shortcut.Arguments = arguments;

                shortcut.Save();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Shortcut).Assembly.FullName,
                    ClassName = typeof(Shortcut).FullName,
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

        public static bool IsTargetExist(string link)
        {
            if (System.IO.File.Exists(link))
            {
                IWshShell shell = new WshShell();
                if (shell.CreateShortcut(link) is IWshShortcut lnk)
                {
                    return System.IO.File.Exists(lnk.TargetPath);
                }
            }

            return false;
        }

        public static string GetTarget(string link)
        {
            if (System.IO.File.Exists(link))
            {
                IWshShell shell = new WshShell();
                if (shell.CreateShortcut(link) is IWshShortcut lnk)
                {
                    return lnk.TargetPath;
                }
            }

            return string.Empty;
        }
    }
}
