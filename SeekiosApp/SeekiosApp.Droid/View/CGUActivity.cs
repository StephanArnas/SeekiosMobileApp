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

namespace SeekiosApp.Droid.View
{
    [Activity(Label = "CGUActivity", Theme = "@style/Theme.Normal")]
    public class CGUActivity : Activity
    {
        public TextView CguContents;

        private Dialog _popupCGU = null;

        //POPUP ELEMENTS
        //private ProgressBar _thirdProgressBar;
        //private XamSvg.SvgImageView _thirdValidateSvgImageView;
        private TextView _stepInformationTextView;
        private TextView _titleTextView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.CGULayout);
            base.OnCreate(savedInstanceState);

            GetObjectsFromView();
            SetDataToView();
        }

        protected override void OnResume()
        {
            base.OnResume();
            CguContents.Click += OnRefreshInProgressButtonClick;
        }

        protected override void OnPause()
        {
            base.OnPause();
            CguContents.Click -= OnRefreshInProgressButtonClick;
        }

        private void SetDataToView()
        {
            
        }

        private void GetObjectsFromView()
        {
            CguContents = FindViewById<TextView>(Resource.Id.mapBase_inProgress);
        }

        private void OnRefreshInProgressButtonClick(object sender, EventArgs e)
        {
            if (_popupCGU == null)
            {
                var refreshInProgressBuilder = new Android.Support.V7.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat_Light_Dialog);
                var inflater = (LayoutInflater)GetSystemService(LayoutInflaterService);
                var view = inflater.Inflate(Resource.Drawable.CGUPopup, null);

                //get the different elements
                _stepInformationTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshInProgress_stepText);
                _titleTextView = view.FindViewById<TextView>(Resource.Id.popupRefreshinProgress_title);

                Android.Text.ISpanned cgu = Android.Text.Html.FromHtml("<html><head></head><body style=\"text-align:justify;\"><h2>Title</h2><br><p>Description here</p><div>" +
                "be deemed, to confer rights or remedies upon any third party.<br/>The Terms contain the entire agreement between you and us regarding " +
                "the use of the Site, and supersede any prior agreement between you and us on such subject matter.The parties acknowledge that no reliance is placed on any represent"
                + "ation made but not expressly contained in these Terms." +
 "Any failure on Yelp's part to exercise or enforce any right or provision of the Terms does not constitute a waiver of such right or provision." +
 "The failure of either party to exercise in any respect any right provided for herein shall not be deemed a waiver of any further rights hereunder." +
 "If any provision of the Terms is found to be unenforceable or invalid, then only that provision shall be modified to reflect the parties' "+
 "intention or eliminated to the minimum extent necessary so that the Terms shall otherwise remain in full force and effect and enforceable." +
 " The Terms, and any rights or obligations hereunder, are not assignable, transferable or sublicensable by you except with Yelp's prior written consent, "
  + "but may be assigned or transferred by us without restriction. Any attempted assignment by you shall violate these Terms and be void." +
 "The section titles in the Terms are for convenience only and have no legal or contractual effect." +
"Copyright © 2016 Yelp Inc., 140 New Montgomery, San Francisco, CA 94105, U.S.A."
                    +"</div></body></html>");
                _stepInformationTextView.TextFormatted = cgu;
                _stepInformationTextView.MovementMethod = new Android.Text.Method.ScrollingMovementMethod();

                //_stepInformationTextView.Text = Resources.GetString(Resource.String.map_TextStep3);

                //Cancel button
                refreshInProgressBuilder.SetNegativeButton("Je n'accepte pas", (senderAlert, args) =>
                {
                    if (_popupCGU != null) _popupCGU.Dismiss();
                });
                refreshInProgressBuilder.SetPositiveButton("J'accepte", (senderAlert, args) =>
                {
                    if (_popupCGU != null) _popupCGU.Dismiss();
                });

                refreshInProgressBuilder.SetView(view);
                _popupCGU = refreshInProgressBuilder.Create();
                _popupCGU.DismissEvent += _popupRefresh_DismissEvent;
            }
            _popupCGU.Show();
        }

        private void _popupRefresh_DismissEvent(object sender, EventArgs e)
        {
            if (_popupCGU != null)
            {
                _popupCGU.Dismiss();
                _popupCGU.Dispose();
                _popupCGU = null;
            }
        }
    }
}