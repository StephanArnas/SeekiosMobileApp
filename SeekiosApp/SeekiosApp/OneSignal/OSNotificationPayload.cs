using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OSNotificationPayloadApp
    {
        public string NotificationID { get; set; }
        public string Sound { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Subtitle { get; set; }
        public string LaunchURL { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
        public List<Dictionary<string, object>> ActionButtons { get; set; }
        public bool ContentAvailable { get; set; }
        public int Badge { get; set; }
        public string SmallIcon { get; set; }
        public string LargeIcon { get; set; }
        public string BigPicture { get; set; }
        public string SmallIconAccentColor { get; set; }
        public string LedColor { get; set; }
        public int LockScreenVisibility { get; set; }
        public string GroupKey { get; set; }
        public string GroupMessage { get; set; }
        public string FromProjectNumber { get; set; }
        public OSNotificationPayloadApp()
        {
            LockScreenVisibility = 1;
            AdditionalData = new Dictionary<string, object>();
            ActionButtons = new List<Dictionary<string, object>>();
        }
    }
}
