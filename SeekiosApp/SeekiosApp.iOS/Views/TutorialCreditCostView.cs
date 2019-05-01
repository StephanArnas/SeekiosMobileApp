using CoreGraphics;
using Foundation;
using SeekiosApp.iOS.Views;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class TutorialCreditCostView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialCreditCostView (IntPtr handle) : base (handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            MoreExplanationButton.TouchDown += MoreExplanationButton_TouchDown;
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("CreditCostTitle");
            MoreExplanationButton.Layer.CornerRadius = 4;
            MoreExplanationButton.Layer.MasksToBounds = true;
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
                size = new nfloat(size * 1.15);
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
            TitleLabel.Text = Application.LocalizedString("CreditCostTitleLabel");
            CreditCost1Label.Text = Application.LocalizedString("CreditCost1");
            CreditCost2Label.Text = Application.LocalizedString("CreditCost2");
            CreditCost3Label.Text = Application.LocalizedString("CreditCost3");
            CreditCost4Label.Text = Application.LocalizedString("CreditCost4");
            CreditCost5Label.Text = Application.LocalizedString("CreditCost5");
            CreditCost6Label.Text = Application.LocalizedString("CreditCost6");
            CreditCost7Label.Text = Application.LocalizedString("CreditCost7");
            MoreExplanationButton.SetTitle(Application.LocalizedString("MoreCreditExplanation"), UIControlState.Normal);
        }

        #endregion

        #region ====== Event ======================================================================

        private void MoreExplanationButton_TouchDown(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(App.TutorialCreditCostLink));
        }

        #endregion
    }
}