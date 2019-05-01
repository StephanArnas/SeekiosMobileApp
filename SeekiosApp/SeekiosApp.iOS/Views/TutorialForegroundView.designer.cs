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
    [Register ("TutorialForegroundView")]
    partial class TutorialForegroundView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView DetailTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView TutorialimageView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DetailTextView != null) {
                DetailTextView.Dispose ();
                DetailTextView = null;
            }

            if (TutorialimageView != null) {
                TutorialimageView.Dispose ();
                TutorialimageView = null;
            }
        }
    }
}