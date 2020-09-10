using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class FeedDownloader
    {

        internal static List<Video> DownladFeedAsync(string channel)
        {
            if (channel.NotNull())
            {
                string feedUrl = UrlHelper.GetFeedUrl(channel);

                XmlDocument document = Task.Run(async () => await DownloadXmlAsync(feedUrl)).Result;

                if (document != null)
                {
                    List<Video> videos = ParseXml(document);

                    return videos;
                }
            }

            return null;
        }

        private static async Task<XmlDocument> DownloadXmlAsync(string feedUrl)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync(feedUrl))
                using (HttpContent content = response.Content)
                {
                    string result = await content.ReadAsStringAsync();
                    xmlDocument.LoadXml(result);
                }

                return xmlDocument;
            }
            catch (WebException ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(FeedDownloader).Assembly.FullName,
                    ClassName = typeof(FeedDownloader).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion

                Debug.WriteLine($"{ex.Status}: {feedUrl}");
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(FeedDownloader).Assembly.FullName,
                    ClassName = typeof(FeedDownloader).FullName,
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

        public static List<Video> ParseXml(XmlDocument xmlDocument)
        {
            List<Video> videos = new List<Video>();

            XmlNodeList entries = xmlDocument.GetElementsByTagName("entry");

            foreach (XmlNode node in entries)
            {
                try
                {
                    Video video = new Video();

                    video.VideoId = node["yt:videoId"]?.InnerText;

                    video.ChannelId = node["yt:channelId"]?.InnerText;

                    video.Title = node["title"]?.InnerText;

                    video.Link = node["link"]?.GetAttribute("href");

                    //video.ThumbnailUrl = node["media:group"]?["media:thumbnail"]?.GetAttribute("url");
                    video.ThumbnailUrl = $"http://img.youtube.com/vi/{video.VideoId}/mqdefault.jpg";

                    //video.Duration = new TimeSpan(0);

                    string published = node["published"]?.InnerText;
                    if (published.NotNull()) video.Published = DateTime.Parse(published);

                    videos.Add(video);
                }
                catch (Exception ex)
                {
                    #region error

                    Error error = new Error
                    {
                        AssemblyName = typeof(FeedDownloader).Assembly.FullName,
                        ClassName = typeof(FeedDownloader).FullName,
                        MethodName = MethodBase.GetCurrentMethod().Name,
                        ExceptionType = ex.GetType().ToString(),
                        Message = ex.Message,
                        Trace = ex.StackTrace
                    };

                    Logger.Log(error);

                    #endregion
                }

            }

            return videos;
        }
    }
}