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
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Services
{
    public class PhoneUnlockedReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //phone was unlocked, do stuff here 
            //if (App.IsDeviceLocked)
            //{
            //    App.Locator.Home.ActivateNotification();
            //    App.IsDeviceLocked = false;
            //}
        }
    }
}