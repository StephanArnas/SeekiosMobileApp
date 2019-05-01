using System;
using Plugin.DeviceInfo;
using System.Net.NetworkInformation;
using Android.Net.Wifi;
using Android.Content;
using Android.App;
using System.Net;
using System.Globalization;
using Android.Telephony;
using Java.Util;

namespace SeekiosApp.Droid.Helper
{
    class DeviceInfoHelper
    {
        public static string Platform { get { return CrossDeviceInfo.Current.Platform.ToString(); } }
        public static string DeviceModel { get { return CrossDeviceInfo.Current.Model; } }
        public static string Version { get { return CrossDeviceInfo.Current.Version; } }
        public static string CountryCode
        {
            get
            {
                return Locale.Default.Language;
            }
        }
        public static string GetDeviceUniqueId(Context context)
        {
            return Platform + Android.Provider.Settings.Secure.GetString(context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);

            //foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
            //{
            //    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
            //        netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            //    {
            //        var address = netInterface.GetPhysicalAddress();
            //        return BitConverter.ToString(address.GetAddressBytes());
            //    }
            //}
        }
        public static string GetDeviceIp(Context context)
        {
            WifiManager wifiManager = (WifiManager)context.GetSystemService(Service.WifiService);
            //IPAddress[] addresses = Dns.GetHostAddresses(Dns.GetHostName());
            var ip = wifiManager.ConnectionInfo.IpAddress;

            var ipString = string.Format(
            "{0}.{1}.{2}.{3}",
            (ip & 0xff),
            (ip >> 8 & 0xff),
            (ip >> 16 & 0xff),
            (ip >> 24 & 0xff));

            return ipString;
        }
    }
}