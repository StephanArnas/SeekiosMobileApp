using Foundation;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class CustomCellModeDontMove : UITableViewCell
    {
		#region Computed Properties

		public UIView TypeView { get { return AlertTypeView; } }

		public UILabel TitleLabel
		{
			get { return AlertTitleLabel; }
			set { AlertTitleLabel = value; }
		}

		public UILabel MessageLabel
		{
			get { return AlertMessageLabel; }
			set { AlertMessageLabel = value; }
		}

		public UILabel RecipientLabel
		{
			get { return AlertRecipientLabel; }
			set { AlertRecipientLabel = value; }
		}

		#endregion

		public CustomCellModeDontMove (IntPtr handle) : base (handle)
        {
        }
    }
}