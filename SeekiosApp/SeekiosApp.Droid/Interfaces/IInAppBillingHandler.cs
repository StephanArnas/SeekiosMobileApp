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
using System.Threading.Tasks;
using Xamarin.InAppBilling;

namespace SeekiosApp.Droid.Services
{
    /// <summary>
    /// Defines the interface that all InAppBillingHandlers used with Google Play In-App Billing need to support.
    /// </summary>
    public interface IInAppBillingHandler
    {
        /// <param name="sku">Sku.</param>
        /// <param name="itemType">Item type.</param>
        /// <param name="payload">Payload.</param>
        /// <summary>
        /// Buys an item.
        /// </summary>
        /// <remarks>To be added.</remarks>
        void BuyProduct(string sku, string itemType, string payload);

        /// <param name="product">Product.</param>
        /// <summary>
        /// Buys an items
        /// </summary>
        /// <remarks>To be added.</remarks>
        void BuyProduct(Product product);

        /// <param name="purchase">Purchased item</param>
        /// <summary>
        /// Consumes the purchased item
        /// </summary>
        /// <returns>
        ///     <c>true</c>, if purchased item was consumed, <c>false</c> otherwise.</returns>
        /// <remarks>To be added.</remarks>
        bool ConsumePurchase(Purchase purchase);

        void ConsumeAll(IList<Purchase> allPurchases);

        /// <param name="token">Token.</param>
        /// <summary>
        /// Consumes the purchased item
        /// </summary>
        /// <returns>
        ///     <c>true</c>, if purchased item was consumed, <c>false</c> otherwise.</returns>
        /// <remarks>To be added.</remarks>
        //bool ConsumePurchase(string token);

        /// <param name="itemType">Item type (inapp or subs)</param>
        /// <summary>
        /// Gets the purchases.
        /// </summary>
        /// <returns>The purchases.</returns>
        /// <remarks>To be added.</remarks>
        Task<IList<Purchase>> GetPurchases(string itemType);

        /// <param name="requestCode">Request code.</param>
        /// <param name="resultCode">Result code.</param>
        /// <param name="data">Data.</param>
        /// <summary>
        /// Handles the activity result.
        /// </summary>
        /// <remarks>To be added.</remarks>
        Task<int> HandleActivityResult(int requestCode, Result resultCode, Intent data);

        /// <param name="skuList">Sku list.</param>
        /// <param name="itemType">Item type.</param>
        /// <summary>
        /// Queries the inventory asynchronously.
        /// </summary>
        /// <returns>List of strings</returns>
        /// <remarks>To be added.</remarks>
        Task<IList<Product>> QueryInventoryAsync(IList<string> skuList, string itemType);
    }
}