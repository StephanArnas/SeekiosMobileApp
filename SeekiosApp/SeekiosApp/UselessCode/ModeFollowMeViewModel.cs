using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Enum;
using SeekiosApp.Helper;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    /*
    public class ModeFollowMeViewModel : MapViewModelBase
    {
        #region ===== Constantes ==================================================================

        /// <summary>
        /// Refresh rate of the tracking mode in minute
        /// </summary>
        public const int DEFAULT_FOLLOW_ME_REFRESH_RATE = 1;

        #endregion

        #region ===== Constructeur ================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataService">data service</param>
        /// <param name="dialogService">dialog service</param>
        public ModeFollowMeViewModel(IDispatchOnUIThread dispatcher
            , IDataService dataService
            , IDialogService dialogService
            , IFollowMeService followMeService
            , IBluetoothService bluetoothService)
            : base(dispatcher, dataService, dialogService)
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            _dialogService = dialogService;
            _dataService = dataService;
            _followMeService = followMeService;
            _bluetoothService = bluetoothService;

            _followMeService.ConnexionStateChanged += OnConnexionStateChanged;
        }

        #endregion

        #region ===== Attributs ===================================================================

        /// <summary>True if the mode is being validated</summary>
        private bool _isWaitingForValidation = false;

        /// <summary>Navigation service</summary>
        private INavigationService _navigationService;

        /// <summary>Dialog service</summary>
        private IDialogService _dialogService;

        /// <summary>Data access service</summary>
        private IDataService _dataService;

        /// <summary>service de gestion du mode follow me</summary>
        private IFollowMeService _followMeService;

        /// <summary>Service bluetooth</summary>
        private IBluetoothService _bluetoothService;

        /// <summary>True if tracking is configured to start after the seekios lost the ble connexion</summary>
        private bool _isTrackingAfterBLEConnexionLoss = false;

        /// <summary>State of the Bluetooth connexon with the seekios</summary>
        private ConnexionState _bleConnexionState = ConnexionState.None;

        /// <summary>SeekiosBLE device</summary>
        private BluetoothDevice _seekiosBLEDevice;

        /// <summary></summary>
        private TimeoutHelper _waitForSeekiosResponseTimeout = new TimeoutHelper();

        /// <summary></summary>
        private TimeoutHelper _waitForSeekiosConnectionTimeout = new TimeoutHelper();

        /// <summary></summary>
        private SeekiosDTO _seekiosInFollowMe;

        #endregion

        #region ===== Propriétés =================================================================

        /// <summary>
        /// True if a mode is currently being validated
        /// </summary>
        public bool IsWaitingForValidation
        {
            get { return _isWaitingForValidation; }
            set { Set(() => this.IsWaitingForValidation, ref _isWaitingForValidation, value); }
        }

        /// <summary>
        /// True if tracking is configured to start after the seekios moves
        /// </summary>
        public bool IsTrackingAfterBLEConnexionLoss
        {
            get { return _isTrackingAfterBLEConnexionLoss; }
            set { Set(() => this.IsTrackingAfterBLEConnexionLoss, ref _isTrackingAfterBLEConnexionLoss, value); }
        }

        /// <summary>
        /// True if the Bluetooth is enabled
        /// </summary>
        public bool IsBLEEnable
        {
            get { return _bluetoothService.IsBluetoothEnable; }
        }

        /// <summary>
        /// State of the Bluetooth connexon with the seekios
        /// </summary>
        public ConnexionState BLEConnexionState
        {
            get { return _bleConnexionState; }
            set { Set(() => this.BLEConnexionState, ref _bleConnexionState, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public BluetoothDevice SeekiosBLEDevice
        {
            get { return _seekiosBLEDevice; }
            set { Set(() => this.SeekiosBLEDevice, ref _seekiosBLEDevice, value); }
        }

        #endregion

        #region ===== Méthodes Publiques ==========================================================

        /// <summary>
        /// Initialize Follow Me mode
        /// </summary>
        public void InitModeFollowMe()
        {
            if (Mode == null) return;

            RefreshTime = DEFAULT_FOLLOW_ME_REFRESH_RATE;
            DecodeTrame(Mode.Trame);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> SendMessageToSeekios()
        {
            BLEConnexionState = ConnexionState.None;

            var result = await App.Locator.ModeDefinition.SelectMode((int)Enum.ModeDefinitionEnum.ModeFollowMe,
                IsTrackingAfterBLEConnexionLoss ? RefreshTime : 0);
            if (!result) return false;

            App.SeekiosChanged += OnSeekiosChanged;
            _waitForSeekiosResponseTimeout.StartTimeout(60000, () =>
            {
                App.SeekiosChanged -= OnSeekiosChanged;
                BLEConnexionState = ConnexionState.MessageNotReceivedBySeekios;
                try
                {
                    _dataService.DeleteMode(Mode.Idmode.ToString());
                }
                catch (WebException e)
                {
                    _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
                }
                catch (TimeoutException e)
                {
                    _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
                }
                App.CurrentUserEnvironment.LsMode.Remove(Mode);
                //});
                return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> ConnectWithSeekios()
        {
            StartLookingForDevices();

            while (BLEConnexionState != ConnexionState.Connected
                && BLEConnexionState != ConnexionState.SeekiosUnreachable)
            { await Task.Delay(1000); }

            if (BLEConnexionState == ConnexionState.SeekiosUnreachable)
            {
                try
                {
                    await _dataService.DeleteMode(Mode.Idmode.ToString());
                }
                catch (WebException e)
                {
                    await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
                }
                catch (TimeoutException e)
                {
                    await _dialogService.ShowError(e, Resources.FacebookConnectionFailedTitle, Resources.FacebookConnectionFailedButtonText, null);
                }
                App.CurrentUserEnvironment.LsMode.Remove(Mode);
            }
            if (BLEConnexionState != ConnexionState.Connected) return false;

            _seekiosInFollowMe = Seekios;
            App.Locator.Alert.IsBackToHome = true;
            App.Locator.ListAlert.Seekios = Seekios;
            _navigationService.NavigateTo(App.LIST_ALERTS_SEEKIOS_ACTIVITY);

            return true;
        }

        /// <summary>
        /// Method called when the mode is restarted
        /// </summary>
        public async void OnRestartFollowMe()
        {
            try
            {
                int creditsDispo = 0;
                if (!int.TryParse(Helper.CreditHelper.TotalCredits, out creditsDispo)) return;
                if (creditsDispo <= 0)
                //if (App.CurrentUserEnvironment.User.RemainingRequest <= 0)
                {
                    await _dialogService.ShowMessage(Resources.UserNoRequestLeft, Resources.UserNoRequestLeftTitle);
                    return;
                }

                var result = await _dataService.RestartModeByMode(Mode.Idmode.ToString());
                if (result != 1)
                {
                    await _dialogService.ShowMessage(Resources.Mode_unableToRestartModeDescr, Resources.Mode_unableToRestartModeTitle);
                    return;
                }
                Mode.StatusDefinition_idstatusDefinition = 1;
                Seekios.HasGetLastInstruction = false;
                RaisePropertyChanged("Mode");
                if (ModeRestarted != null) ModeRestarted.Invoke(this, null);
            }
            catch (WebException e)
            {
                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
            }
            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartLookingForDevices()
        {
            _followMeService.TryToStartFollowMe(Seekios);
            BLEConnexionState = ConnexionState.LookingForSeekios;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckIfFollowMeModeAlreadyInUse()
        {
            //Get all follow me mode in use
            var lsSeekiosInfollowMeMode = App.CurrentUserEnvironment.LsMode.Where(
                el => el.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeFollowMe
                   && el.StatusDefinition_idstatusDefinition == 1).ToList();

            //If there isn't active follow me mode we quit
            if (lsSeekiosInfollowMeMode.Count <= 0) return false;

            //Check if this seekios is in one of those follow me mode
            var followMeModeWithThatSeekios = lsSeekiosInfollowMeMode.FirstOrDefault(el => Mode != null && el.Idmode == Mode.Idmode);
            if (followMeModeWithThatSeekios != null)
            {
                //If the follow me mode is actually running on that seekios
                var isModeRunningOnActualUserDevice = followMeModeWithThatSeekios.Device_iddevice == App.CurrentUserEnvironment.Device.Iddevice;
                if (isModeRunningOnActualUserDevice)
                    _dialogService.ShowMessage(Resources.modeFollowMe_followMeAlreadyConfiguredOnActualDevice, Resources.modeFollowMe_followMeAlreadyConfiguredOnActualDeviceTitle);
                else
                    _dialogService.ShowMessage(Resources.modeFollowMe_followMeAlreadyConfiguredOnAnotherDevice, Resources.modeFollowMe_followMeAlreadyConfiguredOnAnotherDeviceTitle);
                return true;
            }

            var followMeModeWithThatDevice = lsSeekiosInfollowMeMode.FirstOrDefault(el => el.Device_iddevice == App.CurrentUserEnvironment.Device.Iddevice);
            if (followMeModeWithThatDevice == null) return false;

            _dialogService.ShowMessage(Resources.modeFollowMe_otherFollowMeAlreadyConfigured, Resources.modeFollowMe_otherFollowMeAlreadyConfiguredTitle);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task<bool> CheckIfBLEIsEnable()
        {
            if (IsBLEEnable) return true;

            var turnOnBLE = await _dialogService.ShowMessage(Resources.modeFollowMe_enableBluetooth, Resources.modeFollowMe_enableBluetoothTitle,
                Resources.modeFollowMe_enableBluetoothConfirm, Resources.CancelText, null);

            if (!turnOnBLE) return false;

            _bluetoothService.EnableBLE();

            return true;
        }

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// Decode the mode Don't Move thread
        /// </summary>
        /// <param name="trame"></param>
        private void DecodeTrame(string trame)
        {
            var refreshTime = 0;
            if (string.IsNullOrEmpty(trame) || !int.TryParse(trame, out refreshTime))
            {
                IsTrackingAfterBLEConnexionLoss = false;
                return;
            }
            IsTrackingAfterBLEConnexionLoss = true;
            RefreshTime = refreshTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="idSeekios"></param>
        private void OnSeekiosChanged(object sender, int idSeekios)
        {
            if (Seekios.Idseekios != idSeekios || !Seekios.HasGetLastInstruction) return;
            _waitForSeekiosResponseTimeout.StopTimeout();
            App.SeekiosChanged -= OnSeekiosChanged;
            BLEConnexionState = ConnexionState.MessageReceivedBySeekios;

            ConnectWithSeekios();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnexionStateChanged(object sender, ConnexionStateChangedEventArgs e)
        {
            BLEConnexionState = e.DeviceConnexionState;
            if (BLEConnexionState == ConnexionState.Connecting)
            {
                _waitForSeekiosConnectionTimeout.StartTimeout(60000, () =>
                {
                    BLEConnexionState = ConnexionState.ConnectionFailed;
                    try
                    {
                        _dataService.DeleteMode(Mode.Idmode.ToString());
                    }
                    catch (WebException ee)
                    {
                        _dialogService.ShowError(ee, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
                    }
                    catch (TimeoutException ee)
                    {
                        _dialogService.ShowError(ee, Resources.FacebookConnectionFailedTitle, Resources.FacebookConnectionFailedButtonText, null);
                    }
                    App.CurrentUserEnvironment.LsMode.Remove(Mode);
                });
            }
            if (BLEConnexionState == ConnexionState.Connected)
            {
                _waitForSeekiosConnectionTimeout.StopTimeout();
            }

            if (BLEConnexionState == ConnexionState.Disconnected)
            {
                //mode concerned
                var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosInFollowMe.Idseekios);
                if (mode == null) return;

                //update user requests amount
                mode.CountOfTriggeredAlert++;
                mode.StatusDefinition_idstatusDefinition = 4;
                mode.LastTriggeredAlertDate = App.CurrentUserEnvironment.ServerCurrentDateTime;
                App.RaiseSeekiosInformationChangedEverywhere(_seekiosInFollowMe.Idseekios);

                //var tokenSource = new CancellationTokenSource();
                //var ct = tokenSource.Token;
                //var t = Task.Factory.StartNew(() =>
                //{
                //    Task.Delay(10000);
                //    if (ct.IsCancellationRequested) return;

                //}, ct);
                //tokenSource.Cancel();
            }
        }

        #endregion

        #region ===== Évènements ==================================================================

        /// <summary>
        /// Event fired when the mode is restarted
        /// </summary>
        public event EventHandler ModeRestarted;

        /// <summary>
        /// Event fired when a new tracking location is added
        /// </summary>
        public event NewFollowMeTrackingLocationAddedEvent NewFollowMeTrackingLocationAddedNotified;

        #endregion

        #region ===== Handlers ====================================================================

        /// <summary>
        /// Method called when a new tracking location is added
        /// </summary>
        /// <param name="uidSeekios"></param>
        /// <param name="batteryLife"></param>
        /// <param name="signalQuality"></param>
        /// <param name="latLong"></param>
        public void OnNewFollowMeTrackingLocationAdded(string uidSeekios, Tuple<int, int> batteryAndSignal, Tuple<double, double, double, double> latLongAltAcc, DateTime dateCommunication)
        {
            if (_dispatcherService == null) return;

            _dispatcherService.Invoke(() =>
            {
                if (string.IsNullOrEmpty(uidSeekios))
                    return;

                //Parse parameters
                double lat = latLongAltAcc.Item1, lon = latLongAltAcc.Item2, altitude = latLongAltAcc.Item3, accuracy = latLongAltAcc.Item4;

                //Get the concerned seekios
                var seekios = App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.UIdSeekios == uidSeekios);
                if (seekios == null) return;

                //Get the mode concerned
                var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
                if (mode == null) return;

                //add location to user environment
                App.CurrentUserEnvironment.LsLocations.Add(new LocationDTO
                {
                    DateLocationCreation = dateCommunication,
                    Latitude = lat,
                    Longitude = lon,
                    Altitude = altitude,
                    Accuracy = accuracy,
                    Mode_idmode = mode.Idmode
                });

                seekios.BatteryLife = batteryAndSignal.Item1;
                seekios.SignalQuality = batteryAndSignal.Item2;
                seekios.DateLastCommunication = dateCommunication;
                seekios.LastKnownLocation_latitude = lat;
                seekios.LastKnownLocation_longitude = lon;
                seekios.LastKnownLocation_altitude = altitude;
                seekios.LastKnownLocation_accuracy = accuracy;
                seekios.LastKnownLocation_dateLocationCreation = dateCommunication;

                App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);

                if (NewFollowMeTrackingLocationAddedNotified != null) NewFollowMeTrackingLocationAddedNotified.Invoke(lat, lon, altitude, accuracy, dateCommunication);

                if (Seekios == null) return;
                if (Seekios.UIdSeekios != uidSeekios) return;
            });
        }

        #endregion
    }

    public delegate void NewFollowMeTrackingLocationAddedEvent(double lat, double lon, double altitude, double accuracy, DateTime dateCommunication);
    */
}
