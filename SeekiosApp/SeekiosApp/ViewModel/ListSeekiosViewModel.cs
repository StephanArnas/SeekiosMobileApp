using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Model.DTO;
using System.Collections.Generic;
using System.Linq;
using System;
using SeekiosApp.Interfaces;
using SeekiosApp.Properties;
using SeekiosApp.Model.APP;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR.Client;
using SeekiosApp.Extension;

namespace SeekiosApp.ViewModel
{
    public class ListSeekiosViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private IDataService _dataService = null;
        private ISaveDataService _saveDataService = null;
        private INavigationService _navigationService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDispatchOnUIThread _dispatcherService = null;
        private ILocalNotificationService _localNotificationService = null;
        public bool IsNotificationAvailable = true;

        #endregion

        #region ===== Properties ==================================================================

        public List<SeekiosDTO> LsSeekios
        {
            get
            {
                // List sorted by name and by owning (seekios shared by friends are at the end in blue)
                return App.CurrentUserEnvironment.LsSeekios
                    //.Where(el => el.SeekiosName.ToUpper().Contains(SeekiosNameSearching.ToUpper()))
                    .OrderBy(el => el.SeekiosName)
                    .OrderBy(el => el.User_iduser == App.CurrentUserEnvironment.User.IdUser ? 0 : 1).ToList();
            }
        }

        public bool ActivityNeedsUIToBeUpdated { get; set; }

        public bool IsNotFromLogin { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ListSeekiosViewModel(IDataService dataService
            , Interfaces.IDialogService dialogService
            , ISaveDataService saveDataService
            , IDispatchOnUIThread dispatcherService
            , ILocalNotificationService localNotificationService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _saveDataService = saveDataService;
            _dispatcherService = dispatcherService;
            _localNotificationService = localNotificationService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            ActivityNeedsUIToBeUpdated = false;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region NAVIGATION

        public void GoToAddSeekios()
        {
            App.Locator.AddSeekios.IsAdding = true;
            _navigationService.NavigateTo(App.ADD_SEEKIOS_PAGE);
        }

        public void GoToSeekios()
        {
            _navigationService.NavigateTo(App.LIST_SEEKIOS_PAGE);
        }

        public void GoToSeekiosDetail()
        {
            _navigationService.NavigateTo(App.DETAIL_SEEKIOS_PAGE);
        }

        #endregion

        #region REALTIME

        public void SeekiosInstructionTaken(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Update the seekios
            seekios.HasGetLastInstruction = true;
            if (batteryAndSignal != null)
            {
                seekios.BatteryLife = batteryAndSignal.Item1;
                seekios.SignalQuality = batteryAndSignal.Item2;
            }
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.IsRefreshingBattery = false;

            // Update the mode 
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(x => x.Seekios_idseekios == seekios.Idseekios);
            if (mode != null)
            {
                mode.DateModeActivation = DateTime.Now;
            }

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
            });
        }

        public void OnHasToRefreshCredits(string uidSeekios
            , int userCreditsDebitAmount
            , int seekiosCreditsDebitAmount
            , DateTime dateCommunication)
        {
            // Update the seekios & user
            App.CurrentUserEnvironment.User.RemainingRequest += userCreditsDebitAmount;
            if (!string.Empty.Equals(uidSeekios))
            {
                // Get the concerned seekios 
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                if (seekios != null) seekios.FreeCredit += seekiosCreditsDebitAmount;
            }

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseRemainingRequestChangedEverywhere();
            });
        }

        public void OnSOSSentReceived(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Update the seekios & mode
            seekios.IsRefreshingBattery = false;
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.DateLastSOSSent = dateCommunication.ToLocalTime();
            seekios.IsLastSOSRead = false;

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                _localNotificationService.SendNotification(seekios
                    , Resources.NotificationSOSAlertTitle
                    , string.Format(Resources.NotificationSOSAlertContent
                        , seekios.SeekiosName
                        , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                    , true);
            });
        }

        public void OnPowerSavingDisabledReceived(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Update the seekios & mode
            seekios.IsInPowerSaving = false;
            seekios.IsRefreshingBattery = false;
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                //_localNotificationService.SendNotification(seekios
                //    , Resources.NotificationPowerSavingTitle
                //    , string.Format(Resources.NotificationPowerSavingContent
                //        , seekios.SeekiosName
                //        , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                //    , true);
            });
        }

        public void OnCriticalBatteryReceived(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Update the seekios
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.IsRefreshingBattery = false;

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                _localNotificationService.SendNotification(seekios
                    , Resources.NotificationCriticalBatteryTitle
                    , string.Format(Resources.NotificationCriticalBatteryContent
                        , seekios.SeekiosName
                        , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                    , true);
            });
        }

        public async void SubscribeToSignalR()
        {
            if (App.CurrentUserEnvironment == null || App.CurrentUserEnvironment.User == null) return;
            if (App.DeviceIsConnectedToInternet)
            {
                if (App.SeekiosSignalR == null)
                {
                    App.SeekiosSignalR = new HubConnection("http://seekiossignalr.azurewebsites.net");
                    App.SeekiosSignalR.Headers.Add("userId", App.CurrentUserEnvironment.User.IdUser.ToString());

                    // FROM SES

                    // CreditsHub : Refresh Credit
                    App.SeekiosSignalR.CreateHubProxy("CreditsHub").On<string, int, int, DateTime>("RefreshCredits", OnHasToRefreshCredits);
                    // SeekiosHub : GSI Instruction Taken
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, DateTime>("InstructionTaken", SeekiosInstructionTaken);
                    // SeekiosHub : Refresh Position
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("RefreshPosition", App.Locator.BaseMap.OnDemandPositionReceived);
                    // SeekiosHub : Critical Battery
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, DateTime>("CriticalBattery", OnCriticalBatteryReceived);
                    // SeekiosHub : Power Saving Disabled
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, DateTime>("PowerSavingDisabled", OnPowerSavingDisabledReceived);
                    // SeekiosHub : SOS Sent
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, DateTime>("SOSSent", OnSOSSentReceived);
                    // SeekiosHub : SOS Location Sent
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("SOSLocationSent", App.Locator.BaseMap.NotifySOSLocationReceived);
                    // TrackingHub : Add Tracking Location
                    App.SeekiosSignalR.CreateHubProxy("TrackingHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("AddTrackingLocation", App.Locator.ModeTracking.OnAddTrackingLocation);
                    // ZoneHub : Notify seekios out of zone
                    App.SeekiosSignalR.CreateHubProxy("ZoneHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("NotifySeekiosOutOfZone", App.Locator.ModeZone.OnNotifySeekiosOutOfZone);
                    // ZoneHub : Add New Zone Tracking Location
                    App.SeekiosSignalR.CreateHubProxy("ZoneHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("AddNewZoneTrackingLocation", App.Locator.ModeZone.OnNewZoneTrackingLocationAdded);
                    // DontMoveHub : Notify seekios out of zone
                    App.SeekiosSignalR.CreateHubProxy("DontMoveHub").On<string, Tuple<int, int>, DateTime>("NotifySeekiosMoved", App.Locator.ModeDontMove.OnNotifySeekiosMoved);
                    // DontMoveHub : Add New Don't Move Tracking Location
                    App.SeekiosSignalR.CreateHubProxy("DontMoveHub").On<string, Tuple<int, int>, Tuple<double, double, double, double>, DateTime>("AddNewDontMoveTrackingLocation", App.Locator.ModeDontMove.OnNewDontMoveTrackingLocationAdded);

                    // FROM SEEKIOS SERVICE

                    // Update User
                    App.SeekiosSignalR.CreateHubProxy("UserHub").On<string, string>("UpdateUser", App.Locator.Parameter.UpdateUserSignalR);
                    // Insert seekios
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("InsertSeekios", App.Locator.AddSeekios.InsertSeekiosSignalR);
                    // Update seekios
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("UpdateSeekios", App.Locator.AddSeekios.UpdateSeekiosSignalR);
                    // Delete seekios
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("DeleteSeekios", App.Locator.DetailSeekios.DeleteSeekiosSignalR);
                    // Refresh seekios Battery Level
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("RefreshSeekiosBatteryLevel", App.Locator.DetailSeekios.RequestBatteryLevelSignalR);
                    // Refresh seekios Location
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("RefreshSeekiosLocation", App.Locator.BaseMap.RefreshSeekiosPositionSignalR);
                    // Insert Mode Tracking
                    App.SeekiosSignalR.CreateHubProxy("TrackingHub").On<string, string>("InsertModeTracking", App.Locator.ModeSelection.SelectModeSignalR);
                    // Insert Mode Zone
                    App.SeekiosSignalR.CreateHubProxy("ZoneHub").On<string, string>("InsertModeZone", App.Locator.ModeSelection.SelectModeSignalR);
                    // Insert Mode Don't Move
                    App.SeekiosSignalR.CreateHubProxy("DontMoveHub").On<string, string>("InsertModeDontMove", App.Locator.ModeSelection.SelectModeSignalR);
                    // Delete Mode
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("DeleteMode", App.Locator.ModeSelection.DeleteModeSignalR);
                    // Alert SOS Has Been Read
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string>("AlertSOSHasBeenRead", App.Locator.DetailSeekios.NotifyAlertSOSHasBeenReadSignalR);
                    // Insert & Update Alert SOS
                    App.SeekiosSignalR.CreateHubProxy("SeekiosHub").On<string, string, string>("InsertAlertSOSWithRecipient", App.Locator.AlertSOS.InsertOrUpdateAlertSOSSignalR);
                }

                try
                {
                    await App.SeekiosSignalR.Start();
                }
                catch (Exception)
                {
                    await _dialogService.ShowMessage(Resources.SignalRNotSubscribeContent
                        , Resources.SignalRNotSubscribeTitle);
                }
            }
        }

        public void MethodeTestSignalR(string uidSeekios
            , int userCreditsDebitAmount
            , int seekiosCreditsDebitAmount
            , DateTime date)
        {
            var dispatcher = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IDispatchOnUIThread>();
            if (dispatcher != null)
            {
                dispatcher.Invoke(async () =>
                {
                    await _dialogService.ShowMessage("uidSeekios " + uidSeekios + " userDebit: " + userCreditsDebitAmount + " seekiosDebit: " + seekiosCreditsDebitAmount, string.Empty);
                });
            }
        }

        #endregion

        #region RELOAD DATA

        public async void LoadUserEnvironment(string deviceModel
            , string platform
            , string version
            , string uniqueDeviceId
            , string countryCode)
        {
            try
            {
                App.CurrentUserEnvironment = await _dataService.GetUserEnvironment(App.Locator.Login.VersionApplication
                    , platform
                    , deviceModel
                    , version
                    , uniqueDeviceId
                    , countryCode);
                App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;

            }
            catch (Exception)
            {
                await _dialogService.ShowMessage(Resources.LoadUserEnvironmentContent
                    , Resources.LoadUserEnvironmentTitle);
            }
        }

        #endregion

        #region POPUP

        public async void PopupRelaodCreditMonthly()
        {
            RelaodCreditMonthly hasBeenRead = null;
            if (!_saveDataService.Contains(App.NeedToDisplayReloadCreditMonthly))
            {
                hasBeenRead = new RelaodCreditMonthly() { HasBeenRead = true, DateHasBeenReand = DateTime.Now };
                _saveDataService.SaveData(App.NeedToDisplayReloadCreditMonthly, JsonConvert.SerializeObject(hasBeenRead, new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                }));
            }
            else
            {
                var json = _saveDataService.GetData(App.NeedToDisplayReloadCreditMonthly);
                hasBeenRead = JsonConvert.DeserializeObject<RelaodCreditMonthly>(json, new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                });
            }

            int currentMonth = (DateTime.Now.Month == 1 && hasBeenRead.DateHasBeenReand.Month == 12 ? 13 : DateTime.Now.Month);
            if (DateTime.Now.Day >= 16 && DateTime.Now.Hour > 6 && currentMonth > hasBeenRead.DateHasBeenReand.Month)
            {
                if (App.CurrentUserEnvironment.LsSeekios?.Count > 0)
                {
                    await _dialogService.ShowPopupCredit(Resources.RelaodCreditMonthlyTitle
                        , Resources.RelaodCreditMonthlyContent);
                }
                hasBeenRead.DateHasBeenReand = DateTime.Now;
                _saveDataService.SaveData(App.NeedToDisplayReloadCreditMonthly, JsonConvert.SerializeObject(hasBeenRead, new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                }));
            }
        }

        public async void PopupNotificationNotAvailable(Action navigationToTuto)
        {
            if (!IsNotificationAvailable)
            {
                await _dialogService.ShowMessage(Resources.NotificationErrorDescr
                    , Resources.NotificationErrorTitle
                    , Resources.NotificationErrorButtonTuto
                    , Resources.NotificationErrorButtonClose
                    , (result) => { if (result) navigationToTuto?.Invoke(); });
            }
        }

        #endregion

        #endregion
    }
}
