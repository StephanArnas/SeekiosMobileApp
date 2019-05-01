//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SeekiosApp.Interfaces;

//namespace SeekiosApp.Droid.Helper
//{
//    public class DesignBluetoothService : IBluetoothService
//    {
//        private List<BluetoothDevice> _discoveredDevices = new List<BluetoothDevice>();

//        public DesignBluetoothService()
//        {
//            InitializeData();
//        }

//        public List<BluetoothDevice> DiscoveredDevices { get { return _discoveredDevices; } }

//        public bool IsBluetoothEnable
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;
//        public event EventHandler ScanTimeoutElapsed;
//        public event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;

//        public async Task BeginScanningForDevices(int scanTimeout)
//        {
//            Console.WriteLine("Start Begin Scan ...");
//        }

//        public void StopScanningForDevices()
//        {
//            Console.WriteLine("Stop Begin Scan ...");
//        }

//        public void InitializeData()
//        {
//            _discoveredDevices = new List<BluetoothDevice>();

//            _discoveredDevices.Add(new BluetoothDevice
//            {
//                Address = "01:00:0C:CC:CC:CC",
//                Name = "SEEKIOS_4687954654",
//            });

//            _discoveredDevices.Add(new BluetoothDevice
//            {
//                Address = "01:00:0C:CC:CC:CC",
//                Name = "SEEKIOS_8949416198",
//            });

//            _discoveredDevices.Add(new BluetoothDevice
//            {
//                Address = "01:80:C2:00:00:00",
//                Name = "SEEKIOS_316516198",
//            });

//            _discoveredDevices.Add(new BluetoothDevice
//            {
//                Address = "01:80:C2:00:00:00",
//                Name = "SEEKIOS_7895466198",
//            });
//        }

//        public bool StartGattConnexion(BluetoothDevice device, bool autoReconnect)
//        {
//            throw new NotImplementedException();
//        }

//        public bool StopGattConnexion(BluetoothDevice device)
//        {
//            throw new NotImplementedException();
//        }

//        public void EnableBLE()
//        {
//            throw new NotImplementedException();
//        }

//        public void DisableBLE()
//        {
//            throw new NotImplementedException();
//        }
//    }
//}