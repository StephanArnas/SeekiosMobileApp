using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Org.Json;
using Newtonsoft.Json;
using SeekiosApp.Extension;
using Com.OneSignal.Android;
using SeekiosApp.Services;
using SeekiosApp.Model.DTO;
using System.Threading.Tasks;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.Helper
{
    public class OneSignalHelper
    {
        public class IdsAvailableHandler : Java.Lang.Object, OneSignal.IIdsAvailableHandler
        {
            public void IdsAvailable(string oneSignalUserId, string registrationId)
            {
                var user = App.CurrentUserEnvironment.User;
                var idUser = user.IdUser.ToString();
                var credentials = string.Format("{0}/{1}/{2}", user.LastName, user.FirstName, user.Email);
                //Sync email
                OneSignal.SyncHashedEmail(user.Email);
                //Create user tags
                OneSignal.GetTags(new GetTagsHandler(idUser, credentials));
                //Silent notification
                OneSignal.SetInFocusDisplaying(OneSignal.OSInFocusDisplayOption.None);
            }
        }

        public class GetTagsHandler : Java.Lang.Object, OneSignal.IGetTagsHandler
        {
            private string _idUser = string.Empty;
            private string _credentials = string.Empty;

            public GetTagsHandler(string idUser, string credentials)
            {
                _idUser = idUser;
                _credentials = credentials;
            }

            public void TagsAvailable(JSONObject p0)
            {
                if (p0 != null)
                {
                    var dctnary = JsonConvert.DeserializeObject<Dictionary<string, string>>(p0.ToString());
                    if (dctnary.Count > 0)
                    {
                        foreach (var element in dctnary)
                            OneSignal.DeleteTag(element.Key);
                    }
                }
                OneSignal.SendTag(_idUser + (DataService.UseStaging ? "s" : "p"), _credentials);
                //A quoi ça sert???
                var device = App.CurrentUserEnvironment.Device;
                OneSignal.SendTag(device.UidDevice, _credentials);
            }
        }

        public class NotificationOpenedHandler : Java.Lang.Object, OneSignal.INotificationOpenedHandler
        {
            public void NotificationOpened(OSNotificationOpenResult p0)
            {
                if (p0 != null && p0.Notification != null && p0.Notification.Payload != null)
                {
                    var data = p0.Notification.Payload.AdditionalData;
                    if (data != null) HandleNotificationOpened(data);
                }
            }

            private void HandleNotificationOpened(JSONObject data)
            {
                string methodName = string.Empty;
                string uidSeekios = string.Empty;

                if (!data.IsNull("uidSeekios"))
                    uidSeekios = data.Get("uidSeekios").ToString();

                if (!data.IsNull("methodName"))
                    methodName = data.Get("methodName").ToString();

                if (methodName == "RefreshCredits")
                {
                    //go to credits
                }
                else
                {
                    var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                    if (seekios != null)
                    {
                        App.Locator.DetailSeekios.SeekiosSelected = seekios;
                        App.Locator.DetailSeekios.GoToMap(seekios);
                    }
                }
            }
        }

        public class NotificationReceivedHandler : Java.Lang.Object, OneSignal.INotificationReceivedHandler
        {
            private JSONObject _data;
            private Context _context;

            public NotificationReceivedHandler(Context context) { _context = context; }

            public void NotificationReceived(OSNotification p0)
            {
                if (p0 == null || p0.Payload == null) return;
                _data = p0.Payload.AdditionalData;

                //Action to do when the app is in focus (update UI etc)
                //if (p0.IsAppInFocus && _data != null)
                if (_data != null)
                    HandleNotificationData(_data);
                //Action to do when the app is not in focus (save the notification to execute it later)
                //else if (_data != null) SaveDataForNextAppResume(_data);
            }


            #region ===== Private methods ===========================================

            /// <summary>
            /// Handle the notifications from OneSignal that have data passed in order to update the UI
            /// </summary>
            private void HandleNotificationData(JSONObject data)
            {
                string uidSeekios = string.Empty;
                string methodName = string.Empty;
                Tuple<int, int> batteryAndSignal = null;
                Tuple<double, double, double, double> location = null;
                DateTime date = DateTime.Now;
                int userCreditDebitAmount = 0;
                int seekiosCreditDebitAmount = 0;
                int idAlert = 0;
                int idMode = 0;

                if (!data.IsNull("uidSeekios"))
                    uidSeekios = data.Get("uidSeekios").ToString();

                string seekiosName = string.Empty;
                if (null != uidSeekios && !string.Empty.Equals(uidSeekios))
                {
                    var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                    if (seekios != null) seekiosName = seekios.SeekiosName;
                }

                if (!data.IsNull("batterySignal"))
                    batteryAndSignal = JsonConvert.DeserializeObject<Tuple<int, int>>(data.Get("batterySignal").ToString());

                if (!data.IsNull("location"))
                    location = JsonConvert.DeserializeObject<Tuple<double, double, double, double>>(data.Get("location").ToString());

                if (!data.IsNull("date"))
                    date = DateExtension.FormatJsonDateToDateTime(data.Get("date").ToString());

                if (!data.IsNull("userCreditDebitAmount"))
                    userCreditDebitAmount = JsonConvert.DeserializeObject<int>(data.Get("userCreditDebitAmount").ToString());

                if (!data.IsNull("seekiosCreditDebitAmount"))
                    seekiosCreditDebitAmount = JsonConvert.DeserializeObject<int>(data.Get("seekiosCreditDebitAmount").ToString());

                if (!data.IsNull("methodName"))
                    methodName = data.Get("methodName").ToString();

                if (!data.IsNull("idAlert"))
                    idAlert = JsonConvert.DeserializeObject<int>(data.Get("idAlert").ToString());

                if (!data.IsNull("idMode"))
                    idMode = JsonConvert.DeserializeObject<int>(data.Get("idMode").ToString());


                if (!string.IsNullOrEmpty(methodName))
                {
                    SeekiosDTO seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);

                    switch (methodName)
                    {
                        case "RefreshCredits":
                            App.Locator.ListSeekios.OnHasToRefreshCredits(uidSeekios, userCreditDebitAmount, seekiosCreditDebitAmount, date);
                            break;
                        case "RefreshPosition":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.BaseMap.OnDemandPositionReceived(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "InstructionTaken":
                            if (batteryAndSignal != null)
                                App.Locator.ListSeekios.SeekiosInstructionTaken(uidSeekios, batteryAndSignal, date);
                            break;
                        case "NotifySeekiosOutOfZone":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.ModeZone.OnNotifySeekiosOutOfZone(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "AddTrackingLocation":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.ModeTracking.OnAddTrackingLocation(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "AddNewZoneTrackingLocation":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.ModeZone.OnNewZoneTrackingLocationAdded(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "AddNewDontMoveTrackingLocation":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.ModeDontMove.OnNewDontMoveTrackingLocationAdded(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "NotifySeekiosMoved":
                            if (batteryAndSignal != null)
                                App.Locator.ModeDontMove.OnNotifySeekiosMoved(uidSeekios, batteryAndSignal, date);
                            break;
                        case "SOSSent":
                            if (seekios != null)
                                App.Locator.ListSeekios.OnSOSSentReceived(uidSeekios, batteryAndSignal, date);
                            break;
                        case "SOSLocationSent":
                            if (batteryAndSignal != null && location != null)
                                App.Locator.BaseMap.NotifySOSLocationReceived(uidSeekios, batteryAndSignal, location, date);
                            break;
                        case "CriticalBattery":
                            if (seekios != null)
                                App.Locator.ListSeekios.OnCriticalBatteryReceived(uidSeekios, batteryAndSignal, date);
                            break;
                        case "PowerSavingDisabled":
                            if (seekios != null)
                                App.Locator.ListSeekios.OnPowerSavingDisabledReceived(uidSeekios, batteryAndSignal, date);
                            break;
                    }
                }
            }
        }

        #endregion

    }
}