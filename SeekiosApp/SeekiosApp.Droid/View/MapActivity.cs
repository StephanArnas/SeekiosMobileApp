using Android.App;
using Android.Gms.Maps;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.ControlManager;
using SeekiosApp.Enum;
using System.Linq;
using SeekiosApp.ViewModel;
using System;
using System.Threading.Tasks;
using static Android.Views.View;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class MapActivity : MapBaseActivity, IOnTouchListener
    {
        #region ===== Attributs ===================================================================

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Button used to center the map on the zone</summary>
        public XamSvg.SvgImageView CenterOnZoneButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState
                , Resource.Layout.MapLayout
                , Resources.GetString(Resource.String.map_title)
                , null);
            GetObjectsFromView();
            SetDataToView();
        }

        protected sealed override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnResume()
        {
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            base.OnResume();
            CenterOnZoneButton.Click += OnCenterOnZoneButtonClick;
            HistoricTextView.Click += HistoricTextView_Click;
            DeleteModeTextView.Click += DeleteModeTextView_Click1;
        }

        protected override void OnPause()
        {
            base.OnPause();
            CenterOnZoneButton.Click -= OnCenterOnZoneButtonClick;
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            HistoricTextView.Click -= HistoricTextView_Click;
            DeleteModeTextView.Click -= DeleteModeTextView_Click1;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_mapControlManager != null) _mapControlManager.Dispose();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.home:
                    Finish();
                    break;
                default:
                    Finish();
                    break;
            }
            return true;
        }

        #endregion

        #region ===== Initializes View ============================================================

        private void GetObjectsFromView()
        {
            RootLayout = FindViewById<RelativeLayout>(Resource.Id.drawer_layout);
            RefreshInProgressButton = FindViewById<TextView>(Resource.Id.mapBase_inProgress);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            CenterOnZoneButton = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZone_showZone);
            NoPositionYetNotif = FindViewById<TextView>(Resource.Id.map_noPositionsNotif);

            // Bottom layout elements
            BottomLayout = FindViewById<RelativeLayout>(Resource.Id.map_bottomLayout);
            ArrowSvgImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.mapBase_upArrow);
            ArrowModeHistoricLayout = FindViewById<RelativeLayout>(Resource.Id.bottomLayout_top);
            ModeImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.bottomLayout_modeImage);
            PowerSavingImageView = FindViewById<XamSvg.SvgImageView>(Resource.Id.bottomLayout_powerSavingImage);
            ModeTextView = FindViewById<TextView>(Resource.Id.bottomLayout_modeText);
            DeleteModeTextView = FindViewById<TextView>(Resource.Id.bottomLayout_buttonDeleteMode);
            FirstLeftTextView = FindViewById<TextView>(Resource.Id.bottomLayout_firstTextLeft);
            FirstRightTextView = FindViewById<TextView>(Resource.Id.bottomLayout_firstTextRight);
            SecondRightTextView = FindViewById<TextView>(Resource.Id.bottomLayout_secondTextRight);
            ThirdLeftTextView = FindViewById<TextView>(Resource.Id.bottomLayout_thirdTextLeft);
            ThirdRightTextView = FindViewById<TextView>(Resource.Id.bottomLayout_thirdTextRight);
            HistoricTextView = FindViewById<TextView>(Resource.Id.mapBase_historicButton);
            GetLayoutSize();
        }

        private void SetDataToView()
        {
            BottomLayout.SetOnTouchListener(this);
        }

        #endregion

        #region ===== Event =======================================================================

        private void OnCenterOnZoneButtonClick(object sender, EventArgs e)
        {
            if (null != _mapControlManager) _mapControlManager.CenterOnZone();
        }

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

        #region ===== Callback ====================================================================

        public override void OnMapReady(GoogleMap googleMap)
        {
            if (googleMap != null)
            {
                MapWrapperLayout.Init(googleMap);

                if (App.Locator.ModeTracking.MapControlManager != null) App.Locator.ModeTracking.MapControlManager.OnInitTrackingRouteComplete -= OnInitTrackingRouteComplete;

                _mapControlManager = new MapControlManager(googleMap
                    , this
                    , CenterSeekiosSvgImageView
                    , MapViewModelBase.Seekios.Idseekios.ToString());

                // subscribe to tracking Route Drawing events
                _mapControlManager.OnInitTrackingRouteComplete += OnInitTrackingRouteComplete;

                if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                {
                    if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved) MapControlManager.IsInAlert = true;
                    App.Locator.ModeDontMove.MapControlManager = _mapControlManager;
                    App.Locator.ModeDontMove.InitMap();
                    App.Locator.ModeDontMove.InitModeDontMove();
                    if (App.Locator.Map.LsSeekiosOnTracking.Any(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                    {
                        using (var handler = new Handler(Looper.MainLooper))
                        {
                            handler.Post(() => { App.Locator.ModeDontMove.InitDontMoveTrackingRouteAsync(); });
                        }
                    }
                }
                else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                {
                    if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone) MapControlManager.IsInAlert = true;
                    App.Locator.ModeZone.MapControlManager = _mapControlManager;
                    App.Locator.ModeZone.InitMap();
                    var isInAlert = MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1;
                    _mapControlManager.CreateZone(App.Locator.ModeZone.DecodeTrame(MapViewModelBase.Mode.Trame), isInAlert);
                    if (App.Locator.Map.LsSeekiosOnTracking.Any(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                    {
                        using (var handler = new Handler(Looper.MainLooper))
                        {
                            handler.Post(() => { App.Locator.ModeZone.InitZoneTrackingRouteAsync(); });
                        }
                    }
                    CenterOnZoneButton.Visibility = ViewStates.Visible;
                }
                else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    MapControlManager.IsInAlert = false;
                    App.Locator.ModeTracking.MapControlManager = _mapControlManager;
                    App.Locator.ModeTracking.InitMap();
                    LoadingLayout.Visibility = ViewStates.Visible;
                    using (var handler = new Handler(Looper.MainLooper))
                    {
                        handler.Post(() => { App.Locator.ModeTracking.InitTrackingRoute(); });
                    }
                }
                else
                {
                    MapControlManager.IsInAlert = false;
                    App.Locator.Map.MapControlManager = _mapControlManager;
                    App.Locator.Map.InitMap();
                }
                _mapControlManager.RegisterMethodes();
            }
            base.OnMapReady(googleMap);

            //the call to the function is made before the map is initialized => _mapControlManager = null 
            //we need to Hide & Show the markerInfo once we have initialized the control manager in order to reflect the right
            //internet connection state 
            if (!App.DeviceIsConnectedToInternet) UpdateUIOnConnectionStateChanged(false);
        }

        protected override void OnHasToShowNoPositionLayout(object sender, object args)
        {
            base.OnHasToShowNoPositionLayout(null, null);
            if (null != CenterOnZoneButton) CenterOnZoneButton.Visibility = ViewStates.Gone;
        }

        protected override void OnHasToHideNoPositionLayout(object sender, object args)
        {
            base.OnHasToHideNoPositionLayout(null, null);
            if (null != CenterOnZoneButton
                 && null != MapViewModelBase.Mode
                 && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
            {
                CenterOnZoneButton.Visibility = ViewStates.Visible;
            }
        }

        private async Task OnInitTrackingRouteComplete(object o, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (!IsFinishing)
                {
                    if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
                    {
                        App.Locator.ModeDontMove.MapControlManager.CreateRouteForeground(App.Locator.ModeDontMove.LsLocations);
                    }
                    else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
                    {
                        App.Locator.ModeZone.MapControlManager.CreateRouteForeground(App.Locator.ModeZone.LsLocations);
                    }
                    else if (MapViewModelBase.Mode != null && MapViewModelBase.Mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                    {
                        App.Locator.ModeTracking.MapControlManager.CreateRouteForeground(App.Locator.ModeTracking.LsLocations);
                    }
                    if (LoadingLayout != null) LoadingLayout.Visibility = ViewStates.Gone;
                    SetTimerTracking();
                }
            });
        }

        #endregion

        #region ===== BottomLayout Handler : information on mode etc ==============================

        private async void DeleteModeTextView_Click1(object sender, EventArgs e)
        {
            App.Locator.ModeSelection.SeekiosUpdated = MapViewModelBase.Seekios;
            if (await App.Locator.ModeSelection.DeleteMode(MapViewModelBase.Mode))
            {
                TrackingTimerProgressBar.Visibility = ViewStates.Gone;
                TrackingTimerTextView.Visibility = ViewStates.Gone;
                InitBottomLayout();
            }
        }

        private void HistoricTextView_Click(object sender, EventArgs e)
        {
            App.Locator.Map.GoToHistoric();
        }

        #endregion
    }
}