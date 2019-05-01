using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using SeekiosApp.Droid.Services;
using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using System;
using System.Linq;
using XamSvg;
using SeekiosApp.Extension;
using SeekiosApp.Helper;
using SeekiosApp.Droid.Helper;
using Android.Text;
using Android.Text.Style;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class DetailSeekiosActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private SeekiosDTO _seekiosSelected;

        #endregion

        #region ===== Properties ==================================================================

        #region SEEKIOS LAYOUT

        /// <summary>Seekios name</summary>
        public RelativeLayout SeekiosInformationLayout { get; set; }

        /// <summary>Seekios name</summary>
        public TextView SeekiosNameTextView { get; set; }

        /// <summary>Seekios image</summary>
        public RoundedImageView SeekiosRoundedImageView { get; set; }

        /// <summary> Power saving enabled image</summary>
        public SvgImageView PowerSavingImageView { get; set; }

        /// <summary> Firmware update image view </summary>
        public XamSvg.SvgImageView FirmwareUpdateSvgImageView { get; set; }

        /// <summary>Last location</summary>
        public TextView LastLocationTextView { get; set; }

        /// <summary>Image for the mode activated</summary>
        public SvgImageView ModeSvgImageView { get; set; }

        /// <summary>Mode title</summary>
        public TextView ModeTitleTextView { get; set; }

        /// <summary>Alert text</summary>
        public TextView AlertTextView { get; set; }

        public SvgImageView AlertSvgImageView { get; set; }
        public LinearLayout ModeLayout { get; set; }
        public LinearLayout AlertSeekiosLayout { get; set; }
        public RelativeLayout ButtonRightLayout { get; set; }

        #endregion

        #region BATTERY LAYOUT

        /// <summary>Battery image</summary>
        public SvgImageView BatterySvgImageView { get; set; }

        /// <summary>Battery text info</summary>
        public TextView BatteryTextView { get; set; }

        /// <summary>Battery refresh button</summary>
        public TextView BatteryRefreshButton { get; set; }

        /// <summary>Battery loader Progress Bar</summary>
        public ProgressBar BatteryLoaderProgressBar { get; set; }

        #endregion

        #region CREDITS LAYOUT

        /// <summary>Number of total credit</summary>
        public TextView CreditsTextView { get; set; }

        /// <summary>Number of free credit</summary>
        public TextView FreeCreditTextView { get; set; }

        /// <summary>Next reload date for free credit</summary>
        public TextView FreeCreditReloadDateTextView { get; set; }

        /// <summary>Credit Layout</summary>
        public LinearLayout CreditLayout { get; set; }

        #endregion

        #region MAP MODE SHARE PARAM

        /// <summary>Map LinearLayout handling click</summary>
        public LinearLayout MapLinearLayout { get; set; }

        /// <summary>Mode LinearLayout handling click</summary>
        public LinearLayout ModeLinearLayout { get; set; }

        public LinearLayout SOSLinearLayout { get; set; }

        /// <summary>Parameter LinearLayout handling click</summary>
        public LinearLayout ParameterLinearLayout { get; set; }

        /// <summary>Mode layout image<summary>
        public SvgImageView ModeLayoutSvgImageView { get; set; }

        /// <summary>Mode layout text<summary>
        public TextView ModeLayoutTextView { get; set; }

        /// <summary>Mode layout image<summary>
        public SvgImageView ParameterLayoutSvgImageView { get; set; }

        /// <summary>Mode layout text<summary>
        public TextView ParameterLayoutTextView { get; set; }

        #endregion

        #region DELETE SEEKIOS AND INFORMATION

        /// <summary>Mode info image<summary>
        public SvgImageView ModeInformationSvgImageView { get; set; }

        /// <summary>Mode info layout</summary>
        public LinearLayout ModeInformationLayout { get; set; }

        /// <summary>Mode info text</summary>
        public TextView ModeInformationTextView { get; set; }

        /// <summary>Alert info text</summary>
        public TextView AlertInformationTextView { get; set; }

        /// <summary>Tracking info text</summary>
        public TextView TrackingInformationTextView { get; set; }

        #endregion

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.DetailSeekiosLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();

            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.detailSeekios_pageTitle), _seekiosSelected != null ? _seekiosSelected.SeekiosName : string.Empty);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            if (App.Locator.DetailSeekios.IsSeekiosDeleted)
            {
                App.Locator.DetailSeekios.IsSeekiosDeleted = false;
                Finish();
            }
            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
            base.OnResume();
            SetDataToView();
            DisplayLastAlertSOS();

            if (App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated)
            {
                SetDataToView();
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = false;
            }

            App.RemainingRequestChanged += OnRemainingRequestChanged;
            App.SeekiosChanged += App_SeekiosChanged;
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            MapLinearLayout.Click += OnMapLayoutClick;
            ModeLinearLayout.Click += OnModeLayoutClick;
            BatteryRefreshButton.Click += OnBatteryRefreshButtonClick;
            SOSLinearLayout.Click += OnAlertSOSLayoutClick;
            ParameterLinearLayout.Click += OnParameterLayoutClick;
            SeekiosInformationLayout.Click += OnMapLayoutClick;
            FirmwareUpdateSvgImageView.Click += FirmwareUpdateImageView_Click;
            CreditLayout.Click += CreditLayout_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();

            App.RemainingRequestChanged -= OnRemainingRequestChanged;
            App.SeekiosChanged -= App_SeekiosChanged;
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            MapLinearLayout.Click -= OnMapLayoutClick;
            ModeLinearLayout.Click -= OnModeLayoutClick;
            BatteryRefreshButton.Click -= OnBatteryRefreshButtonClick;
            SOSLinearLayout.Click -= OnAlertSOSLayoutClick;
            ParameterLinearLayout.Click -= OnParameterLayoutClick;
            SeekiosInformationLayout.Click -= OnMapLayoutClick;
            FirmwareUpdateSvgImageView.Click -= FirmwareUpdateImageView_Click;
            CreditLayout.Click -= CreditLayout_Click;
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.SeekiosChanged -= App_SeekiosChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            App.RemainingRequestChanged -= OnRemainingRequestChanged;
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region ===== Initializes View ============================================================

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);

            SeekiosInformationLayout = FindViewById<RelativeLayout>(Resource.Id.detailSeekios_seekiosLayout);
            SeekiosNameTextView = FindViewById<TextView>(Resource.Id.detailSeekios_seekiosName);
            ButtonRightLayout = FindViewById<RelativeLayout>(Resource.Id.myseekios_ButtonsRight);
            SeekiosRoundedImageView = FindViewById<RoundedImageView>(Resource.Id.detailSeekios_seekiosImage);
            PowerSavingImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_powerSavingImage);
            FirmwareUpdateSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_firmwareUpdate);
            LastLocationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_lastPosition);
            ModeSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_modeImage);
            ModeTitleTextView = FindViewById<TextView>(Resource.Id.detailSeekios_modeImageText);
            ModeLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_modeSeekiosLayout);
            AlertTextView = FindViewById<TextView>(Resource.Id.detailSeekios_alertImageText);
            AlertSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_alertImage);
            AlertSeekiosLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_alertLayout);

            BatterySvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_batteryPicture);
            BatteryTextView = FindViewById<TextView>(Resource.Id.detailSeekios_batteryText);
            BatteryRefreshButton = FindViewById<TextView>(Resource.Id.detailSeekios_refreshButton);
            BatteryLoaderProgressBar = FindViewById<ProgressBar>(Resource.Id.detailSeekios_loaderBattery);

            CreditsTextView = FindViewById<TextView>(Resource.Id.detailSeekios_credits);
            FreeCreditTextView = FindViewById<TextView>(Resource.Id.detailSeekios_freeCredits);
            FreeCreditReloadDateTextView = FindViewById<TextView>(Resource.Id.detailSeekios_freeCreditsReloadDate);
            CreditLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_creditLayout);

            MapLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_mapLayout);
            ModeLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_modeLayout);
            SOSLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_sosLayout);
            ParameterLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_parameterLayout);
            ModeLayoutSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_modePicture);
            ModeLayoutTextView = FindViewById<TextView>(Resource.Id.detailSeekios_modeText);
            ParameterLayoutSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_parameterPicture);
            ParameterLayoutTextView = FindViewById<TextView>(Resource.Id.detailSeekios_parameterText);

            ModeInformationLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_informationModeLayout);
            ModeInformationSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_informationModePicture);
            ModeInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationModeText);
            AlertInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationAlertText);
            TrackingInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationTrackingText);

            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
        }

        private void SetDataToView()
        {
            // If no position yet, we block mode configurations
            if (_seekiosSelected == null) _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                   && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                ModeLinearLayout.Enabled = false;
                ModeLayoutSvgImageView.SetSvg(this, Resource.Drawable.SelectMode, "62da73=999999");
            }
            else
            {
                ModeLinearLayout.Enabled = true;
                ModeLayoutSvgImageView.SetSvg(this, Resource.Drawable.SelectMode, "999999=62da73");
            }

            var _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnDemand != null)
            {
                _seekiosOnDemand.OnSuccess = null;
                _seekiosOnDemand.OnSuccess += () =>
                {
                    InitialiseSeekiosInformation();
                };
            }

            InitialiseSeekiosInformation();
            InitialiseBatteryInformation();
            //InitialiseInformationLayout();
            RefreshDisplayedCreditCount();
            ModeInformationLayout.Visibility = ViewStates.Gone;

            FreeCreditReloadDateTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_freeCreditsReloadDate)
                , CreditHelper.CalculateNextReload());
        }

        private void InitialiseSeekiosInformation()
        {
            if (null == _seekiosSelected) return;
            // Seekios name
            SeekiosNameTextView.Text = _seekiosSelected.SeekiosName;
            SeekiosNameTextView.SetTextColor(Resources.GetColor(Resource.Color.primary));

            // Seekios image
            if (!string.IsNullOrEmpty(_seekiosSelected.SeekiosPicture))
            {
                var bytes = Convert.FromBase64String(_seekiosSelected.SeekiosPicture);
                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                SeekiosRoundedImageView.SetImageBitmap(imageBitmap);
                imageBitmap.Dispose();
            }
            else SeekiosRoundedImageView.SetImageResource(Resource.Drawable.DefaultSeekios);

            // Seekios need update ? 
            if (App.CurrentUserEnvironment.LastVersionEmbedded != null
                && _seekiosSelected.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded
                && !App.CurrentUserEnvironment.LastVersionEmbedded.IsBetaVersion)
            {
                if (_seekiosSelected.IsInPowerSaving)
                {
                    FirmwareUpdateSvgImageView.TranslationY = (-AccessResources.Instance.SizeOf30Dip());
                }
                FirmwareUpdateSvgImageView.Visibility = ViewStates.Visible;
            }

            // Display or not power saving picture
            if (_seekiosSelected.IsInPowerSaving)
            {
                PowerSavingImageView.Visibility = ViewStates.Visible;
                PowerSavingImageView.BringToFront();
            }
            else PowerSavingImageView.Visibility = ViewStates.Gone;

            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                // Display statement to say there is no position
                LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_lastPositionNone);
            }
            else
            {
                if (_seekiosSelected.LastKnownLocation_dateLocationCreation.HasValue
                && _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                {
                    if (_seekiosSelected.IsOnDemand)
                    {
                        // Display on refresh state (OnDemand)
                        var textToDisplay = Resources.GetString(Resource.String.listSeekios_refreshPosition);
                        var _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
                        if (_seekiosOnDemand != null)
                        {
                            int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                            int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                            LastLocationTextView.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                            _seekiosOnDemand.Timer.UpdateUI = () =>
                            {
                                minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                                seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                                LastLocationTextView.Text = textToDisplay + string.Format(" {00:00}:{01:00}", minutes, seconds);
                            };
                            // Hidden the count down, specific UI when it's the first location
                            _seekiosOnDemand.Timer.Stopped = () =>
                            {
                                LastLocationTextView.Text = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios).LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                            };
                        }
                        else LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_refreshPosition);
                    }
                    else LastLocationTextView.Text = _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                }
                else LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_lastPositionNone);
            }
            LastLocationTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorTitle));

            // Mode name and image
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);
            ModeTitleTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorTitle));

            // Update alert layout && low battery layout
            AlertSeekiosLayout.Visibility = ViewStates.Gone;
            if (_seekiosSelected.BatteryLife <= 20 && _seekiosSelected.LastKnownLocation_dateLocationCreation.HasValue)
            {
                AlertSvgImageView.SetSvg(this, Resource.Drawable.CriticalBattery);
                AlertTextView.Text = _seekiosSelected.BatteryLife + "%";
                AlertSeekiosLayout.Visibility = ViewStates.Visible;
            }
            else if (!_seekiosSelected.IsLastSOSRead)
            {
                AlertSvgImageView.SetSvg(this, Resource.Drawable.SOS, "2FAD62=da2e2e");
                AlertTextView.Text = GetString(Resource.String.detailSeekios_sos);
                AlertSeekiosLayout.Visibility = ViewStates.Visible;
            }

            if (mode != null)
            {
                ModeLayout.Visibility = ViewStates.Visible;
                if (!_seekiosSelected.HasGetLastInstruction
                    && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios)
                    && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                    && !_seekiosSelected.IsRefreshingBattery
                    || !mode.DateModeActivation.HasValue)
                {
                    if (_seekiosSelected.IsInPowerSaving)
                    {
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                        {
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeTracking, "62da73=c8c8c8");
                        }
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                        {
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeDontMove, "62da73=c8c8c8");
                        }
                        else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                        {
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeZone, "62da73=c8c8c8");
                        }
                        ModeTitleTextView.Text = string.Format(GetString(Resource.String.listSeekios_nextNoon), DateHelper.TimeLeftUntilNextNoon());
                    }
                    else
                    {
                        ModeSvgImageView.SetSvg(this, Resource.Drawable.CloudSync);
                        ModeTitleTextView.SetText(Resource.String.listSeekios_synchr);
                    }
                }
                // The seekios has moved or is out of the area
                else if (mode.StatusDefinition_idstatusDefinition != 1)
                {
                    switch (mode.ModeDefinition_idmodeDefinition)
                    {
                        case (int)ModeDefinitionEnum.ModeDontMove:
                            if (mode.StatusDefinition_idstatusDefinition == 3)
                            {
                                LastLocationTextView.SetText(Resource.String.modeDontMove_seekiosMoved);
                                ModeTitleTextView.SetText(Resource.String.detailSeekios_dontMove);
                                ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeDontMove, "62da73=da2e2e");
                            }
                            break;
                        case (int)ModeDefinitionEnum.ModeZone:
                            if (mode.StatusDefinition_idstatusDefinition == 2)
                            {
                                LastLocationTextView.SetText(Resource.String.modeZone_seekiosOutOfZone);
                                ModeTitleTextView.SetText(Resource.String.detailSeekios_zone);
                                ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeZone, "62da73=da2e2e");
                            }
                            break;
                        default:
                            break;
                    }
                    var colorRed = Resources.GetColor(Resource.Color.color_red);
                    ModeTitleTextView.SetTextColor(colorRed);
                    SeekiosNameTextView.SetTextColor(colorRed);
                    LastLocationTextView.SetTextColor(colorRed);
                }
                // Seekios is in a mode
                else
                {
                    // Configuration of the mode layout
                    switch (mode.ModeDefinition_idmodeDefinition)
                    {
                        case 3:
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeTracking);
                            ModeTitleTextView.SetText(Resource.String.listSeekios_modeTracking);
                            break;
                        case 4:
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeDontMove);
                            ModeTitleTextView.SetText(Resource.String.listSeekios_modeDontMove);
                            break;
                        case 5:
                            ModeSvgImageView.SetSvg(this, Resource.Drawable.ModeZone);
                            ModeTitleTextView.SetText(Resource.String.listSeekios_modeZone);
                            break;
                    }
                }


            }
            else if (_seekiosSelected.IsLastSOSRead) ModeLayout.Visibility = ViewStates.Invisible;
        }

        private void InitialiseBatteryInformation()
        {
            if (_seekiosSelected.BatteryLife >= 0 && _seekiosSelected.DateLastCommunication != null && _seekiosSelected.DateLastCommunication.HasValue)
            {
                BatteryTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_batteryPercentage)
                       , _seekiosSelected.BatteryLife
                        , _seekiosSelected.DateLastCommunication.Value.FormatDateTimeFromNow(false));
            }
            else BatteryTextView.Text = Resources.GetString(Resource.String.detailSeekios_noBatteryData);

            // Set battery color
            if (_seekiosSelected.BatteryLife <= 100 && _seekiosSelected.BatteryLife >= 95)
            {
                BatterySvgImageView.SetSvg(this, Resource.Drawable.BatteryFull);
            }
            else if (_seekiosSelected.BatteryLife <= 95 && _seekiosSelected.BatteryLife > 66)
            {
                BatterySvgImageView.SetSvg(this, Resource.Drawable.Battery);
            }
            else if (_seekiosSelected.BatteryLife <= 66 && _seekiosSelected.BatteryLife > 33)
            {
                BatterySvgImageView.SetSvg(this, Resource.Drawable.BatteryMedium);
            }
            else if (_seekiosSelected.BatteryLife <= 33 && _seekiosSelected.BatteryLife > 0)
            {
                BatterySvgImageView.SetSvg(this, Resource.Drawable.BatteryLow);
            }
            else BatterySvgImageView.SetSvg(this, Resource.Drawable.NoBattery);

            // Initialisation of the battery loader
            if (_seekiosSelected.IsRefreshingBattery)
            {
                BatteryLoaderProgressBar.Visibility = ViewStates.Visible;
                BatteryRefreshButton.Enabled = false;
            }
            else
            {
                BatteryLoaderProgressBar.Visibility = ViewStates.Invisible;
                BatteryRefreshButton.Enabled = true;
            }
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #endregion

        #region ===== Private Methods =============================================================

        private string GetModeName(int mode_definition)
        {
            var name = string.Empty;

            switch (mode_definition)
            {
                default:
                    break;
                case (int)ModeDefinitionEnum.ModeZone:
                    name = Resources.GetString(Resource.String.detailSeekios_zone);
                    break;
                case (int)ModeDefinitionEnum.ModeTracking:
                    name = Resources.GetString(Resource.String.detailSeekios_tracking);
                    break;
                case (int)ModeDefinitionEnum.ModeDontMove:
                    name = Resources.GetString(Resource.String.detailSeekios_dontMove);
                    break;
            }
            return name;
        }

        /// <summary>
        /// Display SOS alert popup if needed (IsRead = false)
        /// </summary>
        private void DisplayLastAlertSOS()
        {
            if (App.Locator.DetailSeekios.AlertNeedsToBeRead())
            {
                AlertDialog.Builder AlertDialogBuilder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog_Alert);
                AlertDialog AlertDialog = AlertDialogBuilder.Create();
                AlertDialog.SetTitle(Resources.GetString(Resource.String.detailSeekios_alertPopupTitle));
                AlertDialog.SetMessage(Resources.GetString(Resource.String.detailSeekios_alertPopupContent));
                AlertDialog.SetButton(Resources.GetString(Resource.String.detailSeekios_alertPopupButton), async (senderAlert, args) =>
                {
                    if (await App.Locator.DetailSeekios.NotifyAlertSOSHasBeenRead(_seekiosSelected.Idseekios) <= 0)
                    {
                        Toast.MakeText(this, Resource.String.detailSeekios_alertPopupError, ToastLength.Short);
                    }
                    else SetDataToView();
                    AlertDialog.Dismiss();
                });
                AlertDialog.Show();
            }
        }

        #endregion

        #region ===== Credits Seekios =============================================================

        private void OnRemainingRequestChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                RefreshDisplayedCreditCount();
            });
        }

        public void RefreshDisplayedCreditCount()
        {
            if (_seekiosSelected == null) return;
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.UIdSeekios == _seekiosSelected.UIdSeekios);

            // If there is any free credit on the seekios we display freeCredit
            CreditsTextView.Text = CreditHelper.TotalCredits;

            var resultTuple = StringHelper.GetStartAndEndIndexOfStringInString(Resources.GetString(Resource.String.detailSeekios_freeCredits), CreditHelper.CreditsOffered);
            var formattedinfoText = new SpannableString(string.Format(Resources.GetString(Resource.String.detailSeekios_freeCredits), CreditHelper.CreditsOffered));
            formattedinfoText.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            FreeCreditTextView.SetText(formattedinfoText, null);
        }

        #endregion

        #region ===== Events ======================================================================

        private async void OnBatteryRefreshButtonClick(object sender, EventArgs e)
        {
            if (await App.Locator.DetailSeekios.RequestBatteryLevel())
            {
                BatteryRefreshButton.Enabled = false;
                BatteryLoaderProgressBar.Visibility = ViewStates.Visible;
            }
            else
            {
                BatteryRefreshButton.Enabled = true;
                BatteryLoaderProgressBar.Visibility = ViewStates.Invisible;
            }
        }

        private void OnMapLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToMap(_seekiosSelected);
        }

        private void OnModeLayoutClick(object sender, EventArgs e)
        {
            DisplayModeSelection();
        }

        private void OnAlertSOSLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToAlertSOS();
        }

        private void OnParameterLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToParameter(_seekiosSelected);
        }

        private void App_SeekiosChanged(object sender, int e)
        {
            SetDataToView();
        }

        /// <summary>
        /// Shows the last update available in a popup
        /// </summary>
        private void FirmwareUpdateImageView_Click(object sender, EventArgs e)
        {
            CreateFirmwareUpdatePopup(this);
        }

        /// <summary>
        /// UpdateUI when connection state changed
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            BatteryRefreshButton.Enabled = isConnected;
            BatteryLoaderProgressBar.Visibility = isConnected && _seekiosSelected.IsRefreshingBattery ? ViewStates.Visible : ViewStates.Invisible;
        }

        /// <summary>
        /// Click on the credit layout, navigate to the reload credit
        /// </summary>
        private void CreditLayout_Click(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToBuyCredits();
        }

        #endregion

        #region ===== Select a mode ===============================================================

        #region Properties

        public LinearLayout ModeTracking { get; set; }
        public LinearLayout ModeDontMove { get; set; }
        public LinearLayout ModeZone { get; set; }
        public LinearLayout DeleteModeLayout { get; set; }
        public TextView DeleteTextView { get; set; }

        #endregion

        /// <summary>
        /// Display a popup to pick up a mode
        /// </summary>
        private void DisplayModeSelection()
        {
            Dialog modeDialog = null;
            var modeBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            var inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);
            var view = inflater.Inflate(Resource.Drawable.PopupModeDefinitionChoice, null);

            // Close button
            modeBuilder.SetNegativeButton(Resource.String.detailSeekios_deletePopupButtonClose, (senderAlert, args) =>
            {
                modeDialog.Dismiss();
            });

            // Select Tracking mode 
            ModeTracking = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_tracking);
            ModeTracking.Click += (s, arg) =>
            {
                modeDialog.Dismiss();
                SelectMode(3);
            };

            // Select Don't Move mode 
            ModeDontMove = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_dontMove);
            ModeDontMove.Click += (s, arg) =>
            {
                modeDialog.Dismiss();
                SelectMode(4);
            };

            // Select Zone mode 
            ModeZone = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_zone);
            ModeZone.Click += (s, arg) =>
            {
                modeDialog.Dismiss();
                SelectMode(5);
            };

            // Select no mode
            DeleteModeLayout = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_deleteModeLayout);
            var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == _seekiosSelected.Idseekios).FirstOrDefault();
            if (mode == null)
            {
                DeleteModeLayout.Visibility = ViewStates.Gone;
            }
            // One mode is activated, so we have the choice to delete it
            else
            {
                DeleteTextView = view.FindViewById<TextView>(Resource.Id.modeDefinition_deleteText);
                DeleteTextView.Text = string.Format(Resources.GetString(Resource.String.modeDefinition_delete), GetModeName(mode.ModeDefinition_idmodeDefinition));
                DeleteModeLayout.Click += async (s, arg) =>
                {
                    modeDialog.Dismiss();
                    App.Locator.ModeSelection.SeekiosUpdated = _seekiosSelected;
                    if (await App.Locator.ModeSelection.DeleteMode(mode))
                    {
                        _seekiosSelected.HasGetLastInstruction = false;
                        SetDataToView();
                    }
                };

                var typeValue = new TypedValue();
                Theme.ResolveAttribute(Resource.Attribute.layoutBackgroundSecondly, typeValue, true);
                var backgroundSecondaryColor = Resources.GetColor(typeValue.ResourceId);
                Theme.ResolveAttribute(Resource.Attribute.colorFirst, typeValue, true);
                var textPrimaryColor = Resources.GetColor(typeValue.ResourceId);

                if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    ModeTracking.SetBackgroundColor(backgroundSecondaryColor);
                    view.FindViewById<TextView>(Resource.Id.modeDefinition_trackingTitle).SetTextColor(textPrimaryColor);
                    view.FindViewById<SvgImageView>(Resource.Id.modeDefinition_trackingImg).SetSvg(this, Resource.Drawable.ModeTracking, "62da73");
                }
                else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                {
                    ModeDontMove.SetBackgroundColor(backgroundSecondaryColor);
                    view.FindViewById<TextView>(Resource.Id.modeDefinition_dontMoveTitle).SetTextColor(textPrimaryColor);
                    view.FindViewById<SvgImageView>(Resource.Id.modeDefinition_dontMoveImg).SetSvg(this, Resource.Drawable.ModeDontMove, "62da73");
                }
                else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                {
                    ModeZone.SetBackgroundColor(backgroundSecondaryColor);
                    view.FindViewById<TextView>(Resource.Id.modeDefinition_zoneTitle).SetTextColor(textPrimaryColor);
                    view.FindViewById<SvgImageView>(Resource.Id.modeDefinition_zoneImg).SetSvg(this, Resource.Drawable.ModeZone, "62da73");
                }
            }

            modeBuilder.SetView(view);
            modeDialog = modeBuilder.Create();
            modeDialog.Show();
        }

        /// <summary>
        /// Select a mode
        /// </summary>
        private void SelectMode(int idModeDefinition)
        {
            App.Locator.ModeSelection.SeekiosUpdated = _seekiosSelected;
            App.Locator.ModeZone.IsActivityFocused = true;
            var modeDefinitionEnum = (Enum.ModeDefinitionEnum)System.Enum.Parse(typeof(Enum.ModeDefinitionEnum), idModeDefinition.ToString());
            App.Locator.ModeSelection.GoToMode(modeDefinitionEnum);
        }

        #endregion
    }
}