using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using GalaSoft.MvvmLight.Ioc;

namespace SeekiosApp.iOS.Services
{
    public class LocalNotificationService : ILocalNotificationService
    {
        /// <summary>
        /// On iOS 10.3.3 a local notification does not exist like Android, we need to show a simple popup
        /// </summary>
        public async void SendNotification(SeekiosDTO seekios, string title, string message, bool isAlert = false, bool isLowBattery = false)
        {
            var dialogService = SimpleIoc.Default.GetInstance<IDialogService>();
            if (dialogService == null) throw new Exception("SendNotification: dialogService can not be null");
            await dialogService.ShowMessage(message, title);
        }
    }
}