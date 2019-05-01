//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Android.Content;
//using Android.Bluetooth;
//using System.Threading.Tasks;
//using SeekiosApp.Interfaces;
//using Android.Bluetooth.LE;

//namespace SeekiosApp.Droid.Services
//{

//    public class BluetoothService : IBluetoothService
//    {
//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructeur protected
//        /// </summary>
//        public BluetoothService()
//        {
//            // get a reference to the bluetooth system service
//            this._manager = (BluetoothManager)_appContext.GetSystemService("bluetooth");
//            this._adapter = this._manager.Adapter;
//            this._bleScanHandler = new BLEScanCallbackHandler(new Action<Android.Bluetooth.BluetoothDevice, int, ScanRecord>((device, rssi, scanRecord) =>
//           {
//               //Si le device a déjà été découvert, on quitte
//               if (_discoveredDevices.Select(el => el.Address).Contains(device.Address)) return;

//               var myDevice = new Interfaces.BluetoothDevice
//               {
//                   Address = device.Address,
//                   Name = device.Name,
//                   NativeDeviceObject = device
//               };
//               this._discoveredDevices.Add(myDevice);
//               this.DeviceDiscovered(this, new DeviceDiscoveredEventArgs { Device = myDevice, Rssi = rssi, ScanRecord = scanRecord });
//           }));
//        }

//        #endregion

//        #region ===== Attributs ===================================================================

//        protected BluetoothManager _manager;
//        protected BluetoothAdapter _adapter;
//        private Context _appContext = Android.App.Application.Context;
//        private List<Interfaces.BluetoothDevice> _discoveredDevices = new List<Interfaces.BluetoothDevice>();
//        private BLEScanCallbackHandler _bleScanHandler;

//        #endregion

//        #region ===== Events ======================================================================

//        public event EventHandler ScanTimeoutElapsed = delegate { };
//        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered = delegate { };
//        public event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged = delegate { };
//        public event EventHandler<ServicesDiscoveredEventArgs> ServicesDiscovered = delegate { };

//        #endregion

//        #region ===== Propriétés =================================================================

//        /// <summary>
//        /// Whether or not we're currently scanning for peripheral devices
//        /// </summary>
//        /// <value><c>true</c> if this instance is scanning; otherwise, <c>false</c>.</value>
//        public bool IsScanning
//        {
//            get { return this._isScanning; }
//        }
//        protected bool _isScanning = false;

//        /// <summary>
//        /// Gets the discovered peripherals.
//        /// </summary>
//        /// <value>The discovered peripherals.</value>
//        public List<Interfaces.BluetoothDevice> DiscoveredDevices
//        {
//            get { return this._discoveredDevices; }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool BLEExists
//        {
//            get
//            {
//                return this._adapter ==null ? false : this._adapter.BluetoothLeScanner != null;
//            }
//        }

//        #endregion

//        #region ===== Méthodes Publiques ==========================================================

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <returns></returns>
//        public async Task BeginScanningForDevices(int scanTimeout)
//        {
//            Console.WriteLine("BluetoothLEManager: Starting a scan for devices.");

//            // clear out the list
//            this._discoveredDevices = new List<Interfaces.BluetoothDevice>();

//            if (!(this._adapter != null && this._adapter.BluetoothLeScanner !=null)) return;
//            this._isScanning = true;

//            this._adapter.BluetoothLeScanner.StartScan(_bleScanHandler);

//            // in 10 seconds, stop the scan
//            await Task.Delay(scanTimeout);

//            // if we're still scanning
//            if (this._isScanning)
//            {
//                Console.WriteLine("BluetoothLEManager: Scan timeout has elapsed.");
//                this._adapter.BluetoothLeScanner.StopScan(_bleScanHandler);
//                this._isScanning = false;
//                this.ScanTimeoutElapsed(this, new EventArgs());
//            }
//        }

//        /// <summary>
//        /// Stops the Central Bluetooth Manager from scanning for more devices. Automatically
//        /// called after 10 seconds to prevent battery drain. 
//        /// </summary>
//        public void StopScanningForDevices()
//        {
//            Console.WriteLine("BluetoothLEManager: Stopping the scan for devices.");
//            if (this._adapter == null || this._adapter.BluetoothLeScanner == null) return;
//            this._isScanning = false;

//            this._adapter.BluetoothLeScanner.StopScan(_bleScanHandler);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="device"></param>
//        /// <param name="autoReconnect"></param>
//        /// <returns></returns>
//        public bool StartGattConnexion(Interfaces.BluetoothDevice device, bool autoReconnect)
//        {
//            var nativeDeviceObject = device.NativeDeviceObject as Android.Bluetooth.BluetoothDevice;
//            if (nativeDeviceObject == null) return false;
//            device.NativeGattConnexionObject = nativeDeviceObject.ConnectGatt(_appContext, autoReconnect,
//                new BluetoothLeConnectionCallBackHandler(_appContext, ConnexionStateChanged, ServicesDiscovered, device));
//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="device"></param>
//        /// <returns></returns>
//        public bool StopGattConnexion(Interfaces.BluetoothDevice device)
//        {
//            var nativeGattConnexionObject = device.NativeGattConnexionObject as BluetoothGatt;
//            if (nativeGattConnexionObject == null) return false;
//            nativeGattConnexionObject.Close();
//            return true;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="device"></param>
//        /// <returns></returns>
//        public bool DiscoverServices(Interfaces.BluetoothDevice device)
//        {
//            var nativeGattConnexionObject = device.NativeGattConnexionObject as BluetoothGatt;
//            if (nativeGattConnexionObject == null) return false;
//            return nativeGattConnexionObject.DiscoverServices();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="device"></param>
//        /// <param name="charac"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public bool WriteCharacteristic(Interfaces.BluetoothDevice device, BLECharacteristic charac, string value)
//        {
//            var nativeGattConnexionObject = device.NativeGattConnexionObject as BluetoothGatt;
//            var nativeCharacteristicObject = charac.NativeBLECharacteristicObject as BluetoothGattCharacteristic;
//            if (nativeCharacteristicObject == null || nativeGattConnexionObject == null) return false;

//            var isValueSet = nativeCharacteristicObject.SetValue(value);
//            if (!isValueSet) return false;
//            return nativeGattConnexionObject.WriteCharacteristic(nativeCharacteristicObject);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="device"></param>
//        /// <param name="charac"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public bool WriteCharacteristic(Interfaces.BluetoothDevice device, BLECharacteristic charac, byte[] value)
//        {
//            var nativeGattConnexionObject = device.NativeGattConnexionObject as BluetoothGatt;
//            var nativeCharacteristicObject = charac.NativeBLECharacteristicObject as BluetoothGattCharacteristic;
//            if (nativeCharacteristicObject == null || nativeGattConnexionObject == null) return false;

//            var isValueSet = nativeCharacteristicObject.SetValue(value);
//            if (!isValueSet) return false;
//            return nativeGattConnexionObject.WriteCharacteristic(nativeCharacteristicObject);
//        }

//        /// <summary>
//        /// Enable the BLE
//        /// </summary>
//        public void EnableBLE()
//        {
//            if(this._adapter!=null) this._adapter.Enable();
//        }

//        /// <summary>
//        /// Disable the BLE
//        /// </summary>
//        public void DisableBLE()
//        {
//            if(this._adapter!=null) this._adapter.Disable();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public bool IsBluetoothEnable { get { return _adapter == null ? false : this._adapter.IsEnabled; } }

//        #endregion

//        private class BLEScanCallbackHandler : ScanCallback
//        {
//            private Action<Android.Bluetooth.BluetoothDevice, int, ScanRecord> _onDeviceDetected;

//            public BLEScanCallbackHandler(Action<Android.Bluetooth.BluetoothDevice, int, ScanRecord> onDeviceDetected)
//            {
//                _onDeviceDetected = onDeviceDetected;
//            }

//            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
//            {
//                if (_onDeviceDetected == null) return;
//                _onDeviceDetected(result.Device, result.Rssi, result.ScanRecord);

//            }

//            //[Register("onScanFailed", "(I)V", "GetOnScanFailed_IHandler")]
//            //public virtual void OnScanFailed([GeneratedEnum] ScanFailure errorCode);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private class BluetoothLeConnectionCallBackHandler : BluetoothGattCallback
//        {
//            private Context _context;
//            private EventHandler<ConnexionStateChangedEventArgs> _connexionStateChanged;
//            private EventHandler<ServicesDiscoveredEventArgs> _servicesDescovered;
//            private Interfaces.BluetoothDevice _device;

//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="context"></param>
//            /// <param name="connexionStateChanged"></param>
//            /// <param name="servicesDescovered"></param>
//            /// <param name="device"></param>
//            public BluetoothLeConnectionCallBackHandler(Context context, EventHandler<ConnexionStateChangedEventArgs> connexionStateChanged
//                , EventHandler<ServicesDiscoveredEventArgs> servicesDescovered, Interfaces.BluetoothDevice device)
//            {
//                _context = context;
//                _connexionStateChanged = connexionStateChanged;
//                _servicesDescovered = servicesDescovered;
//                _device = device;
//            }

//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="gatt"></param>
//            /// <param name="status"></param>
//            /// <param name="newState"></param>
//            public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
//            {
//                ConnexionState state = ConnexionState.Disconnected;
//                switch (newState)
//                {
//                    case ProfileState.Connected:
//                        state = ConnexionState.Connected;
//                        break;
//                    case ProfileState.Connecting:
//                        state = ConnexionState.Connecting;
//                        break;
//                    case ProfileState.Disconnected:
//                        state = ConnexionState.Disconnected;
//                        break;
//                    case ProfileState.Disconnecting:
//                        state = ConnexionState.Disconnecting;
//                        break;
//                    default:
//                        break;
//                }
//                if (_connexionStateChanged != null)
//                    _connexionStateChanged(this, new ConnexionStateChangedEventArgs()
//                    {
//                        Device = _device,
//                        DeviceConnexionState = state
//                    });
//            }

//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="gatt"></param>
//            /// <param name="status"></param>
//            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
//            {
//                if (status == GattStatus.Success)
//                {
//                    var lsServices = new List<BLEService>();
//                    foreach (var service in gatt.Services)
//                    {
//                        var lsCharac = new List<BLECharacteristic>();
//                        foreach (var charac in service.Characteristics)
//                        {
//                            lsCharac.Add(new BLECharacteristic()
//                            {
//                                Uuid = charac.Uuid.ToString(),
//                                NativeBLECharacteristicObject = charac
//                            });
//                        }
//                        lsServices.Add(new BLEService()
//                        {
//                            LsCarac = lsCharac,
//                            Uuid = service.Uuid.ToString(),
//                            NativeBLEServiceObject = service
//                        });
//                    }

//                    _servicesDescovered(this, new ServicesDiscoveredEventArgs() { Services = lsServices });
//                }
//            }

//            //Déclanché après l'appel de la fonction ReadRemoteRssi() sur un objet BluetoothGatt (récupéré lors de la connexion)
//            //Indicateur de puissance du signal BLE
//            //public virtual void OnReadRemoteRssi(BluetoothGatt gatt, int rssi, [GeneratedEnum] GattStatus status);

//            //Déclanché après l'appel de la fonction ReadCharacteristic() sur un objet BluetoothGatt (récupéré lors de la connexion)
//            //public virtual void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, [GeneratedEnum] GattStatus status);
//        }
//    }
//}