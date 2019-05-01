using Foundation;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class CustomCellDetailSeekiosView : UITableViewCell
    {
		#region Computed Properties

		public UIImage pictureView
		{
			get { return PictureView.Image; }
			set { PictureView.Image = value; }
		}

		public UILabel titleNameLabel
		{
			get { return TitleNameLabel; }
			set { TitleNameLabel = value; }
		}

		public UILabel detailLabel
		{
			get { return DetailLabel; }
			set { DetailLabel = value; }
		}

		public UIButton Buttondelete { 
			get { return ButtonDelete; } 
			set { ButtonDelete = value; }
		}

        public UIButton buttonDeleteZoneForTouch
        {
            get { return DeleteButtonZoneForTouch; }
            set { DeleteButtonZoneForTouch = value; }
        }

		#endregion

		public CustomCellDetailSeekiosView(IntPtr handle) : base(handle)
        {

		}
    }
}