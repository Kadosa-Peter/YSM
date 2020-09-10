using System;
using System.Timers;

namespace Ysm.Core
{
    public class Timer
    {
        public event EventHandler Done;
        public event EventHandler Tick;

        private readonly System.Timers.Timer _timer;
        private readonly int _duration;
        private int _elapsed = 1;

        public Timer(int interval, int duration)
        {
            _duration = duration;
            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_elapsed == _duration)
            {
                _timer.Stop();
                _timer.Dispose();

                Done?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _elapsed++;

                Tick?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }


    }
}
