using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using SeekiosApp.Enum;
using SeekiosApp.Model.APP;
using SeekiosApp.Extension;

namespace SeekiosApp.ViewModel
{
    public class ModeDontMoveViewModel : MapViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>List of seekios </summary>
        public HashSet<int> LsSeekiosInTrackingAfterMove { get; set; }

        /// <summary>Zone Tracking mode location list</summary>
        public List<LocationDTO> LsLocations
        {
            get
            {
                if (Seekios == null) return new List<LocationDTO>();
                var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == Seekios.Idseekios);
                if (mode == null) return new List<LocationDTO>();
                return App.CurrentUserEnvironment.LsLocations
                    .Where(el => el.Mode_idmode == mode.Idmode && el.IdLocationDefinition == (int)LocationDefinition.DontMove)
                    .ToList();
            }
        }

        /// <summary>List of the email alerts for the mode don't move</summary>
        public List<AlertWithRecipientDTO> LsAlertsModeDontMove { get; set; }

        public bool IsTrackingSettingEnable { get; set; }

        public TrackingSetting TrackingSetting
        {
            get
            {
                var trackingSetting = App.Locator.ModeSelection.LsTrackingSetting.FirstOrDefault(x => x.IdSeekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios && x.ModeDefinition == ModeDefinitionEnum.ModeDontMove);
                if (trackingSetting == null) return new TrackingSetting() { IsEnable = true, RefreshTime = App.DEFAULT_TRACKING_SETTING };
                else return trackingSetting;
            }
        }

        public bool IsPowerSavingEnabled { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ModeDontMoveViewModel(IDispatchOnUIThread dispatcher
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ILocalNotificationService localNotificationService)
            : base(dispatcher, dataService, dialogService, localNotificationService)
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            _dialogService = dialogService;
            _dataService = dataService;
            _localNotificationService = localNotificationService;
            LsSeekiosInTrackingAfterMove = new HashSet<int>();
            LsAlertsModeDontMove = new List<AlertWithRecipientDTO>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void GoToSecondPage()
        {
            if (_navigationService.CurrentPageKey != App.MODE_DONT_MOVE_PAGE)
            {
                _navigationService.NavigateTo(App.MODE_DONT_MOVE_PAGE);
            }
        }

        public void InitModeDontMove()
        {
            if (Mode == null) return;
            RefreshTime = App.DEFAULT_TRACKING_SETTING;
            DecodeTrame(Mode.Trame);
        }

        public async void InitDontMoveTrackingRouteAsync()
        {
            if (MapControlManager == null) return;
            var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
            // Is there location from the database ? 
            //if (mode != null && LsLocations?.Count < 2)
            //{
            try
            {
                var locations = await _dataService.LocationsByMode(mode.Idmode);
                if (locations?.Count > 0)
                {
                    App.CurrentUserEnvironment.LsLocations.RemoveAll(el => el.Mode_idmode == mode.Idmode);
                    App.CurrentUserEnvironment.LsLocations.AddRange(locations);
                    App.CurrentUserEnvironment.LsLocations.RemoveAll(el => el.Idlocation == 0);
                }
            }
            catch (Exception)
            {
                _dispatcherService.Invoke(async () =>
                {
                    await _dialogService.ShowMessage(Resources.NoDataFromLocationByMode
                    , string.Empty
                    , Resources.NoDataFromLocationByModeButton
                    , null);
                });
            }
            //}
            MapControlManager.CreateRouteBackground(LsLocations);
        }

        private int CalculateModeDontMoveExpectedPositionNumber(ModeDTO mode)
        {
            if (mode == null) return -1;

            DateTime creationDate = mode.DateModeCreation;
            DecodeTrame(mode.Trame);
            int refreshRateMin = RefreshTime;
            double minutesGoneSinceActivation = (DateTime.Now - creationDate).TotalMinutes;

            //on est pas assez précis pour des valeurs de tracking plus élevées je pense... A voir
            int positionExpectedNumber = (int)(minutesGoneSinceActivation / refreshRateMin);

            return positionExpectedNumber;
        }

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Decode the mode Don't Move thread
        /// </summary>
        private void DecodeTrame(string trame)
        {
            var refreshTime = 0;
            if (string.IsNullOrEmpty(trame) || !int.TryParse(trame, out refreshTime))
            {
                return;
            }
            else
            {
                if (!App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                RefreshTime = refreshTime;
            }
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>Event fired when a seekios has moved</summary>
        public event SeekiosMovedEvent SeekiosMovedNotified;
        public delegate void SeekiosMovedEvent(int idSeekios);

        /// <summary>Event fired when a new tracking location is added</summary>
        public event NewDontMoveTrackingLocationAddedEvent NewDontMoveTrackingLocationAddedNotified;
        public delegate void NewDontMoveTrackingLocationAddedEvent(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);

        #endregion

        #region ===== Handlers ====================================================================

        /// <summary>
        /// Action performed when a seekios has moved
        /// </summary>
        public void OnNotifySeekiosMoved(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , DateTime dateCommunication)
        {
            // Get the concerned seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Get the concerned mode
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
            if (mode == null) return;

            // Update the seekios & mode
            mode.CountOfTriggeredAlert++;
            mode.StatusDefinition_idstatusDefinition = 3;
            mode.LastTriggeredAlertDate = App.CurrentUserEnvironment.ServerCurrentDateTime;
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;

            // Add the seekios in the list to display the tracking timer
            var refreshTime = 0;
            if (int.TryParse(mode.Trame, out refreshTime) && refreshTime > 0)
            {
                App.Locator.BaseMap.AddSeekiosOnTracking(seekios, mode);
            }

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                SeekiosMovedNotified?.Invoke(seekios.Idseekios);
            _localNotificationService.SendNotification(seekios
                , Resources.NotificationMovedTitle
                , string.Format(Resources.NotificationMovedContent
                    , seekios.SeekiosName
                    , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                    , true);
            });
        }

        /// <summary>
        /// Method called when a new tracking location is added
        /// </summary>
        public async void OnNewDontMoveTrackingLocationAdded(string uidSeekios
            , Tuple<int, int> batteryAndSignal, Tuple<double, double, double, double> latLongAltAcc
            , DateTime dateCommunication)
        {
            // Get the concerned seekios
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;

            // Get the mode concerned
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
            if (mode == null) return;

            // Is there location from the database ? 
            if (LsLocations?.Count == 0)
            {
                try
                {
                    App.CurrentUserEnvironment.LsLocations.AddRange(await _dataService.LocationsByMode(mode.Idmode));
                }
                catch (Exception) { }
            }

            // Add location to user environment
            double lat = latLongAltAcc.Item1
                , lon = latLongAltAcc.Item2
                , altitude = latLongAltAcc.Item3
                , accuracy = latLongAltAcc.Item4;
            var locationToAdd = new LocationDTO
            {
                DateLocationCreation = dateCommunication.ToLocalTime(),
                Latitude = lat,
                Longitude = lon,
                Altitude = altitude,
                Accuracy = accuracy,
                Mode_idmode = mode.Idmode,
                Seekios_idseekios = seekios.Idseekios,
                IdLocationDefinition = (int)LocationDefinition.DontMove
            };
            App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);

            // Add location to historic list
            var seekiosHistoric = App.Locator.Historic.LsSeekiosLocations?.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios);
            if (seekiosHistoric != null && seekiosHistoric.LsLocations?.Count > 0)
            {
                seekiosHistoric.LsLocations.Add(locationToAdd);
            }

            // Update the seekios
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.LastKnownLocation_latitude = lat;
            seekios.LastKnownLocation_longitude = lon;
            seekios.LastKnownLocation_altitude = altitude;
            seekios.LastKnownLocation_accuracy = accuracy;
            seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();

            // Update UI 
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                NewDontMoveTrackingLocationAddedNotified?.Invoke(seekios.Idseekios, lat, lon, altitude, accuracy, dateCommunication.ToLocalTime());
            });
        }

        #endregion
    }
}