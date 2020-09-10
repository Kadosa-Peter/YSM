using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Ysm.Core;

namespace Ysm.Data
{
    internal class MarkerQueries
    {
        internal static List<MarkerGroup> Get()
        {
            List<MarkerGroup> groups = new List<MarkerGroup>();

            try
            {
                DirectoryInfo info = new DirectoryInfo(FileSystem.Markers);

                foreach (FileInfo file in info.GetFiles())
                {
                    try
                    {
                        string json = file.FullName.ReadText();

                        MarkerGroup group = JsonConvert.DeserializeObject<MarkerGroup>(json);

                        groups.Add(group);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }

            return groups;
        }

        internal static MarkerGroup Get(string videoId)
        {
            try
            {
                string path = Path.Combine(FileSystem.Markers, videoId);

                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    return JsonConvert.DeserializeObject<MarkerGroup>(json);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
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

        internal static MarkerGroup Save(Video video, Marker marker)
        {
            List<MarkerGroup> groups = Get();

            try
            {
                MarkerGroup markerGroup = groups.FirstOrDefault(x => x.Id == video.VideoId);

                if (markerGroup != null)
                {
                    marker.GroupId = markerGroup.Id;

                    markerGroup.Markers.Add(marker);
                }
                else
                {
                    markerGroup = CreateMarkerGroup(video, marker);
                }

                Save(markerGroup);

                return markerGroup;
            }
            catch (Exception ex)
            {
                Logger.Log(MethodBase.GetCurrentMethod(), ex);
            }

            return null;
        }

        internal static void Save(MarkerGroup markerGroup)
        {
            try
            {
                string json = JsonConvert.SerializeObject(markerGroup);
                Path.Combine(FileSystem.Markers, markerGroup.Id).WriteText(json);
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static MarkerGroup CreateMarkerGroup(Video video, Marker entry)
        {
            try
            {
                string channelTitle = ChannelQueries.Get_By_Id(video.ChannelId)?.Title;

                MarkerGroup markerGroup = new MarkerGroup();
                markerGroup.Id = video.VideoId;
                markerGroup.Title = video.Title;
                markerGroup.ChannelId = video.ChannelId;
                markerGroup.ChannelTitle = channelTitle;
                markerGroup.Added = DateTimeOffset.Now;
                markerGroup.Published = video.Published;
                markerGroup.Duration = video.Duration;
                markerGroup.Link = video.Link;
                markerGroup.ThumbnailUrl = video.ThumbnailUrl;

                entry.GroupId = markerGroup.Id;

                markerGroup.Markers.Add(entry);

                return markerGroup;
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
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

        internal static void Delete(string id)
        {
            try
            {
                string path = Path.Combine(FileSystem.Markers, id);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void Delete(string id, string entryId)
        {
            try
            {
                MarkerGroup markerGroup = Get().FirstOrDefault(x => x.Id == id);

                if (markerGroup != null)
                {
                    markerGroup.Markers.RemoveAll(x => x.Id == entryId);

                    if (markerGroup.Markers.Count == 0)
                    {
                        Delete(id);
                    }
                    else
                    {
                        Save(markerGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void Update(string groupId, string markId, string comment)
        {
            try
            {
                MarkerGroup markerGroup = Get(groupId);

                if (markerGroup != null)
                {
                    Marker marker = markerGroup.Markers.FirstOrDefault(x => x.Id == markId);

                    if (marker != null)
                    {
                        marker.Comment = comment;

                        Save(markerGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(MarkerQueries).Assembly.FullName,
                    ClassName = typeof(MarkerQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }
    }
}
