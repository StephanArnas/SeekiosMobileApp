//using System;
//using System.Collections.Generic;
//using System.Linq;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Com.Android.Vending.Billing;
//using System.Threading.Tasks;
//using Xamarin.InAppBilling;
//using Xamarin.InAppBilling.Utilities;
//using Newtonsoft.Json;
//using Android.Util;
//using GalaSoft.MvvmLight;
//using SeekiosApp.Interfaces;
//using SeekiosApp.Enum.FromDataBase;

//namespace SeekiosApp.Droid.Services
//{
//    public class InAppBillingHandler : ViewModelBase, IInAppBillingHandler
//    {
//        #region ===== Attributs ===================================================================

//        private const int PurchaseRequestCode = 1001;
//        private Activity _activity;
//        private string _payload;
//        private IInAppBillingService _billingService;
//        private IDataService _dataService;

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        /// <summary>
//        /// Initialise une instance de <see cref="InAppService.InAppBillingHelper"/> class.
//        /// </summary>
//        /// <param name="activity">Activité</param>
//        /// <param name="billingService">Service Billing</param>
//        public InAppBillingHandler(Activity activity, IInAppBillingService billingService, IDataService dataService)
//        {
//            this._billingService = billingService;
//            this._activity = activity;
//            this._dataService = dataService;

//        }

//        #endregion

//        /// <param name="product">The <see cref="!:Xamarin.Android.InAppBilling.Product" /> representing the item the users wants to
//        /// purchase.</param>
//        /// <summary>
//        /// Buys the given <see cref="!:Xamarin.Android.InAppBilling.Product" /></summary>
//        /// <remarks>This method automatically generates a unique GUID and attaches it as the developer payload for this purchase.
//        /// </remarks>
//        public void BuyProduct(Product product)
//        {
//            this._payload = Guid.NewGuid().ToString();
//            this.BuyProduct(product.ProductId, product.Type, this._payload);
//        }

//        /// <param name="product">The <see cref="!:Xamarin.Android.InAppBilling.Product" /> representing the item the users wants to
//        /// purchase.</param>
//        /// <param name="payload">The developer payload to attach to the purchase.</param>
//        /// <summary>
//        /// Buys the given <see cref="!:Xamarin.Android.InAppBilling.Product" /> and attaches the given developer payload to the
//        /// purchase.
//        /// </summary>
//        /// <remarks>To be added.</remarks>
//        public void BuyProduct(Product product, string payload)
//        {
//            this.BuyProduct(product.ProductId, product.Type, payload);
//        }

//        /// <param name="sku">The SKU of the item to purchase.</param>
//        /// <param name="itemType">The type of the item to purchase.</param>
//        /// <param name="payload">The developer payload to attach to the purchase.</param>
//        /// <summary>
//        /// Buys a product based on the given product SKU and Item Type attaching the given payload
//        /// </summary>
//        /// <remarks>To be added.</remarks>
//        public void BuyProduct(string sku, string itemType, string payload)
//        {
//            try
//            {
//                var buyIntent = _billingService.GetBuyIntent(Billing.APIVersion, _activity.PackageName, sku, itemType, payload);
//                int responseCodeFromBundle = buyIntent.GetResponseCodeFromBundle();
//                if (responseCodeFromBundle == BillingResult.OK)
//                {
//                    var parcelable = buyIntent.GetParcelable(Response.BuyIntent) as PendingIntent;
//                    if (parcelable != null)
//                    {
//                        _activity.StartIntentSenderForResult(parcelable.IntentSender, 1001, new Intent(), 0, 0, 0);
//                    }
//                }
//                else
//                {
//                    // If the customer already owns the pack, then it has not been comsumed, so we need to force consumption.
//                    // We also force consumption of all pending purchases
//                    if (BillingResult.ItemAlreadyOwned == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnItemIsAlreadyOwmed?.Invoke(null, sku);
//                        GetPurchases(ItemType.Product).ContinueWith(x =>
//                        {
//                            ConsumeAll(x.Result);
//                        });
//                    }
//                    else if (BillingResult.BillingUnavailable == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnBillingUnavailable?.Invoke(null, null);
//                    }
//                    else if (BillingResult.Error == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnError?.Invoke(null, null);
//                    }
//                    else if (BillingResult.DeveloperError == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnDeveloperError?.Invoke(null, null);
//                    }
//                    else if (BillingResult.UserCancelled == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnUserCanceled?.Invoke(null, null);
//                    }
//                    else if (BillingResult.ItemUnavailable == responseCodeFromBundle)
//                    {
//                        App.Locator.ReloadCredit.OnItemUnavailable?.Invoke(null, null);
//                    }
//                    else App.Locator.ReloadCredit.OnProductPurchasedError(responseCodeFromBundle, sku);
//                }
//            }
//            catch (Exception exception)
//            {
//                App.Locator.ReloadCredit.OnError(string.Format("Erreur {0}", exception.ToString()), null);
//            }
//        }

//        public void ConsumeAll(IList<Purchase> pendingPurchases)
//        {
//            foreach (Purchase p in pendingPurchases)
//            {
//                ConsumePurchase(p);
//            }
//        }

//        /// <param name="token">The purchase token of the purchase to consume.</param>
//        /// <summary>
//        /// Consumes the purchased item
//        /// </summary>
//        /// <returns>
//        ///     <c>true</c> if the purchase is successfully consumed else returns <c>false</c>.</returns>
//        /// <remarks>To be added.</remarks>
//        public bool ConsumePurchase(Purchase purchase)
//        {
//            if (purchase == null)
//            {
//                Log.Info("Seekios", "Purchase receipt is null");
//                throw new ArgumentNullException("Purchase receipt is null");
//            }
//            bool flag; string token = purchase.PurchaseToken;
//            if (string.IsNullOrEmpty(token))
//            {
//                Log.Info("Seekios", "Purchase token is null");
//                throw new ArgumentException("Purchase token is null");
//            }
//            try
//            {
//                int num = this._billingService.ConsumePurchase(Billing.APIVersion, this._activity.PackageName, token);
//                Log.Info("Seekios", "Consuming purchase '{0}', response: {1}", new object[] { token, num });
//                if (num == BillingResult.OK)
//                {
//                    Log.Info("Seekios", "tout est ok, achat consomme");
//                    //App.Locator.ReloadCredit.OnPurchaseConsumed?.Invoke(token, purchase);
//                    flag = true;
//                }
//                else
//                {
//                    Log.Info("Seekios", "error consumption");
//                    App.Locator.ReloadCredit.OnPurchaseConsumedError?.Invoke(num, token);
//                    flag = false;
//                }
//            }
//            catch (Exception exception)
//            {
//                Log.Info("Seekios", string.Format("Error Consume Purchase: {0}", exception.ToString()));
//                App.Locator.ReloadCredit.OnError?.Invoke(string.Format("Error Consume Purchase: {0}", exception.ToString()), null);
//                flag = false;
//            }
//            return flag;
//        }

//        /// <param name="itemType">Item type (product or subs)</param>
//        /// <summary>
//        /// Gets a list of all products of a given item type purchased by the current user.
//        /// </summary>
//        /// <returns>A list of <see cref="T:Xamarin.InAppBilling.Product" />s purchased by the current user.</returns>
//        /// <remarks>To be added.</remarks>
//        public async Task<IList<Purchase>> GetPurchases(string itemType)
//        {
//            Purchase purchase;
//            Bundle bundle;
//            IList<string> stringArrayList;
//            IList<string> list;
//            IList<string> stringArrayList1;
//            IList<Purchase> list1;
//            string empty = string.Empty;
//            List<Purchase> list2 = new List<Purchase>();
//            Log.Info("Seekios", "Getting purchases of type " + itemType);
//            while (true)
//            {
//                try
//                {
//                    bundle = (empty != string.Empty ? this._billingService.GetPurchases(Billing.APIVersion, this._activity.PackageName, itemType, empty) : this._billingService.GetPurchases(Billing.APIVersion, this._activity.PackageName, itemType, null));
//                    if (bundle != null)
//                    {
//                        int responseCodeFromBundle = bundle.GetResponseCodeFromBundle();
//                        if (responseCodeFromBundle != BillingResult.OK)
//                        {
//                            await App.Locator.ReloadCredit.OnGetProductsError?.Invoke(responseCodeFromBundle, bundle);
//                            list1 = null;
//                            break;
//                        }
//                        else if (InAppBillingHandler.ValidOwnedItems(bundle))
//                        {
//                            stringArrayList = bundle.GetStringArrayList(Response.InAppPurchaseItemList);
//                            list = bundle.GetStringArrayList(Response.InAppPurchaseDataList);
//                            stringArrayList1 = bundle.GetStringArrayList(Response.InAppDataSignatureList);
//                            Log.Info("Seekios", "stringArray = " + stringArrayList + " , list=" + list + " stringArray1=" + stringArrayList1);
//                            if (stringArrayList == null || list == null || stringArrayList1 == null)
//                            {
//                                await App.Locator.ReloadCredit.OnError?.Invoke(string.Format("Invalid owned items bundle returned by Google Play Services: {0}", bundle.ToString()), null);
//                                list1 = null;
//                                break;
//                            }
//                        }
//                        else
//                        {
//                            Log.Info("Seekios", "Invalid purchases");
//                            await App.Locator.ReloadCredit.OnInvalidOwnedItemsBundleReturned?.Invoke(null, bundle);
//                            list1 = list2;
//                            break;
//                        }
//                    }
//                    else
//                    {
//                        Log.Info("Seekios", "No items returned from Google Play Services");
//                        await App.Locator.ReloadCredit.OnError?.Invoke("No items returned from Google Play Services", null);
//                        list1 = null;
//                        break;
//                    }
//                }
//                catch (Exception exception)
//                {
//                    Log.Info("Seekios", string.Format("Error retrieving previous purchases: {0}", exception.ToString()));
//                    await App.Locator.ReloadCredit.OnError?.Invoke(string.Format("Error retrieving previous purchases: {0}", exception.ToString()), null);
//                    list1 = null;
//                    break;
//                }


//                for (int i = 0; i < stringArrayList.Count; i++)
//                {
//                    string item = list[i];
//                    string str = stringArrayList1[i];
//                    Log.Info("Seekios", "item=" + item + ", str=" + str);
//                    try
//                    {
//                        purchase = JsonConvert.DeserializeObject<Purchase>(item);
//                    }
//                    catch (Exception exception2)
//                    {
//                        Exception exception1 = exception2;
//                        Log.Info("Seekios", "GetPurchases Error {0}: Unable to deserialize purchase '{1}'.\n Setting Purchase.DeveloperPayload with info returned from Google.", new object[] { exception1.ToString(), item });
//                        purchase = new Purchase()
//                        {
//                            DeveloperPayload = item
//                        };
//                    }
//                    try
//                    {
//                        if (purchase.ProductId.Contains("android.test."))
//                        {
//                            list2.Add(purchase);
//                        }
//                        else//le purchase a pas encore ete achete
//                        {
//                            Log.Info("Seekios", "publicKey=" + "null" + ", signedData=" + empty + ", signature=" + str);
//                            if (purchase.PurchaseState == BillingResult.OK)
//                            {
//                                list2.Add(purchase);
//                            }
//                            else if (_dataService != null && BillingResult.OK == await App.Locator.ReloadCredit.InsertPurchase(purchase.ProductId, empty, str, PlateformeVersionEnum.Android))
//                            {
//                                list2.Add(purchase);
//                                Log.Info("Seekios", "tout est ok, paiement valide");
//                                //maybe add an event like "OnServerValidatedPayment" -> notify the user that our servers validated the payment, its credits ought to be updated soon
//                            }
//                            //else { await App.Locator.ReloadCredit.OnPurchaseFailedValidation(null, purchase); }
//                        }
//                    }
//                    catch (Exception exception4)
//                    {
//                        Exception exception3 = exception4;
//                        await App.Locator.ReloadCredit.OnError(string.Format("Error validating previous purchase {0}: {1}", purchase.ProductId, exception3.ToString()), null);
//                        list1 = null;
//                        return list1;
//                    }
//                }
//                empty = bundle.GetString(Response.InAppContinuationToken);
//                if (string.IsNullOrWhiteSpace(empty))
//                {
//                    return list2;
//                }
//            }
//            return list1;
//        }

//        /// <summary>
//        /// Handles the activity result.
//        /// </summary>
//        public async Task<int> HandleActivityResult(int requestCode, Result resultCode, Intent data)
//        {
//            var reponseCodeFromIntent = 0;
//            var empty = string.Empty;
//            var stringExtra = string.Empty;
//            if (requestCode != 1001 || data == null) return 1;
//            try
//            {
//                reponseCodeFromIntent = data.GetReponseCodeFromIntent();
//                if (BillingResult.ItemAlreadyOwned == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnItemIsAlreadyOwmed?.Invoke(null, null);
//                }
//                if (BillingResult.BillingUnavailable == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnBillingUnavailable?.Invoke(null, null);
//                }
//                else if (BillingResult.Error == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnError?.Invoke("error", "alksdjalds");
//                }
//                else if (BillingResult.DeveloperError == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnDeveloperError?.Invoke(null, null);
//                }
//                else if (BillingResult.UserCancelled == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnUserCanceled?.Invoke(null, null);
//                }
//                else if (BillingResult.ItemUnavailable == reponseCodeFromIntent)
//                {
//                    await App.Locator.ReloadCredit.OnItemUnavailable?.Invoke(null, null);
//                }
//                empty = data.GetStringExtra(Response.InAppPurchaseData);
//                stringExtra = data.GetStringExtra(Response.InAppDataSignature);
//            }
//            catch (Exception exception)
//            {
//                await App.Locator.ReloadCredit.OnError?.Invoke(exception.ToString(), null);
//                return 1;
//            }
//            Purchase purchase = null;
//            try
//            {
//                purchase = JsonConvert.DeserializeObject<Purchase>(empty);
//            }
//            catch (Exception/*exception2*/)
//            {
//                //await App.Locator.ReloadCredit.OnPurchaseFailedValidation?.Invoke(exception2.ToString(), purchase = new Purchase()
//                //{
//                //    DeveloperPayload = empty
//                //});
//                return 1;
//            }
//            try
//            {
//                if (purchase != null && purchase.ProductId.Contains("android.test.")) //this should disapear in production code
//                {
//                    await App.Locator.ReloadCredit.OnPurchaseValidated?.Invoke(reponseCodeFromIntent, purchase);
//                }
//                else
//                {
//                    Log.Info("Seekios", "publicKey=" + "null" + ", signedData=" + empty + ", signature=" + stringExtra);
//                    var resultServer = await App.Locator.ReloadCredit.InsertPurchase(purchase.ProductId, empty, stringExtra, PlateformeVersionEnum.Android);
//                    Log.Info("Seekios", "resultServer: " + resultServer);
//                    if (_dataService != null && purchase != null && resultServer == 1)
//                    {
//                        Log.Info("Seekios", "tout est ok, paiement valide");
//                        await App.Locator.ReloadCredit.OnPurchaseValidated?.Invoke(reponseCodeFromIntent, purchase);
//                    }
//                    else
//                    {
//                        Log.Info("Seekios", "mauvais paiement " +  purchase);
//                        await App.Locator.ReloadCredit.OnPurchaseFailedValidation?.Invoke(null, purchase);
//                        return 1;
//                    }
//                }
//            }
//            catch (Exception exception3)
//            {
//                await App.Locator.ReloadCredit.OnError?.Invoke(string.Format("Error Decoding Returned Packet Information: {0}", exception3.ToString()), null);
//            }
//            return 0;
//        }

//        /// <param name="skuList">Sku list.</param>
//        /// <param name="itemType">The <see cref="!:Xamarin.Android.InAppBilling.ItemType" /> of product being queried.</param>
//        /// <summary>
//        /// Queries the inventory asynchronously and returns a list of <see cref="!:Xamarin.Android.InAppBilling.Product" />s matching
//        /// the given list of SKU numbers.
//        /// </summary>
//        /// <returns>List of <see cref="!:Xamarin.Android.InAppBilling.Product" />s matching the given list of SKUs.</returns>
//        public Task<IList<Product>> QueryInventoryAsync(IList<string> skuList, string itemType)
//        {
//            var getSkuDetailsTask = Task.Factory.StartNew<IList<Product>>(() =>
//            {
//                Bundle querySku = new Bundle();
//                querySku.PutStringArrayList(Billing.ItemIdList, skuList);
//                Bundle skuDetails = _billingService.GetSkuDetails(Billing.APIVersion, _activity.PackageName, itemType, querySku);
//                if (!skuDetails.ContainsKey(Billing.SkuDetailsList))
//                {
//                    return null;
//                }
//                var products = skuDetails.GetStringArrayList(Billing.SkuDetailsList);
//                return (products == null) ? null : products.Select(Newtonsoft.Json.JsonConvert.DeserializeObject<Product>).ToList();
//            });

//            return getSkuDetailsTask;
//        }


//        private static bool ValidOwnedItems(Bundle purchased)
//        {
//            return (!purchased.ContainsKey(Response.InAppPurchaseItemList) || !purchased.ContainsKey(Response.InAppPurchaseDataList) ? false : purchased.ContainsKey(Response.InAppDataSignatureList));
//        }
//    }
//}