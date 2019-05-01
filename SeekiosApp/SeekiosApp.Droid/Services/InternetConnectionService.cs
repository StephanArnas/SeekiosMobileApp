using Android.Content;
using Android.Net;
using SeekiosApp.Interfaces;
using System;

namespace SeekiosApp.Droid.Services
{
    public class InternetConnectionService : IInternetConnectionService
    {
        private Context _context;
        private ConnectivityManager _connectionManager;

        public void Initialize(object context, string connectivity)
        {
            _context = (Context)context;
            _connectionManager = (ConnectivityManager)_context.GetSystemService(connectivity);
        }

        public bool IsDeviceConnectedToInternet()
        {
            if (_context == null) throw new System.Exception("Service not initialized");
            NetworkInfo activeConnection = _connectionManager.ActiveNetworkInfo;
            return activeConnection != null && activeConnection.IsConnected;
        }

        public bool IsDeviceBeingConnectedToInternet()
        {
            if (_context == null) throw new System.Exception("Service not initialized");
            NetworkInfo activeConnection = _connectionManager.ActiveNetworkInfo;
            return activeConnection != null && activeConnection.IsConnectedOrConnecting;
        }

        //public override void OnReceive(Context context, Intent intent)
        //{
        //    _connectionManager = (ConnectivityManager)_context.GetSystemService(Context.ConnectivityService);
        //    var networkInfo = _connectionManager.ActiveNetworkInfo;
        //    if (networkInfo.IsConnected)
        //        OnInternetStateChanged.Invoke(true);
        //    else OnInternetStateChanged.Invoke(false);
        //}

        //public delegate void OnInternetStateChangedHandler(bool isConnected);

        //public event OnInternetStateChangedHandler OnInternetStateChanged;
    }
}