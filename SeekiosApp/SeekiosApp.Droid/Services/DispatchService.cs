using System;
using Android.App;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Services
{
    public class DispatchService : IDispatchOnUIThread
    {
        private Activity _owner;
        public DispatchService()
        {
        }
        public void SetContext(Activity owner)
        {
            _owner = owner;
        }
        public void Invoke(Action action)
        {
            _owner.RunOnUiThread(action);
        }
    }
}