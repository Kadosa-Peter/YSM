using System;
using YoutubeExplode.Models;
using YoutubeExplode.Models.MediaStreams;

namespace Ysm.Downloader.Download
{
    public class DownloadWindowEventArgs : EventArgs
    {
        public MuxedStreamInfo MuxedStream { get; set; }

        public VideoStreamInfo VideoStream { get; set; }

        public AudioStreamInfo AudioStream { get; set; }

        public Video Video { get; set; }

        public bool Convert { get; set; }

        public string Format { get; set; }

        public bool OnlyAudio { get; set; }

        public string Output { get; set; }
    }
}