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
    public class TutorialPowerSavingActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        #endregion

        #region ===== Properties ==================================================================

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.TutorialPowerSavingLayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.powerSaving_explanationTitle1);
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
        }

        private void SetDataToView()
        {

        }

        #endregion
    }
}