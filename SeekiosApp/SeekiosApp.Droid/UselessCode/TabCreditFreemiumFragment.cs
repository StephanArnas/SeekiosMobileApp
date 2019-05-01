//using System;
//using Android.OS;
//using Android.Views;
//using Android.Gms.Ads;
//using Android.Widget;
//using Android.Gms.Ads.Reward;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabCreditFreemiumFragment : Android.Support.V4.App.Fragment, IRewardedVideoAdListener
//    {
//        #region ===== Attributs ===================================================================

//        /// <summary>Parent Activity</summary>
//        private ReloadCreditActivity _context;

//        // publicité
//        private bool _isRewardedVideoLoading;
//        private static string _adMobUserId = "pub-7409748762242725";
//        private static string _adMobUnitId = "ca-app-pub-7409748762242725/6642938099";
//        private static object _lock = new object();

//        #endregion

//        #region ===== Propriétés ==================================================================

//        /// <summary>Bouton pour lancer la video</summary>
//        public RelativeLayout DisplayVideoButton { get; set; }

//        /// <summary>Bouton pour lancer la video</summary>
//        public TextView DisplayVideoTextView { get; set; }

//        /// <summary>Nombre de credit</summary>
//        public TextView CreditTextView { get; set; }

//        public IRewardedVideoAd RewardedVideoAd { get; set; }

//        #endregion

//        #region ===== Cycle De Vie ================================================================

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        public TabCreditFreemiumFragment(ReloadCreditActivity context)
//        {
//            _context = context;
//        }

//        /// <summary>
//        /// Création de la page
//        /// </summary>
//        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            var view = inflater.Inflate(Resource.Layout.TabCreditFreemium, container, false);
//            GetObjectsFromView(view);
//            SetDataToView(view);

//            RewardedVideoAd = MobileAds.GetRewardedVideoAdInstance(_context);
//            RewardedVideoAd.RewardedVideoAdListener = this;
//            LoadRewardedVideoAd();

//            return view;
//        }

//        /// <summary>
//        /// Reprise de la page
//        /// </summary>
//        public override void OnResume()
//        {
//            base.OnResume();
//            DisplayVideoButton.Click += DisplayVideoButton_Click;
//        }

//        /// <summary>
//        /// Suspension de la page
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            DisplayVideoButton.Click -= DisplayVideoButton_Click;
//        }

//        /// <summary>
//        /// Suspension de la page
//        /// </summary>
//        public override void OnStop()
//        {
//            base.OnStop();
//        }

//        /// <summary>
//        /// Destruction de la page
//        /// </summary>
//        public override void OnDestroy()
//        {
//            base.OnDestroy();
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            DisplayVideoButton = view.FindViewById<RelativeLayout>(Resource.Id.tabCreditFreemieum_DisplayVideo);
//            DisplayVideoTextView = view.FindViewById<TextView>(Resource.Id.tabCreditFreemieum_TextDisplayVideo);
//            CreditTextView = view.FindViewById<TextView>(Resource.Id.tabCreditFreemieum_remainingRequest);
//        }

//        /// <summary>
//        /// Fais les liaisons entre les données du vue modèle et les objets de la vue
//        /// </summary>
//        private void SetDataToView(Android.Views.View view)
//        {
//            CreditTextView.Text = App.CurrentUserEnvironment.User.RemainingRequest.ToString();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void DisplayVideoButton_Click(object sender, EventArgs e)
//        {
//            LoadRewardedVideoAd();
//            ShowRewardedVideo();
//            return;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="mp"></param>
//        //public void OnCompletion(MediaPlayer mp)
//        //{
//        //    VideoPlayer.Visibility = ViewStates.Gone;
//        //    DisplayVideoButton.Visibility = ViewStates.Visible;
//        //    App.CurrentUserEnvironment.User.RemainingRequest++;
//        //    App.CurrentUserEnvironment.User.NumberView++;
//        //    //App.RaiseRemainingRequestChangedEverywhere();
//        //    CreditTextView.Text = App.CurrentUserEnvironment.User.RemainingRequest.ToString();

//        //    App.Locator.ReloadCredit.UpdateUser();
//        //}

//        #endregion

//        #region ===== Ads =========================================================================

//        #region AdMob

//        public void LoadRewardedVideoAd()
//        {
//            lock (_lock)
//            {
//                if (!_isRewardedVideoLoading && !RewardedVideoAd.IsLoaded)
//                {
//                    _isRewardedVideoLoading = true;
//                    //Bundle extras = new Bundle();
//                    //extras.PutBoolean("_noRefresh", true);
//                    //Bundle bundle = new Bundle();
//                    //bundle.PutString("color_bg", "0000FF");

//                    //AdColonyBundleBuilder.SetZoneId(_adColonyZoneId);

//                    //var location = new Android.Locations.Location(LocationService);
//                    //var adMobAdapter = new AdMobAdapter();

//                    /*AdRequest adRequest = new AdRequest.Builder()
//                        //.AddTestDevice(AdRequest.DeviceIdEmulator)
//                        //.AddTestDevice("FE5692B3DAD1B4CE3BE3BDA2FF4B6103")
//                        //.AddNetworkExtrasBundle(Java.Lang.Class.FromType(typeof(AdMobAdapter)), extras)
//                        //.AddNetworkExtrasBundle(Java.Lang.Class.FromType(typeof(VungleAdapter)), extras)
//                        .AddNetworkExtrasBundle(Java.Lang.Class.FromType(typeof(AdColonyAdapter)), AdColonyBundleBuilder.Build())
//                        //.SetLocation(location)
//                        .Build();
//                    RewardedVideoAd.UserId = _adMobUserId;
//                    RewardedVideoAd.LoadAd(_adMobUnitId, adRequest);*/
//                }
//            }
//        }

//        public void ShowRewardedVideo()
//        {
//            if (RewardedVideoAd.IsLoaded)
//            {
//                RewardedVideoAd.Show();
//            }
//        }

//        public void OnRewarded(IRewardItem reward)
//        {
//            Toast.MakeText(_context, string.Format("OnRewarded ! currency: {0} amount: {1}", reward.GetType(), reward.Amount), ToastLength.Short).Show();
//        }

//        public void OnRewardedVideoAdClosed()
//        {
//            Toast.MakeText(_context, "OnRewardedVideoAdClosed", ToastLength.Short).Show();
//            LoadRewardedVideoAd();
//        }

//        public void OnRewardedVideoAdFailedToLoad(int errorCode)
//        {
//            lock (_lock)
//            {
//                _isRewardedVideoLoading = false;
//            }
//            Toast.MakeText(_context, "OnRewardedVideoAdFailedToLoad Code error : " + errorCode, ToastLength.Short).Show();
//        }

//        public void OnRewardedVideoAdLeftApplication()
//        {
//            Toast.MakeText(_context, "OnRewardedVideoAdLeftApplication", ToastLength.Short).Show();
//        }

//        public void OnRewardedVideoAdLoaded()
//        {
//            lock (_lock)
//            {
//                _isRewardedVideoLoading = false;
//            }
//            Toast.MakeText(_context, "OnRewardedVideoAdLoaded", ToastLength.Short).Show();
//        }

//        public void OnRewardedVideoAdOpened()
//        {
//            Toast.MakeText(_context, "OnRewardedVideoAdOpened", ToastLength.Short).Show();
//        }

//        public void OnRewardedVideoStarted()
//        {
//            Toast.MakeText(_context, "OnRewardedVideoStarted", ToastLength.Short).Show();
//        }

//        #endregion

//        #endregion

//    }

//    public class MyAdListener : AdListener
//    {
//        private Action _onAdClosed;

//        public MyAdListener(Action onAdClosed)
//        {
//            _onAdClosed = onAdClosed;
//        }

//        public override void OnAdClosed()
//        {
//            if (_onAdClosed != null)
//                _onAdClosed();
//        }
//    }
//}