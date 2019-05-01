using Android.OS;
using Android.Views;
using Android.Widget;
using Android.App;
using SeekiosApp.ViewModel;
using System;
using SeekiosApp.Droid.Services;
using System.Threading.Tasks;
using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using System.Linq;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ModeDontMove2Activity : AppCompatActivityBase
    {
        #region ===== Properties ==================================================================

        public TextView SaveButton { get; set; }
        public Spinner RefreshTrackingSpinner { get; set; }
        public Switch TrackingSwitch { get; set; }
        public TextView RefreshRateTextView { get; set; }
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
            SetContentView(Resource.Layout.ModeDontMoveLayout2);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();
            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.modeDontMove_titlePage), "2/2");
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }

            App.Locator.ModeDontMove.IsTrackingSettingEnable = App.Locator.ModeDontMove.TrackingSetting.IsEnable;
            TrackingSwitch.Checked = App.Locator.ModeDontMove.IsTrackingSettingEnable;
            TrackingSwitchCheckedChanged(TrackingSwitch, null);
            RefreshTrackingSpinner.SetSelection(SeekiosApp.Helper.SpinnerHelper.ReverseValueSpinner(App.Locator.ModeDontMove.TrackingSetting.RefreshTime));
            PowerSavingSwitch.Checked = App.Locator.ModeDontMove.TrackingSetting.IsPowerSavingEnabled;
            PowerSavingSwitchCheckedChanged(PowerSavingSwitch, null);
        }

        protected override void OnResume()
        {
            base.OnResume();
            RefreshTrackingSpinner.ItemSelected += RefreshTrackingSpinner_ItemSelected;
            TrackingSwitch.CheckedChange += TrackingSwitchCheckedChanged;
            PowerSavingSwitch.CheckedChange += PowerSavingSwitchCheckedChanged;
            PowerSavingInfoButton.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click += OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click += OnPowerSavingInfoButtonClicked;
            SaveButton.Click += OnClickNextPage;
        }

        protected override void OnPause()
        {
            base.OnPause();
            RefreshTrackingSpinner.ItemSelected -= RefreshTrackingSpinner_ItemSelected;
            TrackingSwitch.CheckedChange -= TrackingSwitchCheckedChanged;
            PowerSavingSwitch.CheckedChange -= PowerSavingSwitchCheckedChanged;
            PowerSavingInfoButton.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingImaveView.Click -= OnPowerSavingInfoButtonClicked;
            PowerSavingTitleTextView.Click -= OnPowerSavingInfoButtonClicked;
            SaveButton.Click -= OnClickNextPage;
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
            SaveButton = FindViewById<TextView>(Resource.Id.modeDM_saveButton);
            RefreshTrackingSpinner = FindViewById<Spinner>(Resource.Id.modeDMConfiguration_refreshRateSpinner);
            TrackingSwitch = FindViewById<Switch>(Resource.Id.modeDM_tracking_switch);
            RefreshRateTextView = FindViewById<TextView>(Resource.Id.modeDMConfiguration_refreshRate);

            PowerSavingSwitch = FindViewById<Switch>(Resource.Id.modeDMPowerSaving_switch);
            PowerSavingInfoButton = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDMPowerSaving_aboutImage);
            PowerSavingImaveView = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDMPowerSaving_image);
            PowerSavingTitleTextView = FindViewById<TextView>(Resource.Id.modeDMPowerSaving_title);
            PowerSavingExplanationTextView = FindViewById<TextView>(Resource.Id.modeDMPowerSaving_explanation);
            PowerSavingParamTitleTextView = FindViewById<TextView>(Resource.Id.modeDMPowerSaving_paramTitle);
        }

        private void SetDataToView()
        {
            var adapter = ArrayAdapter.CreateFromResource(this
                            , Resource.Array.refreshRate_array
                            , Resource.Layout.AddSeekiosSpinnerText);
            adapter.SetDropDownViewResource(Resource.Layout.SpinnerDropDownItem);
            RefreshTrackingSpinner.Adapter = adapter;
            App.Locator.ModeDontMove.InitModeDontMove();
            MapViewModelBase.RefreshTime = 0;
            RefreshTrackingAfterMoveDescriptionText();

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

        #endregion

        #region ===== Private Methodes ============================================================

        private void RefreshTrackingSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (TrackingSwitch.Checked)
            {
                MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshTrackingSpinner.SelectedItemPosition);
                if (!App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                RefreshTrackingAfterMoveDescriptionText();
            }
        }

        private void TrackingSwitchCheckedChanged(object sender, EventArgs e)
        {
            RefreshTrackingAfterMoveDescriptionText();
            if (TrackingSwitch.Checked)
            {
                App.Locator.ModeDontMove.IsTrackingSettingEnable = true;
                RefreshTrackingSpinner.Enabled = true;
                if (!App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Add(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                MapViewModelBase.RefreshTime = SeekiosApp.Helper.SpinnerHelper.GetValueSpinner(RefreshTrackingSpinner.SelectedItemPosition);
            }
            else
            {
                App.Locator.ModeDontMove.IsTrackingSettingEnable = false;
                if (App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.ModeDontMove.LsSeekiosInTrackingAfterMove.Remove(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
                RefreshTrackingSpinner.Enabled = false;
                MapViewModelBase.RefreshTime = 0;
            }
            RefreshTrackingAfterMoveDescriptionText();
        }

        private void RefreshTrackingAfterMoveDescriptionText()
        {
            RefreshTrackingSpinner.Visibility = TrackingSwitch.Checked ? ViewStates.Visible : ViewStates.Gone;
            RefreshRateTextView.Visibility = TrackingSwitch.Checked ? ViewStates.Visible : ViewStates.Gone;
        }

        #endregion

        #region ===== Event =======================================================================

        private async void OnClickNextPage(object sender, EventArgs e)
        {
            if (await App.Locator.ModeSelection.SelectMode(ModeDefinitionEnum.ModeDontMove))
            {
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                App.Locator.ModeDontMove.IsGoingBack = true;
                App.RaiseSeekiosInformationChangedEverywhere(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                Finish();
            }
        }

        private void PowerSavingSwitchCheckedChanged(object sende, EventArgs e)
        {
            if (PowerSavingSwitch.Checked)
            {
                App.Locator.ModeDontMove.IsPowerSavingEnabled = true;
            }
            else App.Locator.ModeDontMove.IsPowerSavingEnabled = false;
        }

        private void OnPowerSavingInfoButtonClicked(object sender, EventArgs e)
        {
            App.Locator.Parameter.GoToTutorialPowerSaving();
        }

        #endregion
    }
}