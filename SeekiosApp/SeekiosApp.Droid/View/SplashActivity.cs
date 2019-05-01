using Android.App;
using Android.OS;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Android.Content;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Droid.Services;
using Android.Widget;
using Android.Views;
using Android.Views.Animations;
using System.Threading;
using SeekiosApp.Interfaces;
using Android.Util;
using SeekiosApp.Services;
using Android.Content.PM;
using Android.Content.Res;

namespace SeekiosApp.Droid.View
{
    [Activity(MainLauncher = true, NoHistory = true, Theme = "@style/Theme.Splash")]
    public class SplashActivity : AppCompatActivityBase
    {
        #region ===== Properties ==================================================================

        public XamSvg.SvgImageView LogoSeekios { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            Timer.Start();
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.SplashScreenLayout);

            HockeyApp.Android.CrashManager.Register(this, "07d00a23e09147d6980fd86f4b695da7", new HockeyCrashManagerListener(this));
            AccessResources.CreateInstance(this);

            InitDependances();
            RegisterAppVersion();
            AppCompatActivityBase.CurrentActivity = this;
            (ServiceLocator.Current.GetInstance<INavigationService>() as AppCompatNavigationService).NavigateTo(App.LOGIN_PAGE);
        }

        #endregion

        #region ===== Private Methods =============================================================

        private void InitDependances()
        {
            // Navigation services
            Log.Debug("SplashActivity", "InitDependances : Initializing AppCompatNavigationService...");
            var nav = new AppCompatNavigationService();
            nav.Configure(App.LOGIN_PAGE, typeof(LoginActivity));
            nav.Configure(App.NEED_UPDATE_PAGE, typeof(NeedUpdateActivity));
            nav.Configure(App.CGU_PAGE, typeof(CGUActivity));
            nav.Configure(App.LIST_SEEKIOS_PAGE, typeof(ListSeekiosActivity));
            nav.Configure(App.DETAIL_SEEKIOS_PAGE, typeof(DetailSeekiosActivity));
            nav.Configure(App.MAP_PAGE, typeof(MapActivity));
            nav.Configure(App.MAP_HISTORIC_PAGE, typeof(MapHistoricActivity));
            nav.Configure(App.MODE_ZONE_PAGE, typeof(ModeZoneActivity));
            nav.Configure(App.MODE_ZONE_2_PAGE, typeof(ModeZone3Activity));
            nav.Configure(App.MODE_ZONE_3_PAGE, typeof(ModeZone2Activity));
            nav.Configure(App.MODE_DONT_MOVE_PAGE, typeof(ModeDontMove2Activity));
            nav.Configure(App.MODE_DONT_MOVE_2_PAGE, typeof(ModeDontMoveActivity));
            nav.Configure(App.MODE_TRACKING_PAGE, typeof(ModeTrackingActivity));
            nav.Configure(App.ALERT_PAGE, typeof(AlertActivity));
            nav.Configure(App.ALERT_SOS_PAGE, typeof(AlertSOSActivity));
            nav.Configure(App.PARAMETER_PAGE, typeof(ParameterActivity));
            nav.Configure(App.ABOUT_PAGE, typeof(AboutActivity));
            nav.Configure(App.ADD_SEEKIOS_PAGE, typeof(AddSeekiosActivity));
            //nav.Configure(App.MAP_ALL_SEEKIOS_PAGE, typeof(MapAllSeekiosActivity));
            nav.Configure(App.RELOAD_CREDIT_PAGE, typeof(ReloadCreditActivity));
            nav.Configure(App.TRANSACTION_HISTORIC_PAGE, typeof(TransactionHistoricActivity));
            nav.Configure(App.LIST_TUTORIAL_PAGE, typeof(ListTutorialActivity));
            nav.Configure(App.TUTORIAL_PAGE, typeof(TutorialFirstLaunchActivity));
            nav.Configure(App.TUTORIAL_BACKGROUND_FIRST_LAUNCH_PAGE, typeof(TutorialBackgroundActivity));
            nav.Configure(App.TUTORIAL_CREDITCOST_PAGE, typeof(TutorialCreditCostActivity));
            nav.Configure(App.TUTORIAL_POWERSAVING_PAGE, typeof(TutorialPowerSavingActivity));

            // Registration of services
            Log.Debug("SplashActivity", "InitDependances : Initializing SimpleIoc...");
            SimpleIoc.Default.Reset();
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<INavigationService>(() => nav, true);
            SimpleIoc.Default.Register<Interfaces.IDialogService, AppCompatDialogService>(true);
            SimpleIoc.Default.Register<ISaveDataService, SaveDataService>();
            SimpleIoc.Default.Register<IDispatchOnUIThread, DispatchService>(true);
            SimpleIoc.Default.Register<IInternetConnectionService, InternetConnectionService>(true);
            SimpleIoc.Default.Register<ITimer, TimerWrapper>();
            SimpleIoc.Default.Register<ILocalNotificationService, LocalNotificationService>();
            ViewModel.ViewModelLocator.Initialize();

            (ServiceLocator.Current.GetInstance<IDispatchOnUIThread>() as DispatchService).SetContext(this);
            (ServiceLocator.Current.GetInstance<IInternetConnectionService>() as InternetConnectionService).Initialize(this, ConnectivityService);

            // Instantiation of the data service
            var SaveDataService = ServiceLocator.Current.GetInstance<ISaveDataService>();
            SaveDataService.Init(ApplicationContext);

            // Initialization of the internet connection service
            var InternetConnectionService = ServiceLocator.Current.GetInstance<IInternetConnectionService>();
            InternetConnectionService.Initialize(ApplicationContext, ConnectivityService);

            Log.Debug("SplashActivity", "InitDependances : Starting CloseGestureService...");
            StartService(new Intent(this, typeof(CloseGestureService)));

            // Check if notification are enabled
            if (!Android.Support.V4.App.NotificationManagerCompat.From(this).AreNotificationsEnabled())
            {
                App.Locator.ListSeekios.IsNotificationAvailable = false;
            }
        }

        private void RegisterAppVersion()
        {
            var dataService = (ServiceLocator.Current.GetInstance<IDataService>() as DataService);
            var packageInfo = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.MetaData); //sans PackageInfoFlags.MetaData => semble causer java.lang.RuntimeException: android.os.DeadObjectException
            App.Locator.Login.VersionApplication = packageInfo.VersionName;
        }
        
        #endregion
    }
}