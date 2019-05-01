using Foundation;
using Plugin.DeviceInfo;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SeekiosApp.iOS.Helper
{
    public class DeviceInfoHelper
    {
        public static string Platform { get { return CrossDeviceInfo.Current.Platform.ToString(); } }
        public static string DeviceModel { get { return CrossDeviceInfo.Current.Model; } }
        public static string Version { get { return CrossDeviceInfo.Current.Version; } }
        public static string CountryCode
        {
            get
            {
                return NSLocale.CurrentLocale.LanguageCode;
            }
        }
        public static string GetDeviceUniqueId
        {
            get
            {
                return UIKit.UIDevice.CurrentDevice.IdentifierForVendor.AsString();
            }
        }
        public static string GetDeviceIp
        {
            get
            {
                var ipAddress = string.Empty;
                foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                        {
                            if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ipAddress = addrInfo.Address.ToString();
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(ipAddress)) ipAddress = "x.x.x.x";
                return ipAddress;
            }
        }
    }
}