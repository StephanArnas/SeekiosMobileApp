using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OSNotificationActionApp
    {
        public enum ActionType
        {
            // notification was tapped on.
            Opened,

            // user tapped on an action from the notification.
            ActionTaken
        }

        public string ActionID { get; set; }
        public ActionType Type { get; set; }
    }
}
