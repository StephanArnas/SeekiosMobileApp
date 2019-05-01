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
    [Register ("MapHistoricView")]
    partial class MapHistoricView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIDatePicker actionSheetDatePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangeMapTypeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FocusOnSeekiosButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView LoadingIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LowerDateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MapKit.MKMapView MapViewControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MapZoomInButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MapZoomOutButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NextPositionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel OldDateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PositionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PreviousPositionButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RecentDateLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISlider Slider { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UpperDateLabel { get; set; }

        [Action ("NextPositionButton_Click:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void NextPositionButton_Click (UIKit.UIButton sender);

        [Action ("PreviousPositionButton_Click:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void PreviousPositionButton_Click (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (actionSheetDatePicker != null) {
                actionSheetDatePicker.Dispose ();
                actionSheetDatePicker = null;
            }

            if (ChangeMapTypeButton != null) {
                ChangeMapTypeButton.Dispose ();
                ChangeMapTypeButton = null;
            }

            if (FocusOnSeekiosButton != null) {
                FocusOnSeekiosButton.Dispose ();
                FocusOnSeekiosButton = null;
            }

            if (LoadingIndicator != null) {
                LoadingIndicator.Dispose ();
                LoadingIndicator = null;
            }

            if (LowerDateLabel != null) {
                LowerDateLabel.Dispose ();
                LowerDateLabel = null;
            }

            if (MapViewControl != null) {
                MapViewControl.Dispose ();
                MapViewControl = null;
            }

            if (MapZoomInButton != null) {
                MapZoomInButton.Dispose ();
                MapZoomInButton = null;
            }

            if (MapZoomOutButton != null) {
                MapZoomOutButton.Dispose ();
                MapZoomOutButton = null;
            }

            if (NextPositionButton != null) {
                NextPositionButton.Dispose ();
                NextPositionButton = null;
            }

            if (OldDateLabel != null) {
                OldDateLabel.Dispose ();
                OldDateLabel = null;
            }

            if (PositionLabel != null) {
                PositionLabel.Dispose ();
                PositionLabel = null;
            }

            if (PreviousPositionButton != null) {
                PreviousPositionButton.Dispose ();
                PreviousPositionButton = null;
            }

            if (RecentDateLabel != null) {
                RecentDateLabel.Dispose ();
                RecentDateLabel = null;
            }

            if (Slider != null) {
                Slider.Dispose ();
                Slider = null;
            }

            if (UpperDateLabel != null) {
                UpperDateLabel.Dispose ();
                UpperDateLabel = null;
            }
        }
    }
}