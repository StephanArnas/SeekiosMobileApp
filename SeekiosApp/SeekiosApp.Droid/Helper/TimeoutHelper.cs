using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;


namespace SeekiosApp.Helper
{
    public class TimeoutHelper
    {
        private bool _timeoutCancellationRequested = false;

        /// <summary>
        /// return true if the timeout is running
        /// </summary>
        public bool IsTimeoutRunning { get; private set; }

        /// <summary>
        /// Start a timeout
        /// </summary>
        /// <param name="timeout">time to wait</param>
        /// <param name="timeoutCallback">callback for the timeout</param>
        /// <param name="cancellationCallback">callback for the timeout cancellation</param>
        public async void WaitTimeout(int timeout, Action timeoutCallback, Action cancellationCallback = null)
        {
            _timeoutCancellationRequested = false;
            if (timeoutCallback == null) return;
            IsTimeoutRunning = true;
            var i = 0;
            while (i * 500 < timeout)
            {
                if (_timeoutCancellationRequested)
                {
                    if (cancellationCallback != null) cancellationCallback.Invoke();
                    IsTimeoutRunning = false;
                    return;
                }
                i++;
                await Task.Delay(500);
            }
            timeoutCallback.Invoke();
            IsTimeoutRunning = false;
        }

        /// <summary>
        /// Stop the timeout
        /// </summary>
        public void StopTimeout()
        {
            _timeoutCancellationRequested = true;
        }
    }
}