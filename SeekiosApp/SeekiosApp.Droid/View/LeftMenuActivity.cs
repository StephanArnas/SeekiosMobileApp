using Android.OS;
using System;
using Android.Widget;
using SeekiosApp.Droid.CustomComponents;
using Android.Graphics;
using Android.Views;
using SeekiosApp.Droid.Services;
using Android.Support.V4.Widget;
//using Xamarin.Facebook.Login;
//using Xamarin.Facebook;
using HockeyApp.Android;

namespace SeekiosApp.Droid.View
{
    public class LeftMenuActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private bool _leftMenuExist = false;
        private InternetConnectionService _connectionService = new InternetConnectionService();

        #endregion

        #region ===== Properties ==================================================================

        public RoundedImageView UserImage { get; set; }
        public LinearLayout AddSeekiosLayout { get; set; }
        public LinearLayout CreditsLayout { get; set; }
        public LinearLayout GuideLayout { get; set; }
        public LinearLayout FeedbackLayout { get; set; }
        public TextView UserFullNameUserTextView { get; set; }
        public TextView UserEmailTextView { get; set; }
        public TextView UserCreditNumberTextView { get; set; }
        public RelativeLayout UserTopLayout { get; set; }
        public DrawerLayout DrawerNavigation { get; private set; }

        #endregion

        #region ===== Life Cycle ==================================================================

        protected void OnCreate(Bundle bundle
            , int idLayout
            , bool leftMenuExist = true
            , bool isBackButton = false)
        {
            base.OnCreate(bundle);

            _leftMenuExist = leftMenuExist;
            SetContentView(idLayout);
            _connectionService.Initialize(this, ConnectivityService);

            if (_leftMenuExist) // create menu
            {
                GetObjectsFromView();
                SetDataToView();
            }

            if (ToolbarPage != null) // create toolbar
            {
                SetSupportActionBar(ToolbarPage);
                if (isBackButton)
                {
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                }
            }

            FeedbackManager.Register(this); //!important cf https://support.hockeyapp.net/discussions/problems/63855-feedback-feature-not-working-hockey-android-sdk
            // Allow users to connect to your app with Facebook
            // can crash if commented
            //FacebookSdk.SdkInitialize(ApplicationContext);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (!_leftMenuExist) return;
            SetDataToView();

            AddSeekiosLayout.Click += OnAddSeekiosClick;
            UserTopLayout.Click += OnParameterClick;
            FeedbackLayout.Click += OnFeedbackButtonLayout_Clicked;
            CreditsLayout.Click += OnCreditsClick;
            GuideLayout.Click += OnGuideClick;
            App.RemainingRequestChanged += App_RemainingRequestChanged; ;
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (!_leftMenuExist) return;

            AddSeekiosLayout.Click -= OnAddSeekiosClick;
            UserTopLayout.Click -= OnParameterClick;
            FeedbackLayout.Click -= OnFeedbackButtonLayout_Clicked;
            CreditsLayout.Click -= OnCreditsClick;
            GuideLayout.Click -= OnGuideClick;
            App.RemainingRequestChanged -= App_RemainingRequestChanged;
            if (DrawerNavigation != null) DrawerNavigation.CloseDrawers();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            FeedbackManager.Unregister();

            if (!_leftMenuExist) return;
        }

        #endregion

        #region ===== ActionBar ===================================================================

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        #endregion

        #region ===== Initialize View =============================================================

        /// <summary>
        /// Get object from view
        /// </summary>
        private void GetObjectsFromView()
        {
            UserImage = FindViewById<RoundedImageView>(Resource.Id.leftMenu_userImg);
            UserCreditNumberTextView = FindViewById<TextView>(Resource.Id.leftMenu_creditUser);
            UserFullNameUserTextView = FindViewById<TextView>(Resource.Id.leftMenu_fullname);
            UserEmailTextView = FindViewById<TextView>(Resource.Id.leftMenu_email);

            AddSeekiosLayout = FindViewById<LinearLayout>(Resource.Id.leftMenu_gridAddSeekios);
            CreditsLayout = FindViewById<LinearLayout>(Resource.Id.leftMenu_gridCredits);
            GuideLayout = FindViewById<LinearLayout>(Resource.Id.leftMenu_gridGuide);
            FeedbackLayout = FindViewById<LinearLayout>(Resource.Id.leftMenu_gridFeedback);
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            DrawerNavigation = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            UserTopLayout = FindViewById<RelativeLayout>(Resource.Id.leftMenu_top);
        }

        /// <summary>
        /// Set the data from the View Model
        /// </summary>
        private void SetDataToView()
        {
            UserFullNameUserTextView.Text = App.Locator.LeftMenu.UserFullName;
            UserCreditNumberTextView.Text = SeekiosApp.Helper.CreditHelper.TotalCredits;
            UserEmailTextView.Text = App.CurrentUserEnvironment.User.Email;
            if (!string.IsNullOrEmpty(App.CurrentUserEnvironment.User.UserPicture))
            {
                var bytes = Convert.FromBase64String(App.CurrentUserEnvironment.User.UserPicture);
                UserImage.SetImageBitmap(BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length));
            }
            else UserImage.SetImageResource(Resource.Drawable.DefaultUser);
        }

        #endregion

        #region ===== Event =======================================================================

        /// <summary>
        /// Navigate to the page user parameter
        /// </summary>
        private void OnParameterClick(object sender, EventArgs e)
        {
            App.Locator.LeftMenu.GoToParameter();
        }

        /// <summary>
        /// Navigate to the page add a seekios
        /// </summary>
        private void OnAddSeekiosClick(object sender, EventArgs e)
        {
            App.Locator.LeftMenu.GoToAddSeekios();
        }

        /// <summary>
        /// Navigate to the page tutorial
        /// </summary>
        private void OnGuideClick(object sender, EventArgs e)
        {
            App.Locator.LeftMenu.GoToListTutorial();
        }

        /// <summary>
        /// Navigate to the page map of all seekios
        /// </summary>
        private void OnMapSeekiosClick(object sender, EventArgs e)
        {
            if (App.CurrentUserEnvironment.LsSeekios.Count > 0)
            {
                App.Locator.LeftMenu.GoToSeekiosMapAllSeekios();
            }
        }

        /// <summary>
        /// Navigate to the page that contains credit information
        /// </summary>
        private void OnCreditsClick(object sender, EventArgs e)
        {
            App.Locator.Credits.GoToCreditHistoric();
        }

        /// <summary>
        /// Navigate to the feedback user
        /// </summary>
        private void OnFeedbackButtonLayout_Clicked(object sender, EventArgs e)
        {
            if (_connectionService.IsDeviceConnectedToInternet()
                && App.CurrentUserEnvironment.User.Email != null)
            {
                FeedbackManager.SetUserEmail(App.CurrentUserEnvironment.User.Email);
                FeedbackManager.SetUserName(string.Format("{0} {1}"
                    , App.CurrentUserEnvironment.User.FirstName
                    , App.CurrentUserEnvironment.User.LastName));
                FeedbackManager.ShowFeedbackActivity(this);
                FeedbackManager.CheckForAnswersAndNotify(this);
            }
            else
            {
                Toast.MakeText(this, Resource.String.errorEmail, ToastLength.Long);
            }
        }

        /// <summary>
        /// Update the number of credit
        /// </summary>
        private void App_RemainingRequestChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() => { App.CurrentUserEnvironment.User.RemainingRequest.ToString(); });
        }

        #endregion
    }
}