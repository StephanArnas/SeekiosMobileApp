using SeekiosApp.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static SeekiosApp.Model.APP.OneSignal.OneSignalHandler;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OneSignalBuilder
    {
        // notificationReceived - Delegate is called when a push notification is received when the user is in your game.
        // notification = The Notification dictionary filled from a serialized native OSNotification object
        public delegate void NotificationReceived(OSNotificationApp notification);

        // notificationOpened - Delegate is called when a push notification is opened.
        // result = The Notification open result describing : 1. The notification opened 2. The action taken by the user
        public delegate void NotificationOpened(OSNotificationOpenedResultApp result);

        public delegate void IdsAvailableCallback(string playerID, string pushToken);
        public delegate void TagsReceived(Dictionary<string, object> tags);

        public delegate void OnPostNotificationSuccess(Dictionary<string, object> response);
        public delegate void OnPostNotificationFailure(Dictionary<string, object> response);

        public IdsAvailableCallback IdsAvailableDelegate { get; set; }
        public TagsReceived TagsReceivedDelegate { get; set; }
        public string AppID { get; set; }
        public string GoogleProjectNumber { get; set; }
        public Dictionary<string, bool> IOSSettings { get; set; }
        public OSInFocusDisplayOptionEnum DisplayOption { get; set; }
        public NotificationReceived NotificationReceivedDelegate { get; set; }
        public NotificationOpened NotificationOpenedDelegate { get; set; }

        internal IOneSignal _parent = null;
        internal OnPostNotificationSuccess _postNotificationSuccessDelegate;
        internal OnPostNotificationFailure _postNotificationFailureDelegate;

        public OneSignalBuilder(IOneSignal parent)
        {
            DisplayOption = OSInFocusDisplayOptionEnum.InAppAlert;
            _parent = parent;
        }

        public OneSignalBuilder HandleNotificationReceived(NotificationReceived inNotificationReceivedDelegate)
        {
            NotificationReceivedDelegate = inNotificationReceivedDelegate;
            return this;
        }

        public OneSignalBuilder HandleNotificationOpened(NotificationOpened inNotificationOpenedDelegate)
        {
            NotificationOpenedDelegate = inNotificationOpenedDelegate;
            return this;
        }

        public OneSignalBuilder InFocusDisplaying(OSInFocusDisplayOptionEnum display)
        {
            DisplayOption = display;
            return this;
        }

        public OneSignalBuilder Settings(Dictionary<string, bool> settings)
        {
            IOSSettings = settings;
            return this;
        }

        // Called from the native SDK
        public void OnPostNotificationSuccessCallback(Dictionary<string, object> response)
        {
            if (_postNotificationSuccessDelegate != null)
            {
                OnPostNotificationSuccess tempPostNotificationSuccessDelegate = _postNotificationSuccessDelegate;
                _postNotificationFailureDelegate = null;
                _postNotificationSuccessDelegate = null;
                tempPostNotificationSuccessDelegate(response);
            }
        }

        public void OnPostNotificationFailedCallback(Dictionary<string, object> response)
        {
            if (_postNotificationFailureDelegate != null)
            {
                OnPostNotificationFailure tempPostNotificationFailureDelegate = _postNotificationFailureDelegate;
                _postNotificationFailureDelegate = null;
                _postNotificationSuccessDelegate = null;
                tempPostNotificationFailureDelegate(response);
            }
        }

        public void EndInit()
        {
            _parent.Init();
        }
    }
}
