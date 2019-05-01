using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using SeekiosApp.Droid.View;
using Android.Graphics;
using Android.Support.V7.App;
using SeekiosApp.Model.DTO;
using SeekiosApp.Droid.Services;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.Helper
{
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// Send a local notification on the phone
        /// </summary>
        public void SendNotification(SeekiosDTO seekios, string title, string message, bool isAlert = false, bool isLowBattery = false)
        {
            var context = AppCompatActivityBase.CurrentActivity;
            if (seekios == null) throw new Exception("SendNotification: seekios can not be null");
            if (context == null) throw new Exception("SendNotification: context can not be null");

            var intent = new Intent(context, typeof(DetailSeekiosActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            using (var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.UpdateCurrent))
            using (var builder = new NotificationCompat.Builder(context))
            {
                Bitmap largeIcon = null;

                if (!string.IsNullOrEmpty(seekios.SeekiosPicture))
                {
                    try
                    {
                        var bytes = Convert.FromBase64String(seekios.SeekiosPicture);
                        largeIcon = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                    }
                    catch (System.Exception)
                    {
                        largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.DefaultSeekios);
                    }
                }
                else largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.DefaultSeekios);

                if (isAlert) largeIcon = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.AlertNotificationIcon);

                // For Lollipop notification must be WHITE + TRANSPARENT
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    if (isLowBattery) builder.SetSmallIcon(Resource.Drawable.BatteryWarning);
                    else builder.SetSmallIcon(Resource.Drawable.IconTransparent);
                }
                else builder.SetSmallIcon(Resource.Drawable.Icon);
                builder.SetLargeIcon(largeIcon)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetAutoCancel(true)
                    .SetColor(isLowBattery ? 14298670 : 8435503)
                    .SetDefaults((int)NotificationDefaults.Sound | (int)NotificationDefaults.Vibrate);
                if (pendingIntent != null) builder.SetContentIntent(pendingIntent);

                ((NotificationManager)context.GetSystemService(Context.NotificationService)).Notify(1, builder.Build());

                largeIcon.Dispose();
            }
        }
    }
}