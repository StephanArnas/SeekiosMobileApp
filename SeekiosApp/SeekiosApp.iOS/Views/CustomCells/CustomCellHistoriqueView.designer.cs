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
    [Register ("CustomCellHistoriqueView")]
    partial class CustomCellHistoriqueView
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DateTimeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView HistoriqueImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NumberOfElementsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SeekiosNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TitleNameLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DateTimeLabel != null) {
                DateTimeLabel.Dispose ();
                DateTimeLabel = null;
            }

            if (HistoriqueImageView != null) {
                HistoriqueImageView.Dispose ();
                HistoriqueImageView = null;
            }

            if (NumberOfElementsLabel != null) {
                NumberOfElementsLabel.Dispose ();
                NumberOfElementsLabel = null;
            }

            if (SeekiosNameLabel != null) {
                SeekiosNameLabel.Dispose ();
                SeekiosNameLabel = null;
            }

            if (TitleNameLabel != null) {
                TitleNameLabel.Dispose ();
                TitleNameLabel = null;
            }
        }
    }
}