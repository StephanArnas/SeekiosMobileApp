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
using SeekiosApp.Droid.CustomComponents;
using SeekiosApp.Droid.Services;
using SeekiosApp.ViewModel;

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "ModeDontMove2Activity", Theme = "@style/Theme.Normal")]
    public class ModeDontMoveActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private ListAlertModeZoneAdapter _alertsAdapter = null;
        private ListView AlertListView = null;

        #endregion

        #region ===== Propriétés ==================================================================

        public TextView NextButton { get; set; }
        public LinearLayout EmptyListView { get; set; }
        public TextView AddAlertTextView { get; set; }
        public XamSvg.SvgImageView EmptyAlertSvgImage { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.ModeDontMoveLayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.Title = string.Format(Resources.GetString(Resource.String.modeDontMove_titlePage), "1/2");
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (App.Locator.ModeDontMove.IsGoingBack)
            {
                App.Locator.ModeDontMove.IsGoingBack = false;
                Finish();
            }

            _alertsAdapter.NotifyDataSetChanged();

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
            NextButton = FindViewById<TextView>(Resource.Id.modeDM_next);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            AlertListView = FindViewById<ListView>(Resource.Id.modeDM_alertList);
            EmptyListView = FindViewById<LinearLayout>(Resource.Id.modeDM_emptyList);
            AddAlertTextView = FindViewById<TextView>(Resource.Id.modeDM_createButton);
            EmptyAlertSvgImage = FindViewById<XamSvg.SvgImageView>(Resource.Id.modeDM_alertImgActionClick);
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

        #region ===== Private Methodes ============================================================

        private void UpdateUI()
        {
            NextButton.Text = AlertListView.Count <= 0 ? Resources.GetString(Resource.String.modeZone_pass) : Resources.GetString(Resource.String.modeZone_continue);
        }

        #endregion

        #region ===== Event =======================================================================

        private void OnClickNextPage(object sender, EventArgs e)
        {
            App.Locator.ModeDontMove.GoToSecondPage();
        }

        private void OnAddAlert(object sender, EventArgs e)
        {
            App.Locator.ModeZone.WaitingForAlerts = true;   // le mode zone attend de recevoir une alerte, utilise pour savoir ou storer l'alerte
            App.Locator.ModeZone.EditingAlerts = false;     // le mode zone indique que l'on va editer une alerte, utilise lors de l'enregistrement de l;alerte
            App.Locator.ModeZone.GoToAlertDetail(null, -1);
        }

        private void UpdateUIOnConnectionStateChanged(bool isConnected)
        {
            NextButton.Enabled = isConnected;
        }

        #endregion
    }
}