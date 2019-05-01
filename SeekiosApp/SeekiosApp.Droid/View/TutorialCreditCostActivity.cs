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

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class TutorialCreditCostActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        #endregion

        #region ===== Properties ==================================================================

        public TextView MoreButton { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.TutorialCreditCostLayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.tutorialCost_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        protected override void OnResume()
        {
            MoreButton.Click += MoreButton_Click;
            base.OnResume();
        }

        protected override void OnPause()
        {
            MoreButton.Click -= MoreButton_Click;
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
            MoreButton = FindViewById<TextView>(Resource.Id.tutorialCost_moreButton);
        }

        private void SetDataToView()
        {

        }

        #endregion

        #region ===== Event =======================================================================

        private void MoreButton_Click(object sender, EventArgs e)
        {
            using (var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(App.TutorialCreditCostLink)))
            {
                StartActivity(intent);
            }
        }

        #endregion
    }
}