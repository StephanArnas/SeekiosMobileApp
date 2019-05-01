using Foundation;
using SeekiosApp.iOS.Views;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class TutorialForegroundView : UIViewController
    {
        #region ===== Attributs ===================================================================

        public int PageIndex = 0;
        public string Details;
        public string ImageFile;

        #endregion

        #region ====== Constructor ================================================================

        public TutorialForegroundView(IntPtr handle) : base(handle) { }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TutorialimageView.Image = UIImage.FromBundle(ImageFile);
            DetailTextView.Text = Details;
			DetailTextView.Editable = false;
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

        #endregion
    }
}