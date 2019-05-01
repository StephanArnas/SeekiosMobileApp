using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Interfaces
{
    public interface ILocalNotificationService
    {
        void SendNotification(SeekiosDTO seekios, string title, string message, bool isAlert = false, bool isLowBattery = false);
    }
}
