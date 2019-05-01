using Foundation;
using SeekiosApp.iOS.Views;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class NeedUpdateView : UIViewController
    {
        public NeedUpdateView (IntPtr handle) : base (handle)
        {

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            GoToStoreButton.ContentEdgeInsets = new UIEdgeInsets(5, 10, 5, 10);
            GoToStoreButton.Layer.CornerRadius = 4;
            GoToStoreButton.Layer.MasksToBounds = true;
            GoToStoreButton.TouchUpInside += GoToStoreButton_TouchUpInside;
            GoToStoreButton.SetTitle(Application.LocalizedString("DoUpdate"), UIControlState.Normal);
            UpdateAppLabel.Text = Application.LocalizedString("UpdateApp");
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            GoToStoreButton.TouchUpInside -= GoToStoreButton_TouchUpInside;
        }

        private void GoToStoreButton_TouchUpInside(object sender, EventArgs e)
        {
            //https://itunes.apple.com/us/app/seekios/id1173443647?ls=1&mt=8
            UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/seekios/id1173443647?ls=1&mt=8"));
        }
    }
}