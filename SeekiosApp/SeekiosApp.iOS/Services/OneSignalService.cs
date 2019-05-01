using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SeekiosApp.Interfaces;
using SeekiosApp.Services;
using SeekiosApp.Model.APP.OneSignal;
using Com.OneSignal.iOS;
using Newtonsoft.Json;
using SeekiosApp.Extension;
using SeekiosApp.Helper;

namespace SeekiosApp.iOS.Services
{
    public class OneSignalIOS : IOneSignal
    {
        #region ===== Properties ==================================================================

        public const string kOSSettingsKeyAutoPrompt = "kOSSettingsKeyAutoPrompt";
        public const string kOSSettingsKeyInAppLaunchURL = "kOSSettingsKeyInAppLaunchURL";

        #endregion

        #region ===== Attributs ===================================================================

        internal OneSignalBuilder _builder = null;
        private LogLevelEnum _logLevel = LogLevelEnum.INFO;
        private LogLevelEnum _visualLogLevel = LogLevelEnum.NONE;

        #endregion

        #region ===== Constructor =================================================================

        public OneSignalIOS() { }

        public OneSignalBuilder StartInit(string appID, string googleProjectNumber = "")
        {
            if (_builder == null)
            {
                _builder = new OneSignalBuilder(this);
            }
            _builder.AppID = appID;
            return _builder;
        }

        #endregion

        #region ===== Interface ===================================================================

        public void Init()
        {
            bool autoPrompt = false, inAppLaunchURL = false;

            if (_builder.IOSSettings != null)
            {
                if (_builder.IOSSettings.ContainsKey(kOSSettingsKeyAutoPrompt))
                {
                    autoPrompt = _builder.IOSSettings[kOSSettingsKeyAutoPrompt];
                }
                if (_builder.IOSSettings.ContainsKey(kOSSettingsKeyInAppLaunchURL))
                {
                    inAppLaunchURL = _builder.IOSSettings[kOSSettingsKeyInAppLaunchURL];
                }
            }

            var convertedLogLevel = (OneSLogLevel)((ulong)((int)_logLevel));
            var convertedVisualLevel = (OneSLogLevel)((ulong)((int)_visualLogLevel));

            OneSignal.SetLogLevel(convertedLogLevel, convertedVisualLevel);

            var dict = new Foundation.NSDictionary("kOSSettingsKeyInAppLaunchURL"
                , new Foundation.NSNumber(inAppLaunchURL)
                , "kOSSettingsKeyAutoPrompt"
                , new Foundation.NSNumber(autoPrompt)
                , "kOSSettingsKeyInFocusDisplayOption"
                , new Foundation.NSNumber((int)_builder.DisplayOption));

            OneSignal.InitWithLaunchOptions(new Foundation.NSDictionary()
                , _builder.AppID
                , NotificationReceivedHandler
                , NotificationOpenedHandler
                , dict);
        }

        private void NotificationReceivedHandler(OSNotification notification)
        {
            _builder.NotificationReceivedDelegate?.Invoke(OSNotificationToNative(notification));
        }

        private void NotificationOpenedHandler(OSNotificationOpenedResult notification)
        {
            _builder.NotificationOpenedDelegate?.Invoke(OSNotificationOpenedResultToNative(notification));
        }

        public void RegisterForPushNotifications()
        {
            OneSignal.RegisterForPushNotifications();
        }

        public void SendTag(string tagName, string tagValue)
        {
            OneSignal.SendTag(tagName, tagValue);
        }

        public void SendTags(IDictionary<string, string> tags)
        {
            var jsonString = JsonConvert.SerializeObject(tags);
            OneSignal.SendTagsWithJsonString(jsonString);
        }

        public void GetTags(OneSignalBuilder.TagsReceived tagsReceivedDelegate)
        {
            _builder.TagsReceivedDelegate = tagsReceivedDelegate;
            GetTags();
        }

        public void GetTags()
        {
            OneSignal.GetTags(GetTagsHandler);
        }

        public void DeleteTag(string key)
        {
            OneSignal.DeleteTag(key);
        }

        public void DeleteTags(IList<string> keys)
        {
            var objs = new Foundation.NSObject[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                objs[i] = (Foundation.NSString)keys[i];
            }
            OneSignal.DeleteTags(objs);
        }

        public void IdsAvailable()
        {
            OneSignal.IdsAvailable(IdsAvailableHandler);
        }

        public void IdsAvailable(OneSignalBuilder.IdsAvailableCallback inIdsAvailableDelegate)
        {
            _builder.IdsAvailableDelegate = inIdsAvailableDelegate;
            IdsAvailable();
        }

        public void SetSubscription(bool enable)
        {
            OneSignal.SetSubscription(enable);
        }

        public void PostNotification(Dictionary<string, object> data)
        {
            var jsonString = JsonConvert.SerializeObject(data);
            OneSignal.PostNotificationWithJsonString(jsonString, PostNotificationSuccessHandler, PostNotificationFailureHandler);
        }

        public void SetLogLevel(LogLevelEnum logLevel, LogLevelEnum visualLevel)
        {
            var convertedLogLevel = (OneSLogLevel)((ulong)((int)logLevel));
            var convertedVisualLevel = (OneSLogLevel)((ulong)((int)visualLevel));
            OneSignal.SetLogLevel(convertedLogLevel, convertedVisualLevel);
        }

        public void SyncHashedEmail(string email)
        {
            OneSignal.SyncHashedEmail(email);
        }

        public void PromptLocation()
        {
            OneSignal.PromptLocation();
        }

        #endregion

        #region ===== Handlers ====================================================================

        public void IdsAvailableHandler(string playerID, string pushToken)
        {
            _builder?.IdsAvailableDelegate(playerID, pushToken);
        }

        public void GetTagsHandler(Foundation.NSDictionary result)
        {
            var dict = ParserHelper.ConvertToDictionaryParser(result.ToString());
            _builder?.TagsReceivedDelegate(dict);
        }

        public void PostNotificationSuccessHandler(Foundation.NSDictionary result)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.ToString());
            _builder?.OnPostNotificationSuccessCallback(dict);
        }

        public void PostNotificationFailureHandler(Foundation.NSError error)
        {
            if (error.UserInfo != null && error.UserInfo["returned"] != null)
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(error.UserInfo.ToString());
                _builder?.OnPostNotificationFailedCallback(dict);
            }
            else
            {
                _builder?.OnPostNotificationFailedCallback(new Dictionary<string, object> { { "error", "HTTP no response error" } });
            }
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private OSNotificationOpenedResultApp OSNotificationOpenedResultToNative(OSNotificationOpenedResult result)
        {
            var openresult = new OSNotificationOpenedResultApp();
            openresult.Action = new OSNotificationActionApp();
            openresult.Action.ActionID = result.Action.ActionID;
            openresult.Action.Type = (OSNotificationActionApp.ActionType)(int)result.Action.Type;
            openresult.Notification = OSNotificationToNative(result.Notification);

            return openresult;
        }

        private OSNotificationApp OSNotificationToNative(OSNotification notif)
        {
            var notification = new OSNotificationApp();
            notification.DisplayTypeNotification = (OSNotificationApp.DisplayType)notif.DisplayType;
            notification.Shown = notif.Shown;
            notification.SilentNotification = notif.SilentNotification;
            notification.Payload = new OSNotificationPayloadApp();

            notification.Payload.ActionButtons = new List<Dictionary<string, object>>();
            if (notif.Payload.ActionButtons != null)
            {
                for (int i = 0; i < (int)notif.Payload.ActionButtons.Count; ++i)
                {
                    var element = notif.Payload.ActionButtons.GetItem<Foundation.NSDictionary>((uint)i);
                    if (element.ToString().IsJson())
                    {
                        notification.Payload.ActionButtons.Add(JsonConvert.DeserializeObject<Dictionary<string, object>>(element.ToString()));
                    }
                }
            }

            notification.Payload.AdditionalData = new Dictionary<string, object>();
            if (notif.Payload.AdditionalData != null)
            {
                foreach (KeyValuePair<Foundation.NSObject, Foundation.NSObject> element in notif.Payload.AdditionalData)
                {
                    notification.Payload.AdditionalData.Add((Foundation.NSString)element.Key, element.Value);
                }
            }

            notification.Payload.Badge = (int)notif.Payload.Badge;
            notification.Payload.Body = notif.Payload.Body;
            notification.Payload.ContentAvailable = notif.Payload.ContentAvailable;
            notification.Payload.LaunchURL = notif.Payload.LaunchURL;
            notification.Payload.NotificationID = notif.Payload.NotificationID;
            notification.Payload.Sound = notif.Payload.Sound;
            notification.Payload.Subtitle = notif.Payload.Subtitle;
            notification.Payload.Title = notif.Payload.Title;

            return notification;
        }

        #endregion
    }
}