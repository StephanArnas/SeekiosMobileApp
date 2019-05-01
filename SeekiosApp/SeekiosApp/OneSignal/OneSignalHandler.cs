using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeekiosApp.Model.APP.OneSignal
{
    public class OneSignalHandler
    {
        #region ===== Attributes ==================================================================

        private const string TAG = "OneSignalHandler";

        #endregion

        #region ===== Handler =====================================================================

        public OneSignalBuilder.IdsAvailableCallback IdsAvailableCallback()
        {
            return delegate (string playerID, string pushToken)
            {
            };
        }

        public OneSignalBuilder.TagsReceived TagsReceived(string playerID, string pushToken)
        {
            return delegate (Dictionary<string, object> tags)
            {

            };
        }

        public OneSignalBuilder.NotificationReceived NotificationReceivedDelegate()
        {
            return delegate (OSNotificationApp notification)
            {
                //string uidSeekios = string.Empty;
                //Tuple<int, int> batteryAndSignal = null;
                //Tuple<double, double, double, double> location = null;
                //DateTime date = DateTime.Now;
                //string deserialization = string.Empty;
                //int userCreditDebitAmount = 0;
                //int seekiosCreditDebitAmount = 0;
                //int idAlert = 0;
                //int idMode = 0;
                //string methodName = string.Empty;
                //string seekiosName = string.Empty;

                //if (notification == null || notification.Payload == null || notification.Payload.AdditionalData == null) return;

                //var datas = notification.Payload.AdditionalData;

                //foreach (var data in datas)
                //{
                //    if (data.Key.Trim() == "uidSeekios")
                //    {
                //        uidSeekios = data.Value.ToString().Trim();
                //    }
                //    else if (data.Key.Trim() == "batterySignal")
                //    {
                //        deserialization = data.Value.ToString().Trim().Replace(';', ',').Replace('=', ':');
                //        var result = JsonConvert.DeserializeObject<BatteryDeserialization>(deserialization);
                //        batteryAndSignal = new Tuple<int, int>(result.Item1, result.Item2);
                //    }
                //    else if (data.Key.Trim() == "location")
                //    {
                //        deserialization = data.Value.ToString().Trim().Replace(';', ',').Replace('=', ':');
                //        var result = JsonConvert.DeserializeObject<LocationDeserialization>(deserialization);
                //        //location = new Tuple<double, double, double, double>(result.Item1, result.Item2, result.Item3, result.Item4 >= 0 && location.Item4 <= 10 ? 0 : location.Item4);
                //        location = new Tuple<double, double, double, double>(result.Item1, result.Item2, result.Item3, result.Item4);
                //    }
                //    else if (data.Key.Trim() == "date")
                //    {
                //        deserialization = data.Value.ToString().Trim().TrimEnd('/');
                //        deserialization = "{\"Date\": \"\\" + deserialization + "\\/\"}";
                //        JsonSerializerSettings settings = new JsonSerializerSettings
                //        {
                //            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                //            Formatting = Formatting.Indented
                //        };
                //        date = JsonConvert.DeserializeObject<DateDeserialization>(deserialization, settings).Date.ToLocalTime();
                //    }
                //    else if (data.Key.Trim() == "userCreditDebitAmount")
                //    {
                //        deserialization = data.Value.ToString().Trim();
                //        int.TryParse(deserialization, out userCreditDebitAmount);
                //    }
                //    else if (data.Key.Trim() == "seekiosCreditDebitAmount")
                //    {
                //        deserialization = data.Value.ToString().Trim();
                //        int.TryParse(deserialization, out seekiosCreditDebitAmount);
                //    }
                //    else if (data.Key.Trim() == "methodName")
                //    {
                //        methodName = data.Value.ToString().Trim();
                //    }
                //    else if (data.Key.Trim() == "idAlert")
                //    {
                //        deserialization = data.Value.ToString().Trim();
                //        int.TryParse(deserialization, out idAlert);
                //    }
                //    else if (data.Key.Trim() == "idMode")
                //    {
                //        deserialization = data.Value.ToString().Trim();
                //        int.TryParse(deserialization, out idMode);
                //    }
                //}

                //var seekios = App.CurrentUserEnvironment.LsSeekios.Where(el => el.UIdSeekios == uidSeekios).FirstOrDefault();

                //if (!string.IsNullOrEmpty(uidSeekios))
                //{
                //    seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                //    if (seekios != null)
                //    {
                //        seekiosName = seekios.SeekiosName;
                //    }
                //}

                //switch (methodName)
                //{
                //    case "RefreshCredits":
                //        App.Locator.ListSeekios.OnHasToRefreshCredits(uidSeekios
                //            , userCreditDebitAmount
                //            , seekiosCreditDebitAmount
                //            , date);
                //        break;
                //    case "RefreshPosition":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.BaseMap.OnDemandPositionReceived(uidSeekios
                //                , batteryAndSignal
                //                , location
                //                , date);
                //        }
                //        break;
                //    case "InstructionTaken":
                //        if (batteryAndSignal != null)
                //        {
                //            App.Locator.ListSeekios.SeekiosInstructionTaken(uidSeekios
                //                , batteryAndSignal
                //                , date);
                //        }
                //        break;
                //    case "NotifySeekiosOutOfZone":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.ModeZone.OnNotifySeekiosOutOfZone(uidSeekios
                //                , batteryAndSignal
                //                , location
                //                , date);
                //        }
                //        break;
                //    case "NotifySeekiosMoved":
                //        if (batteryAndSignal != null)
                //        {
                //            App.Locator.ModeDontMove.OnNotifySeekiosMoved(uidSeekios
                //                , batteryAndSignal
                //                , date);
                //        }
                //        break;
                //    case "AddTrackingLocation":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.ModeTracking.OnAddTrackingLocation(uidSeekios
                //            , batteryAndSignal
                //            , location
                //            , date);
                //        }
                //        break;
                //    case "AddNewZoneTrackingLocation":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.ModeZone.OnNewZoneTrackingLocationAdded(uidSeekios
                //                , batteryAndSignal
                //                , location
                //                , date);
                //        }
                //        break;
                //    case "AddNewDontMoveTrackingLocation":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.ModeDontMove.OnNewDontMoveTrackingLocationAdded(uidSeekios
                //                , batteryAndSignal
                //                , location
                //                , date);
                //        }
                //        break;
                //    case "SOSSent":
                //        if (seekios != null)
                //        {
                //            App.Locator.ListSeekios.OnSOSSentReceived(uidSeekios
                //                , batteryAndSignal
                //                , date);
                //        }
                //        break;
                //    case "SOSLocationSent":
                //        if (batteryAndSignal != null && location != null)
                //        {
                //            App.Locator.BaseMap.NotifySOSLocationReceived(uidSeekios
                //                , batteryAndSignal
                //                , location
                //                , date);
                //        }
                //        break;
                //    case "CriticalBattery":
                //        if (seekios != null)
                //        {
                //            App.Locator.ListSeekios.OnCriticalBatteryReceived(uidSeekios
                //                , batteryAndSignal
                //                , date);
                //        }
                //        break;
                //    case "PowerSavingDisabled":
                //        if (seekios != null)
                //        {
                //            App.Locator.ListSeekios.OnPowerSavingDisabledReceived(uidSeekios
                //                , batteryAndSignal
                //                , date);
                //        }
                //        break;
                //}
            };
        }

        public OneSignalBuilder.NotificationOpened NotificationOpenedDelegate()
        {
            return delegate (OSNotificationOpenedResultApp result)
            {

            };
        }

        #endregion

        #region ===== Deserialization Classes =====================================================

        public class BatteryDeserialization
        {
            public int Item1 { get; set; }
            public int Item2 { get; set; }
        }

        public class LocationDeserialization
        {
            public double Item1 { get; set; }
            public double Item2 { get; set; }
            public double Item3 { get; set; }
            public double Item4 { get; set; }
        }

        public class DateDeserialization
        {
            public DateTime Date { get; set; }
        }

        public class InputValue
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        #endregion
    }
}