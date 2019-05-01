using Foundation;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.iOS.ModeZone;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.CustomComponents.CustomPicker;
using SeekiosApp.iOS.Views.TableSources;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class ModeZoneSecondView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private SeekiosDTO _seekiosSelected;

        #endregion

        #region ===== Constructor =================================================================

        public ModeZoneSecondView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            App.Locator.ModeZone.IsTrackingAfterOOZ = true;
            EmptyAlertButton.TouchUpInside += AddAlertButton_TouchUpInside;
            EmptyAlertImageButton.TouchUpInside += AddAlertButton_TouchUpInside;
            AddAlertButton.TouchUpInside += AddAlertButton_TouchUpInside;
            NextButton.TouchUpInside += NextButton_Click;
            PreviousButton.TouchUpInside += PreviousButton_TouchUpInside;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (App.Locator.ModeZone.IsGoingBack)
            {
                Title = "1/3";
                GoBack(false);
                return;
            }
            Title = Application.LocalizedString("ModeZone2Title");

            Tableview.Source = new ModeZoneThirdSource(this);
            UpdateAlertData();
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (App.Locator.ModeZone.IsGoingBack)
            {
                Title = "1/3";
            }
            else
            {
                Title = "2/3";
            }
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            InitialiseAllStrings();

            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;

            AddAlertButton.Layer.MasksToBounds = true;
            AddAlertButton.Layer.CornerRadius = 4;


            PreviousButton.Layer.MasksToBounds = true;
            PreviousButton.Layer.CornerRadius = 4;

            NextButton.Layer.MasksToBounds = true;
            NextButton.Layer.CornerRadius = 4;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void UpdateAlertData()
        {

            if (App.Locator.ModeZone.LsAlertsModeZone?.Count > 0)
            {
                Tableview.ReloadData();
                Tableview.Hidden = false;
            }
            else Tableview.Hidden = true;
            InitialiseAllStrings();
        }

        #endregion

        #region ===== Private Methods =============================================================

        public void InitialiseAllStrings()
        {
            MobileNotificationLabel.Text = Application.LocalizedString("MobileNotification");
            NotifActivatedLabel.Text = Application.LocalizedString("NotifActivatedZone");
            EmailAlertLabel.Text = Application.LocalizedString("EmailAlert");
            AddAlertButton.SetTitle(Application.LocalizedString("CreateButton"), UIControlState.Normal);
            NextButton.SetTitle(Application.LocalizedString("Next"), UIControlState.Normal);
            EmptyAlertButton.SetTitle(Application.LocalizedString(Application.LocalizedString("NoEmailAlertConfigured")), UIControlState.Normal);
            PreviousButton.SetTitle(Application.LocalizedString("Previous"), UIControlState.Normal);
            if (App.Locator.ModeZone.LsAlertsModeZone?.Count > 0)
            {
                NextButton.SetTitle(Application.LocalizedString("Next"), UIControlState.Normal);
            }
            else NextButton.SetTitle(Application.LocalizedString("Skip"), UIControlState.Normal);
        }

        #endregion

        #region ===== Event =======================================================================

        private void AddAlertButton_TouchUpInside(object sender, EventArgs e)
        {
            // button was clicked
            // the mode expect an alert
            App.Locator.ModeZone.WaitingForAlerts = true;
            // the mode is in edit alert
            App.Locator.ModeZone.EditingAlerts = false;
            // navigate to add alert
            App.Locator.ModeZone.GoToAlertDetail(null, -1);
        }

        private void PreviousButton_TouchUpInside(object sender, EventArgs e)
        {
            App.Locator.ModeZone.WaitingForAlerts = false;
            GoBack(true);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            App.Locator.ModeZone.GoToThirdPage();
        }

        #endregion
    }
}