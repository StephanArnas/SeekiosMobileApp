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
    [Register ("CustomCellTutorial")]
    partial class CustomCellTutorial
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TutorialNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView TutorialPictureView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (TutorialNameLabel != null) {
                TutorialNameLabel.Dispose ();
                TutorialNameLabel = null;
            }

            if (TutorialPictureView != null) {
                TutorialPictureView.Dispose ();
                TutorialPictureView = null;
            }
        }
    }
}