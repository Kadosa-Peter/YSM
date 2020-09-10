using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ysm.Core;

namespace Ysm.Data
{
    internal class PlaylistQueries
    {
        internal static void CreateDefaults()
        {
            if (Exists("Favorites") == false)
            {
                Playlist favorites = new Playlist();
                favorites.Name = "Favorites";
                favorites.Id = "Favorites";
                favorites.Default = true;
                Save(favorites);
            }

            if (Exists("WatchLater") == false)
            {
                Playlist watchLater = new Playlist();
                watchLater.Name = "Watch Later";
                watchLater.Id = "WatchLater";
                watchLater.Default = true;
                Save(watchLater);
            }
        }

        internal static void Save(Playlist playlist)
        {
            try
            {
                if (playlist != null)
                {
                    if (Directory.Exists(FileSystem.Playlists) == false)
                        Directory.CreateDirectory(FileSystem.Playlists);

                    string path = Path.Combine(FileSystem.Playlists, playlist.Id);

                    string json = JsonConvert.SerializeObject(playlist, Formatting.Indented);

                    path.WriteText(json);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static Playlist Get(string id)
        {
            try
            {
                string path = Path.Combine(FileSystem.Playlists, id);

                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    return JsonConvert.DeserializeObject<Playlist>(json);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
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

        internal static List<Playlist> GetAll()
        {
            try
            {
                ConcurrentBag<Playlist> playlists = new ConcurrentBag<Playlist>();

                ParallelOptions options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount - 1 };

                Parallel.ForEach(new DirectoryInfo(FileSystem.Playlists).GetFiles(), options, file =>
                {
                    try
                    {
                        string json = file.FullName.ReadText();

                        if (json.NotNull())
                        {
                            Playlist playlist = JsonConvert.DeserializeObject<Playlist>(json);

                            if (playlist != null)
                                playlists.Add(playlist);
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), ex);
                    }
                });

                return playlists.ToList();
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
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

        internal static void Insert(Video video, string id)
        {
            try
            {
                Playlist playlist = Find(id);

                if (playlist != null)
                {
                    playlist.Add(video);

                    Save(playlist);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void RemoveAll(string id)
        {
            try
            {
                Playlist playlist = Find(id);

                if (playlist != null)
                {
                    playlist.RemoveAll();

                    Save(playlist);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void Remove(string videoId, string playlistId)
        {
            try
            {
                Playlist playlist = Find(playlistId);

                if (playlist != null)
                {
                    playlist.Remove(videoId);

                    Save(playlist);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void Delete(string playlistId)
        {
            try
            {
                string path = Path.Combine(FileSystem.Playlists, playlistId);

                if (File.Exists(path))
                {
                    try
                    {
                        File.Delete(path);
                    }
                    catch (Exception e)
                    {
                        Logger.Log(MethodBase.GetCurrentMethod(), e);
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        internal static void Rename(string id, string name)
        {
            try
            {
                Playlist playlist = Find(id);

                if (playlist != null)
                {
                    playlist.Name = name;

                    Save(playlist);
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
                    MethodName = MethodBase.GetCurrentMethod().Name,
                    ExceptionType = ex.GetType().ToString(),
                    Message = ex.Message,
                    Trace = ex.StackTrace,
                };

                Logger.Log(error);

                #endregion
            }
        }

        private static Playlist Find(string id)
        {
            try
            {
                string path = Path.Combine(FileSystem.Playlists, id);

                if (File.Exists(path))
                {
                    string json = path.ReadText();

                    return JsonConvert.DeserializeObject<Playlist>(json);
                }

            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PlaylistQueries).Assembly.FullName,
                    ClassName = typeof(PlaylistQueries).FullName,
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

        private static bool Exists(string id)
        {
            string path = Path.Combine(FileSystem.Playlists, id);

            return File.Exists(path);
        }
    }
}
