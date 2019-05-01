//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Enum;
//using SeekiosApp.Extension;
//using SeekiosApp.Interfaces;
//using SeekiosApp.Model.APP;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//namespace SeekiosApp.ViewModel
//{
//    /*
//    public class ModeInTimeViewModel : MapViewModelBase
//    {
//        #region ===== Constants ===================================================================

//        private const int NUMBER_OF_ADDRESSES = 5;

//        #endregion

//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="dataService">data service</param>
//        /// <param name="dialogService">dialog service</param>
//        public ModeInTimeViewModel(IDispatchOnUIThread dispatcher
//            , IDataService dataService
//            , IDialogService dialogService)
//            : base(dispatcher, dataService, dialogService)
//        {
//            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
//            _dialogService = dialogService;
//            _dataService = dataService;
//        }

//        #endregion

//        #region ===== Attributs ===================================================================

//        /// <summary>True if the mode is being validated</summary>
//        private bool _isWaitingForValidation = false;

//        /// <summary>Highlighted lcoation</summary>
//        private LocationDTO _selectedLocation;

//        /// <summary>Interval of time during which locations have to be displayed</summary>
//        private AmountOfTime _locationsSince = AmountOfTime.Undefined;

//        /// <summary>Navigation service</summary>
//        private INavigationService _navigationService;

//        /// <summary>Dialog service</summary>
//        private IDialogService _dialogService;

//        /// <summary>Data access service</summary>
//        private IDataService _dataService;

//        /// <summary>Culture info</summary>
//        private CultureInfo _enCultureInfo = new CultureInfo("en-US");

//        /// <summary>list of times defined for tracking</summary>
//        private List<TimeDay> _timePickerList = new List<TimeDay>();

//        #endregion

//        #region ===== Propriétés ==================================================================

//        /// <summary>string list used to display the time picked for the daily tracking</summary>
//        public List<TimeDay> TimePickerList
//        {
//            get
//            {
//                return _timePickerList;
//            }
//            set
//            {
//                _timePickerList = value;
//            }
//        }

//        /// <summary>string list used to display the time picked for the daily tracking UTC</summary>
//        public List<TimeDay> TimePickerListUTC
//        {
//            get
//            {
//                if (_timePickerList != null)
//                {
//                    var timePickerListUTC = new List<TimeDay>();

//                    foreach (var time in _timePickerList)
//                    {
//                        var actualTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, DateTime.Now.Second);
//                        //When the stime is set we get the value in UTC for the seekios
//                        var timeUTC = TimeZoneInfo.ConvertTime(actualTime, TimeZoneInfo.Utc);
//                        timePickerListUTC.Add(new TimeDay(timeUTC.Hour, timeUTC.Minute, time.Monday, time.Tuesday,
//                            time.Wednesday, time.Thursday, time.Friday, time.Saturday, time.Sunday
//                            ));
//                    }
//                    return timePickerListUTC;
//                }
//                return null;
//            }
//        }

//        /// <summary>
//        /// Position chosen on the map to be used for the trame
//        /// </summary>
//        public LatitudeLongitude PositionChosenOnMap { get; set; }

//        /// <summary>
//        /// Title for the position chosen on map
//        /// </summary>
//        public string PositionChosenOnMapTitle { get; set; }

//        /// <summary>
//        /// True if the mode is waiting for validation
//        /// </summary>
//        public bool IsWaitingForValidation
//        {
//            get { return _isWaitingForValidation; }
//            set { Set(() => this.IsWaitingForValidation, ref _isWaitingForValidation, value); }
//        }

//        /// <summary>
//        /// Tracking mode location list
//        /// </summary>
//        public List<LocationDTO> LsLocations
//        {
//            get
//            {
//                if (Seekios == null) return new List<LocationDTO>();
//                var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
//                if (mode == null) return new List<LocationDTO>();

//                return App.CurrentUserEnvironment.LsLocations
//                    .Where(el => el.Mode_idmode == mode.Idmode &&
//                    (el.DateLocationCreation > DateTime.Now.AddMonths(-1) && LocationSince == AmountOfTime.FromAMonth
//                    || el.DateLocationCreation > DateTime.Now.AddDays(-7) && LocationSince == AmountOfTime.FromAWeek
//                    || el.DateLocationCreation > DateTime.Now.AddDays(-2) && LocationSince == AmountOfTime.FromYesterday
//                    || el.DateLocationCreation > DateTime.Now.AddDays(-1) && LocationSince == AmountOfTime.Today
//                    || LocationSince == AmountOfTime.Undefined))
//                    .OrderBy(el => el.DateLocationCreation).ToList();
//            }
//        }
//        /// <summary>
//        /// Hightlighted location
//        /// </summary>
//        public LocationDTO SelectedLocation
//        {
//            get { return _selectedLocation; }
//            set
//            {
//                _selectedLocation = value;
//                if (value != null && MapControlManager != null) MapControlManager.ChangeSelectedLocation(value);
//            }
//        }

//        /// <summary>
//        /// Interval of time during which locations have to be displayed
//        /// </summary>
//        public AmountOfTime LocationSince
//        {
//            get { return _locationsSince; }
//            set
//            {
//                _locationsSince = value;
//                InitTrackingRoute();
//                SelectedLocation = LsLocations.LastOrDefault();
//            }
//        }

//        /// <summary>
//        /// First date of tracking
//        /// </summary>
//        public string TrackingFirstDate
//        {
//            get
//            {
//                if (LsLocations.Count == 0) return Resources.Tracking_noLocation;
//                var location = LsLocations.FirstOrDefault();
//                if (location == null) return string.Empty;
//                return DateExtension.FormatDateTimeFromNow(LsLocations.First().DateLocationCreation);
//            }
//        }

//        /// <summary>
//        /// Last date of tracking
//        /// </summary>
//        public string TrackingLastDate
//        {
//            get
//            {
//                var location = LsLocations.LastOrDefault();
//                if (location == null) return string.Empty;
//                return DateExtension.FormatDateTimeFromNow(LsLocations.Last().DateLocationCreation);
//            }
//        }

//        #endregion

//        #region ===== Méthodes publiques ==========================================================

//        /// <summary>
//        /// Initialize mode
//        /// </summary>
//        public async void InitMode()
//        {
//            if (Mode != null && !string.IsNullOrEmpty(Mode.Trame))
//                _timePickerList = DecodeTrame(Mode.Trame);
//            //if (App.Locator.ModeInTime.PositionChosenOnMap != null)
//            //{
//            //    Address address = await MapControlManager.GetAddressFromLatLong(App.Locator.ModeInTime.PositionChosenOnMap);
//            //    PositionChosenOnMap = new LatitudeLongitude(address.Latitude, address.Longitude);
//            //    PositionChosenOnMapTitle = address.AddressName;
//            //    RaisePropertyChanged("AddressGet");
//            //}
//        }

//        /// <summary>
//        /// Delete an item in the list
//        /// </summary>
//        /// <param name="position"></param>
//        public void DeleteTimeItem(TimeDay timeDay)
//        {
//            TimePickerList.Remove(timeDay);
//            RaisePropertyChanged("ItemDeleted");
//        }

//        /// <summary>
//        /// Insert mode
//        /// </summary>
//        /// <returns></returns>
//        public async Task<bool> SaveMode()
//        {
//            try
//            {
//                if (_timePickerList == null || _timePickerList.Count <= 0)
//                {
//                    var msg = Resources.DailyTrackEmptyTimerList;
//                    var title = Resources.DailyTrackCannotSaveMode;
//                    await _dialogService.ShowMessage(msg, title);
//                    return false;
//                }

//                if (PositionChosenOnMap == null)
//                {
//                    var msg = Resources.InTimeNoPositionChosen;
//                    var title = Resources.DailyTrackCannotSaveMode;
//                    await _dialogService.ShowMessage(msg, title);
//                    return false;
//                }

//                // user out of requests
//                int creditsDispo = 0;
//                if (!int.TryParse(Helper.CreditHelper.TotalCredits, out creditsDispo)) return false;
//                if (creditsDispo <= 0)
//                //if (App.CurrentUserEnvironment.User.RemainingRequest <= 0)
//                {
//                    var msg = Resources.UserNoRequestLeft;
//                    var title = Resources.UserNoRequestLeftTitle;
//                    await _dialogService.ShowMessage(msg, title);
//                    return false;
//                }


//                if (Mode == null || Mode.ModeDefinition_idmodeDefinition != (int)Enum.ModeDefinitionEnum.ModeInTime)
//                {

//                    // new mode
//                    var modeToAdd = new ModeDTO
//                    {
//                        ModeDefinition_idmodeDefinition = (int)Enum.ModeDefinitionEnum.ModeInTime,
//                        Seekios_idseekios = Seekios.Idseekios,
//                        DateModeCreation = DateTime.Now,
//                        StatusDefinition_idstatusDefinition = 1,
//                        NotificationPush = 1,
//                        Trame = CodeTrame(TimePickerListUTC),
//                        Device_iddevice = App.CurrentUserEnvironment.Device.Iddevice
//                    };

//                    // add mode
//                    var idMode = await _dataService.InsertMode(modeToAdd);

//                    // error handling
//                    if (idMode == -1)
//                    {
//                        var msg = Resources.UnexpectedError;
//                        var title = Resources.UnexpectedErrorTitle + Resources.CannotSaveMode; //TODO : dans le fichier ressource !
//                        await _dialogService.ShowMessage(msg, title);
//                        return false;
//                    }

//                    Seekios.HasGetLastInstruction = false;

//                    // add mode to local data
//                    modeToAdd.Idmode = idMode;
//                    // replace last mode from data
//                    var modeToDelete = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == Seekios.Idseekios).FirstOrDefault();
//                    if (modeToDelete != null) App.CurrentUserEnvironment.LsMode.Remove(modeToDelete);
//                    App.CurrentUserEnvironment.LsMode.Add(modeToAdd);
//                }
//                // update zone
//                else
//                {
//                    // coding thread
//                    var trame = CodeTrame(TimePickerListUTC);
//                    if (Mode.Trame == trame)
//                    {
//                        var msg = Resources.TimersNotChanged;
//                        var title = Resources.UpdateTimersFailed;
//                        await _dialogService.ShowMessage(msg, title);
//                        return false;
//                    }
//                    var oldTrame = Mode.Trame;
//                    Mode.Trame = trame;

//                    // update mode
//                    var nbModeUpdated = await _dataService.UpdateMode(Mode);

//                    //chekc that timer and mode have been updated
//                    if (nbModeUpdated != 2)
//                    {
//                        Mode.Trame = oldTrame;
//                        var msg = Resources.DailyTrackUpdateFailed;
//                        var title = Resources.UpdateTimersFailed;
//                        await _dialogService.ShowMessage(msg, title);
//                        return false;
//                    }
//                    Seekios.HasGetLastInstruction = false;
//                }

//                return true;
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.FacebookConnectionFailedTitle, Resources.FacebookConnectionFailedButtonText, null);
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }

//            return false;
//        }

//        /// <summary>
//        /// Initialize tracking route sorted ASC date
//        /// </summary>
//        public void InitTrackingRoute()
//        {
//            if (MapControlManager == null) return;
//            SelectedLocation = null;
//            MapControlManager.CreateRoute(LsLocations);
//        }

//        /// <summary>
//        /// Define default location date interval : 1 month max
//        /// </summary>
//        /// <returns></returns>
//        public AmountOfTime DefineDefaultLocationSince()
//        {
//            var location = LsLocations.FirstOrDefault();
//            if (location == null)
//            {
//                _locationsSince = AmountOfTime.Today;
//                return AmountOfTime.Today;
//            }

//            //If first location dates for more than a week
//            if (location.DateLocationCreation < DateTime.Now.AddDays(-7)) _locationsSince = AmountOfTime.FromAMonth;
//            //If first location dates for more than two days
//            else if (location.DateLocationCreation < DateTime.Now.AddDays(-2)) _locationsSince = AmountOfTime.FromAWeek;
//            //If first location dates for more than one day
//            else if (location.DateLocationCreation < DateTime.Now.AddMonths(-1)) _locationsSince = AmountOfTime.FromYesterday;
//            else _locationsSince = AmountOfTime.Today;

//            return LocationSince;
//        }

//        /// <summary>
//        /// Get interval of time where there are no locations
//        /// </summary>
//        /// <returns></returns>
//        public List<AmountOfTime> GetUselessAmountOfTime()
//        {
//            var lsUselessAmoutOfTime = new List<AmountOfTime>();
//            lsUselessAmoutOfTime.Add(AmountOfTime.Today);
//            lsUselessAmoutOfTime.Add(AmountOfTime.FromYesterday);
//            lsUselessAmoutOfTime.Add(AmountOfTime.FromAWeek);
//            lsUselessAmoutOfTime.Add(AmountOfTime.FromAMonth);

//            if (LsLocations.Count == 0) return lsUselessAmoutOfTime;

//            lsUselessAmoutOfTime.Clear();

//            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-1) && el.DateLocationCreation < DateTime.Now).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.Today);
//            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-2) && el.DateLocationCreation < DateTime.Now.AddDays(-1)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromYesterday);
//            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddDays(-7) && el.DateLocationCreation < DateTime.Now.AddDays(-2)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromAWeek);
//            if (LsLocations.Where(el => el.DateLocationCreation > DateTime.Now.AddMonths(-1) && el.DateLocationCreation < DateTime.Now.AddDays(-7)).Count() == 0) lsUselessAmoutOfTime.Add(AmountOfTime.FromAMonth);

//            return lsUselessAmoutOfTime;
//        }

//        /// <summary>
//        /// Method called when a tracking location is added in database by the seekios
//        /// </summary>
//        /// <param name="uidSeekios"></param>
//        /// <param name="batteryAndsignal"></param>
//        /// <param name="latLong"></param>
//        /// <param name="accuracy"></param>
//        public void OnAddInTimeLocation(string uidSeekios, Tuple<int, int> batteryAndsignal, LatitudeLongitude latLong, double accuracy)
//        {
//            if (_dispatcherService == null) return;

//            _dispatcherService.Invoke(() =>
//            {
//                if (string.IsNullOrEmpty(uidSeekios))
//                    return;

//                var provider = new CultureInfo("en-US");

//                var dateLocation = App.CurrentUserEnvironment.ServerCurrentDateTime;

//                //Get seekios concerned
//                var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
//                if (seekios == null) return;

//                //Get mode concerned
//                var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
//                if (mode == null) return;

//                //Remove a request from the user account
//                mode.CountOfTriggeredAlert++;
//                mode.LastTriggeredAlertDate = App.CurrentUserEnvironment.ServerCurrentDateTime;

//                //Add location to user environment
//                App.CurrentUserEnvironment.LsLocations.Add(new LocationDTO
//                {
//                    DateLocationCreation = dateLocation,
//                    Latitude = latLong.Latitude,
//                    Longitude = latLong.Longitude,
//                    Accuracy = accuracy,
//                    Mode_idmode = mode.Idmode
//                });

//                seekios.BatteryLife = batteryAndsignal.Item1;
//                seekios.SignalQuality = batteryAndsignal.Item2;
//                seekios.LastKnownLocation_latitude = latLong.Latitude;
//                seekios.LastKnownLocation_longitude = latLong.Longitude;
//                seekios.LastKnownLocation_accuracy = accuracy;
//                seekios.LastKnownLocation_dateLocationCreation = dateLocation;

//                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);

//                if (OnInTimePositionAdded != null) OnInTimePositionAdded.Invoke(latLong.Latitude, latLong.Longitude, accuracy);

//                if (Seekios == null) return;
//                if (Seekios.UIdSeekios != uidSeekios) return;

//                InitTrackingRoute();
//                SelectedLocation = LsLocations.LastOrDefault();
//            });
//        }

//        /// <summary>
//        /// Go to location when a position is chosen in AutoCompleteTextView
//        /// </summary>
//        /// <param name="latlong"></param>
//        /// <param name="title"></param>
//        public void GoToLocation(LatitudeLongitude latlong, string title)
//        {
//            //MapControlManager.GoToLocation(latlong, title, true);
//        }

//        /// <summary>
//        /// getAddressList
//        /// </summary>
//        /// <param name="address"></param>
//        public async Task<List<string>> GetAddressList(string address)
//        {
//            //return await MapControlManager.GetAddressList(address);
//            return null;
//        }

//        /// <summary>
//        /// Get LAtLong from address
//        /// </summary>
//        /// <param name="address"></param>
//        /// <returns></returns>
//        public LatitudeLongitude GetLatLongFromAddress(string address)
//        {
//            //return MapControlManager.GetLatLongFromAdress(address);
//            return null;
//        }

//        /// <summary>
//        /// Show message if error detected
//        /// </summary>
//        /// <param name="title"></param>
//        /// <param name="message"></param>
//        public void ShowErrorMessage(ErrorCode errorCode, string title, string message)
//        {
//            if (errorCode == ErrorCode.CustomError && !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(message))
//                _dialogService.ShowMessage(message, title);
//            else if (errorCode == ErrorCode.GeocoderError)
//                _dialogService.ShowError(Resources.GeoCoderServiceNotAvailable, Resources.GeoCoderServiceNotAvailableTitle, "Ok", null);
//        }

//        #endregion

//        #region ===== Évènements ==================================================================

//        /// <summary>
//        /// Event fired when a tracking position is added
//        /// </summary>
//        public event NewTrackingPositionAdded OnInTimePositionAdded;

//        public delegate void NewTrackingPositionAdded(double lat, double lon, double accuracy);

//        #endregion

//        #region ===== CODE & DECODE TRAME =========================================================

//        /// <summary>
//        /// Decode thread from database into a time list
//        /// </summary>
//        /// <param name="trame">thread</param>
//        /// <returns></returns>
//        private List<TimeDay> DecodeTrame(string trame)
//        {
//            if (string.IsNullOrEmpty(trame.ToString())) return null;

//            var listOfTimeLocal = new List<TimeDay>();

//            if (Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeInTime)
//            {
//                try
//                {
//                    var body = trame;
//                    if (body == string.Empty) return listOfTimeLocal;

//                    var bodySplit = body.Split(';');
//                    var coords = bodySplit[3].Split(':');
//                    var timers = bodySplit[2].Split('+');

//                    //save position in view model to update the map
//                    //App.Locator.ModeInTime.PositionChosenOnMap = new LatitudeLongitude(double.Parse(coords[0], _enCultureInfo), double.Parse(coords[1], _enCultureInfo));

//                    //timer parsing handling: (1234-17:30) 
//                    foreach (var timerStr in timers)
//                    {
//                        //initialise booleans to false
//                        bool monday = false;
//                        bool tuesday = false;
//                        bool wednesday = false;
//                        bool thursday = false;
//                        bool friday = false;
//                        bool saturday = false;
//                        bool sunday = false;

//                        //handle days in the trame: (1234)
//                        var timerDays = timerStr.Split('-')[0].ToCharArray();
//                        foreach (var dayStr in timerDays)
//                        {
//                            if (int.Parse(dayStr.ToString()) == 1) monday = true;
//                            if (int.Parse(dayStr.ToString()) == 2) tuesday = true;
//                            if (int.Parse(dayStr.ToString()) == 3) wednesday = true;
//                            if (int.Parse(dayStr.ToString()) == 4) thursday = true;
//                            if (int.Parse(dayStr.ToString()) == 5) friday = true;
//                            if (int.Parse(dayStr.ToString()) == 6) saturday = true;
//                            if (int.Parse(dayStr.ToString()) == 0) sunday = true;
//                        }
//                        //handle timer in trame: 17:30
//                        var timer = timerStr.Split('-')[1].Split(':');

//                        // convert the UTC time in Local time to display it to the user
//                        int hour = int.Parse(timer[0]);
//                        int minute = int.Parse(timer[1]);
//                        var actualTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, DateTime.Now.Second, DateTimeKind.Utc);
//                        var timeLocal = TimeZoneInfo.ConvertTime(actualTime, TimeZoneInfo.Local);

//                        //create the object and add it to the list
//                        listOfTimeLocal.Add(new TimeDay()
//                        {
//                            Hour = timeLocal.Hour,
//                            Minute = timeLocal.Minute,
//                            Monday = monday,
//                            Tuesday = tuesday,
//                            Wednesday = wednesday,
//                            Thursday = thursday,
//                            Friday = friday,
//                            Saturday = saturday,
//                            Sunday = sunday
//                        });
//                    }
//                }
//                catch (Exception)
//                {
//                    //TODO: Handle exception
//                    return null;
//                }
//            }

//            return listOfTimeLocal;
//        }

//        /// <summary>
//        /// Code timers into a thread
//        /// </summary>
//        /// <param name="zone">liste des timers</param>
//        /// <returns></returns>
//        private string CodeTrame(List<TimeDay> listOfTime)
//        {
//            if (listOfTime == null) return null;
//            List<string> listOfStr = new List<string>();

//            var trame = "2;30;"; //start of the trame

//            foreach (var time in listOfTime)
//            {
//                listOfStr.Add(CodeDaysForTrame(time));
//            }
//            trame += string.Join("+", listOfStr);
//            trame += ";" + string.Format(_enCultureInfo, "{0:0.#########}:{1:0.#########}", PositionChosenOnMap.Latitude, PositionChosenOnMap.Longitude) + ";20";

//            return trame;
//        }

//        /// <summary>
//        /// Code the string of days used for the trame, ex: 124/17:30
//        /// </summary>
//        /// <param name="timeDay"></param>
//        /// <returns></returns>
//        private string CodeDaysForTrame(TimeDay timeDay)
//        {
//            var trame = string.Empty;
//            if (timeDay.Monday) trame += (int)DayTrame.Monday;
//            if (timeDay.Tuesday) trame += (int)DayTrame.Tuesday;
//            if (timeDay.Wednesday) trame += (int)DayTrame.Wednesday;
//            if (timeDay.Thursday) trame += (int)DayTrame.Thursday;
//            if (timeDay.Friday) trame += (int)DayTrame.Friday;
//            if (timeDay.Saturday) trame += (int)DayTrame.Saturday;
//            if (timeDay.Sunday) trame += (int)DayTrame.Sunday;
//            trame += "-" + timeDay.ToString();
//            return trame;
//        }

//        /// <summary>
//        /// Enum for the days : used in trame
//        /// </summary>
//        private enum DayTrame
//        {
//            Monday = 1,
//            Tuesday = 2,
//            Wednesday = 3,
//            Thursday = 4,
//            Friday = 5,
//            Saturday = 6,
//            Sunday = 0
//        }

//        #endregion
//    }
//    */
//}

