using System;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Globalization;
using System.Threading;
using SeekiosApp.ViewModel;
using Android.Gms.Maps;
using SeekiosApp.Droid.ControlManager;
using SeekiosApp.Droid.View.FragmentView;
using SeekiosApp.Helper;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class MapHistoricActivity : MapBaseActivity
    {
        #region ===== Attributs ===================================================================

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Date start location</summary>
        public TextView UpperDateTextView { get; set; }

        /// <summary>Date end location</summary>
        public TextView LowerDateTextView { get; set; }

        /// <summary>Next position</summary>
        public TextView NextPositionButton { get; set; }

        /// <summary>Previous position</summary>
        public TextView PreviousPositionButton { get; set; }

        /// <summary>The current position of the marker on the total of the positions</summary>
        public TextView CurrentPositionTextView { get; set; }

        public TextView NoPositionsTextView { get; set; }

        /// <summary>Cursor to navigate through all the locations</summary>
        public SeekBar LocationsSeekBar { get; set; }

        /// <summary>Loading progress bar</summary>
        public ProgressBar LoadingProgressBar { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle
                , Resource.Layout.MapHistoricLayout
                , Resources.GetString(Resource.String.historic_title)
                , null);

            GetObjectsFromView();
            SetDataToView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpperDateTextView.Click += UpperDateTextView_Click;
            LowerDateTextView.Click += LowerDateTextView_Click;
            LocationsSeekBar.ProgressChanged += LocationsSeekBar_ProgressChanged;
            NextPositionButton.Click += NextPositionButton_Click;
            PreviousPositionButton.Click += PreviousPositionButton_Click;
            App.Locator.Historic.PropertyChanged += Historic_PropertyChanged;
            App.Locator.Historic.IsHistoryActivated = true;
        }

        protected override void OnPause()
        {
            base.OnPause();
            UpperDateTextView.Click -= UpperDateTextView_Click;
            LowerDateTextView.Click -= LowerDateTextView_Click;
            LocationsSeekBar.ProgressChanged -= LocationsSeekBar_ProgressChanged;
            NextPositionButton.Click -= NextPositionButton_Click;
            PreviousPositionButton.Click -= PreviousPositionButton_Click;
            App.Locator.Historic.PropertyChanged -= Historic_PropertyChanged;
            App.Locator.Historic.IsHistoryActivated = false;
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

        #region ===== Initialisze View ============================================================

        private void GetObjectsFromView()
        {
            UpperDateTextView = FindViewById<TextView>(Resource.Id.upperdate);
            LowerDateTextView = FindViewById<TextView>(Resource.Id.lowerdate);
            CurrentPositionTextView = FindViewById<TextView>(Resource.Id.historic_currentPosition);
            LocationsSeekBar = FindViewById<SeekBar>(Resource.Id.historic_seekbar);
            LoadingProgressBar = FindViewById<ProgressBar>(Resource.Id.loading_progressbar);
            NextPositionButton = FindViewById<TextView>(Resource.Id.historic_nextPosition);
            PreviousPositionButton = FindViewById<TextView>(Resource.Id.historic_previousPosition);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            NoPositionsTextView = FindViewById<TextView>(Resource.Id.historic_noPosition);
        }

        private void SetDataToView()
        {
            RefreshInProgressButton.Enabled = false;

            // Loading progress bar
            LoadingProgressBar.Visibility = ViewStates.Visible;

            // Set up the text of the current position
            CurrentPositionTextView.Text = string.Format(Resources.GetString(Resource.String.historic_currentPosition), 1, 1);

            // Set up the dates
            UpperDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_toDate), App.Locator.Historic.CurrentUpperDate.ToLocalTime().ToString("M"));
            LowerDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_fromDate), App.Locator.Historic.CurrentLowerDate.ToLocalTime().ToString("M"));

            // Get the data for one month ago
            var result = App.Locator.Historic.GetLocationsBySeekios(
                MapViewModelBase.Seekios.Idseekios
                , App.Locator.Historic.CurrentLowerDate
                , App.Locator.Historic.CurrentUpperDate);

            App.Locator.Historic.OnGetLocationsComplete += OnGetLocationsComplete;

            // Set the seekbar with default values
            LocationsSeekBar.Max = 0;
            LocationsSeekBar.Progress = 0;

            // If the data seekios has already been loaded
            if (App.Locator.Historic.LsSeekiosLocations != null && App.Locator.Historic.LsSeekiosLocations.Any(a => a.IdSeekios == MapViewModelBase.Seekios.Idseekios))
            {
                UpdateView();
                LoadingProgressBar.Visibility = ViewStates.Gone;
            }
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private async System.Threading.Tasks.Task OnGetLocationsComplete(object o, EventArgs e)
        {
            LoadingProgressBar.Visibility = ViewStates.Gone;
            if (App.Locator.Historic.LsSeekiosLocations != null && App.Locator.Historic.LsSeekiosLocations.Any(a => a.IdSeekios == MapViewModelBase.Seekios.Idseekios))
            {
                UpdateView();
            }
            else
            {
                CurrentPositionTextView.SetText(Resource.String.historic_noCurrentPosition);
                UpperDateTextView.Visibility = ViewStates.Gone;
                LowerDateTextView.Visibility = ViewStates.Gone;
                NextPositionButton.Visibility = ViewStates.Gone;
                PreviousPositionButton.Visibility = ViewStates.Gone;
                LocationsSeekBar.Visibility = ViewStates.Gone;
            }
            if ((bool)o)
            {
                LowerDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_fromDate), App.Locator.Historic.CurrentLowerDate.ToLocalTime().ToString("M"));
            }
        }

        /// <summary>
        /// Display a date picker popup to pick the start dates
        /// </summary>
        private void UpperDateTextView_Click(object sender, EventArgs e)
        {
            if (App.Locator.Historic != null && App.Locator.Historic.SelectedSeekiosLocations != null)
            {
                DatePickerFragment dialog = DatePickerFragment.NewInstance((upperDate) =>
                {
                    if (upperDate != App.Locator.Historic.CurrentUpperDate)
                    {
                        upperDate = DateHelper.GetLastSecondOfDay(upperDate);
                        //upperDate = upperDate.AddDays(1).AddSeconds(-1);
                        LoadingProgressBar.Visibility = ViewStates.Visible;
                        UpperDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_toDate), upperDate.Date.ToLocalTime().ToString("M"));
                        App.Locator.Historic.GetLocationsBySeekios(MapViewModelBase.Seekios.Idseekios, App.Locator.Historic.CurrentLowerDate, upperDate, true).Wait();
                        App.Locator.Historic.CurrentUpperDate = upperDate;
                    }
                }
                , App.Locator.Historic.CurrentUpperDate
                , App.Locator.Historic.CurrentLowerDate
                , DateHelper.GetSystemTime());

                dialog.Show(SupportFragmentManager, "UpperDate");
            }
        }

        /// <summary>
        /// Display a date picker popup to pick the end date
        /// </summary>
        private void LowerDateTextView_Click(object sender, EventArgs e)
        {
            if (App.Locator.Historic != null && App.Locator.Historic.SelectedSeekiosLocations != null)
            {
                DatePickerFragment dialog = DatePickerFragment.NewInstance((lowerDate) =>
                {
                    if (lowerDate != App.Locator.Historic.CurrentLowerDate)
                    {
                        LoadingProgressBar.Visibility = ViewStates.Visible;
                        LowerDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_toDate), lowerDate.Date.ToLocalTime().ToString("M"));
                        App.Locator.Historic.GetLocationsBySeekios(MapViewModelBase.Seekios.Idseekios, lowerDate, App.Locator.Historic.CurrentUpperDate, true).Wait();
                        App.Locator.Historic.CurrentLowerDate = lowerDate;
                    }
                }
                , App.Locator.Historic.CurrentLowerDate
                , App.Locator.Historic.SelectedSeekiosLocations.LimitLowerDate ?? DateTime.MinValue
                , App.Locator.Historic.CurrentUpperDate);

                dialog.Show(SupportFragmentManager, "LowerDate");
            }
        }

        /// <summary>
        /// Update the view
        /// </summary>
        private void UpdateView()
        {
            var item = App.Locator.Historic.SelectedSeekiosLocations;
            //var o = App.Locator.Historic.LsSeekiosLocations[0];
            if (item != null)
            {
                if (App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count > 0)
                {
                    App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count - 1];
                    if (LocationsSeekBar != null) LocationsSeekBar.Enabled = true;
                    LocationsSeekBar.Max = item.LsLocations.Count - 1;
                    LocationsSeekBar.Progress = item.LsLocations.Count - 1;

                    var result = string.Format(Resources.GetString(Resource.String.historic_currentPosition), item.LsLocations.Count, item.LsLocations.Count);
                    if (CurrentPositionTextView != null) CurrentPositionTextView.Text = result;

                    //if (UpperDateTextView != null) UpperDateTextView.Visibility = ViewStates.Visible;
                    //if (LowerDateTextView != null) LowerDateTextView.Visibility = ViewStates.Visible;
                    if (NextPositionButton != null) NextPositionButton.Visibility = ViewStates.Visible;
                    if (PreviousPositionButton != null) PreviousPositionButton.Visibility = ViewStates.Visible;
                    //if (UpperDateLabelTextView != null) UpperDateLabelTextView.Visibility = ViewStates.Visible;
                    //if (LowerDateLabelTextView != null) LowerDateLabelTextView.Visibility = ViewStates.Visible;
                    //if (LocationsSeekBar != null) LocationsSeekBar.Visibility = ViewStates.Visible;

                    if (NoPositionsTextView != null)  NoPositionsTextView.Visibility = ViewStates.Gone;
                }
                else
                {
                    if (NoPositionsTextView != null)
                    {
                        NoPositionsTextView.Visibility = ViewStates.Visible;
                        NoPositionsTextView.SetText(Resource.String.historic_noCurrentPosition);
                    }
                    if (CurrentPositionTextView != null)  CurrentPositionTextView.Visibility = ViewStates.Gone;
                    //CurrentPositionTextView.SetText(Resource.String.historic_noCurrentPosition);
                    //if (UpperDateTextView != null) UpperDateTextView.Visibility = ViewStates.Gone;
                    //if (UpperDateLabelTextView != null) UpperDateLabelTextView.Visibility = ViewStates.Gone;
                    //if (LowerDateTextView != null) LowerDateTextView.Visibility = ViewStates.Gone;
                    //if (LowerDateLabelTextView != null) LowerDateLabelTextView.Visibility = ViewStates.Gone;
                    if (NextPositionButton != null) NextPositionButton.Visibility = ViewStates.Gone;
                    if (PreviousPositionButton != null) PreviousPositionButton.Visibility = ViewStates.Gone;
                    if (LocationsSeekBar != null) LocationsSeekBar.Enabled = false;
                    //if (LocationsSeekBar != null) LocationsSeekBar.Visibility = ViewStates.Gone;
                }
            }
        }

        /// <summary>
        /// Update the current position in the text view
        /// </summary>
        private void UpdateCurrentPosition(int index)
        {
            App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[index];
            CurrentPositionTextView.Text = string.Format(Resources.GetString(Resource.String.historic_currentPosition)
                , index + 1
                , App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count);
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Trigger on the ViewModel 
        /// </summary>
        private void Historic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LsLocations")
            {
                var item = App.Locator.Historic.LsSeekiosLocations.FirstOrDefault(f => f.IdSeekios == MapViewModelBase.Seekios.Idseekios);
                if (item != null && item.LimitLowerDate > App.Locator.Historic.CurrentLowerDate)
                    LowerDateTextView.Text = string.Format(Resources.GetString(Resource.String.historic_fromDate), item.LimitLowerDate.Value.ToLocalTime().ToString("M"));
                UpdateView();
                LoadingProgressBar.Visibility = ViewStates.Gone;
            }
        }


        /// <summary>
        /// Seek bar position changed
        /// </summary>
        private void LocationsSeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (App.Locator.Historic.SelectedSeekiosLocations.LsLocations != null
                && App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count > 0)
            {
                App.Locator.Historic.SelectedLocationHistory = App.Locator.Historic.SelectedSeekiosLocations.LsLocations[LocationsSeekBar.Progress];
                CurrentPositionTextView.Text = string.Format(Resources.GetString(Resource.String.historic_currentPosition)
                    , LocationsSeekBar.Progress + 1
                    , App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count);
            }
        }

        /// <summary>
        /// Display the next position
        /// </summary>
        private void NextPositionButton_Click(object sender, EventArgs e)
        {
            if (App.Locator.Historic.SelectedSeekiosLocations != null
                    && LocationsSeekBar.Progress < App.Locator.Historic.SelectedSeekiosLocations.LsLocations.Count - 1)
            {
                var index = LocationsSeekBar.Progress + 1;
                LocationsSeekBar.Progress = index;
                UpdateCurrentPosition(index);
            }
        }

        /// <summary>
        /// Display the previous position
        /// </summary>
        private void PreviousPositionButton_Click(object sender, EventArgs e)
        {
            if (LocationsSeekBar.Progress > 0)
            {
                var index = LocationsSeekBar.Progress - 1;
                LocationsSeekBar.Progress = index;
                UpdateCurrentPosition(index);
            }
        }

        #endregion

        #region ===== Callback ====================================================================

        /// <summary>
        /// Callback d'initialisation de la map
        /// </summary>
        public override void OnMapReady(GoogleMap googleMap)
        {
            if (googleMap != null)
            {
                MapWrapperLayout.Init(googleMap);

                // Initialisation de la map
                _mapControlManager = new MapControlManager(googleMap
                    , this
                    , null
                    , null
                    , CenterSeekiosSvgImageView
                    , MapViewModelBase.Seekios.Idseekios.ToString());
                App.Locator.Historic.MapControlManager = _mapControlManager;
                App.Locator.Historic.InitMap();

                _mapControlManager.RegisterMethodes();
            }
            base.OnMapReady(googleMap);
        }

        #endregion
    }
}