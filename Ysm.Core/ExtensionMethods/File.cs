using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace Ysm.Core
{
    public static partial class ExtensionMethods
    {
        public static string ReadText(this string path)
        {
            if (File.Exists(path))
            {
                string text;

                using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
                {
                    text = reader.ReadToEnd();
                }

                return text;
            }

            return string.Empty;
        }

        public static void WriteText(this string path, string text, bool append = false)
        {
            using (StreamWriter writer = new StreamWriter(path, append, Encoding.UTF8))
            {
                writer.Write(text);
            }
        }

        public static BitmapImage ToImage(this string @this)
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@this, UriKind.Absolute);
                image.EndInit();

                return image;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ExtensionMethods).Assembly.FullName,
                    ClassName = typeof(ExtensionMethods).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }

        public static FileInfo Find(this IEnumerable<FileInfo> enumerable, string fileName)
        {
            foreach (FileInfo fileInfo in enumerable)
            {
                string name = Path.GetFileNameWithoutExtension(fileInfo.Name);

                if (name == fileName)
                {
                    return fileInfo;
                }
            }

            return null;
        }
    }
}
