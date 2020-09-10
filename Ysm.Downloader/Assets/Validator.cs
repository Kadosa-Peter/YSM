using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using Ysm.Core;

namespace Ysm.Downloader.Assets
{
    // https://stackoverflow.com/questions/39777659/extract-the-video-id-from-youtube-url-in-net
    public class Validator
    {
        public static string ValidateVideoId(string id)
        {
            if (TryValidate(id))
            {
                return id; 
            }

            return TryParse(id);
        }

        private static string TryParse(string url)
        {
            try
            {
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                {
                    try
                    {
                        uri = new UriBuilder("http", url).Uri;
                    }
                    catch
                    {
                        // invalid url
                        return "";
                    }
                }

                string host = uri.Host;
                string[] youTubeHosts = { "www.youtube.com", "youtube.com", "youtu.be", "www.youtu.be" };
                if (!youTubeHosts.Contains(host))
                    return "";

                NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

                if (query.AllKeys.Contains("v"))
                {
                    return Regex.Match(query["v"], @"^[a-zA-Z0-9_-]{11}$").Value;
                }
                else if (query.AllKeys.Contains("u"))
                {
                    // some urls have something like "u=/watch?v=AAAAAAAAA16"
                    return Regex.Match(query["u"], @"/watch\?v=([a-zA-Z0-9_-]{11})").Groups[1].Value;
                }
                else
                {
                    // remove a trailing forward space
                    string last = uri.Segments.Last().Replace("/", "");
                    if (Regex.IsMatch(last, @"^v=[a-zA-Z0-9_-]{11}$"))
                        return last.Replace("v=", "");

                    string[] segments = uri.Segments;
                    if (segments.Length > 2 && segments[segments.Length - 2] != "v/" && segments[segments.Length - 2] != "watch/")
                        return "";

                    return Regex.Match(last, @"^[a-zA-Z0-9_-]{11}$").Value;
                }
            }
            catch (Exception e)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), e);
            }

            return null;
        }

        private static bool TryValidate(string videoId)
        {
            if (videoId.IsNull())
                return false;

            if (videoId.Length != 11)
                return false;

            if (videoId.StartsWith("http") || videoId.StartsWith("www"))
                return false;

            return !Regex.IsMatch(videoId, @"[^0-9a-zA-Z_\-]");
        }
    }
}
