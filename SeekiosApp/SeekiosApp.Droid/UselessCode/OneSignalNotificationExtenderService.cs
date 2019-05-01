//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Com.Onesignal;
//using Android.Support.V4.App;

//namespace SeekiosApp.Droid.Services
//{
//    public class OneSignalNotificationExtenderService : NotificationExtenderService, NotificationCompat.IExtender
//    {
//        protected override bool OnNotificationProcessing(OSNotificationReceivedResult p0)
//        {
//            NotificationCompat.Builder builder = new NotificationCompat.Builder(this);

//            if (p0.Payload.AdditionalData != null) return true;
//            //builder.SetVisibility((int)Android.App.NotificationVisibility.Secret);

//            OverrideSettings overrideSettings = new OverrideSettings();

//            DisplayNotification(overrideSettings);
//            overrideSettings.Extender = (NotificationCompat.IExtender)Extend(builder);

//            return false;
//        }

//        public NotificationCompat.Builder Extend(NotificationCompat.Builder builder)
//        {
//            //builder.Notification.Visibility = Android.App.NotificationVisibility.Secret;
//            return builder;
//        }

//    }
//}