using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YoutubeExplode;
using Ysm.Core;

namespace Ysm.Data
{
    internal static class ExplodeDownloader
    {
        public static List<Video> DownladVideos(string channel, bool downloadAll)
        {
            if (channel.NotNull())
            {
                IEnumerable<YoutubeExplode.Models.Video> playlistVideos = DownloadPlaylistAsync(channel, downloadAll).Result;

                if (playlistVideos != null)
                {
                    if (downloadAll)
                    {
                        return CreateVideos(playlistVideos, channel).ToList();

                    }
                    else
                    {
                        return CreateVideos(playlistVideos, channel).Take(15).ToList();
                    }
                }
            }

            return null;
        }

        private static async Task<IEnumerable<YoutubeExplode.Models.Video>> DownloadPlaylistAsync(string channel, bool downloadAll)
        {
            try
            {
                int page = downloadAll ? 50 : 1;

                YoutubeClient client = new YoutubeClient();
                IReadOnlyList<YoutubeExplode.Models.Video> result = await client.GetChannelUploadsAsync(channel, page);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ExplodeDownloader Error: {channel}");

                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ExplodeDownloader).Assembly.FullName,
                    ClassName = typeof(ExplodeDownloader).FullName,
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

        private static List<Video> CreateVideos(IEnumerable<YoutubeExplode.Models.Video> playlistVideos, string channel)
        {
            List<Video> videos = new List<Video>();

            try
            {
                foreach (YoutubeExplode.Models.Video v in playlistVideos)
                {
                    Video video = new Video();

                    video.VideoId = v.Id;

                    video.ChannelId = channel;

                    video.Title = v.Title;

                    //video.Duration = v.Duration;

                    video.Published = v.UploadDate.DateTime;

                    video.ThumbnailUrl = $"http://img.youtube.com/vi/{v.Id}/mqdefault.jpg";

                    video.Link = $"https://www.youtube.com/watch?v={v.Id}";

                    videos.Add(video);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ExplodeDownloader).Assembly.FullName,
                    ClassName = typeof(ExplodeDownloader).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace
                };

                Logger.Log(error);

                #endregion
            }

            return videos;
        }
    }
}
