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
using SeekiosApp.ViewModel;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using SeekiosApp.Model.APP;
using SeekiosApp.Droid.CustomComponents.Adapter;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.View.FragmentView
{
    public class InTimeMetaDataFragment : Android.Support.V4.App.Fragment
    {
        #region ===== Attributs ===================================================================

        private InTimeFragmentAdapter _listAdapter;

        private Context _context;

        private List<string> _listOfStr = new List<string>();

        public InTimeMetaDataFragment(Context context)
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
            var view = inflater.Inflate(Resource.Layout.InTimeMetaDataFragmentLayout, container, false);

            GetObjectsFromView(view);
            SetDataToView();

            //if (App.Locator.ModeInTime.IsWaitingForValidation)
            //{
            //    UpdateUI();
            //}

            return view;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnResume()
        {
            //AddTimeFloatingActionButton.Click += AddTimeFloatingActionButton_Click;
            //App.Locator.ModeInTime.PropertyChanged += ModeInTime_PropertyChanged;
            //TrackingMarkerSelection.ProgressChanged += OnTrackingMarkerSelectionChanged;
            base.OnResume();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            //AddTimeFloatingActionButton.Click -= AddTimeFloatingActionButton_Click;
            //App.Locator.ModeInTime.PropertyChanged -= ModeInTime_PropertyChanged;
            //TrackingMarkerSelection.ProgressChanged -= OnTrackingMarkerSelectionChanged;
            base.OnPause();
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            TrackingMarkerSelection = view.FindViewById<SeekBar>(Resource.Id.inTime_trackingMarkerSelection);
            FirstDateTextView = view.FindViewById<TextView>(Resource.Id.inTime_trackingFirstDate);
            LastDateTextView = view.FindViewById<TextView>(Resource.Id.inTime_trackingLastDate);
            BottomGridView = view.FindViewById<GridLayout>(Resource.Id.inTime_bottomGridView);
            NoLocationTextView = view.FindViewById<TextView>(Resource.Id.inTime_noLocation);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeinTime_activateSince);
            CountOfLocTextView = view.FindViewById<TextView>(Resource.Id.modeinTime_countOfLoc);
            TrackingDescTextView = view.FindViewById<TextView>(Resource.Id.inTime_trackingDesc);
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.inTime_onEdition);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeinTime_onMap);
            AddTimeFloatingActionButton = view.FindViewById<FloatingActionButton>(Resource.Id.modeinTime_floatingActionButton);
            TimerListView = view.FindViewById<ListView>(Resource.Id.modeinTime_listview);
            InfoListTextView = view.FindViewById<TextView>(Resource.Id.modeinTime_infoList);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            //App.Locator.ModeInTime.InitMode();
            //InitialiseTimerListView();

            //if (App.Locator.ModeInTime.IsWaitingForValidation)
            //{
            //    BottomGridView.Visibility = ViewStates.Gone;
            //    NoLocationTextView.Visibility = ViewStates.Gone;
            //    TrackingDescTextView.Visibility = ViewStates.Visible;

            //    EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
            //    MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
            //}
            //else
            //{
            //    TrackingDescTextView.Visibility = ViewStates.Gone;
            //    TrackingMarkerSelection.Max = App.Locator.ModeInTime.LsLocations.Count - 1;
            //    TrackingMarkerSelection.Progress = App.Locator.ModeInTime.LsLocations.Count - 1;
            //    BottomGridView.Visibility = App.Locator.ModeInTime.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
            //    NoLocationTextView.Visibility = App.Locator.ModeInTime.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            //    FirstDateTextView.Text = App.Locator.ModeInTime.TrackingFirstDate;
            //    LastDateTextView.Text = App.Locator.ModeInTime.TrackingLastDate;

            //    InitializeActivateSinceTextView();
            //    InitializeCountOfLocTextView();
            //    EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
            //    MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
            //}
        }

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// Initialise time til location tracking started
        /// </summary>
        private void InitializeActivateSinceTextView()
        {
            if (MapViewModelBase.Mode != null)
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
        }

        /// <summary>
        /// Initialise count of Locations
        /// </summary>
        private void InitializeCountOfLocTextView()
        {
            //var countOfTriggeredAlert = App.Locator.ModeInTime.LsLocations.Count;
            //var textCountOfLoc = string.Format(Resources.GetString(Resource.String.modeTracking_countOfLocStat), countOfTriggeredAlert);

            //var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfLoc);
            //var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfLoc);
            //formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            //CountOfLocTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// Intialise list view
        /// </summary>
        private void InitialiseTimerListView()
        {
            _listAdapter = new InTimeFragmentAdapter(this);
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
            //if (App.Locator.ModeInTime.TimePickerList == null)
            //    App.Locator.ModeInTime.TimePickerList = new List<TimeDay>();

            //max 3 timers per day
            //if (App.Locator.ModeInTime.TimePickerList.Count < 3)
            //{
            //    TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (TimeDay time)
            //    {
            //        //add timers if they do not already exist
            //        if (!App.Locator.ModeInTime.TimePickerList.Exists(el => el.Equals(time)))
            //            App.Locator.ModeInTime.TimePickerList.Add(time);
            //        else App.Locator.ModeInTime.ShowErrorMessage(ErrorCode.CustomError, Resources.GetString(Resource.String.inTime_timerAlreadyExist_title),
            //                                                        Resources.GetString(Resource.String.inTime_timerAlreadyExist));
            //        UpdateUI();

            //    }, DateTime.Now);
            //    frag.Show(FragmentManager, TimePickerFragment.TAG);
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeInTime_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //switch (e.PropertyName)
            //{
            //    case "ItemDeleted":
            //        UpdateUI();
            //        break;
            //    case "LsLocations":
            //        TrackingMarkerSelection.Max = App.Locator.ModeInTime.LsLocations.Count - 1;
            //        TrackingMarkerSelection.Progress = App.Locator.ModeInTime.LsLocations.Count - 1;
            //        BottomGridView.Visibility = App.Locator.ModeInTime.LsLocations.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
            //        NoLocationTextView.Visibility = App.Locator.ModeInTime.LsLocations.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
            //        InitializeCountOfLocTextView();
            //        break;
            //    case "TrackingFirstDate": FirstDateTextView.Text = App.Locator.ModeInTime.TrackingFirstDate; break;
            //    case "TrackingLastDate": LastDateTextView.Text = App.Locator.ModeInTime.TrackingLastDate; break;
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateUI()
        {
            //_listAdapter.NotifyDataSetChanged();
            //if (App.Locator.ModeInTime.TimePickerList.Count > 2)
            //{
            //    InfoListTextView.Text = Resources.GetString(Resource.String.inTime_cannotAddTimer);
            //    AddTimeFloatingActionButton.ColorNormal = Resources.GetColor(Resource.Color.lightGray);
            //    AddTimeFloatingActionButton.Clickable = false;
            //}
            //else
            //{
            //    InfoListTextView.Text = string.Empty;
            //    AddTimeFloatingActionButton.ColorNormal = Resources.GetColor(Resource.Color.primary);
            //    AddTimeFloatingActionButton.Clickable = true;
            //}
        }

        /// <summary>
        /// Change the value of the location when the seekbar is used
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTrackingMarkerSelectionChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //if (App.Locator.ModeInTime.LsLocations.Count <= 0) return;
            //App.Locator.ModeInTime.SelectedLocation = App.Locator.ModeInTime.LsLocations[TrackingMarkerSelection.Progress];
        }

        #endregion
    }
}