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

namespace SeekiosApp.Droid.Services
{
    public class PhoneLockedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //if (!App.IsDeviceLocked)
            //{
            //    App.Locator.Home.DesactivateNotification();
            //    App.IsDeviceLocked = true;
            //}
        }
    }
}