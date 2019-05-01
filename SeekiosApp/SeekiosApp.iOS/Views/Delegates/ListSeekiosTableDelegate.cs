using System;
using UIKit;

namespace SeekiosApp.iOS.Classes
{
    public class ListSeekiosTableDelegate : UITableViewDelegate
    {
        #region ===== Attributs ===================================================================

		private ListSeekiosView _controller;

        #endregion

        #region ===== Constructor =================================================================

        public ListSeekiosTableDelegate() { }

        public ListSeekiosTableDelegate(ListSeekiosView controller)
        {
            _controller = controller;
        }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return 93.0f;
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            // output selected row
            Console.WriteLine("Row selected: {0}", indexPath.Row);
            if (indexPath.Section == 0)
            {
                App.Locator.DetailSeekios.SeekiosSelected = App.Locator.ListSeekios.LsSeekios[indexPath.Row];
                App.Locator.ListSeekios.GoToSeekiosDetail();
            }
        }

        #endregion
    }
}