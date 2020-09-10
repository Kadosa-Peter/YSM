namespace Ysm.Downloader.Assets
{
    public class DownloadModeWindowResult
    {
        public PlaylistDownloadMode PlaylistDownloadMode { get; set; }

        public int PreferredQuality { get; set; }

        public bool IsCancelled { get; set; }
    }
}