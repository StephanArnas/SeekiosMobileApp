using CoreGraphics;
using Foundation;
using SeekiosApp.iOS.Views;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class TutorialPowerSavingView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialPowerSavingView(IntPtr handle) : base (handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("PowerSaving");
            base.ViewWillAppear(animated);
            SetDataAndStyleToView();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                var lastElement = ScrollView.Subviews[14]; // keep the order of the elements in the view
                size = lastElement.Frame.Y + lastElement.Frame.Height + 50;
                size = new nfloat(size * 1.1);
                _heightOfThePage = size;
                ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, size);
            }
            else ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            InitialiseAllStrings();
        }

        private void InitialiseAllStrings()
        {
            Title1Label.Text = Application.LocalizedString("PSTitle1");
            Description1Label.Text = Application.LocalizedString("PSDescription1");
            Title2Label.Text = Application.LocalizedString("PSTitle2");
            Description2Label.Text = Application.LocalizedString("PSDescription2");
            Title3Label.Text = Application.LocalizedString("PSTitle3");
            Description31Label.Text = Application.LocalizedString("PSDescription31");
            Description32Label.Text = Application.LocalizedString("PSDescription32");
            Title4Label.Text = Application.LocalizedString("PSTitle4");
            Description41Label.Text = Application.LocalizedString("PSDescription41");
            Description42Label.Text = Application.LocalizedString("PSDescription42");
            Description43Label.Text = Application.LocalizedString("PSDescription43");
            Description44Label.Text = Application.LocalizedString("PSDescription44");
        }

        #endregion
    }
}