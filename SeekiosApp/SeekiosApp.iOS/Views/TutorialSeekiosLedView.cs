using CoreGraphics;
using Foundation;
using SeekiosApp.iOS.Views;
using System;
using System.Linq;
using UIKit;

namespace SeekiosApp.iOS
{
	public partial class TutorialSeekiosLedView : BaseViewController
    {
        #region ===== Attributs ===================================================================

        private static nfloat _heightOfThePage = 0;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialSeekiosLedView(IntPtr handle) : base(handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();
		}

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("SeekiosLEDsShort");
            base.ViewWillAppear(animated);
            SetDataAndStyleToView();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_heightOfThePage == 0)
            {
                nfloat size = 0;
                var lastElement = ScrollView.Subviews[27]; // keep the order of the elements in the view
                size = lastElement.Frame.Y + lastElement.Frame.Height + 50;
                size = new nfloat(size * 1.1);
                _heightOfThePage = size;
                ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, size);
            }
            else ScrollView.ContentSize = new CGSize(View.Frame.Size.Width, _heightOfThePage);
            ScrollView.DirectionalLockEnabled = true;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
		{
            StarterBlinkingLabel.Text = Application.LocalizedString("StarterBlinking");
            StarterTitleLabel.Text = Application.LocalizedString("StarterTitle");
            StarterExplanationLabel.Text = Application.LocalizedString("StarterExplanation");

            ExtinctionBlinkingLabel.Text = Application.LocalizedString("ExtinctionBlinking");
            ExtinctionTitleLabel.Text = Application.LocalizedString("ExtinctionTitle");
            ExtinctionExplanationLabel.Text = Application.LocalizedString("ExtinctionExplanation");

            SOSBlinkingLabel.Text = Application.LocalizedString("SOSBlinking");
            SOSTitleLabel.Text = Application.LocalizedString("SOSTitle");
            SOSExplanationLabel.Text = Application.LocalizedString("SOSExplanation");

            BatteryTitleLabel.Text = Application.LocalizedString("BatteryTitle");
            BatteryExplanationLabel.Text = Application.LocalizedString("BatteryExplanation");
            GreenBetweenLabel.Text = Application.LocalizedString("Between1");
            OrangeBetweenLabel.Text = Application.LocalizedString("Between2");
            RedBetweenLabel.Text = Application.LocalizedString("Between3");

            CommunicationLabel.Text = Application.LocalizedString("SOSBlinking");
            CommunicationTitleLabel.Text = Application.LocalizedString("CommunicateTitle");
            CommunicationExplanationLabel.Text = Application.LocalizedString("CommunicateExplanation");
        }

		#endregion
	}
}