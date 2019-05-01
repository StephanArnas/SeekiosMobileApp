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
    [Register ("CustomCellDetailSeekiosView")]
    partial class CustomCellDetailSeekiosView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonDelete { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DeleteButtonZoneForTouch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DetailLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PictureView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonDelete != null) {
                ButtonDelete.Dispose ();
                ButtonDelete = null;
            }

            if (DeleteButtonZoneForTouch != null) {
                DeleteButtonZoneForTouch.Dispose ();
                DeleteButtonZoneForTouch = null;
            }

            if (DetailLabel != null) {
                DetailLabel.Dispose ();
                DetailLabel = null;
            }

            if (PictureView != null) {
                PictureView.Dispose ();
                PictureView = null;
            }

            if (TitleNameLabel != null) {
                TitleNameLabel.Dispose ();
                TitleNameLabel = null;
            }
        }
    }
}