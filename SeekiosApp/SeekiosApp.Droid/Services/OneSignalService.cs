using System.Collections.Generic;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP.OneSignal;
using Org.Json;
using Newtonsoft.Json;
using Com.OneSignal.Android;

namespace SeekiosApp.Droid.Services
{
    public class OneSignalAndroid /*: IOneSignal*/
    {
        //#region ===== Properties ==================================================================


        //#endregion

        //#region ===== Attributs ===================================================================

        //internal OneSignalBuilder _builder = null;

        //#endregion

        //#region ===== Constructor =================================================================

        //public OneSignalAndroid() { }

        //public OneSignalBuilder StartInit(string appID, string googleProjectNumber)
        //{
        //    if (_builder == null)
        //    {
        //        _builder = new OneSignalBuilder(this);
        //    }
        //    _builder.AppID = appID;
        //    _builder.GoogleProjectNumber = googleProjectNumber;
        //    return _builder;
        //}

        //#endregion

        //#region ===== Interface ===================================================================

        //public void SendTag(string tagName, string tagValue)
        //{
        //    OneSignal.SendTag(tagName, tagValue);
        //}

        //public void SendTags(IDictionary<string, string> tags)
        //{
        //    OneSignal.SendTags(JsonConvert.SerializeObject(tags));
        //}

        //public void GetTags()
        //{
        //    OneSignal.GetTags(new GetTagsHandler(_builder));
        //}

        //public void DeleteTag(string key)
        //{
        //    OneSignal.DeleteTag(key);
        //}

        //public void DeleteTags(IList<string> keys)
        //{
        //    OneSignal.DeleteTags(JsonConvert.SerializeObject(keys));
        //}

        //public void IdsAvailable()
        //{
        //    OneSignal.IdsAvailable(new IdsAvailableHandler(_builder));
        //}

        //public void IdsAvailable(OneSignalBuilder.IdsAvailableCallback inIdsAvailableDelegate)
        //{
        //    _builder.IdsAvailableDelegate = inIdsAvailableDelegate;
        //    IdsAvailable();
        //}

        //public void RegisterForPushNotifications() { } // Doesn't apply to Android as the Native SDK always registers with GCM.

        //public void EnableVibrate(bool enable)
        //{
        //    OneSignal.EnableVibrate(enable);
        //}

        //public void EnableSound(bool enable)
        //{
        //    OneSignal.EnableSound(enable);
        //}

        //public void SetInFocusDisplaying(OneSignal.OSInFocusDisplayOption display)
        //{
        //    OneSignal.SetInFocusDisplaying((int)display);
        //}

        //public void SetSubscription(bool enable)
        //{
        //    OneSignal.SetSubscription(enable);
        //}

        //public void PostNotification(Dictionary<string, object> data)
        //{
        //    OneSignal.PostNotification(JsonConvert.SerializeObject(data), new PostNotificationResponseHandler(_builder));
        //}

        //public void SyncHashedEmail(string email)
        //{
        //    OneSignal.SyncHashedEmail(email);
        //}

        //public void PromptLocation()
        //{
        //    OneSignal.PromptLocation();
        //}

        //public void ClearOneSignalNotifications()
        //{
        //    OneSignal.ClearOneSignalNotifications();
        //}

        //public void SetLogLevel(OneSignal.LOG_LEVEL logLevel, OneSignal.LOG_LEVEL visualLevel)
        //{
        //    OneSignal.SetLogLevel((int)logLevel, (int)visualLevel);
        //}

        //public void SetLogLevel(LogLevelEnum logLevel, LogLevelEnum visualLevel)
        //{
            
        //}

        //public void Init()
        //{
            
        //}

        //#endregion

        //#region ===== Handlers ====================================================================

        //private class NotificationReceivedHandler : Java.Lang.Object, OneSignal.INotificationReceivedHandler
        //{
        //    private OneSignalBuilder _builder = null;

        //    public NotificationReceivedHandler(OneSignalBuilder builder)
        //    {
        //        _builder = builder;
        //    }

        //    public void NotificationReceived(OSNotification notification)
        //    {
        //        _builder?.NotificationReceivedDelegate(OSNotificationToNative(notification));
        //    }
        //}

        //private class NotificationOpenedHandler : Java.Lang.Object, OneSignal.INotificationOpenedHandler
        //{
        //    private OneSignalBuilder _builder = null;

        //    public NotificationOpenedHandler(OneSignalBuilder builder)
        //    {
        //        _builder = builder;
        //    }

        //    public void NotificationOpened(OSNotificationOpenResult result)
        //    {
        //        _builder?.NotificationOpenedDelegate(OSNotificationOpenedResultToNative(result));
        //    }
        //}

        //private class IdsAvailableHandler : Java.Lang.Object, OneSignal.IIdsAvailableHandler
        //{
        //    private OneSignalBuilder _builder = null;

        //    public IdsAvailableHandler(OneSignalBuilder builder)
        //    {
        //        _builder = builder;
        //    }

        //    public void IdsAvailable(string p0, string p1)
        //    {
        //        _builder?.IdsAvailableDelegate(p0, p1);
        //    }
        //}

        //private class GetTagsHandler : Java.Lang.Object, OneSignal.IGetTagsHandler
        //{
        //    private OneSignalBuilder _builder = null;

        //    public GetTagsHandler(OneSignalBuilder builder)
        //    {
        //        _builder = builder;
        //    }

        //    public void TagsAvailable(JSONObject jsonObject)
        //    {
        //        Dictionary<string, object> dict = null;
        //        if (jsonObject != null)
        //        {
        //            dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject.ToString());
        //        }
        //        _builder?.TagsReceivedDelegate(dict);
        //    }
        //}

        //private class PostNotificationResponseHandler : Java.Lang.Object, OneSignal.IPostNotificationResponseHandler
        //{
        //    private OneSignalBuilder _builder = null;

        //    public PostNotificationResponseHandler(OneSignalBuilder builder)
        //    {
        //        _builder = builder;
        //    }

        //    public void OnSuccess(JSONObject jsonObject)
        //    {
        //        Dictionary<string, object> dict = null;
        //        if (jsonObject != null)
        //            dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject.ToString());
        //        _builder?.OnPostNotificationSuccessCallback(dict);
        //    }

        //    public void OnFailure(JSONObject jsonObject)
        //    {
        //        Dictionary<string, object> dict = null;
        //        if (jsonObject != null)
        //            dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonObject.ToString());
        //        _builder?.OnPostNotificationFailedCallback(dict);
        //    }
        //}

        //#endregion

        //#region ===== Private Methods =============================================================

        //private static OSNotificationApp OSNotificationToNative(OSNotification notif)
        //{
        //    var notification = new OSNotificationApp();
        //    notification.Shown = notif.Shown;
        //    notification.AndroidNotificationId = notif.AndroidNotificationId;
        //    notif.GroupedNotifications = notif.GroupedNotifications;
        //    notif.IsAppInFocus = notif.IsAppInFocus;

        //    notification.Payload = new OSNotificationPayloadApp();


        //    notification.Payload.ActionButtons = new List<Dictionary<string, object>>();
        //    if (notif.Payload.ActionButtons != null)
        //    {
        //        foreach (OSNotificationPayload.ActionButton button in notif.Payload.ActionButtons)
        //        {
        //            var d = new Dictionary<string, object>();
        //            d.Add(button.Id, button.Text);
        //            notification.Payload.ActionButtons.Add(d);
        //        }
        //    }

        //    notification.Payload.AdditionalData = new Dictionary<string, object>();
        //    if (notif.Payload.AdditionalData != null)
        //    {
        //        var iterator = notif.Payload.AdditionalData.Keys();
        //        while (iterator.HasNext)
        //        {
        //            var key = (string)iterator.Next();
        //            notification.Payload.AdditionalData.Add(key, notif.Payload.AdditionalData.Get(key));
        //        }
        //    }

        //    notification.Payload.Body = notif.Payload.Body;
        //    notification.Payload.LaunchURL = notif.Payload.LaunchURL;
        //    notification.Payload.NotificationID = notif.Payload.NotificationID;
        //    notification.Payload.Sound = notif.Payload.Sound;
        //    notification.Payload.Title = notif.Payload.Title;
        //    notification.Payload.BigPicture = notif.Payload.BigPicture;
        //    notification.Payload.FromProjectNumber = notif.Payload.FromProjectNumber;
        //    notification.Payload.GroupMessage = notif.Payload.GroupKey;
        //    notification.Payload.GroupMessage = notif.Payload.GroupMessage;
        //    notification.Payload.LargeIcon = notif.Payload.LargeIcon;
        //    notification.Payload.LedColor = notif.Payload.LedColor;
        //    notification.Payload.LockScreenVisibility = notif.Payload.LockScreenVisibility;
        //    notification.Payload.SmallIcon = notif.Payload.SmallIcon;
        //    notification.Payload.SmallIconAccentColor = notif.Payload.SmallIconAccentColor;

        //    return notification;
        //}

        //private static OSNotificationOpenedResultApp OSNotificationOpenedResultToNative(OSNotificationOpenResult result)
        //{
        //    OSNotificationActionApp.ActionType actionType = OSNotificationActionApp.ActionType.Opened;
        //    if (result.Action.Type == OSNotificationAction.ActionType.Opened)
        //        actionType = OSNotificationActionApp.ActionType.Opened;
        //    else
        //        actionType = OSNotificationActionApp.ActionType.ActionTaken;

        //    var openresult = new OSNotificationOpenedResultApp();
        //    openresult.Action = new OSNotificationActionApp();
        //    OSNotificationAction action = result.Action;
        //    openresult.Action.ActionID = action.ActionID;
        //    openresult.Action.Type = actionType;

        //    openresult.Notification = OSNotificationToNative(result.Notification);

        //    return openresult;
        //}

        //#endregion
    }
}
