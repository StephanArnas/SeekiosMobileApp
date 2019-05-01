using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.ViewModel;
using SeekiosApp.Interfaces;
using SeekiosApp.Droid.Services;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Extension;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using Android.Support.V4.App;

namespace SeekiosApp.Droid.View.FragmentView
{
    class FollowMeMetaDataFragment : Fragment
    {
        #region ===== Propriétées =================================================================

        public TextView StatusTextView { get; set; }

        public LinearLayout EditMetaDataLinearLayout { get; set; }

        public Switch TrackingAfterDisconnectSwitch { get; set; }

        public Spinner RefreshTimeSpinner { get; set; }

        public LinearLayout MapMetaDataLinearLayout { get; set; }

        public TextView ActivateSinceTextView { get; set; }

        public TextView CountOfConnectionLossAlertTextView { get; set; }

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
            var view = inflater.Inflate(Resource.Layout.FollowMeMetaDataFragmentLayout, container, false);

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
            TrackingAfterDisconnectSwitch.CheckedChange += OnTrackingAfterDisconnectSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected += OnRefreshTimeSpinnerItemSelected;
            RestartModeButton.Click += OnRestartModeButtonClick;
            App.Locator.ModeFollowMe.PropertyChanged += OnFollowMePropertyChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
            TrackingAfterDisconnectSwitch.CheckedChange -= OnTrackingAfterDisconnectSwitchCheckedChange;
            RefreshTimeSpinner.ItemSelected -= OnRefreshTimeSpinnerItemSelected;
            RestartModeButton.Click -= OnRestartModeButtonClick;
            App.Locator.ModeFollowMe.PropertyChanged -= OnFollowMePropertyChanged;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        /// <summary>
        /// Récupère les objets de la vue
        /// </summary>
        private void GetObjectsFromView(Android.Views.View view)
        {
            StatusTextView = view.FindViewById<TextView>(Resource.Id.modeFollowMe_status);
            EditMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeFollowMe_onEdition);
            TrackingAfterDisconnectSwitch = view.FindViewById<Switch>(Resource.Id.modeFollowMe_trackingAfterConnectionLossSwitch);
            RefreshTimeSpinner = view.FindViewById<Spinner>(Resource.Id.modeFollowMe_trackingRefreshTimeSpinner);
            MapMetaDataLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.modeFollowMe_onMap);
            ActivateSinceTextView = view.FindViewById<TextView>(Resource.Id.modeFollowMe_activateSince);
            CountOfConnectionLossAlertTextView = view.FindViewById<TextView>(Resource.Id.modeFollowMe_countOfDisconnect);
            TrackingRefreshTimeStat = view.FindViewById<TextView>(Resource.Id.modeFollowMe_trackingRefreshTimeStat);
            RestartModeRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.modeFollowMe_restartModeLayout);
            RestartModeButton = view.FindViewById<Button>(Resource.Id.modeFollowMe_restartMode);
        }

        /// <summary>
        /// Initialise les objets de la vue avec les données
        /// </summary>
        private void SetDataToView()
        {
            //Initialise le mode don't move 
            App.Locator.ModeFollowMe.InitModeFollowMe();

            if (App.Locator.ModeFollowMe.IsWaitingForValidation)
            {
                StatusTextView.SetText(Resource.String.modeFollowMe_explanations);
                var adapter = ArrayAdapter.CreateFromResource(Context
                    , Resource.Array.refreshTime_array
                    , Resource.Layout.AddSeekiosSpinnerText);
                adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
                RefreshTimeSpinner.Adapter = adapter;
                TrackingAfterDisconnectSwitch.Checked = App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss;
                RefreshTimeSpinner.Enabled = App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss;
                EditMetaDataLinearLayout.Visibility = ViewStates.Visible;
                MapMetaDataLinearLayout.Visibility = ViewStates.Gone;
                SelectCorrectRefreshTimeItem();
            }
            else
            {
                if (MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 1)
                    StatusTextView.SetText(Resource.String.modeFollowMe_seekiosReachable);
                else 
                    StatusTextView.SetText(Resource.String.modeFollowMe_seekiosUnreachable);

                EditMetaDataLinearLayout.Visibility = ViewStates.Gone;
                MapMetaDataLinearLayout.Visibility = ViewStates.Visible;
                InitializeActivateSinceTextView();
                InitializeCountOfConnectionLossAlertTextView();
                if (App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss)
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
        private void InitializeCountOfConnectionLossAlertTextView()
        {
            var countOfTriggeredAlert = MapViewModelBase.Mode.CountOfTriggeredAlert;
            var textCountOfOutAlert = string.Format(Resources.GetString(Resource.String.modeFollowMe_connectionLossAlert), countOfTriggeredAlert);

            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textCountOfOutAlert);
            var formattedTextCountOfTriggeredAlert = new SpannableString(textCountOfOutAlert);
            formattedTextCountOfTriggeredAlert.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.MainColor)), resultTuple.Item1, resultTuple.Item2, 0);
            CountOfConnectionLossAlertTextView.SetText(formattedTextCountOfTriggeredAlert, TextView.BufferType.Spannable);
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeTrackingRefreshTimeStatTextView()
        {
            if (TrackingRefreshTimeStat == null) return;
            if (MapViewModelBase.Mode == null) return;

            var refreshTimeStat = StringExtension.secondeToString(MapViewModelBase.RefreshTime * 60);
            var textTrackingRefreshTimeStat = string.Format(Resources.GetString(Resource.String.modeFollowMe_trackingRefreshTimeStat), refreshTimeStat);

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
        private void OnTrackingAfterDisconnectSwitchCheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            RefreshTimeSpinner.Enabled = TrackingAfterDisconnectSwitch.Checked;
            App.Locator.ModeFollowMe.IsTrackingAfterBLEConnexionLoss = TrackingAfterDisconnectSwitch.Checked;
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
            App.Locator.ModeFollowMe.OnRestartFollowMe();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFollowMePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Mode":
                    if (MapViewModelBase.Mode == null) break;
                    RestartModeRelativeLayout.Visibility = MapViewModelBase.Mode.StatusDefinition_idstatusDefinition == 3 ? ViewStates.Visible : ViewStates.Gone;
                    break;
                case "BLEConnexionState":
                    (ServiceLocator.Current.GetInstance<IDispatchOnUIThread>() as DispatchService).Invoke(() =>
                    {
                        switch (App.Locator.ModeFollowMe.BLEConnexionState)
                        {
                            case Interfaces.ConnexionState.None:
                                StatusTextView.SetText(Resource.String.modeFollowMe_explanations);
                                break;
                            case Interfaces.ConnexionState.SeekiosUnreachable:
                                StatusTextView.SetText(Resource.String.modeFollowMe_seekiosUnreachable);
                                break;
                            case Interfaces.ConnexionState.Connecting:
                            case Interfaces.ConnexionState.LookingForSeekios:
                                StatusTextView.SetText(Resource.String.modeFollowMe_lookingForSeekios);
                                break;
                            case Interfaces.ConnexionState.Disconnecting:
                            case Interfaces.ConnexionState.Disconnected:
                                StatusTextView.SetText(Resource.String.modeFollowMe_connexionLost);
                                break;
                            case Interfaces.ConnexionState.Connected:
                                StatusTextView.SetText(Resource.String.modeFollowMe_connected);
                                break;
                            default:
                                break;
                        }
                    });
                    break;
            }
        }

        #endregion

        #region ===== Handlers ====================================================================

        #endregion
    }
}