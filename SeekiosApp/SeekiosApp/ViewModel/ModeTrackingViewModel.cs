using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using SeekiosApp.Enum;
using SeekiosApp.Extension;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SeekiosApp.ViewModel
{
    public class ModeTrackingViewModel : MapViewModelBase
    {
        #region ===== Constants ===================================================================

        /// <summary>Refresh rate in minutes</summary>
        public const int DEFAULT_TRACKING_REFRESH_RATE = 1;

        #endregion

        #region ===== Attributs ===================================================================

        private AmountOfTime _locationsSince = AmountOfTime.Undefined;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>True if the mode is waiting for validation</summary>
        public bool IsWaitingForValidation { get; set; }

        /// <summary>Tracking mode location list</summary>
        public List<LocationDTO> LsLocations
        {
            get
            {
                if (Seekios == null) return new List<LocationDTO>();
                var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
                if (mode == null) return new List<LocationDTO>();
                return App.CurrentUserEnvironment.LsLocations
                    .Where(el => el.Mode_idmode == mode.Idmode &&
                    el.IdLocationDefinition == (int)LocationDefinition.Tracking)
                    //(el.DateLocationCreation > DateTime.Now.AddMonths(-1) && LocationSince == AmountOfTime.FromAMonth
                    //|| el.DateLocationCreation > DateTime.Now.AddDays(-7) && LocationSince == AmountOfTime.FromAWeek
                    //|| el.DateLocationCreation > DateTime.Now.AddDays(-2) && LocationSince == AmountOfTime.FromYesterday
                    //|| el.DateLocationCreation > DateTime.Now.AddDays(-1) && LocationSince == AmountOfTime.Today
                    //|| LocationSince == AmountOfTime.Undefined))
                    .OrderBy(el => el.DateLocationCreation)
                    .ToList();
            }
        }

        /// <summary>Interval of time during which locations have to be displayed</summary>
        public AmountOfTime LocationSince
        {
            get { return _locationsSince; }
            set
            {
                _locationsSince = value;
                RaisePropertyChanged("LsLocations");
                RaisePropertyChanged("TrackingFirstDate");
                RaisePropertyChanged("TrackingLastDate");
                InitTrackingRoute();
            }
        }

        public TrackingSetting TrackingSetting
        {
            get
            {
                var trackingSetting = App.Locator.ModeSelection.LsTrackingSetting.FirstOrDefault(x => x.IdSeekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios && x.ModeDefinition == ModeDefinitionEnum.ModeTracking);
                if (trackingSetting == null) return new TrackingSetting() { IsEnable = true, RefreshTime = 1 };
                else return trackingSetting;
            }
        }

        public bool IsPowerSavingEnabled { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ModeTrackingViewModel(IDispatchOnUIThread dispatcherService
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ILocalNotificationService localNotificationService)
            : base(dispatcherService, dataService, dialogService, localNotificationService) { }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Initialize mode
        /// </summary>
        public void InitMode()
        {
            RefreshTime = DEFAULT_TRACKING_REFRESH_RATE;
            var refreshTime = DEFAULT_TRACKING_REFRESH_RATE;
            if (Mode != null && int.TryParse(Mode.Trame, out refreshTime))
            {
                RefreshTime = refreshTime;
            }
        }

        /// <summary>
        /// Initialize tracking route sorted ASC date
        /// </summary>
        public async void InitTrackingRoute()
        {
            if (MapControlManager == null) return;
            var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
            // Is there location from the database ? 
            //if (mode != null && LsLocations?.Count < 2)    // Even if we have 1 position, we get position in db in case of some delay
            //{
            try
            {
                // Skip, the seekios is in power saving and it's waiting for activation
                if (!Seekios.HasGetLastInstruction
                    && Seekios.IsInPowerSaving
                    && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS)
                {
                    MapControlManager.CreateRouteBackground(LsLocations);
                }

                // Get positions from database
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

        /// <summary>
        /// Used to know if the actual tracking is probably missing positions
        /// If we miss only one position 
        /// </summary>
        private int CalculateModeTrackingExpectedPositionNumber(ModeDTO mode)
        {
            if (mode == null) return -1;

            DateTime creationDate = mode.DateModeCreation;
            int refreshRateMin = int.Parse(mode.Trame);
            double minutesGoneSinceActivation = (DateTime.Now - creationDate).TotalMinutes;

            // On est pas assez précis pour des valeurs de tracking plus élevées que 1/5min je pense... A voir
            int positionExpectedNumber = (int)(minutesGoneSinceActivation / refreshRateMin);

            return positionExpectedNumber;
        }

        /// <summary>
        /// Get interval of time where there are no locations
        /// </summary>
        public List<AmountOfTime> GetUselessAmountOfTime()
        {
            var lsUselessAmoutOfTime = new List<AmountOfTime>();
            lsUselessAmoutOfTime.Add(AmountOfTime.Today);
            lsUselessAmoutOfTime.Add(AmountOfTime.FromYesterday);
            lsUselessAmoutOfTime.Add(AmountOfTime.FromAWeek);
            lsUselessAmoutOfTime.Add(AmountOfTime.FromAMonth);

            if (LsLocations.Count == 0) return lsUselessAmoutOfTime;

            lsUselessAmoutOfTime.Clear();

            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-1) && el.DateLocationCreation < DateTime.Now).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.Today);
            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-2) && el.DateLocationCreation < DateTime.Now.AddDays(-1)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromYesterday);
            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-7) && el.DateLocationCreation < DateTime.Now.AddDays(-2)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromAWeek);
            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddMonths(-1) && el.DateLocationCreation < DateTime.Now.AddDays(-7)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromAMonth);

            return lsUselessAmoutOfTime;
        }

        /// <summary>
        /// Method called when a message is get when a location is added by SignalR
        /// </summary>
        public void OnAddTrackingLocation(string uidSeekios
            , Tuple<int, int> batteryAndsignal
            , Tuple<double, double, double, double> latLongAltAcc
            , DateTime dateCommunication)
        {
            if (Math.Floor(Math.Abs(latLongAltAcc.Item1)) == 0) return; // Evite de considerer les positions a zero...
            if (string.IsNullOrEmpty(uidSeekios)) return;
            // Parse parameters
            double lat = latLongAltAcc.Item1, lon = latLongAltAcc.Item2, altitude = latLongAltAcc.Item3, accuracy = latLongAltAcc.Item4;
            // Get the seekios 
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;
            // Get mode
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
            if (mode == null) return;
            mode.CountOfTriggeredAlert++;
            mode.LastTriggeredAlertDate = App.CurrentUserEnvironment.ServerCurrentDateTime;
            // Add locaion to user environment
            var locationToAdd = new LocationDTO
            {
                DateLocationCreation = dateCommunication.ToLocalTime(),
                Latitude = lat,
                Longitude = lon,
                Altitude = altitude,
                Accuracy = accuracy,
                Mode_idmode = mode.Idmode,
                Seekios_idseekios = seekios.Idseekios,
                IdLocationDefinition = (int)LocationDefinition.Tracking
            };
            var seekiosHistoric = App.Locator.Historic.LsSeekiosLocations?.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios);
            if (seekiosHistoric != null)
            {
                seekiosHistoric.LsLocations.Add(locationToAdd);
            }
            App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);
            // Update the seekios
            seekios.BatteryLife = batteryAndsignal.Item1;
            seekios.SignalQuality = batteryAndsignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.LastKnownLocation_latitude = lat;
            seekios.LastKnownLocation_longitude = lon;
            seekios.LastKnownLocation_altitude = altitude;
            seekios.LastKnownLocation_accuracy = accuracy;
            seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();

            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                // Stop and start the timer
                var seekiosOnTracking = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == seekios.Idseekios);
                if (seekiosHistoric != null && seekiosHistoric.LsLocations?.Count > 0)
                {
                    seekiosOnTracking.Timer.Stop();
                }
                // Raise event 
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                OnTrackingPositionAdded?.Invoke(seekios.Idseekios, lat, lon, altitude, accuracy, dateCommunication.ToLocalTime());
            });
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>Event fired when a tracking position is added</summary>
        public event NewTrackingPositionAdded OnTrackingPositionAdded;
        public delegate void NewTrackingPositionAdded(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);

        #endregion
    }
}