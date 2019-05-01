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
    [Register ("TutorialBackgroundView")]
    partial class TutorialBackgroundView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonSkip { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonSkip != null) {
                ButtonSkip.Dispose ();
                ButtonSkip = null;
            }
        }
    }
}