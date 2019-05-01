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

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ListTutorialActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private ListTutorialAdapter _tutorialAdapter = null; 

        #endregion

        #region ===== Properties ==================================================================

        public ListView TutorialListView { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.ListTutorialLayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.listTutorial_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
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

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            TutorialListView = FindViewById<ListView>(Resource.Id.listTutorial_listView);
        }

        private void SetDataToView()
        {
            _tutorialAdapter = new ListTutorialAdapter(this);
            TutorialListView.Adapter = _tutorialAdapter;
            TutorialListView.ChoiceMode = ChoiceMode.Single;
            TutorialListView.ItemsCanFocus = true;
            TutorialListView.ItemClick += TutorialListView_ItemClick;
        }

        private void TutorialListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position == 0)
            {
                App.Locator.Parameter.GoToTutorial();
            }
            else if (e.Position == 1)
            {
                App.Locator.Parameter.GoToTutorialPowerSaving();
            }
            else if (e.Position == 2)
            {
                App.Locator.Parameter.GoToTutorialCreditCost();
            }
            else if (e.Position == 3)
            {
                using (var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.TutorialHelpLink)))
                {
                    StartActivity(intent);
                }
            }
        }

        #endregion
    }
}