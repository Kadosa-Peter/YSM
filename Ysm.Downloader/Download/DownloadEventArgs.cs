using System;

namespace Ysm.Downloader.Download
{
    public class DownloadEventArgs : EventArgs
    {
        public double Percent { get; set; }

        public string Message { get; set; }

        public bool IsComplete { get; set; }

        public bool IsFailed { get; set; }

        public bool IsConverting { get; set; }

        public string Output { get; set; }
    }
}
