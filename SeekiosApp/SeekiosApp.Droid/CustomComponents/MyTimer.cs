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

namespace SeekiosApp.Droid.CustomComponents
{
    /// <summary>
    /// Timer that handles display timeout for password Error handling
    /// </summary>
    public class MyTimer : CountDownTimer
    {
        private TextView _passwordEditText;

        public MyTimer(long millTilFinish, long millEventTick, TextView passwordEditText) : base(millTilFinish, millEventTick)
        {
            _passwordEditText = passwordEditText;
        }

        public override void OnFinish()
        {
            HideError();
        }

        public override void OnTick(long millisUntilFinished)
        {
            HideError();
        }
        private void HideError()
        {
            _passwordEditText.SetError(string.Empty, null);
            _passwordEditText.Error = null;
            _passwordEditText.RequestFocus();
            Cancel();
        }
    }
}