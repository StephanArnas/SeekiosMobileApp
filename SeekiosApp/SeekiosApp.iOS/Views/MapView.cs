using System;
using UIKit;
using MapKit;
using System.Linq;
using SeekiosApp.Model.DTO;
using SeekiosApp.ViewModel;
using SeekiosApp.iOS.Helper;
using SeekiosApp.iOS.Views.BaseView;
using SeekiosApp.iOS.ControlManager;
using SeekiosApp.Enum;
using SeekiosApp.Model.APP;
using System.Threading.Tasks;
using DACircularProgress;
using CoreGraphics;
using Foundation;
using SeekiosApp.Extension;

namespace SeekiosApp.iOS
{
    public partial class MapView : BaseViewControllerMap
    {
        #region ===== Attributs ===================================================================

        private bool _firstInitilise = false;

        private UIAlertController _popupRefresh = null;
        private UIButton _timerButton;
        private LabeledCircularProgressView _progressView = null;
        private SeekiosDTO _seekiosSelected = null;
        private ModeDTO _modeSelected = null;
        private SeekiosOnDemand _seekiosOnDemand = null;
        private SeekiosOnTracking _seekiosOnTracking = null;

        private bool _isBottomViewOpen = false;
        private bool _isAllowToSlideUpAndDown = false;
        private DateTime _lastBottomViewOpen = DateTime.Now;
        private UITapGestureRecognizer _tapSlideUpAndDown = null;
        private UIPanGestureRecognizer _panSlideUpAndDown = null;
        NSLayoutConstraint _constraintYUp = null;
        NSLayoutConstraint _constraintYDown = null;
        CGAffineTransform _arrowTransform;

        #endregion

        #region ===== Properties ==================================================================

        public static bool IsRefreshEnable { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public MapView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _seekiosSelected = App.Locator.DetailSeekios.SeekiosSelected;
            _modeSelected = App.CurrentUserEnvironment.LsMode.FirstOrDefault(m => m.Seekios_idseekios == _seekiosSelected.Idseekios);

            MapViewModelBase.OnSeekiosRefreshRequestSent += OnSeekiosRefreshRequestSent;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += OnSeekiosOutOfZoneNotified;
            App.Locator.ModeDontMove.SeekiosMovedNotified += OnSeekiosMovedNotified;
            App.Locator.ModeTracking.OnTrackingPositionAdded += OnNewTrackingPositionAdded;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified += OnNewZoneTrackingLocationAdded;
            App.Locator.ModeDontMove.NewDontMoveTrackingLocationAddedNotified += OnNewDontMoveTrackingLocationAdded;
            RefreshInProgressButton.TouchUpInside += RefreshInProgressButton_TouchUpInside;
            DeleteModeButton.TouchUpInside += DeleteMode_Click;
            HistoricButton.TouchUpInside += HistoricButton_Click;
            DisplayTutoButton.TouchUpInside += DisplayTutoButton_TouchUpInside;
            App.SeekiosChanged += RaiseSeekiosChanged;

            if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
            {
                _firstInitilise = true;
            }
            if (_firstInitilise)
            {
                MapViewControl.Hidden = true;
                RefreshPosition.Hidden = false;
                LblRefreshPositionText.Hidden = false;
                FirstRefresh1Image.Hidden = false;
                DisplayTutoButton.Hidden = false;
                StepSeekiosLabel.Hidden = false;
                ChangeMapTypeButton.Hidden = true;
                FocusOnSeekiosButton.Hidden = true;
                HistoricButton.Hidden = true;
                RefreshPosition.TouchUpInside += RefreshOnInitialize;
            }
            else
            {
                MapViewControl.Hidden = false;
                RefreshPosition.Hidden = true;
                LblRefreshPositionText.Hidden = true;
                FirstRefresh1Image.Hidden = true;
                DisplayTutoButton.Hidden = true;
                StepSeekiosLabel.Hidden = true;
                ChangeMapTypeButton.Hidden = false;
            }
            TimerLabel.Hidden = true;
            RefreshInProgressButton.Hidden = true;

            if (_modeSelected != null) _isAllowToSlideUpAndDown = true;
            _arrowTransform = ArrowImage.Transform;
            _tapSlideUpAndDown = new UITapGestureRecognizer(BottomView_Tap);
            _panSlideUpAndDown = new UIPanGestureRecognizer(BottomView_Tap);
            BottomView.AddGestureRecognizer(_tapSlideUpAndDown);
            BottomView.AddGestureRecognizer(_panSlideUpAndDown);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            BecomeActiveAction = () => BecomeActiveNotifcationCallback();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            MapViewModelBase.OnSeekiosRefreshRequestSent -= OnSeekiosRefreshRequestSent;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified -= OnSeekiosOutOfZoneNotified;
            App.Locator.ModeDontMove.SeekiosMovedNotified -= OnSeekiosMovedNotified;
            App.Locator.ModeTracking.OnTrackingPositionAdded -= OnNewTrackingPositionAdded;
            App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified -= OnNewZoneTrackingLocationAdded;
            App.Locator.ModeDontMove.NewDontMoveTrackingLocationAddedNotified -= OnNewDontMoveTrackingLocationAdded;
            RefreshInProgressButton.TouchUpInside -= RefreshInProgressButton_TouchUpInside;
            DeleteModeButton.TouchUpInside -= DeleteMode_Click;
            HistoricButton.TouchUpInside -= HistoricButton_Click;
            DisplayTutoButton.TouchUpInside -= DisplayTutoButton_TouchUpInside;
            App.SeekiosChanged -= RaiseSeekiosChanged;

            if (MapViewControl != null)
            {
                MapViewControl.MapType = MKMapType.Hybrid;
                MapViewControl.Delegate = null;
                MapViewControl.RemoveFromSuperview();
                MapViewControl.Dispose();
                MapViewControl = null;
            }
            _tapSlideUpAndDown.Dispose();
            _panSlideUpAndDown.Dispose();
            _seekiosSelected = null;
            _seekiosOnDemand = null;
            _seekiosOnTracking = null;
            _modeSelected = null;
            if (_timerButton != null) _timerButton.Dispose();
            _timerButton = null;
            if (_progressView != null) _progressView.Dispose();
            _progressView = null;
            if (_popupRefresh != null) _popupRefresh.Dispose();
            _popupRefresh = null;

            RefreshPosition.TouchUpInside -= RefreshOnInitialize;
            RefreshPosition.Dispose();
            LblRefreshPositionText.Dispose();
            FirstRefresh1Image.Dispose();
            DisplayTutoButton.Dispose();
            StepSeekiosLabel.Dispose();
            ChangeMapTypeButton.Dispose();
            FocusOnSeekiosButton.Dispose();
            HistoricButton.Dispose();
            TimerLabel.Dispose();
            RefreshInProgressButton.Dispose();
            ChronoWaitForStartLabel.Dispose();
            DeleteModeButton.Dispose();
            FirstLeftLabel.Dispose();
            SecondLeftLabel.Dispose();
            ModeImage.Dispose();
            ModeLabel.Dispose();
            ModePowerSavingImage.Dispose();
            ArrowImage.Dispose();
            FirstRightLabel.Dispose();
            SecondRightLabel.Dispose();
            ThirdLeftLabel.Dispose();
            ThirdRightLabel.Dispose();
            FocusOnZoneButton.Dispose();
            BottomHeadView.Dispose();
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();
            if (_firstInitilise) TimerLabel.Center = new CGPoint(View.Frame.Width / 2, TimerLabel.Frame.Y + 60);
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            // Initialise all strings
            Title = Application.LocalizedString("MapTitle");
            StepSeekiosLabel.Text = Application.LocalizedString("SeekiosFirstSetupExplanation");
            InitialiseAllStrings();

            // Init UI
            InitialiseUIElements();

            // Init timers / map / bottom view
            SetTimerOnDemand();
            InitialiseBottomView(_modeSelected);
        }

        public override void SetMapControlManager()
        {
            if (_mapControlManager == null)
            {
                _mapControlManager = new MapControlManager(MapViewControl
                    , this
                    , FocusOnSeekiosButton
                    , FocusOnZoneButton
                    , ChangeMapTypeButton
                    , MapZoomInButton
                    , MapZoomOutButton
                    , MapViewModelBase.Seekios.Idseekios.ToString());

                if (MapViewModelBase.Mode != null
                    && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                {
                    if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1) MapControlManager.IsOutOf = true;
                    App.Locator.ModeDontMove.MapControlManager = _mapControlManager;
                    App.Locator.ModeDontMove.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                    App.Locator.ModeDontMove.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                    App.Locator.ModeDontMove.InitMap();
                    App.Locator.ModeDontMove.InitModeDontMove();
                    if (App.Locator.Map.LsSeekiosOnTracking.Any(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                    {
                        Task.Factory.StartNew(() => { App.Locator.ModeDontMove.InitDontMoveTrackingRouteAsync(); });
                    }
                }
                else if (MapViewModelBase.Mode != null
                    && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                {
                    if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1) MapControlManager.IsOutOf = true;
                    App.Locator.ModeZone.MapControlManager = _mapControlManager;
                    App.Locator.ModeZone.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                    App.Locator.ModeZone.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                    App.Locator.ModeZone.InitMap();
                    _mapControlManager.CreateZone(App.Locator.ModeZone.DecodeTrame(MapViewModelBase.Mode.Trame)
                        , MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1);
                    if (App.Locator.Map.LsSeekiosOnTracking.Any(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                    {
                        Task.Factory.StartNew(() => { App.Locator.ModeZone.InitZoneTrackingRouteAsync(); });
                    }
                    FocusOnZoneButton.Hidden = false;
                }
                else if (MapViewModelBase.Mode != null
                    && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    MapControlManager.IsOutOf = false;
                    App.Locator.ModeTracking.MapControlManager = _mapControlManager;
                    App.Locator.ModeTracking.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;
                    App.Locator.ModeTracking.MapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;
                    App.Locator.ModeTracking.InitMap();
                    App.Locator.ModeTracking.InitMode();
                    Task.Factory.StartNew(() => { App.Locator.ModeTracking.InitTrackingRoute(); });
                }
                else
                {
                    MapControlManager.IsOutOf = false;
                    App.Locator.Map.MapControlManager = _mapControlManager;
                    App.Locator.Map.InitMap();
                }
                SetZoom();
                _mapControlManager.RegisterMethodes();
                ModeZoneFirstView.IsOnModeZone = false;
            }
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void SetTimer(object sender, EventArgs e)
        {
            if (sender != null)
            {
                _timerButton = (UIKit.UIButton)sender;
                _timerButton.UserInteractionEnabled = false;
                _timerButton.Enabled = false;
            }
            else _timerButton = null;
        }

        public void SetView()
        {
            if (_firstInitilise)
            {
                LblRefreshPositionText.Text = Application.LocalizedString("FirstLocationAsked");
                FirstRefresh1Image.Image = UIImage.FromBundle("FirstRefresh-2");
                StepSeekiosLabel.Hidden = false;
                RefreshPosition.SetTitle(Application.LocalizedString("RefreshButton"), UIControlState.Normal);
            }
        }

        #endregion

        #region ===== Private Methods =============================================================

        private void InitialiseAllStrings()
        {
            ChronoWaitForStartLabel.Text = Application.LocalizedString("WaitingForPosition");
            RefreshInProgressButton.SetTitle(Application.LocalizedString("InProgress"), UIControlState.Normal);
            LblRefreshPositionText.Text = Application.LocalizedString("NoFirstPositionExplanation");
            StepSeekiosLabel.Text = Application.LocalizedString("StepOnePowerOn");
            RefreshPosition.SetTitle(Application.LocalizedString("UpdatePosition"), UIControlState.Normal);
            DeleteModeButton.SetTitle(Application.LocalizedString("DeleteModeMap"), UIControlState.Normal);
            FirstLeftLabel.Text = Application.LocalizedString("ActiveSince");
            SecondLeftLabel.Text = Application.LocalizedString("TrackingIfAlert");
            ThirdLeftLabel.Text = Application.LocalizedString("NumberOfPosition");
        }

        private void InitialiseBottomView(ModeDTO mode)
        {
            if (mode == null)
            {
                _isAllowToSlideUpAndDown = false;
                ArrowImage.Hidden = true;
                ModeImage.Hidden = true;
                ModeLabel.Hidden = true;
                ModePowerSavingImage.Hidden = true;
                if (_progressView != null) _progressView.Hidden = true;
            }
            else
            {
                ModeImage.Hidden = false;
                ModeLabel.Hidden = false;

                if (_seekiosSelected.IsInPowerSaving) ModePowerSavingImage.Hidden = false;
                else ModePowerSavingImage.Hidden = true;

                if (!_seekiosSelected.HasGetLastInstruction
                    && !App.Locator.Map.LsSeekiosOnDemand.Any(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios)
                    && mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS
                    && !_seekiosSelected.IsRefreshingBattery
                    || !mode.DateModeActivation.HasValue)
                {
                    _isAllowToSlideUpAndDown = false;
                    ArrowImage.Hidden = true;

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
                        if (_progressView != null) _progressView.Hidden = true;
                        ChronoWaitForStartLabel.Hidden = true;
                    }
                    else
                    {
                        ModeImage.Image = UIImage.FromBundle("InProgress");
                        ModeLabel.Text = Application.LocalizedString("InProgress");
                    }
                }
                else
                {
                    _isAllowToSlideUpAndDown = true;
                    ArrowImage.Hidden = false;
                    var isInAlert = mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved
                            || mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone;
                    ThirdLeftLabel.Hidden = !isInAlert;
                    ThirdRightLabel.Hidden = !isInAlert;
                    if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                    {
                        ModeImage.Image = UIImage.FromBundle("ModeTracking");
                        ModeLabel.Text = Application.LocalizedString("ModeTrackingTitle");
                        FirstRightLabel.Text = mode.DateModeActivation.HasValue
                                ? mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                        SecondRightLabel.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(mode);
                        ThirdLeftLabel.Hidden = false;
                        ThirdRightLabel.Hidden = false;
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                    {
                        if (isInAlert)
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeDontMoveAlert");
                            FirstLeftLabel.Text = Application.LocalizedString("ActiveSinceInAlert");
                            FirstRightLabel.Text = mode.LastTriggeredAlertDate.Value.ToLocalTime().FormatDateTimeFromNow(false);
                            SecondRightLabel.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(mode);
                            ThirdRightLabel.Text = App.Locator.ModeDontMove.LsLocations.Count.ToString();
                        }
                        else
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeDontMove");
                            FirstLeftLabel.Text = Application.LocalizedString("ActiveSince");
                            FirstRightLabel.Text = mode.DateModeActivation.HasValue
                                ? mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                            SecondRightLabel.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(mode);
                        }
                        ModeLabel.Text = Application.LocalizedString("ModeDontMove");
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                    {
                        if (isInAlert)
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeZoneAlert");
                            FirstLeftLabel.Text = Application.LocalizedString("ActiveSinceInAlert");
                            FirstRightLabel.Text = mode.LastTriggeredAlertDate.Value.ToLocalTime().FormatDateTimeFromNow(false);
                            SecondRightLabel.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(mode);
                            ThirdRightLabel.Text = App.Locator.ModeZone.LsLocations.Count.ToString();
                        }
                        else
                        {
                            ModeImage.Image = UIImage.FromBundle("ModeZone");
                            FirstLeftLabel.Text = Application.LocalizedString("ActiveSince");
                            FirstRightLabel.Text = mode.DateModeActivation.HasValue
                                ? mode.DateModeActivation.Value.ToLocalTime().FormatDateTimeFromNow(false)
                                : mode.DateModeCreation.ToLocalTime().FormatDateTimeFromNow(false);
                            SecondRightLabel.Text = App.Locator.ModeSelection.DisplayModeRefreshTimeTracking(mode);
                        }
                        ModeLabel.Text = Application.LocalizedString("ModeZone");
                    }
                }
            }
        }

        private void InitialiseUIElements()
        {
            // formatting UI elements
            RefreshInProgressButton.Layer.CornerRadius = 4;
            RefreshInProgressButton.Layer.MasksToBounds = true;
            RefreshPosition.Layer.CornerRadius = 4;
            RefreshPosition.Layer.MasksToBounds = true;
            HistoricButton.Layer.CornerRadius = 4;
            HistoricButton.Layer.MasksToBounds = true;
            HistoricButton.ContentEdgeInsets = new UIEdgeInsets(0, 5, 0, 5);
            DeleteModeButton.Layer.CornerRadius = 4;
            DeleteModeButton.Layer.MasksToBounds = true;
            DeleteModeButton.ContentEdgeInsets = new UIEdgeInsets(0, 5, 0, 5);
            StepSeekiosLabel.TextColor = UIColor.FromRGB(255, 76, 57);
            FocusOnZoneButton.Hidden = true;
            ChronoWaitForStartLabel.Hidden = true;
        }

        #region TIMER ONDEMAND

        private void SetTimerOnDemand()
        {
            _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnDemand != null)
            {
                IsRefreshEnable = false;
                // update the timer with the correct countdown (it's a security because sometimes it gets some secondes of delay)
                _seekiosOnDemand.Timer.CountDown = (_seekiosOnDemand.DateEndRefreshTimer - DateTime.Now).TotalSeconds;
                // display the timer and the right value
                TimerLabel.Hidden = false;
                int minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                int seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                TimerLabel.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
                // display the right element when it's the first location
                if (_firstInitilise)
                {
                    RefreshPosition.Enabled = false;
                    RefreshInProgressButton.Hidden = true;
                    LblRefreshPositionText.Text = Application.LocalizedString("FirstLocationAsked");
                    FirstRefresh1Image.Image = UIImage.FromBundle("FirstRefresh-2");
                }
                else RefreshInProgressButton.Hidden = false;
                // event
                RefreshInProgressButton.TouchUpInside += RefreshInProgressButton_TouchUpInside;
                // the UpdateUI is called at every ticks of the timer, update the count down
                _seekiosOnDemand.Timer.UpdateUI = () =>
                {
                    minutes = (int)_seekiosOnDemand.Timer.CountDown / 60;
                    seconds = (int)_seekiosOnDemand.Timer.CountDown - (minutes * 60);
                    TimerLabel.Text = string.Format("{00:00}:{01:00}", minutes, seconds);
                };
                _seekiosOnDemand.Timer.Stopped = DisplaySeekiosOnDemandTimerStopped;
                _seekiosOnDemand.OnSuccess = DisplaySeekiosOnDemandOnSuccess;
                _seekiosOnDemand.OnFailed = DisplaySeekiosOnDemandOnFailed;
            }
            else IsRefreshEnable = true;
        }

        private void DisplayMapAndHideFirstLocation()
        {
            MapViewControl.Hidden = false;
            RefreshPosition.Hidden = true;
            LblRefreshPositionText.Hidden = true;
            FirstRefresh1Image.Hidden = true;
            DisplayTutoButton.Hidden = true;
            StepSeekiosLabel.Hidden = true;
            ChangeMapTypeButton.Hidden = false;
            FocusOnSeekiosButton.Hidden = false;
            HistoricButton.Hidden = false;
            RefreshInProgressButton.Hidden = true;
            TimerLabel.Hidden = true;
        }

        private void DisplaySeekiosOnDemandTimerStopped()
        {
            TimerLabel.Hidden = true;
            RefreshInProgressButton.Hidden = true;
            if (_timerButton != null)
            {
                // enable the refresh button on the seekios marker
                _timerButton.Enabled = true;
                _timerButton.UserInteractionEnabled = true;
                _timerButton = null;
            }
            if (_firstInitilise)
            {
                if (_seekiosSelected.LastKnownLocation_latitude == App.DefaultLatitude
                    && _seekiosSelected.LastKnownLocation_longitude == App.DefaultLongitude)
                {
                    // the first location failed, let's try again
                    RefreshPosition.Enabled = true;
                }
                else
                {
                    // display the map
                    DisplayMapAndHideFirstLocation();
                    _firstInitilise = false;
                }
            }
        }

        private void DisplaySeekiosOnDemandOnSuccess()
        {
            _popupRefresh = null;
            _seekiosOnDemand = null;
            DisplayMapAndHideFirstLocation();
            RefreshMarkerSeekios();
        }

        private void DisplaySeekiosOnDemandOnFailed()
        {
            _seekiosOnDemand = null;
            if (_popupRefresh != null)
            {
                DismissViewController(false, null);
            }
            if (_firstInitilise)
            {
                LblRefreshPositionText.Text = Application.LocalizedString("SeekiosFailedToLocate");
                FirstRefresh1Image.Image = UIImage.FromBundle("FirstRefresh-3");
                StepSeekiosLabel.Hidden = true;
                RefreshPosition.SetTitle(Application.LocalizedString("RefreshButtonFailed"), UIControlState.Normal);
            }
            RefreshMarkerSeekios();
        }

        #endregion

        #region TIMER TRACKING

        private void SetTimerTracking()
        {
            _seekiosOnTracking = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnTracking != null)
            {
                SetLabeledCircularProgressView();
                // display the right UI elements
                _progressView.Hidden = false;
                // bind the callback timer
                if (!_seekiosSelected.HasGetLastInstruction
                    && _seekiosSelected.IsInPowerSaving
                    && _modeSelected != null
                    && _modeSelected.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.RAS)
                {
                    if (_progressView != null) _progressView.Hidden = true;
                    ChronoWaitForStartLabel.Hidden = true;
                }
                else if (_seekiosOnTracking.Timer.CountDown <= 0) ChronoWaitForStartLabel.Hidden = false;
                _seekiosOnTracking.Timer.Started = () =>
                {
                    if (_seekiosOnTracking.Timer.CountDown <= 0) ChronoWaitForStartLabel.Hidden = false;
                    else ChronoWaitForStartLabel.Hidden = true;
                    var remainingTime = TimeSpan.FromSeconds(_seekiosOnTracking.Timer.CountDown < 0 ? 0 : _seekiosOnTracking.Timer.CountDown);
                    var isLowerThanAnHour = (_seekiosOnTracking.Timer.CountDown < 3600);
                    _progressView.Tag = (int)_seekiosOnTracking.Timer.CountDown;
                    _progressView.ProgressLabel.Text = string.Format("{0:D2}:{1:D2}"
                        , isLowerThanAnHour ? remainingTime.Minutes : remainingTime.Hours
                        , isLowerThanAnHour ? remainingTime.Seconds : remainingTime.Minutes);
                    if (_seekiosOnTracking.MaxRefreshTime == 0) _progressView.SetProgress((float)0, false);
                    else _progressView.SetProgress((float)decimal.Divide((int)_seekiosOnTracking.Timer.CountDown, _seekiosOnTracking.MaxRefreshTime), false);
                };
                _seekiosOnTracking.Timer.UpdateUI = () =>
                {
                    if (_seekiosOnTracking.Timer.CountDown <= 0)
                    {
                        ChronoWaitForStartLabel.Hidden = false;
                    }
                    var remainingTime = TimeSpan.FromSeconds(_seekiosOnTracking.Timer.CountDown < 0 ? 0 : _seekiosOnTracking.Timer.CountDown);
                    var isLowerThanAnHour = (_seekiosOnTracking.Timer.CountDown < 3600);
                    _progressView.ProgressLabel.Text = string.Format("{0:D2}:{1:D2}"
                        , isLowerThanAnHour ? remainingTime.Minutes : remainingTime.Hours
                        , isLowerThanAnHour ? remainingTime.Seconds : remainingTime.Minutes);
                    if (_seekiosOnTracking.MaxRefreshTime == 0) _progressView.SetProgress((float)0, false);
                    else _progressView.SetProgress((float)decimal.Divide((int)_seekiosOnTracking.Timer.CountDown, _seekiosOnTracking.MaxRefreshTime), false);
                };
                _seekiosOnTracking.Timer.Stopped = DisplaySeekiosOnTrackingTimerStopped;
                _seekiosOnTracking.SetAndStartTimer();
            }
        }

        private void DisplaySeekiosOnTrackingTimerStopped()
        {
            if (_seekiosOnTracking.Timer.CountDown <= 0) ChronoWaitForStartLabel.Hidden = false;
        }

        private void SetLabeledCircularProgressView()
        {
            if (_progressView == null)
            {
                _progressView = new LabeledCircularProgressView(new CGRect((float)View.Bounds.X + 10, (float)View.Bounds.Y + 75, 60, 60));
                _progressView.RoundedCorners = true;
                _progressView.TrackTintColor = UIColor.FromRGBA(51, 51, 51, 200);
                _progressView.InnerTintColor = UIColor.Clear;
                var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == _seekiosSelected.Idseekios);
                if (mode != null && mode.StatusDefinition_idstatusDefinition != 1)
                {
                    _progressView.ProgressTintColor = UIColor.FromRGB(255, 76, 57);
                }
                else
                {
                    _progressView.ProgressTintColor = UIColor.FromRGB(98, 218, 115);
                }
                _progressView.Progress = 0f;
                _progressView.ProgressLabel.Text = "00:00";
                _progressView.ProgressLabel.TextColor = UIColor.FromRGBA(51, 51, 51, 200);
                _progressView.ProgressLabel.Font = UIFont.SystemFontOfSize(15);
                _progressView.Hidden = true;
                ChronoWaitForStartLabel.Hidden = true;
                View.AddSubview(_progressView);
            }
        }

        #endregion

        private void SetZoom()
        {
            if (_seekiosSelected == null) return;
            if (_seekiosSelected.LastKnownLocation_accuracy > 0)
            {
                // display the area for accuracy (triangulation)
                _mapControlManager.CreateAccuracyArea(_seekiosSelected.LastKnownLocation_latitude
                    , _seekiosSelected.LastKnownLocation_longitude
                    , _seekiosSelected.LastKnownLocation_accuracy);
            }
            else
            {
                // big zoom
                _mapControlManager.CenterInLocalisation(_seekiosSelected.LastKnownLocation_latitude
                    , _seekiosSelected.LastKnownLocation_longitude
                    , (float)ZoomLevelEnum.BigZoom
                    , true);
            }
        }

        private void RefreshMarkerSeekios()
        {
            IsRefreshEnable = true;
            _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(s => s.Idseekios == _seekiosSelected.Idseekios);
            MapViewModelBase.Seekios = _seekiosSelected;
            if (_mapControlManager.SelectedAnnotation != null)
            {
                _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
            }
            _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                , _seekiosSelected.SeekiosName
                , _seekiosSelected.SeekiosPicture
                , _seekiosSelected.LastKnownLocation_dateLocationCreation
                , _seekiosSelected.LastKnownLocation_latitude
                , _seekiosSelected.LastKnownLocation_longitude
                , _seekiosSelected.LastKnownLocation_accuracy
                , App.CurrentUserEnvironment.LsMode.Any(x => x.Seekios_idseekios == _seekiosSelected.Idseekios && x.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                , App.CurrentUserEnvironment.LsMode.Any(x => x.Seekios_idseekios == _seekiosSelected.Idseekios && x.StatusDefinition_idstatusDefinition != (int)StatutDefinitionEnum.RAS));
            SetZoom();
        }

        #endregion

        #region ===== Event =======================================================================

        #region HANDLE APP BACKGROUND / FORGROUND TIMER 

        private void BecomeActiveNotifcationCallback()
        {
            _seekiosSelected = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == _seekiosSelected.Idseekios);
            SetTimerTracking();

            _seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == _seekiosSelected.Idseekios);
            if (_seekiosOnDemand != null)
            {
                if (_seekiosSelected.DateLastOnDemandRequest.HasValue)
                {
                    var dateEndRefreshTimer = _seekiosSelected.DateLastOnDemandRequest.Value.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND);
                    if (dateEndRefreshTimer > DateTime.Now)
                    {
                        _seekiosOnDemand.Timer.Stop();
                        App.Locator.Map.LsSeekiosOnDemand.Remove(_seekiosOnDemand);
                        App.Locator.Map.AddSeekiosOnDemand(_seekiosSelected, dateEndRefreshTimer);
                        SetTimerOnDemand();
                    }
                    else
                    {
                        _seekiosOnDemand.Timer.Stop();
                        App.Locator.Map.LsSeekiosOnDemand.Remove(_seekiosOnDemand);
                    }
                }
            }
        }

        #endregion

        #region EVENT FROM BOTTOM VIEW

        private void BottomView_Tap()
        {
            if (!_isAllowToSlideUpAndDown) return;
            if (_lastBottomViewOpen.AddMilliseconds(500) > DateTime.Now) return;
            if (!_isBottomViewOpen)
            {
                if (_constraintYDown != null) NSLayoutConstraint.DeactivateConstraints(new NSLayoutConstraint[] { _constraintYDown });
                _constraintYUp = NSLayoutConstraint.Create(BottomView
                    , NSLayoutAttribute.CenterY
                    , NSLayoutRelation.Equal
                    , View
                    , NSLayoutAttribute.CenterY
                    , new nfloat(1.62)
                    , new nfloat(0));
                _constraintYUp.Active = true;
                NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { _constraintYUp });
                _isBottomViewOpen = true;
                ArrowImage.Transform = CGAffineTransform.MakeRotation((float)Math.PI);
                UIView.Animate(0.4, () =>
                {
                    View.LayoutIfNeeded();
                    View.UpdateConstraints();
                });
            }
            else
            {
                if (_constraintYUp != null) NSLayoutConstraint.DeactivateConstraints(new NSLayoutConstraint[] { _constraintYUp });
                _constraintYDown = NSLayoutConstraint.Create(BottomView
                    , NSLayoutAttribute.CenterY
                    , NSLayoutRelation.Equal
                    , View
                    , NSLayoutAttribute.CenterY
                    , new nfloat(2.17)
                    , new nfloat(0));
                _constraintYDown.Active = true;
                NSLayoutConstraint.ActivateConstraints(new NSLayoutConstraint[] { _constraintYDown });
                _isBottomViewOpen = false;
                ArrowImage.Transform = _arrowTransform;
                UIView.Animate(0.2, () =>
                {
                    View.LayoutIfNeeded();
                    View.UpdateConstraints();
                });
            }
            _lastBottomViewOpen = DateTime.Now;
        }

        private void HistoricButton_Click(object sender, EventArgs e)
        {
            //Tuple<int, int> batteryAndsignal = new Tuple<int, int>(86, 56);
            //Tuple<double, double, double, double> location = new Tuple<double, double, double, double>(new Random().Next(1, 80), new Random().Next(1, 40), 200, 0);
            //App.Locator.BaseMap.OnDemandPositionReceived("^FGbWSqp", batteryAndsignal, location, DateTime.Now);
            //App.Locator.ModeDontMove.OnNewDontMoveTrackingLocationAdded("^FGbWSqp", batteryAndsignal, location, DateTime.Now);
            App.Locator.Map.GoToHistoric();
        }

        private async void DeleteMode_Click(object sender, EventArgs e)
        {
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(f => f.Seekios_idseekios == MapViewModelBase.Seekios.Idseekios);
            App.Locator.ModeSelection.SeekiosUpdated = MapViewModelBase.Seekios;
            if (await App.Locator.ModeSelection.DeleteMode(mode))
            {
                // no last instruction anymore
                _seekiosSelected.HasGetLastInstruction = false;
                MapViewModelBase.Seekios.HasGetLastInstruction = false;
                // remove seekios from the list of alerts 
                if (App.Locator.BaseMap.LsSeekiosAlertState.Contains(_seekiosSelected.Idseekios))
                {
                    App.Locator.BaseMap.LsSeekiosAlertState.Remove(_seekiosSelected.Idseekios);
                }
                // delete the zone
                _mapControlManager.DeleteZone();
                // delete the seekios from the tracking list
                if (_seekiosOnTracking != null)
                {
                    if (_seekiosOnTracking.Timer.IsRunning) _seekiosOnTracking.Timer.Stop();
                    App.Locator.Map.LsSeekiosOnTracking.Remove(_seekiosOnTracking);
                }
                // hide tracking elements views
                if (_progressView != null) _progressView.Hidden = true;
                DeleteModeButton.Hidden = true;
                FocusOnZoneButton.Hidden = true;
                ChronoWaitForStartLabel.Hidden = true;
                // recreate the marker if the mode makes the marker red, we need it to become green
                RefreshMarkerSeekios();
                // block the bottom view
                if (_isBottomViewOpen)
                {
                    BottomView_Tap();
                }
                BottomView.RemoveGestureRecognizer(_tapSlideUpAndDown);
                BottomView.RemoveGestureRecognizer(_panSlideUpAndDown);
                InitialiseBottomView(null);
                _modeSelected = null;
            }
        }

        private void RaiseSeekiosChanged(object sender, int e)
        {
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(m => m.Seekios_idseekios == _seekiosSelected.Idseekios);
            if (mode == null && _isBottomViewOpen)
            {
                BottomView_Tap();
            }
            if (_seekiosOnDemand != null && _seekiosOnDemand.Timer.IsRunning)
            {
                var dateEndRefreshTimer = _seekiosSelected.DateLastOnDemandRequest.Value.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND);
                if (dateEndRefreshTimer > DateTime.Now)
                {
                    _seekiosOnDemand.Timer.Stop();
                    App.Locator.Map.LsSeekiosOnDemand.Remove(_seekiosOnDemand);
                    App.Locator.Map.AddSeekiosOnDemand(_seekiosSelected, dateEndRefreshTimer);
                    SetTimerOnDemand();
                }
            }
            InitialiseBottomView(mode);
        }

        #region FIRST POSITION

        private async void RefreshOnInitialize(object sender, EventArgs e)
        {
            if (await App.Locator.BaseMap.RefreshSeekiosPosition())
            {
                RefreshPosition.Enabled = false;
                SetTimer(sender, e);
            }
        }

        private void DisplayTutoButton_TouchUpInside(object sender, EventArgs e)
        {
            UIApplication.SharedApplication.OpenUrl(new NSUrl(App.TutorialHelpLink));
        }

        #endregion

        #endregion

        #region INIT TRACKING

        private async Task OnInitTrackingRouteComplete(object sender, EventArgs eventArg)
        {
            InvokeOnMainThread(() =>
            {
                if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                {
                    ThirdRightLabel.Text = App.Locator.ModeDontMove.LsLocations.Count.ToString();
                    App.Locator.ModeDontMove.MapControlManager.CreateRouteForeground(App.Locator.ModeDontMove.LsLocations);
                }
                else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                {
                    ThirdRightLabel.Text = App.Locator.ModeZone.LsLocations.Count.ToString();
                    App.Locator.ModeZone.MapControlManager.CreateRouteForeground(App.Locator.ModeZone.LsLocations);
                }
                else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    ThirdRightLabel.Text = App.Locator.ModeTracking.LsLocations.Count.ToString();
                    App.Locator.ModeTracking.MapControlManager.CreateRouteForeground(App.Locator.ModeTracking.LsLocations);
                }

                SetTimerTracking();
            });
        }

        #endregion

        #region REFRESH F5

        private void RefreshInProgressButton_TouchUpInside(object sender, EventArgs e)
        {
            _popupRefresh = AlertControllerHelper.CreateAlertOnClickRefreshButton(_seekiosSelected.SeekiosName);
            PresentViewController(_popupRefresh, true, null);
        }

        private void OnSeekiosRefreshRequestSent(object sender, EventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                SetView();
                SetTimerOnDemand();
            });
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
                // creation of a new annotation with a green color
                MapControlManager.IsOutOf = false;
                if (_mapControlManager.SelectedAnnotation != null)
                {
                    _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
                }
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , false
                    , false);
                SetZoom();
                InitialiseBottomView(_modeSelected);
                App.Locator.ModeTracking.InitTrackingRoute();
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
            if (_mapControlManager != null)
            {
                // creation of a new annotation with a red color
                MapControlManager.IsOutOf = true;
                ModeZoneFirstView.IsOnModeZone = false;
                if (_mapControlManager.SelectedAnnotation != null)
                {
                    _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
                }
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , true
                    , true);
                SetZoom();
                InitialiseBottomView(_modeSelected);
                SetTimerTracking();
            }
        }

        private void OnNewZoneTrackingLocationAdded(int idSeekios
            , double lat
            , double lon
            , double alt
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (_mapControlManager != null)
            {
                // creation of a new annotation with a red color
                MapControlManager.IsOutOf = true;
                ModeZoneFirstView.IsOnModeZone = false;
                if (_mapControlManager.SelectedAnnotation != null)
                {
                    _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
                }
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , true
                    , true);
                SetZoom();
                InitialiseBottomView(_modeSelected);
                App.Locator.ModeZone.InitZoneTrackingRouteAsync();
            }
        }

        #endregion

        #region MODE DONT MOVE

        private void OnSeekiosMovedNotified(int idSeekios)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            // creation of a new annotation with a red color
            MapControlManager.IsOutOf = true;
            ModeZoneFirstView.IsOnModeZone = false;
            if (_mapControlManager.SelectedAnnotation != null)
            {
                _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
            }
            _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                , _seekiosSelected.SeekiosName
                , _seekiosSelected.SeekiosPicture
                , _seekiosSelected.DateLastCommunication
                , _seekiosSelected.LastKnownLocation_latitude
                , _seekiosSelected.LastKnownLocation_longitude
                , _seekiosSelected.LastKnownLocation_accuracy
                , true
                , true);
            SetZoom();
            InitialiseBottomView(_modeSelected);
            SetTimerTracking();
        }

        private void OnNewDontMoveTrackingLocationAdded(int idSeekios
            , double lat
            , double lon
            , double altitude
            , double accuracy
            , DateTime dateCommunication)
        {
            if (MapViewModelBase.Seekios == null || MapViewModelBase.Seekios.Idseekios != idSeekios) return;
            if (_mapControlManager != null)
            {
                // creation of a new annotation with a red color
                MapControlManager.IsOutOf = true;
                ModeZoneFirstView.IsOnModeZone = false;
                if (_mapControlManager.SelectedAnnotation != null)
                {
                    _mapControlManager.MapViewControl.RemoveAnnotation(_mapControlManager.SelectedAnnotation);
                }
                _mapControlManager.CreateSeekiosMarkerAsync(_seekiosSelected.Idseekios.ToString()
                    , _seekiosSelected.SeekiosName
                    , _seekiosSelected.SeekiosPicture
                    , dateCommunication
                    , lat
                    , lon
                    , accuracy
                    , true
                    , true);
                SetZoom();
                InitialiseBottomView(_modeSelected);
                App.Locator.ModeDontMove.InitDontMoveTrackingRouteAsync();
            }
        }

        #endregion

        #endregion
    }
}