using System;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Widget;

namespace SeekiosApp.Droid.Helper
{
    class GooglePlayServicesHelper
    {
        /// <summary>
        /// Retourne vrai si les services google play sont disponnible sur le device exécutant l'appli
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool IsPlayServicesAvailable(Context context)
        {
            int resultCode = GooglePlayServicesUtil.IsGooglePlayServicesAvailable(context);
            if (resultCode != ConnectionResult.Success)
            {
                if (GooglePlayServicesUtil.IsUserRecoverableError(resultCode))
                    Console.WriteLine(GooglePlayServicesUtil.GetErrorString(resultCode));
                else
                    Console.WriteLine("Sorry, this device is not supported");
                return false;
            }
            else
            {
                Console.WriteLine("Google Play Services is available");
                return true;
            }
        }
    }
}