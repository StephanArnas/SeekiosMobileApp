//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace SeekiosApp.Interfaces
//{
//    public interface IBluetoothService
//    {
//        bool IsBluetoothEnable { get; }
        
//        void EnableBLE();
//        void DisableBLE();
        
//        List<BluetoothDevice> DiscoveredDevices { get; }
//        Task BeginScanningForDevices(int scanTimeout);
//        void StopScanningForDevices();
        
//        bool StartGattConnexion(BluetoothDevice device, bool autoReconnect);
//        bool StopGattConnexion(BluetoothDevice device);
        
//        event EventHandler ScanTimeoutElapsed;
//        event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;
//        event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;
//    }

//    public class DeviceDiscoveredEventArgs : EventArgs
//    {
//        public BluetoothDevice Device;
//        public int Rssi;
//        public object ScanRecord;

//        public DeviceDiscoveredEventArgs() : base()
//        { }
//    }

//    public class ConnexionStateChangedEventArgs : EventArgs
//    {
//        public BluetoothDevice Device;
//        public ConnexionState DeviceConnexionState;

//        public ConnexionStateChangedEventArgs() : base()
//        { }
//    }

//    public class ServicesDiscoveredEventArgs : EventArgs
//    {
//        public List<BLEService> Services;

//        public ServicesDiscoveredEventArgs() : base()
//        { }
//    }

//    public class BLEService
//    {
//        public string Uuid { get; set; }
//        public List<BLECharacteristic> LsCarac { get; set; }
//        public object NativeBLEServiceObject { get; set; }
//    }

//    public class BLECharacteristic
//    {
//        public string Uuid { get; set; }
//        public object NativeBLECharacteristicObject { get; set; }
//    }

//    public class BluetoothDevice
//    {
//        public string Name { get; set; }
//        public string Address { get; set; }
//        public object NativeDeviceObject { get; set; }
//        public object NativeGattConnexionObject { get; set; }
//    }

//    public enum ConnexionState
//    {
//        None,
//        MessageReceivedBySeekios,
//        MessageNotReceivedBySeekios,
//        LookingForSeekios,
//        SeekiosUnreachable,
//        Disconnecting,
//        Disconnected,
//        Connecting,
//        ConnectionFailed,
//        Connected,
//    }
//}
