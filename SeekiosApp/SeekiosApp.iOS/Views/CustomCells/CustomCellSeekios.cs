using Foundation;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class CustomCellSeekios : UITableViewCell
    {

		#region Computed Properties

		public UIImageView seekiosImage
        {
            get { return SeekiosImage; }
            set { SeekiosImage = value; }
        }

        public string seekiosNameLabel
        {
            get { return SeekiosNameLabel.Text; }
            set
            {
                NeedUpdateButton.TouchDown -= NeedUpdateButton_TouchDown;
                NeedUpdateButton.TouchDown += NeedUpdateButton_TouchDown;
                SeekiosNameLabel.Text = value;
            }
        }

        public UIColor SeekiosNameLabelColor
        {
            get { return SeekiosNameLabel.TextColor; }
            set { SeekiosNameLabel.TextColor = value; }
        }

        public string lastPositionLabel
        {
            get { return LastPositionLabel.Text; }
            set { LastPositionLabel.Text = value; }
        }

        public UIColor LastPositionLabelColor
        {
            get { return LastPositionLabel.TextColor; }
            set { LastPositionLabel.TextColor = value; }
        }

        public UIImage alertImage
        {
            get { return AlertImage.Image; }
            set { AlertImage.Image = value; }
        }

        public bool AlertImageHidden
        {
            get { return AlertImage.Hidden; }
            set { AlertImage.Hidden = value; }
        }

        public string alertLabel
        {
            get { return AlertLabel.Text; }
            set { AlertLabel.Text = value; }
        }

        public bool AlertLabelHidden
        {
            get { return AlertLabel.Hidden; }
            set { AlertLabel.Hidden = value; }
        }

        public UIColor AlertLabelColor
        {
            get { return AlertLabel.TextColor; }
            set { AlertLabel.TextColor = value; }
        }

        public UIImage modeImage
        {
            get { return ModeImage.Image; }
            set { ModeImage.Image = value; }
        }

        public bool ModeImageHidden
        {
            get { return ModeImage.Hidden; }
            set { ModeImage.Hidden = value; }
        }

        public string modeLabel
        {
            get { return ModeLabel.Text; }
            set { ModeLabel.Text = value; }
        }

        public UIColor modeTinColor
        {
            get { return ModeImage.TintColor; }
            set { ModeImage.TintColor = value; }
        }

        public bool ModeLabelHidden
        {
            get { return ModeLabel.Hidden; }
            set { ModeLabel.Hidden = value; }
        }

        public UIColor ModeLabelColor
        {
            get { return ModeLabel.TextColor; }
            set { ModeLabel.TextColor = value; }
        }

        public bool PowerSavingImageHidden
        {
            get { return PowerSavingImage.Hidden; }
            set { PowerSavingImage.Hidden = value; }
        }

        public bool NeedUpdateButtonHidden
        {
            get { return NeedUpdateButton.Hidden; }
            set { NeedUpdateButton.Hidden = value; }
        }

        public void MoveUpdateButton()
        {
            var constraintY = NSLayoutConstraint.Create(NeedUpdateButton, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, SeekiosImage, NSLayoutAttribute.CenterY, 1, 3);
            constraintY.Active = true;
            NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { constraintY });
        }

        public void SetCircularSeekiosImage()
        {
            SeekiosImage.Layer.CornerRadius = SeekiosImage.Frame.Size.Width / 2;
            SeekiosImage.ClipsToBounds = true;
        }

        public event EventHandler TouchDown;

        #endregion

        public CustomCellSeekios(IntPtr handle) : base(handle)
        {
        }

        private void NeedUpdateButton_TouchDown(object sender, EventArgs e)
        {
            TouchDown?.Invoke(null, EventArgs.Empty);
        }
    }
}