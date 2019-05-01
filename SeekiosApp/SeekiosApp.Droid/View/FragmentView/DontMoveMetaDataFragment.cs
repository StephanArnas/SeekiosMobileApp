using System;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Support.V4.App;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Droid.Services;
using SeekiosApp.Extension;
using SeekiosApp.ViewModel;
using SeekiosApp.Interfaces;

namespace SeekiosApp.Droid.View.FragmentView
{
    class DontMoveMetaDataFragment : Fragment
    {
        #region ===== Propriétées =================================================================

        public TextView AddressTextView { get; set; }

        public LinearLayout EditMetaDataLinearLayout { get; set; }

        public Switch TrackingAfterMoveSwitch { get; set; }

        public Spinner RefreshTimeSpinner { get; set; }

        public LinearLayout MapMetaDataLinearLayout { get; set; }

        public TextView ActivateSinceTextView { get; set; }

        public TextView CountOfOutAlertTextView { get; set; }

        public TextView TrackingRefreshTimeStat { get; set; }

        public RelativeLayout RestartModeRelativeLayout { get; set; }

        public Button RestartModeButton { get; set; }

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
            var view = inflater.Inflate(Resource.Layout.DontMoveMetaDataFragmentLayout, container, false);

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
            MapViewModelBase.OnSeekiosRefreshRequestEnded += OnSeekiosRefreshRequestEnded;
            DisplayActualSeekiosLocationAdress();
            App.Locator.ModeDontMove.SeekiosMovedNotified += OnSeekiosMovedNotified;
            TrackingAfterMoveSwitch.CheckedChange += OnTrackingAfterMoveSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected += OnRefreshTimeSpinnerItemSelected;
            RestartModeButton.Click += OnRestartModeButtonClick;
            App.Locator.ModeDontMove.PropertyChanged += OnModeDontMovePropertyChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            MapViewModelBase.OnSeekiosRefreshRequestEnded -= OnSeekiosRefreshRequestEnded;
            App.Locator.ModeDontMove.SeekiosMovedNotified -= OnSeekiosMovedNotified;
            TrackingAfterMoveSwitch.CheckedChange -= OnTrackingAfterMoveSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected -= OnRefreshTimeSpinnerItemSelected;
            RestartModeButton.Click -= OnRestartModeButtonClick;
            App.Locator.ModeDontMove.PropertyChanged -= OnModeDontMovePropertyChanged;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            AddressTextView = view.FindViewById<TextView>(Resource.Id.modeDontMove_address);
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeDontMove_onEdition);
            TrackingAfterMoveSwitch = view.FindViewById<Switch>(Resource.Id.modeDontMove_trackingAfterMoveSwitch);
            RefreshTimeSpinner = view.FindViewById<Spinner>(Resource.Id.modeDontMove_trackingRefreshTimeSpinner);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeDontMove_onMap);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeDontMove_activateSince);
            CountOfOutAlertTextView = view.FindViewById<TextView>(Resource.Id.modeDontMove_countOfMove);
            TrackingRefreshTimeStat = view.FindViewById<TextView>(Resource.Id.modeDontMove_trackingRefreshTimeStat);
            RestartModeRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.modeDontMove_restartModeLayout);
            RestartModeButton = view.FindViewById<Button>(Resource.Id.modeDontMove_restartMode);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            //Initialise le mode don't move 
            App.Locator.ModeDontMove.InitModeDontMove();

            if (App.Locator.ModeDontMove.IsWaitingForValidation)
            {
                var adapter = ArrayAdapter.CreateFromResource(Context
                    , Resource.Array.refreshTime_array
                    , Resource.Layout.AddSeekiosSpinnerText);
                adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
                RefreshTimeSpinner.Adapter = adapter;
                TrackingAfterMoveSwitch.Checked = App.Locator.ModeDontMove.IsTrackingAfterMove;
                RefreshTimeSpinner.Enabled = TrackingAfterMoveSwitch.Checked;
                EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
                MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
                SelectCorrectRefreshTimeItem();
            }
            else
            {
                EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
                MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
                InitializeActivateSinceTextView();
                InitializeCountOfOutAlertTextView();
                if (App.Locator.ModeDontMove.IsTrackingAfterMove)
                {
                    TrackingRefreshTimeStat.Visibility = ViewStates.Visible;
                    InitializeTrackingRefreshTimeStatTextView();
                    RestartModeRelativeLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    TrackingRefreshTimeStat.Visibility = ViewStates.Gone;
                    if (MapViewModelBase.Mode != null)
                        RestartModeRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 3 ? ViewStates.Visible : ViewStates.Gone;
                }
            }
            DisplayActualSeekiosLocationAdress();
        }

        #endregion

        #region ===== Méthodes Privées ============================================================

        /// <summary>
        /// Affiche l'adresse postale du seekios
        /// </summary>
        private void DisplayActualSeekiosLocationAdress()
        {
            try
            {
                var lsLoc = new Geocoder(Context).GetFromLocation(MapViewModelBase.Seekios.LastKnownLocation_latitude, MapViewModelBase.Seekios.LastKnownLocation_longitude, 1);
                if (lsLoc.Count > 0) AddressTextView.Text = lsLoc[0].SubThoroughfare + " " + lsLoc[0].Thoroughfare + " " + lsLoc[0].PostalCode + " " + lsLoc[0].Locality + " " + lsLoc[0].CountryName;
            }
            catch (Exception ex)
            {

            }
        }

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
        private void InitializeCountOfOutAlertTextView()
        {
            var countOfTriggeredAlert = MapViewModelBase.Mode.CountOfTriggeredAlert;
            var textCountOfOutAlert = string.Format(Resources.GetString(Resource.String.modeDontMove_moveAlert), countOfTriggeredAlert);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfOutAlert);
            var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfOutAlert);
            formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            CountOfOutAlertTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeTrackingRefreshTimeStatTextView()
        {
            if (TrackingRefreshTimeStat == null) return;
            if (MapViewModelBase.Mode == null) return;

            var refreshTimeStat = StringExtension.secondeToString(MapViewModelBase.RefreshTime * 60);
            var textTrackingRefreshTimeStat = string.Format(Resources.GetString(Resource.String.modeDontMove_trackingRefreshTimeStat), refreshTimeStat);

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
        private void OnTrackingAfterMoveSwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RefreshTimeSpinner.Enabled = TrackingAfterMoveSwitch.Checked;
            App.Locator.ModeDontMove.IsTrackingAfterMove = TrackingAfterMoveSwitch.Checked;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRestartModeButtonClick(object sender, EventArgs e)
        {
            App.Locator.ModeDontMove.OnRestartDontMove();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnModeDontMovePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Mode":
                    if (MapViewModelBase.Mode == null) break;
                    RestartModeRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 3 ? ViewStates.Visible : ViewStates.Gone;
                    break;
            }
        }

        #endregion

        #region ===== Handlers ====================================================================

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSeekiosRefreshRequestEnded(object sender, System.EventArgs e)
        {
            (ServiceLocator.Current.GetInstance<IDispatchOnUIThread>() as DispatchService).Invoke(() =>
            {
                DisplayActualSeekiosLocationAdress();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnSeekiosMovedNotified()
        {
            InitializeCountOfOutAlertTextView();
            RestartModeRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 3 ? ViewStates.Visible : ViewStates.Gone;
        }

        #endregion
    }
}