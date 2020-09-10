using System;
using System.Windows.Threading;

namespace Ysm.Assets.Trial
{
    class TrialTimer
    {
        public delegate void TrialTimerDelegate(double left);

        public event TrialTimerDelegate Elapsed;

        private double _hours;

        private readonly DispatcherTimer _timer;

        public TrialTimer(double hours)
        {
            _hours = hours;

            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(1, 0, 0);
            _timer.Tick += Timer_Tick;
        }

        public void Start()
        {
            _timer.IsEnabled = true;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _hours--;

            if (_hours < 0)
            {
                _hours = 0;
            }

            if (Math.Abs(_hours) < 0.1)
            {
                _timer.Stop();
            }

            Elapsed?.Invoke(_hours);

        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
