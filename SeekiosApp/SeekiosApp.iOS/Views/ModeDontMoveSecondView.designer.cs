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
    [Register ("ModeDontMoveSecondView")]
    partial class ModeDontMoveSecondView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonCreateAlert { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel EmailAlertLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EmptyAlertButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EmptyAlertImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MobileNotificationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NotifActivatedLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView TableViewDontMove { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonCreateAlert != null) {
                ButtonCreateAlert.Dispose ();
                ButtonCreateAlert = null;
            }

            if (EmailAlertLabel != null) {
                EmailAlertLabel.Dispose ();
                EmailAlertLabel = null;
            }

            if (EmptyAlertButton != null) {
                EmptyAlertButton.Dispose ();
                EmptyAlertButton = null;
            }

            if (EmptyAlertImageButton != null) {
                EmptyAlertImageButton.Dispose ();
                EmptyAlertImageButton = null;
            }

            if (MobileNotificationLabel != null) {
                MobileNotificationLabel.Dispose ();
                MobileNotificationLabel = null;
            }

            if (NextButton != null) {
                NextButton.Dispose ();
                NextButton = null;
            }

            if (NotifActivatedLabel != null) {
                NotifActivatedLabel.Dispose ();
                NotifActivatedLabel = null;
            }

            if (TableViewDontMove != null) {
                TableViewDontMove.Dispose ();
                TableViewDontMove = null;
            }
        }
    }
}