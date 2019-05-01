using System;
using UIKit;
using SeekiosApp.iOS.Views.TableSources;
using SeekiosApp.iOS.Views;

namespace SeekiosApp.iOS
{
    public partial class ModeDontMoveSecondView : BaseViewController
    {
        #region ===== Constructor =================================================================

        public ModeDontMoveSecondView (IntPtr handle) : base (handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            EmptyAlertButton.TouchUpInside += ButtonCreateAlert_TouchUpInside;
            EmptyAlertImageButton.TouchUpInside += ButtonCreateAlert_TouchUpInside;
            ButtonCreateAlert.TouchUpInside += ButtonCreateAlert_TouchUpInside;
            NextButton.TouchUpInside += NextButton_TouchUpInside;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (App.Locator.ModeDontMove.IsGoingBack)
            {
                GoBack(false);
            }
            base.ViewWillAppear(animated);
            TableViewDontMove.Source = new ModeDontMoveSource(this);
            UpdateAlertData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            if (!App.Locator.ModeDontMove.IsGoingBack) Title = "1/2";
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            Title = Application.LocalizedString("DMTitle2");

            InitialiseAllStrings();

            App.Locator.ModeZone.WaitingForAlerts = false;

            NextButton.Layer.CornerRadius = 4;
            NextButton.Layer.MasksToBounds = true;
            ButtonCreateAlert.Layer.CornerRadius = 4;
            ButtonCreateAlert.Layer.MasksToBounds = true;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void UpdateAlertData(){

			if (App.Locator.ModeDontMove.LsAlertsModeDontMove?.Count > 0)
			{
				TableViewDontMove.ReloadData();
                TableViewDontMove.Hidden = false;
			}
			else TableViewDontMove.Hidden = true;
            InitialiseAllStrings();
        }

        #endregion

        #region ===== Private Methods =============================================================

        private void InitialiseAllStrings()
        {
            MobileNotificationLabel.Text = Application.LocalizedString("MobileNotification");
            NotifActivatedLabel.Text = Application.LocalizedString("NotifActivatedDontMove");
            EmailAlertLabel.Text = Application.LocalizedString("EmailAlert");
            ButtonCreateAlert.SetTitle(Application.LocalizedString("CreateButton"), UIControlState.Normal);
            EmptyAlertButton.SetTitle(Application.LocalizedString("NoEmailAlertConfigured"), UIControlState.Normal);
            if (App.Locator.ModeDontMove.LsAlertsModeDontMove?.Count > 0)
            {
                NextButton.SetTitle(Application.LocalizedString("Next"), UIControlState.Normal);
            }
            else NextButton.SetTitle(Application.LocalizedString("Skip"), UIControlState.Normal);
        }

        #endregion

        #region ===== Event =======================================================================

        private  void ButtonCreateAlert_TouchUpInside(object sender, EventArgs e)
		{
			// the mode expect an alert
			App.Locator.ModeZone.WaitingForAlerts = true;
			// the mode is in edit alert
			App.Locator.ModeZone.EditingAlerts = false;
			// navigate to add alert
			App.Locator.ModeZone.GoToAlertDetail(null, -1);
		}

        private void NextButton_TouchUpInside(object sender, EventArgs e)
        {
            App.Locator.ModeDontMove.GoToSecondPage();
        }

        #endregion
    }
}