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
    [Register ("AboutView")]
    partial class AboutView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton FacebookButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton InstagramButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LinkedinButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton PrivacyPolicyButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SeekiosButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton TwitterButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel VersionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (FacebookButton != null) {
                FacebookButton.Dispose ();
                FacebookButton = null;
            }

            if (InstagramButton != null) {
                InstagramButton.Dispose ();
                InstagramButton = null;
            }

            if (LinkedinButton != null) {
                LinkedinButton.Dispose ();
                LinkedinButton = null;
            }

            if (PrivacyPolicyButton != null) {
                PrivacyPolicyButton.Dispose ();
                PrivacyPolicyButton = null;
            }

            if (SeekiosButton != null) {
                SeekiosButton.Dispose ();
                SeekiosButton = null;
            }

            if (TwitterButton != null) {
                TwitterButton.Dispose ();
                TwitterButton = null;
            }

            if (VersionLabel != null) {
                VersionLabel.Dispose ();
                VersionLabel = null;
            }
        }
    }
}