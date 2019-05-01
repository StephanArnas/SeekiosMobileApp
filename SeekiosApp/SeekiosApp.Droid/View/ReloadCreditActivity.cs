using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SeekiosApp.Droid.Services;
using SeekiosApp.Droid.CustomComponents.Adapter;
using Xamarin.InAppBilling;
using Java.Util;
using SeekiosApp.Droid.View.FragmentView;
using SeekiosApp.Model.DTO;
using Android.Util;
using System.Threading.Tasks;
using Xamarin.InAppBilling.Utilities;
using Newtonsoft.Json;
using SeekiosApp.Enum.FromDataBase;

namespace SeekiosApp.Droid.View
{
    [Activity(Theme = "@style/Theme.Normal")]
    public class ReloadCreditActivity : AppCompatActivityBase
    {
        #region ===== Attributs ===================================================================

        private bool _isDiplayingSmth = false;
        private const string _publicKey = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArUR0ggqbrL8muNt/VtcM+AjrT4Yolg4GV0fxBXcejnU0KnVFk/7joyRNbLwYuL6BJfvJZ7kZxra2T9gd5j7lXwNsmI16jmbllCOeMIWnpwzGWhchdCC84xfiCJyrXxl8u6bsmam4NStHwZnRqFG789bPvZYDF5syOmru+2pGoENfxcPy0Oltn41Em0+XpZnJn1ntHzTUFcay1CCwSrUJPQsuPIJijCV48sBIWQjKCqANfMI9VI4gtHep82aJUjfacwl+WFJFI95XQH7yrL8AInzY/9dC7k38cBMUS8FMHfPlbOY0ZfUNpo5u1zN7Y6QJX107XMGx0XtiMXk3MSEgAQIDAQAB";

        #endregion

        #region ===== Properties ==================================================================

        public Xamarin.InAppBilling.InAppBillingServiceConnection ServiceConnection { get; set; }

        public Xamarin.InAppBilling.InAppBillingHandler BillingHandler { get; set; }

        public IList<Product> LsProducts { get; set; }

        #region Observation Pack

        public Button PackObservationBuyButton { get; set; }
        public TextView PackObsTitle { get; set; }
        public TextView PackObsPrice { get; set; }
        public TextView PackObsRefillAmount { get; set; }

        #endregion

        #region Discovery Pack

        public Button PackDiscoveryBuyButton { get; set; }
        public TextView PackDiscoTitle { get; set; }
        public TextView PackDiscoPrice { get; set; }
        public TextView PackDiscoRefillAmount { get; set; }

        #endregion

        #region Exploration Pack

        public Button PackExplorationBuyButton { get; set; }
        public TextView PackExploTitle { get; set; }
        public TextView PackExploPrice { get; set; }
        public TextView PackExploRefillAmount { get; set; }

        #endregion

        #region Adventure Pack

        public Button PackAdventureBuyButton { get; set; }
        public TextView PackAdvTitle { get; set; }
        public TextView PackAdvPrice { get; set; }
        public TextView PackAdvRefillAmount { get; set; }


        #endregion

        #region Epic Pack

        public Button PackEpicBuyButton { get; set; }
        public TextView PackEpicTitle { get; set; }
        public TextView PackEpicPrice { get; set; }
        public TextView PackEpicRefillAmount { get; set; }

        #endregion

        #endregion

        #region ===== Life Cycle ==================================================================

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ReloadCreditLayout);

            GetObjectsFromView();
            SetDataToView();

            if (ToolbarPage != null)
            {
                ToolbarPage.SetTitle(Resource.String.reloadCredit_title);
                SetSupportActionBar(ToolbarPage);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            }
            LoadInAppBilling();
        }

        protected override void OnResume()
        {
            base.OnResume();
            PackObservationBuyButton.Click += OnClickBuyObservationPack;
            PackDiscoveryBuyButton.Click += OnClickBuyDiscoveryPack;
            PackExplorationBuyButton.Click += OnClickBuyExplorationPack;
            PackEpicBuyButton.Click += OnClickBuyEpicPack;
            PackAdventureBuyButton.Click += OnClickBuyAdventurePack;
        }

        protected override void OnPause()
        {
            base.OnPause();
            PackObservationBuyButton.Click -= OnClickBuyObservationPack;
            PackDiscoveryBuyButton.Click -= OnClickBuyDiscoveryPack;
            PackExplorationBuyButton.Click -= OnClickBuyExplorationPack;
            PackEpicBuyButton.Click -= OnClickBuyEpicPack;
            PackAdventureBuyButton.Click -= OnClickBuyAdventurePack;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                if (ServiceConnection != null)
                {
                    ServiceConnection.Disconnect();
                    ServiceConnection.Dispose();
                }
            }
            catch (Exception) { }
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

        private void GetObjectsFromView()
        {
            ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            LoadingLayout = FindViewById<RelativeLayout>(Resource.Id.loadingPanel);

            #region Observation

            PackObsTitle = FindViewById<TextView>(Resource.Id.packTitle_obs);
            PackObsPrice = FindViewById<TextView>(Resource.Id.packPrice_obs);
            PackObservationBuyButton = FindViewById<Button>(Resource.Id.buyButton_obs);
            PackObsRefillAmount = FindViewById<TextView>(Resource.Id.creditNumber_obs);

            #endregion

            #region Discovery

            PackDiscoTitle = FindViewById<TextView>(Resource.Id.packTitle_disco);
            PackDiscoPrice = FindViewById<TextView>(Resource.Id.packPrice_disco);
            PackDiscoveryBuyButton = FindViewById<Button>(Resource.Id.buyButton_discovery);
            PackDiscoRefillAmount = FindViewById<TextView>(Resource.Id.creditNumber_discovery);

            #endregion

            #region Exploration

            PackExploTitle = FindViewById<TextView>(Resource.Id.packTitle_explo);
            PackExploPrice = FindViewById<TextView>(Resource.Id.packPrice_explo);
            PackExplorationBuyButton = FindViewById<Button>(Resource.Id.buyButton_explo);
            PackExploRefillAmount = FindViewById<TextView>(Resource.Id.creditNumber_explo);

            #endregion

            #region Adventure

            PackAdvTitle = FindViewById<TextView>(Resource.Id.packTitle_adv);
            PackAdvPrice = FindViewById<TextView>(Resource.Id.packPrice_adventure);
            PackAdventureBuyButton = FindViewById<Button>(Resource.Id.buyButton_adventure);
            PackAdvRefillAmount = FindViewById<TextView>(Resource.Id.creditNumber_adventure);

            #endregion

            #region Epic

            PackEpicTitle = FindViewById<TextView>(Resource.Id.packTitle_epic);
            PackEpicPrice = FindViewById<TextView>(Resource.Id.packPrice_epic);
            PackEpicBuyButton = FindViewById<Button>(Resource.Id.buyButton_epic);
            PackEpicRefillAmount = FindViewById<TextView>(Resource.Id.creditNumber_epic);

            #endregion
        }

        private void SetDataToView()
        {
            LoadingLayout.Visibility = ViewStates.Visible;
            ToolbarPage.SetTitle(Resource.String.reloadCredit_title);
        }

        #endregion

        #region ===== InAppBilling ================================================================

        private void LoadInAppBilling()
        {
            ServiceConnection = new Xamarin.InAppBilling.InAppBillingServiceConnection(this, _publicKey);
            ServiceConnection.OnInAppBillingError += ServiceConnection_OnInAppBillingError;
            ServiceConnection.OnConnected += ServiceConnection_OnConnected;

            try
            {
                ServiceConnection.Connect();
            }
            catch (Java.Lang.Exception) { }
            catch (Java.Lang.Throwable) { }
        }

        #endregion

        #region ===== Private Methods =============================================================

        public void InitializeObservationLayout()
        {
            var observationPack = LsProducts.FirstOrDefault(el => el.ProductId == ("observation_pack"));
            if (observationPack != null)
            {
                PackObsTitle.Text = observationPack.Title.Replace("(Seekios)", string.Empty);
                PackObsPrice.Text = observationPack.Price;
                PackObsRefillAmount.Text = observationPack.Description;
            }
        }

        public void InitializeDiscoveryLayout()
        {
            var discoveryPack = LsProducts.FirstOrDefault(el => el.ProductId == ("discovery_pack"));
            if (discoveryPack != null)
            {
                PackDiscoTitle.Text = discoveryPack.Title.Replace("(Seekios)", string.Empty);
                PackDiscoPrice.Text = discoveryPack.Price;
                PackDiscoRefillAmount.Text = discoveryPack.Description;
            }

        }

        public void InitializeExplorationLayout()
        {
            var explorationPack = LsProducts.FirstOrDefault(el => el.ProductId == ("exploration_pack"));
            if (explorationPack != null)
            {
                PackExploTitle.Text = explorationPack.Title.Replace("(Seekios)", string.Empty);
                PackExploPrice.Text = explorationPack.Price;
                PackExploRefillAmount.Text = explorationPack.Description;
            }
        }

        public void InitializeAdventureLayout()
        {
            var adventurePack = LsProducts.FirstOrDefault(el => el.ProductId == ("adventure_pack"));
            if (adventurePack != null)
            {
                PackAdvTitle.Text = adventurePack.Title.Replace("(Seekios)", string.Empty);
                PackAdvPrice.Text = adventurePack.Price;
                PackAdvRefillAmount.Text = adventurePack.Description;
            }
        }

        public void InitializeEpicLayout()
        {
            var epicPack = LsProducts.FirstOrDefault(el => el.ProductId == ("epic_pack"));
            if (epicPack != null)
            {
                PackEpicTitle.Text = epicPack.Title.Replace("(Seekios)", string.Empty);
                PackEpicPrice.Text = epicPack.Price;
                PackEpicRefillAmount.Text = epicPack.Description;
            }
        }

        #endregion

        #region ===== Events ======================================================================

        private async void ServiceConnection_OnConnected()
        {
            // Subscribe to event where the billing handler could be wrong
            BillingHandler = ServiceConnection.BillingHandler;
            BillingHandler.InAppBillingProcesingError += BillingHandler_InAppBillingProcesingError;
            BillingHandler.QueryInventoryError += BillingHandler_QueryInventoryError;
            BillingHandler.OnPurchaseConsumed += BillingHandler_OnPurchaseConsumed;

            // Get the list of product from the playstore
            LsProducts = await BillingHandler.QueryInventoryAsync(new string[]
            {
                "observation_pack"
                , "discovery_pack"
                , "exploration_pack"
                , "adventure_pack"
                , "epic_pack"
            }, ItemType.Product);

            // Initalize the view with the values from the playstore
            if (LsProducts?.Count > 0)
            {
                InitializeObservationLayout();
                InitializeDiscoveryLayout();
                InitializeExplorationLayout();
                InitializeAdventureLayout();
                InitializeEpicLayout();
                LoadingLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                await App.Locator.ReloadCredit.ShowMessage(
                    Resources.GetString(Resource.String.packOnGetProductsError)
                    , Resources.GetString(Resource.String.packOnErrorTitle));
                Finish();
            }
        }

        protected async override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            Log.Info("Seekios", "requestCode: " + resultCode + " resultCode: " + resultCode + " data: " + data.GetReponseCodeFromIntent());

            // Get data from the callback
            BillingHandler.HandleActivityResult(requestCode, resultCode, data);
            var inAppPurchaseData = data.GetStringExtra(Response.InAppPurchaseData);
            var inAppDataSignature = data.GetStringExtra(Response.InAppDataSignature);
            Purchase purchaseItem = null;

            try
            {
                // Deserialize data
                purchaseItem = JsonConvert.DeserializeObject<Purchase>(inAppPurchaseData);

                if (purchaseItem == null)
                {
                    Log.Info("Seekios", "Error deserialize purchaseItem ! InAppPurchaseData: " + inAppPurchaseData + " InAppDataSignature: " + inAppDataSignature);
                    return;
                }
                else if (purchaseItem.ProductId.Contains("android.test.")) //this should disapear in production code
                {
                    Log.Info("Seekios", "Error productId contains android.test ! InAppPurchaseData: " + inAppPurchaseData + " InAppDataSignature: " + inAppDataSignature);
                    return;
                }
                else
                {
                    // Save the in app purchase in the database
                    Log.Info("Seekios", "Sucess deserialize purchaseItem ! InAppPurchaseData: " + inAppPurchaseData + " InAppDataSignature: " + inAppDataSignature);
                    var resultServer = await App.Locator.ReloadCredit.InsertPurchase(purchaseItem.ProductId
                        , inAppPurchaseData
                        , inAppDataSignature
                        , PlateformeVersionEnum.Android);
                    Log.Info("Seekios", "resultServer: " + resultServer);
                    if (resultServer == 1)
                    {
                        // Consume all the purchases
                        Log.Info("Seekios", "The InsertPurchases succed ! resultServer" + resultServer);
                        var purchases = BillingHandler.GetPurchases(ItemType.Product);
                        if (purchases?.Count > 0)
                        {
                            foreach (var purchase in purchases)
                            {
                                BillingHandler.ConsumePurchase(purchase);
                                Log.Info("Seekios", "Consume purchase ! productId: " + purchase.ProductId);
                            }
                        }
                        else Log.Info("Seekios", "No purchase(s) to consume");
                    }
                    else OnWebservicesError(resultServer.ToString());
                }
            }
            catch (Exception) { /*OnError();*/ }
        }

        #region CLICK PACK

        private void OnClickBuyObservationPack(object sender, EventArgs args)
        {
            BillingHandler.BuyProduct(LsProducts.FirstOrDefault(el => el.ProductId == "observation_pack"));
        }

        private void OnClickBuyDiscoveryPack(object sender, EventArgs args)
        {
            BillingHandler.BuyProduct(LsProducts.FirstOrDefault(el => el.ProductId == "discovery_pack"));
        }

        private void OnClickBuyExplorationPack(object sender, EventArgs args)
        {
            BillingHandler.BuyProduct(LsProducts.FirstOrDefault(el => el.ProductId == "exploration_pack"));
        }

        private void OnClickBuyAdventurePack(object sender, EventArgs args)
        {
            BillingHandler.BuyProduct(LsProducts.FirstOrDefault(el => el.ProductId == "adventure_pack"));
        }

        private void OnClickBuyEpicPack(object sender, EventArgs args)
        {
            BillingHandler.BuyProduct(LsProducts.FirstOrDefault(el => el.ProductId == "epic_pack"));
        }

        #endregion

        #region SUCCESS

        private async void BillingHandler_OnPurchaseConsumed(string token)
        {
            Log.Info("Seekios", "OnPurchaseConsumed: " + token);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnPurchaseConsumed)
                , Resources.GetString(Resource.String.packOnSuccessTitle));
            _isDiplayingSmth = false;
        }

        #endregion

        #region ERROR

        private async void ServiceConnection_OnInAppBillingError(InAppBillingErrorType error, string message)
        {
            Log.Info("Seekios", "OnInAppBillingError: " + message);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnBillingUnavailable)
                , Resources.GetString(Resource.String.packOnErrorTitle));
            _isDiplayingSmth = false;
            if (!ServiceConnection.Connected) Finish();
        }

        private async void BillingHandler_InAppBillingProcesingError(string message)
        {
            Log.Info("Seekios", "InAppBillingProcesingError: " + message);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnBillingUnavailable)
                , Resources.GetString(Resource.String.packOnErrorTitle));
            _isDiplayingSmth = false;
            Finish();
        }

        private async void BillingHandler_QueryInventoryError(int responseCode, Bundle skuDetails)
        {
            Log.Info("Seekios", "QueryInventoryError: " + responseCode);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnGetProductsError)
                , Resources.GetString(Resource.String.packOnErrorTitle));
            _isDiplayingSmth = false;
        }

        private async void BillingHandler_OnProductPurchasedError(int responseCode, string sku)
        {
            Log.Info("Seekios", "OnProductPurchasedError: " + responseCode);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnError)
                , Resources.GetString(Resource.String.packOnPurchaseFailedValidation));
            _isDiplayingSmth = false;
        }

        private async void OnError()
        {
            Log.Info("Seekios", "OnError");
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                Resources.GetString(Resource.String.packOnError)
                , Resources.GetString(Resource.String.packOnErrorTitle));
            _isDiplayingSmth = false;
        }

        private async void OnWebservicesError(string code)
        {
            Log.Info("Seekios", "OnWebservicesError: " + code);
            if (_isDiplayingSmth) return;
            _isDiplayingSmth = true;
            await App.Locator.ReloadCredit.ShowMessage(
                string.Format(Resources.GetString(Resource.String.packErrorOnWebservices), code)
                , Resources.GetString(Resource.String.packOnErrorTitle));
            _isDiplayingSmth = false;
        }

        #endregion

        #endregion
    }
}