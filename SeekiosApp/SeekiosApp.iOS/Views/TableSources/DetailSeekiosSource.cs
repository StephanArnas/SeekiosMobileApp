using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using SeekiosApp.Enum;
using SeekiosApp.iOS.Helper;
using SeekiosApp.Model.DTO;
using UIKit;

namespace SeekiosApp.iOS.Views.TableSources
{
    public class DetailSeekiosSource : UITableViewSource
    {
        #region ===== Attributs ===================================================================

        private SeekiosDTO _seekiosSelected = null;
        private string _cellIdentifier = "tableView";
        private DetailSeekiosView _controller = null;
        //	string[] itemName = new string[] { "Carte", "Recharge credits", "Mode Zone", "Mode Dont Move", "Mode Tracking", "Parametres" };
        //	string[] itemDetail = new string[] { "Vous pouvez rafraichir la position du seekios", " ", "Vous etes alerte si le seekios sort de la zone", "Vous etes alerte si le seekios bouge", "suivre le chemin du seekios", " " };
        //	string[] itemImage = new string[] { "MapDetail", "CreditUser", "ModeZone", "ModeDontMove", "ModeTracking", "SeekiosParameter" };

        #endregion

        #region ===== Constructor =================================================================

        public DetailSeekiosSource(DetailSeekiosView _controller, SeekiosDTO _seekiosSelected)
        {
            this._controller = _controller;
            this._seekiosSelected = _seekiosSelected;
        }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override nint NumberOfSections(UITableView tableView)
        {
            return (_controller._firstInitialise == true ? 3 : 4);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
                return 1;
            else if (section == 1)
                return 1;
            else if (section == 2)
                return (_controller._firstInitialise == true ? 2 : 3);
            else if (section == 3)
                return 2;
            else
                return 1;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return " test ";
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return 30;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint section)
        {
            if (section == (_controller._firstInitialise == true ? 2 : 3))
            {
                return 30;
            }
            else
            {
                return 0.001f;
            }
        }

        public override UIView GetViewForFooter(UITableView tableView, nint section)
        {
            if (section == (_controller._firstInitialise == true ? 2 : 3))
            {
                var customView = new UIView();
                customView.Frame = new RectangleF(0f, 0f, (float)tableView.Frame.Width, 30f);
                customView.BackgroundColor = UIColor.FromRGB(241, 241, 241);

                var firstlLine = new UILabel();
                firstlLine.Frame = new RectangleF(0, 0f, (float)tableView.Frame.Width, 0.5f);
                firstlLine.BackgroundColor = UIColor.FromRGB(224, 224, 224);

                var lastLine = new UILabel();
                lastLine.Frame = new RectangleF(0, 29f, (float)tableView.Frame.Width, 0.5f);
                lastLine.BackgroundColor = UIColor.FromRGB(224, 224, 224);

                customView.AddSubview(firstlLine);
                customView.AddSubview(lastLine);
                return customView;
            }
            else
            {
                var customView = new UIView();
                customView.Frame = new RectangleF(0f, 0f, 0f, 0f);
                return customView;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var customView = new UIView();
            customView.Frame = new RectangleF(0f, 0f, (float)tableView.Frame.Width, 30f);
            customView.BackgroundColor = UIColor.FromRGB(241, 241, 241);

            var firstlLine = new UILabel();
            firstlLine.Frame = new RectangleF(0, 0f, (float)tableView.Frame.Width, 0.5f);
            firstlLine.BackgroundColor = UIColor.FromRGB(224, 224, 224);

            var lastLine = new UILabel();
            lastLine.Frame = new RectangleF(0, 29f, (float)tableView.Frame.Width, 0.5f);
            lastLine.BackgroundColor = UIColor.FromRGB(224, 224, 224);

            customView.AddSubview(firstlLine);
            customView.AddSubview(lastLine);

            return customView;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 60;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as CustomCellDetailSeekiosView;
            if (cell == null)
            {
                cell = (CustomCellDetailSeekiosView)new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            if (indexPath.Section == 0) // Map View
            {
                SetCell(cell, -1);
                cell.titleNameLabel.Text = Application.LocalizedString("Map");
                cell.detailLabel.Text = Application.LocalizedString("UpdateSeekiosPosition");
                cell.pictureView = UIImage.FromBundle("MapDetail2");
            }
            else if (indexPath.Section == 1) // Refresh Credit
            {
                SetCell(cell, -1);
                cell.titleNameLabel.Text = Application.LocalizedString("RefillCredits");
                cell.detailLabel.Text = string.Format(Application.LocalizedString("CreditsWithCreditsOffered")
                    , SeekiosApp.Helper.CreditHelper.TotalCredits
                    , SeekiosApp.Helper.CreditHelper.CreditsOffered);
                cell.pictureView = UIImage.FromBundle("IconAddCredits");
            }
            else if (indexPath.Section == 2) // Mode or Parameter Selection
            {
                if (_controller._firstInitialise == true) // Parameter
                {
                    SetParameterRow(cell, indexPath);
                }
                else
                {  // Mode
                    SetModeRow(cell, indexPath);
                }
            }
            else if (indexPath.Section == 3) // Parameter
            {
                SetParameterRow(cell, indexPath);
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as CustomCellDetailSeekiosView;
            if (indexPath.Section == 0)
            {
                App.Locator.DetailSeekios.GoToMap(_seekiosSelected);
            }
            else if (indexPath.Section == 1)
            {
                App.Locator.DetailSeekios.GoToBuyCredits();
            }
            else if (indexPath.Section == 2)
            {
                if (_controller._firstInitialise == true)
                {
                    if (indexPath.Row == 0)
                        App.Locator.DetailSeekios.GoToAlertSOS();
                    else if (indexPath.Row == 1)
                        App.Locator.DetailSeekios.GoToParameter(_seekiosSelected);
                }
                else
                {
                    if (indexPath.Row == 0)
                        _controller.SelectMode((int)ModeDefinitionEnum.ModeTracking);
                    else if (indexPath.Row == 1)
                        _controller.SelectMode((int)ModeDefinitionEnum.ModeDontMove);
                    else
                        _controller.SelectMode((int)ModeDefinitionEnum.ModeZone);
                }
            }
            else if (indexPath.Section == 3)
            {
                if (indexPath.Row == 0)
                    App.Locator.DetailSeekios.GoToAlertSOS();
                else if (indexPath.Row == 1)
                    App.Locator.DetailSeekios.GoToParameter(_seekiosSelected);
            }
            tableView.DeselectRow(indexPath, true);
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void SetCell(CustomCellDetailSeekiosView cell, int modeValue)
        {
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);

            if (mode != null && mode.ModeDefinition_idmodeDefinition == modeValue)
            {
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                cell.Buttondelete.Hidden = false;
                cell.buttonDeleteZoneForTouch.Hidden = false;
                if (mode.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS)
                {
                    cell.titleNameLabel.TextColor = UIColor.FromRGB(255, 76, 57);
                }
                else cell.titleNameLabel.TextColor = UIColor.FromRGB(98, 218, 115);
                cell.buttonDeleteZoneForTouch.TouchUpInside += async (s, e) =>
                {
                    if (await _controller.DeleteMode(mode))
                    {
                        cell.Buttondelete.Hidden = true;
                        cell.buttonDeleteZoneForTouch.Hidden = true;
                        cell.titleNameLabel.TextColor = UIColor.FromRGB(102, 102, 102);
                    }
                };
            }
            else
            {
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                cell.Buttondelete.Hidden = true;
                cell.buttonDeleteZoneForTouch.Hidden = true;
                cell.titleNameLabel.TextColor = UIColor.FromRGB(102, 102, 102);
                cell.detailLabel.TextColor = UIColor.FromRGB(187, 187, 187);
            }
        }

        private void SetModeRow(CustomCellDetailSeekiosView cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                SetCell(cell, (int)Enum.ModeDefinitionEnum.ModeTracking);
                cell.titleNameLabel.Text = Application.LocalizedString("ModeTracking");
                cell.detailLabel.Text = Application.LocalizedString("TrackingDescr");
                cell.pictureView = UIImage.FromBundle("ModeTracking");
            }
            else if (indexPath.Row == 1)
            {
                SetCell(cell, (int)Enum.ModeDefinitionEnum.ModeDontMove);
                cell.titleNameLabel.Text = Application.LocalizedString("ModeDontMove");
                cell.detailLabel.Text = Application.LocalizedString("DontMoveDescr");
                cell.pictureView = UIImage.FromBundle("ModeDontMove");
            }
            else if (indexPath.Row == 2)
            {
                SetCell(cell, (int)Enum.ModeDefinitionEnum.ModeZone);
                cell.titleNameLabel.Text = Application.LocalizedString("ModeZone");
                cell.detailLabel.Text = Application.LocalizedString("ZoneDescr");
                cell.pictureView = UIImage.FromBundle("ModeZone");
            }
        }

        private void SetParameterRow(CustomCellDetailSeekiosView cell, NSIndexPath indexPath)
        {
            if (indexPath.Row == 0)
            {
                SetCell(cell, -1);
                cell.titleNameLabel.Text = Application.LocalizedString("SOSAlert");
                cell.detailLabel.Text = Application.LocalizedString("ConfigureEmail");
                cell.pictureView = UIImage.FromBundle("AlertSOS");
            }
            else if (indexPath.Row == 1)
            {
                SetCell(cell, -1);
                cell.titleNameLabel.Text = Application.LocalizedString("Parameters");
                cell.detailLabel.Text = Application.LocalizedString("ModifyNameImage");
                cell.pictureView = UIImage.FromBundle("Settings");
            }
        }

        #endregion
    }
}