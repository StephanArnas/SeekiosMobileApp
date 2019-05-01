using Foundation;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class CustomCellTutorial : UITableViewCell
    {
        public UIImage tutorialPictureView
        {
            get { return TutorialPictureView.Image; }
            set { TutorialPictureView.Image = value; }
        }

        public UILabel tutorialNameLabel
        {
            get { return TutorialNameLabel; }
            set { TutorialNameLabel = value; }
        }

        public CustomCellTutorial(IntPtr handle) : base (handle)
        {
        }
    }
}