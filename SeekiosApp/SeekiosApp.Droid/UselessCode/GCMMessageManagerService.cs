//using System;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Util;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Model.DTO;
//using Android.Locations;

//namespace SeekiosApp.Droid.Services
//{
//    [Service]
//    class GCMMessageManagerService : Service
//    {
//        public static string RECEIVED_MESSAGE;

//        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
//        {
//            Log.Debug("GCMMessageManagerService", "GCMMessageManagerService started");

//            DoWork();

//            return StartCommandResult.Sticky;
//        }

//        public void DoWork()
//        {
//            //Si on demande la localisation pour vérifier la proximité
//            if (RECEIVED_MESSAGE.StartsWith("[LocationPlease]"))
//            {
//                //On essaie de la récupérer
//                var loc = new LocationHelper(this);
//                loc.SendLocationRequest(new Action<Location>(location =>
//                {
//                    if (location != null) App.Locator.GCM.UpdateUserLocation(location.Latitude, location.Longitude);
//                    Log.Debug("MyGcmListenerService", "[LocationPlease]");
//                    App.Locator.GCM.ConfirmChangeUserLocationRequestTreated();
//                    Log.Debug("MyGcmListenerService", "[LocationPlease] OK OK OK");
//                    StopSelf();
//                }));
//                return;
//            }
//            //Si la demande de localisation n'a pas aboutie on nous demande l'appairage bluetooth
//            if (RECEIVED_MESSAGE.StartsWith("[PairingPlease]"))
//            {
//                //init();
//                //var seekiosMacAddress = RECEIVED_MESSAGE.Substring(14);
//                //_seekiosMoving = App.CurrentUserEnvironment.LsSeekios.Where(el => el.MacAddress == seekiosMacAddress).FirstOrDefault();
//                //if (_seekiosMoving == null) return;
//                ////On lance l'appairage
//                //_bluetoothManager.BeginScanningForDevices();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[SeekiosMoved]"))
//            {
//                var parameters = RECEIVED_MESSAGE.Substring(14);
//                var splittedParameters = parameters.Split('|');
//                var seekiosName = splittedParameters[0];
//                var seekiosPicture = splittedParameters[1];

//                var modeDontMoveTitle = Resources.GetString(Resource.String.modeDefinition_dontmove);
//                var modeDontMoveAlert = Resources.GetString(Resource.String.gcmMessage_modeDontMoveAlert);

//                NotificationHelper.SendNotification(this, uidSeekios, modeDontMoveTitle, string.Format(modeDontMoveAlert, seekiosName));
//                StopSelf();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[SeekiosOutOfZone]"))
//            {
//                var parameters = RECEIVED_MESSAGE.Substring(18);
//                var splittedParameters = parameters.Split('|');
//                var seekiosName = splittedParameters[0];
//                var seekiosPicture = splittedParameters[1];

//                var modeZoneTitle = Resources.GetString(Resource.String.modeDefinition_zone);
//                var modeZoneOutAlert = Resources.GetString(Resource.String.gcmMessage_modeZoneOutAlert);

//                NotificationHelper.SendNotification(this, modeZoneTitle, string.Format(modeZoneOutAlert, seekiosName));
//                StopSelf();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[FriendShipRequested]"))
//            {
//                var parameters = RECEIVED_MESSAGE.Substring(21);
//                var splittedParameters = parameters.Split('|');
//                var firstName = splittedParameters[0];
//                var lastName = splittedParameters[1];
//                var userPicture = splittedParameters[2];

//                var friendshipRequestTitle = Resources.GetString(Resource.String.gcmMessage_friendshipRequestTitle);
//                var friendshipRequestContent = Resources.GetString(Resource.String.gcmMessage_friendshipRequestContent);

//                NotificationHelper.SendNotification(this, friendshipRequestTitle, string.Format(friendshipRequestContent, firstName, lastName));
//                StopSelf();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[SOS]"))
//            {
//                var seekiosName = RECEIVED_MESSAGE.Substring(5);

//                var SOSTitle = Resources.GetString(Resource.String.gcmMessage_SOSTitle);
//                var SOSContent = Resources.GetString(Resource.String.gcmMessage_SOSContent);

//                NotificationHelper.SendNotification(this, SOSTitle, string.Format(SOSContent, seekiosName));
//                StopSelf();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[SOSLocation]"))
//            {
//                var seekiosName = RECEIVED_MESSAGE.Substring(13);

//                var SOSTitle = Resources.GetString(Resource.String.gcmMessage_SOSLocationTitle);
//                var SOSContent = Resources.GetString(Resource.String.gcmMessage_SOSLocationContent);

//                NotificationHelper.SendNotification(this, SOSTitle, string.Format(SOSContent, seekiosName));
//                StopSelf();
//                return;
//            }
//            if (RECEIVED_MESSAGE.StartsWith("[BLEConnexionLoss]"))
//            {
//                var seekiosName = RECEIVED_MESSAGE.Substring(18);

//                var connexionLostTitle = Resources.GetString(Resource.String.gcmMessage_connexionLostTitle);
//                var connexionLostContent = Resources.GetString(Resource.String.gcmMessage_connexionLostContent);

//                NotificationHelper.SendNotification(this, string.Format(connexionLostTitle, seekiosName), string.Format(connexionLostContent, seekiosName));
//                StopSelf();
//                return;
//            }
//            NotificationHelper.SendNotification(this, "Seekios", RECEIVED_MESSAGE);
//            StopSelf();
//        }

//        #region Gestion de l'appairage (vérification de proximité par bluetooth)

//        //Initialise le BluetoothManager
//        private void init()
//        {
//            _bluetoothManager = new BluetoothService();
//            _bluetoothManager.DeviceDiscovered += BluetoothManager_DeviceDiscovered;
//            _bluetoothManager.ScanTimeoutElapsed += BluetoothManager_ScanTimeoutElapsed;
//        }

//        private BluetoothService _bluetoothManager;
//        private SeekiosDTO _seekiosMoving;

//        /// <summary>
//        /// Si on n'a trouvé aucun périphérique correspondant au seekios alors on alerte l'utilisateur : le seekios n'est pas à proximité
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void BluetoothManager_ScanTimeoutElapsed(object sender, EventArgs e)
//        {
//            NotificationHelper.SendNotification(this, "Mode Don't Move", "Le seekios " + _seekiosMoving.SeekiosName + " a bougé !");
//            StopSelf();
//        }

//        /// <summary>
//        /// Si on trouve un périphérique
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void BluetoothManager_DeviceDiscovered(object sender, Interfaces.DeviceDiscoveredEventArgs e)
//        {
//            //Si ce n'est pas le bon on s'en fout
//            //if (e.Device.Address != _seekiosMoving.MacAddress) return;
//            ////Sinon on arrête le scan : tout va bien, l'utilisateur est à côté de son seekios
//            //_bluetoothManager.StopScanningForDevices();
//            //StopSelf();
//        }

//        public override IBinder OnBind(Intent intent)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion
//    }
//}