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
    [Register ("AlertView")]
    partial class AlertView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddRecipientButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton AddRecipientFromContactBookButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView AlertMessageTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AlertTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField AlertTitleTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton EmptyDataButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MaxSizeMessageLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MessageTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RecipientsTitleLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ScrollViewContact { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ViewBehindScrollView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddRecipientButton != null) {
                AddRecipientButton.Dispose ();
                AddRecipientButton = null;
            }

            if (AddRecipientFromContactBookButton != null) {
                AddRecipientFromContactBookButton.Dispose ();
                AddRecipientFromContactBookButton = null;
            }

            if (AlertMessageTextField != null) {
                AlertMessageTextField.Dispose ();
                AlertMessageTextField = null;
            }

            if (AlertTitleLabel != null) {
                AlertTitleLabel.Dispose ();
                AlertTitleLabel = null;
            }

            if (AlertTitleTextField != null) {
                AlertTitleTextField.Dispose ();
                AlertTitleTextField = null;
            }

            if (EmptyDataButton != null) {
                EmptyDataButton.Dispose ();
                EmptyDataButton = null;
            }

            if (MaxSizeMessageLabel != null) {
                MaxSizeMessageLabel.Dispose ();
                MaxSizeMessageLabel = null;
            }

            if (MessageTitleLabel != null) {
                MessageTitleLabel.Dispose ();
                MessageTitleLabel = null;
            }

            if (RecipientsTitleLabel != null) {
                RecipientsTitleLabel.Dispose ();
                RecipientsTitleLabel = null;
            }

            if (ScrollViewContact != null) {
                ScrollViewContact.Dispose ();
                ScrollViewContact = null;
            }

            if (ViewBehindScrollView != null) {
                ViewBehindScrollView.Dispose ();
                ViewBehindScrollView = null;
            }
        }
    }
}