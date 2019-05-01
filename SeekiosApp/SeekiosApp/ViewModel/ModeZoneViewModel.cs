using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
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
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class ModeZoneViewModel : MapViewModelBase
    {
        #region ===== Constants ===================================================================

        public const int DEFAULT_ZONE_REFRESH_RATE = 1;
        public int MAX_NUMBER_OF_POINTS = 10;

        #endregion

        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;
        private FavoriteAreaDTO _selectedFavoriteArea = null;
        private CultureInfo _enCultureInfo = new CultureInfo("en-US");
        private List<LatitudeLongitude> _lsAreaCoords = null;

        private bool _editingAlerts = false;
        private bool _invalidViews = false;
        private bool _waitingForAlerts = false;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Area coordinates from bdd</summary>
        public List<LatitudeLongitude> LsAreaCoords
        {
            get
            {
                return _lsAreaCoords;
            }
        }

        /// <summary>Area coordinates from map</summary>
        public List<LatitudeLongitude> LsAreaCoordsMap { get; set; }

        /// <summary>Selected favorite area</summary>
        public FavoriteAreaDTO SelectedFavoriteArea
        {
            get
            {
                return _selectedFavoriteArea;
            }
            set
            {
                Set(() => this.SelectedFavoriteArea, ref _selectedFavoriteArea, value);
            }
        }

        /// <summary>True if in edit mode</summary>
        public bool IsOnEditMode { get; set; }

        /// <summary>True if the mode zone activity is focused in edit mode</summary>
        public bool IsActivityFocused { get; set; }

        /// <summary> List of seekios which are configured to have a tracking activated after out of zone </summary>
        public HashSet<int> LsSeekiosInTrackingAfterOOZ { get; set; }

        /// <summary>True if a seekiso is configured to track after OOZ</summary>
        public bool IsTrackingAfterOOZ { get; set; }

        /// <summary>Zone Tracking mode location list</summary>
        public List<LocationDTO> LsLocations
        {
            get
            {
                if (Seekios == null) return new List<LocationDTO>();
                var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
                if (mode == null) return new List<LocationDTO>();
                return App.CurrentUserEnvironment.LsLocations
                    .Where(el => el.Mode_idmode == mode.Idmode && el.IdLocationDefinition == (int)LocationDefinition.Zone).ToList();
            }
        }

        /// <summary>List of the email alerts for the mode zone</summary>
        public List<AlertWithRecipientDTO> LsAlertsModeZone { get; set; }

        public bool EditingAlerts
        {
            get
            {
                return _editingAlerts;
            }
            set
            {
                _editingAlerts = value;
            }
        }

        public bool InvalidViews
        {
            get
            {
                return _invalidViews;
            }
            set
            {
                _invalidViews = value;
            }
        }

        public bool WaitingForAlerts
        {
            get
            {
                return _waitingForAlerts;
            }
            set
            {
                _waitingForAlerts = value;
            }
        }

        public bool IsTrackingSettingEnable { get; set; }

        public TrackingSetting TrackingSetting
        {
            get
            {
                var trackingSetting = App.Locator.ModeSelection.LsTrackingSetting.FirstOrDefault(x => x.IdSeekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios && x.ModeDefinition == ModeDefinitionEnum.ModeZone);
                if (trackingSetting == null) return new TrackingSetting() { IsEnable = true, RefreshTime = App.DEFAULT_TRACKING_SETTING };
                else return trackingSetting;
            }
        }

        public bool IsPowerSavingEnabled { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ModeZoneViewModel(IDataService dataService
            , Interfaces.IDialogService dialogService
            , IDispatchOnUIThread dispatcherService
            , ILocalNotificationService localNotificationService)
            : base(dispatcherService, dataService, dialogService, localNotificationService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _localNotificationService = localNotificationService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            _lsAreaCoords = new List<LatitudeLongitude>();
            IsGoingBack = false;
            LsSeekiosInTrackingAfterOOZ = new HashSet<int>();
            LsAlertsModeZone = new List<AlertWithRecipientDTO>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region Init

        /// <summary>
        /// Initialize mode
        /// </summary>
        public void InitMode()
        {
            _lsAreaCoords?.Clear();
            if (Mode == null || string.IsNullOrEmpty(Mode.Trame)) return;
            _lsAreaCoords = DecodeTrame(Mode.Trame);
        }

        /// <summary>
        /// Initialize tracking route sorted ASC date
        /// </summary>
        public async void InitZoneTrackingRouteAsync()
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

        private int CalculateModeZoneExpectedPositionNumber(ModeDTO mode)
        {
            if (mode == null) return -1;

            DateTime creationDate = mode.DateModeCreation;
            DecodeTrame(mode.Trame);
            int refreshRateMin = RefreshTime;
            double minutesGoneSinceActivation = (DateTime.Now - creationDate).TotalMinutes;

            // On est pas assez précis pour des valeurs de tracking plus élevées je pense... A voir
            int positionExpectedNumber = (int)(minutesGoneSinceActivation / refreshRateMin);

            return positionExpectedNumber;
        }

        #endregion

        #region Zone

        /// <summary>
        /// Update area in database
        /// </summary>
        public async Task<bool> UpdateZone(bool isIOS = false)
        {
            try
            {
                // If the area is made of at least 3 points
                // And we check if the lines are not crossed
                if (LsAreaCoordsMap == null || LsAreaCoordsMap.Count < 3 || !CheckCorrectAreaFormat(LsAreaCoordsMap))
                {
                    await _dialogService.ShowMessage(Resources.ModeZone_NotValidZoneDescription, Resources.ModeZone_NotValidZoneTitle);
                    return false;
                }

                // Add a zone
                // Need to remove the last point, it's use just for drawing
                if (isIOS && LsAreaCoordsMap.Count > 2) LsAreaCoordsMap.RemoveAt(LsAreaCoordsMap.Count - 1);

                return await App.Locator.ModeSelection.SelectMode(ModeDefinitionEnum.ModeZone);
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e
                    , Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            return false;
        }

        /// <summary>
        /// Initialize zone
        /// </summary>
        public void InitZone()
        {
            var isInAlert = Mode != null && Mode.StatusDefinition_idstatusDefinition != 1;
            if (LsAreaCoords != null && LsAreaCoords.Count > 0)
            {
                MapControlManager.CreateZone(LsAreaCoords.Select(el => new LatitudeLongitude(el.Latitude, el.Longitude)).ToList(), isInAlert, true);
                MapControlManager.CreateUndoActionsToRemoveActualZone();
            }
        }

        /// <summary>
        /// Center the map on the zone
        /// </summary>
        public void CenterOnZone()
        {
            MapControlManager.CenterOnZone();
        }

        #endregion

        #region Favorite

        /// <summary>
        /// Add an area to favorite
        /// </summary>
        public async Task<bool> AddFavoriteArea(FavoriteAreaDTO favoriteAreaDTO)
        {
            try
            {
                if (favoriteAreaDTO == null) return false;

                // add area in database
                favoriteAreaDTO.User_iduser = App.CurrentUserEnvironment.User.IdUser;
                var id = await _dataService.InsertFavoriteArea(favoriteAreaDTO);

                favoriteAreaDTO.DateAddedFavorite = App.CurrentUserEnvironment.ServerCurrentDateTime;
                favoriteAreaDTO.IdfavoriteArea = id;

                // error handling
                if (id <= 0)
                {
                    var msg = Resources.UpdateZoneFailed;
                    var title = Resources.UpdateZoneFailedTitle;
                    await _dialogService.ShowMessage(msg, title);
                    return false;
                }

                // add area to local data
                App.CurrentUserEnvironment.LsFavoriteArea.Add(favoriteAreaDTO);
                SelectedFavoriteArea = favoriteAreaDTO;

                return true;
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e
                    , Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            return false;
        }

        /// <summary>
        /// Update area in database
        /// </summary>
        public async Task<bool> UpdateFavoriteArea(FavoriteAreaDTO favoriteAreaDTO)
        {
            try
            {
                if (favoriteAreaDTO == null || favoriteAreaDTO.IdfavoriteArea == 0) return false;

                // update in database
                favoriteAreaDTO.User_iduser = App.CurrentUserEnvironment.User.IdUser;
                var countOfUpdated = await _dataService.UpdateFavoriteArea(favoriteAreaDTO);

                // error handling
                if (countOfUpdated <= 0)
                {
                    var msg = Resources.UpdateZoneFailed;
                    var title = Resources.UpdateZoneFailedTitle;
                    await _dialogService.ShowMessage(msg, title);
                    return false;
                }

                return true;
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e
                    , Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            return false;
        }

        /// <summary>
        /// Delete an area from database
        /// </summary>
        public async Task<bool> DeleteFavoriteArea(FavoriteAreaDTO favoriteAreaDTO)
        {
            try
            {
                if (favoriteAreaDTO == null || favoriteAreaDTO.IdfavoriteArea == 0) return false;

                // delete area from database
                var result = await _dataService.DeleteFavoriteArea(favoriteAreaDTO.IdfavoriteArea);

                // error handling
                if (result <= 0)
                {
                    var msg = Resources.DeleteZoneFailed;
                    var title = Resources.DeleteZoneFailedTitle;
                    await _dialogService.ShowMessage(msg, title);
                    return false;
                }

                // suppress local favorite area
                App.CurrentUserEnvironment.LsFavoriteArea.Remove(favoriteAreaDTO);

                return true;
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e
                    , Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            return false;
        }

        #endregion

        #region New Locations

        /// <summary>
        /// Get a new location if the seekios leaves the zone
        /// </summary>
        public void OnNotifySeekiosOutOfZone(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , Tuple<double, double, double, double> latLongAltAcc
            , DateTime dateCommunication)
        {
            // Parse parameters
            double lat = latLongAltAcc.Item1, lon = latLongAltAcc.Item2, altitude = latLongAltAcc.Item3, accuracy = latLongAltAcc.Item4;

            // Seekios concerned
            var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
            if (seekios == null) return;
            // Add seekios as "in alert" in the list
            App.Locator.BaseMap.LsSeekiosAlertState.Add(seekios.Idseekios);

            // Mode concerned
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
            if (mode == null) return;

            // Update user requests amount
            mode.CountOfTriggeredAlert++;
            mode.StatusDefinition_idstatusDefinition = (int)StatutDefinitionEnum.SeekiosOutOfZone;
            mode.LastTriggeredAlertDate = App.CurrentUserEnvironment.ServerCurrentDateTime;

            // Add location to user environment
            var locationToAdd = new LocationDTO
            {
                DateLocationCreation = dateCommunication.ToLocalTime(),
                Latitude = lat,
                Longitude = lon,
                Altitude = altitude,
                Mode_idmode = mode.Idmode,
                Accuracy = accuracy,
                Seekios_idseekios = seekios.Idseekios,
                IdLocationDefinition = (int)LocationDefinition.Zone
            };
            var seekiosHistoric = App.Locator.Historic.LsSeekiosLocations?.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios);
            if (seekiosHistoric != null && seekiosHistoric.LsLocations?.Count > 0)
            {
                seekiosHistoric.LsLocations.Add(locationToAdd);
            }
            App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);

            // Update the seekios 
            seekios.BatteryLife = batteryAndSignal.Item1;
            seekios.SignalQuality = batteryAndSignal.Item2;
            seekios.DateLastCommunication = dateCommunication.ToLocalTime();
            seekios.LastKnownLocation_latitude = lat;
            seekios.LastKnownLocation_longitude = lon;
            seekios.LastKnownLocation_altitude = altitude;
            seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();
            seekios.LastKnownLocation_accuracy = accuracy;

            // Add the seekios in the list to display the tracking timer
            var refreshTime = 0;
            var param = mode.Trame.Split(';');
            if (param?.Count() > 0)
            {
                if (int.TryParse(param.First(), out refreshTime) && refreshTime > 0)
                {
                    App.Locator.BaseMap.AddSeekiosOnTracking(seekios, mode);
                }
            }

            // Update UI
            if (_dispatcherService == null) return;
            _dispatcherService.Invoke(() =>
            {
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                OnSeekiosOutOfZoneNotified?.Invoke(seekios.Idseekios, lat, lon, altitude, accuracy, dateCommunication.ToLocalTime());
                _localNotificationService.SendNotification(seekios
                    , Resources.NotificationOutOfZoneTitle
                    , string.Format(Resources.NotificationOutOfZoneContent
                        , seekios.SeekiosName
                        , dateCommunication.ToLocalTime().FormatDateTimeFromNow())
                    , true);
            });
        }

        /// <summary>
        /// Get a new location in tracking mode
        /// </summary>
        public void OnNewZoneTrackingLocationAdded(string uidSeekios
            , Tuple<int, int> batteryAndSignal
            , Tuple<double, double, double, double> latLongAltAcc
            , DateTime dateCommunication)
        {
            if (_dispatcherService == null) return;
            if (string.IsNullOrEmpty(uidSeekios)) return;

            _dispatcherService.Invoke(async () =>
            {
                // Parse parameters
                double lat = latLongAltAcc.Item1, lon = latLongAltAcc.Item2, altitude = latLongAltAcc.Item3, accuracy = latLongAltAcc.Item4;

                // Seekios concerned
                var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                if (seekios == null) return;

                // Zone concerned
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

                // Add lcoation to user environment
                var locationToAdd = new LocationDTO
                {
                    DateLocationCreation = dateCommunication.ToLocalTime(),
                    Latitude = lat,
                    Longitude = lon,
                    Altitude = altitude,
                    Mode_idmode = mode.Idmode,
                    Accuracy = accuracy,
                    Seekios_idseekios = seekios.Idseekios,
                    IdLocationDefinition = (int)LocationDefinition.Zone
                };
                var seekiosHistoric = App.Locator.Historic.LsSeekiosLocations?.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios);
                if (seekiosHistoric != null && seekiosHistoric.LsLocations?.Count > 0)
                {
                    seekiosHistoric.LsLocations.Add(locationToAdd);
                }
                App.CurrentUserEnvironment.LsLocations.Add(locationToAdd);

                // Update seekios
                seekios.BatteryLife = batteryAndSignal.Item1;
                seekios.SignalQuality = batteryAndSignal.Item2;
                seekios.DateLastCommunication = dateCommunication.ToLocalTime();
                seekios.LastKnownLocation_latitude = lat;
                seekios.LastKnownLocation_longitude = lon;
                seekios.LastKnownLocation_altitude = altitude;
                seekios.LastKnownLocation_accuracy = accuracy;
                seekios.LastKnownLocation_dateLocationCreation = dateCommunication.ToLocalTime();

                // Raise event to update UI 
                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                OnNewZoneTrackingLocationAddedNotified?.Invoke(seekios.Idseekios, lat, lon, altitude, accuracy, dateCommunication.ToLocalTime());
            });
        }

        /// <summary>
        /// Verify if a seekios is in the zone being defined
        /// </summary>
        public bool VerifyIfSeekiosIsInZone(LatitudeLongitude seekiosPos, List<LatitudeLongitude> zone)
        {
            return MapControlManager.IsSeekiosInZone(seekiosPos, zone);
        }

        #endregion

        #region Area Logic

        /// <summary>
        /// Decode thread from database into a GPS coordinates list
        /// </summary>
        public List<LatitudeLongitude> DecodeTrame(string trame)
        {
            if (string.IsNullOrEmpty(trame)) return null;

            var listOFCoord = new List<LatitudeLongitude>();
            RefreshTime = App.DEFAULT_TRACKING_SETTING;

            try
            {
                var index = 0;
                foreach (var param in trame.Split(';'))
                {
                    if (index == 0)
                    {
                        index++;
                        continue;
                    }
                    else if (index == 1)
                    {
                        var refreshTimeTrame = int.Parse(param);
                        if (refreshTimeTrame > 0)
                        {
                            LsSeekiosInTrackingAfterOOZ.Add(Seekios.Idseekios);
                            RefreshTime = refreshTimeTrame;
                        }
                        index++;
                        continue;
                    }
                    else
                    {
                        var latLong = param.Split(':');
                        var latitude = Double.Parse(latLong[0], _enCultureInfo);
                        var longitude = Double.Parse(latLong[1], _enCultureInfo);
                        listOFCoord.Add(new LatitudeLongitude(latitude, longitude));
                        index++;
                    }
                }
            }
            catch (Exception)
            {
                //TODO: Handle exception
                return null;
            }
            return listOFCoord;
        }

        /// <summary>
        /// Code GPS coordinates into a thread
        /// </summary>
        public string CodeTrame(List<LatitudeLongitude> zone)
        {
            if (zone?.Count <= 0) return null;
            var trame = RefreshTime.ToString();
            foreach (var point in zone)
            {
                trame += string.Format(_enCultureInfo, ";{0:0.#########}:{1:0.#########}", point.Latitude, point.Longitude);
            }
            return trame;
        }

        /// <summary>
        /// Check area format
        /// </summary>
        public bool CheckCorrectAreaFormat(List<LatitudeLongitude> zone)
        {
            //Check each vector
            for (int i = 0; i < zone.Count - 1; i++)
            {
                // Le vecteur d'un segment AB se calcule avec x = xB-xA et y = yB-yA
                var xA = zone[i].Latitude;
                var yA = zone[i].Longitude;
                var xB = zone[i + 1].Latitude;
                var yB = zone[i + 1].Longitude;
                var vectorAB = new Tuple<double, double>(xB - xA, yB - yA);
                // On parcourt les segments suivant qui ne touche pas le segment actuel
                for (int j = i + 2; j < zone.Count && (i == 0 ? j < zone.Count - 1 : true); j++)
                {
                    var xC = zone[j].Latitude;
                    var yC = zone[j].Longitude;
                    // Vérification si j == zone.Count : segment à prendre avec le premier point de la zone (dernier segment)
                    var xD = zone[(j == zone.Count - 1) ? 0 : j + 1].Latitude;
                    var yD = zone[(j == zone.Count - 1) ? 0 : j + 1].Longitude;

                    var vectorCD = new Tuple<double, double>(xD - xC, yD - yC);
                    var vectorAD = new Tuple<double, double>(xD - xA, yD - yA);
                    var vectorAC = new Tuple<double, double>(xC - xA, yC - yA);
                    var vectorCB = new Tuple<double, double>(xB - xC, yB - yC);
                    var vectorCA = new Tuple<double, double>(xA - xC, yA - yC);

                    var vectorProductABxCD = vectorAB.Item1 * vectorCD.Item2 - vectorAB.Item2 * vectorCD.Item1;
                    var vectorProductABxAD = vectorAB.Item1 * vectorAD.Item2 - vectorAB.Item2 * vectorAD.Item1;
                    var vectorProductABxAC = vectorAB.Item1 * vectorAC.Item2 - vectorAB.Item2 * vectorAC.Item1;
                    var vectorProductCDxCB = vectorCD.Item1 * vectorCB.Item2 - vectorCD.Item2 * vectorCB.Item1;
                    var vectorProductCDxCA = vectorCD.Item1 * vectorCA.Item2 - vectorCD.Item2 * vectorCA.Item1;

                    // Si intersection
                    if (vectorProductABxCD != 0 && vectorProductABxAD * vectorProductABxAC <= 0 && vectorProductCDxCB * vectorProductCDxCA <= 0) return false;
                }
            }
            return true;
        }

        #endregion

        #region Navigation

        /// <summary>
        /// Navigate to the ModeZone 2 page
        /// Needs at least 3 points on the map
        /// </summary>
        public async void GoToSecondPage(List<LatitudeLongitude> listOfPoints)
        {
            if (listOfPoints == null || listOfPoints.Count < 3 || !CheckCorrectAreaFormat(listOfPoints))
            {
                await _dialogService.ShowMessage(Resources.ModeZone_NotValidZoneDescription, Resources.ModeZone_NotValidZoneTitle);
                return;
            }
            LsAreaCoordsMap = listOfPoints;
            if (_navigationService.CurrentPageKey != App.MODE_ZONE_3_PAGE)
            {
                _navigationService.NavigateTo(App.MODE_ZONE_3_PAGE);
            }
        }

        /// <summary>
        /// Go to the ModeZone 3 page
        /// </summary>
        public void GoToThirdPage()
        {
            if (_navigationService.CurrentPageKey != App.MODE_ZONE_2_PAGE)
            {
                _navigationService.NavigateTo(App.MODE_ZONE_2_PAGE);
            }
        }

        /// <summary>
        /// Go to the alert detail page
        /// </summary>
        public void GoToAlertDetail(AlertWithRecipientDTO item, int position)
        {
            if (_navigationService.CurrentPageKey != App.ALERT_PAGE)
            {
                if (item == null)
                {
                    App.Locator.Alert.IsNew = true;
                    App.Locator.ModeZone.EditingAlerts = false;
                }
                else
                {
                    App.Locator.Alert.IsNew = false;
                    App.Locator.Alert.IdAlert = item.IdAlert;
                    App.Locator.Alert.ContentAlert = item.Content;
                    App.Locator.Alert.TitleAlert = item.Title;
                    App.Locator.Alert.LsRecipients = item.LsRecipients;
                    App.Locator.ModeZone.EditingAlerts = true;
                    App.Locator.Alert.ConvertAlertDTOToClassMember(item);
                }
                App.Locator.ModeZone.WaitingForAlerts = true;
                _navigationService.NavigateTo(App.ALERT_PAGE);
            }
        }

        /// <summary>
        /// Go to the previous page
        /// </summary>
        public void GoBack()
        {
            _navigationService.GoBack();
        }

        #endregion

        #endregion

        #region ===== Event =======================================================================

        /// <summary>Fired when a seekios is out of zone</summary>
        public event SeekiosOutOfZoneNotified OnSeekiosOutOfZoneNotified;
        public delegate void SeekiosOutOfZoneNotified(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);

        /// <summary>Fired when a seekios is back in zone</summary>
        public event SeekiosBackInZoneNotified OnSeekiosBackInZoneNotified;
        public delegate void SeekiosBackInZoneNotified(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);

        /// <summary>Fired when a lcoation is added</summary>
        public event NewZoneTrackingLocationAddedNotified OnNewZoneTrackingLocationAddedNotified;
        public delegate void NewZoneTrackingLocationAddedNotified(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);

        #endregion
    }
}