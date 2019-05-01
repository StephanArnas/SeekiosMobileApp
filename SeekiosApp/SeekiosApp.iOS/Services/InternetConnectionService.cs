using System;
using System.Net;
using SystemConfiguration;
using CoreFoundation;
using SeekiosApp.Interfaces;

namespace SeekiosApp.iOS.Services
{
	public enum NetworkStatus
	{
		NotReachable,
		ReachableViaCarrierDataNetwork,
		ReachableViaWiFiNetwork
	}

    public class InternetConnectionService : IInternetConnectionService
    {
		private static string HostName = "www.google.com";


        public void Initialize(object context, string connectivity)
        {
            // not required
        }

        private bool IsReachableWithoutRequiringConnection(NetworkReachabilityFlags flags)
		{
			// is it reachable with the current network configuration?
			bool isReachable = (flags & NetworkReachabilityFlags.Reachable) != 0;

			// do we need a connection to reach it?
			bool noConnectionRequired = (flags & NetworkReachabilityFlags.ConnectionRequired) == 0
				|| (flags & NetworkReachabilityFlags.IsWWAN) != 0;

			return isReachable && noConnectionRequired;
		}

		// is the host reachable with the current network configuration
		private bool IsHostReachable(string host)
		{
			if (string.IsNullOrEmpty(host))
            {
                return false;
            }

            using (var r = new NetworkReachability(host))
			{
				NetworkReachabilityFlags flags;

				if (r.TryGetFlags(out flags))
                {
                    return IsReachableWithoutRequiringConnection(flags);
                }
            }
			return false;
		}

        public bool IsDeviceConnectedToInternet()
        {
			return IsHostReachable(HostName);
        }

        public bool IsDeviceBeingConnectedToInternet()
        {
			return IsHostReachable(HostName);
        }
    }
}