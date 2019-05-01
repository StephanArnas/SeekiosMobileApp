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
    [Register ("ModeZoneFirstView")]
    partial class ModeZoneFirstView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ChangeMapTypeButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FocusOnSeekiosButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FocusOnZoneButton { get; set; }

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
        UIKit.UIButton NextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NumberOfPointsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton RefreshInProgressButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SurfaceLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TimerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton UndoButton { get; set; }

        [Action ("RefreshInProgressButton_Click:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void RefreshInProgressButton_Click (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ChangeMapTypeButton != null) {
                ChangeMapTypeButton.Dispose ();
                ChangeMapTypeButton = null;
            }

            if (FocusOnSeekiosButton != null) {
                FocusOnSeekiosButton.Dispose ();
                FocusOnSeekiosButton = null;
            }

            if (FocusOnZoneButton != null) {
                FocusOnZoneButton.Dispose ();
                FocusOnZoneButton = null;
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

            if (NextButton != null) {
                NextButton.Dispose ();
                NextButton = null;
            }

            if (NumberOfPointsLabel != null) {
                NumberOfPointsLabel.Dispose ();
                NumberOfPointsLabel = null;
            }

            if (RefreshInProgressButton != null) {
                RefreshInProgressButton.Dispose ();
                RefreshInProgressButton = null;
            }

            if (SurfaceLabel != null) {
                SurfaceLabel.Dispose ();
                SurfaceLabel = null;
            }

            if (TimerLabel != null) {
                TimerLabel.Dispose ();
                TimerLabel = null;
            }

            if (UndoButton != null) {
                UndoButton.Dispose ();
                UndoButton = null;
            }
        }
    }
}