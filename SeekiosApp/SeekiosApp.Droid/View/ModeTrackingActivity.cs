using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;
using SeekiosApp.Enum;
using SeekiosApp.ViewModel;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ModeTrackingActivity : AppCompatActivityBase
    {
        #region ===== Properties ==================================================================

        public Spinner RefreshRateSpinner { get; set; }

        public TextView SaveModeTrackingButton { get; set; }

        public XamSvg.SvgImageView PowerSavingImaveView { get; set; }

        public TextView PowerSavingTitleTextView { get; set; }

        public TextView PowerSavingExplanationTextView { get; set; }

        public TextView PowerSavingParamTitleTextView { get; set; }

        public Switch PowerSavingSwitch { get; set; }

        public XamSvg.SvgImageView PowerSavingInfoButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.ModeTrackingLayout);
            base.OnCreate(bundle);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.trackingConfiguration_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }

            PowerSavingSwitch.Checked = App.Locator.ModeTracking.TrackingSetting.IsPowerSavingEnabled;
            PowerSavingSwitchCheckedChanged(PowerSavingSwitch, null);
        }

        protected override void OnResume()
        {
            base.OnResume();
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;
            SaveModeTrackingButton.Click += OnSaveModeTrackingButtonClick;
            PowerSavingInfoButton.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingSwitch.CheckedChange += PowerSavingSwitchCheckedChanged;
        }

        protected override void OnPause()
        {
            base.OnPause();
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
            SaveModeTrackingButton.Click -= OnSaveModeTrackingButtonClick;
            PowerSavingInfoButton.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingSwitch.CheckedChange -= PowerSavingSwitchCheckedChanged;
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            RefreshRateSpinner = FindViewById<Spinner>(Resource.Id.trackingConfiguration_refreshRateSpinner);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            SaveModeTrackingButton = FindViewById<TextView>(Resource.Id.modeTracking_saveButton);

            // Power saving form
            PowerSavingSwitch = FindViewById<Switch>(Resource.Id.trackingPowerSaving_switch);
            PowerSavingInfoButton = FindViewById<XamSvg.SvgImageView>(Resource.Id.trackingPowerSaving_aboutImage);
            PowerSavingImaveView = FindViewById<XamSvg.SvgImageView>(Resource.Id.trackingPowerSaving_image);
            PowerSavingTitleTextView = FindViewById<TextView>(Resource.Id.trackingPowerSaving_title);
            PowerSavingExplanationTextView = FindViewById<TextView>(Resource.Id.trackingPowerSaving_explanation);
            PowerSavingParamTitleTextView = FindViewById<TextView>(Resource.Id.trackingPowerSaving_paramTitle);
            if (App.Locator.ModeTracking.TrackingSetting.RefreshTime < 30)
            {
                PowerSavingSwitch.Enabled = false;
                PowerSavingTitleTextView.Text = GetString(Resource.String.powerSaving_explanationTitle1) + " (>= 30min)";
            }
        }

        private void SetDataToView()
        {
            // set up the refreshRate spinner values
            var adapter = ArrayAdapter.CreateFromResource(this
                , Resource.Array.refreshRate_array
                , Resource.Layout.AddSeekiosSpinnerText);
            adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
            RefreshRateSpinner.Adapter = adapter;
            // init credits
            var seekios = App.Locator.DetailSeekios.SeekiosSelected;
            // set default value for tracking
            RefreshRateSpinner.SetSelection(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeTracking.TrackingSetting.RefreshTime));
            RefreshRateSpinner.ItemSelected += RefreshRateSpinner_ItemSelected;

            /* Hide power saving manipulation if firmware version too old */
            if (App.Locator.DetailSeekios.SeekiosSelected.VersionEmbedded_idversionEmbedded < (int)VersionEmbeddedEnum.V1007)
            {
                PowerSavingSwitch.Visibility = ViewStates.Gone;
                PowerSavingInfoButton.Visibility = ViewStates.Gone;
                PowerSavingImaveView.Visibility = ViewStates.Gone;
                PowerSavingTitleTextView.Visibility = ViewStates.Gone;
                PowerSavingExplanationTextView.Visibility = ViewStates.Gone;
                PowerSavingParamTitleTextView.Visibility = ViewStates.Gone;
            }
        }

        private void RefreshRateSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshRateSpinner.SelectedItemPosition) < 30)
            {
                PowerSavingSwitch.Checked = false;
                PowerSavingSwitch.Enabled = false;
                PowerSavingTitleTextView.Text = GetString(Resource.String.powerSaving_explanationTitle1) + " (>= 30min)";
            }
            else
            {
                PowerSavingSwitch.Enabled = true;
                PowerSavingTitleTextView.Text = GetString(Resource.String.powerSaving_explanationTitle1);
            }
        }

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// UpdateUI when connection state changed
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            SaveModeTrackingButton.Enabled = isConnected;
        }

        /// <summary>
        /// Save mode tracking
        /// </summary>
        private async void OnSaveModeTrackingButtonClick(object sender, EventArgs e)
        {
            MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshRateSpinner.SelectedItemPosition);
            if (await App.Locator.ModeSelection.SelectMode(ModeDefinitionEnum.ModeTracking))
            {
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                LoadingLayout.Visibility = ViewStates.Gone;
                Finish();
            }
        }

        private void PowerSavingSwitchCheckedChanged(object sender, EventArgs e)
        {
            if (PowerSavingSwitch.Checked)
            {
                App.Locator.ModeTracking.IsPowerSavingEnabled = true;
            }
            else App.Locator.ModeTracking.IsPowerSavingEnabled = false;
        }

        private void OnPowerSavingInfoButtonClicked(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        #endregion
    }
}