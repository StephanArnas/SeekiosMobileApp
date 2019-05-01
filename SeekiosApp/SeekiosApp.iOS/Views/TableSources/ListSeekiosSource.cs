using System;
using System.Collections.Generic;
using System.Linq;
using SeekiosApp.Extension;
using SeekiosApp.iOS.Helper;
using UIKit;
using SeekiosApp.Model.DTO;
using SeekiosApp.Enum;
using Foundation;

namespace SeekiosApp.iOS.Classes
{
    public class ListSeekiosSource : UITableViewDataSource
    {
        #region ===== Attributs ===================================================================

        private List<SeekiosDTO> _mySeekios = null;
        private List<SeekiosDTO> _sharedSeekios = null;
        private ListSeekiosView _controller = null;

        #endregion

        #region ===== Properties ==================================================================

        public string CellID
        {
            get { return "CustomCellSeekios"; }
        }

        #endregion

        #region ===== Constructos =================================================================

        public ListSeekiosSource()
        {
            _mySeekios = new List<SeekiosDTO>();
            _sharedSeekios = new List<SeekiosDTO>();
            Initialize();
        }

        public ListSeekiosSource(ListSeekiosView Controller) : base()
        {
            _controller = Controller;
            _mySeekios = new List<SeekiosDTO>();
            _sharedSeekios = new List<SeekiosDTO>();
            Initialize();
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private void Initialize()
        {
            var listSeekiosLocal = App.Locator.ListSeekios.LsSeekios;
            var userIdLocal = App.CurrentUserEnvironment.User.IdUser;

            for (int i = 0; i < listSeekiosLocal.Count; i++)
            {
                if (userIdLocal == listSeekiosLocal[i].User_iduser)
                {
                    _mySeekios.Add(listSeekiosLocal[i]);
                }
                else
                {
                    _sharedSeekios.Add(listSeekiosLocal[i]);
                }
            }
        }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override nint NumberOfSections(UITableView tableView)
        {
            // Hard coded 1 section
            return 2;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            if (section == 0) return string.Empty;//"Mes Seekios";
            else return string.Empty;// "Seekios partagés avec moi";
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            if (section == 0) return _mySeekios.Count;
            else return _sharedSeekios.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellID, indexPath) as CustomCellSeekios;
            if (indexPath.Section == 0)
            {
                var item = _mySeekios[indexPath.Row];
                //cell.LastPositionLabel.AdjustsFontSizeToFitWidth = true;
                //cell.LastPositionLabel.SizeToFit();

                // seekios name
                cell.seekiosNameLabel = item.SeekiosName;

                // seekios picture
                cell.SetCircularSeekiosImage();
                if (string.IsNullOrEmpty(item.SeekiosPicture))
                {
                    cell.seekiosImage.Image = UIImage.FromBundle("DefaultSeekios");
                }
                else
                {
                    using (var dataDecoded = new NSData(item.SeekiosPicture
                    , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                    {
                        cell.seekiosImage.Image = new UIImage(dataDecoded);
                    }
                }

                // Need update
                if (App.CurrentUserEnvironment.LastVersionEmbedded != null 
                    && item.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded
                    && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
                {
                    if (item.IsInPowerSaving)
                    {
                        cell.MoveUpdateButton();
                    }
                    cell.NeedUpdateButtonHidden = false;
                    cell.TouchDown -= Cell_TouchDown;
                    cell.TouchDown += Cell_TouchDown;
                }

                // Display or not power saving picture
                if (item.IsInPowerSaving) cell.PowerSavingImageHidden = false;
                else cell.PowerSavingImageHidden = true;

                // Last location
                if (item.LastKnownLocation_latitude == App.DefaultLatitude
                    && item.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    // Display statement to say there is no position
                    cell.lastPositionLabel = Application.LocalizedString("FirstPosition");
                }
                else
                {
                    if (item.LastKnownLocation_dateLocationCreation.HasValue && item.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                    {
                        if (item.IsOnDemand)
                        {
                            // Display on refresh state (OnDemand)
                            var textToDisplay = Application.LocalizedString("RefreshPosition");
                            var _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == item.Idseekios);
                            if (_seekiosOnDemand != null)
                            {
                                int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                cell.lastPositionLabel = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                                _seekiosOnDemand.Timer.UpdateUI = () =>
                                {
                                    minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                    seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                    cell.lastPositionLabel = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                                };
                                // Hidden the count down, specific UI when it's the first location
                                _seekiosOnDemand.Timer.Stopped = () =>
                                {
                                    cell.lastPositionLabel = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == item.UIdSeekios).LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                                };
                            }
                            else cell.lastPositionLabel = Application.LocalizedString("RefreshPosition");
                        }
                        else
                        {
                            // Display date of the last position
                            cell.lastPositionLabel = item.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                        }
                    }
                    else
                    {
                        // Display no position
                        cell.lastPositionLabel = Application.LocalizedString("NoPosition");
                    }
                }

                var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == item.Idseekios).FirstOrDefault();
                IEnumerable<AlertDTO> alerts = null;

                cell.AlertImageHidden = true;
                cell.AlertLabelHidden = true;
                cell.ModeImageHidden = true;
                cell.ModeLabelHidden = true;
                cell.ModeLabelColor = UIColor.FromRGB(153, 153, 153);
                cell.AlertLabelColor = UIColor.FromRGB(153, 153, 153);
                cell.SeekiosNameLabelColor = UIColor.FromRGB(98, 218, 115);
                cell.LastPositionLabelColor = UIColor.FromRGB(153, 153, 153);

                if (mode != null)
                {
                    alerts = App.CurrentUserEnvironment.LsAlert.Where(el => el.IdMode == mode.Idmode);
                    // Mode has been request by the user, we are waiting the answer of the seekios
                    if (!item.HasGetLastInstruction
                        && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == item.Idseekios)
                        && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                        && !item.IsRefreshingBattery
                        || !mode.DateModeActivation.HasValue)
                    {
                        cell.ModeImageHidden = false;
                        cell.ModeLabelHidden = false;
                        cell.alertImage = null;
                        cell.alertLabel = string.Empty;

                        if (item.IsInPowerSaving)
                        {
                            if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                            {
                                cell.modeImage = UIImage.FromBundle("ModeTracking").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                            }
                            if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                            {
                                cell.modeImage = UIImage.FromBundle("ModeDontMove").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                            }
                            else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                            {
                                cell.modeImage = UIImage.FromBundle("ModeZone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                            }
                            cell.modeTinColor = UIColor.FromRGB(200, 200, 200);
                            cell.modeLabel = string.Format(Application.LocalizedString("NextNoon"), SeekiosApp.Helper.DateHelper.TimeLeftUntilNextNoon());
                        }
                        else
                        {
                            cell.modeImage = UIImage.FromBundle("InProgress");
                            cell.modeLabel = Application.LocalizedString("InProgress");
                        }
                    }
                    // The seekios has moved or is out of the area
                    else if (mode.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS)
                    {
                        cell.ModeImageHidden = false;
                        cell.ModeLabelHidden = false;

                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved)
                        {
                            cell.lastPositionLabel = Application.LocalizedString("SeekiosMoved");
                            cell.modeImage = UIImage.FromBundle("ModeDontMoveAlert");
                            cell.modeLabel = Application.LocalizedString("DontMove");
                        }
                        else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone)
                        {
                            cell.lastPositionLabel = Application.LocalizedString("SeekiosOutOfZone");
                            cell.modeImage = UIImage.FromBundle("ModeZoneAlert");
                            cell.modeLabel = Application.LocalizedString("Zone");
                        }

                        cell.ModeLabelColor = UIColor.FromRGB(255, 76, 57);
                        cell.SeekiosNameLabelColor = UIColor.FromRGB(255, 76, 57);
                        cell.LastPositionLabelColor = UIColor.FromRGB(255, 76, 57);
                        cell.alertImage = UIImage.FromBundle("AlarmAlert");
                    }
                    // seekios is in a mode
                    else
                    {
                        cell.ModeImageHidden = false;
                        cell.ModeLabelHidden = false;

                        switch (mode.ModeDefinition_idmodeDefinition)
                        {
                            case (int)ModeDefinitionEnum.ModeTracking:
                                cell.modeImage = UIImage.FromBundle("ModeTracking");
                                cell.modeLabel = Application.LocalizedString("Tracking");
                                break;
                            case (int)ModeDefinitionEnum.ModeDontMove:
                                cell.modeImage = UIImage.FromBundle("ModeDontMove");
                                cell.modeLabel = Application.LocalizedString("Don't Move");
                                break;
                            case (int)ModeDefinitionEnum.ModeZone:
                                cell.modeImage = UIImage.FromBundle("ModeZone");
                                cell.modeLabel = Application.LocalizedString("Zone");
                                break;
                        }
                    }
                }
                if (!item.IsLastSOSRead)
                {
                    if (cell.ModeImageHidden == false && cell.ModeLabelHidden == false)
                    {
                        cell.AlertImageHidden = false;
                        cell.AlertLabelHidden = false;
                        cell.alertLabel = Application.LocalizedString("SOSAlert");
                        cell.AlertLabelColor = UIColor.FromRGB(255, 76, 56);
                        cell.alertImage = UIImage.FromBundle("AlertSOSAlert");
                    }
                    else
                    {
                        cell.ModeImageHidden = false;
                        cell.ModeLabelHidden = false;
                        cell.modeLabel = Application.LocalizedString("SOSAlert"); ;
                        cell.ModeLabelColor = UIColor.FromRGB(255, 76, 56);
                        cell.modeImage = UIImage.FromBundle("AlertSOSAlert");
                    }
                    cell.lastPositionLabel = item.DateLastSOSSent != null ? item.DateLastSOSSent.Value.FormatDateFromNowAlertSOS() : string.Empty;
                }
                else if (item.BatteryLife <= 20)
                {
                    if (item.LastKnownLocation_latitude == App.DefaultLatitude
                        && item.LastKnownLocation_longitude == App.DefaultLongitude)
                    {
                        cell.ModeImageHidden = true;
                        cell.ModeLabelHidden = true;
                        cell.AlertImageHidden = true;
                        cell.AlertLabelHidden = true;
                    }
                    else
                    {
                        if (cell.ModeImageHidden == false && cell.ModeLabelHidden == false)
                        {
                            cell.AlertImageHidden = false;
                            cell.AlertLabelHidden = false;
                            cell.alertLabel = item.BatteryLife + "%";
                            cell.alertImage = UIImage.FromBundle("CriticalBattery");
                        }
                        else
                        {
                            cell.ModeImageHidden = false;
                            cell.ModeLabelHidden = false;
                            cell.modeLabel = item.BatteryLife + "%";
                            cell.modeImage = UIImage.FromBundle("CriticalBattery");
                        }
                    }
                }
            }
            return cell;
        }

        private void Cell_TouchDown(object sender, EventArgs e)
        {
            var popup = AlertControllerHelper.CreatePopupSeekiosNeedUpdate(() =>
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://seekios.com/Home/UserManual"));
            });
            _controller.PresentViewController(popup, true, null);
        }

        #endregion
    }
}