
using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Widget;
using SeekiosApp.Droid.Services;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class NeedUpdateActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        public TextView OpenPlayStoreButton { get; set; }
        public XamSvg.SvgImageView OpenPlayStoreButton2 { get; set; }

        #endregion

        #region ===== Properties ==================================================================



        #endregion

        #region ===== Life Cycle ==================================================================

        /// <summary>
        /// View creation
        /// </summary>
        protected override void OnCreate(Bundle bundle)
        {
            SetContentView(Resource.Layout.NeedUpdateActivity);
            base.OnCreate(bundle);

            GetObjectsFromView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.needUpdate_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
        }

        /// <summary>
        /// View resumed
        /// </summary>
        protected override void OnResume()
        {
            OpenPlayStoreButton.Click += OpenPlayStoreButton_Click;
            OpenPlayStoreButton2.Click += OpenPlayStoreButton_Click;
            base.OnResume();
        }

        /// <summary>
        /// View paused
        /// </summary>
        protected override void OnPause()
        {
            OpenPlayStoreButton.Click -= OpenPlayStoreButton_Click;
            OpenPlayStoreButton2.Click -= OpenPlayStoreButton_Click;
            base.OnPause();
        }

        /// <summary>
        /// View destryed
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        #endregion

        #region ===== Initialize View =============================================================

        /// <summary>
        /// Get object from the view
        /// </summary>
        private void GetObjectsFromView()
        {
            OpenPlayStoreButton = FindViewById<TextView>(Resource.Id.launchPlayStore);
            OpenPlayStoreButton2 = FindViewById<XamSvg.SvgImageView>(Resource.Id.launchPlayStore2);
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Open the play store
        /// </summary>
        private void OpenPlayStoreButton_Click(object sender, System.EventArgs e)
        {
            string appPackageName = PackageName;
            try
            {
                StartActivity(new Intent(Intent.ActionView, Uri.Parse("https://play.google.com/store/apps/details?id=seekiosapp.droid")));
            }
            catch (Android.Content.ActivityNotFoundException exception)
            {
                var builder = new AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
                builder.SetTitle(Resource.String.needUpdate_popupTitle);
                builder.SetMessage(Resource.String.needUpdate_popupContent);
                builder.SetCancelable(true);
                var dialog = builder.Create();
                dialog.Show();
            }
        }

        #endregion
    }
}