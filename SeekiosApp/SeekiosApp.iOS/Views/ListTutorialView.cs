using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.TableSources;

namespace SeekiosApp.iOS
{
    public partial class ListTutorialView : BaseViewController
    {
        #region ===== Constructor =================================================================

        public ListTutorialView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("SeekiosHelp");
            base.ViewWillAppear(animated);
            Tableview.Source = new ListTutorialSource();
            Tableview.ReloadData();
        }

        public override void ViewWillDisappear(bool animated)
        {
            Title = Application.LocalizedString("Help");
            base.ViewWillDisappear(animated);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView() { }

        #endregion
    }
}