using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ysm.Core;

namespace Ysm.Downloader.Assets
{
    public static class UrlHelper
    {
        public static string GetUrl(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string url;

                    while ((url = sr.ReadLine()) != null)
                    {
                        if (url.StartsWith("URL="))
                        {
                            url = url.Replace("URL=", "");
                            url = Regex.Replace(url, "[\n\r\t]", "");

                            return url;
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex.Message);
            }

            return null;
        }

    }
}
