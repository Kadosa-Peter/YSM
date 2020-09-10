using System;
using System.Timers;
using System.Windows;

namespace Ysm.Assets
{
    public class DownloadTimer
    {
        private static readonly Lazy<DownloadTimer> Instance = new Lazy<DownloadTimer>(() => new DownloadTimer());

        public static DownloadTimer Default => Instance.Value;

        private DownloadTimer() { }

        private Timer _timer;

        public void Start()
        {
            if (Settings.Default.AutoDownload)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Dispose();
                }

                int minutes = Settings.Default.AutoDownloadMinutes;

                if (minutes < 5)
                    minutes = 5;
                if (minutes > 90)
                    minutes = 90;

                _timer = new Timer(minutes * 60 * 1000);
                _timer.Elapsed += Timer_Elapsed;

                _timer.Start();
            }
        }

        public void Stop()
        {
            _timer?.Stop();
        }

        public void Reset()
        {
            _timer?.Stop();
            _timer?.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { VideoServiceWrapper.Default.Start(null); }));
        }
    }
}
