using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.refractored.fab;
using SeekiosApp.Extension;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using SeekiosApp.ViewModel;
using Android.Support.V4.App;
using SeekiosApp.Helper;
using SeekiosApp.Droid.CustomComponents.Adapter;
using SeekiosApp.Model.APP;

namespace SeekiosApp.Droid.View.FragmentView
{
    public class DailyTrackMetaDataFragment : Android.Support.V4.App.Fragment
    {
        #region ===== Attributs ===================================================================

        private DailyTrackFragmentAdapter _listAdapter;

        private Context _context;

        private List<string> _listOfStr = new List<string>();

        public DailyTrackMetaDataFragment(Context context)
        {
            _context = context;
        }

        #endregion

        #region ===== Propriétées =================================================================

        public SeekBar TrackingMarkerSelection { get; set; }

        public TextView FirstDateTextView { get; set; }

        public TextView LastDateTextView { get; set; }

        public GridLayout BottomGridView { get; set; }

        public TextView NoLocationTextView { get; set; }

        public LinearLayout EditMetaDataLinearLayout { get; set; }

        public LinearLayout MapMetaDataLinearLayout { get; set; }

        public TextView ActivateSinceTextView { get; set; }

        public TextView CountOfLocTextView { get; set; }

        public TextView TimeStatTextView { get; set; }

        public TextView TrackingDescTextView { get; set; }

        public FloatingActionButton AddTimeFloatingActionButton { get; set; }

        public ListView TimerListView { get; set; }

        public TextView InfoListTextView { get; set; }

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
            var view = inflater.Inflate(Resource.Layout.DailyTrackMetaDataFragmentLayout, container, false);

            GetObjectsFromView(view);
            SetDataToView();

            if (App.Locator.ModeDailyTrack.IsWaitingForValidation)
            {
                UpdateUI();
            }

            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnResume()
        {
            AddTimeFloatingActionButton.Click += AddTimeFloatingActionButton_Click;
            App.Locator.ModeDailyTrack.PropertyChanged += ModeDailyTrack_PropertyChanged;
            TrackingMarkerSelection.ProgressChanged += OnTrackingMarkerSelectionChanged;
            base.OnResume();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            AddTimeFloatingActionButton.Click -= AddTimeFloatingActionButton_Click;
            App.Locator.ModeDailyTrack.PropertyChanged -= ModeDailyTrack_PropertyChanged;
            TrackingMarkerSelection.ProgressChanged -= OnTrackingMarkerSelectionChanged;
            base.OnPause();
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            TrackingMarkerSelection = view.FindViewById<SeekBar>(Resource.Id.dailyTrack_trackingMarkerSelection);
            FirstDateTextView = view.FindViewById<TextView>(Resource.Id.dailyTrack_trackingFirstDate);
            LastDateTextView = view.FindViewById<TextView>(Resource.Id.dailyTrack_trackingLastDate);
            BottomGridView = view.FindViewById<GridLayout>(Resource.Id.dailyTrack_bottomGridView);
            NoLocationTextView = view.FindViewById<TextView>(Resource.Id.dailyTrack_noLocation);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeDailyTrack_activateSince);
            CountOfLocTextView = view.FindViewById<TextView>(Resource.Id.modeDailyTrack_countOfLoc);
            TimeStatTextView = view.FindViewById<TextView>(Resource.Id.modeDailyTrack_timeStats);
            TrackingDescTextView = view.FindViewById<TextView>(Resource.Id.dailyTrack_trackingDesc);
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.dailyTrack_onEdition);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeDailyTrack_onMap);
            AddTimeFloatingActionButton = view.FindViewById<FloatingActionButton>(Resource.Id.modeDailyTrack_floatingActionButton);
            TimerListView = view.FindViewById<ListView>(Resource.Id.modeDailyTrack_listview);
            InfoListTextView = view.FindViewById<TextView>(Resource.Id.modeDailyTrack_infoList);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            App.Locator.ModeDailyTrack.InitMode();
            InitialiseTimerListView();

            if (App.Locator.ModeDailyTrack.IsWaitingForValidation)
            {
                BottomGridView.Visibility = ViewStates.Gone;
                NoLocationTextView.Visibility = ViewStates.Gone;
                TrackingDescTextView.Visibility = ViewStates.Visible;

                EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
                MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                TrackingDescTextView.Visibility = ViewStates.Gone;
                TrackingMarkerSelection.Max = App.Locator.ModeDailyTrack.LsLocations.Count - 1;
                TrackingMarkerSelection.Progress = App.Locator.ModeDailyTrack.LsLocations.Count - 1;
                BottomGridView.Visibility = App.Locator.ModeDailyTrack.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                NoLocationTextView.Visibility = App.Locator.ModeDailyTrack.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
                FirstDateTextView.Text = App.Locator.ModeDailyTrack.TrackingFirstDate;
                LastDateTextView.Text = App.Locator.ModeDailyTrack.TrackingLastDate;

                InitializeActivateSinceTextView();
                InitializeCountOfLocTextView();
                InitializeRefreshTimeStatTextView();
                EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
                MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
            }
        }

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// Initialise time til location tracking started
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
        /// Initialise count of Locations
        /// </summary>
        private void InitializeCountOfLocTextView()
        {
            var countOfTriggeredAlert = App.Locator.ModeDailyTrack.LsLocations.Count;
            var textCountOfLoc = string.Format(Resources.GetString(Resource.String.modeTracking_countOfLocStat), countOfTriggeredAlert);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfLoc);
            var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfLoc);
            formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            CountOfLocTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// Initialise the defined timers textview
        /// </summary>
        private void InitializeRefreshTimeStatTextView()
        {
            _listOfStr.Clear();

            foreach (var time in App.Locator.ModeDailyTrack.TimePickerList)
            {
                _listOfStr.Add(time.ToString());
            }
            var timerString = string.Join(", ", _listOfStr);
            TimeStatTextView.Text = string.Format(Resources.GetString(Resource.String.dailyTrack_timeStats), timerString);
        }

        /// <summary>
        /// Intialise list view
        /// </summary>
        private void InitialiseTimerListView()
        {
            _listAdapter = new DailyTrackFragmentAdapter(this);
            TimerListView.Adapter = _listAdapter;
            TimerListView.ChoiceMode = ChoiceMode.Single;
            TimerListView.ItemsCanFocus = true;
        }

        #endregion

        #region ===== Events ======================================================================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTimeFloatingActionButton_Click(object sender, EventArgs e)
        {
            if (App.Locator.ModeDailyTrack.TimePickerList == null)
                App.Locator.ModeDailyTrack.TimePickerList = new List<Time>();

            //if seekios = freemium : only 1 timer can be added
            //if seekios = premium : 4 timers can be configured
            if (App.Locator.ModeDailyTrack.TimePickerList.Count == 0 && MapViewModelBase.Seekios.Subscription_idsubscription == 2
                || App.Locator.ModeDailyTrack.TimePickerList.Count < 4 && MapViewModelBase.Seekios.Subscription_idsubscription == 1)
            {
                TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (Time time)
                {
                    //add timers if they do not already exist
                    if (!App.Locator.ModeDailyTrack.TimePickerList.Contains(time))
                        App.Locator.ModeDailyTrack.TimePickerList.Add(time);
                    UpdateUI();

                }, DateTime.Now);
                frag.Show(FragmentManager, TimePickerFragment.TAG);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeDailyTrack_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ItemDeleted":
                    UpdateUI();
                    break;
                case "LsLocations":
                    TrackingMarkerSelection.Max = App.Locator.ModeDailyTrack.LsLocations.Count - 1;
                    TrackingMarkerSelection.Progress = App.Locator.ModeDailyTrack.LsLocations.Count - 1;
                    BottomGridView.Visibility = App.Locator.ModeDailyTrack.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
                    NoLocationTextView.Visibility = App.Locator.ModeDailyTrack.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
                    InitializeCountOfLocTextView();
                    break;
                case "TrackingFirstDate": FirstDateTextView.Text = App.Locator.ModeDailyTrack.TrackingFirstDate; break;
                case "TrackingLastDate": LastDateTextView.Text = App.Locator.ModeDailyTrack.TrackingLastDate; break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUI()
        {
            _listAdapter.NotifyDataSetChanged();
            if (App.Locator.ModeDailyTrack.TimePickerList.Count > 0 && MapViewModelBase.Seekios.Subscription_idsubscription == 2
                || App.Locator.ModeDailyTrack.TimePickerList.Count >= 4 && MapViewModelBase.Seekios.Subscription_idsubscription == 1)
            {
                InfoListTextView.Text = Resources.GetString(Resource.String.dailyTrack_cannotAddTimer);
                AddTimeFloatingActionButton.ColorNormal = Resources.GetColor(Resource.Color.lightGray);
                AddTimeFloatingActionButton.Clickable = false;
            }
            else
            {
                InfoListTextView.Text = string.Empty;
                AddTimeFloatingActionButton.ColorNormal = Resources.GetColor(Resource.Color.primary);
                AddTimeFloatingActionButton.Clickable = true;
            }
        }

        /// <summary>
        /// Change the value of the location when the seekbar is used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrackingMarkerSelectionChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (App.Locator.ModeDailyTrack.LsLocations.Count <= 0) return;
            App.Locator.ModeDailyTrack.SelectedLocation = App.Locator.ModeDailyTrack.LsLocations[TrackingMarkerSelection.Progress];
        }

        #endregion
    }
}