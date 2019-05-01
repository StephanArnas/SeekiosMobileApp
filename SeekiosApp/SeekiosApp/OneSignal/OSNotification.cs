using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OSNotificationApp
    {
        public enum DisplayType
        {
            // notification shown in the notification shade.
            Notification,

            // notification shown as an in app alert.
            InAppAlert,

            // notification was silent and not displayed.
            None
        }
        public bool IsAppInFocus { get; set; }
        public bool Shown { get; set; }
        public bool SilentNotification { get; set; }
        public int AndroidNotificationId { get; set; }
        public DisplayType DisplayTypeNotification { get; set; }
        public OSNotificationPayloadApp Payload { get; set; }
    }
}
