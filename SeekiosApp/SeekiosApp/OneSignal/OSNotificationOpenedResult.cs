using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OSNotificationOpenedResultApp
    {
        public OSNotificationActionApp Action { get; set; }
        public OSNotificationApp Notification { get; set; }
    }
}
