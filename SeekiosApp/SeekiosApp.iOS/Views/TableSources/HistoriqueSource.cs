using System;
using System.Collections.Generic;
using System.Linq;
using SeekiosApp.Enum;
using SeekiosApp.Enum.FromDataBase;
using SeekiosApp.iOS.Helper;
using SeekiosApp.Model.DTO;
using UIKit;

namespace SeekiosApp.iOS
{
    public class HistoriqueSource : UITableViewSource
    {
        #region ===== Attributs ===================================================================

        private List<OperationDTO> _historyData = null;
        private TransactionHistoricView _controller = null;
        private OperationDTO objDTO = null;

        #endregion

        #region ===== Properties ==================================================================

        private string _cellIdentifier = "tableView";

        string transactionHistoric_date = Application.LocalizedString("transactionHistoric_date");
        string transactionHistoric_monthlyCredits = Application.LocalizedString("transactionHistoric_monthlyCredits");
        string transactionHistoric_tracking = Application.LocalizedString("transactionHistoric_tracking");
        string transactionHistoric_zoneTracking = Application.LocalizedString("transactionHistoric_zoneTracking");
        string transactionHistoric_zone = Application.LocalizedString("transactionHistoric_zone");
        string transactionHistoric_dontmove = Application.LocalizedString("transactionHistoric_dontmove");
        string transactionHistoric_dontMoveTracking = Application.LocalizedString("transactionHistoric_dontMoveTracking");
        string transactionHistoric_refreshPosition = Application.LocalizedString("transactionHistoric_refreshPosition");
        string transactionHistoric_refreshBattery = Application.LocalizedString("transactionHistoric_refreshBattery");
        string transactionHistoric_modeInSettings = Application.LocalizedString("transactionHistoric_modeInSettings");
        string transactionHistoric_sos = Application.LocalizedString("transactionHistoric_sos");
        string transaction_epicPack = Application.LocalizedString("transaction_epicPack");
        string transaction_adventurePack = Application.LocalizedString("transaction_adventurePack");
        string transaction_explorationPack = Application.LocalizedString("transaction_explorationPack");
        string transaction_observationPack = Application.LocalizedString("transaction_observationPack");
        string transaction_discoveryPack = Application.LocalizedString("transaction_discoveryPack");
        string transaction_instantPack = Application.LocalizedString("transaction_instantPack");
        string transaction_subscriptionPack = Application.LocalizedString("transaction_subscriptionPack");
        string transactionHistoric_deletedSeekios = Application.LocalizedString("transactionHistoric_deletedSeekios");

        #endregion

        #region ===== Constructor =================================================================

        public HistoriqueSource(TransactionHistoricView controller)
        {
            _controller = controller;
            _historyData = new List<OperationDTO>();
            _historyData = App.Locator.TransactionHistoric.LsOperation;
        }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return this._historyData.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            return 75;
        }

        public override UITableViewCell GetCell(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as CustomCellHistoriqueView;
            if (cell == null)
            {
                cell = (CustomCellHistoriqueView)new UITableViewCell(UITableViewCellStyle.Default, _cellIdentifier);
            }
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            objDTO = _historyData[indexPath.Row];

            //Init date
            var date = objDTO.DateEnd.Value.ToLocalTime();
            var time = string.Format("{0}h{1:00}", date.Hour, date.Minute);
            cell.dateTimeLabel = string.Format(transactionHistoric_date, date.ToShortDateString(), time);

            //Seekios Name
            if (objDTO.IdSeekios != null)
            {
                if (App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == objDTO.IdSeekios) != null)
                    cell.seekiosNameLabel = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == objDTO.IdSeekios).SeekiosName;
                else cell.seekiosNameLabel = transactionHistoric_deletedSeekios;
            }

            //Credit amount + or -
            cell.numberOfElementsLabel = objDTO.CreditAmount.ToString();
            if (objDTO.CreditAmount >= 0)
            {
                cell.ElementsLabel.TextColor = UIColor.FromRGB(98, 218, 115);
                cell.numberOfElementsLabel = string.Format("+{0}", objDTO.CreditAmount.ToString());
            }

            //Initialize components with respect to the operationType
            switch (objDTO.OperationType)
            {
                default:
                    break;
                case (int)OperationTypeEnum.ConfigureMode:
                    var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Idmode == objDTO.IdMode);
                    if (mode == null)
                    {
                        cell.titleNameLabel = transactionHistoric_modeInSettings;
                        cell.historiqueImageView.Image = UIImage.FromBundle("InProgress");
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                    {
                        cell.titleNameLabel = transactionHistoric_zone;
                        cell.historiqueImageView.Image = UIImage.FromBundle("ModeZone");
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                    {
                        cell.titleNameLabel = transactionHistoric_dontmove;
                        cell.historiqueImageView.Image = UIImage.FromBundle("ModeDontMove");
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                    {
                        cell.titleNameLabel = transactionHistoric_tracking;
                        cell.historiqueImageView.Image = UIImage.FromBundle("ModeTracking");
                    }
                    break;
                case (int)OperationTypeEnum.RefreshBattery:
                    cell.titleNameLabel = transactionHistoric_refreshBattery;
                    cell.historiqueImageView.Image = UIImage.FromBundle("Battery70");
                    break;
                case (int)OperationTypeEnum.RefreshPosition:
                    cell.titleNameLabel = transactionHistoric_refreshPosition;
                    cell.historiqueImageView.Image = UIImage.FromBundle("RefreshSeekios");
                    break;
                case (int)OperationTypeEnum.SeekiosMonthlyGift:
                    cell.titleNameLabel = transactionHistoric_monthlyCredits;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.SendSOS:
                    cell.titleNameLabel = transactionHistoric_sos;
                    cell.historiqueImageView.Image = UIImage.FromBundle("AlertSOS");
                    break;
                case (int)OperationTypeEnum.AddDontMoveTrackingPosition:
                    cell.titleNameLabel = transactionHistoric_dontMoveTracking;
                    cell.historiqueImageView.Image = UIImage.FromBundle("ModeDontMove");
                    break;
                case (int)OperationTypeEnum.AddTrackingPosition:
                    cell.titleNameLabel = transactionHistoric_tracking;
                    cell.historiqueImageView.Image = UIImage.FromBundle("ModeTracking");
                    break;
                case (int)OperationTypeEnum.AddZoneTrackingPosition:
                    cell.titleNameLabel = transactionHistoric_zoneTracking;
                    cell.historiqueImageView.Image = UIImage.FromBundle("ModeZone");
                    break;
                case (int)OperationTypeEnum.BoughtCredits1:
                    cell.titleNameLabel = transaction_observationPack;
                    cell.seekiosNameLabel = transaction_instantPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits2:
                    cell.titleNameLabel = transaction_discoveryPack;
                    cell.seekiosNameLabel = transaction_instantPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits3:
                    cell.titleNameLabel = transaction_explorationPack;
                    cell.seekiosNameLabel = transaction_instantPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits4:
                    cell.titleNameLabel = transaction_adventurePack;
                    cell.seekiosNameLabel = transaction_instantPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits5:
                    cell.titleNameLabel = transaction_epicPack;
                    cell.seekiosNameLabel = transaction_instantPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits1Subscription:
                    cell.titleNameLabel = transaction_instantPack;
                    cell.seekiosNameLabel = transaction_subscriptionPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits2Subscription:
                    cell.titleNameLabel = transaction_discoveryPack;
                    cell.seekiosNameLabel = transaction_subscriptionPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits3Subscription:
                    cell.titleNameLabel = transaction_explorationPack;
                    cell.seekiosNameLabel = transaction_subscriptionPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits4Subscription:
                    cell.titleNameLabel = transaction_adventurePack;
                    cell.seekiosNameLabel = transaction_subscriptionPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
                case (int)OperationTypeEnum.BoughtCredits5Subscription:
                    cell.titleNameLabel = transaction_epicPack;
                    cell.seekiosNameLabel = transaction_subscriptionPack;
                    cell.historiqueImageView.Image = UIImage.FromBundle("IconCredit");
                    break;
            }
            return cell;
        }

        #endregion
    }
}

