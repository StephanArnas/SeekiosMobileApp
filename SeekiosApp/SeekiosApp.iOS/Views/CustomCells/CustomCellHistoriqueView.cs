using Foundation;
using System;
using UIKit;

namespace SeekiosApp.iOS
{
    public partial class CustomCellHistoriqueView : UITableViewCell
    {

		#region Computed Properties

		public UIImageView historiqueImageView
		{
			get { return HistoriqueImageView; }
			set { HistoriqueImageView = value; }
		}

		public string titleNameLabel
		{
			get { return TitleNameLabel.Text; }
			set { TitleNameLabel.Text = value; }
		}

		public string seekiosNameLabel
		{
			get { return SeekiosNameLabel.Text; }
			set { SeekiosNameLabel.Text = value; }
		}

		public string dateTimeLabel
		{
			get { return DateTimeLabel.Text; }
			set { DateTimeLabel.Text = value; }
		}

		public string numberOfElementsLabel
		{
			get { return NumberOfElementsLabel.Text; }
			set { NumberOfElementsLabel.Text = value; }
		}

		public UILabel ElementsLabel
		{ 
			get { return NumberOfElementsLabel; }
			set { NumberOfElementsLabel = value; }
		}

		#endregion

        public CustomCellHistoriqueView (IntPtr handle) : base (handle)
        {
        }
    }
}