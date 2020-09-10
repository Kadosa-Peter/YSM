using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;
using Ysm.Core;
using Ysm.Downloader.Assets;

namespace Ysm.Downloader.Download
{
    public static class PreferredStream
    {
        public static async Task<StreamObj> GetAsync(string id, bool audioOnly, int preferredQuality)
        {
            try
            {
                YoutubeClient client = new YoutubeClient();

                var streamSet = await client.GetVideoMediaStreamInfosAsync(id);
                var audioStream = streamSet.Audio.OrderByDescending(a => a.Bitrate).First();

                if (audioOnly)
                {
                    return new StreamObj { AudioStream = audioStream };
                }
                else
                {
                    List<StreamObj> streamObjs = new List<StreamObj>();
                    List<StreamObj> preferredStreamObjs = new List<StreamObj>();
                    List<int> qualityLevels = new List<int> { 2880, 2160, 1440, 1080, 720, 480, 360 };

                    // 1080p and above
                    foreach (VideoStreamInfo info in streamSet.Video.Where(x => (int)x.VideoQuality > 4))
                    {
                        int qualityLabel = info.VideoQualityLabel.GetQualityLevel();
                        streamObjs.Add(new StreamObj { AudioStream = audioStream, VideoStream = info, QualityLevel = qualityLabel });
                    }

                    // 720p and below
                    foreach (MuxedStreamInfo info in streamSet.Muxed.Where(x => (int)x.VideoQuality > 1))
                    {
                        int qualityLevel = info.VideoQualityLabel.GetQualityLevel();
                        streamObjs.Add(new StreamObj { AudioStream = audioStream, MuxedStream = info, QualityLevel = qualityLevel });
                    }

                    foreach (int qualityLevel in qualityLevels)
                    {
                        if (qualityLevel > preferredQuality) continue;

                        if (streamObjs.Any(x => x.QualityLevel == qualityLevel) == false) continue;

                        preferredStreamObjs = streamObjs.Where(x => x.QualityLevel == qualityLevel).ToList();

                        break;
                    }

                    if (preferredStreamObjs.Any(x => x.MuxedStream != null))
                    {
                        return preferredStreamObjs.FirstOrDefault(x => x.MuxedStream.Container == Container.Mp4) ??
                               preferredStreamObjs.FirstOrDefault(x => x.MuxedStream.Container == Container.WebM);
                    }
                    else
                    {
                        return preferredStreamObjs.FirstOrDefault(x => x.VideoStream.Container == Container.Mp4) ??
                               preferredStreamObjs.FirstOrDefault(x => x.VideoStream.Container == Container.WebM);
                    }
                }
            }
            catch (Exception ex)
            {
                #region error

                Error error = new Error
                {
                    AssemblyName = typeof(PreferredStream).Assembly.FullName,
                    ClassName = typeof(PreferredStream).FullName,
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
