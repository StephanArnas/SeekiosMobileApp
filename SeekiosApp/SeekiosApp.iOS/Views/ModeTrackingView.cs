using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.ModeZone;
using System.Collections.Generic;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.CustomComponents.CustomPicker;
using SeekiosApp.iOS.Helper;
using SeekiosApp.ViewModel;
using System.Linq;
using SeekiosApp.Enum;
using CoreGraphics;

namespace SeekiosApp.iOS
{
    public partial class ModeTrackingView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private RefreshPositionPickerView _picker = null;
        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ===== Constructor =================================================================

        public ModeTrackingView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetPickerToView();
            ActivateButton.TouchUpInside += ActivateButton_TouchUpInside;
            TitlePowerSavingButton.TouchDown += TitlePowerSavingButton_TouchDown;
            // set default value 
            _picker.InitDefaultValue(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeTracking.TrackingSetting.RefreshTime));
            PowerSavingSwitch.On = App.Locator.ModeTracking.TrackingSetting.IsPowerSavingEnabled;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                var lastElement = ScrollView.Subviews[10]; // keep the order of the elements in the view
                size = lastElement.Frame.Y + lastElement.Frame.Height + 50;
                size = new nfloat(size * 1.2);
                _heightOfThePage = size;
                ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, size);
            }
            else ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            // displaying
            Title = Application.LocalizedString("ModeTrackingTitle");
            InitialiseAllStrings();
            ActivateButton.Layer.CornerRadius = 4;
            ActivateButton.Layer.MasksToBounds = true;
            if (App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded < (int)VersionEmbeddedEnum.V1007)
            {
                TitlePowerSavingButton.Hidden = true;
                DescriptionPowerSavingLabel.Hidden = true;
                ActivatePowerSavingLabel.Hidden = true;
                PowerSavingImage.Hidden = true;
                PowerSavingSwitch.Hidden = true;
                GoTutorialImage.Hidden = true;
            }
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void SetPickerToView()
        {
            _picker = new RefreshPositionPickerView();
            _picker.InitEventPickerView(MinutesPickerView);
            MinutesPickerView.InputView = _picker;
            MinutesPickerView.InputAccessoryView = _picker.GetToolbar(ScrollView
                , TitlePowerSavingButton
                , PowerSavingSwitch);
        }

        private void InitialiseAllStrings()
        {
            TitleWorkingLabel.Text = Application.LocalizedString("Functionning");
            DescriptionWorkingLabel.Text = Application.LocalizedString("TrackingPositionExplanation");
            TitleTrackingLabel.Text = Application.LocalizedString("TrackingParameterTitle");
            if (App.Locator.ModeTracking.TrackingSetting.RefreshTime < 30)
            {
                PowerSavingSwitch.Enabled = false;
                TitlePowerSavingButton.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace") + " (>=30 min)", UIControlState.Normal);
            }
            else TitlePowerSavingButton.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace"), UIControlState.Normal);
            DescriptionPowerSavingLabel.Text = Application.LocalizedString("DMDescriptionPowerSaving");
            ActivatePowerSavingLabel.Text = Application.LocalizedString("DMActivatePowerSaving");
            UpdatePositionLabel.Text = Application.LocalizedString("TrackingUpdatePosition");
            ActivateButton.SetTitle(Application.LocalizedString("ActivateTracking"), UIControlState.Normal);
        }

        #endregion

        #region ===== Event =======================================================================

        private void TitlePowerSavingButton_TouchDown(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        private async void ActivateButton_TouchUpInside(object sender, EventArgs e)
        {
            App.Locator.ModeTracking.IsPowerSavingEnabled = (MapViewModelBase.RefreshTime < 30) ? false : PowerSavingSwitch.On;
            if (await App.Locator.ModeSelection.SelectMode(ModeDefinitionEnum.ModeTracking))
            {
                GoBack(false);
            }
        }

        #endregion
    }
}