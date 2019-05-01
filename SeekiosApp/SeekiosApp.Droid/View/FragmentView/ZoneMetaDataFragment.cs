using System.Linq;

using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using SeekiosApp.Droid.Helper;
using Android.Support.V4.App;
using Android.Content;
using System;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using SeekiosApp.Extension;
using SeekiosApp.ViewModel;

namespace SeekiosApp.Droid.View.FragmentView
{
    class ZoneMetaDataFragment : Fragment
    {
        #region ===== Propriétées =================================================================

        public TextView IsSeekiosInZoneTextView { get; set; }

        public TextView ExplanationTextView { get; set; }

        public TextView CountOfPointsTextView { get; set; }

        public TextView SurfaceTextView { get; set; }

        public GridLayout BottomLayout { get; set; }

        private Polygon _zonePolygon;
        public Polygon ZonePolygon
        {
            get { return _zonePolygon; }
            set
            {
                _zonePolygon = value;
                InitializeSurfaceStatTextView();
            }
        }

        public LinearLayout EditMetaDataLinearLayout { get; set; }

        public Switch TrackingAfterZoneSwitch { get; set; }

        public Spinner RefreshTimeSpinner { get; set; }

        public LinearLayout MapMetaDataLinearLayout { get; set; }

        public TextView ActivateSinceTextView { get; set; }

        public TextView CountOfOutAlertTextView { get; set; }

        public TextView SurfaceStatTextView { get; set; }

        public TextView TrackingRefreshTimeStat { get; set; }

        public RelativeLayout NotifyBackInZoneRelativeLayout { get; set; }

        public Button NotifyBackInZone { get; set; }

        #endregion

        #region ===== Cycle De Vie ================================================================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="savedInstanceState"></param>
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inflater"></param>
        /// <param name="container"></param>
        /// <param name="savedInstanceState"></param>
        /// <returns></returns>
        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ZoneMetaDataFragmentLayout, container, false);

            GetObjectsFromView(view);
            SetDataToView();

            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnResume()
        {
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += OnSeekiosOutOfZoneNotified;
            App.Locator.ModeZone.OnSeekiosBackInZoneNotified += OnSeekiosBackInZoneNotified;
            TrackingAfterZoneSwitch.CheckedChange += OnTrackingAfterZoneSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected += OnRefreshTimeSpinnerItemSelected;
            NotifyBackInZone.Touch += OnNotifyBackInZoneTouch;
            NotifyBackInZone.Click += OnNotifyBackInZoneClick;
            App.Locator.ModeZone.PropertyChanged += OnModeZonePropertyChanged;

            base.OnResume();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified -= OnSeekiosOutOfZoneNotified;
            App.Locator.ModeZone.OnSeekiosOutOfZoneNotified -= OnSeekiosBackInZoneNotified;
            TrackingAfterZoneSwitch.CheckedChange -= OnTrackingAfterZoneSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected -= OnRefreshTimeSpinnerItemSelected;
            NotifyBackInZone.Touch -= OnNotifyBackInZoneTouch;
            NotifyBackInZone.Click -= OnNotifyBackInZoneClick;
            App.Locator.ModeZone.PropertyChanged -= OnModeZonePropertyChanged;

            base.OnPause();
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            IsSeekiosInZoneTextView = view.FindViewById<TextView>(Resource.Id.modeZone_isSeekiosInZoneTextView);
            CountOfPointsTextView = view.FindViewById<TextView>(Resource.Id.modeZone_countOfPoints);
            SurfaceTextView = view.FindViewById<TextView>(Resource.Id.modeZone_surface);
            ExplanationTextView = view.FindViewById<TextView>(Resource.Id.modeZone_explanationTextView);
            BottomLayout = view.FindViewById<GridLayout>(Resource.Id.modeZone_bottomGridView);
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeZone_onEdition);
            TrackingAfterZoneSwitch = view.FindViewById<Switch>(Resource.Id.modeZone_trackingAfterZoneSwitch);
            RefreshTimeSpinner = view.FindViewById<Spinner>(Resource.Id.modeZone_trackingRefreshTimeSpinner);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeZone_onMap);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeZone_activateSince);
            CountOfOutAlertTextView = view.FindViewById<TextView>(Resource.Id.modeZone_countOfOutAlert);
            SurfaceStatTextView = view.FindViewById<TextView>(Resource.Id.modeZone_surfaceStat);
            TrackingRefreshTimeStat = view.FindViewById<TextView>(Resource.Id.modeZone_trackingRefreshTimeStat);
            NotifyBackInZoneRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.modeZone_notifyBackInZoneLayout);
            NotifyBackInZone = view.FindViewById<Button>(Resource.Id.modeZone_notifyBackInZone);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            if (App.Locator.ModeZone.IsOnEditMode)
            {
                RefreshMarkers();
                var adapter = ArrayAdapter.CreateFromResource(Context
                    , Resource.Array.refreshTime_array
                    , Resource.Layout.AddSeekiosSpinnerText);
                adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
                RefreshTimeSpinner.Adapter = adapter;
                SelectCorrectRefreshTimeItem();
                IsSeekiosInZoneTextView.Visibility = ViewStates.Gone;
                EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
                MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
                TrackingAfterZoneSwitch.Checked = App.Locator.ModeZone.IsTrackingAfterZone;
                RefreshMarkers();
            }
            else
            {
                ExplanationTextView.Visibility = ViewStates.Gone;
                IsSeekiosInZoneTextView.Visibility = ViewStates.Visible;
                if (MapViewModelBase.Mode != null)
                    IsSeekiosInZoneTextView.Text = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 2 ?
                        Resources.GetString(Resource.String.modeZone_seekiosOutOfZone) : Resources.GetString(Resource.String.modeZone_seekiosInZone);
                EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
                MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
                InitializeActivateSinceTextView();
                InitializeCountOfOutAlertTextView();
                InitializeSurfaceStatTextView();
                if (App.Locator.ModeZone.IsTrackingAfterZone)
                {
                    TrackingRefreshTimeStat.Visibility = ViewStates.Visible;
                    InitializeTrackingRefreshTimeStatTextView();
                    NotifyBackInZoneRelativeLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    TrackingRefreshTimeStat.Visibility = ViewStates.Gone;
                    if (MapViewModelBase.Mode != null)
                        NotifyBackInZoneRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 2 ? ViewStates.Visible : ViewStates.Gone;
                }
            }
        }

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// Rafraichit les différentes variables affichés sur la map
        /// </summary>
        public void RefreshMarkers()
        {
            /*if (ZonePolygon == null || ZonePolygon.Points.Count == 0)
            {
                CountOfPointsTextView.Text = "0";
                SurfaceTextView.Text = string.Format("0 {0}", Resources.GetString(Resource.String.unit_squarre));
                //BottomLayout.Visibility = ViewStates.Gone;
                ExplanationTextView.Visibility = ViewStates.Visible;
                return;
            }
            BottomLayout.Visibility = ViewStates.Visible;
            ExplanationTextView.Visibility = ViewStates.Gone;

            if (ZonePolygon.Points.Count == 1) CountOfPointsTextView.Text = "1";
            else CountOfPointsTextView.Text = (ZonePolygon.Points.Count - 1).ToString();
            */
            var surface = AreaHelper.CalculateAreaOfGPSPolygonOnEarthInSquareMeters(ZonePolygon.Points.ToList());
            SurfaceTextView.Text = AreaHelper.SerializeArea(surface);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        private void OnSeekiosOutOfZoneNotified(double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
        {
            IsSeekiosInZoneTextView.Text = Resources.GetString(Resource.String.modeZone_seekiosOutOfZone);
            NotifyBackInZoneRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 2 ? ViewStates.Visible : ViewStates.Gone;
            InitializeCountOfOutAlertTextView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        private void OnSeekiosBackInZoneNotified(double lat, double lon, double alt, double accuracy, DateTime dateCommunication)
        {
            IsSeekiosInZoneTextView.Text = Resources.GetString(Resource.String.modeZone_seekiosInZone);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrackingAfterZoneSwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RefreshTimeSpinner.Enabled = TrackingAfterZoneSwitch.Checked;
            App.Locator.ModeZone.IsTrackingAfterZone = TrackingAfterZoneSwitch.Checked;
            InitializeCountOfOutAlertTextView();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyBackInZoneTouch(object sender, Android.Views.View.TouchEventArgs e)
        {
            /*if (e.Event.Action == MotionEventActions.Down)
                MapBaseActivity.BottomLayoutViewPager.CanSwipe = false;
            else if (e.Event.Action != MotionEventActions.Move)
                MapBaseActivity.BottomLayoutViewPager.CanSwipe = true;
            e.Handled = false;*/
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnNotifyBackInZoneClick(object sender, EventArgs e)
        {
            App.Locator.ModeZone.OnNotifySeekiosBackInZone();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeActivateSinceTextView()
        {
            if (MapViewModelBase.Mode == null) return;

            var NbrOfDaysFromNow = Math.Ceiling((DateTime.Now - MapViewModelBase.Mode.DateModeCreation).TotalDays);
            var textActivateSince = string.Empty;

            if (NbrOfDaysFromNow < 2) textActivateSince = string.Format(Resources.GetString(Resource.String.metaData_activateSinceSingle), NbrOfDaysFromNow);
            else textActivateSince = string.Format(Resources.GetString(Resource.String.metaData_activateSince), NbrOfDaysFromNow);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textActivateSince);
            var formattedTextActivateSince = new SpannableString(textActivateSince);
            formattedTextActivateSince.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            ActivateSinceTextView.SetText(formattedTextActivateSince, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCountOfOutAlertTextView()
        {
            if (MapViewModelBase.Mode == null) return;
            var countOfTriggeredAlert = MapViewModelBase.Mode.CountOfTriggeredAlert;
            var textCountOfOutAlert = string.Format(Resources.GetString(Resource.String.modeZone_outAlert), countOfTriggeredAlert);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfOutAlert);
            var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfOutAlert);
            formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            CountOfOutAlertTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeSurfaceStatTextView()
        {
            if (ZonePolygon == null) return;
            if (SurfaceStatTextView == null) return;

            var surface = AreaHelper.CalculateAreaOfGPSPolygonOnEarthInSquareMeters(ZonePolygon.Points.ToList());
            var surfaceStat = AreaHelper.SerializeArea(surface);
            var textSurfaceStat = string.Format(Resources.GetString(Resource.String.modeZone_surfaceStr), surfaceStat);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textSurfaceStat);
            var formattedTextSurfaceStat = new SpannableString(textSurfaceStat);
            formattedTextSurfaceStat.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            SurfaceStatTextView.SetText(formattedTextSurfaceStat, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeTrackingRefreshTimeStatTextView()
        {
            if (TrackingRefreshTimeStat == null) return;
            if (MapViewModelBase.Mode == null) return;

            var refreshTimeStat = StringExtension.secondeToString(MapViewModelBase.RefreshTime * 60);
            var textTrackingRefreshTimeStat = string.Format(Resources.GetString(Resource.String.modeZone_trackingRefreshTimeStat), refreshTimeStat);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textTrackingRefreshTimeStat);
            var formattedTextTrackingRefreshTimeStat = new SpannableString(textTrackingRefreshTimeStat);
            formattedTextTrackingRefreshTimeStat.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            TrackingRefreshTimeStat.SetText(formattedTextTrackingRefreshTimeStat, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModeZonePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Mode":
                    if (MapViewModelBase.Mode == null) break;
                    IsSeekiosInZoneTextView.Text = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 2 ?
                        Resources.GetString(Resource.String.modeZone_seekiosOutOfZone) : Resources.GetString(Resource.String.modeZone_seekiosInZone);
                    NotifyBackInZoneRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 2 ? ViewStates.Visible : ViewStates.Gone;
                    break;
                case "IsTrackingAfterZone":
                    TrackingAfterZoneSwitch.Checked = App.Locator.ModeZone.IsTrackingAfterZone;
                    TrackingRefreshTimeStat.Visibility = App.Locator.ModeZone.IsTrackingAfterZone ? ViewStates.Visible : ViewStates.Gone;
                    break;
                case "RefreshTime":
                    if (App.Locator.ModeZone.IsTrackingAfterZone && !App.Locator.ModeZone.IsOnEditMode)
                        InitializeTrackingRefreshTimeStatTextView();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRefreshTimeSpinnerItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            switch (e.Position)
            {
                case 0: MapViewModelBase.RefreshTime = 1; break;    //1min
                case 1: MapViewModelBase.RefreshTime = 2; break;    //2min
                case 2: MapViewModelBase.RefreshTime = 5; break;    //5min
                case 3: MapViewModelBase.RefreshTime = 15; break;   //15min
                case 4: MapViewModelBase.RefreshTime = 30; break;   //30min
                case 5: MapViewModelBase.RefreshTime = 60; break;   //1h
                case 6: MapViewModelBase.RefreshTime = 120; break;  //2h
                case 7: MapViewModelBase.RefreshTime = 300; break;  //5h
                case 8: MapViewModelBase.RefreshTime = 600; break;  //10h
                case 9: MapViewModelBase.RefreshTime = 1440; break; //24h
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SelectCorrectRefreshTimeItem()
        {
            switch (MapViewModelBase.RefreshTime)
            {
                case 1: RefreshTimeSpinner.SetSelection(0); break;    //1min
                case 2: RefreshTimeSpinner.SetSelection(1); break;    //2min
                case 5: RefreshTimeSpinner.SetSelection(2); break;    //5min
                case 15: RefreshTimeSpinner.SetSelection(3); break;   //15min
                case 30: RefreshTimeSpinner.SetSelection(4); break;   //30min
                case 60: RefreshTimeSpinner.SetSelection(5); break;   //1h
                case 120: RefreshTimeSpinner.SetSelection(6); break;  //2h
                case 300: RefreshTimeSpinner.SetSelection(7); break;  //5h
                case 600: RefreshTimeSpinner.SetSelection(8); break;  //10h
                case 1440: RefreshTimeSpinner.SetSelection(9); break;   //24h
                default:
                    break;
            }
        }

        #endregion
    }
}