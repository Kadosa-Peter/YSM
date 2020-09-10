using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ysm.Core
{
    public static class Files
    {
        public static string ShortenFilePath(string path)
        {
            // The fully qualified file name must be less than 260 characters.
            // Directory + File + Extension(optional).

            if (path.Length < 260) return path;

            try
            {
                string directory = path.Remove(path.LastIndexOfAny(new[] { '\\' }));
                string file = path.Substring(path.LastIndexOfAny(new[] { '\\' }) + 1);

                string extension = Path.GetExtension(path);

                int length = directory.Length + extension.Length;

                length = 258 - length;

                file = file.Remove(length);

                return $@"{directory}\{file}{extension}";
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Files).Assembly.FullName,
                    ClassName = typeof(Files).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return string.Empty;
        }

        public static string CleanFileName(string name)
        {
            name = Path.GetInvalidFileNameChars().Aggregate(name, (current, character) => current.Replace(character.ToString(CultureInfo.InvariantCulture), string.Empty));
            Regex regex = new Regex(@"[ ]{2,}", RegexOptions.None);
            name = regex.Replace(name, @" ");
            return name;
        }

        public static string VerifyFilePath(string path)
        {
            if (File.Exists(path))
            {
                int number = 1;

                string directory = Path.GetDirectoryName(path);
                string name = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);

                while (File.Exists($@"{directory}\{name} ({number}){extension}"))
                {
                    number++;
                }

                path = $@"{directory}\{name} ({number}){extension}";
            }

            return path;
        }

        public static string VerifyFolderPath(string path)
        {
            if (Directory.Exists(path))
            {
                int number = 1;

                while (Directory.Exists($"{path} ({number})"))
                {
                    number++;
                }

                path = $@"{path} ({number})";
            }

            return path;
        }

    }
}
