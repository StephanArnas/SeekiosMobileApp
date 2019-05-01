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
using Android.Content.PM;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class AboutActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private InternetConnectionService _connectionService = new InternetConnectionService();

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Redirect to facebook page</summary>  
        public XamSvg.SvgImageView FacebookImageview { get; private set; }

        /// <summary>Redirect to Twitter page</summary>  
        public XamSvg.SvgImageView TwitterImageview { get; private set; }

        /// <summary>Redirect to in page</summary>  
        public XamSvg.SvgImageView InImageview { get; private set; }

        /// <summary>Redirect to instagram page</summary>  
        public XamSvg.SvgImageView InstagramImageview { get; private set; }

        /// <summary>Redirect to Seekios.com webpage</summary> 
        public TextView SeekiosLinkTextview { get; set; }

        /// <summary>Redirect to Politique de confidentialite webpage</summary> 
        public TextView SeekiosPrivacyTextview { get; set; }

        /// <summary>Version number of the application</summary> 
        public TextView VersionNumberTextView { get; set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.AboutPageLayout);
            Title = Resources.GetString(Resource.String.parameter_about);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            _connectionService.Initialize(this, ConnectivityService);
        }

        protected override void OnResume()
        {
            base.OnResume();
            FacebookImageview.Click += OpenFacebookWebPage;
            TwitterImageview.Click += OpenTwitterWebPage;
            InImageview.Click += OpenLinkedInWebPage;
            InstagramImageview.Click += OpenInstagramWebPage;
            SeekiosLinkTextview.Click += RedirectToSeekiosWebPage;
            SeekiosPrivacyTextview.Click += RedirectToSeekoisPrivacyWebPage;
        }

        protected override void OnPause()
        {
            base.OnPause();
            FacebookImageview.Click -= OpenFacebookWebPage;
            TwitterImageview.Click -= OpenTwitterWebPage;
            InImageview.Click -= OpenLinkedInWebPage;
            InstagramImageview.Click -= OpenInstagramWebPage;
            SeekiosLinkTextview.Click -= RedirectToSeekiosWebPage;
            SeekiosPrivacyTextview.Click -= RedirectToSeekoisPrivacyWebPage;
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Finish();
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region ===== Initialize View =============================================================

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            FacebookImageview = FindViewById<XamSvg.SvgImageView>(Resource.Id.about_fbicon);
            TwitterImageview = FindViewById<XamSvg.SvgImageView>(Resource.Id.about_twittericon);
            InImageview = FindViewById<XamSvg.SvgImageView>(Resource.Id.about_inicon);
            InstagramImageview = FindViewById<XamSvg.SvgImageView>(Resource.Id.about_instaicon);

            VersionNumberTextView = FindViewById<TextView>(Resource.Id.versionNumber);

            SeekiosLinkTextview = FindViewById<TextView>(Resource.Id.about_seekios_siteurl);
            SeekiosPrivacyTextview = FindViewById<TextView>(Resource.Id.about_privacy_url);
        }

        private void SetDataToView()
        {
            VersionNumberTextView.Text = PackageManager.GetPackageInfo(PackageName, PackageInfoFlags.MetaData).VersionName;
        }

        #endregion

        #region ======  Social Network Link =======================================================

        private void OpenFacebookWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.FacebookLink);
        }

        private void OpenTwitterWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.TwitterLink);
        }

        private void OpenLinkedInWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.LinkedinLink);
        }

        private void OpenInstagramWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.InstagramLink);
        }

        private void RedirectToSeekiosWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.SeekiosLink);
        }

        private void RedirectToSeekoisPrivacyWebPage(object sender, EventArgs e)
        {
            OpenInWebbroser(App.PolicyLink);
        }

        private void OpenInWebbroser(string url)
        {
            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(url));
            StartActivity(intent);
        }

        #endregion

    }
}