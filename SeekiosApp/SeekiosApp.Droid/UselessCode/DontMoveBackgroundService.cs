//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Util;
//using SeekiosApp.Model.DTO;
//using System.Threading;
//using SeekiosApp.Interfaces;
//using SeekiosApp.Constants;
//using Newtonsoft.Json;
//using SeekiosApp.Enum;
//using SeekiosApp.ViewModel;

//namespace SeekiosApp.Droid.Services
//{
//    [Service]
//    public class DontMoveBackgroundService : Service
//    {
//        #region Properties

//        public static bool IsServiceRunning { get; set; }

//        #endregion

//        #region Attributes

//        /// <summary>
//        /// 
//        /// </summary>
//        private const int _SCAN_TIMEOUT = 999999;

//        /// <summary>
//        /// 
//        /// </summary>
//        private BluetoothDevice _device;

//        /// <summary>
//        /// 
//        /// </summary>
//        private BluetoothService _bluetoothService;

//        /// <summary>
//        /// 
//        /// </summary>
//        private Thread _writeCharacteristicThread;

//        /// <summary>
//        /// Seekios to connect with
//        /// </summary>
//        private static List<SeekiosDTO> _seekiosInDontMove = new List<SeekiosDTO>();

//        /// <summary>
//        /// 
//        /// </summary>
//        private static SeekiosDTO _seekiosConnected;

//        /// <summary>
//        /// True if the ble scan is enable
//        /// </summary>
//        private bool _bleScanEnable = false;

//        #endregion

//        #region LifeCycle

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="intent"></param>
//        /// <param name="flags"></param>
//        /// <param name="startId"></param>
//        /// <returns></returns>
//        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
//        {
//            Log.Debug("DontMoveBackgroundService", "on start");

//            IsServiceRunning = true;

//            var notifTitle = Resources.GetString(Resource.String.dontMoveBackgroundService_notificationTitle);
//            var notifContent = string.Format(Resources.GetString(Resource.String.dontMoveBackgroundService_notificationContent), "");

//            var notificationBuilder = new Notification.Builder(this)
//                //.SetSmallIcon(Resource.Drawable.ModeDontMove)
//                .SetSmallIcon(Resource.Drawable.FollowMeNotifIcon)
//                .SetColor(Resources.GetColor(Resource.Color.primary).ToArgb())
//                .SetContentTitle(notifTitle)
//                .SetContentText(notifContent);

//            StartForeground(45641678, notificationBuilder.Build());

//            new Thread(new ThreadStart(() =>
//            {
//                InitializeSeekiosList();
//            })).Start();

//            return StartCommandResult.Sticky;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="intent"></param>
//        /// <returns></returns>
//        public override IBinder OnBind(Intent intent)
//        {
//            return new DontMoveBackgroundServiceBinder(this);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="rootIntent"></param>
//        public virtual void OnTaskRemoved(Intent rootIntent)
//        {
//            IsServiceRunning = false;
//            base.OnTaskRemoved(rootIntent);
//        }

//        #endregion

//        #region Bluetooth

//        /// <summary>
//        /// 
//        /// </summary>
//        private bool InitializeBLE()
//        {
//            Log.Debug("DontMoveBackgroundService", "InitializeBLE");

//            _bluetoothService = new BluetoothService();
//            if (!_bluetoothService.IsBluetoothEnable) _bluetoothService.EnableBLE();
//            if (!_bluetoothService.BLEExists)
//            {
//                _bluetoothService = null;
//                StopService();
//                return false;
//            }
//            _bluetoothService.DeviceDiscovered += OnDeviceDiscovered; ;
//            _bluetoothService.ScanTimeoutElapsed += OnScanTimeoutElapsed; ;
//            _bluetoothService.ConnexionStateChanged += OnConnexionStateChanged;
//            _bluetoothService.ServicesDiscovered += OnServicesDiscovered;
//            Log.Debug("DontMoveBackgroundService", "InitializeBLE OK");
//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void StartBLEScan()
//        {
//            if (_bluetoothService == null)
//                if (!InitializeBLE()) return;
//            Log.Debug("DontMoveBackgroundService", "BeginScanningForDevices...");
//            _bluetoothService.BeginScanningForDevices(_SCAN_TIMEOUT);
//            _bleScanEnable = true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void StopBLEScan()
//        {
//            if (_bluetoothService == null)
//                if (!InitializeBLE()) return;
//            Log.Debug("DontMoveBackgroundService", "BeginScanningForDevices...");
//            _bluetoothService.StopScanningForDevices();
//            _bleScanEnable = false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnServicesDiscovered(object sender, ServicesDiscoveredEventArgs e)
//        {
//            Log.Debug("DontMoveBackgroundService", "OnServicesDiscovered");
//            var service = e.Services.FirstOrDefault(el => el.Uuid == "5290e830-6449-11e6-8b77-86f30ca893d3");// d393a80c-f386-778b-e611-496430e89052
//            if (service == null) return;
//            var charac = service.LsCarac.FirstOrDefault(el => el.Uuid == "6570267c-644f-11e6-8b77-86f30ca893d3");// d393a80c-f386-778b-e611-4f647c267065
//            if (charac == null) return;
//            Log.Debug("DontMoveBackgroundService", "OnServicesDiscovered service exist");

//            if (_writeCharacteristicThread != null && _writeCharacteristicThread.IsAlive)
//                _writeCharacteristicThread.Abort();
//            _writeCharacteristicThread = new Thread(new ThreadStart(() =>
//            {
//                var timestamp = 0;
//                var signature = ModeDontMoveViewModel.SeekiosAuthenticationSignature(_seekiosConnected.UIdSeekios, out timestamp);
//                var timestampBytes = GetBytes(timestamp);
//                var signatureBytes = HexStringToByteArray(signature);
//                var lsBytes = new List<byte>();
//                lsBytes.AddRange(timestampBytes);
//                lsBytes.AddRange(signatureBytes);

//                while (_bluetoothService.WriteCharacteristic(_device, charac, lsBytes.ToArray()) != true)
//                {
//                    Thread.Sleep(500);
//                }
//                Log.Debug("DontMoveBackgroundService", "WriteCharacteristic ok");
//            }));
//            _writeCharacteristicThread.Start();
//        }

//        private static byte[] HexStringToByteArray(string hex)
//        {
//            return Enumerable.Range(0, hex.Length)
//                             .Where(x => x % 2 == 0)
//                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
//                             .ToArray();
//        }

//        private static byte[] GetBytes(string str)
//        {
//            byte[] bytes = new byte[str.Length * sizeof(char)];
//            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
//            return bytes;
//        }

//        private static byte[] GetBytes(int intVal)
//        {
//            byte[] intValBytes = BitConverter.GetBytes(intVal);
//            if (!BitConverter.IsLittleEndian)
//                Array.Reverse(intValBytes);
//            return intValBytes;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnConnexionStateChanged(object sender, ConnexionStateChangedEventArgs e)
//        {
//            Log.Debug("DontMoveBackgroundService", "OnConnexionStateChanged " + e.DeviceConnexionState.ToString());
//            if (e.DeviceConnexionState == ConnexionState.Disconnected
//                || e.DeviceConnexionState == ConnexionState.Disconnecting)
//            {
//                if (_writeCharacteristicThread != null && _writeCharacteristicThread.IsAlive)
//                    _writeCharacteristicThread.Abort();
//                StartBLEScan();
//                return;
//            }
//            if (e.DeviceConnexionState != ConnexionState.Connected) return;

//            _bleScanEnable = false;

//            _bluetoothService.DiscoverServices(e.Device);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnScanTimeoutElapsed(object sender, EventArgs e)
//        {
//            Log.Debug("DontMoveBackgroundService", "OnScanTimeoutElapsed");
//            if (_seekiosInDontMove.Count <= 0)
//            {
//                _bleScanEnable = false;
//                return;
//            }

//            _bluetoothService.BeginScanningForDevices(_SCAN_TIMEOUT);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnDeviceDiscovered(object sender, DeviceDiscoveredEventArgs e)
//        {
//            Log.Debug("DontMoveBackgroundService", "OnDeviceDiscovered " + e.Device.Name + " " + e.Device.Address);
//            var seekiosAndProduction = _seekiosInDontMove.FirstOrDefault(el => el.MacAddress == e.Device.Address);
//            if (seekiosAndProduction == null) return;

//            _device = e.Device;
//            _bluetoothService.StartGattConnexion(e.Device, false);
//            _seekiosConnected = seekiosAndProduction;
//        }

//        #endregion

//        #region Public methods

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seekios"></param>
//        /// <param name="production"></param>
//        public void AddSeekiosInDontMove(SeekiosDTO seekios)
//        {
//            if (_seekiosInDontMove.FirstOrDefault(el => el.Idseekios == seekios.Idseekios) != null) return;
//            if (_bluetoothService == null && !InitializeBLE()) return;
//            Log.Debug("DontMoveBackgroundService", "AddSeekiosInDontMove " + seekios.SeekiosName + " " + seekios.MacAddress);
//            _seekiosInDontMove.Add(seekios);
//            if (!_bleScanEnable) StartBLEScan();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seekios"></param>
//        /// <param name="production"></param>
//        public void AddSeekiosInDontMove(List<SeekiosDTO> lsSeekios)
//        {
//            foreach (var seekios in lsSeekios)
//            {
//                AddSeekiosInDontMove(seekios);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seekios"></param>
//        /// <param name="production"></param>
//        public void RemoveSeekiosInDontMove(SeekiosDTO seekios)
//        {
//            var seekiosToRemove = _seekiosInDontMove.FirstOrDefault(el => el.Idseekios == seekios.Idseekios);
//            if (seekiosToRemove == null) return;

//            Log.Debug("DontMoveBackgroundService", "RemoveSeekiosInDontMove " + seekiosToRemove.SeekiosName + " " + seekiosToRemove.MacAddress);
//            _seekiosInDontMove.Remove(seekiosToRemove);

//            if (_seekiosInDontMove.Count <= 0)
//            {
//                StopBLEScan();
//                StopService();
//            }

//            return;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="seekios"></param>
//        /// <param name="production"></param>
//        public void RemoveSeekiosInDontMove(List<SeekiosDTO> lsSeekios)
//        {
//            foreach (var seekios in lsSeekios)
//            {
//                RemoveSeekiosInDontMove(seekios);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void ReinitializeService()
//        {
//            _seekiosInDontMove.Clear();
//            InitializeSeekiosList();
//        }

//        #endregion

//        #region Privates methods

//        /// <summary>
//        /// 
//        /// </summary>
//        private void InitializeSeekiosList()
//        {
//            var lsSeekiosInDontMove = new List<SeekiosDTO>();

//            //Get saved data 
//            var _saveDataService = new SaveDataService();
//            _saveDataService.Init(Application.Context);

//            if (!_saveDataService.Contains(SaveDataConstants.UserEnvironment)) return;

//            var json = _saveDataService.GetData(SaveDataConstants.UserEnvironment);
//            if (string.IsNullOrEmpty(json)) return;

//            var userEnvironment = JsonConvert.DeserializeObject<UserEnvironmentDTO>(json);
//            if (userEnvironment == null) return;

//            //Check if there is any seekios in don't move mode with RAS status
//            lsSeekiosInDontMove = userEnvironment.LsSeekios.Where(
//                el => userEnvironment.LsMode.FirstOrDefault(
//                    m => m.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove
//                    && m.Seekios_idseekios == el.Idseekios
//                    && m.StatusDefinition_idstatusDefinition == 1) != null).ToList();
//            if (lsSeekiosInDontMove.Count <= 0) return;

//            AddSeekiosInDontMove(lsSeekiosInDontMove);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void StopService()
//        {
//            IsServiceRunning = false;
//            StopForeground(true);
//            StopSelf();
//        }

//        #endregion
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class DontMoveBackgroundServiceBinder : Binder
//    {
//        public DontMoveBackgroundServiceBinder(DontMoveBackgroundService service)
//        {
//            _service = service;
//        }

//        protected DontMoveBackgroundService _service;
//        public DontMoveBackgroundService Service
//        {
//            get { return _service; }
//        }

//        public bool IsBound { get; set; }
//    }

//    /// <summary>
//    /// 
//    /// </summary>
//    public class DontMoveBackgroundServiceConnection : Java.Lang.Object, IServiceConnection, IDontMoveService
//    {
//        #region Constructor

//        /// <summary>
//        /// Bind itself with the don't move background service 
//        /// </summary>
//        public DontMoveBackgroundServiceConnection()
//        {
//            Application.Context.BindService(new Intent(Application.Context, typeof(DontMoveBackgroundService)), this, Bind.AutoCreate);
//        }

//        #endregion

//        #region Attributes

//        /// <summary>Binder for communication with the service</summary>
//        private DontMoveBackgroundServiceBinder _binder;

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Action to start the service
//        /// </summary>
//        public static Action StartDontMoveService;

//        #endregion

//        #region Public methods

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="name"></param>
//        /// <param name="service"></param>
//        public void OnServiceConnected(ComponentName name, IBinder service)
//        {
//            DontMoveBackgroundServiceBinder serviceBinder = service as DontMoveBackgroundServiceBinder;

//            if (serviceBinder != null)
//            {
//                this._binder = serviceBinder;
//                this._binder.IsBound = true;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="name"></param>
//        public void OnServiceDisconnected(ComponentName name)
//        {
//            this._binder.IsBound = false;
//        }

//        /// <summary>
//        /// Start the don't move background service
//        /// </summary>
//        public void StartService()
//        {
//            if (StartDontMoveService == null || IsServiceRunning()) return;
//            StartDontMoveService();
//        }

//        /// <summary>
//        /// Add a seekios in don't move mode to the service
//        /// </summary>
//        /// <param name="seekios"></param>
//        public void AddSeekiosInDontMove(SeekiosDTO seekios)
//        {
//            _binder.Service.AddSeekiosInDontMove(seekios);
//        }

//        /// <summary>
//        /// Add a list of seekios in don't move mode to the service
//        /// </summary>
//        /// <param name="lsSeekios"></param>
//        public void AddSeekiosInDontMove(List<SeekiosDTO> lsSeekios)
//        {
//            _binder.Service.AddSeekiosInDontMove(lsSeekios);
//        }

//        /// <summary>
//        /// Remove a seekios in don't move mode from the service
//        /// </summary>
//        /// <param name="seekios"></param>
//        public void RemoveSeekiosInDontMove(SeekiosDTO seekios)
//        {
//            _binder.Service.RemoveSeekiosInDontMove(seekios);
//        }

//        /// <summary>
//        /// Remove a list of seekios in don't move mode from the service
//        /// </summary>
//        /// <param name="lsSeekios"></param>
//        public void RemoveSeekiosInDontMove(List<SeekiosDTO> lsSeekios)
//        {
//            _binder.Service.RemoveSeekiosInDontMove(lsSeekios);
//        }

//        /// <summary>
//        /// Check if the service is launch
//        /// </summary>
//        /// <returns></returns>
//        public bool IsServiceRunning()
//        {
//            return DontMoveBackgroundService.IsServiceRunning;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public void ReinitializeService()
//        {
//            _binder.Service.ReinitializeService();
//        }

//        #endregion
//    }
//}