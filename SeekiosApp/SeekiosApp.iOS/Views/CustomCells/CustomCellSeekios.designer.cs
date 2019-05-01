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
    [Register ("CustomCellSeekios")]
    partial class CustomCellSeekios
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView AlertImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AlertLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LastPositionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ModeImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ModeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NeedUpdateButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView PowerSavingImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SeekiosImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SeekiosNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AlertImage != null) {
                AlertImage.Dispose ();
                AlertImage = null;
            }

            if (AlertLabel != null) {
                AlertLabel.Dispose ();
                AlertLabel = null;
            }

            if (LastPositionLabel != null) {
                LastPositionLabel.Dispose ();
                LastPositionLabel = null;
            }

            if (ModeImage != null) {
                ModeImage.Dispose ();
                ModeImage = null;
            }

            if (ModeLabel != null) {
                ModeLabel.Dispose ();
                ModeLabel = null;
            }

            if (NeedUpdateButton != null) {
                NeedUpdateButton.Dispose ();
                NeedUpdateButton = null;
            }

            if (PowerSavingImage != null) {
                PowerSavingImage.Dispose ();
                PowerSavingImage = null;
            }

            if (SeekiosImage != null) {
                SeekiosImage.Dispose ();
                SeekiosImage = null;
            }

            if (SeekiosNameLabel != null) {
                SeekiosNameLabel.Dispose ();
                SeekiosNameLabel = null;
            }
        }
    }
}