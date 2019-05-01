//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Gms.Gcm;
//using Android.Util;
//using SeekiosApp.Droid.View;
//using Android.Locations;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Model.DTO;

//namespace SeekiosApp.Droid.GCM
//{
//    [Service(Exported = false), IntentFilter(new[] { "com.google.android.c2dm.intent.RECEIVE" })]
//    public class GcmListenerBackgroundService : GcmListenerService
//    {
//        public override void OnMessageReceived(string from, Bundle data)
//        {
//            var message = data.GetString("alert"); //pour onesignal
//            //var message = data.GetString("message"); //pour signalR
//            Log.Debug("MyGcmListenerService", "From:    " + from);
//            Log.Debug("MyGcmListenerService", "Message: " + message);

//            Services.GCMMessageManagerService.RECEIVED_MESSAGE = message;
//            StartService(new Intent(this, typeof(Services.GCMMessageManagerService)));
//        }
//    }
//}