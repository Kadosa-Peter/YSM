using System;
using System.Collections.Generic;
using System.Linq;
using Ysm.Downloader.Views;

namespace Ysm.Downloader.Download
{
    internal class DownloadMonitor
    {
        #region Singleton

        private static readonly Lazy<DownloadMonitor> Instance = new Lazy<DownloadMonitor>(() => new DownloadMonitor());

        public static DownloadMonitor Default => Instance.Value;

        #endregion

        private readonly int _simultaneouslyDownload = Environment.ProcessorCount - 1;
        //private readonly int _simultaneouslyDownload = 1;

        private readonly List<DownloadView> _downloads;

        private DownloadMonitor()
        {
            _downloads = new List<DownloadView>();
        }

        public void Add(DownloadView view)
        {
            _downloads.Add(view);

            view.Loaded += View_Loaded;
            view.DownloadComplete += View_DownloadComplete;
            view.DownloadCanceled += View_DownloadCanceled;
        }

        private void View_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_downloads.Count(view => view.IsDownloading) < _simultaneouslyDownload)
            {
                if (sender is DownloadView view)
                {
                    view.StartDownload();
                }
            }
        }

        private void View_DownloadCanceled(object sender, EventArgs e)
        {
            if (sender is DownloadView view)
            {
                view.Loaded -= View_Loaded;
                view.DownloadComplete -= View_DownloadComplete;
                view.DownloadCanceled -= View_DownloadCanceled;

                _downloads.Remove(view);
            }
        }
     
        private void View_DownloadComplete(object sender, EventArgs e)
        {
            _downloads.Remove(sender as DownloadView);

            if (_downloads.Any() && _downloads.Count(x => x.IsDownloading) < _simultaneouslyDownload)
            {
                foreach (DownloadView view in _downloads)
                {
                    // Se azt ne indítsd el, ami kész van se azt (újra) ami folyamatban van.
                    if (view.IsComplete == false && view.IsDownloading == false)
                    {
                        view.StartDownload();
                    }
                }
            }
        }
    }
}
