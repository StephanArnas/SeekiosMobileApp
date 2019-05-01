//using System;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using System.Threading;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Interfaces;
//using Android.Util;

//namespace SeekiosApp.Droid.Services
//{
//    [Service]
//    public class FollowMeBackgroundService : Service
//    {
//        #region Properties
        
//        /// <summary>
//        /// Seekios to connect with
//        /// </summary>
//        public static SeekiosDTO Seekios { get; set; }

//        /// <summary>
//        /// Seekios to connect with
//        /// </summary>
//        public static IDataService DataService { get; set; }

//        /// <summary>
//        /// True if the seekios is working
//        /// </summary>
//        private static bool _isWorking = false;
//        public static bool IsWorking
//        {
//            get { return _isWorking; }
//            private set { _isWorking = value; }
//        }

//        /// <summary>
//        /// Timeout for the devices scan
//        /// </summary>
//        private static int _scanTimeout = 60000;
//        public static int ScanTimeout
//        {
//            get { return _scanTimeout; }
//            set { _scanTimeout = value; }
//        }

//        /// <summary>
//        /// True if the seekios device is found
//        /// </summary>
//        public static bool SeekiosFound { get; private set; }

//        /// <summary>
//        /// Raise when gatt connexion state change
//        /// </summary>
//        public static event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;

//        #endregion

//        #region Attributes

//        /// <summary>
//        /// 
//        /// </summary>
//        private BluetoothDevice _device;

//        /// <summary>
//        /// 
//        /// </summary>
//        private BluetoothService _bluetoothService;

//        #endregion

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="intent"></param>
//        /// <param name="flags"></param>
//        /// <param name="startId"></param>
//        /// <returns></returns>
//        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
//        {
//            Log.Debug("FollowMeBackgroundService", "on start");
//            DoWork();
//            return StartCommandResult.Sticky;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void DoWork()
//        {
//            var notifTitle = Resources.GetString(Resource.String.serviceFollowMe_notificationTitle);
//            var notifContent = string.Format(Resources.GetString(Resource.String.serviceFollowMe_notificationContent), Seekios.SeekiosName);

//            var notificationBuilder = new Notification.Builder(this)
//                .SetSmallIcon(Resource.Drawable.FollowMeNotifIcon)
//                .SetColor(Resources.GetColor(Resource.Color.primary).ToArgb())
//                .SetContentTitle(notifTitle)
//                .SetContentText(notifContent);

//            StartForeground(123456123, notificationBuilder.Build());
//            new Thread(new ThreadStart(() =>
//            {
//                IsWorking = true;

//                if (DataService == null || Seekios == null)
//                {
//                    IsWorking = false;
//                    StopSelf();
//                    return;
//                }
//                Log.Debug("FollowMeBackgroundService", "SeekiosProduction OK");

//                _bluetoothService = new BluetoothService();
//                _bluetoothService.DeviceDiscovered += OnDeviceDiscovered;
//                _bluetoothService.ScanTimeoutElapsed += OnScanTimeoutElapsed;
//                _bluetoothService.ConnexionStateChanged += OnConnexionStateChanged;

//                Log.Debug("FollowMeBackgroundService", "registering OK");
//                RaiseConnexionStateChanged(ConnexionState.LookingForSeekios);

//                SeekiosFound = false;

//                Log.Debug("FollowMeBackgroundService", "BeginScanningForDevices...");
//                _bluetoothService.BeginScanningForDevices(ScanTimeout);

//                Log.Debug("FollowMeBackgroundService", "waiting...");
//                while (IsWorking) { Thread.Sleep(5000); };
//                StopSelf();
//            })).Start();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="state"></param>
//        public void RaiseConnexionStateChanged(ConnexionState state)
//        {
//            Log.Debug("FollowMeBackgroundService", "RaiseConnexionStateChanged : " + state.ToString());
//            if (ConnexionStateChanged != null)
//                ConnexionStateChanged.Invoke(this, new ConnexionStateChangedEventArgs
//                {
//                    Device = _device,
//                    DeviceConnexionState = state
//                });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnScanTimeoutElapsed(object sender, EventArgs e)
//        {
//            Log.Debug("FollowMeBackgroundService", "OnScanTimeoutElapsed");
//            RaiseConnexionStateChanged(ConnexionState.SeekiosUnreachable);
//            IsWorking = false;
//            StopSelf();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnDeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
//        {
//            Log.Debug("FollowMeBackgroundService", "OnDeviceDiscovered");
//            //Si le device a déjà été découvert, on quitte
//            if (e.Device.Address != Seekios.MacAddress) return;

//            Log.Debug("FollowMeBackgroundService", "good device");
//            _bluetoothService.StopScanningForDevices();
//            Log.Debug("FollowMeBackgroundService", "StopScanningForDevices ok");
//            _device = e.Device;
//            SeekiosFound = true;

//            RaiseConnexionStateChanged(ConnexionState.Connecting);

//            Log.Debug("FollowMeBackgroundService", "StartGattConnexion...");
//            _bluetoothService.StartGattConnexion(_device, true);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="state"></param>
//        private async void OnConnexionStateChanged(object sender, ConnexionStateChangedEventArgs state)
//        {
//            Log.Debug("FollowMeBackgroundService", "OnConnexionStateChanged : " + state.ToString());
//            RaiseConnexionStateChanged(state.DeviceConnexionState);

//            if (state.DeviceConnexionState != ConnexionState.Disconnected) return;

//            //TODO : alert cloud that connexion is broken
//            var rs = await DataService.NotifyBLEConnexionLost(Seekios.Idseekios);

//            IsWorking = false;
//            StopSelf();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="intent"></param>
//        /// <returns></returns>
//        public override IBinder OnBind(Intent intent)
//        {
//            return null;
//        }
//    }
//}