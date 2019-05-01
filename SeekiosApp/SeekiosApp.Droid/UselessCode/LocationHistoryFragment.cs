//using Android.OS;
//using Android.Support.V4.App;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.ViewModel;
//using System;
//using System.Linq;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    class LocationHistoryFragment : Fragment
//    {
//        #region ===== Attribut ====================================================================

//        private MapViewModelBase _mapBase;
//        private string _dateFormat = string.Empty;
//        private string _countOfLocHistoryMany = string.Empty;
//        private string _countOfLocHistoryOne = string.Empty;
//        private string _countOfLocHistoryZero = string.Empty;

//        #endregion

//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="mapBase"></param>
//        public LocationHistoryFragment(MapViewModelBase mapBase)
//        {
//            _mapBase = mapBase;
//        }

//        #endregion

//        #region ===== Propriétées =================================================================

//        public GridLayout LocContainerGridView { get; set; }

//        public SeekBar LocSliderSeekBar { get; set; }

//        //public TextView LocFirstDateTextView { get; set; }

//        //public TextView LocLastDateTextView { get; set; }

//        public TextView CountOfLocHistoryTextView { get; set; }

//        public TextView NoLocationTextView { get; set; }

//        //public RangeSliderView DateRangeSelector { get; set; }

//        public TextView LocLowerDateTextView { get; set; }

//        public TextView LocUpperDateTextView { get; set; }


//        #endregion

//        #region ===== Cycle De Vie ================================================================

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="savedInstanceState"></param>
//        public override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="inflater"></param>
//        /// <param name="container"></param>
//        /// <param name="savedInstanceState"></param>
//        /// <returns></returns>
//        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            var view = inflater.Inflate(Resource.Layout.LocationHistoryFragmentLayout, container, false);

//            GetObjectsFromView(view);
//            SetDataToView();

//            return view;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override void OnResume()
//        {
//            base.OnResume();
//            _mapBase.PropertyChanged += OnBaseMapPropertyChanged;
//            LocSliderSeekBar.ProgressChanged += OnLocSliderSeekBarSelectionChanged;
//            LocLowerDateTextView.Click += OnLocLowerDateTextViewClick;
//            LocUpperDateTextView.Click += OnLocUpperDateTextViewClick;
//            //DateRangeSelector.Touch += OnDateRangeSelectorTouch;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            _mapBase.PropertyChanged -= OnBaseMapPropertyChanged;
//            LocSliderSeekBar.ProgressChanged -= OnLocSliderSeekBarSelectionChanged;
//            LocLowerDateTextView.Click -= OnLocLowerDateTextViewClick;
//            LocUpperDateTextView.Click -= OnLocUpperDateTextViewClick;
//            //DateRangeSelector.Touch -= OnDateRangeSelectorTouch;
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            LocSliderSeekBar = view.FindViewById<SeekBar>(Resource.Id.locHistory_locSlider);
//            //LocFirstDateTextView = view.FindViewById<TextView>(Resource.Id.locHistory_locFirstDate);
//            //LocLastDateTextView = view.FindViewById<TextView>(Resource.Id.locHistory_locLastDate);
//            CountOfLocHistoryTextView = view.FindViewById<TextView>(Resource.Id.locHistory_countOfLocHistory);
//            LocContainerGridView = view.FindViewById<GridLayout>(Resource.Id.locHistory_locSliderContainer);
//            NoLocationTextView = view.FindViewById<TextView>(Resource.Id.locHistory_noLocation);
//            LocLowerDateTextView = view.FindViewById<TextView>(Resource.Id.locHistory_loclowerDate);
//            LocUpperDateTextView = view.FindViewById<TextView>(Resource.Id.locHistory_locUpperDate);
//            //DateRangeSelector = view.FindViewById<RangeSliderView>(Resource.Id.locHistory_dateRangeSelector);
//        }

//        /// <summary>
//        /// Initialise les objets de la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            _dateFormat = Resources.GetString(Resource.String.locHistory_dateFormat);
//            _countOfLocHistoryMany = Resources.GetString(Resource.String.locHistory_countOfLocHistoryMany);
//            _countOfLocHistoryOne = Resources.GetString(Resource.String.locHistory_countOfLocHistoryOne);
//            _countOfLocHistoryZero = Resources.GetString(Resource.String.locHistory_countOfLocHistoryZero);

//            // On donne la valeur par défaut de la fourchette basse et haute des dates encadrant l'historique des localisations
//            if (_mapBase.LsLocHistory.Count > 0)
//            {
//                // Si historique il y a, ces dates sont définis par la plus ancienne et la plus récente localisation
//                _mapBase.LowerLocDate = _mapBase.LsLocHistory.Select(el => el.DateLocationCreation).FirstOrDefault();
//                _mapBase.UpperLocDate = _mapBase.LsLocHistory.Select(el => el.DateLocationCreation).LastOrDefault();
//            }
//            else
//            {
//                // Sinon c'est la date du jour
//                _mapBase.LowerLocDate = DateTime.Now;
//                _mapBase.UpperLocDate = DateTime.Now;
//            }

//            //LocFirstDateTextView.Text = _mapBase.FirstLocDateStr;
//            //LocLastDateTextView.Text = _mapBase.LastLocDateStr;

//            LocLowerDateTextView.Text = _mapBase.LowerLocDate.ToString(_dateFormat);
//            LocUpperDateTextView.Text = _mapBase.UpperLocDate.ToString(_dateFormat);

//            RefreshLocSlider();
//        }

//        #endregion

//        #region ===== Handlers ====================================================================

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnBaseMapPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            switch (e.PropertyName)
//            {
//                //case "FirstLocDateStr": LocFirstDateTextView.Text = _mapBase.FirstLocDateStr; break;
//                //case "LastLocDateStr": LocLastDateTextView.Text = _mapBase.LastLocDateStr; break;
//                case "LowerLocDate":
//                    RefreshLocSlider();
//                    LocLowerDateTextView.Text = _mapBase.LowerLocDate.ToString(_dateFormat);
//                    break;
//                case "UpperLocDate":
//                    RefreshLocSlider();
//                    LocUpperDateTextView.Text = _mapBase.UpperLocDate.ToString(_dateFormat);
//                    break;
//                case "LsLocHistory":
//                    RefreshLocSlider();
//                    break;
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void RefreshLocSlider()
//        {
//            LocContainerGridView.Visibility = _mapBase.LsLocHistory.Count > 0 ? ViewStates.Visible : ViewStates.Gone;
//            NoLocationTextView.Visibility = _mapBase.LsLocHistory.Count > 0 ? ViewStates.Gone : ViewStates.Visible;
//            if (_mapBase.LsLocHistory.Count != 0)
//            {
//                LocSliderSeekBar.Max = _mapBase.LsLocHistory.Count - 1;
//                LocSliderSeekBar.Progress = _mapBase.LsLocHistory.Count - 1;
//                _mapBase.SelectedLocationHistory = _mapBase.LsLocHistory.Last();
//            }
//            else
//            {
//                _mapBase.SelectedLocationHistory = null;
//                _mapBase.RemoveHistoryMarkers();
//            }

//            var countOfLocationHistory = _mapBase.LsLocHistory.Count();
//            if (countOfLocationHistory > 1) CountOfLocHistoryTextView.Text = string.Format(_countOfLocHistoryMany, countOfLocationHistory);
//            else if (countOfLocationHistory == 1) CountOfLocHistoryTextView.Text = string.Format(_countOfLocHistoryOne, countOfLocationHistory);
//            else CountOfLocHistoryTextView.Text = string.Format(_countOfLocHistoryZero, countOfLocationHistory);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnLocSliderSeekBarSelectionChanged(object sender, SeekBar.ProgressChangedEventArgs e)
//        {
//            if (_mapBase.LsLocHistory.Count <= 0) return;
//            _mapBase.SelectedLocationHistory = _mapBase.LsLocHistory[LocSliderSeekBar.Progress];
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void OnLocLowerDateTextViewClick(object sender, System.EventArgs e)
//        {
//            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
//            {
//                if (time > _mapBase.UpperLocDate) _mapBase.UpperLocDate = time;
//                _mapBase.LowerLocDate = time;
//            }, _mapBase.LowerLocDate);
//            frag.Show(FragmentManager, DatePickerFragment.TAG);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnLocUpperDateTextViewClick(object sender, EventArgs e)
//        {
//            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
//            {
//                if (time < _mapBase.LowerLocDate) _mapBase.LowerLocDate = time;
//                _mapBase.UpperLocDate = time;
//            }, _mapBase.UpperLocDate);
//            frag.Show(FragmentManager, DatePickerFragment.TAG);
//        }

//        #endregion
//    }
//}