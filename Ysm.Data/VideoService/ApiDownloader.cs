using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Google.Apis.YouTube.v3;
using Ysm.Core;

namespace Ysm.Data
{
    public static class ApiDownloader
    {
        internal static List<Video> Download(string channel, bool downloadAll)
        {
            if (downloadAll)
            {
                return DownloadAll(channel);
            }
            else
            {
                return DownloadFifteen(channel);
            }
        }

        private static List<Video> DownloadAll(string channelId)
        {
            try
            {
                List<Video> videos = new List<Video>();

                YouTubeService youTubeService = AuthenticationService.Default.YouTubeService;

                string token = string.Empty;

                while (token != null)
                {
                    PlaylistItemsResource.ListRequest request = youTubeService.PlaylistItems.List("snippet,contentDetails");
                    request.MaxResults = 50;
                    request.PageToken = token;
                    request.PlaylistId = UrlHelper.GeUploadPlaylistId(channelId);

                    PlaylistItemListResponse response = request.Execute();

                    token = response.NextPageToken;

                    foreach (PlaylistItem item in response.Items)
                    {
                        try
                        {
                            Video video = new Video();
                            video.ChannelId = channelId;
                            video.VideoId = item.ContentDetails.VideoId;
                            video.Link = $"https://www.youtube.com/watch?v={item.ContentDetails.VideoId}";
                            video.Title = item.Snippet.Title;
                            video.ThumbnailUrl = $"http://img.youtube.com/vi/{video.VideoId}/mqdefault.jpg";
                            if (item.Snippet.PublishedAt != null)
                                video.Published = item.Snippet.PublishedAt.Value;
                            video.Duration = new TimeSpan(0);
                            videos.Add(video);
                        }
                        catch (Exception ex)
                        {
                            #region error

                            Error error = new Error
                            {
                                AssemblyName = typeof(ApiDownloader).Assembly.FullName,
                                ClassName = typeof(ApiDownloader).FullName,
                                MethodName = MethodBase.GetCurrentMethod().Name,
                                ExceptionType = ex.GetType().ToString(),
                                Message = ex.Message,
                                Trace = ex.StackTrace
                            };

                            Logger.Log(error);

                            #endregion
                        }
                    }
                }

                return videos.ToList();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ApiDownloader).Assembly.FullName,
                    ClassName = typeof(ApiDownloader).FullName,
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

        private static List<Video> DownloadFifteen(string channelId)
        {
            try
            {
                List<Video> videos = new List<Video>();

                YouTubeService youTubeService = AuthenticationService.Default.YouTubeService;

                PlaylistItemsResource.ListRequest request = youTubeService.PlaylistItems.List("snippet,contentDetails");
                request.MaxResults = 50;
                request.PlaylistId = UrlHelper.GeUploadPlaylistId(channelId);

                PlaylistItemListResponse response = request.Execute();
                
                foreach (PlaylistItem item in response.Items)
                {
                    try
                    {
                        Video video = new Video();
                        video.ChannelId = channelId;
                        video.VideoId = item.ContentDetails.VideoId;
                        video.Link = $"https://www.youtube.com/watch?v={item.ContentDetails.VideoId}";
                        video.Title = item.Snippet.Title;
                        video.ThumbnailUrl = $"http://img.youtube.com/vi/{video.VideoId}/mqdefault.jpg";
                        if (item.Snippet.PublishedAt != null)
                            video.Published = item.Snippet.PublishedAt.Value;
                        video.Duration = new TimeSpan(0);
                        videos.Add(video);
                    }
                    catch (Exception ex)
                    {
                        #region error

                        Error error = new Error
                        {
                            AssemblyName = typeof(ApiDownloader).Assembly.FullName,
                            ClassName = typeof(ApiDownloader).FullName,
                            MethodName = MethodBase.GetCurrentMethod().Name,
                            ExceptionType = ex.GetType().ToString(),
                            Message = ex.Message,
                            Trace = ex.StackTrace
                        };

                        Logger.Log(error);

                        #endregion
                    }
                }

                return videos.Take(15).ToList();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(ApiDownloader).Assembly.FullName,
                    ClassName = typeof(ApiDownloader).FullName,
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
    }
}
