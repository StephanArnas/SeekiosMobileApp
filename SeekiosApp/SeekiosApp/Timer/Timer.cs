using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Timers
{
    public class Timer
    {
        #region ===== Properties ==================================================================

        public bool IsRunning { get; private set; }
        public TimeSpan Interval { get; set; }
        public Action Tick { get; set; }
        public bool RunOnce { get; set; }
        public Action Stopped { get; set; }
        public Action Started { get; set; }
        public Action UpdateUI { get; set; }
        public double CountDown { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public Timer(TimeSpan interval, Action tick = null, bool runOnce = false)
        {
            Interval = interval;
            Tick = tick;
            RunOnce = runOnce;
        }

        public Timer() { }

        #endregion

        #region ===== Public Methods ==============================================================

        public Timer Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                Started?.Invoke();
                var t = RunTimer();
            }
            return this;
        }

        public void Stop()
        {
            IsRunning = false;
            Stopped?.Invoke();
        }

        private async Task RunTimer()
        {
            while (IsRunning)
            {
                await Task.Delay(Interval);
                if (IsRunning)
                {
                    Tick?.Invoke();
                    if (RunOnce)
                    {
                        Stop();
                    }
                }
            }
        }

        #endregion
    }
}