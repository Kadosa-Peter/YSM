using YoutubeExplode.Models.MediaStreams;

namespace Ysm.Downloader.Download
{
    public class StreamObj
    {
        public MuxedStreamInfo MuxedStream { get; set; }

        public VideoStreamInfo VideoStream { get; set; }

        public AudioStreamInfo AudioStream { get; set; }

        public int QualityLevel { get; set; }
    }
}
