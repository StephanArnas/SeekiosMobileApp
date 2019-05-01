using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Enum;
using SeekiosApp.Extension;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class MapViewModelBase : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        protected IDispatchOnUIThread _dispatcherService = null;
        protected Interfaces.IDialogService _dialogService = null;
        protected IDataService _dataService = null;
        protected ISaveDataService _saveDataService = null;
        protected ILocalNotificationService _localNotificationService = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Seekios mode</summary>
        public static ModeDTO Mode { get { return App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault(); } }

        /// <summary>Seekios</summary>
        public static SeekiosDTO Seekios { get; set; }

        /// <summary>Refresh rate in minute</summary>
        public static int RefreshTime { get; set; }

        /// <summary>Map control manager</summary>
        public IMapControlManager MapControlManager { get; set; }

        /// <summary>Return the state of the refresh action : 0 = step 1, 1 = step 2, 2 = step 3</summary>
        public int RefreshPositionStep { get; set; }

        /// <summary>If the page needs to go back</summary>
        public bool IsGoingBack { get; set; }

        /// <summary>Bool that states if the map is in normal mode or satellite mode</summary>
        public bool IsInNormalMode { get; set; }

        /// <summary>List if a Seekios is in alert or not</summary>
        public HashSet<int> LsSeekiosAlertState { get; set; }

        /// <summary>List of seekios that requested a new position (OnDemand)</summary>
        public List<SeekiosOnDemand> LsSeekiosOnDemand { get; set; }

        /// <summary>List of seekios that are in a tracking mode</summary>
        public List<SeekiosOnTracking> LsSeekiosOnTracking { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public MapViewModelBase(IDispatchOnUIThread dispatcherService
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ILocalNotificationService localNotificationService)
        {
            _dispatcherService = dispatcherService;
            _dataService = dataService;
            _dialogService = dialogService;
            _localNotificationService = localNotificationService;
            _saveDataService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ISaveDataService>();
            LsSeekiosAlertState = new HashSet<int>();
            LsSeekiosOnDemand = new List<SeekiosOnDemand>();
            LsSeekiosOnTracking = new List<SeekiosOnTracking>();
            IsInNormalMode = true;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Initialize the map with markers
        /// </summary>
        public virtual void InitMap()
        {
            MapControlManager.InitMap((float)ZoomLevelEnum.MediumZoom, true);

            if (Seekios != null && Seekios.LastKnownLocation_dateLocationCreation.HasValue)
            {
                var isDontMove = Mode != null && Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove;
                var isInAlert = Mode != null && Mode.StatusDefinition_idstatusDefinition != 1;
                if (Seekios.LastKnownLocation_accuracy >= 0 && Seekios.LastKnownLocation_accuracy <= 10)
                {
                    Seekios.LastKnownLocation_accuracy = 0;
                }
                MapControlManager.CreateSeekiosMarkerAsync(Seekios.Idseekios.ToString()
                    , Seekios.SeekiosName
                    , Seekios.SeekiosPicture
                    , Seekios.LastKnownLocation_dateLocationCreation.Value
                    , Seekios.LastKnownLocation_latitude
                    , Seekios.LastKnownLocation_longitude
                    , Seekios.LastKnownLocation_accuracy
                    , isDontMove
                    , isInAlert);
            }
        }

        /// <summary>
        /// Refresh seekios position  
        /// </summary>
        public async Task<bool> RefreshSeekiosPosition()
        {
            var seekiosToRefresh = App.Locator.DetailSeekios.SeekiosSelected;
            try
            {
                // If the seekios is already in refresh state
                if (LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == seekiosToRefresh.Idseekios)) return false;

                // If user has enough credits
                int creditsDispo = 0;
                if (!int.TryParse(Helper.CreditHelper.TotalCredits, out creditsDispo)) return false;
                if (creditsDispo <= 0)
                {
                    var msg = Resources.UserNoRequestLeft;
                    var title = Resources.UserNoRequestLeftTitle;
                    await _dialogService.ShowMessage(msg, title);
                    return false;
                }

                // If the seekios is in power saving, cancel the request
                if (App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == seekiosToRefresh.Idseekios).IsInPowerSaving == true)
                {
                    await _dialogService.ShowMessage(Resources.PowerSavingOn
                        , Resources.PowerSavingOnTitle
                        , Resources.PowerSavingTuto
                        , Resources.Close, (result2) => { if (result2) App.Locator.Parameter.GoToTutorialPowerSaving(); });
                    return false;
                }

                // Make the request in BDD
                _dialogService.ShowLoadingLayout();
                var result = await _dataService.RefreshSeekiosLocation(seekiosToRefresh.Idseekios);
                if (result != 1)
                {
                    var msg = Resources.RefreshSeekiosFailed;
                    var title = Resources.RefreshSeekiosFailedTitle;
                    _dispatcherService.Invoke(async () => await _dialogService.ShowMessage(msg, title));
                }
                _dialogService.HideLoadingLayout();

                // Udpate the seekios 
                var index = App.CurrentUserEnvironment.LsSeekios.IndexOf(App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == seekiosToRefresh.Idseekios));
                App.CurrentUserEnvironment.LsSeekios[index].DateLastOnDemandRequest = DateTime.Now;
                App.CurrentUserEnvironment.LsSeekios[index].HasGetLastInstruction = false;
                App.Locator.DetailSeekios.SeekiosSelected = App.CurrentUserEnvironment.LsSeekios[index];

                // Add the new seekios is the refresh seekios list
                AddSeekiosOnDemand(seekiosToRefresh, DateTime.Now.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND));

                // Raise event for update UI
                OnSeekiosRefreshRequestSent?.Invoke(null, null);
                return true;
            }

            catch (TimeoutException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (WebException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (Exception)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.UnexpectedError
                    , Resources.UnexpectedErrorTitle
                    , Resources.Close, null));
            }
            _dialogService.HideLoadingLayout();
            return false;
        }

        /// <summary>
        /// The user asked a refresh seekios position on other device
        /// </summary>
        public void RefreshSeekiosPositionSignalR(string uidDevice, string idSeekiosStr)
        {
            if (string.IsNullOrEmpty(idSeekiosStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idSeekios = 0;
            if (int.TryParse(idSeekiosStr, out idSeekios))
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == idSeekios);
                if (seekios != null)
                {
                    // If the seekios is already in refresh state
                    if (LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == seekios.Idseekios)) return;

                    // Udpate the seekios 
                    var index = App.CurrentUserEnvironment.LsSeekios.IndexOf(App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == seekios.Idseekios));
                    App.CurrentUserEnvironment.LsSeekios[index].DateLastOnDemandRequest = DateTime.Now;
                    App.CurrentUserEnvironment.LsSeekios[index].HasGetLastInstruction = false;
                    App.Locator.DetailSeekios.SeekiosSelected = App.CurrentUserEnvironment.LsSeekios[index];

                    if (_dispatcherService != null)
                    _dispatcherService.Invoke(() =>
                    {
                        // Add the new seekios is the refresh seekios list
                        AddSeekiosOnDemand(seekios, DateTime.Now.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND));

                        // Raise event for update UI
                        App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                        App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                        OnSeekiosRefreshRequestSent?.Invoke(null, null);
                        App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                    });
                }
            }
        }

        /// <summary>
        /// Action performed when a seekios sends his location in response to a refresh request
        /// </summary>
        /// location item 1 : latitude
        /// location item 2 : longitude
        /// location item 3 : altitude
        /// location item 4 : accuracy
        public void OnDemandPositionReceived(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , Tuple<double, double, double, double> location
            , DateTime dateCommunication)
        {
            if (string.IsNullOrEmpty(uidSeekios)) return;
            var dateLocation = App.CurrentUserEnvironment.ServerCurrentDateTime;
            // Get the concerned seekios 
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;
            // Update seekios data
            double latitude = location.Item1;
            double longitude = location.Item2;
            double altitude = location.Item3;
            double accuracy = location.Item4;
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.LastKnownLocation_accuracy = accuracy;//accuracy > 0 && accuracy < 10 ? 0 : accuracy;
            seekios.LastKnownLocation_altitude = altitude;
            seekios.LastKnownLocation_latitude = latitude;
            seekios.LastKnownLocation_longitude = longitude;
            seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();
            seekios.DateLastOnDemandRequest = null;
            // Add new location
            var locationToAdd = new LocationDTO
            {
                DateLocationCreation = seekios.LastKnownLocation_dateLocationCreation.Value,
                Latitude = seekios.LastKnownLocation_latitude,
                Longitude = seekios.LastKnownLocation_longitude,
                Altitude = seekios.LastKnownLocation_altitude,
                Accuracy = seekios.LastKnownLocation_accuracy,
                IdLocationDefinition = (int)LocationDefinition.OnDemand,
                Seekios_idseekios = seekios.Idseekios,
            };
            App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);
            var seekiosHistoric = App.Locator.Historic.LsSeekiosLocations?.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios);
            if (seekiosHistoric != null && seekiosHistoric.LsLocations?.Count > 0)
            {
                seekiosHistoric.LsLocations.Add(locationToAdd);
            }

            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
            // Update the seekios in the on demand list
            var seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == seekios.Idseekios);
                if (seekiosOnDemand != null)
                {
                    seekiosOnDemand.Timer.Stop();
                    seekiosOnDemand.OnSuccess?.Invoke();
                    App.Locator.Map.LsSeekiosOnDemand.Remove(seekiosOnDemand);
                }
            // Update seekios information everywhere in the app
            App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
            });
        }

        /// <summary>
        /// Add a seekios to the list OnDemand
        /// Start the count down
        /// </summary>
        public void AddSeekiosOnDemand(SeekiosDTO seekios, DateTime dateEndRefreshTimer)
        {
            // setup the seekios on demand
            var seekiosOnDemand = new SeekiosOnDemand()
            {
                Seekios = seekios,
                DateEndRefreshTimer = dateEndRefreshTimer
            };
            // setup the timer
            var timer = new Timers.Timer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.CountDown = (dateEndRefreshTimer - DateTime.Now).TotalSeconds;
            timer.Tick = async () =>
            {
                if (timer.CountDown <= 0)
                {
                    timer.Stop();
                    seekiosOnDemand.OnFailed?.Invoke();
                    App.Locator.Map.LsSeekiosOnDemand.Remove(seekiosOnDemand);
                    var dialogService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<Interfaces.IDialogService>();
                    await dialogService.ShowMessage(string.Format(Resources.OnDemandMessageFailedBody
                        , seekios.SeekiosName)
                        , Resources.OnDemandMessageFailedTitle);
                }
                else
                {
                    timer.UpdateUI?.Invoke();
                    timer.CountDown--;
                }
            };
            seekiosOnDemand.Timer = timer;
            // add the seekios in the list that contains all the seekios in refreshing state (OnDemand)
            App.Locator.Map.LsSeekiosOnDemand.Add(seekiosOnDemand);
            // start the timer
            timer.Start();
        }

        /// <summary>
        /// Add a seekios to the list OnDemand
        /// Start the count down
        /// </summary>
        public void AddSeekiosOnTracking(SeekiosDTO seekios, ModeDTO mode)
        {
            // if a seekios on tracking is already present (protection against double entry)
            var seekiosOnTrackingAlreadyCreated = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == seekios.Idseekios);
            if (seekiosOnTrackingAlreadyCreated != null)
            {
                if (seekiosOnTrackingAlreadyCreated.Timer != null && seekiosOnTrackingAlreadyCreated.Timer.IsRunning)
                {
                    seekiosOnTrackingAlreadyCreated.Timer.Stop();
                }
                App.Locator.Map.LsSeekiosOnTracking.RemoveAll(x => x.Seekios.Idseekios == seekios.Idseekios);
            }
            // setup the seekios on demand
            var seekiosOnTracking = new SeekiosOnTracking()
            {
                Seekios = seekios,
                Mode = mode
            };
            // get the refresh time
            var refreshTime = 0;
            var param = mode.Trame.Split(';');
            if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
            {
                if (param?.Count() > 1)
                {
                    if (int.TryParse(param[1], out refreshTime))
                    {
                        seekiosOnTracking.RefreshTime = refreshTime;
                    }
                }
            }
            else
            {
                if (param?.Count() > 1)
                {
                    if (int.TryParse(param[1], out refreshTime))
                    {
                        seekiosOnTracking.RefreshTime = refreshTime;
                    }
                }
            }
            seekiosOnTracking.Timer = new Timers.Timer();
            seekiosOnTracking.Timer.Interval = new TimeSpan(0, 0, 1);
            // add the seekios in the list that contains all the seekios in refreshing state (OnDemand)
            App.Locator.Map.LsSeekiosOnTracking.Add(seekiosOnTracking);
        }

        /// <summary>
        /// Initialize alert state
        /// </summary>
        public void InitialiseLsAlertState()
        {
            foreach (var mode in App.CurrentUserEnvironment.LsMode)
            {
                if (mode.StatusDefinition_idstatusDefinition == 2)
                {
                    App.Locator.BaseMap.LsSeekiosAlertState.Add(mode.Seekios_idseekios);
                }
            }
        }

        /// <summary>
        /// Notify a new SOS location is comming
        /// </summary>
        public void NotifySOSLocationReceived(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , Tuple<double, double, double, double> location
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Update seekios data
            double latitude = location.Item1;
            double longitude = location.Item2;
            double altitude = location.Item3;
            double accuracy = location.Item4;
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.LastKnownLocation_accuracy = accuracy;
            seekios.LastKnownLocation_altitude = altitude;
            seekios.LastKnownLocation_latitude = latitude;
            seekios.LastKnownLocation_longitude = longitude;
            seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();

            // Add new location
            var locationToAdd = new LocationDTO
            {
                DateLocationCreation = seekios.LastKnownLocation_dateLocationCreation.Value,
                Latitude = seekios.LastKnownLocation_latitude,
                Longitude = seekios.LastKnownLocation_longitude,
                Altitude = seekios.LastKnownLocation_altitude,
                Accuracy = seekios.LastKnownLocation_accuracy,
                IdLocationDefinition = (int)LocationDefinition.SOS
            };
            App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                _localNotificationService.SendNotification(seekios
                    , Resources.NotificationSOSPositionTitle
                    , string.Format(Resources.NotificationSOSPositionContent
                        , seekios.SeekiosName
                        , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                    , true);
            });
        }

        /// <summary>
        /// Save the state of the map (satellite / plan)
        /// </summary>
        public void SaveDataChangeMap(bool isNormal)
        {
            _saveDataService.SaveData(App.MapChange, isNormal);
        }

        #endregion

        #region ===== Event =======================================================================

        public delegate void SeekiosHasRefreshedBadSeekios(int seekiosId);

        public static event EventHandler OnSeekiosRefreshRequestSent;

        #endregion
    }
}