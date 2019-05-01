using System;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Helper
{
    public class TimerWrapper : System.Timers.Timer, ITimer
    {
        private System.Timers.ElapsedEventHandler _e;

        public TimerWrapper() : base() { }

        public void SetEndEvent(EventHandler e)
        {
            _e = new System.Timers.ElapsedEventHandler(e);
            Elapsed += _e;
        }

        public void UnsetEndEvent(EventHandler e)
        {
            if (_e != null) Elapsed -= _e;
        }

        public void SetEnabled(bool enabled, int interval)
        {
            Enabled = enabled;
            Interval = interval;
        }

    }
}