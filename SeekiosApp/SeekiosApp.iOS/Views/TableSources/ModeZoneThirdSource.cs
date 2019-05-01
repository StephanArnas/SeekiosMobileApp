using CoreAnimation;
using Foundation;
using SeekiosApp.iOS.ModeZone;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace SeekiosApp.iOS.Views.TableSources
{
    public class ModeZoneThirdSource : UITableViewSource
    {
        #region ===== Attributs ===================================================================

        private int _currentIndex = 0;
        private string _cellIdentifier = "tableView";
        private AlertWithRecipientDTO _currentAlert = null;
		private ModeZoneSecondView _controller;
		#endregion

		#region ===== Constructor =================================================================

		public ModeZoneThirdSource(ModeZoneSecondView _controller) {
			this._controller = _controller;
		}

		#endregion

		#region ===== Public Overrides Methodes ===================================================

		public override nint RowsInSection(UITableView tableview, nint section)
        {
            return App.Locator.ModeZone.LsAlertsModeZone.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 60;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as CustomCellModeZoneView;
            if (cell == null)
            {
				cell = (CustomCellModeZoneView) new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);
            }

            _currentIndex = indexPath.Row;
            _currentAlert = App.Locator.ModeZone.LsAlertsModeZone[_currentIndex];
            tableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			// rounded alert type
			CALayer profileImageCircle = cell.TypeView.Layer;
            profileImageCircle.CornerRadius = cell.TypeView.Frame.Size.Width / 2;
            cell.TypeView.ClipsToBounds = true;

            // title and body 
            cell.TitleLabel.Text = _currentAlert.Title;
            cell.MessageLabel.Text = _currentAlert.Content;

            // number of recipient
            CALayer profileImageCircle1 = cell.RecipientLabel.Layer;
            profileImageCircle1.CornerRadius = cell.RecipientLabel.Frame.Size.Width / 2;
            cell.RecipientLabel.ClipsToBounds = true;
            cell.RecipientLabel.Text = _currentAlert.LsRecipients.Count.ToString();

            return cell;
        }

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle)
			{
				case UITableViewCellEditingStyle.Delete:
					// remove the item from the underlying data source
					App.Locator.ModeZone.LsAlertsModeZone.RemoveAt(indexPath.Row);
					_controller.UpdateAlertData();
					break;
				case UITableViewCellEditingStyle.None:
					Console.WriteLine("CommitEditingStyle:None called");
					break;
			}
		}

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            App.Locator.ModeZone.GoToAlertDetail(_currentAlert, _currentIndex);
        }

        #endregion
    }
}
