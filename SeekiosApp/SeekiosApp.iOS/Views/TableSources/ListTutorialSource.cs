using Foundation;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace SeekiosApp.iOS.Views.TableSources
{
    public class ListTutorialSource : UITableViewSource
    {
        #region ===== Attributs ===================================================================

        private string _cellIdentifier = "tableView";

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 5;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 60;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as CustomCellTutorial;
            if (cell == null)
            {
                cell = (CustomCellTutorial)new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (indexPath.Section == 0)
            {
				if (indexPath.Row == 0)
				{
					cell.tutorialNameLabel.Text = Application.LocalizedString("GeneralHelp");
					cell.tutorialPictureView = UIImage.FromBundle("TutoIcon1");
				}
				else if (indexPath.Row == 1)
				{
					cell.tutorialNameLabel.Text = Application.LocalizedString("SeekiosLEDs");
					cell.tutorialPictureView = UIImage.FromBundle("SeekiosLed");
                }
                else if (indexPath.Row == 2)
                {
                    cell.tutorialNameLabel.Text = Application.LocalizedString("PowerSaving");
                    cell.tutorialPictureView = UIImage.FromBundle("PowerSaving");
                }
                else if (indexPath.Row == 3)
                {
                    cell.tutorialNameLabel.Text = Application.LocalizedString("CreditCostTitle");
                    cell.tutorialPictureView = UIImage.FromBundle("IconCredit");
                }
                else if (indexPath.Row == 4)
                {
                    cell.tutorialNameLabel.Text = Application.LocalizedString("OnlineHelp");
                    cell.tutorialPictureView = UIImage.FromBundle("Tutorial");
                }
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
				if (indexPath.Row == 0)
				{
					App.Locator.Parameter.GoToTutorialBackground();
				}
				else if (indexPath.Row == 1)
				{
					App.Locator.Parameter.GoToSeekiosLed();
				}
                else if (indexPath.Row == 2)
                {
                    App.Locator.Parameter.GoToTutorialPowerSaving();
                }
                else if (indexPath.Row == 3)
                {
                    App.Locator.Parameter.GoToTutorialCreditCost();
                }
                else if (indexPath.Row == 4)
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl(App.TutorialHelpLink));
                }
            }
        }

        #endregion
    }
}
