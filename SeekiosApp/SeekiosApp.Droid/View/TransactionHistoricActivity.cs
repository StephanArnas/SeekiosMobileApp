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
using SeekiosApp.Droid.Services;
using SeekiosApp.Droid.CustomComponents.Adapter;
using SeekiosApp.Model.DTO;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class TransactionHistoricActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private TransactionHistoricAdapter _historicAdapter = null;

        #endregion

        #region ===== Properties ==================================================================

        public ListView TransactionHistoricListView { get; set; }
        public LinearLayout EmptyHistoricListLayout { get; set; }
        public Button RefreshButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.TransactionHistoricLayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();
            GetTransactionHistoric();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.transactionHistoricTitle);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            if (_historicAdapter != null) _historicAdapter.NotifyDataSetChanged();
            RefreshButton.Click += OnRefreshButtonClick;
            base.OnResume();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return true;
        }

        #endregion

        #region ===== Initialisation Vue ==========================================================

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);
            EmptyHistoricListLayout = FindViewById<LinearLayout>(Resource.Id.transactionHistoric_emptyLayout);
            TransactionHistoricListView = FindViewById<ListView>(Resource.Id.transactionHistoric_transactionList);
            RefreshButton = FindViewById<Button>(Resource.Id.transactionEmptyRefreshButton);
        }

        private void SetDataToView()
        {
            _historicAdapter = new TransactionHistoricAdapter(this);
            TransactionHistoricListView.EmptyView = EmptyHistoricListLayout;
            TransactionHistoricListView.Adapter = _historicAdapter;
            TransactionHistoricListView.ChoiceMode = ChoiceMode.Single;
            TransactionHistoricListView.ItemsCanFocus = true;
        }

        private void GetTransactionHistoric()
        {
            RunOnUiThread(async () =>
            {
                LoadingLayout.Visibility = ViewStates.Visible;
                await App.Locator.TransactionHistoric.GetTransactionHistoricByUser();
                _historicAdapter.NotifyDataSetChanged();
                LoadingLayout.Visibility = ViewStates.Gone;
            });
        }

        private void OnRefreshButtonClick(object sender, EventArgs e)
        {
            GetTransactionHistoric();
        }

        #endregion
    }
}