using System;
using UIKit;
using SeekiosApp.iOS.Helper;
using SeekiosApp.Model.DTO;
using SeekiosApp.Extension;
using System.Linq;
using SeekiosApp.Enum;
using SeekiosApp.iOS.Views;
using SeekiosApp.iOS.Views.TableSources;
using Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeekiosApp.iOS
{
    public partial class DetailSeekiosView : BaseViewController
    {
        #region ===== Properties ==================================================================

        private SeekiosDTO _seekiosSelected = null;
        public bool _firstInitialise = false;

        #endregion

        #region ===== Constructor =================================================================

        public DetailSeekiosView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            App.SeekiosChanged += App_SeekiosChanged;
            App.RemainingRequestChanged += OnRemainingRequestChanged;
            App.Locator.ModeDontMove.SeekiosMovedNotified += ModeDontMove_SeekiosMovedNotified;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += ModeZone_OnSeekiosOutOfZoneNotified;
            RefreshBatteryButton.TouchUpInside += RefreshBatteryButton_TouchUpInside;
            HelpCreditCostButton.TouchDown += HelpCreditCostButton_TouchDown;
            NeedUpdateButton.TouchDown += NeedUpdateButton_TouchDown;
            HeadView.AddGestureRecognizer(new UITapGestureRecognizer(() => { App.Locator.DetailSeekios.GoToMap(_seekiosSelected); }));
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            App.Locator.ModeZone.IsGoingBack = false;
            App.Locator.ModeDontMove.IsGoingBack = false;
        }

        public override void ViewWillAppear(bool animated)
        {
            if (App.Locator.AddSeekios.IsGoingBack)
            {
                App.Locator.AddSeekios.IsGoingBack = false;
                GoBack(false);
                return;
            }
            Title = Application.LocalizedString("DetailSeekiosTitle");
            _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios);
            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                _firstInitialise = true;
            }
            else _firstInitialise = false;
            base.ViewWillAppear(animated);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            UpdateBatteryView();
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            App.SeekiosChanged -= App_SeekiosChanged;
            App.RemainingRequestChanged -= OnRemainingRequestChanged;
            App.Locator.ModeDontMove.SeekiosMovedNotified -= ModeDontMove_SeekiosMovedNotified;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified -= ModeZone_OnSeekiosOutOfZoneNotified;
            RefreshBatteryButton.TouchUpInside -= RefreshBatteryButton_TouchUpInside;
            HelpCreditCostButton.TouchDown -= HelpCreditCostButton_TouchDown;
            NeedUpdateButton.TouchDown -= NeedUpdateButton_TouchDown;

            SeekiosImageView.Dispose();
            CreditsTitleLabel.Dispose();
            NextRefillLabel.Dispose();
            LastRefreshBatteryLabel.Dispose();
            RefreshBatteryButton.Dispose();
            BatteryLabel.Dispose();
            BatteryIndicator.Dispose();
            RefreshBatteryButton.Dispose();
            //Tableview.Dispose();

            _seekiosSelected = null;
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            if (_seekiosSelected == null) return;

            DisplayLastAlertSOS();
            InitialiseAllStrings();

            Tableview.Source = new DetailSeekiosSource(this, _seekiosSelected);
            //Tableview.ReloadData();

            // round corner on the button refresh battery
            RefreshBatteryButton.Layer.CornerRadius = 4;
            RefreshBatteryButton.Layer.MasksToBounds = true;
            App.Locator.ModeZone.IsActivityFocused = false;
            SeekiosImageView.Layer.CornerRadius = SeekiosImageView.Frame.Size.Width / 2;
            SeekiosImageView.ClipsToBounds = true;

            BatteryIndicator.Hidden = true;
            BatteryIndicator.StopAnimating();
            RefreshBatteryButton.Enabled = true;

            RefreshDisplayedCreditCount();
            InitializeSeekiosInformation();
            InitializeModeInformation();
            InitializeAlertSOSAndBatteryCritiqueInformation();
            InitializeBatteryInformation();

            DateReloadSeekiosCredit.Text = SeekiosApp.Helper.CreditHelper.CalculateNextReload();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Select a mode
        /// </summary>
        public void SelectMode(int idModeDefinition)
        {
            App.Locator.ModeSelection.SeekiosUpdated = _seekiosSelected;
            App.Locator.ModeZone.IsActivityFocused = true;
            var modeDefinitionEnum = (ModeDefinitionEnum)System.Enum.Parse(typeof(ModeDefinitionEnum), idModeDefinition.ToString());
            App.Locator.ModeSelection.GoToMode(modeDefinitionEnum);
        }

        /// <summary>
        /// Deletes the mode.
        /// </summary>
        public async Task<bool> DeleteMode(ModeDTO mode)
        {
            App.Locator.ModeSelection.SeekiosUpdated = _seekiosSelected;
            var result = await App.Locator.ModeSelection.DeleteMode(mode);
            if (result)
            {
                _seekiosSelected.HasGetLastInstruction = false;
                if (App.Locator.BaseMap.LsSeekiosAlertState.Contains(_seekiosSelected.Idseekios))
                {
                    App.Locator.BaseMap.LsSeekiosAlertState.Remove(_seekiosSelected.Idseekios);
                }
                SetDataAndStyleToView();
            }
            return result;
        }

        #endregion

        #region ===== Private Methods =============================================================

        private void InitializeSeekiosInformation()
        {
            // Seekios picture
            if (string.IsNullOrEmpty(_seekiosSelected.SeekiosPicture))
            {
                SeekiosImageView.Image = UIImage.FromBundle("DefaultSeekios");
            }
            else
            {
                using (var dataDecoded = new NSData(_seekiosSelected.SeekiosPicture
                    , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    SeekiosImageView.Image = new UIImage(dataDecoded);
                }
            }

            // Seekios name
            SeekiosNameLabel.Text = _seekiosSelected.SeekiosName;

            // Seekios need update ? 
            if (App.CurrentUserEnvironment.LastVersionEmbedded != null
                && _seekiosSelected.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded
                && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
            {
                if (_seekiosSelected.IsInPowerSaving)
                {
                    using (var constraintY = NSLayoutConstraint.Create(NeedUpdateButton
                        , NSLayoutAttribute.CenterY
                        , NSLayoutRelation.Equal
                        , SeekiosImageView
                        , NSLayoutAttribute.CenterY
                        , 1, 3))
                    {
                        constraintY.Active = true;
                        NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { constraintY });
                    }
                }
                NeedUpdateButton.Hidden = false;
            }

            // Display or not power saving picture
            if (_seekiosSelected.IsInPowerSaving) PowerSavingImage.Hidden = false;
            else PowerSavingImage.Hidden = true;

            // Seekios last position
            if (_firstInitialise)
            {
                Tableview.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, false);
                // Display statement to say there is no position
                SeekiosLastPositionLabel.Text = Application.LocalizedString("FirstPosition");
            }
            else
            {
                if (_seekiosSelected.LastKnownLocation_dateLocationCreation.HasValue
                    && _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                {
                    if (_seekiosSelected.IsOnDemand)
                    {
                        // Display on refresh state (OnDemand)
                        var textToDisplay = Application.LocalizedString("RefreshPosition");
                        var _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
                        if (_seekiosOnDemand != null)
                        {
                            int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                            int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                            SeekiosLastPositionLabel.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                            _seekiosOnDemand.Timer.UpdateUI = () =>
                            {
                                minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                SeekiosLastPositionLabel.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                            };
                            // Hidden the count down, specific UI when it's the first location
                            _seekiosOnDemand.Timer.Stopped = () =>
                            {
                                SeekiosLastPositionLabel.Text = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios).LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                            };
                        }
                        else SeekiosLastPositionLabel.Text = Application.LocalizedString("RefreshPosition");
                    }
                    else
                    {
                        // Display date of the last position
                        SeekiosLastPositionLabel.Text = _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                    }
                }
                else
                {
                    // Display no position
                    Tableview.ScrollToRow(NSIndexPath.FromRowSection(0, 0), UITableViewScrollPosition.Top, false);
                    SeekiosLastPositionLabel.Text = Application.LocalizedString("NoPosition");
                }
            }
        }

        private void InitializeModeInformation()
        {
            // Reset display normal
            ModeImage.Hidden = true;
            ModeLabel.Hidden = true;
            AlertImage.Hidden = true;
            AlertLabel.Hidden = true;
            ModeLabel.TextColor = UIColor.FromRGB(153, 153, 153);
            AlertLabel.TextColor = UIColor.FromRGB(153, 153, 153);
            SeekiosNameLabel.TextColor = UIColor.FromRGB(98, 218, 115);
            SeekiosLastPositionLabel.TextColor = UIColor.FromRGB(153, 153, 153);

            // Display the mode
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);
            IEnumerable<AlertDTO> alerts = null;

            if (mode != null)
            {
                alerts = App.CurrentUserEnvironment.LsAlert.Where(el => el.IdMode == mode.Idmode);
                // Mode has been request by the user, we are waiting the answer of the seekios
                if (!_seekiosSelected.HasGetLastInstruction
                    && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios)
                    && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                    && !_seekiosSelected.IsRefreshingBattery
                    || !mode.DateModeActivation.HasValue)
                {
                    ModeImage.Hidden = false;
                    ModeLabel.Hidden = false;
                    AlertImage.Image = null;
                    AlertLabel.Text = string.Empty;

                    if (_seekiosSelected.IsInPowerSaving)
                    {
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeTracking").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        }
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeDontMove").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        }
                        else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeZone").ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                        }
                        ModeImage.TintColor = UIColor.FromRGB(200, 200, 200);
                        ModeLabel.Text = string.Format(Application.LocalizedString("NextNoon"), SeekiosApp.Helper.DateHelper.TimeLeftUntilNextNoon());
                    }
                    else
                    {
                        ModeImage.Image = UIImage.FromBundle("InProgress");
                        ModeLabel.Text = Application.LocalizedString("InProgress");
                    }
                }
                // The seekios has moved or is out of the area
                else if (mode.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS)
                {
                    ModeImage.Hidden = false;
                    ModeLabel.Hidden = false;

                    if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved)
                    {
                        SeekiosLastPositionLabel.Text = Application.LocalizedString("SeekiosMoved");
                        ModeImage.Image = UIImage.FromBundle("ModeDontMoveAlert");
                        ModeLabel.Text = Application.LocalizedString("DontMove");
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone)
                    {
                        SeekiosLastPositionLabel.Text = Application.LocalizedString("SeekiosOutOfZone");
                        ModeImage.Image = UIImage.FromBundle("ModeZoneAlert");
                        ModeLabel.Text = Application.LocalizedString("Zone");
                    }

                    AlertLabel.TextColor = UIColor.FromRGB(255, 76, 57);
                    ModeLabel.TextColor = UIColor.FromRGB(255, 76, 57);
                    SeekiosNameLabel.TextColor = UIColor.FromRGB(255, 76, 57);
                    SeekiosLastPositionLabel.TextColor = UIColor.FromRGB(255, 76, 57);
                    AlertImage.Image = UIImage.FromBundle("AlarmAlert");
                }
                // Seekios is in a mode
                else
                {
                    ModeImage.Hidden = false;
                    ModeLabel.Hidden = false;

                    switch (mode.ModeDefinition_idmodeDefinition)
                    {
                        case (int)ModeDefinitionEnum.ModeTracking:
                            ModeImage.Image = UIImage.FromBundle("ModeTracking");
                            ModeLabel.Text = Application.LocalizedString("Tracking");
                            break;
                        case (int)ModeDefinitionEnum.ModeDontMove:
                            ModeImage.Image = UIImage.FromBundle("ModeDontMove");
                            ModeLabel.Text = Application.LocalizedString("DontMove");
                            break;
                        case (int)ModeDefinitionEnum.ModeZone:
                            ModeImage.Image = UIImage.FromBundle("ModeZone");
                            ModeLabel.Text = Application.LocalizedString("Zone");
                            break;
                    }
                }
            }
        }

        private void InitializeAlertSOSAndBatteryCritiqueInformation()
        {
            if (!_seekiosSelected.IsLastSOSRead)
            {
                if (ModeImage.Hidden == false && ModeLabel.Hidden == false)
                {
                    AlertImage.Hidden = false;
                    AlertLabel.Hidden = false;
                    AlertLabel.Text = Application.LocalizedString("SOSAlert");
                    AlertLabel.TextColor = UIColor.FromRGB(255, 76, 56);
                    AlertImage.Image = UIImage.FromBundle("AlertSOSAlert");
                }
                else
                {
                    ModeImage.Hidden = false;
                    ModeLabel.Hidden = false;
                    ModeLabel.Text = Application.LocalizedString("SOSAlert");
                    ModeLabel.TextColor = UIColor.FromRGB(255, 76, 56);
                    ModeImage.Image = UIImage.FromBundle("AlertSOSAlert");
                }
                SeekiosLastPositionLabel.Text = _seekiosSelected.DateLastSOSSent.HasValue ? _seekiosSelected.DateLastSOSSent.Value.FormatDateFromNowAlertSOS() : string.Empty;
            }
            else if (_seekiosSelected.BatteryLife <= 20)
            {
                if (_firstInitialise)
                {
                    ModeImage.Hidden = true;
                    ModeLabel.Hidden = true;
                    AlertImage.Hidden = true;
                    AlertLabel.Hidden = true;
                }
                else
                {
                    if (ModeImage.Hidden == false && ModeLabel.Hidden == false)
                    {
                        AlertImage.Hidden = false;
                        AlertLabel.Hidden = false;
                        AlertLabel.Text = _seekiosSelected.BatteryLife + "%";
                        AlertImage.Image = UIImage.FromBundle("CriticalBattery");
                    }
                    else
                    {
                        ModeImage.Hidden = false;
                        ModeLabel.Hidden = false;
                        ModeLabel.Text = _seekiosSelected.BatteryLife + "%";
                        ModeImage.Image = UIImage.FromBundle("CriticalBattery");
                    }
                }
            }
        }

        private void InitializeBatteryInformation()
        {
            // First time ? Need to refresh the position
            if (_firstInitialise)
            {
                BatteryBackgroundImage.Hidden = true;
                LastRefreshBatteryLabel.Text = Application.LocalizedString("UpdatePosition");
                RefreshBatteryButton.Enabled = false;
            }
            else
            {
                RefreshBatteryButton.Enabled = true;
                // Make the indicator loading
                if (_seekiosSelected.IsRefreshingBattery)
                {
                    BatteryIndicator.Hidden = false;
                    BatteryIndicator.StartAnimating();
                    RefreshBatteryButton.Enabled = false;
                }
                // Display the value
                if (_seekiosSelected.DateLastCommunication.HasValue)
                {
                    LastRefreshBatteryLabel.Text = string.Format(Application.LocalizedString("BatteryLevel")
                        , _seekiosSelected.BatteryLife
                        , _seekiosSelected.DateLastCommunication.Value.FormatDateTimeFromNow(false));
                    BatteryBackgroundImage.Hidden = false;
                }
                // No information about the battery
                else
                {
                    BatteryBackgroundImage.Hidden = true;
                    LastRefreshBatteryLabel.Text = Application.LocalizedString("NoInformation");
                }
            }
        }

        private void UpdateBatteryView()
        {
            if (_seekiosSelected.BatteryLife > 95)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery100");
            }
            else if (_seekiosSelected.BatteryLife > 85)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery90");
            }
            else if (_seekiosSelected.BatteryLife > 75)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery80");
            }
            else if (_seekiosSelected.BatteryLife > 65)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery70");
            }
            else if (_seekiosSelected.BatteryLife > 55)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery60");
            }
            else if (_seekiosSelected.BatteryLife > 45)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery50");
            }
            else if (_seekiosSelected.BatteryLife > 35)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery40");
            }
            else if (_seekiosSelected.BatteryLife > 25)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery30");
            }
            else if (_seekiosSelected.BatteryLife > 15)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery20");
            }
            else if (_seekiosSelected.BatteryLife > 5)
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery10");
            }
            else
            {
                BatteryBackgroundImage.Image = UIImage.FromBundle("Battery0");
            }
            BatteryBackgroundImage.SetNeedsDisplay();
        }

        private void DisplayLastAlertSOS()
        {
            if (App.Locator.DetailSeekios.AlertNeedsToBeRead())
            {
                var popup = AlertControllerHelper.CreateAlertToInformAlertSOS(async () =>
                {
                    if (await App.Locator.DetailSeekios.NotifyAlertSOSHasBeenRead(_seekiosSelected.Idseekios) <= 0)
                    {
                        await AlertControllerHelper.ShowAlert(string.Empty
                            , Application.LocalizedString("AlertCannotBeSavedAsRead")
                            , Application.LocalizedString("Close"));
                    }
                    else
                    {
                        _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios);
                        SetDataAndStyleToView();
                        UpdateBatteryView();
                    }
                });
                PresentViewController(popup, true, () => { popup.Dispose(); });
            }
        }

        private void InitialiseAllStrings()
        {
            CreditsTitleLabel.Text = Application.LocalizedString("Credits");
            NextRefillLabel.Text = Application.LocalizedString("NextRefill");
            LastRefreshBatteryLabel.Text = Application.LocalizedString("Battery");
            RefreshBatteryButton.SetTitle(Application.LocalizedString("Update"), UIControlState.Normal);
            BatteryLabel.Text = Application.LocalizedString("Battery");
        }

        #endregion

        #region ===== Credits Seekios =============================================================

        private void OnRemainingRequestChanged(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                if (App.Locator.BaseMap == null /*|| App.Locator.BaseMap.RemaningRequest == null*/) return;//ca en principe ca se peut pas, mais on sait jamais
                RefreshDisplayedCreditCount();
            });
        }

        public void RefreshDisplayedCreditCount()
        {
            if (_seekiosSelected == null) return;
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == _seekiosSelected.UIdSeekios);

            // If there is any free credit on the seekios we display freeCredit
            if (CreditsLabel != null)
            {
                CreditsLabel.Text = SeekiosApp.Helper.CreditHelper.TotalCredits;
                var firstAttributes = new UIStringAttributes
                {
                    ForegroundColor = UIColor.FromRGB(98, 218, 115)
                };
                var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(Application.LocalizedString("CreditOfferedAmount"), SeekiosApp.Helper.CreditHelper.CreditsOffered);
                var offeredCreditsText = new NSMutableAttributedString(string.Format(Application.LocalizedString("CreditOfferedAmount")
                    , SeekiosApp.Helper.CreditHelper.CreditsOffered));
                offeredCreditsText.SetAttributes(firstAttributes.Dictionary
                    , new NSRange(resultTuple.Item1, SeekiosApp.Helper.CreditHelper.CreditsOffered.Length));
                CreditsFreeLabel.AttributedText = offeredCreditsText;
            }

            if (Tableview != null)
            {
                Tableview.BeginUpdates();
                Tableview.ReloadSections(Foundation.NSIndexSet.FromIndex(1), UITableViewRowAnimation.None);
                Tableview.EndUpdates();
            }
        }

        #endregion

        #region ===== Events ======================================================================

        private void App_SeekiosChanged(object sender, int e)
        {
            InvokeOnMainThread(() =>
            {
                _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios);

                if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                    && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    _firstInitialise = true;
                }
                else _firstInitialise = false;

                if (_firstInitialise)
                {
                    _firstInitialise = false;
                    Tableview.Source = new DetailSeekiosSource(this, _seekiosSelected);
                    Tableview.ReloadData();
                    if (_seekiosSelected.LastKnownLocation_dateLocationCreation.HasValue
                            && _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                    {
                        SeekiosLastPositionLabel.Text = _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                    }
                    else
                    {
                        Tableview.ScrollToRow(NSIndexPath.FromRowSection(0, 0)
                            , UITableViewScrollPosition.Top, false);
                        SeekiosLastPositionLabel.Text = Application.LocalizedString("NoPosition");
                    }
                }
                SetDataAndStyleToView();
                UpdateBatteryView();
            });
        }

        private async void RefreshBatteryButton_TouchUpInside(object sender, EventArgs e)
        {
            if (await App.Locator.DetailSeekios.RequestBatteryLevel())
            {
                // Update UI with a loader to show the battery is refreshing
                BatteryIndicator.Hidden = false;
                BatteryIndicator.StartAnimating();
                RefreshBatteryButton.Enabled = false;
                _seekiosSelected.IsRefreshingBattery = true;
            }
        }

        private void HelpCreditCostButton_TouchDown(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialCreditCost();
        }

        private void ModeZone_OnSeekiosOutOfZoneNotified(int idSeekios, double lat, double lon, double altitude, double accuracy, DateTime dateCommunication)
        {
            SetDataAndStyleToView();
        }

        private void ModeDontMove_SeekiosMovedNotified(int idSeekios)
        {
            SetDataAndStyleToView();
        }

        private void NeedUpdateButton_TouchDown(object sender, EventArgs e)
        {
            var popup = AlertControllerHelper.CreatePopupSeekiosNeedUpdate(() =>
            {
                UIApplication.SharedApplication.OpenUrl(new NSUrl("https://seekios.com/Home/UserManual"));
            });
            PresentViewController(popup, true, null);
        }

        #endregion
    }
}