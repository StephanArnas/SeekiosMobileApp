//using System;
//using System.Linq;
//using Android.OS;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using Xamarin.InAppBilling;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Android.Util;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabCreditPackFragment : Android.Support.V4.App.Fragment
//    {
//        #region ===== Attributs ===================================================================

//        private ReloadCreditActivity _context = null;
//        private bool _isSubscriptionPack = false;

//        #endregion

//        #region ===== Properties ==================================================================

//        #region Observation Pack

//        public Button PackObservationBuyButton { get; set; }
//        public TextView PackObsTitle { get; set; }
//        public TextView PackObsPrice { get; set; }
//        public TextView PackObsRefillAmount { get; set; }

//        #endregion

//        #region Discovery Pack

//        public Button PackDiscoveryBuyButton { get; set; }
//        public TextView PackDiscoTitle { get; set; }
//        public TextView PackDiscoPrice { get; set; }
//        public TextView PackDiscoRefillAmount { get; set; }

//        #endregion

//        #region Exploration Pack

//        public Button PackExplorationBuyButton { get; set; }
//        public TextView PackExploTitle { get; set; }
//        public TextView PackExploPrice { get; set; }
//        public TextView PackExploRefillAmount { get; set; }

//        #endregion

//        #region Adventure Pack

//        public Button PackAdventureBuyButton { get; set; }
//        public TextView PackAdvTitle { get; set; }
//        public TextView PackAdvPrice { get; set; }
//        public TextView PackAdvRefillAmount { get; set; }


//        #endregion

//        #region Epic Pack

//        public Button PackEpicBuyButton { get; set; }
//        public TextView PackEpicTitle { get; set; }
//        public TextView PackEpicPrice { get; set; }
//        public TextView PackEpicRefillAmount { get; set; }

//        #endregion

//        #endregion

//        #region ===== Constructor =================================================================

//        public TabCreditPackFragment(ReloadCreditActivity context, bool IsSubscriptionPack = false)
//        {
//            _isSubscriptionPack = IsSubscriptionPack;
//            _context = context;
//        }

//        #endregion

//        #region ===== Life Cycle ==================================================================

//        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            Android.Views.View view = null;
//            if (_isSubscriptionPack) view = inflater.Inflate(Resource.Layout.TabSubscriptionCredit, container, false);
//            else view = inflater.Inflate(Resource.Layout.TabCreditPack, container, false);

//            #region Observation

//            PackObsTitle = view.FindViewById<TextView>(Resource.Id.packTitle_obs);
//            PackObsPrice = view.FindViewById<TextView>(Resource.Id.packPrice_obs);
//            PackObservationBuyButton = view.FindViewById<Button>(Resource.Id.buyButton_obs);
//            PackObsRefillAmount = view.FindViewById<TextView>(Resource.Id.creditNumber_obs);

//            #endregion

//            #region Discovery

//            PackDiscoTitle = view.FindViewById<TextView>(Resource.Id.packTitle_disco);
//            PackDiscoPrice = view.FindViewById<TextView>(Resource.Id.packPrice_disco);
//            PackDiscoveryBuyButton = view.FindViewById<Button>(Resource.Id.buyButton_discovery);
//            PackDiscoRefillAmount = view.FindViewById<TextView>(Resource.Id.creditNumber_discovery);

//            #endregion

//            #region Exploration

//            PackExploTitle = view.FindViewById<TextView>(Resource.Id.packTitle_explo);
//            PackExploPrice = view.FindViewById<TextView>(Resource.Id.packPrice_explo);
//            PackExplorationBuyButton = view.FindViewById<Button>(Resource.Id.buyButton_explo);
//            PackExploRefillAmount = view.FindViewById<TextView>(Resource.Id.creditNumber_explo);

//            #endregion

//            #region Adventure

//            PackAdvTitle = view.FindViewById<TextView>(Resource.Id.packTitle_adv);
//            PackAdvPrice = view.FindViewById<TextView>(Resource.Id.packPrice_adventure);
//            PackAdventureBuyButton = view.FindViewById<Button>(Resource.Id.buyButton_adventure);
//            PackAdvRefillAmount = view.FindViewById<TextView>(Resource.Id.creditNumber_adventure);

//            #endregion

//            #region Epic

//            PackEpicTitle = view.FindViewById<TextView>(Resource.Id.packTitle_epic);
//            PackEpicPrice = view.FindViewById<TextView>(Resource.Id.packPrice_epic);
//            PackEpicBuyButton = view.FindViewById<Button>(Resource.Id.buyButton_epic);
//            PackEpicRefillAmount = view.FindViewById<TextView>(Resource.Id.creditNumber_epic);

//            #endregion

//            return view;
//        }

//        public override void OnResume()
//        {
//            base.OnResume();
//            PackObservationBuyButton.Click += OnClickBuyObservationPack;
//            PackDiscoveryBuyButton.Click += OnClickBuyDiscoveryPack;
//            PackExplorationBuyButton.Click += OnClickBuyExplorationPack;
//            PackEpicBuyButton.Click += OnClickBuyEpicPack;
//            PackAdventureBuyButton.Click += OnClickBuyAdventurePack;
//        }

//        public override void OnPause()
//        {
//            base.OnPause();
//            PackObservationBuyButton.Click -= OnClickBuyObservationPack;
//            PackDiscoveryBuyButton.Click -= OnClickBuyDiscoveryPack;
//            PackExplorationBuyButton.Click -= OnClickBuyExplorationPack;
//            PackEpicBuyButton.Click -= OnClickBuyEpicPack;
//            PackAdventureBuyButton.Click -= OnClickBuyAdventurePack;
//        }

//        #endregion

//        #region ===== Public methods ==============================================================

//        public void InitializeAllLayouts()
//        {
//            if (_context.LsProducts.Count <= 0) return;
//            InitializeObservationLayout();
//            InitializeDiscoveryLayout();
//            InitializeExplorationLayout();
//            InitializeAdventureLayout();
//            InitializeEpicLayout();
//        }

//        public void InitializeObservationLayout()
//        {
//            var observationPack = _context.LsProducts.FirstOrDefault(el => el.ProductId == ("observation_pack"));
//            if (observationPack != null )
//            {
//                PackObsTitle.Text = observationPack.Title.Replace("(Seekios)", string.Empty);
//                PackObsPrice.Text = observationPack.Price;
//                PackObsRefillAmount.Text = observationPack.Description;
//            }
//        }

//        public void InitializeDiscoveryLayout()
//        {
//            var discoveryPack = _context.LsProducts.FirstOrDefault(el => el.ProductId == ("discovery_pack"));
//            if (discoveryPack != null)
//            {
//                PackDiscoTitle.Text = discoveryPack.Title.Replace("(Seekios)", string.Empty);
//                PackDiscoPrice.Text = discoveryPack.Price;
//                PackDiscoRefillAmount.Text = discoveryPack.Description;
//            }

//        }

//        public void InitializeExplorationLayout()
//        {
//            var explorationPack = _context.LsProducts.FirstOrDefault(el => el.ProductId == ("exploration_pack"));
//            if (explorationPack != null)
//            {
//                PackExploTitle.Text = explorationPack.Title.Replace("(Seekios)", string.Empty);
//                PackExploPrice.Text = explorationPack.Price;
//                PackExploRefillAmount.Text = explorationPack.Description;
//            }
//        }

//        public void InitializeAdventureLayout()
//        {
//            var adventurePack = _context.LsProducts.FirstOrDefault(el => el.ProductId == ("adventure_pack"));
//            if (adventurePack != null)
//            {
//                PackAdvTitle.Text = adventurePack.Title.Replace("(Seekios)", string.Empty);
//                PackAdvPrice.Text = adventurePack.Price;
//                PackAdvRefillAmount.Text = adventurePack.Description;
//            }
//        }

//        public void InitializeEpicLayout()
//        {
//            var epicPack = _context.LsProducts.FirstOrDefault(el => el.ProductId == ("epic_pack"));
//            if (epicPack != null)
//            {
//                PackEpicTitle.Text = epicPack.Title.Replace("(Seekios)", string.Empty);
//                PackEpicPrice.Text = epicPack.Price;
//                PackEpicRefillAmount.Text = epicPack.Description;
//            }
//        }

//        #endregion

//        #region ===== Events ======================================================================

//        private void OnClickBuyObservationPack(object sender, EventArgs args)
//        {
//            _context.BillingHandler.BuyProduct(_context.LsProducts.FirstOrDefault(el => el.ProductId == "observation_pack"));
//        }

//        private void OnClickBuyDiscoveryPack(object sender, EventArgs args)
//        {
//            _context.BillingHandler.BuyProduct(_context.LsProducts.FirstOrDefault(el => el.ProductId == "discovery_pack"));
//        }

//        private void OnClickBuyExplorationPack(object sender, EventArgs args)
//        {
//            _context.BillingHandler.BuyProduct(_context.LsProducts.FirstOrDefault(el => el.ProductId == "exploration_pack"));
//        }

//        private void OnClickBuyAdventurePack(object sender, EventArgs args)
//        {
//            _context.BillingHandler.BuyProduct(_context.LsProducts.FirstOrDefault(el => el.ProductId == "adventure_pack"));
//        }

//        private void OnClickBuyEpicPack(object sender, EventArgs args)
//        {
//            _context.BillingHandler.BuyProduct(_context.LsProducts.FirstOrDefault(el => el.ProductId == "epic_pack"));
//        }

//        #endregion
//    }
//}