// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace SeekiosApp.iOS
{
    [Register ("ModeZoneThirdView")]
    partial class ModeZoneThirdView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ActivateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ActivatePowerSavingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionPowerSavingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionTrackingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DescriptionWorkingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField MinutesPickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView MyPickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PickerViewButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PowerSavingImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PowerSavingNextImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch PowerSavingSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TitlePowerSavingButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleTrackingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleWorkingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch TrackingZoneSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UpdatePositionLabel { get; set; }

        [Action ("NextButton_Click:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void NextButton_Click (UIKit.UIButton sender);

        [Action ("PreviousButton_Click:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PreviousButton_Click (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ActivateButton != null) {
                ActivateButton.Dispose ();
                ActivateButton = null;
            }

            if (ActivatePowerSavingLabel != null) {
                ActivatePowerSavingLabel.Dispose ();
                ActivatePowerSavingLabel = null;
            }

            if (DescriptionPowerSavingLabel != null) {
                DescriptionPowerSavingLabel.Dispose ();
                DescriptionPowerSavingLabel = null;
            }

            if (DescriptionTrackingLabel != null) {
                DescriptionTrackingLabel.Dispose ();
                DescriptionTrackingLabel = null;
            }

            if (DescriptionWorkingLabel != null) {
                DescriptionWorkingLabel.Dispose ();
                DescriptionWorkingLabel = null;
            }

            if (MinutesPickerView != null) {
                MinutesPickerView.Dispose ();
                MinutesPickerView = null;
            }

            if (MyPickerView != null) {
                MyPickerView.Dispose ();
                MyPickerView = null;
            }

            if (PickerViewButton != null) {
                PickerViewButton.Dispose ();
                PickerViewButton = null;
            }

            if (PowerSavingImage != null) {
                PowerSavingImage.Dispose ();
                PowerSavingImage = null;
            }

            if (PowerSavingNextImage != null) {
                PowerSavingNextImage.Dispose ();
                PowerSavingNextImage = null;
            }

            if (PowerSavingSwitch != null) {
                PowerSavingSwitch.Dispose ();
                PowerSavingSwitch = null;
            }

            if (ScrollView != null) {
                ScrollView.Dispose ();
                ScrollView = null;
            }

            if (TitlePowerSavingButton != null) {
                TitlePowerSavingButton.Dispose ();
                TitlePowerSavingButton = null;
            }

            if (TitleTrackingLabel != null) {
                TitleTrackingLabel.Dispose ();
                TitleTrackingLabel = null;
            }

            if (TitleWorkingLabel != null) {
                TitleWorkingLabel.Dispose ();
                TitleWorkingLabel = null;
            }

            if (TrackingZoneSwitch != null) {
                TrackingZoneSwitch.Dispose ();
                TrackingZoneSwitch = null;
            }

            if (UpdatePositionLabel != null) {
                UpdatePositionLabel.Dispose ();
                UpdatePositionLabel = null;
            }
        }
    }
}