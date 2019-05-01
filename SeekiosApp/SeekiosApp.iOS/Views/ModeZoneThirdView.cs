using System;
using UIKit;
using SeekiosApp.Model.DTO;
using SeekiosApp.iOS.Views.TableSources;
using SeekiosApp.iOS.Helper;
using SeekiosApp.iOS.Views;
using System.Collections.Generic;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using SeekiosApp.iOS.Views.CustomComponents.CustomPicker;
using SeekiosApp.ViewModel;
using System.Linq;
using CoreGraphics;
using SeekiosApp.Enum;

namespace SeekiosApp.iOS
{
    public partial class ModeZoneThirdView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private SeekiosDTO _seekiosSelected = null;
        private RefreshPositionPickerView _picker = null;
        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ===== Constructor =================================================================

        public ModeZoneThirdView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetPickerView();
            TrackingZoneSwitch.ValueChanged += TrackingZoneSwitch_ValueChanged;
            ActivateButton.TouchUpInside += ActivateButton_TouchUpInside;
            TitlePowerSavingButton.TouchDown += TitlePowerSavingButton_TouchDown;

            App.Locator.ModeZone.IsTrackingSettingEnable = App.Locator.ModeZone.TrackingSetting.IsEnable;
            TrackingZoneSwitch.On = App.Locator.ModeZone.IsTrackingSettingEnable;
            TrackingZoneSwitch_ValueChanged(TrackingZoneSwitch, null);
            _picker.InitDefaultValue(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeZone.TrackingSetting.RefreshTime));
            PowerSavingSwitch.On = App.Locator.ModeZone.TrackingSetting.IsPowerSavingEnabled;
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("ModeZone3Title");
            base.ViewWillAppear(animated);
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

        public override void ViewWillDisappear(bool animated)
        {
            if (!App.Locator.ModeZone.IsGoingBack) Title = "3/3";
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            InitialiseAllStrings();
            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
            if (_seekiosSelected.VersionEmbedded_idversionEmbedded < (int)VersionEmbeddedEnum.V1007)
            {
                TitlePowerSavingButton.Hidden = true;
                PowerSavingNextImage.Hidden = true;
                PowerSavingImage.Hidden = true;
                DescriptionPowerSavingLabel.Hidden = true;
                ActivatePowerSavingLabel.Hidden = true;
                PowerSavingSwitch.Hidden = true;
            }
            // set up the data
            App.Locator.ModeZone.WaitingForAlerts = false;

            // set up the ui component 
            ActivateButton.Layer.MasksToBounds = true;
            ActivateButton.Layer.CornerRadius = 4;
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void SetPickerView()
        {
            _picker = new RefreshPositionPickerView();
            _picker.InitEventPickerView(MinutesPickerView);
            MinutesPickerView.InputView = _picker;
            MinutesPickerView.InputAccessoryView = _picker.GetToolbar(ScrollView);
        }

        private void InitialiseAllStrings()
        {
            TitleWorkingLabel.Text = Application.LocalizedString("Functionning");
            DescriptionWorkingLabel.Text = Application.LocalizedString("ZoneTrackingExplanation");
            TitleTrackingLabel.Text = Application.LocalizedString("DMTitleTracking");
            DescriptionTrackingLabel.Text = Application.LocalizedString("ZoneDescriptionTracking");
            TitlePowerSavingButton.SetTitle(Application.LocalizedString("DMTitlePowerSavingWithSpace"), UIControlState.Normal);
            DescriptionPowerSavingLabel.Text = Application.LocalizedString("DMDescriptionPowerSaving");
            ActivatePowerSavingLabel.Text = Application.LocalizedString("DMActivatePowerSaving");
            UpdatePositionLabel.Text = Application.LocalizedString("ZoneUpdatePosition");
            ActivateButton.SetTitle(Application.LocalizedString("ActivateZone"), UIControlState.Normal);
        }

        #endregion

        #region ===== Event =======================================================================

        private void TitlePowerSavingButton_TouchDown(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        private void TrackingZoneSwitch_ValueChanged(object sender, EventArgs e)
        {
            if ((sender as UISwitch).On)
            {
                App.Locator.ModeZone.IsTrackingSettingEnable = true;
                UpdatePositionLabel.Hidden = false;
                PickerViewButton.Hidden = false;
                MinutesPickerView.Hidden = false;
            }
            else
            {
                App.Locator.ModeZone.IsTrackingSettingEnable = false;
                UpdatePositionLabel.Hidden = true;
                PickerViewButton.Hidden = true;
                MinutesPickerView.Hidden = true;
            }
        }

        private async void ActivateButton_TouchUpInside(object sender, EventArgs e)
        {
            App.Locator.ModeZone.IsActivityFocused = false;
            if (!TrackingZoneSwitch.On) MapViewModelBase.RefreshTime = 0;
            App.Locator.ModeZone.IsPowerSavingEnabled = PowerSavingSwitch.On;

            if (await App.Locator.ModeZone.UpdateZone())
            {
                App.Locator.ModeZone.IsGoingBack = true;
                if (App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                App.Locator.ModeZone.IsOnEditMode = false;
                GoBack(true);
            }
        }

        #endregion
    }
}
