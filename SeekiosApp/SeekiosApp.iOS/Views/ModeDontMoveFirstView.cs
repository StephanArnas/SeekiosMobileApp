using System;
using UIKit;
using SeekiosApp.ViewModel;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.CustomComponents.CustomPicker;
using SeekiosApp.iOS.Helper;
using SeekiosApp.Enum;
using System.Linq;
using CoreGraphics;

namespace SeekiosApp.iOS
{
    public partial class ModeDontMoveFirstView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private RefreshPositionPickerView _picker = null;
        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ===== Constructor =================================================================

        public ModeDontMoveFirstView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetPickerView();
            TrackingSwitch.ValueChanged += TrackingDontMoveSwitch_ValueChanged;
            ActivateButton.TouchUpInside += ActivateButton_TouchUpInside;
            TitlePowerSavingButton.TouchDown += TitlePowerSavingButton_TouchDown;

            App.Locator.ModeDontMove.IsTrackingSettingEnable = App.Locator.ModeDontMove.TrackingSetting.IsEnable;
            TrackingSwitch.On = App.Locator.ModeDontMove.IsTrackingSettingEnable;
            TrackingDontMoveSwitch_ValueChanged(TrackingSwitch, null);
            _picker.InitDefaultValue(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeDontMove.TrackingSetting.RefreshTime));
            PowerSavingSwitch.On = App.Locator.ModeDontMove.TrackingSetting.IsPowerSavingEnabled;
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("DMTitle");
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            Title = "2/2";
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                var lastElement = ScrollView.Subviews[13]; // keep the order of the elements in the view
                size = lastElement.Frame.Y + lastElement.Frame.Height + 50;
                size = new nfloat(size * 1.2);
                _heightOfThePage = size;
                ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, size);
            }
            else ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
        }

        public override void ViewDidUnload()
		{
			App.Locator.ModeDontMove.LsAlertsModeDontMove?.Clear();
			base.ViewDidUnload();
		}

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            // InitialiseAllStrings
            InitialiseAllStrings();
            ActivateButton.Layer.CornerRadius = 4;
            ActivateButton.Layer.MasksToBounds = true;
            App.Locator.ModeDontMove.InitModeDontMove();

            if (App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded < (int)VersionEmbeddedEnum.V1007)
            {
                TitlePowerSavingButton.Hidden = true;
                PowerSavingNextImage.Hidden = true;
                PowerSavingImage.Hidden = true;
                DescriptionPowerSavingLabel.Hidden = true;
                ActivatePowerSavingLabel.Hidden = true;
                PowerSavingSwitch.Hidden = true;
            }
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void InitialiseAllStrings()
        {
            TitleWorkingLabel.Text = Application.LocalizedString("Functionning");
            DescriptionWorkingLabel.Text = Application.LocalizedString("TrackingAfterMovedExplanation");
            TitleTrackingLabel.Text = Application.LocalizedString("DMTitleTracking");
            DescriptionTrackingLabel.Text = Application.LocalizedString("DMDescriptionTracking");
            UpdatePositionLabel.Text = Application.LocalizedString("DMUpdatePosition");
            TitlePowerSavingButton.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace"), UIControlState.Normal);
            DescriptionPowerSavingLabel.Text = Application.LocalizedString("DMDescriptionPowerSaving");
            ActivatePowerSavingLabel.Text = Application.LocalizedString("DMActivatePowerSaving");
            ActivateButton.SetTitle(Application.LocalizedString("DMActivate"), UIControlState.Normal);
        }

        private void SetPickerView()
        {
            _picker = new RefreshPositionPickerView();
            _picker.InitEventPickerView(MinutesPickerViewTextField);
            MinutesPickerViewTextField.InputView = _picker;
            MinutesPickerViewTextField.InputAccessoryView = _picker.GetToolbar(ScrollView);
        }

        #endregion

        #region ===== Event =======================================================================

        private void TitlePowerSavingButton_TouchDown(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        private void TrackingDontMoveSwitch_ValueChanged(object sender, EventArgs e)
        {
            if ((sender as UISwitch).On)
            {
                App.Locator.ModeDontMove.IsTrackingSettingEnable = true;
                UpdatePositionLabel.Hidden = false;
                MinutesPickerViewTextField.Hidden = false;
                MinutesPickerView.Hidden = false;
            }
            else
            {
                App.Locator.ModeDontMove.IsTrackingSettingEnable = false;
                UpdatePositionLabel.Hidden = true;
                MinutesPickerViewTextField.Hidden = true;
                MinutesPickerView.Hidden = true;
            }
        }

        private async void ActivateButton_TouchUpInside(object sender, EventArgs e)
        {
            if (!TrackingSwitch.On) MapViewModelBase.RefreshTime = 0;
            App.Locator.ModeDontMove.IsPowerSavingEnabled = PowerSavingSwitch.On;
            if (await App.Locator.ModeSelection.SelectMode(ModeDefinitionEnum.ModeDontMove))
            {
                if (!App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                App.Locator.ModeDontMove.IsGoingBack = true;
                GoBack(true);
            }
        }

        #endregion
    }
}