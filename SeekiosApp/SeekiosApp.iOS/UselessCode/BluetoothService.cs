//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using SeekiosApp.Interfaces;

//namespace SeekiosApp.iOS
//{
//	public class BluetoothService : IBluetoothService
//	{
//		public BluetoothService()
//		{
//		}

//		public bool IsBluetoothEnable
//		{

//			get { return IsBluetoothEnable; }
//			//return true;
//		}

//		List<BluetoothDevice> IBluetoothService.DiscoveredDevices
//		{
//			get
//			{
//				throw new NotImplementedException();
//			}
//		}



//		event EventHandler<ConnexionStateChangedEventArgs> IBluetoothService.ConnexionStateChanged
//		{
//			add
//			{
//				throw new NotImplementedException();
//			}

//			remove
//			{
//				throw new NotImplementedException();
//			}
//		}

//		event EventHandler<DeviceDiscoveredEventArgs> IBluetoothService.DeviceDiscovered
//		{
//			add
//			{
//				throw new NotImplementedException();
//			}

//			remove
//			{
//				throw new NotImplementedException();
//			}
//		}

//		event EventHandler IBluetoothService.ScanTimeoutElapsed
//		{
//			add
//			{
//				throw new NotImplementedException();
//			}

//			remove
//			{
//				throw new NotImplementedException();
//			}
//		}

//		Task IBluetoothService.BeginScanningForDevices(int scanTimeout)
//		{
//			throw new NotImplementedException();
//		}

//		void IBluetoothService.DisableBLE()
//		{
//			throw new NotImplementedException();
//		}

//		void IBluetoothService.EnableBLE()
//		{
//			throw new NotImplementedException();
//		}

//		bool IBluetoothService.StartGattConnexion(BluetoothDevice device, bool autoReconnect)
//		{
//			throw new NotImplementedException();
//		}

//		bool IBluetoothService.StopGattConnexion(BluetoothDevice device)
//		{
//			throw new NotImplementedException();
//		}

//		void IBluetoothService.StopScanningForDevices()
//		{
//			throw new NotImplementedException();
//		}
//	}
//}

