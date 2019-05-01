using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.Threading.Tasks;
using static SeekiosApp.Droid.Helper.OneSignalHelper;
using Com.OneSignal.Android;

namespace SeekiosApp.Droid.Services
{
    public class OneSignalNotificationBackgroundService : IntentService
    {

        #region Properties

        public static bool IsServiceRunning { get; set; }

        #endregion

        public override IBinder OnBind(Intent intent)
        {
            return new OneSignalNotificationBackgroundServiceBinder(this);
        }

        #region LifeCycle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("OneSignalNotificationBackgroundService", "on start");

            IsServiceRunning = true;

            new Task(() => 
            {
                //Connect to OneSignal
                OneSignal.Init(this, "1077123816365", "ee8851b0-f171-4de0-b86b-74ef18eefa02", new NotificationOpenedHandler(), new NotificationReceivedHandler(this));
                OneSignal.SetSubscription(true);
                OneSignal.IdsAvailable(new IdsAvailableHandler());
            });

            return StartCommandResult.Sticky;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootIntent"></param>
        public override void OnTaskRemoved(Intent rootIntent)
        {
            IsServiceRunning = false;
            base.OnTaskRemoved(rootIntent);
        }

        protected override void OnHandleIntent(Intent intent)
        {
            Log.Debug("OneSignalNotificationBackgroundService", "handle intent");
            string dataString = intent.DataString;
            Log.Debug("OneSignalNotificationBackgroundService", dataString);
        }

        #endregion
    }

    public class OneSignalNotificationBackgroundServiceBinder : Binder
    {
        public OneSignalNotificationBackgroundServiceBinder(OneSignalNotificationBackgroundService service)
        {
            _service = service;
        }

        protected OneSignalNotificationBackgroundService _service;
        public OneSignalNotificationBackgroundService Service
        {
            get { return _service; }
        }

        public bool IsBound { get; set; }
    }

    public class OneSignalNotificationBackgroundServiceConnection : Java.Lang.Object, IServiceConnection
    {
        #region Constructor

        /// <summary>
        /// Bind itself with the don't move background service 
        /// </summary>
        public OneSignalNotificationBackgroundServiceConnection()
        {
            Application.Context.BindService(new Intent(Application.Context, typeof(OneSignalNotificationBackgroundService)), this, Bind.AutoCreate);
        }

        #endregion

        #region Attributes

        /// <summary>Binder for communication with the service</summary>
        private OneSignalNotificationBackgroundServiceBinder _binder;

        #endregion

        #region Properties

        /// <summary>
        /// Action to start the service
        /// </summary>
        public static Action StartOneSignalService;

        #endregion

        #region Public methods

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            OneSignalNotificationBackgroundServiceBinder serviceBinder = service as OneSignalNotificationBackgroundServiceBinder;

            if (serviceBinder != null)
            {
                this._binder = serviceBinder;
                this._binder.IsBound = true;
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this._binder.IsBound = false;
        }

        /// <summary>
        /// Start the don't move background service
        /// </summary>
        public void StartService()
        {
            if (StartOneSignalService == null || IsServiceRunning()) return;
            StartOneSignalService();
        }

        /// <summary>
        /// Check if the service is launch
        /// </summary>
        /// <returns></returns>
        public bool IsServiceRunning()
        {
            return OneSignalNotificationBackgroundService.IsServiceRunning;
        }

        #endregion
    }
}