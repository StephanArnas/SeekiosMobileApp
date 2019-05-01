using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;
using SeekiosApp.Droid.CustomComponents;
using System.Threading.Tasks;

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "ModeZone3Activity", Theme = "@style/Theme.Normal")]
    public class ModeZone2Activity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private ListAlertModeZoneAdapter _alertsAdapter = null;

        #endregion

        #region ===== Properties ==================================================================

        public TextView NextButton { get; set; }
        public LinearLayout EmptyListView { get; set; }
        public TextView AddAlertTextView { get; set; }
        public XamSvg.SvgImageView EmptyAlertSvgImage { get; set; }
        public ListView AlertListView { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.ModeZone2Layout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.modeZone_titlePage), "2/3");
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            if (App.Locator.ModeZone.IsGoingBack) Finish();
            //subscribe to connection state changed event
            OnConnectionStateChanged += UpdateUIOnConnectionStateChanged;

            base.OnResume();

            //_alertsAdapter.NotifyDataSetChanged();
            _alertsAdapter = new ListAlertModeZoneAdapter(this);
            AlertListView.Adapter = _alertsAdapter;
            AlertListView.ChoiceMode = ChoiceMode.Single;
            AlertListView.ItemsCanFocus = true;
            AlertListView.EmptyView = EmptyListView;

            NextButton.Click += OnClickNextPage;
            AddAlertTextView.Click += OnAddAlert;
            EmptyAlertSvgImage.Click += OnAddAlert;
            App.Locator.ModeZone.WaitingForAlerts = false;
            App.Locator.ModeZone.EditingAlerts = false;

            UpdateUI();
        }

        protected override void OnPause()
        {
            base.OnPause();

            NextButton.Click -= OnClickNextPage;
            AddAlertTextView.Click -= OnAddAlert;
            EmptyAlertSvgImage.Click -= OnAddAlert;
            //subscribe to connection state changed event
            OnConnectionStateChanged -= UpdateUIOnConnectionStateChanged;
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

        /// <summary>
        /// Get objects from view
        /// </summary>
        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            NextButton = FindViewById<TextView>(Resource.Id.modeZone3_suivant);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            AlertListView = FindViewById<ListView>(Resource.Id.modeZone3_alertsSeekiosList);
            EmptyListView = FindViewById<LinearLayout>(Resource.Id.modeZone3_emptyField);
            AddAlertTextView = FindViewById<TextView>(Resource.Id.modeZone_createButton);
            EmptyAlertSvgImage = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeZone3_alertImgActionClick);
        }

        /// <summary>
        /// Initialize the view with the data
        /// </summary>
        private void SetDataToView()
        {
            _alertsAdapter = new ListAlertModeZoneAdapter(this);
            AlertListView.Adapter = _alertsAdapter;
            AlertListView.ChoiceMode = ChoiceMode.Single;
            AlertListView.ItemsCanFocus = true;
            AlertListView.EmptyView = EmptyListView;

            App.Locator.ModeZone.WaitingForAlerts = false;
        }

        #endregion

        #region ===== Private Methods =============================================================

        private void UpdateUI()
        {
            NextButton.Text = AlertListView.Count <= 0 ? Resources.GetString(Resource.String.modeZone_pass) : Resources.GetString(Resource.String.modeZone_continue);
        }

        #endregion

        #region ===== Event =======================================================================

        private void OnClickNextPage(object sender, EventArgs e)
        {
            App.Locator.ModeZone.GoToThirdPage();
        }

        private void OnClickPrevPage(object sender, EventArgs e)
        {
            App.Locator.ModeZone.WaitingForAlerts = false;
            App.Locator.ModeZone.GoBack();
        }

        private void OnAddAlert(object sender, EventArgs e)
        {
            App.Locator.ModeZone.WaitingForAlerts = true;//le mode zone attend de recevoir une alerte, utilise pour savoir ou storer l'alerte
            App.Locator.ModeZone.EditingAlerts = false;//le mode zone indique que l'on va editer une alerte, utilise lors de l'enregistrement de l;alerte
            App.Locator.ModeZone.GoToAlertDetail(null, -1);
        }

        /// <summary>
        /// UpdateUI when connection state changed
        /// </summary>
        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            NextButton.Enabled = isConnected;
        }

        #endregion
    }
}