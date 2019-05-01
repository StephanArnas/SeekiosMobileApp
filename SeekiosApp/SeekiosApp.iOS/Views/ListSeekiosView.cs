using SeekiosApp.iOS.Classes;
using System;
using UIKit;
using SeekiosApp.iOS.Views;
using Xamarin.SWRevealViewController;

using SeekiosApp.Model.APP.OneSignal;
using GalaSoft.MvvmLight.Ioc;
using SeekiosApp.Interfaces;
using SeekiosApp.iOS.Services;
using System.Collections.Generic;
using SeekiosApp.Services;
using System.Linq;

namespace SeekiosApp.iOS
{
    public partial class ListSeekiosView : UIViewController
    {
        #region ===== Attributs ===================================================================

        //private bool _isOneSignalRemovedTag = false;

        #endregion

        #region ===== Properties ==================================================================

        public UIBarButtonItem SliderViewButton { get { return SliderBarButton; } }

        #endregion

        #region ====== Constructor ================================================================

        public ListSeekiosView(IntPtr handle) : base(handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(98, 218, 115);
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
            NavigationController.NavigationBarHidden = false;
            App.Locator.Login.SaveFirstLaunchTuto();

            Title = Application.LocalizedString("ListSeekiosTitle");
            if (DataService.UseStaging) Title += " (stag)";
            SetOneSignal();

            AddSeekiosBarButton.Clicked += AddSeekiosBarButton_Clicked;
            DefaultListSeekiosBackgroundButton.TouchUpInside += AddSeekiosBarButton_Clicked;
            DefaultListSeekiosText.TouchUpInside += AddSeekiosBarButton_Clicked;
            App.SeekiosChanged += App_SeekiosChanged;
            App.Locator.ModeDontMove.SeekiosMovedNotified += ModeDontMove_SeekiosMovedNotified;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += ModeZone_OnSeekiosOutOfZoneNotified;
            Seekios.iOS.Services.DialogService.CurrentViewController = this;

            // Display popup if the new reload credit is available
            App.Locator.ListSeekios.PopupRelaodCreditMonthly();

            // Display a popup if the notification push are not registered
            App.Locator.ListSeekios.PopupNotificationNotAvailable(() =>
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(App.TutorialNotificationLink));
            });

            // Register to SignalR
            App.Locator.ListSeekios.SubscribeToSignalR();

            // Get the uidDevice (required by webservice for identify the sender of the broadcast)
            App.UidDevice = Helper.DeviceInfoHelper.GetDeviceUniqueId;
        }

        public override void ViewWillAppear(bool animated)
        {
            NavigationService.LeftMenuView.RevealViewController().PanGestureRecognizer.Enabled = true;
            NavigationController.InteractivePopGestureRecognizer.Enabled = true;

            NavigationController.NavigationBarHidden = false;
            base.ViewWillAppear(animated);
            App.Locator.AddSeekios.IsGoingBack = false;
            SetDataAndStyleToView();
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public void SetDataAndStyleToView()
        {
            DefaultListSeekiosText.SetTitle(Application.LocalizedString("NoSeekios"), UIControlState.Normal);
            App.Locator.BaseMap.InitialiseLsAlertState();

            if (App.CurrentUserEnvironment.LsSeekios.Count > 0)
            {
                Tableview.Hidden = false;
                Tableview.DataSource = new ListSeekiosSource(this);
                Tableview.Delegate = new ListSeekiosTableDelegate(this);
                Tableview.ReloadData();
            }
            else
            {
                Tableview.Hidden = true;
            }
        }

        #endregion

        #region ====== Private Methodes ===========================================================

        private void App_SeekiosChanged(object sender, int e)
        {
            SetDataAndStyleToView();
        }

        private void SetOneSignal()
        {
            var oneSignalHandler = new OneSignalHandler();
            var oneSignal = new OneSignalIOS();
            oneSignal.StartInit("4dbbcd4b-8108-4711-a923-92ac93cb48b4")
                //.HandleNotificationReceived(oneSignalHandler.NotificationReceivedDelegate())
                //.HandleNotificationOpened(oneSignalHandler.NotificationOpenedDelegate())
                .InFocusDisplaying(OSInFocusDisplayOptionEnum.Notification)
                .Settings(new Dictionary<string, bool> { { OneSignalIOS.kOSSettingsKeyAutoPrompt, false }, { OneSignalIOS.kOSSettingsKeyInAppLaunchURL, false } })
                .EndInit();

            oneSignal.IdsAvailable((playerID, pushToken) =>
            {
                oneSignal.GetTags(async (result) =>
                {
                    try
                    {
                        //if (result != null && !_isOneSignalRemovedTag)
                        //{
                        //    oneSignal.DeleteTags(result.Keys.ToList());
                        //    _isOneSignalRemovedTag = true;
                        //}
                        await System.Threading.Tasks.Task.Delay(3900);
                        var key = App.CurrentUserEnvironment.User.IdUser.ToString() + (DataService.UseStaging ? "s" : "p");
                        var value = string.Format("{0}/{1}/{2}"
                            , App.CurrentUserEnvironment.User.FirstName
                            , App.CurrentUserEnvironment.User.LastName
                            , App.CurrentUserEnvironment.User.Email);
                        // TODO : register the PlayerId in DB
                        oneSignal.SendTag(key, value);
                    }
                    catch (Exception)
                    {
                        // TODO : handle popup for this exception ? 
                    }
                });
            });

            oneSignal.SetSubscription(true);
            oneSignal.RegisterForPushNotifications();
            oneSignal.SyncHashedEmail(App.CurrentUserEnvironment.User.Email);

            if (SimpleIoc.Default.IsRegistered<IOneSignal>())
                SimpleIoc.Default.Unregister<IOneSignal>();
            SimpleIoc.Default.Register<IOneSignal>(() => oneSignal);
        }

        #endregion

        #region ====== Event ======================================================================

        private void AddSeekiosBarButton_Clicked(object sender, EventArgs e)
        {
            App.Locator.ListSeekios.GoToAddSeekios();
        }

        private void ModeZone_OnSeekiosOutOfZoneNotified(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication)
        {
            SetDataAndStyleToView();
        }

        private void ModeDontMove_SeekiosMovedNotified(int idSeekios)
        {
            SetDataAndStyleToView();
        }

        #endregion
    }
}