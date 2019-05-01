using SeekiosApp.Services;
using System;
using System.Drawing;
using Xamarin.SWRevealViewController;
using UIKit;
using Plugin.Connectivity.Abstractions;
using SeekiosApp.iOS.Helper;
using Foundation;

namespace SeekiosApp.iOS.Views
{
    public abstract class BaseViewController : UIViewController
    {
        #region ===== Attributs ===================================================================

        private ConnectivityView customView = null;
        private NSObject _didBecomeActiveNotificationObserver;
        public Action BecomeActiveAction;

        #endregion

        #region ====== Constructor ================================================================

        public BaseViewController(IntPtr handle) : base(handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(98, 218, 115);
            NavigationController.NavigationBar.BarStyle = UIBarStyle.Default;
            NavigationController.NavigationBar.TitleTextAttributes = new UIStringAttributes() { ForegroundColor = UIColor.White };
            if (NavigationService.LeftMenuView != null) NavigationService.LeftMenuView.RevealViewController().PanGestureRecognizer.Enabled = false;
            NavigationController.InteractivePopGestureRecognizer.Enabled = false;
            NavigationController.NavigationBarHidden = false;
            App.ConnectivityChanged += OnConnectivityChanged;
            _didBecomeActiveNotificationObserver = NSNotificationCenter.DefaultCenter.AddObserver(UIApplication.DidBecomeActiveNotification, BecomeActiveNotifcationCallback);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //if (UIApplication.SharedApplication.BackgroundRefreshStatus == UIBackgroundRefreshStatus.Denied || UIApplication.SharedApplication.BackgroundRefreshStatus == UIBackgroundRefreshStatus.Restricted)
            //{
            //    var popup = AlertControllerHelper.CreateBackgroundNotificationAreNotAvailable();
            //    PresentViewController(popup, true, null);
            //}

            Seekios.iOS.Services.DialogService.CurrentViewController = this;
            if (customView != null) customView.DataConnectivity();
            SetDataAndStyleToView();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (customView == null)
            {
                customView = new ConnectivityView();
                customView.Frame = new RectangleF(0f, 24f, (float)this.View.Frame.Size.Width, 0f);
                customView.Hidden = true;
                View.AddSubview(customView);
                customView.DataConnectivity();
            }

            if (_didBecomeActiveNotificationObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_didBecomeActiveNotificationObserver);
            }
        }

        #endregion

        #region ====== Public Methods =============================================================

        public void GoBack(bool animated)
        {
            NavigationController.PopViewController(animated);
        }

        #endregion

        #region ====== Private Methods ============================================================

        private void OnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                App.Locator.ListSeekios.SubscribeToSignalR();
                App.Locator.ListSeekios.LoadUserEnvironment(DeviceInfoHelper.DeviceModel
                    , DeviceInfoHelper.Platform
                    , DeviceInfoHelper.Version
                    , DeviceInfoHelper.GetDeviceUniqueId
                    , DeviceInfoHelper.CountryCode);
            }
            customView.DataConnectivity();
        }

        private void BecomeActiveNotifcationCallback(NSNotification obj)
        {
            BecomeActiveAction?.Invoke();
            App.Locator.ListSeekios.SubscribeToSignalR();
            App.Locator.ListSeekios.LoadUserEnvironment(DeviceInfoHelper.DeviceModel
                    , DeviceInfoHelper.Platform
                    , DeviceInfoHelper.Version
                    , DeviceInfoHelper.GetDeviceUniqueId
                    , DeviceInfoHelper.CountryCode);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public abstract void SetDataAndStyleToView();

        #endregion
    }
}
