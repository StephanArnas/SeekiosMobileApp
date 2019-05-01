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
    [Register ("MapAllSeekiosView")]
    partial class MapAllSeekiosView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        MapKit.MKMapView MapViewControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MapZoomInButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton MapZoomOutButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
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
        }
    }
}