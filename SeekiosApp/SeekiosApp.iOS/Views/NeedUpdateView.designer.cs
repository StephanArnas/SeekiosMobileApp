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
    [Register ("NeedUpdateView")]
    partial class NeedUpdateView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GoToStoreButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView GoToStoreImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel UpdateAppLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (GoToStoreButton != null) {
                GoToStoreButton.Dispose ();
                GoToStoreButton = null;
            }

            if (GoToStoreImage != null) {
                GoToStoreImage.Dispose ();
                GoToStoreImage = null;
            }

            if (UpdateAppLabel != null) {
                UpdateAppLabel.Dispose ();
                UpdateAppLabel = null;
            }
        }
    }
}