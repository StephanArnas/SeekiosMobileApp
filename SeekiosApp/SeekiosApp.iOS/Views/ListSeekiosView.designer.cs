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
    [Register ("ListSeekiosView")]
    partial class ListSeekiosView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem AddSeekiosBarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DefaultListSeekiosBackgroundButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DefaultListSeekiosText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIBarButtonItem SliderBarButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView Tableview { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AddSeekiosBarButton != null) {
                AddSeekiosBarButton.Dispose ();
                AddSeekiosBarButton = null;
            }

            if (DefaultListSeekiosBackgroundButton != null) {
                DefaultListSeekiosBackgroundButton.Dispose ();
                DefaultListSeekiosBackgroundButton = null;
            }

            if (DefaultListSeekiosText != null) {
                DefaultListSeekiosText.Dispose ();
                DefaultListSeekiosText = null;
            }

            if (SliderBarButton != null) {
                SliderBarButton.Dispose ();
                SliderBarButton = null;
            }

            if (Tableview != null) {
                Tableview.Dispose ();
                Tableview = null;
            }
        }
    }
}