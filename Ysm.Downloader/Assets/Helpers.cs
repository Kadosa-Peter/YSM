using System;
using System.Reflection;
using Ysm.Core;

namespace Ysm.Downloader.Assets
{
    public static class Helpers
    {
        public static double ConvertBytesToMegabytes(long bytes)
        {
            return bytes / 1024f / 1024f;
        }

        //https://www.somacon.com/p576.php
        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.### XB", e.g. "4.2 KB" or "1.434 GB"
        public static string GetBytesReadable(long i)
        {
            // Get absolute value
            long absolute_i = i < 0 ? -i : i;
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = i >> 50;
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = i >> 40;
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = i >> 30;
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = i >> 20;
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = i >> 10;
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = readable / 1024;

            if (suffix != "MB")
            {
                readable = Math.Round(readable, 2);
            }
            else
            {
                readable = Math.Round(readable, 0);
            }

            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        public static string GetPlaylistId(string url)
        {
            try
            {
                string id = url;
                id = id.Substring(id.IndexOf("=", StringComparison.Ordinal) + 1);
                if (id.Contains("&"))
                {
                    id = id.Remove(id.IndexOf("&", StringComparison.Ordinal));
                }

                return id;

            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Helpers).Assembly.FullName,
                    ClassName = typeof(Helpers).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }

        public static string GetVideoIdFromMixedUrl(string url)
        {
            try
            {
                string id = url;

                id = id.Substring(id.IndexOf("=", StringComparison.Ordinal) + 1);

                id = id.Remove(11);

                return id;

            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Helpers).Assembly.FullName,
                    ClassName = typeof(Helpers).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }

        public static string GetPlaylistIdFromMixedUrl(string url)
        {
            try
            {
                string id = url;

                id = id.Substring(id.IndexOf("list=", StringComparison.Ordinal) + 5);

                if (id.Contains("&"))
                {
                    id = id.Remove(id.IndexOf("&", StringComparison.Ordinal));
                }

                return id;

            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(Helpers).Assembly.FullName,
                    ClassName = typeof(Helpers).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }

            return null;
        }
    }
}
