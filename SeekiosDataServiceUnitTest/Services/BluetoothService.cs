using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosDataServiceUnitTest.Services
{
    class BluetoothService : IBluetoothService
    {
        public List<BluetoothDevice> DiscoveredDevices
        {
            get
            {
                return new List<BluetoothDevice>();
            }
        }

        public bool IsBluetoothEnable
        {
            get
            {
                return true;
            }
        }

        public event EventHandler<ConnexionStateChangedEventArgs> ConnexionStateChanged;
        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;
        public event EventHandler ScanTimeoutElapsed;

        public Task BeginScanningForDevices(int scanTimeout)
        {
            return Task.FromResult<object>(null);
        }

        public void DisableBLE()
        {
           
        }

        public void EnableBLE()
        {
            
        }

        public bool StartGattConnexion(BluetoothDevice device, bool autoReconnect)
        {
            return true;
        }

        public bool StopGattConnexion(BluetoothDevice device)
        {
            return true;
        }

        public void StopScanningForDevices()
        {
            
        }
    }
}
