//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Util;
//using System.Threading.Tasks;
//using Com.Onesignal;
//using Org.Json;
//using Newtonsoft.Json;
//using SeekiosApp.Extension;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Constants;

//namespace SeekiosApp.Droid.Services
//{
//    [Service]
//    public class OneSignalNotificationService : Service
//    {
//        public override IBinder OnBind(Intent intent)
//        {
//            Log.Debug("OneSignalNotificationService", "OnBind");
//            return null;
//        }

//        public override void OnCreate()
//        {
//            Log.Debug("OneSignalNotificationService", "OnCreate");
//            base.OnCreate();
//        }

//        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
//        {
//            Log.Debug("OneSignalNotificationService", "OnStartCommand");

//            var notifTitle = Resources.GetString(Resource.String.dontMoveBackgroundService_notificationTitle);
//            var notifContent = string.Format(Resources.GetString(Resource.String.dontMoveBackgroundService_notificationContent), "");

//            var notificationBuilder = new Notification.Builder(this)
//                //.SetSmallIcon(Resource.Drawable.ModeDontMove)
//                .SetSmallIcon(Resource.Drawable.FollowMeNotifIcon)
//                .SetColor(Resources.GetColor(Resource.Color.primary).ToArgb())
//                .SetContentTitle(notifTitle)
//                .SetContentText(notifContent);

//            StartForeground(startId, notificationBuilder.Build());

//            //Connect to OneSignal
//            OneSignal.Init(this, "1077123816365", "ee8851b0-f171-4de0-b86b-74ef18eefa02", new NotificationOpenedHandler(), new NotificationReceivedHandler(this));
//            OneSignal.SetSubscription(true);
//            //Initialize OneSignal Ids to handle segments
//            OneSignal.IdsAvailable(new IdsAvailableHandler());
//            Log.Debug("OneSignalNotificationService", "OneSignal.Init done");

//            new Task(() =>
//            {
//                while (true) { }
//            }).Start();

//            return StartCommandResult.Sticky;
//        }

//        public override void OnTaskRemoved(Intent rootIntent)
//        {
//            Log.Debug("OneSignalNotificationService", "on stop");
//            base.OnTaskRemoved(rootIntent);
//        }
//    }

//    public class IdsAvailableHandler : Java.Lang.Object, OneSignal.IIdsAvailableHandler
//    {
//        public void IdsAvailable(string oneSignalUserId, string registrationId)
//        {
//            Log.Debug("OneSignalNotificationService", "IdsAvailableHandler start");
//            var _saveDataService = new SaveDataService();
//            _saveDataService.Init(Application.Context);

//            if (!_saveDataService.Contains(SaveDataConstants.UserEnvironment)) return;

//            var json = _saveDataService.GetData(SaveDataConstants.UserEnvironment);
//            if (string.IsNullOrEmpty(json)) return;

//            var userEnvironment = JsonConvert.DeserializeObject<UserEnvironmentDTO>(json);
//            if (userEnvironment == null) return;

//            Log.Debug("OneSignalNotificationService", "IdsAvailableHandler userEnvironment ok");
//            var idUser = userEnvironment.User.IdUser.ToString();
//            //Create user tags
//            OneSignal.GetTags(new GetTagsHandler(idUser, registrationId));
//            //Silent notification
//            OneSignal.SetInFocusDisplaying(OneSignal.OSInFocusDisplayOption.None);
//        }
//    }

//    public class GetTagsHandler : Java.Lang.Object, OneSignal.IGetTagsHandler
//    {
//        private string _idUser = string.Empty;
//        private string _registrationId = string.Empty;

//        public GetTagsHandler(string idUser, string registrationId)
//        {
//            _idUser = idUser;
//            _registrationId = registrationId;
//        }

//        public void TagsAvailable(JSONObject p0)
//        {
//            if (p0 != null)
//            {
//                var dctnary = JsonConvert.DeserializeObject<Dictionary<string, string>>(p0.ToString());
//                if (dctnary.Count > 0)
//                {
//                    foreach (var element in dctnary)
//                        OneSignal.DeleteTag(element.Key);
//                }
//            }
//            OneSignal.SendTag(_idUser, _registrationId);
//        }
//    }

//    public class NotificationOpenedHandler : Java.Lang.Object, OneSignal.INotificationOpenedHandler
//    {
//        public void NotificationOpened(OSNotificationOpenResult p0)
//        {
//        }
//    }

//    public class NotificationReceivedHandler : Java.Lang.Object, OneSignal.INotificationReceivedHandler
//    {
//        private Context _context;
//        public NotificationReceivedHandler(Context context)
//        {
//            _context = context;
//        }

//        public void NotificationReceived(OSNotification p0)
//        {
//            Log.Debug("OneSignalNotificationService", "NotificationReceived");
//            //Action to do when the app is in focus (update UI etc)
//            //if (p0.IsAppInFocus && _data != null)
//            HandleNotificationData(p0.Payload.Body, p0.Payload.AdditionalData);
//            //Action to do when the app is not in focus (save the notification to execute it later)
//            //else if (_data != null) SaveDataForNextAppResume(_data);
//        }

//        #region ===== Private methods ===========================================

//        /// <summary>
//        /// Handle the notifications from OneSignal that have data passed in order to update the UI
//        /// </summary>
//        private void HandleNotificationData(string methodName, JSONObject data)
//        {
//            string uidSeekios = string.Empty;
//            Tuple<int, int> batteryAndSignal = null;
//            Tuple<double, double, double, double> location = null;
//            DateTime date = DateTime.Now;
//            int userCreditDebitAmount = 0;
//            int seekiosCreditDebitAmount = 0;

//            if (!data.IsNull("uidSeekios"))
//                uidSeekios = data.Get("uidSeekios").ToString();
//            var seekiosName = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios).SeekiosName;

//            if (!data.IsNull("batterySignal"))
//                batteryAndSignal = JsonConvert.DeserializeObject<Tuple<int, int>>(data.Get("batterySignal").ToString());

//            if (!data.IsNull("location"))
//                location = JsonConvert.DeserializeObject<Tuple<double, double, double, double>>(data.Get("location").ToString());

//            if (!data.IsNull("date"))
//            {
//                date = DateExtension.FormatJsonDateToDateTime(data.Get("date").ToString());
//            }

//            if (!data.IsNull("userCreditDebitAmount"))
//                userCreditDebitAmount = JsonConvert.DeserializeObject<int>(data.Get("userCreditDebitAmount").ToString());

//            if (!data.IsNull("seekiosCreditDebitAmount"))
//                seekiosCreditDebitAmount = JsonConvert.DeserializeObject<int>(data.Get("seekiosCreditDebitAmount").ToString());

//            Log.Debug("OneSignalNotificationService", "NotificationHelper.SendNotification");
//            NotificationHelper.SendNotification(_context, methodName, "Coucou");

//            //switch (methodName)
//            //{
//            //    default:
//            //        break;
//            //    case "RefreshCredits":
//            //        App.Locator.Home.OnHasToRefreshCredits(uidSeekios, userCreditDebitAmount, seekiosCreditDebitAmount, date);
//            //        break;
//            //    case "RefreshPosition":
//            //        App.Locator.BaseMap.OnHasToRefreshPosition(uidSeekios, batteryAndSignal, location, date);
//            //        break;
//            //    case "NotifySeekiosOutOfZone":

//            //        var modeZoneTitle = _context.Resources.GetString(Resource.String.modeDefinition_zone);
//            //        var modeZoneOutAlert = _context.Resources.GetString(Resource.String.gcmMessage_modeZoneOutAlert);

//            //        NotificationHelper.SendNotification(_context, modeZoneTitle, string.Format(modeZoneOutAlert, seekiosName), true);
//            //        App.Locator.ModeZone.OnNotifySeekiosOutOfZone(uidSeekios, batteryAndSignal, location, date);
//            //        break;
//            //    case "AddTrackingLocation":
//            //        App.Locator.ModeTracking.OnAddTrackingLocation(uidSeekios, batteryAndSignal, location, date);
//            //        break;
//            //    case "AddNewZoneTrackingLocation":
//            //        App.Locator.ModeZone.OnNewZoneTrackingLocationAdded(uidSeekios, batteryAndSignal, location, date);
//            //        break;
//            //    case "AddNewDontMoveTrackingLocation":
//            //        App.Locator.ModeDontMove.OnNewDontMoveTrackingLocationAdded(uidSeekios, batteryAndSignal, location, date);
//            //        break;
//            //    case "NotifySeekiosMoved":
//            //        var modeDontMoveTitle = _context.Resources.GetString(Resource.String.modeDefinition_dontmove);
//            //        var modeDontMoveAlert = _context.Resources.GetString(Resource.String.gcmMessage_modeDontMoveAlert);

//            //        NotificationHelper.SendNotification(_context, modeDontMoveTitle, string.Format(modeDontMoveAlert, seekiosName), true);

//            //        App.Locator.ModeDontMove.OnNotifySeekiosMoved(uidSeekios, batteryAndSignal);
//            //        break;
//            //    case "InstructionTaken":
//            //        App.Locator.ListSeekios.SeekiosInstructionTaken(uidSeekios, batteryAndSignal, date);
//            //        break;
//            //    case "SOSSent":
//            //        var SOSTitle = _context.Resources.GetString(Resource.String.gcmMessage_SOSTitle);
//            //        var SOSContent = _context.Resources.GetString(Resource.String.gcmMessage_SOSContent);

//            //        NotificationHelper.SendNotification(_context, SOSTitle, string.Format(SOSContent, seekiosName), true);
//            //        break;
//            //    case "SOSLocationSent":
//            //        var SOSLocTitle = _context.Resources.GetString(Resource.String.gcmMessage_SOSLocationTitle);
//            //        var SOSLocContent = _context.Resources.GetString(Resource.String.gcmMessage_SOSLocationContent);

//            //        NotificationHelper.SendNotification(_context, SOSLocTitle, string.Format(SOSLocContent, seekiosName));
//            //        break;
//            //}

//            //if (_messageReceived.StartsWith("[FriendShipRequested]"))
//            //{
//            //    var parameters = _messageReceived.Substring(21);
//            //    var splittedParameters = parameters.Split('|');
//            //    var firstName = splittedParameters[0];
//            //    var lastName = splittedParameters[1];
//            //    var userPicture = splittedParameters[2];

//            //    var friendshipRequestTitle = _context.Resources.GetString(Resource.String.gcmMessage_friendshipRequestTitle);
//            //    var friendshipRequestContent = _context.Resources.GetString(Resource.String.gcmMessage_friendshipRequestContent);

//            //    NotificationHelper.SendNotification(_context, friendshipRequestTitle, string.Format(friendshipRequestContent, firstName, lastName));
//            //    return;
//            //}
//        }
//    }

//    #endregion
//}