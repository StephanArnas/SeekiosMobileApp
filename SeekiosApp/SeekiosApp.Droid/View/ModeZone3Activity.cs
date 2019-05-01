using System;

using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;
using SeekiosApp.ViewModel;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "ModeZone2", Theme = "@style/Theme.Normal")]
    public class ModeZone3Activity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================


        #endregion

        #region ===== Properties ==================================================================

        public TextView NextButton { get; set; }
        public TextView RefreshRateText { get; set; }
        public Spinner RefreshTrackingSpinner { get; set; }
        public Switch ActiveTracker { get; set; }

        public XamSvg.SvgImageView PowerSavingImaveView { get; set; }
        public TextView PowerSavingTitleTextView { get; set; }
        public TextView PowerSavingExplanationTextView { get; set; }
        public TextView PowerSavingParamTitleTextView { get; set; }
        public Switch PowerSavingSwitch { get; set; }
        public XamSvg.SvgImageView PowerSavingInfoButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.ModeZone3Layout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.modeZone_titlePage), "3/3");
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }

            RefreshTrackingSpinner.ItemSelected += RefreshTrackingSpinner_ItemSelected;
            RefreshTrackingSpinner.SetSelection(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeZone.TrackingSetting.RefreshTime));
            ActiveTracker.CheckedChange += TrackingSwitchCheckedChange;
            App.Locator.ModeZone.IsTrackingSettingEnable = App.Locator.ModeZone.TrackingSetting.IsEnable;
            ActiveTracker.Checked = App.Locator.ModeZone.IsTrackingSettingEnable;
            PowerSavingSwitch.Checked = App.Locator.ModeZone.TrackingSetting.IsPowerSavingEnabled;
            TrackingSwitchCheckedChange(ActiveTracker, null);
        }

        protected override void OnResume()
        {
            base.OnResume();
            NextButton.Click += OnClickNextPage;
            PowerSavingSwitch.CheckedChange += PowerSavingSwitchCheckedChanged;
            PowerSavingSwitch.Checked = App.Locator.ModeZone.IsPowerSavingEnabled;
            PowerSavingSwitchCheckedChanged(PowerSavingSwitch, null);
            PowerSavingInfoButton.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click += OnPowerSavingInfoButtonClicked;
        }

        protected override void OnPause()
        {
            base.OnPause();
            NextButton.Click -= OnClickNextPage;
            RefreshTrackingSpinner.ItemSelected -= RefreshTrackingSpinner_ItemSelected;
            PowerSavingSwitch.CheckedChange -= PowerSavingSwitchCheckedChanged;
            ActiveTracker.CheckedChange -= TrackingSwitchCheckedChange;
            PowerSavingInfoButton.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click -= OnPowerSavingInfoButtonClicked;
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
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            NextButton = FindViewById<TextView>(Resource.Id.modeZone2_saveButton);

            RefreshRateText = FindViewById<TextView>(Resource.Id.modeZoneConfiguration_refreshRate);
            RefreshTrackingSpinner = FindViewById<Spinner>(Resource.Id.modeZoneConfiguration_refreshRateSpinner);
            ActiveTracker = FindViewById<Switch>(Resource.Id.modeZone2_tracking_switch);

            /* Power saving form */
            PowerSavingImaveView = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZonePowerSaving_image);
            PowerSavingTitleTextView = FindViewById<TextView>(Resource.Id.modeZonePowerSaving_title);
            PowerSavingExplanationTextView = FindViewById<TextView>(Resource.Id.modeZonePowerSaving_explanation);
            PowerSavingParamTitleTextView = FindViewById<TextView>(Resource.Id.modeZonePowerSaving_paramTitle);
            PowerSavingSwitch = FindViewById<Switch>(Resource.Id.modeZonePowerSaving_switch);
            PowerSavingInfoButton = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZonePowerSaving_aboutImage);
        }

        private void SetDataToView()
        {
            var adapter = ArrayAdapter.CreateFromResource(this
                            , Resource.Array.refreshRate_array
                            , Resource.Layout.AddSeekiosSpinnerText);
            adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
            RefreshTrackingSpinner.Adapter = adapter;
            MapViewModelBase.RefreshTime = 0;
            RefreshRateText.Visibility = ViewStates.Gone;
            RefreshTrackingSpinner.Visibility = ViewStates.Gone;
            
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

        #endregion

        #region ===== Event =======================================================================

        private async void OnClickNextPage(object sender, EventArgs e)
        {
            App.Locator.ModeZone.IsActivityFocused = false;
            if (await App.Locator.ModeZone.UpdateZone())
            {
                if (App.Locator.ModeZone.IsTrackingAfterOOZ)
                {
                    App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                App.Locator.ModeZone.IsGoingBack = true;
                App.Locator.ModeZone.IsOnEditMode = false;
                Finish();
            }
        }

        private void RefreshTrackingSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (ActiveTracker.Checked)
            {
                MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshTrackingSpinner.SelectedItemPosition);
                App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
            }
        }

        private void TrackingSwitchCheckedChange(object sender, EventArgs e)
        {
            var refreshRateVisibility = ActiveTracker.Checked ? ViewStates.Visible : ViewStates.Gone;
            RefreshRateText.Visibility = refreshRateVisibility;
            RefreshTrackingSpinner.Visibility = refreshRateVisibility;
            if (ActiveTracker.Checked)
            {
                App.Locator.ModeZone.IsTrackingSettingEnable = true;
                RefreshTrackingSpinner.Enabled = true;
                App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshTrackingSpinner.SelectedItemPosition);
            }
            else
            {
                App.Locator.ModeZone.IsTrackingSettingEnable = false;
                RefreshTrackingSpinner.Enabled = false;
                if (App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeZone.LsSeekiosInTrackingAfterOOZ.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                MapViewModelBase.RefreshTime = 0;
            }
        }

        private void PowerSavingSwitchCheckedChanged(object sende, EventArgs e)
        {
            if (PowerSavingSwitch.Checked)
            {
                App.Locator.ModeZone.IsPowerSavingEnabled = true;
            }
            else App.Locator.ModeZone.IsPowerSavingEnabled = false;
        }

        private void OnPowerSavingInfoButtonClicked(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        #endregion
    }
}