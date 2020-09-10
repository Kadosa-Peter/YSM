using System;

namespace Ysm.Downloader.Download
{
    interface IDownloader
    {
        event EventHandler<DownloadEventArgs> DownloadStarted;
        event EventHandler<DownloadEventArgs> DownloadFinished;
        event EventHandler<DownloadEventArgs> DownloadProgessChanged;

        void Download(string title, string dir);

        void DownloadAudio(string title, string dir);

        void DownloadAndConvert(string title, string dir, string format);

        void Cancel();
    }
}