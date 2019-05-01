using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SeekiosApp.Interfaces;
using System.Timers;

namespace SeekiosApp.iOS.Services
{
    public class Timer : ITimer
    {
        //private ElapsedEventHandler _e;

        public Timer() : base() { }

        public void SetEndEvent(EventHandler e)
        {
            //_e = new ElapsedEventHandler(e);
            //Elapsed += _e;
        }

        public void UnsetEndEvent(EventHandler e)
        {
            //if (_e != null) Elapsed -= _e;
        }

        public void SetEnabled(bool enabled, int interval)
        {
            //Enabled = enabled;
            //Interval = interval;
        }
    }
}