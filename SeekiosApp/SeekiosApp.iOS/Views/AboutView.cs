using Foundation;
using SeekiosApp.iOS.Views;
using System;
using UIKit;
using SeekiosApp.iOS.Helper;

namespace SeekiosApp.iOS
{
    public partial class AboutView : BaseViewController
    {
        #region ===== Constructor =================================================================

        public AboutView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            FacebookButton.TouchUpInside += FacebookButton_TouchUpInside;
            TwitterButton.TouchUpInside += TwitterButton_TouchUpInside;
            InstagramButton.TouchUpInside += InstagramButton_TouchUpInside;
            LinkedinButton.TouchUpInside += LinkedinButton_TouchUpInside;

            SeekiosButton.TouchUpInside += SeekiosButton_TouchUpInside;
            PrivacyPolicyButton.TouchUpInside += PrivacyPolicyButton_TouchUpInside;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            Title = Application.LocalizedString("About");
            PrivacyPolicyButton.SetTitle(Application.LocalizedString("PrivacyPolicy"), UIControlState.Normal);
            VersionLabel.Text = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
        }

        #endregion

        #region ===== Privates Methodes ===========================================================

        #endregion

        #region ====== Event ======================================================================

        private void PrivacyPolicyButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.PolicyLink);
        }

        private void SeekiosButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.SeekiosLink);
        }

        private void LinkedinButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.LinkedinLink);
        }

        private void InstagramButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.InstagramLink);
        }

        private void TwitterButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.TwitterLink);
        }

        private void FacebookButton_TouchUpInside(object sender, EventArgs e)
        {
            OpenSafari(App.FacebookLink);
        }

        private void OpenSafari(string url)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(url));
        }

        #endregion
    }
}