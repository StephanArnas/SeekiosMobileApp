using Android.App;
using Android.OS;
using SeekiosApp.Droid.Services;
using Android.Gms.Maps;
using SeekiosApp.Droid.ControlManager;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using SeekiosApp.ViewModel;
using SeekiosApp.Interfaces;
using System;
using Android.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Droid.Helper;
using System.Linq;
using XamSvg;
using Android.Views.Animations;
using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using SeekiosApp.Model.APP;
using System.Timers;
using Android.Content.Res;
using static Android.Views.Animations.Animation;
using SeekiosApp.Helper;
using Android.Content.PM;
using SeekiosApp.Extension;
using Java.Lang;
using static Android.Views.View;
using Android.Content;
using Android.Locations;

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "MapBaseActivity")]
    public abstract class MapBaseActivity : AppCompatActivityBase, IOnMapReadyCallback, IAnimationListener, IOnTouchListener
    {
        #region ===== Attributs ===================================================================

        protected MapControlManager _mapControlManager = null;
        protected IDispatchOnUIThread _dispatcherService = null;

        private Dialog _popupRefresh = null;
        private SeekiosDTO _seekiosSelected = null;
        private SeekiosOnDemand _seekiosOnDemand = null;
        protected SeekiosOnTracking _seekiosOnTracking = null;

        // popup elements
        private ProgressBar _thirdProgressBar;
        private SvgImageView _thirdValidateSvgImageView;
        private TextView _stepInformationTextView;
        private TextView _timerTextView;
        private TextView _titleTextView;

        protected bool _hasMapBeenCreated = false;
        protected bool _hasSeekiosWithPositions = true;

        // animations
        private Animation _toMiddleAnim;
        private Animation _fromMiddleAnim;

        //bottom layout
        private bool _isAllowToSlideUpAndDown = false;
        private int _bottomLayoutUpPosition = 0;
        private int _bottomLayoutDownPosition = 0;
        private bool _isGoingUp = true;

        #endregion

        #region ===== Properties ==================================================================

        public MapFragment MapFrag { get; set; }
        public SvgImageView CenterSeekiosSvgImageView { get; set; }
        public SvgImageView ChangeMapTypeSvgImageView { get; set; }
        public TextView RefreshInProgressButton { get; set; }
        public TextView ThirdLeftTextView { get; set; }
        public TextView ThirdRightTextView { get; set; }
        public MapWrapperLayout MapWrapperLayout { get; set; }
        public TextView TrackingTimerTextView { get; set; }
        public ProgressBar TrackingTimerProgressBar { get; set; }
        public SvgImageView InfoRefreshButton { get; set; }
        public LinearLayout InfoRefreshLayout { get; set; }
        public TextView TimerTextView { get; set; }
        public RelativeLayout CreditLayout { get; set; }
        public TextView NoPositionYetNotif { get; set; }
        public SvgImageView FirstLocationRefreshSvg { get; set; }
        public TextView RefreshFirstButton { get; set; }
        public TextView RefreshFirstText { get; set; }

        //Bottom Layout elements
        public RelativeLayout BottomLayout { get; set; }
        public RelativeLayout ArrowModeHistoricLayout { get; set; }
        public XamSvg.SvgImageView ArrowSvgImageView { get; set; }
        public XamSvg.SvgImageView ModeImageView { get; set; }
        public XamSvg.SvgImageView PowerSavingImageView { get; set; }
        public TextView ModeTextView { get; set; }
        public TextView HistoricTextView { get; set; }
        public TextView DeleteModeTextView { get; set; }
        public TextView FirstLeftTextView { get; set; }
        public TextView FirstRightTextView { get; set; }
        public TextView SecondRightTextView { get; set; }
        public RelativeLayout RootLayout { get; set; }

        #endregion

        #region ===== Life cycle ==================================================================

        protected void OnCreate(Bundle savedInstanceState
            , int idLayout
            , string actionBarTitle
            , Android.Support.V4.App.Fragment modeMetaDataFragment)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(idLayout);

            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
            _dispatcherService = ServiceLocator.Current.GetInstance<IDispatchOnUIThread>() as DispatchService;

            // prepare animation for switch between satellite and normal modes
            _toMiddleAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.to_middle);
            _fromMiddleAnim = AnimationUtils.LoadAnimation(this, Resource.Animation.from_middle);
            _toMiddleAnim.SetAnimationListener(this);
            _fromMiddleAnim.SetAnimationListener(this);

            GetObjectsFromView();
            SetDataToView(actionBarTitle);

            if (ToolbarPage != null)
            {
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            App.SeekiosChanged += App_SeekiosChanged;
            MapViewModelBase.OnSeekiosRefreshRequestSent += OnSeekiosRefreshRequestSent;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += OnSeekiosOutOfZoneNotified;
            App.Locator.ModeDontMove.SeekiosMovedNotified += OnSeekiosMovedNotified;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified += OnNewZoneTrackingLocationAdded;
            App.Locator.ModeDontMove.NewDontMoveTrackingLocationAddedNotified += OnNewDontMoveTrackingLocationAdded;
            App.Locator.ModeTracking.OnTrackingPositionAdded += OnNewTrackingPositionAdded;

            MapFrag.GetMapAsync(this);
            if (!(this is ModeZoneActivity))
            {
                if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                    && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    OnHasToShowNoPositionLayout(null, null);//visible true
                }
                else OnHasToHideNoPositionLayout(this, null);//visible false
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            App.SeekiosChanged -= App_SeekiosChanged;
            MapViewModelBase.OnSeekiosRefreshRequestSent -= OnSeekiosRefreshRequestSent;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified -= OnSeekiosOutOfZoneNotified;
            App.Locator.ModeDontMove.SeekiosMovedNotified -= OnSeekiosMovedNotified;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified -= OnNewZoneTrackingLocationAdded;
            App.Locator.ModeDontMove.NewDontMoveTrackingLocationAddedNotified -= OnNewDontMoveTrackingLocationAdded;
            App.Locator.ModeTracking.OnTrackingPositionAdded -= OnNewTrackingPositionAdded;
        }

        protected override void OnResume()
        {
            base.OnResume();
            InitializeMapType();
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            if (_mapControlManager != null) _mapControlManager.RegisterMethodes();
            if (ChangeMapTypeSvgImageView != null) ChangeMapTypeSvgImageView.Click += OnChangeMapTypeSvgImageViewClick;
            if (RefreshFirstButton != null) RefreshFirstButton.Click += RefreshFirstButton_Click;
        }

        protected override void OnPause()
        {
            base.OnPause();
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            if (_mapControlManager != null) _mapControlManager.UnregisterMethodes();
            if (ChangeMapTypeSvgImageView != null) ChangeMapTypeSvgImageView.Click -= OnChangeMapTypeSvgImageViewClick;
            if (RefreshFirstButton != null) RefreshFirstButton.Click -= RefreshFirstButton_Click;
            //App.Locator.Home.PropertyChanged -= OnNewTrackingData;
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            MapFrag = (MapFragment)FragmentManager.FindFragmentById(Resource.Id.mapBase_map);
            MapWrapperLayout = FindViewById<MapWrapperLayout>(Resource.Id.mapBase_layout);

            CenterSeekiosSvgImageView = FindViewById<SvgImageView>(Resource.Id.mapBase_showSeekios);
            ChangeMapTypeSvgImageView = FindViewById<SvgImageView>(Resource.Id.mapBase_changeType);
            RefreshInProgressButton = FindViewById<TextView>(Resource.Id.mapBase_inProgress);
            InfoRefreshButton = FindViewById<SvgImageView>(Resource.Id.mapBase_infoRefresh);
            InfoRefreshLayout = FindViewById<LinearLayout>(Resource.Id.mapBase_refreshLayout);
            TimerTextView = FindViewById<TextView>(Resource.Id.mapBase_timerInfo);

            TrackingTimerProgressBar = FindViewById<ProgressBar>(Resource.Id.mapTracking_progressBar);
            TrackingTimerTextView = FindViewById<TextView>(Resource.Id.mapTracking_timerTrackingText);

            NoPositionYetNotif = FindViewById<TextView>(Resource.Id.map_noPositionsNotif);
            RefreshFirstButton = FindViewById<TextView>(Resource.Id.mapBase_buttonRefreshFirst);
            RefreshFirstText = FindViewById<TextView>(Resource.Id.mapBase_textRefreshFirst);
            FirstLocationRefreshSvg = FindViewById<SvgImageView>(Resource.Id.mapBase_firstLocationSvgImage);

            SecondRightTextView = FindViewById<TextView>(Resource.Id.bottomLayout_secondTextRight);
        }

        private void SetDataToView(string actionBarTitle)
        {
            if (TrackingTimerProgressBar != null)
            {
                TrackingTimerProgressBar.Visibility = ViewStates.Gone;
                TrackingTimerTextView.Visibility = ViewStates.Gone;
            }

            ToolbarPage.Title = actionBarTitle;
            var color = "#bb62da73";    // TODO : centraliser dans un fichier ressource
            ToolbarPage.SetBackgroundColor(Android.Graphics.Color.ParseColor(color));

            if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
            {
                // Si le seekios est en dehors de la zone et qu'un tracking est configuré
                App.Locator.ModeTracking.InitMode();
            }

            InitializeMapType();
            SetTimerOnDemand(false);
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #endregion

        #region ===== Handlers ====================================================================

        #region BUTTON MAP

        private void InitializeMapType()
        {
            if (ChangeMapTypeSvgImageView != null && App.Locator.BaseMap.IsInNormalMode)
            {
                ChangeMapTypeSvgImageView.SetSvg(this, Resource.Drawable.icon_switchsatellite_android);
            }
            else if (ChangeMapTypeSvgImageView != null)
            {
                ChangeMapTypeSvgImageView.SetSvg(this, Resource.Drawable.icon_switchmap_android);
            }
            if (_mapControlManager != null) _mapControlManager.ChangeMapType(App.Locator.BaseMap.IsInNormalMode);
        }

        #endregion

        #region SEEKIOS ONDEMAND

        private async void RefreshFirstButton_Click(object sender, EventArgs e)
        {
            if (await App.Locator.BaseMap.RefreshSeekiosPosition())
            {
                FirstLocationRefreshSvg.SetSvg(this, Resource.Drawable.FirstLocation2);
                RefreshFirstButton.Enabled = false;
                RefreshFirstButton.SetText(Resource.String.locating);
                RefreshFirstText.SetText(Resource.String.firstRefreshInProgress);
            }
        }

        private void SetTimerOnDemand(bool needToDisplayPopupRefresh)
        {
            _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnDemand != null && TimerTextView != null)
            {
                // update the timer with the correct countdown (it's a security because sometimes it gets some secondes of delay)
                _seekiosOnDemand.Timer.CountDown = (_seekiosOnDemand.DateEndRefreshTimer - DateTime.Now).TotalSeconds;
                // display the timer and the right value
                TimerTextView.Visibility = ViewStates.Visible;
                InfoRefreshLayout.Visibility = ViewStates.Visible;
                InfoRefreshLayout.Click += OnRefreshInProgressButtonClick;
                int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                TimerTextView.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
                if (needToDisplayPopupRefresh) SetPopupTimerOnDemand(minutes, seconds);
                // display the right element when it's the first location
                if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                    && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    FirstLocationRefreshSvg.SetSvg(this, Resource.Drawable.FirstLocation2);
                    RefreshFirstButton.Enabled = false;
                    RefreshFirstButton.SetText(Resource.String.locating);
                    RefreshFirstText.SetText(Resource.String.firstRefreshInProgress);
                    InfoRefreshLayout.TranslationY = InfoRefreshLayout.TranslationY + 60;
                    TimerTextView.TranslationY = InfoRefreshLayout.TranslationY + 60;
                }
                // the UpdateUI is called at every ticks of the timer, update the count down
                _seekiosOnDemand.Timer.UpdateUI = () =>
                {
                    minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                    seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                    TimerTextView.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
                    if (_timerTextView != null) _timerTextView.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
                };
                _seekiosOnDemand.OnSuccess = DisplaySeekiosOnDemandOnSuccess;
                _seekiosOnDemand.OnFailed = DisplaySeekiosOnDemandOnFailed;
            }
        }

        private void SetPopupTimerOnDemand(int minutes, int seconds)
        {
            var refreshInProgressBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
            var view = inflater.Inflate(Resource.Drawable.PopupRefreshInProgress, null);

            _timerTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_timerText);
            _titleTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshinProgress_title);
            _thirdProgressBar = view.FindViewById<ProgressBar>(Resource.Id.popupRefreshInProgress_progressBarStep3);
            _thirdValidateSvgImageView = view.FindViewById<SvgImageView>(Resource.Id.popupRefreshInProgress_validateImageStep3);
            _stepInformationTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_stepText);

            _timerTextView.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
            _thirdProgressBar.Visibility = ViewStates.Visible;
            _thirdValidateSvgImageView.Visibility = ViewStates.Gone;
            _stepInformationTextView.Text = Resources.GetString(Resource.String.map_TextStep3);

            refreshInProgressBuilder.SetNegativeButton(Resource.String.map_closePopup, (senderAlert, args) =>
            {
                if (_popupRefresh != null) _popupRefresh.Dismiss();
            });

            refreshInProgressBuilder.SetView(view);
            _popupRefresh = refreshInProgressBuilder.Create();
            _popupRefresh.DismissEvent += PopupRefresh_DismissEvent;
            _popupRefresh.Show();

            view.Dispose();
            inflater.Dispose();
            refreshInProgressBuilder.Dispose();
        }

        private void OnRefreshInProgressButtonClick(object sender, EventArgs e)
        {
            if (_popupRefresh == null)
            {
                var refreshInProgressBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
                var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
                var view = inflater.Inflate(Resource.Drawable.PopupRefreshInProgress, null);

                //get the different elements
                _stepInformationTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_stepText);
                _timerTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_timerText);
                _titleTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshinProgress_title);
                _thirdProgressBar = view.FindViewById<ProgressBar>(Resource.Id.popupRefreshInProgress_progressBarStep3);
                _thirdValidateSvgImageView = view.FindViewById<SvgImageView>(Resource.Id.popupRefreshInProgress_validateImageStep3);
                _thirdProgressBar.Visibility = ViewStates.Visible;
                _thirdValidateSvgImageView.Visibility = ViewStates.Gone;
                _stepInformationTextView.Text = Resources.GetString(Resource.String.map_TextStep3);

                // cancel button
                refreshInProgressBuilder.SetNegativeButton(Resource.String.map_closePopup, (senderAlert, args) =>
                {
                    if (_popupRefresh != null) _popupRefresh.Dismiss();
                });
                refreshInProgressBuilder.SetView(view);
                _popupRefresh = refreshInProgressBuilder.Create();
                _popupRefresh.DismissEvent += PopupRefresh_DismissEvent;
            }
            _popupRefresh.Show();
        }

        private void PopupRefresh_DismissEvent(object sender, EventArgs e)
        {
            if (_popupRefresh != null)
            {
                _popupRefresh.Dismiss();
                _popupRefresh.Dispose();
                _popupRefresh = null;
            }
        }

        #endregion

        #region CHRONO TRACKING

        public void SetTimerTracking()
        {
            _seekiosOnTracking = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnTracking != null)
            {
                // Skip if there is no mode
                if (MapViewModelBase.Mode == null) return;

                // Skip if we are on the map mode zone, don't need to take care of the timer tracking
                if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone
                    && App.Locator.ModeZone.IsOnEditMode) return;

                // Skip if we are waiting for the mode to be activated
                if (!_seekiosSelected.HasGetLastInstruction
                    && _seekiosSelected.IsInPowerSaving
                    && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS) return;

                // Display the right UI elements
                TrackingTimerProgressBar.Visibility = ViewStates.Visible;
                TrackingTimerTextView.Visibility = ViewStates.Visible;

                //Change timer color if it's in alert
                TrackingTimerProgressBar.SetProgressDrawableTiled(
                    MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS
                    ? Resources.GetDrawable(Resource.Drawable.CircleProgressBar2Red)
                    : Resources.GetDrawable(Resource.Drawable.CircleProgressBar2));

                // Init timer
                DisplaySeekiosOnTrackingStarted();

                // Bind the callback timer
                _seekiosOnTracking.SetAndStartTimer();
                _seekiosOnTracking.Timer.UpdateUI += DisplaySeekiosOnTrackingUpdateUI;
            }
        }

        private void DisplaySeekiosOnTrackingUpdateUI()
        {
            var remainingTime = TimeSpan.FromSeconds(_seekiosOnTracking.Timer.CountDown < 0 ? 0 : _seekiosOnTracking.Timer.CountDown);
            var isLowerThanAnHour = (_seekiosOnTracking.Timer.CountDown < 3600);
            TrackingTimerTextView.Text = string.Format("{0:D2}:{1:D2}"
                , isLowerThanAnHour ? remainingTime.Minutes : remainingTime.Hours
                , isLowerThanAnHour ? remainingTime.Seconds : remainingTime.Minutes);
            TrackingTimerProgressBar.Progress = (int)_seekiosOnTracking.Timer.CountDown;
            InitBottomLayout(false);
        }

        private void DisplaySeekiosOnTrackingStarted()
        {
            TrackingTimerProgressBar.Max = _seekiosOnTracking.MaxRefreshTime;
            var remainingTime = TimeSpan.FromSeconds(_seekiosOnTracking.Timer.CountDown < 0 ? 0 : _seekiosOnTracking.Timer.CountDown);
            var isLowerThanAnHour = (_seekiosOnTracking.Timer.CountDown < 3600);
            TrackingTimerTextView.Text = string.Format("{0:D2}:{1:D2}"
                , isLowerThanAnHour ? remainingTime.Minutes : remainingTime.Hours
                , isLowerThanAnHour ? remainingTime.Seconds : remainingTime.Minutes);
            TrackingTimerProgressBar.Progress = (int)_seekiosOnTracking.Timer.CountDown;
        }

        #endregion

        #region HANDLE BOTTOM LAYOUT

        protected void InitBottomLayout(bool hasToMoveToBottom = true)
        {
            // Bouton Delete
            if (DeleteModeTextView == null || BottomLayout == null) return;
            DeleteModeTextView.SetText(Resource.String.map_deleteButton);
            // Place the layout
            if (hasToMoveToBottom) BottomLayout.SetY(_bottomLayoutDownPosition);

            // Init mode
            if (MapViewModelBase.Mode == null)
            {
                _isAllowToSlideUpAndDown = false;
                ArrowSvgImageView.Visibility = ViewStates.Gone;
                ModeImageView.Visibility = ViewStates.Gone;
                ModeTextView.Visibility = ViewStates.Gone;
                if (TrackingTimerProgressBar != null) TrackingTimerProgressBar.Visibility = ViewStates.Invisible;
            }
            else
            {
                ModeImageView.Visibility = ViewStates.Visible;
                ModeTextView.Visibility = ViewStates.Visible;

                if (_seekiosSelected.IsInPowerSaving) PowerSavingImageView.Visibility = ViewStates.Visible;
                else PowerSavingImageView.Visibility = ViewStates.Invisible;

                if (!_seekiosSelected.HasGetLastInstruction
                    && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios)
                    && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                    && !_seekiosSelected.IsRefreshingBattery
                    || !MapViewModelBase.Mode.DateModeActivation.HasValue)
                {
                    _isAllowToSlideUpAndDown = false;
                    ArrowSvgImageView.Visibility = ViewStates.Gone;
                    if (_seekiosSelected.IsInPowerSaving)
                    {
                        if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeTracking, "62da73=c8c8c8");
                        }
                        if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeDontMove, "62da73=c8c8c8");
                        }
                        else if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeZone, "62da73=c8c8c8");
                        }
                        ModeTextView.Text = string.Format(GetString(Resource.String.listSeekios_nextNoon), DateHelper.TimeLeftUntilNextNoon());
                    }
                    else
                    {
                        ModeImageView.SetSvg(this, Resource.Drawable.CloudSync);
                        ModeTextView.SetText(Resource.String.listSeekios_synchr);
                    }
                }
                else
                {
                    _isAllowToSlideUpAndDown = true;
                    ArrowSvgImageView.Visibility = ViewStates.Visible;

                    if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                    {
                        ModeImageView.SetSvg(this, Resource.Drawable.ModeTracking);
                        ModeTextView.SetText(Resource.String.modeDefinition_tracking);
                        FirstLeftTextView.SetText(Resource.String.map_firstTitle);
                        FirstRightTextView.Text = MapViewModelBase.Mode.DateModeActivation.HasValue
                                ? MapViewModelBase.Mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : MapViewModelBase.Mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                        ThirdRightTextView.Text = App.Locator.ModeTracking.LsLocations != null ? App.Locator.ModeTracking.LsLocations.Count.ToString() : "0";
                        ThirdRightTextView.Visibility = ViewStates.Visible;
                        ThirdLeftTextView.Visibility = ViewStates.Visible;
                    }
                    else if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                    {
                        if (App.Locator.Map.IsSeekiosInAlert)
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeDontMove, "62da73=da2e2e");
                            FirstLeftTextView.SetText(Resource.String.map_firstTitleAlert);
                            FirstRightTextView.Text = MapViewModelBase.Mode.LastTriggeredAlertDate.Value.ToLocalTime().FormatDateTimeFromNow(false);
                            ThirdRightTextView.Text = App.Locator.ModeDontMove.LsLocations != null ? App.Locator.ModeDontMove.LsLocations.Count.ToString() : "0";
                            ThirdRightTextView.Visibility = ViewStates.Visible;
                            ThirdLeftTextView.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            ModeImageView.SetSvg(this, (_seekiosSelected.IsInPowerSaving) ? Resource.Drawable.ModeDontMove : Resource.Drawable.ModeDontMove);
                            FirstLeftTextView.SetText(Resource.String.map_firstTitle);
                            FirstRightTextView.Text = MapViewModelBase.Mode.DateModeActivation.HasValue
                                ? MapViewModelBase.Mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : MapViewModelBase.Mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                            ThirdRightTextView.Visibility = ViewStates.Invisible;
                            ThirdLeftTextView.Visibility = ViewStates.Invisible;
                        }
                        ModeTextView.SetText(Resource.String.modeDefinition_dontmove);
                    }
                    else if (MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                    {
                        if (App.Locator.Map.IsSeekiosInAlert)
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeZone, "62da73=da2e2e");
                            FirstLeftTextView.SetText(Resource.String.map_firstTitleAlert);
                            FirstRightTextView.Text = MapViewModelBase.Mode.LastTriggeredAlertDate.Value.ToLocalTime().FormatDateTimeFromNow(false);
                            ThirdRightTextView.Text = App.Locator.ModeZone.LsLocations != null ? App.Locator.ModeZone.LsLocations.Count.ToString() : "0";
                            ThirdRightTextView.Visibility = ViewStates.Visible;
                            ThirdLeftTextView.Visibility = ViewStates.Visible;
                        }
                        else
                        {
                            ModeImageView.SetSvg(this, Resource.Drawable.ModeZone);
                            FirstLeftTextView.SetText(Resource.String.map_firstTitle);
                            FirstRightTextView.Text = MapViewModelBase.Mode.DateModeActivation.HasValue
                                ? MapViewModelBase.Mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : MapViewModelBase.Mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                            ThirdRightTextView.Visibility = ViewStates.Invisible;
                            ThirdLeftTextView.Visibility = ViewStates.Invisible;
                        }
                        ModeTextView.SetText(Resource.String.modeDefinition_zone);
                    }
                    SecondRightTextView.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(MapViewModelBase.Mode);
                }
            }
        }

        protected void GetLayoutSize()
        {
            BottomLayout.Post(new Runnable(() =>
            {
                //the rootsize is the size of the screen without the AppBar (where stands the battery info etc)
                //in order to move the bottomLayout at the right size we need to use this rootSize to place the layout
                int rootSize = RootLayout.Height;

                //Init bottom layout up and down position : value for Y
                _bottomLayoutDownPosition = rootSize - ArrowModeHistoricLayout.Height;
                _bottomLayoutUpPosition = rootSize - BottomLayout.Height;
                InitBottomLayout();
            }));
        }

        /// <summary>
        /// Handle Touch event
        /// </summary>
        //http://stackoverflow.com/questions/9398057/android-move-a-view-on-touch-move-action-move
        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            if (!_isAllowToSlideUpAndDown) return false;
            switch (e.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Down:
                    if (_isGoingUp && MapViewModelBase.Mode != null)
                    {
                        // Change arrow image
                        ArrowSvgImageView.SetSvg(this, Resource.Drawable.DownArrow);

                        // Animate the layout
                        v.Animate()
                           .Y(_bottomLayoutUpPosition)
                           .SetDuration(300)
                           .Start();
                        _isGoingUp = false;
                    }
                    else if (MapViewModelBase.Mode != null)
                    {
                        // Change arrow image
                        ArrowSvgImageView.SetSvg(this, Resource.Drawable.UpArrow);

                        // Animate the layout
                        v.Animate()
                           .Y(_bottomLayoutDownPosition)
                           .SetDuration(200)
                           .Start();
                        _isGoingUp = true;
                    }
                    break;
                case MotionEventActions.Up:
                    break;
                case MotionEventActions.PointerDown:
                    break;
                case MotionEventActions.PointerUp:
                    break;
                case MotionEventActions.Move:
                    break;
            }
            return true;
        }

        #endregion

        #endregion

        #region ===== Event =======================================================================

        #region GOOGLE MAP

        public virtual void OnMapReady(GoogleMap googleMap)
        {
            if (googleMap != null)
            {
                _hasMapBeenCreated = true;
                googleMap.SetPadding(0
                    , AccessResources.Instance.SizeOf70Dip(), 0
                    , AccessResources.Instance.SizeOf80Dip);
                if (App.Locator.BaseMap.IsInNormalMode) googleMap.MapType = GoogleMap.MapTypeNormal;
                else googleMap.MapType = GoogleMap.MapTypeSatellite;
            }
            if (!App.DeviceIsConnectedToInternet)
            {
                UpdateUIOnConnectionStateChanged(false);
            }
        }

        protected void InitCheckGoogleMap()
        {
            var timer = new Timer();
            timer.Interval = 1500;  //1.5 secondes devrait etre largement suffisant
            timer.AutoReset = false;
            timer.Start();
            timer.Elapsed += (object sender, ElapsedEventArgs e) => OnNoGoogleMapResponse(timer);
        }

        private void OnNoGoogleMapResponse(Timer timer)
        {
            timer.Stop();
            timer.Enabled = false;
            timer.Dispose();
            if (!_hasMapBeenCreated && _hasSeekiosWithPositions)
            {
                //TODO: ici on doit decider de quoi faire pour indiquer que l'on doit mettre a jour ou installer google services
                try
                {
                    RunOnUiThread(() =>
                    {
                        if (MapFrag.View != null) MapFrag.View.Visibility = ViewStates.Invisible;
                        if (NoPositionYetNotif != null)
                        {
                            ChangeMapTypeSvgImageView.Visibility = ViewStates.Invisible;
                            NoPositionYetNotif.Visibility = ViewStates.Visible;
                            var noGoogleServicesText = Resources.GetString(Resource.String.map_noGoogleMaps);
                            var textToDisplay = new Android.Text.SpannableString(noGoogleServicesText);
                            var clickButton = new ClickableSpan2();
                            clickButton.Click += o => StartGoogleServicesSettings(this);
                            var begin = noGoogleServicesText.IndexOf("Services Google");    //should check -1. but should always be the same...
                            var end = begin + "Services Google".Length;
                            textToDisplay.SetSpan(clickButton, begin, end, Android.Text.SpanTypes.InclusiveExclusive);
                            textToDisplay.SetSpan(new Android.Text.Style.ForegroundColorSpan(Android.Graphics.Color.Green), begin, end, Android.Text.SpanTypes.InclusiveExclusive);
                            NoPositionYetNotif.TextFormatted = textToDisplay;
                            NoPositionYetNotif.MovementMethod = new Android.Text.Method.LinkMovementMethod();
                        }
                        if (CenterSeekiosSvgImageView != null) CenterSeekiosSvgImageView.Visibility = ViewStates.Gone;
                    });
                }
                catch (System.Exception) { }
            }
        }

        public void StartGoogleServicesSettings(Android.Content.Context context)
        {
            {
                string[] schemes = { "package", "market", "https" };
                var schemeIndex = 0;
                var _package = "com.google.android.gms";
                var package = _package;
                {
                    try
                    {
                        var versionCode = PackageManager.GetPackageInfo(package, PackageInfoFlags.MetaData).VersionCode;
                        if (versionCode < 8705471) //not up to date enough
                        {
                            HandleBadGoogleServices(ref package, ref schemeIndex);
                        }
                    }
                    catch (System.Exception) //not found
                    {
                        HandleBadGoogleServices(ref package, ref schemeIndex);
                    }
                }
                var intent = new Android.Content.Intent();
                if (0 == schemeIndex) intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                var pack = Android.Net.Uri.FromParts(schemes[schemeIndex], package, null);
                intent.SetData(pack);
                try
                {
                    context.StartActivity(intent);
                }
                catch (System.Exception)   //the only case we cannot deal with is when playstore is installed but deactivated by the user.
                                           //in that case, we force the https link instead
                {
                    HandleBadGoogleServices(ref _package, ref schemeIndex, true);
                    Android.Content.Intent _intent = new Android.Content.Intent();
                    Android.Net.Uri _pack = Android.Net.Uri.FromParts(schemes[schemeIndex], _package, null);
                    _intent.SetData(_pack);
                    context.StartActivity(_intent);
                }
            }
        }

        public void HandleBadGoogleServices(ref string package, ref int schemeIndex, bool isStoreNotAvailable = false)
        {
            if (isStoreNotAvailable || !IsGooglePlayStoreAvaible())
            {
                package = "play.google.com/store/apps/details?id=" + package;
                schemeIndex = 2;
            }
            else
            {
                package = "details?id=" + package;
                schemeIndex = 1;
            }
        }

        public bool IsGooglePlayStoreAvaible()
        {
            try
            {
                PackageInfo packageInfo = PackageManager.GetPackageInfo("com.android.vending", PackageInfoFlags.MetaData);
                string verName = packageInfo.VersionName;
                int verCode = packageInfo.VersionCode;
                if (verCode < 0) return false;
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region IS INTERNET AVAILABLE

        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            if (_mapControlManager != null
                && _mapControlManager.SelectedMarker != null
                && _mapControlManager.SelectedMarker.IsInfoWindowShown)
            {
                _mapControlManager.SelectedMarker.HideInfoWindow();
                _mapControlManager.SelectedMarker.ShowInfoWindow();
            }
        }

        #endregion

        #region NAVIGATION

        private void HistoricButton_Click(object sender, EventArgs e)
        {
            App.Locator.Map.GoToHistoric();
        }

        #endregion

        #region REFRESH F5

        private void OnSeekiosRefreshRequestSent(object sender, EventArgs e)
        {
            if (_mapControlManager != null && _mapControlManager.SelectedMarker != null)
            {
                _mapControlManager.SelectedMarker.ShowInfoWindow();
            }
            SetTimerOnDemand(true);
        }

        private void DisplaySeekiosOnDemandOnSuccess()
        {
            //if one of the ui elements does not exist yet (page creation not finished)
            InfoRefreshLayout.Visibility = ViewStates.Gone;
            TimerTextView.Visibility = ViewStates.Gone;
            if (_timerTextView != null) _timerTextView.Visibility = ViewStates.Gone;
            if (_thirdProgressBar != null) _thirdProgressBar.Visibility = ViewStates.Gone;
            if (_thirdValidateSvgImageView != null) _thirdValidateSvgImageView.Visibility = ViewStates.Visible;
            if (_titleTextView != null) _titleTextView.Text = Resources.GetString(Resource.String.map_refreshEndedTitle);
            if (_stepInformationTextView != null) _stepInformationTextView.Text = Resources.GetString(Resource.String.map_refreshEndedContent);
            InfoRefreshLayout.Click -= OnRefreshInProgressButtonClick;
            OnHasToHideNoPositionLayout(this, null);

            if (_mapControlManager != null)
            {
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , DateTime.Now
                    , _seekiosSelected.LastKnownLocation_latitude
                    , _seekiosSelected.LastKnownLocation_longitude
                    , _seekiosSelected.LastKnownLocation_accuracy
                    , App.CurrentUserEnvironment.LsMode.Any(x => x.Seekios_idseekios == _seekiosSelected.Idseekios && x.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                    , App.CurrentUserEnvironment.LsMode.Any(x => x.Seekios_idseekios == _seekiosSelected.Idseekios && x.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS));

                if (_mapControlManager.SelectedMarker != null)
                {
                    _mapControlManager.SelectedMarker.ShowInfoWindow();
                }
            }
            _seekiosOnDemand = null;
        }

        private void DisplaySeekiosOnDemandOnFailed()
        {
            //update UI if it the first time the seekios is being located
            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                FirstLocationRefreshSvg.SetSvg(this, Resource.Drawable.FirstLocation3);
                RefreshFirstButton.Enabled = true;
                RefreshFirstButton.SetText(Resource.String.retry);
                RefreshFirstText.SetText(Resource.String.firstRefreshFailed);
            }
            if (_mapControlManager != null && _mapControlManager.SelectedMarker != null && (CurrentActivity.ActivityKey == App.MAP_PAGE))
            {
                try
                {
                    // sometimes it could crash : JniEnvironment+InstanceMethods.CallVoidMethod
                    // android.runtime.JavaProxyThrowable: System.ArgumentException: Handle must be valid.
                    // -- Java.Interop.JniEnvironment.InstanceMethods.CallVoidMethod(JniObjectReference instance, JniMethodInfo method)<c3080753ed8e425394afa7bee93551fe>:0
                    // -- Android.Runtime.JNIEnv.CallVoidMethod(IntPtr jobject, IntPtr jmethod)<ba500304b6e34ab093c445308ffd2df1>:0
                    // -- Android.Gms.Maps.Model.Marker.ShowInfoWindow()<e0758d0567d44648aba18d5f720111c9>:0
                    // -- SeekiosApp.Droid.View.MapBaseActivity.DisplaySeekiosOnDemandOnFailed() < afabc747850b47908dadba324416fa42 >:0
                    _mapControlManager.SelectedMarker.ShowInfoWindow();
                }
                catch (System.Exception) { }
            }
            if (App.Locator.BaseMap != null)
            {
                App.Locator.BaseMap.RefreshPositionStep = -1;
            }
            if (InfoRefreshLayout != null)
            {
                InfoRefreshLayout.Visibility = ViewStates.Gone;
                InfoRefreshLayout.Click -= OnRefreshInProgressButtonClick;
            }
            if (TimerTextView != null)
            {
                TimerTextView.Visibility = ViewStates.Gone;
            }
            if (_popupRefresh != null)
            {
                if (!_popupRefresh.IsShowing)
                {
                    _popupRefresh.Show();
                }
                if (_thirdProgressBar != null) _thirdProgressBar.Visibility = ViewStates.Gone;
                if (_stepInformationTextView != null) _stepInformationTextView.Text = Resources.GetString(Resource.String.map_refreshFailedSubTitle);
                if (_thirdValidateSvgImageView != null)
                {
                    _thirdValidateSvgImageView.SetSvg(this, Resource.Drawable.RoundedDelete);
                    _thirdValidateSvgImageView.Visibility = ViewStates.Visible;
                }
                if (_thirdProgressBar != null) { _titleTextView.Text = Resources.GetString(Resource.String.map_refreshFailedTitle); }
                if (_timerTextView != null) _timerTextView.Visibility = ViewStates.Invisible;
            }
            _seekiosOnDemand = null;
        }

        #endregion

        #region LAYOUT

        protected virtual void OnHasToShowNoPositionLayout(object sender, object args)
        {
            if (null != NoPositionYetNotif)
            {
                NoPositionYetNotif.Text = Resources.GetString(Resource.String.map_noPositionsYetMessage);
                NoPositionYetNotif.Visibility = ViewStates.Visible;
            }
            if (null != CenterSeekiosSvgImageView) CenterSeekiosSvgImageView.Visibility = ViewStates.Gone;
            if (null != ChangeMapTypeSvgImageView) ChangeMapTypeSvgImageView.Visibility = ViewStates.Gone;
            if (null != HistoricTextView) HistoricTextView.Visibility = ViewStates.Gone;

            MapFrag.View.Visibility = ViewStates.Invisible;//hides the map
            _hasSeekiosWithPositions = false;
            if (RefreshFirstButton != null) RefreshFirstButton.Visibility = ViewStates.Visible;
            if (RefreshFirstText != null) RefreshFirstText.Visibility = ViewStates.Visible;
        }

        protected virtual void OnHasToHideNoPositionLayout(object sender, object args)
        {
            if (NoPositionYetNotif != null) NoPositionYetNotif.Visibility = ViewStates.Gone;
            if (CenterSeekiosSvgImageView != null) CenterSeekiosSvgImageView.Visibility = ViewStates.Visible;
            if (MapFrag != null && MapFrag.View != null) MapFrag.View.Visibility = ViewStates.Visible;//hides the map
            if (null != ChangeMapTypeSvgImageView) ChangeMapTypeSvgImageView.Visibility = ViewStates.Visible;
            if (null != FirstLocationRefreshSvg) FirstLocationRefreshSvg.Visibility = ViewStates.Gone;
            if (null != HistoricTextView) HistoricTextView.Visibility = ViewStates.Visible;
            _hasSeekiosWithPositions = true;
            if (RefreshFirstButton != null) RefreshFirstButton.Visibility = ViewStates.Gone;
            if (RefreshFirstText != null) RefreshFirstText.Visibility = ViewStates.Gone;
            if (!_hasMapBeenCreated && sender != null)
            {
                // gere le non affichage de la map (quand pas de google services)
                InitCheckGoogleMap();
            }
        }

        private void OnChangeMapTypeSvgImageViewClick(object sender, EventArgs e)
        {
            if (_mapControlManager != null)
            {
                ChangeMapTypeSvgImageView.Enabled = false;
                App.Locator.BaseMap.IsInNormalMode = !App.Locator.BaseMap.IsInNormalMode;
                _mapControlManager.ChangeMapType(App.Locator.BaseMap.IsInNormalMode);
                ChangeMapTypeSvgImageView.ClearAnimation();
                ChangeMapTypeSvgImageView.StartAnimation(_toMiddleAnim);
            }
        }

        public void OnAnimationEnd(Animation animation)
        {
            if (animation == _toMiddleAnim)
            {
                if (App.Locator.BaseMap.IsInNormalMode)
                {
                    ChangeMapTypeSvgImageView.SetSvg(this, Resource.Drawable.icon_switchsatellite_android);
                }
                else ChangeMapTypeSvgImageView.SetSvg(this, Resource.Drawable.icon_switchmap_android);
                ChangeMapTypeSvgImageView.ClearAnimation();
                ChangeMapTypeSvgImageView.StartAnimation(_fromMiddleAnim);
            }
            else
            {
                ChangeMapTypeSvgImageView.Enabled = true;
            }
        }

        public void OnAnimationRepeat(Animation animation) { }

        public void OnAnimationStart(Animation animation) { }

        private void App_SeekiosChanged(object sender, int e)
        {
            _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == _seekiosSelected.Idseekios);
            if (!_isGoingUp) InitBottomLayout(true);
            else InitBottomLayout();
        }

        #endregion

        #region MODE TRACKING

        private void OnNewTrackingPositionAdded(int idSeekios
           , double lat
            , double lon
            , double alt
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (_mapControlManager != null)
            {
                if (MapFrag.View.Visibility == ViewStates.Invisible)
                {
                    OnHasToHideNoPositionLayout(this, null);//if map is hiden, load the map
                }
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , false
                    , false);
                InitBottomLayout(false);
                RunOnUiThread(() =>
                {
                    App.Locator.ModeTracking.InitTrackingRoute();
                });
            }
        }

        #endregion

        #region MODE ZONE

        private void OnSeekiosOutOfZoneNotified(int idSeekios
            , double lat
            , double lon
            , double alt
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (MapFrag.View.Visibility == ViewStates.Invisible)
            {
                //if map is hiden, load the map
                OnHasToHideNoPositionLayout(this, null);
            }
            if (_mapControlManager != null)
            {
                _mapControlManager.SetZoneInAlert(true);
                //Creation of a new marker with a red color
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , false
                    , true);
            }
            InitBottomLayout(false);
        }

        private void OnNewZoneTrackingLocationAdded(int idSeekios
            , double lat
            , double lon
            , double alt
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (MapFrag.View.Visibility == ViewStates.Invisible)
            {
                //if map is hiden, load the map
                OnHasToHideNoPositionLayout(this, null);
            }
            if (_mapControlManager != null && MapViewModelBase.Seekios != null)
            {
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , true
                    , true);
            }
            InitBottomLayout(false);
            App.Locator.ModeZone.InitZoneTrackingRouteAsync();
        }

        #endregion

        #region MODE DONT MOVE

        private void OnSeekiosMovedNotified(int idSeekios)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (MapFrag.View.Visibility == ViewStates.Invisible)
            {
                //if map is hiden, load the map
                OnHasToHideNoPositionLayout(this, null);
            }
            if (null != _mapControlManager)
            {
                // creation of a new marker with a red color
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , _seekiosSelected.DateLastCommunication
                    , _seekiosSelected.LastKnownLocation_latitude
                    , _seekiosSelected.LastKnownLocation_longitude
                    , _seekiosSelected.LastKnownLocation_accuracy
                    , true
                    , true);
            }
            InitBottomLayout(false);
        }

        private void OnNewDontMoveTrackingLocationAdded(int idSeekios
            , double lat
            , double lon
            , double altitude
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (MapFrag.View.Visibility == ViewStates.Invisible)
            {
                //if map is hiden, load the map
                OnHasToHideNoPositionLayout(this, null);
            }
            if (_mapControlManager != null && MapViewModelBase.Seekios != null)
            {
                _mapControlManager.RemoveMarker(_seekiosSelected.Idseekios.ToString());
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , true
                    , true);
                _mapControlManager.CenterInMarker(_seekiosSelected.Idseekios.ToString());
            }
            InitBottomLayout(false);
            App.Locator.ModeDontMove.InitDontMoveTrackingRouteAsync();
        }

        #endregion

        #endregion

        #region ===== Select a mode ===============================================================

        public void DisplayModeSelection()
        {
            Dialog moreDialog = null;
            var moreBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
            var inflater = (LayoutInflater)this.GetSystemService(LayoutInflaterService);
            var view = inflater.Inflate(Resource.Drawable.PopupMore, null);

            // Close button
            moreBuilder.SetNegativeButton(Resource.String.detailSeekios_deletePopupButtonClose, (senderAlert, args) => { moreDialog.Dismiss(); });

            string adressResult = string.Empty;

            System.Threading.Tasks.Task.Factory.StartNew(async () =>
            {
                try
                {
                    var geocoder = new Geocoder(Application.Context
                        , Java.Util.Locale.ForLanguageTag(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
                    var result = await geocoder.GetFromLocationAsync(
                        MapViewModelBase.Seekios.LastKnownLocation_latitude
                        , MapViewModelBase.Seekios.LastKnownLocation_longitude, 1);
                    var address = result.FirstOrDefault();
                    if (address != null)
                    {
                        RunOnUiThread(() =>
                        {
                            if (!string.IsNullOrEmpty(address.GetAddressLine(0))) adressResult += address.GetAddressLine(0) + ", ";
                            if (!string.IsNullOrEmpty(address.Locality)) adressResult += address.Locality + ", ";
                            if (!string.IsNullOrEmpty(address.AdminArea)) adressResult += address.AdminArea + ", ";
                            if (!string.IsNullOrEmpty(address.PostalCode)) adressResult += address.PostalCode + ", ";
                            if (!string.IsNullOrEmpty(address.CountryName)) adressResult += address.CountryName;
                            var AdressTextView = view.FindViewById<TextView>(Resource.Id.more_address);
                            AdressTextView.Text = adressResult;
                        });
                    }
                }
                catch (System.Exception ex)
                {
                }
            });

            // Select Navigate to the seekios position 
            var navigateToLayout = view.FindViewById<LinearLayout>(Resource.Id.more_navigateTo);
            navigateToLayout.Click += (s, arg) =>
            {
                var mapIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(
                string.Format("geo:0,0?q={0},{1}(seekios: {2})"
                    , MapViewModelBase.Seekios.LastKnownLocation_latitude.ToString().Replace(',', '.')
                    , MapViewModelBase.Seekios.LastKnownLocation_longitude.ToString().Replace(',', '.')
                    , MapViewModelBase.Seekios.SeekiosName)));
                CurrentActivity.StartActivity(mapIntent);
                mapIntent.Dispose();
            };

            // Select Share position
            var shareLayout = view.FindViewById<LinearLayout>(Resource.Id.more_share);
            shareLayout.Click += (s, arg) =>
            {
                var shareIntent = new Intent(Intent.ActionSend);
                shareIntent.SetType("text/plain");
                shareIntent.PutExtra(Intent.ExtraText, StringHelper.GoogleMapLinkShare(
                    MapViewModelBase.Seekios.LastKnownLocation_latitude,
                    MapViewModelBase.Seekios.LastKnownLocation_longitude));
                shareIntent.PutExtra(Intent.ExtraSubject, "Coordonnées du seekios partagées");
                CurrentActivity.StartActivity(shareIntent);
                shareIntent.Dispose();
            };

            // Select Copy Adress in memory
            var copyAdress = view.FindViewById<LinearLayout>(Resource.Id.more_copyAdress);
            copyAdress.Click += (s, arg) =>
            {
                var clipboard = (ClipboardManager)GetSystemService(ClipboardService);
                clipboard.Text = adressResult;
                clipboard.Dispose();
                moreDialog.Dismiss();
            };

            moreBuilder.SetView(view);
            moreDialog = moreBuilder.Create();
            moreDialog.Show();
        }

        #endregion
    }

    public class ClickableSpan2 : Android.Text.Style.ClickableSpan
    {
        public Action<Android.Views.View> Click;

        public override void OnClick(Android.Views.View widget)
        {
            Click?.Invoke(widget);
        }
    }
}