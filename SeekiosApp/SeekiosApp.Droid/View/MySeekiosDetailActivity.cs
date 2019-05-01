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
using System.Collections.Generic;
using SeekiosApp.Helper;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class MySeekiosDetailActivity : AppCompatActivityBase
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
        public ImageView PowerSavingImageView { get; set; }

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

        /// <summary>Alert image</summary>
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

        /// <summary>Battery loader layout</summary>
        public LinearLayout BatteryLoaderLayout { get; set; }

        #endregion

        #region CREDITS LAYOUT

        ///// <summary>Monthly credits number text</summary>
        //public TextView MonthlyCreditNumberTextView { get; set; }

        ///// <summary>Text which informs how much time before credit refill</summary>
        //public TextView MonthlyCreditInfoTextView { get; set; }

        ///// <summary>General credits number tex</summary>
        //public TextView GeneralCreditNumberTextView { get; set; }

        //public LinearLayout MonthlyCreditLayout { get; set; }

        //public LinearLayout GeneralCreditLayout { get; set; }

        #endregion

        #region MAP MODE SHARE PARAM

        /// <summary>Map LinearLayout handling click</summary>
        public LinearLayout MapLinearLayout { get; set; }

        /// <summary>Mode LinearLayout handling click</summary>
        public LinearLayout ModeLinearLayout { get; set; }

        public LinearLayout SOSLinearLayout { get; set; }

        /// <summary>Share LinearLayout handling click</summary>
        //public LinearLayout ShareLinearLayout { get; set; }

        /// <summary>Parameter LinearLayout handling click</summary>
        public LinearLayout ParameterLinearLayout { get; set; }

        /// <summary>Mode layout image<summary>
        public SvgImageView ModeLayoutSvgImageView { get; set; }

        /// <summary>Mode layout text<summary>
        public TextView ModeLayoutTextView { get; set; }

        /// <summary>Mode layout image<summary>
        //public SvgImageView ShareLayoutSvgImageView { get; set; }

        /// <summary>Mode layout text<summary>
        public TextView ShareLayoutTextView { get; set; }
        /// <summary>Mode layout image<summary>
        public SvgImageView ParameterLayoutSvgImageView { get; set; }

        /// <summary>Mode layout text<summary>
        public TextView ParameterLayoutTextView { get; set; }

        #endregion

        #region DELETE SEEKIOS AND INFORMATION

        /// <summary>Delete seekios button</summary>
        //public TextView DeleteSeekiosButton { get; set; }

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

        /// <summary>Share info layout</summary>
        public LinearLayout ShareInformationLayout { get; set; }

        /// <summary>Share info text</summary>
        public TextView ShareInformationTextView { get; set; }

        #endregion

        private Dialog _popupCreditCosts = null;
        private TextView _textePopupTextView;
        private TextView _textePopupTextView2;
        #endregion

        #region ===== Cycle De Vie ================================================================

        /// <summary>
        /// Création de la page
        /// </summary>
        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.MySeekiosDetailLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();
            //SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.detailSeekios_pageTitle), _seekiosSelected != null ? _seekiosSelected.SeekiosName : string.Empty);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            App.RemainingRequestChanged += OnRemainingRequestChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            App.Locator.ListSeekios.PropertyChanged += OnListSeekiosPropertyChanged;
            App.SeekiosChanged += OnSeekiosChanged;
        }

        /// <summary>
        /// Reprise de la page
        /// </summary>
        protected override void OnResume()
        {
            if (App.Locator.DetailSeekios.IsSeekiosDeleted)
            {
                App.Locator.DetailSeekios.IsSeekiosDeleted = false;
                Finish();
            }

            base.OnResume();
            //App.Locator.Login.AppIsNotGoingBackground(null, null);

            SetDataToView();
            DisplayLastAlertSOS();
            //subscribe to connection state changed event - needs to subscribe before calling base
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;

            if (App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated)
            {
                UpdateUI();
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = false;
            }
            MapLinearLayout.Click += OnMapLayoutClick;
            ModeLinearLayout.Click += OnModeLayoutClick;
            //DeleteSeekiosButton.Click += DeleteSeekiosButton_Click;
            BatteryRefreshButton.Click += OnBatteryRefreshButtonClick;
            //ShareLinearLayout.Click += OnShareLayoutClick;
            SOSLinearLayout.Click += OnAlertSOSLayoutClick;
            ParameterLinearLayout.Click += OnParameterLayoutClick;
            SeekiosInformationLayout.Click += OnParameterLayoutClick;
            FirmwareUpdateSvgImageView.Click += FirmwareUpdateImageView_Click;
            //Credits clicks
            //GeneralCreditLayout.Click += GeneralCreditLayoutClick;
            //MonthlyCreditLayout.Click += MonthlyCreditLayoutClick;

            //var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == App.Locator.MySeekiosDetail.SeekiosSelected.Idseekios);
        }

        /// <summary>
        /// Suspension de la page
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();
            MapLinearLayout.Click -= OnMapLayoutClick;
            ModeLinearLayout.Click -= OnModeLayoutClick;
            //DeleteSeekiosButton.Click -= DeleteSeekiosButton_Click;
            BatteryRefreshButton.Click -= OnBatteryRefreshButtonClick;
            //ShareLinearLayout.Click -= OnShareLayoutClick;
            SOSLinearLayout.Click -= OnAlertSOSLayoutClick;
            ParameterLinearLayout.Click -= OnParameterLayoutClick;
            SeekiosInformationLayout.Click -= OnParameterLayoutClick;
            FirmwareUpdateSvgImageView.Click -= FirmwareUpdateImageView_Click;
            //Credits clicks
            //GeneralCreditLayout.Click -= GeneralCreditLayoutClick;
            //MonthlyCreditLayout.Click -= MonthlyCreditLayoutClick;

            //what's the point of this ?
            //var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == App.Locator.MySeekiosDetail.SeekiosSelected.Idseekios);

            //subscribe to connection state changed event
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.Locator.ListSeekios.PropertyChanged -= OnListSeekiosPropertyChanged;
            App.SeekiosChanged -= OnSeekiosChanged;
        }

        /// <summary>
        /// Suppression de la page
        /// </summary>
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

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Initialise les objets de la vue
        /// </summary>
        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);

            SeekiosInformationLayout = FindViewById<RelativeLayout>(Resource.Id.detailSeekios_seekiosLayout);
            SeekiosNameTextView = FindViewById<TextView>(Resource.Id.detailSeekios_seekiosName);
            ButtonRightLayout = FindViewById<RelativeLayout>(Resource.Id.myseekios_ButtonsRight);
            SeekiosRoundedImageView = FindViewById<RoundedImageView>(Resource.Id.detailSeekios_seekiosImage);
            PowerSavingImageView = FindViewById<ImageView>(Resource.Id.detailSeekios_powerSavingImage);
            FirmwareUpdateSvgImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.detailSeekios_firmwareUpdate);
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
            BatteryLoaderLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_loaderLayoutBattery);

            //MonthlyCreditNumberTextView = FindViewById<TextView>(Resource.Id.detailSeekios_creditFreeNumber);
            //MonthlyCreditInfoTextView = FindViewById<TextView>(Resource.Id.detailSeekios_creditFreeText);
            //GeneralCreditNumberTextView = FindViewById<TextView>(Resource.Id.detailSeekios_creditGeneralNumber);
            //MonthlyCreditLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_creditFreeLayout);
            //GeneralCreditLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_creditGeneralLayout);

            MapLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_mapLayout);
            ModeLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_modeLayout);
            //ShareLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_shareLayout);
            SOSLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_sosLayout);
            ParameterLinearLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_parameterLayout);
            ModeLayoutSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_modePicture);
            ModeLayoutTextView = FindViewById<TextView>(Resource.Id.detailSeekios_modeText);
            //ShareLayoutSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_sharePicture);
            //Disable color button
            //ShareLayoutSvgImageView.SetSvg(this, Resource.Drawable.Share2, "ffffff=999999");
            ShareLayoutTextView = FindViewById<TextView>(Resource.Id.detailSeekios_shareText);
            ParameterLayoutSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_parameterPicture);
            ParameterLayoutTextView = FindViewById<TextView>(Resource.Id.detailSeekios_parameterText);

            //DeleteSeekiosButton = FindViewById<TextView>(Resource.Id.detailSeekios_deleteSeekiosButton);
            ModeInformationLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_informationModeLayout);
            ModeInformationSvgImageView = FindViewById<SvgImageView>(Resource.Id.detailSeekios_informationModePicture);
            ModeInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationModeText);
            AlertInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationAlertText);
            TrackingInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationTrackingText);
            ShareInformationLayout = FindViewById<LinearLayout>(Resource.Id.detailSeekios_informationShareLayout);
            ShareInformationTextView = FindViewById<TextView>(Resource.Id.detailSeekios_informationShareText);

            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
        }

        /// <summary>
        /// Initialise la vue
        /// </summary>
        private void SetDataToView()
        {
            //If no position yet, we block mode configurations
            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                   && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                ModeLinearLayout.Enabled = false;
                ModeLayoutSvgImageView.SetSvg(this, Resource.Drawable.Pin, "62da73=999999");
            }
            else
            {
                ModeLinearLayout.Enabled = true;
                ModeLayoutSvgImageView.SetSvg(this, Resource.Drawable.Pin, "999999=62da73");
            }

            InitialiseSeekiosInformation();
            InitialiseBatteryInformation();
            InitialiseInformationLayout();
            InitialiseCreditsInformation();
        }

        private void InitialiseSeekiosInformation()
        {
            if (null == _seekiosSelected) return;
            // seekios name
            SeekiosNameTextView.Text = _seekiosSelected.SeekiosName;
            SeekiosNameTextView.SetTextColor(Resources.GetColor(Resource.Color.primary));

            // seekios image
            if (!string.IsNullOrEmpty(_seekiosSelected.SeekiosPicture))
            {
                var bytes = Convert.FromBase64String(_seekiosSelected.SeekiosPicture);
                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
                SeekiosRoundedImageView.SetImageBitmap(imageBitmap);
                imageBitmap.Dispose();
            }
            else SeekiosRoundedImageView.SetImageResource(Resource.Drawable.DefaultSeekios);

            // Power saving and firmware update images
            if(_seekiosSelected.IsInPowerSaving)
            {
                PowerSavingImageView.Visibility = ViewStates.Visible;
            }
            else
            {
                PowerSavingImageView.Visibility = ViewStates.Gone;
            }

            if (_seekiosSelected.VersionEmbedded_idversionEmbedded != App.CurrentUserEnvironment.LastVersionEmbedded.IdVersionEmbedded)
            {
                FirmwareUpdateSvgImageView.Visibility = ViewStates.Visible;
                if(_seekiosSelected.IsInPowerSaving)
                {
                    RelativeLayout.LayoutParams rl = (RelativeLayout.LayoutParams)FirmwareUpdateSvgImageView.LayoutParameters;
                    rl.TopMargin -= 25;
                    FirmwareUpdateSvgImageView.LayoutParameters = rl;
                }
            }
            else
            {
                FirmwareUpdateSvgImageView.Visibility = ViewStates.Gone;
            }

            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                // display statement to say there is no position
                LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_lastPositionNone);
            }
            else
            {
                if (_seekiosSelected.LastKnownLocation_dateLocationCreation.HasValue
                && _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.Year != 1)
                {
                    if (_seekiosSelected.IsOnDemand)
                    {
                        // display on refresh state (OnDemand)
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
                            // hidden the count down, specific UI when it's the first location
                            _seekiosOnDemand.Timer.Stopped = () =>
                            {
                                LastLocationTextView.Text = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.UIdSeekios == App.Locator.DetailSeekios.SeekiosSelected.UIdSeekios).LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                            };
                        }
                        else LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_refreshPosition);
                    }
                    else
                    {
                        // display date of the last position
                        LastLocationTextView.Text = _seekiosSelected.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
                    }
                }
                else
                {
                    // display no position
                    LastLocationTextView.Text = Resources.GetString(Resource.String.listSeekios_lastPositionNone);
                }
            }
            LastLocationTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorTitle));

            // mode name and image
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);
            ModeTitleTextView.SetTextColor(Resources.GetColor(Resource.Color.textColorTitle));

            //update alert layout && low battery layout
            if (_seekiosSelected.BatteryLife <= 20)
            {
                AlertSvgImageView.SetSvg(this, Resource.Drawable.CriticalBattery);
                AlertTextView.Text = _seekiosSelected.BatteryLife + "%";
                AlertSeekiosLayout.Visibility = ViewStates.Visible;
            }
            if (!_seekiosSelected.IsLastSOSRead)
            {
                AlertSvgImageView.SetSvg(this, Resource.Drawable.SOS, "2FAD62=da2e2e");
                AlertTextView.Text = GetString(Resource.String.detailSeekios_sos);
                AlertSeekiosLayout.Visibility = ViewStates.Visible;
            }
            else if (_seekiosSelected.BatteryLife > 20 && _seekiosSelected.IsLastSOSRead) AlertSeekiosLayout.Visibility = ViewStates.Gone;

            if (mode != null)
            {
                ModeLayout.Visibility = ViewStates.Visible;
                if (!_seekiosSelected.HasGetLastInstruction)
                {
                    ModeSvgImageView.SetSvg(this, Resource.Drawable.CloudSync);
                    ModeTitleTextView.SetText(Resource.String.listSeekios_synchr);
                    //AlertSeekiosLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    //configuration of the mode layout
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
                        default:
                            //ButtonRightLayout.Visibility = ViewStates.Gone;
                            break;
                    }
                }

                if (mode.StatusDefinition_idstatusDefinition != 1)
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
            }
            else
            {
                ModeLayout.Visibility = ViewStates.Invisible;
            }
        }

        private void InitialiseBatteryInformation()
        {
            if (_seekiosSelected.BatteryLife >= 0 && _seekiosSelected.DateLastCommunication != null && _seekiosSelected.DateLastCommunication.HasValue)
                BatteryTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_batteryPercentage)
                    , _seekiosSelected.BatteryLife
                    , _seekiosSelected.DateLastCommunication.Value.ToString("dd/MM/yyyy")
                    , _seekiosSelected.DateLastCommunication.Value.ToString("H:mm"));

            else BatteryTextView.Text = Resources.GetString(Resource.String.detailSeekios_noBatteryData);

            //Set battery color
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

            //Initialisation of the battery loader
            if (!_seekiosSelected.HasGetLastInstruction)
            {
                //BatteryLoaderLayout.Visibility = ViewStates.Visible;
                BatteryRefreshButton.Enabled = false;
            }
            else
            {
                BatteryLoaderLayout.Visibility = ViewStates.Gone;
                BatteryRefreshButton.Enabled = true;
            }
        }

        private void InitialiseCreditsInformation()
        {
            //TODO: implement the credit model
            // credit monthly
            //MonthlyCreditInfoTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_freeCreditsReloadDate),
            //    SeekiosApp.Helper.CreditHelper.CalculateNextReload(_seekiosSelected.SeekiosDateCreation));
            //MonthlyCreditNumberTextView.Text = _seekiosSelected.FreeCredit.ToString();

            //// credit permanent
            //GeneralCreditNumberTextView.Text = SeekiosApp.Helper.CreditHelper.CreditUser;
            //GeneralCreditNumberTextView.SetTextSize(ComplexUnitType.Dip, 25);
            //if (App.CurrentUserEnvironment.User.RemainingRequest > 999)
            //    GeneralCreditNumberTextView.SetTextSize(ComplexUnitType.Dip, 20);
        }

        private void InitialiseInformationLayout()
        {
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);
            if (mode != null && _seekiosSelected.HasGetLastInstruction)
            {
                var timeSinceModeCreation = DateHelper.GetSystemTime() - mode.DateModeCreation;
                ModeInformationTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_modeInformationText),
                                                                     GetModeName(mode.ModeDefinition_idmodeDefinition), Math.Truncate(timeSinceModeCreation.TotalDays) + 1);
                switch (mode.ModeDefinition_idmodeDefinition)
                {
                    default:
                        break;
                    case (int)ModeDefinitionEnum.ModeDontMove:
                        ModeInformationSvgImageView.SetSvg(this, Resource.Drawable.ModeDontMove);
                        break;
                    case (int)ModeDefinitionEnum.ModeTracking:
                        ModeInformationSvgImageView.SetSvg(this, Resource.Drawable.ModeTracking);
                        break;
                    case (int)ModeDefinitionEnum.ModeZone:
                        ModeInformationSvgImageView.SetSvg(this, Resource.Drawable.ModeZone);
                        break;
                }

                if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    AlertInformationTextView.Text = Resources.GetString(Resource.String.detailSeekios_noAlertOnMode);
                }
                else
                {
                    var alertCount = App.CurrentUserEnvironment.LsAlert.Count(el => el.IdMode == mode.Idmode);
                    AlertInformationTextView.Text = alertCount > 0
                        ? string.Format(Resources.GetString(Resource.String.detailSeekios_alertInformationText), alertCount)
                        : AlertInformationTextView.Text = Resources.GetString(Resource.String.detailSeekios_onlyNotif);
                }

                //TODO : handle the advanced settings (for instance, tracking when out of zone etc...)
                TrackingInformationTextView.Visibility = ViewStates.Gone;
                ModeInformationLayout.Visibility = ViewStates.Visible;
            }
            else
            {
                ModeInformationLayout.Visibility = ViewStates.Gone;
            }

            if (App.CurrentUserEnvironment.LsSharing.Where(el => el.Seekios_IdSeekios == _seekiosSelected.Idseekios).Count() <= 0)
            {
                ShareInformationLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                ShareInformationLayout.Visibility = ViewStates.Visible;
                var shareNbr = App.CurrentUserEnvironment.LsSharing.Where(el => el.Seekios_IdSeekios == _seekiosSelected.Idseekios).Count();
                if (shareNbr > 0)
                    ShareInformationTextView.Text = string.Format(Resources.GetString(Resource.String.detailSeekios_shareText), shareNbr);
                else ShareInformationTextView.Text = Resources.GetString(Resource.String.detailSeekios_noSharing);
            }
        }

        #endregion

        #region ===== Méthodes publiques ==========================================================

        #endregion

        #region ===== Méthodes privées ============================================================

        /// <summary>
        /// Update UI when action realised
        /// </summary>
        private void UpdateUI()
        {
            RunOnUiThread(() =>
            {
                SetDataToView();
            });
        }

        /// <summary>
        /// 
        /// </summary>
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
                        Toast.MakeText(this, Resource.String.detailSeekios_alertPopupError, ToastLength.Short);
                    else
                    {
                        _seekiosSelected.IsLastSOSRead = true;
                        UpdateUI();
                    }
                    AlertDialog.Dismiss();
                });
                AlertDialog.Show();
            }
        }

        #endregion

        #region ===== Events ======================================================================

        private void OnBatteryRefreshButtonClick(object sender, EventArgs e)
        {
            if (_seekiosSelected.IsInPowerSaving)
            {
                Toast.MakeText(this, Resources.GetString(Resource.String.detailSeekios_powerSavingRefreshDisabledPopupContent), ToastLength.Short);
            }
            else
            {
                RunOnUiThread(async () =>
                {
                    BatteryRefreshButton.Enabled = false;
                    BatteryLoaderLayout.Visibility = ViewStates.Visible;
                    await App.Locator.DetailSeekios.RequestBatteryLevel();
                });
            }
        }

        private void OnSeekiosChanged(object sender, int e)
        {
            RunOnUiThread(() => InitialiseInformationLayout());
        }

        /// <summary>
        /// Navigate to the page map
        /// </summary>
        private void OnMapLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToMap(_seekiosSelected);
        }

        /// <summary>
        /// Navigate to the alert defined for a mode zone or mode don't move
        /// </summary>
        private void OnAlertSeekiosLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToSeekiosAlert();
        }

        /// <summary>
        /// Open mode seekios selection popup
        /// </summary>
        private void OnModeLayoutClick(object sender, EventArgs e)
        {
            DisplayModeSelection();
        }

        /// <summary>
        /// Go to alert sos
        /// </summary>
        private void OnAlertSOSLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToAlertSOS();
        }

        /// <summary>
        /// On paramater clicked
        /// </summary>
        private void OnParameterLayoutClick(object sender, EventArgs e)
        {
            App.Locator.DetailSeekios.GoToParameter(_seekiosSelected);
        }

        private void OnListSeekiosPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //UiUpdate is sent when an instruction is taken and so is "LsSeekios" => to be changed
            if (e.PropertyName == "UIUpdate" || e.PropertyName == "LsSeekios")
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// Shows the last update available in a popup
        /// </summary>
        private async void FirmwareUpdateImageView_Click(object sender, EventArgs e)
        {
            CreateFirmwareUpdatePopup(this);
        }

        /// <summary>
        /// UpdateUI when connection state changed
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            BatteryRefreshButton.Enabled = isConnected;
            //DeleteSeekiosButton.Enabled = isConnected;
            BatteryLoaderLayout.Visibility = isConnected && _seekiosSelected.IsRefreshingBattery ? ViewStates.Visible : ViewStates.Gone;
        }

        private void OnRemainingRequestChanged(object sender, EventArgs e)
        {
            //TODO : it's not refresing the credit number here !
            RunOnUiThread(() => InitialiseCreditsInformation());
        }

        #endregion

        #region ===== Choix du mode ===============================================================

        #region Properties

        public LinearLayout ModeTracking { get; set; }
        public LinearLayout ModeDontMove { get; set; }
        public LinearLayout ModeZone { get; set; }
        public LinearLayout DeleteModeLayout { get; set; }
        public TextView DeleteTextView { get; set; }

        #endregion

        /// <summary>
        /// Popup des modes disponibles pour un Seekios
        /// </summary>
        private void DisplayModeSelection()
        {
            Dialog listSeekiosDialog = null;
            var listSeekiosBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            var inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);
            var view = inflater.Inflate(Resource.Drawable.PopupModeDefinitionChoice, null);

            //Cancel button
            listSeekiosBuilder.SetNegativeButton(Resource.String.detailSeekios_deletePopupButtonCancel, (senderAlert, args) =>
            {
                listSeekiosDialog.Dismiss();
            });

            DeleteModeLayout = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_deleteModeLayout);

            /// <summary>Désactivation du mode</summary>
            var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == _seekiosSelected.Idseekios).FirstOrDefault();


            /// <summary>Sélection du mode tracking</summary>
            ModeTracking = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_tracking);
            ModeTracking.Click += (s, arg) =>
            {
                listSeekiosDialog.Dismiss();
                SelectMode(3);
            };

            /// <summary>Sélection du mode don't move</summary>
            ModeDontMove = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_dontMove);
            ModeDontMove.Click += (s, arg) =>
            {
                listSeekiosDialog.Dismiss();
                SelectMode(4);
            };

            /// <summary>Sélection du mode zone</summary>
            ModeZone = view.FindViewById<LinearLayout>(Resource.Id.modeDefinition_zone);
            ModeZone.Click += (s, arg) =>
            {
                listSeekiosDialog.Dismiss();
                SelectMode(5);
            };

            // aucun mode
            if (mode == null)
            {
                DeleteModeLayout.Visibility = ViewStates.Gone;
            }
            // un mode est activé alors on donne la possibilité de le désactiver
            else
            {
                DeleteTextView = view.FindViewById<TextView>(Resource.Id.modeDefinition_deleteText);
                DeleteTextView.Text = string.Format(Resources.GetString(Resource.String.modeDefinition_delete), GetModeName(mode.ModeDefinition_idmodeDefinition));

                DeleteModeLayout.Click += (s, arg) =>
                {
                    RunOnUiThread(async () =>
                    {
                        LoadingLayout.Visibility = ViewStates.Visible;
                        App.Locator.ModeDefinition.SeekiosUpdated = _seekiosSelected;
                        listSeekiosDialog.Dismiss();
                        if (await App.Locator.ModeDefinition.DeleteMode(mode))
                        {
                            _seekiosSelected.HasGetLastInstruction = false;
                            if (App.Locator.BaseMap.LsSeekiosAlertState.Contains(_seekiosSelected.Idseekios))
                            {
                                App.Locator.BaseMap.LsSeekiosAlertState.Remove(_seekiosSelected.Idseekios);
                            }
                            UpdateUI();
                        }
                        LoadingLayout.Visibility = ViewStates.Invisible;
                    });
                };

                var typeValue = new TypedValue();
                Theme.ResolveAttribute(Resource.Attribute.layoutBackgroundSecondly, typeValue, true);
                var backgroundSecondaryColor = Resources.GetColor(typeValue.ResourceId);
                Theme.ResolveAttribute(Resource.Attribute.colorFirst, typeValue, true);
                var textPrimaryColor = Resources.GetColor(typeValue.ResourceId);
                switch (mode.ModeDefinition_idmodeDefinition)
                {
                    case 3:
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                        {
                            ModeTracking.SetBackgroundColor(backgroundSecondaryColor);
                            view.FindViewById<TextView>(Resource.Id.modeDefinition_trackingTitle).SetTextColor(textPrimaryColor);
                            view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_trackingImg).SetSvg(this, Resource.Drawable.ModeTracking, "62da73");
                        }
                        break;
                    case 4:
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                        {
                            ModeDontMove.SetBackgroundColor(backgroundSecondaryColor);
                            view.FindViewById<TextView>(Resource.Id.modeDefinition_dontMoveTitle).SetTextColor(textPrimaryColor);
                            view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_dontMoveImg).SetSvg(this, Resource.Drawable.ModeDontMove, "62da73");
                        }
                        break;
                    case 5:
                        if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                        {
                            ModeZone.SetBackgroundColor(backgroundSecondaryColor);
                            view.FindViewById<TextView>(Resource.Id.modeDefinition_zoneTitle).SetTextColor(textPrimaryColor);
                            view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_zoneImg).SetSvg(this, Resource.Drawable.ModeZone, "62da73");
                        }
                        break;
                    //case 6:
                    //    ModeInTime.SetBackgroundColor(backgroundSecondaryColor);
                    //    view.FindViewById<TextView>(Resource.Id.modeDefinition_inTimeTitle).SetTextColor(textPrimaryColor);
                    //    view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_inTimeImg).SetSvg(this, Resource.Drawable.ModeInTime, "62da73");
                    //    break;
                    //case 7:
                    //    ModeFollowMe.SetBackgroundColor(backgroundSecondaryColor);
                    //    view.FindViewById<TextView>(Resource.Id.modeDefinition_followMeTitle).SetTextColor(textPrimaryColor);
                    //    view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_followMeImg).SetSvg(this, Resource.Drawable.ModeFollowMe, "62da73");
                    //    break;
                    //case 8: break;
                    //case 9:
                    //    ModeDailyTrack.SetBackgroundColor(backgroundSecondaryColor);
                    //    view.FindViewById<TextView>(Resource.Id.modeDefinition_dailyTrackTitle).SetTextColor(textPrimaryColor);
                    //    view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_dailyTrackImg).SetSvg(this, Resource.Drawable.ModeDailyTrack, "62da73");
                    //    break;
                    //case 11:
                    //    ModeRoad.SetBackgroundColor(backgroundSecondaryColor);
                    //    view.FindViewById<TextView>(Resource.Id.modeDefinition_roadTitle).SetTextColor(textPrimaryColor);
                    //    view.FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDefinition_roadImg).SetSvg(this, Resource.Drawable.ModeRoad, "62da73");
                    //    break;
                    default:
                        break;
                }
            }

            listSeekiosBuilder.SetView(view);
            listSeekiosDialog = listSeekiosBuilder.Create();
            listSeekiosDialog.Show();
        }

        /// <summary>
        /// Choisir un mode
        /// </summary>
        /// <param name="idModeDefinition">identifiant du mode à choisir</param>
        private void SelectMode(int idModeDefinition)
        {
            App.Locator.ModeDefinition.SeekiosUpdated = _seekiosSelected;
            App.Locator.ModeZone.IsActivityFocused = true;
            var modeDefinitionEnum = (Enum.ModeDefinitionEnum)System.Enum.Parse(typeof(Enum.ModeDefinitionEnum), idModeDefinition.ToString());
            App.Locator.ModeDefinition.GoToMode(modeDefinitionEnum);
        }

        #endregion
    }
}