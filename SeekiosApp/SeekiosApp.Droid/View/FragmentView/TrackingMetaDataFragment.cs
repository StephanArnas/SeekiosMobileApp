using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using SeekiosApp.Extension;
using SeekiosApp.ViewModel;
using System;

namespace SeekiosApp.Droid.View.FragmentView
{
    class TrackingMetaDataFragment : Fragment
    {
        #region ===== Propriétées =================================================================

        public SeekBar TrackingMarkerSelection { get; set; }

        public TextView FirstDateTextView { get; set; }

        public TextView LastDateTextView { get; set; }

        public GridLayout BottomGridView { get; set; }

        public TextView NoLocationTextView { get; set; }

        public LinearLayout EditMetaDataLinearLayout { get; set; }

        public Spinner RefreshTimeSpinner { get; set; }

        public LinearLayout MapMetaDataLinearLayout { get; set; }

        public TextView ActivateSinceTextView { get; set; }

        public TextView CountOfLocTextView { get; set; }

        public TextView RefreshTimeStatTextView { get; set; }

        public TextView TrackingDescTextView { get; set; }

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
            var view = inflater.Inflate(Resource.Layout.TrackingMetaDataFragmentLayout, container, false);

            GetObjectsFromView(view);
            SetDataToView();

            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnResume()
        {
            base.OnResume();
            App.Locator.ModeTracking.PropertyChanged += OnModeTrackingPropertyChanged;
            TrackingMarkerSelection.ProgressChanged += OnTrackingMarkerSelectionChanged;
            RefreshTimeSpinner.ItemSelected += OnRefreshTimeSpinnerItemSelected;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            App.Locator.ModeTracking.PropertyChanged -= OnModeTrackingPropertyChanged;
            TrackingMarkerSelection.ProgressChanged -= OnTrackingMarkerSelectionChanged;
            RefreshTimeSpinner.ItemSelected -= OnRefreshTimeSpinnerItemSelected;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            TrackingMarkerSelection = view.FindViewById<SeekBar>(Resource.Id.modeTracking_trackingMarkerSelection);
            FirstDateTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_trackingFirstDate);
            LastDateTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_trackingLastDate);
            BottomGridView = view.FindViewById<GridLayout>(Resource.Id.modeTracking_bottomGridView);
            NoLocationTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_noLocation);
            RefreshTimeSpinner = view.FindViewById<Spinner>(Resource.Id.modeTracking_refreshTimeSpinner);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_activateSince);
            CountOfLocTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_countOfLoc);
            RefreshTimeStatTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_refreshTimeStat);
            TrackingDescTextView = view.FindViewById<TextView>(Resource.Id.modeTracking_trackingDesc); 
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeTracking_onEdition);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeTracking_onMap);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            App.Locator.ModeTracking.InitMode();

            if (App.Locator.ModeTracking.IsWaitingForValidation)
            {
                BottomGridView.Visibility = ViewStates.Gone;
                NoLocationTextView.Visibility = ViewStates.Gone;
                TrackingDescTextView.Visibility = ViewStates.Visible;

                var adapter = ArrayAdapter.CreateFromResource(Context
                    , Resource.Array.refreshTime_array
                    , Resource.Layout.AddSeekiosSpinnerText);
                adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
                RefreshTimeSpinner.Adapter = adapter;
                EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
                MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                TrackingDescTextView.Visibility = ViewStates.Gone;
                TrackingMarkerSelection.Max = App.Locator.ModeTracking.LsLocations.Count - 1;
                TrackingMarkerSelection.Progress = App.Locator.ModeTracking.LsLocations.Count - 1;
                BottomGridView.Visibility = App.Locator.ModeTracking.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                NoLocationTextView.Visibility = App.Locator.ModeTracking.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
                FirstDateTextView.Text = App.Locator.ModeTracking.TrackingFirstDate;
                LastDateTextView.Text = App.Locator.ModeTracking.TrackingLastDate;

                InitializeActivateSinceTextView();
                InitializeCountOfLocTextView();
                InitializeRefreshTimeStatTextView();
                EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
                MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
            }
        }

        #endregion

        #region ===== Handlers ====================================================================

        /// <summary>
        /// Déclenchement du property changed
        /// </summary>
        private void OnModeTrackingPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LsLocations":
                    TrackingMarkerSelection.Max = App.Locator.ModeTracking.LsLocations.Count - 1;
                    TrackingMarkerSelection.Progress = App.Locator.ModeTracking.LsLocations.Count - 1;
                    BottomGridView.Visibility = App.Locator.ModeTracking.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                    NoLocationTextView.Visibility = App.Locator.ModeTracking.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
                    InitializeCountOfLocTextView();
                    break;
                case "TrackingFirstDate": FirstDateTextView.Text = App.Locator.ModeTracking.TrackingFirstDate; break;
                case "TrackingLastDate": LastDateTextView.Text = App.Locator.ModeTracking.TrackingLastDate; break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrackingMarkerSelectionChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (App.Locator.ModeTracking.LsLocations.Count <= 0) return;
            App.Locator.ModeTracking.SelectedLocation = App.Locator.ModeTracking.LsLocations[TrackingMarkerSelection.Progress];
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

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// 
        /// </summary>
        private void InitializeActivateSinceTextView()
        {
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
        private void InitializeCountOfLocTextView()
        {
            var countOfTriggeredAlert = App.Locator.ModeTracking.LsLocations.Count;
            var textCountOfLoc = string.Format(Resources.GetString(Resource.String.modeTracking_countOfLocStat), countOfTriggeredAlert);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfLoc);
            var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfLoc);
            formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            CountOfLocTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeRefreshTimeStatTextView()
        {
            var refreshTimeStat = StringExtension.secondeToString(MapViewModelBase.RefreshTime * 60);
            var textRefreshTimeStat = string.Format(Resources.GetString(Resource.String.modeTracking_refreshTimeStat), refreshTimeStat);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textRefreshTimeStat);
            var formattedTextRefreshTimeStat = new SpannableString(textRefreshTimeStat);
            formattedTextRefreshTimeStat.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            RefreshTimeStatTextView.SetText(formattedTextRefreshTimeStat, TextView.BufferType.Spannable);
        }

        #endregion
    }
}