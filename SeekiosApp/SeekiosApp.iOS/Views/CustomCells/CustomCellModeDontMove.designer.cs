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
    [Register ("CustomCellModeDontMove")]
    partial class CustomCellModeDontMove
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AlertMessageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AlertRecipientLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AlertTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView AlertTypeView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imageAlertUser { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AlertMessageLabel != null) {
                AlertMessageLabel.Dispose ();
                AlertMessageLabel = null;
            }

            if (AlertRecipientLabel != null) {
                AlertRecipientLabel.Dispose ();
                AlertRecipientLabel = null;
            }

            if (AlertTitleLabel != null) {
                AlertTitleLabel.Dispose ();
                AlertTitleLabel = null;
            }

            if (AlertTypeView != null) {
                AlertTypeView.Dispose ();
                AlertTypeView = null;
            }

            if (imageAlertUser != null) {
                imageAlertUser.Dispose ();
                imageAlertUser = null;
            }
        }
    }
}