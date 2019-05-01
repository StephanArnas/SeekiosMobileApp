using System;
using SeekiosApp.Interfaces;
using Foundation;
using UIKit;
using CoreFoundation;

namespace SeekiosApp.iOS.Services
{
    public class DispatchService : IDispatchOnUIThread
    {
        public void Invoke(Action action)
        {
            using (var pool = new NSAutoreleasePool())
            {
                try
                {
                    pool.InvokeOnMainThread(delegate
                    {
                        action.Invoke();
                    });
                }
                catch (Exception)
                {
                    //TODO : Error msg ? 
                }
            }
        }
    }
}