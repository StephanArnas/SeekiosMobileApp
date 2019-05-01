using Foundation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using UIKit;
using SeekiosApp.Interfaces;
using SeekiosApp.iOS.Services;
using GalaSoft.MvvmLight.Threading;
//using Facebook.CoreKit;
using SeekiosApp.Model.APP.OneSignal;
using System.Collections.Generic;
using System;
using HockeyApp.iOS;
using SeekiosApp.iOS.Views.Delegates;
using Xamarin.InAppPurchase;
using SeekiosApp.Helper;
using SeekiosApp.Services;
using SeekiosApp.iOS.Helper;
using Microsoft.Practices.ServiceLocation;

namespace SeekiosApp.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        #region ===== Properties ==================================================================

        public override UIWindow Window { get; set; }
        public static InAppPurchaseManager PurchaseManager { get; set; }
        
        #endregion

        #region ===== Attributs ===================================================================

        private OneSignalHandler _oneSignalHandler = new OneSignalHandler();

        #endregion

        #region ===== Override Methods ============================================================

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Registration of the navigation
            var navigation = new SeekiosApp.Services.NavigationService();
            navigation.Initialize((UINavigationController)Window.RootViewController);
            navigation.Configure(App.LOGIN_PAGE, "LoginView");
            navigation.Configure(App.NEED_UPDATE_PAGE, "NeedUpdateView");
            navigation.Configure(App.LIST_SEEKIOS_PAGE, "ListSeekiosView");
            navigation.Configure(App.DETAIL_SEEKIOS_PAGE, "DetailSeekiosView");
            navigation.Configure(App.MAP_PAGE, "MapView");
            navigation.Configure(App.MAP_HISTORIC_PAGE, "MapHistoricView");
            navigation.Configure(App.MODE_ZONE_PAGE, "ModeZoneFirstView");
            navigation.Configure(App.MODE_ZONE_2_PAGE, "ModeZoneThirdView");
            navigation.Configure(App.MODE_ZONE_3_PAGE, "ModeZoneSecondView");
            navigation.Configure(App.MODE_DONT_MOVE_PAGE, "ModeDontMoveFirstView");
            navigation.Configure(App.MODE_DONT_MOVE_2_PAGE, "ModeDontMoveSecondView");
            navigation.Configure(App.MODE_TRACKING_PAGE, "ModeTrackingView");
            navigation.Configure(App.ALERT_PAGE, "AlertView");
            navigation.Configure(App.ALERT_SOS_PAGE, "AlertSOSView");
            navigation.Configure(App.PARAMETER_PAGE, "ParameterUserView");
            navigation.Configure(App.ABOUT_PAGE, "AboutView");
            navigation.Configure(App.ADD_SEEKIOS_PAGE, "AddSeekiosView");
            navigation.Configure(App.MAP_ALL_SEEKIOS_PAGE, "MapAllSeekiosView");
            navigation.Configure(App.RELOAD_CREDIT_PAGE, "RechargeCreditsView");
            navigation.Configure(App.TRANSACTION_HISTORIC_PAGE, "TransactionHistoricView");
            navigation.Configure(App.LIST_TUTORIAL_PAGE, "ListTutorialView");
            navigation.Configure(App.TUTORIAL_BACKGROUND_FIRST_LAUNCH_PAGE, "TutorialBackgroundView"); 
            navigation.Configure(App.TUTORIAL_POWERSAVING_PAGE, "TutorialPowerSavingView"); 
            navigation.Configure(App.TUTORIAL_CREDITCOST_PAGE, "TutorialCreditCostView"); 
            navigation.Configure(App.TUTORIAL_SEEKIOS_LED_PAGE, "TutorialSeekiosLedView");

            // Registration of the others services
            SimpleIoc.Default.Reset();
            //ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider());
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<INavigationService>(() => navigation);
            SimpleIoc.Default.Register<ISaveDataService, SaveDataService>();
            SimpleIoc.Default.Register<Interfaces.IDialogService, Seekios.iOS.Services.DialogService>();
            SimpleIoc.Default.Register<IDispatchOnUIThread, DispatchService>();
            SimpleIoc.Default.Register<IMapControlManager, ControlManager.MapControlManager>();
            SimpleIoc.Default.Register<ITimer, Services.Timer>();
            SimpleIoc.Default.Register<ILocalNotificationService, LocalNotificationService>();
            ViewModel.ViewModelLocator.Initialize();

            // MVVM Light's DispatcherHelper for cross-thread handling.
            DispatcherHelper.Initialize(application);

            // Setup the application version 
            App.Locator.Login.VersionApplication = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();

            // Initialize HockeyApp
            var hockeyApp = BITHockeyManager.SharedHockeyManager;
            hockeyApp.Configure("ba744679c8894526b78271606dbae95e", new HockeyAppCrashDelegate());
            hockeyApp.DisableMetricsManager = true;
            hockeyApp.CrashManager.CrashManagerStatus = BITCrashManagerStatus.AutoSend;
            hockeyApp.StartManager();

            // Set up the style of all the navigation bars in your application
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(98, 218, 115);
            UINavigationBar.Appearance.TintColor = UIColor.White;
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);

            // Subscribe to notification
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());
                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound);
            }

            // Set up in app purchases
            InitializeInAppPurchases();
            var controller = new PackView();
            controller.AttachToPurchaseManager(null, PurchaseManager);

            return true;
        }

        private async void InitializeInAppPurchases()
        {
            PurchaseManager = new InAppPurchaseManager();
            // assembly public key
            string value = Xamarin.InAppPurchase.Utilities.Security.Unify(
                new string[] { "1322f985c2","a34166b24","ab2b367","851cc6" },
                new int[] { 0, 1, 2, 3 });
            PurchaseManager.PublicKey = value;
            PurchaseManager.ApplicationUserName = "Seekios";

            // be sure the user can make payment
            if (!PurchaseManager.CanMakePayments)
            {
                // the user is not able to make payment
                await AlertControllerHelper.ShowAlert(Application.LocalizedString("PaymentFailure")
                            , Application.LocalizedString("CantPurchase")
                            , Application.LocalizedString("Close"));
            }

            // be sure the user has access to internet
            PurchaseManager.NoInternetConnectionAvailable += async () =>
            {
                // the user is has not internet
                await AlertControllerHelper.ShowAlert(Application.LocalizedString("PaymentFailure")
                            , Application.LocalizedString("NoInternet")
                            , Application.LocalizedString("Close"));
            };

            // display any invalid product IDs 
            PurchaseManager.ReceivedInvalidProducts += (productIDs) =>
            {
                Console.WriteLine("The following IDs were rejected by the iTunes App Store:");
                foreach (string ID in productIDs)
                {
                    Console.WriteLine(ID);
                }
            };

            // setup automatic purchase persistance and load any previous purchases
            PurchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.LocalFile;
            PurchaseManager.PersistenceFilename = "AtomicData";
            PurchaseManager.ShuffleProductsOnPersistence = false;
            PurchaseManager.RestoreProducts();
            PurchaseManager.QueryInventory(new string[]
            {
                "com.thingsoftomorrow.seekios.observation",
                "com.thingsoftomorrow.seekios.discovery",
                "com.thingsoftomorrow.seekios.exploration",
                "com.thingsoftomorrow.seekios.aventure",
                "com.thingsoftomorrow.seekios.epopee"
            });
        }

        public override void DidReceiveRemoteNotification(UIApplication application
            , NSDictionary userInfo
            , Action<UIBackgroundFetchResult> completionHandler)
        {
            if (App.CurrentUserEnvironment == null
                || App.CurrentUserEnvironment.User == null
                || string.IsNullOrEmpty(App.CurrentUserEnvironment.User.Email)) return;

            if (_oneSignalHandler == null)
            {
                _oneSignalHandler = new OneSignalHandler();
            }

            if (userInfo != null && userInfo.Count > 0)
            {
                if (userInfo.ContainsKey(new NSString("custom")))
                {
                    var result = SeekiosApp.Helper.StringHelper.RemoveWhitespace(userInfo["custom"].ToString());
                    var dic = ParserHelper.ConvertToDictionaryParser2(result);

                    var notification = new OSNotificationApp();
                    notification.Payload = new OSNotificationPayloadApp();
                    notification.Payload.AdditionalData = new Dictionary<string, object>();
                    notification.Payload.AdditionalData = dic;

                    var handlerReceive = _oneSignalHandler.NotificationReceivedDelegate();
                    handlerReceive.Invoke(notification);
                }
            }
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            App.Locator.ListSeekios.IsNotificationAvailable = false;
        }

        [Export("oneSignalDidFailRegisterForRemoteNotification:error:")]
        public void OneSignalDidFailRegisterForRemoteNotification(UIApplication app, NSError error)
        {
            Console.WriteLine("oneSignalDidFailRegisterForRemoteNotification:error:");
        }

        [Export("oneSignalApplicationWillResignActive:")]
        public void OneSignalApplicationWillResignActive(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationWillResignActive:");
        }

        [Export("oneSignalApplicationDidBecomeActive:")]
        public void OneSignalApplicationDidBecomeActive(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationDidBecomeActive:");
        }

        [Export("oneSignalApplicationDidEnterBackground:")]
        public void OneSignalApplicationDidEnterBackground(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationDidEnterBackground:");
        }

        [Export("oneSignalApplicationWillTerminate:")]
        public void OneSignalApplicationWillTerminate(UIApplication application)
        {
            Console.WriteLine("oneSignalApplicationWillTerminate:");
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // We need to handle URLs by passing them to their own OpenUrl in order to make the SSO authentication works.
            //return ApplicationDelegate.SharedInstance.OpenUrl(application, url, sourceApplication, annotation);
            return false;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public async override void WillTerminate(UIApplication application)
        {
            try
            {
                var oneSignal = (ServiceLocator.Current.GetInstance<IOneSignal>());
                oneSignal.DeleteTag(App.CurrentUserEnvironment.User.IdUser.ToString() + (DataService.UseStaging ? "s" : "p"));
                oneSignal.SetSubscription(false);
                if (App.CurrentUserEnvironment.Device != null)
                {
                    await App.Locator.Login.Disconnect(App.CurrentUserEnvironment.Device.UidDevice);
                }
            }
            catch (Exception) { /* Stephan : I think we don't need any error message */ }
        }

        #endregion
    }
}


