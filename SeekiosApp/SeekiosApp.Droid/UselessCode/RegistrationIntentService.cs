//using System;
//using Android.App;
//using Android.Content;
//using Android.Util;
//using Android.Gms.Gcm;
//using Android.Gms.Gcm.Iid;
//using System.Threading;
//using SeekiosApp.Droid.Helper;

//namespace SeekiosApp.Droid.GCM
//{
//    [Service(Exported = false)]
//    class RegistrationIntentService : IntentService
//    {
//        static object locker = new object();

//        public RegistrationIntentService() : base("RegistrationIntentService") { GCMRegistrationToken = string.Empty; }

//        protected override void OnHandleIntent(Intent intent)
//        {
//            while (App.InternetConnectionService.IsDeviceConnectedToInternet() && GCMRegistrationTokenStatus != GCMRegistrationTokenStatus.Available)
//            {
//                try
//                {
//                    GCMRegistrationTokenStatus = GCMRegistrationTokenStatus.Asking;
//                    Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
//                    lock (locker)
//                    {
//                        var instanceID = InstanceID.GetInstance(this);
//                        GCMRegistrationToken = instanceID.GetToken("809546198888", GoogleCloudMessaging.InstanceIdScope, null);//1031878310850
//                        GCMRegistrationTokenStatus = GCMRegistrationTokenStatus.Available;
//                        Log.Info("RegistrationIntentService", "GCM Registration Token: " + GCMRegistrationToken);
//                        SendRegistrationToAppServer(GCMRegistrationToken);
//                        Subscribe(GCMRegistrationToken);
//                    }
//                }
//                catch (Exception e)
//                {
//                    Log.Debug("RegistrationIntentService", "Failed to get a registration token");
//                    GCMRegistrationTokenStatus = GCMRegistrationTokenStatus.NotFound;
//                }
//            }
//        }

//        private static string _CMRegistrationToken;

//        public static string GCMRegistrationToken
//        {
//            get
//            {

//                return GCMRegistrationTokenStatus == GCMRegistrationTokenStatus.NotFound ? "noToken" : _CMRegistrationToken;
//            }
//            private set { _CMRegistrationToken = value; }
//        }


//        public static GCMRegistrationTokenStatus GCMRegistrationTokenStatus = GCMRegistrationTokenStatus.None;

//        void SendRegistrationToAppServer(string token)
//        {
//            // Add custom implementation here as needed.
//            //envoyer au serveur l'id utilisateur et le token pour qu'il l'enregistre en Bdd affin de pouvoir notifier le téléphone
//        }

//        void Subscribe(string token)
//        {
//            var pubSub = GcmPubSub.GetInstance(this);
//            pubSub.Subscribe(token, "/topics/global", null);
//        }

//        void Unsubscribe(string token)
//        {
//            var pubSub = GcmPubSub.GetInstance(this);
//            pubSub.Unsubscribe(token, "/topics/global");
//        }
//    }

//    public enum GCMRegistrationTokenStatus
//    {
//        None,
//        Asking,
//        NotFound,
//        Available
//    }
//}