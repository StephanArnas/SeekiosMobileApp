using System;
using Android.App;
using Android.Content;
using Android.OS;
using System.Threading;

namespace SeekiosApp.Droid.Services
{
    [Service]
    public class CloseGestureService : Service
    {
        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            DoWork();
            return StartCommandResult.Sticky;
        }

        public void DoWork()
        {
            var t = new Thread(() => { while (true) { Thread.Sleep(99999999); } });
            t.Start();
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            if (App.CurrentUserEnvironment != null && App.Locator.Login.GetSavedCredentials() != null)
            {
                App.Locator.Login.SaveCurrentCredentials();
            }
            base.OnTaskRemoved(rootIntent);
        }
    }
}