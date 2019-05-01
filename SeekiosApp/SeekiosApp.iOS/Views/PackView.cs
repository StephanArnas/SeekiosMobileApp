using Foundation;
using System;
using UIKit;
using Xamarin.InAppPurchase;
using Xamarin.InAppPurchase.Utilities;
using System.Collections.Generic;
using SeekiosApp.iOS.Views;
using SeekiosApp.Enum.FromDataBase;
using SeekiosApp.iOS.Helper;

namespace SeekiosApp.iOS
{
    public partial class PackView : UIViewController, IPurchaseViewController
    {
        #region ===== Attributs ===================================================================

        private InAppPurchaseManager _purchaseManager = null;

        #endregion

        #region ===== Properties ==================================================================

        public static List<InAppProduct> ListInAppProducts { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public PackView(IntPtr handle) : base(handle)
        {
            _purchaseManager = AppDelegate.PurchaseManager;
        }

        public PackView() : base()
        {
            ListInAppProducts = new List<InAppProduct>();
        }

        #endregion

        #region ====== Life Cycle =================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ObservationButton.TouchUpInside += ObservationButton_TouchUpInside;
            DiscoveryButton.TouchUpInside += DiscoveryButton_TouchUpInside;
            ExplorationButton.TouchUpInside += ExplorationButton_TouchUpInside;
            AdventureButton.TouchUpInside += AdventureButton_TouchUpInside;
            EpicButton.TouchUpInside += EpicButton_TouchUpInside;
            InformationTextView.Editable = false;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            InitialiseAllStrings();

            //this.ParentViewController.NavigationItem.Title = @"Pack";
            this.ParentViewController.NavigationItem.Title = "";

            // Round Corner of Buttonn
            ObservationButton.Layer.CornerRadius = 4;
            ObservationButton.Layer.MasksToBounds = true;

            DiscoveryButton.Layer.CornerRadius = 4;
            DiscoveryButton.Layer.MasksToBounds = true;

            ExplorationButton.Layer.CornerRadius = 4;
            ExplorationButton.Layer.MasksToBounds = true;

            AdventureButton.Layer.CornerRadius = 4;
            AdventureButton.Layer.MasksToBounds = true;

            EpicButton.Layer.CornerRadius = 4;
            EpicButton.Layer.MasksToBounds = true;
        }

        private void InitialiseAllStrings()
        {
            ObservationTitle.Text = Application.LocalizedString("Observation");
            DiscoveryTitle.Text = Application.LocalizedString("Discovery");
            ExplorationTitle.Text = Application.LocalizedString("Exploration");
            AdventureTitle.Text = Application.LocalizedString("Adventure");
            EpicTitle.Text = Application.LocalizedString("Epic");
            ObsCreditLabel.Text = Application.LocalizedString("Credits");
            DiscoCreditLabel.Text = Application.LocalizedString("Credits");
            ExploCreditLabel.Text = Application.LocalizedString("Credits");
            EpicCreditLabel.Text = Application.LocalizedString("Credits");
            AdvCreditLabel.Text = Application.LocalizedString("Credits");

            InformationTextView.Text = Application.LocalizedString("CreditUseInformation");
            if (ListInAppProducts.Count > 0) SetupUIWithPacks();
        }

        #endregion

        #region ====== In Purchase ================================================================

        public async void SetupInAppPurchase()
        {
            // assembly public key
            string value = Xamarin.InAppPurchase.Utilities.Security.Unify(
                new string[] { "1322f985c2",
                    "a34166b24",
                    "ab2b367",
                    "851cc6" },
                new int[] { 0, 1, 2, 3 });

            _purchaseManager.PublicKey = value;
            _purchaseManager.ApplicationUserName = "Seekios";

            // be sure the user can make payment
            if (!_purchaseManager.CanMakePayments)
            {
                // the user is not able to make payment
                await AlertControllerHelper.ShowAlert(Application.LocalizedString("PaymentFailure")
                            , Application.LocalizedString("CantPurchase")
                            , Application.LocalizedString("Close"));
            }

            // be sure the user has access to internet
            _purchaseManager.NoInternetConnectionAvailable += async () =>
            {
                // the user is has not internet
                await AlertControllerHelper.ShowAlert(Application.LocalizedString("PaymentFailure")
                            , Application.LocalizedString("NoInternet")
                            , Application.LocalizedString("Close"));
            };

            // display any invalid product IDs 
            _purchaseManager.ReceivedInvalidProducts += (productIDs) =>
            {
                Console.WriteLine("The following IDs were rejected by the iTunes App Store:");
                foreach (string ID in productIDs)
                {
                    Console.WriteLine(ID);
                }
            };

            // setup automatic purchase persistance and load any previous purchases
            _purchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.LocalFile;
            _purchaseManager.PersistenceFilename = "AtomicData";
            _purchaseManager.ShuffleProductsOnPersistence = false;
            _purchaseManager.RestoreProducts();
            _purchaseManager.QueryInventory(new string[]
            {
                "com.thingsoftomorrow.seekios.observation",
                "com.thingsoftomorrow.seekios.discovery",
                "com.thingsoftomorrow.seekios.exploration",
                "com.thingsoftomorrow.seekios.aventure",
                "com.thingsoftomorrow.seekios.epopee"
            });

            if (ListInAppProducts != null && ListInAppProducts.Count != 0)
            {
                ListInAppProducts.Clear();
            }

            //AttachToPurchaseManager(null, _purchaseManager);
        }

        private void LoadData()
        {
            if (_purchaseManager != null && _purchaseManager.Count > 0)
            {
                // find all purchased products
                foreach (InAppProduct product in _purchaseManager)
                {
                    // take action based on the product typee
                    switch (product.ProductType)
                    {
                        case InAppProductType.NonConsumable:
                            if (product.Downloadable)
                            {
                                ListInAppProducts.Add(product);
                            }
                            //else
                            //{
                            //    ListInAppProducts.Add(product);
                            //}
                            break;
                        case InAppProductType.Consumable:
                            ListInAppProducts.Add(product);
                            break;
                        case InAppProductType.AutoRenewableSubscription:
                            ListInAppProducts.Add(product);
                            break;
                        case InAppProductType.FreeSubscription:
                            ListInAppProducts.Add(product);
                            break;
                        case InAppProductType.NonRenewingSubscription:
                            ListInAppProducts.Add(product);
                            break;
                        case InAppProductType.Unknown:
                            ListInAppProducts.Add(product);
                            break;
                    }
                }
            }
        }

        private void SetupUIWithPacks()
        {
            foreach (var product in ListInAppProducts)
            {
                switch (product.ProductIdentifier)
                {
                    case "com.thingsoftomorrow.seekios.observation":
                        ObservationButton.SetTitle(string.Format(Application.LocalizedString("BuyPrice"), product.FormattedPrice), UIControlState.Normal);
                        //ObservationEuroLabel.Text = product.FormattedPrice; -- HIDDEN CONTROL
                        break;
                    case "com.thingsoftomorrow.seekios.discovery":
                        DiscoveryButton.SetTitle(string.Format(Application.LocalizedString("BuyPrice"), product.FormattedPrice), UIControlState.Normal);
                        //DiscoveryEuroLabel.Text = product.FormattedPrice;
                        break;
                    case "com.thingsoftomorrow.seekios.exploration":
                        ExplorationButton.SetTitle(string.Format(Application.LocalizedString("BuyPrice"), product.FormattedPrice), UIControlState.Normal);
                        //ExplorationEuroLabel.Text = product.FormattedPrice;
                        break;
                    case "com.thingsoftomorrow.seekios.aventure":
                        AdventureButton.SetTitle(string.Format(Application.LocalizedString("BuyPrice"), product.FormattedPrice), UIControlState.Normal);
                        //AdventureEuroLabel.Text = product.FormattedPrice;
                        break;
                    case "com.thingsoftomorrow.seekios.epopee":
                        EpicButton.SetTitle(string.Format(Application.LocalizedString("BuyPrice"), product.FormattedPrice), UIControlState.Normal);
                        //EpicEuroLabel.Text = product.FormattedPrice;
                        break;
                }
            }
        }

        private async void BuyProducts(int index)
        {
            if (ListInAppProducts.Count <= 0 || index == -1) return;
            if (!App.DeviceIsConnectedToInternet)
            {
                await AlertControllerHelper.ShowAlert(Application.LocalizedString("PaymentFailure")
                    , Application.LocalizedString("NoInternet")
                    , Application.LocalizedString("Close"));
                return;
            }

            var alert = new UIAlertView(Application.LocalizedString("BuyPack")
                , string.Format(Application.LocalizedString("SureToBuy")
                , GetProductTitle(ListInAppProducts[index].ProductIdentifier)
                , ListInAppProducts[index].FormattedPrice)
                , null
                , Application.LocalizedString("Cancel")
                , Application.LocalizedString("Buy"));

            // wireup events
            alert.CancelButtonIndex = 0;
            alert.Clicked += (caller, buttonArgs) =>
            {
                // does the user want to purchase?
                if (buttonArgs.ButtonIndex == 1)
                {
                    // yes, request purchase item	
                    _purchaseManager.BuyProduct(ListInAppProducts[index]);
                }
            };

            // display dialog
            alert.Show();
        }

        private string GetProductTitle(string productId)
        {
            var result = string.Empty;
            switch (productId)
            {
                case "com.thingsoftomorrow.seekios.observation":
                    result = Application.LocalizedString("ObservationPack");
                    break;
                case "com.thingsoftomorrow.seekios.discovery":
                    result = Application.LocalizedString("DiscoveryPack");
                    break;
                case "com.thingsoftomorrow.seekios.exploration":
                    result = Application.LocalizedString("ExplorationPack");
                    break;
                case "com.thingsoftomorrow.seekios.aventure":
                    result = Application.LocalizedString("AdventurePack");
                    break;
                case "com.thingsoftomorrow.seekios.epopee":
                    result = Application.LocalizedString("EpicPack");
                    break;
            }
            return result;
        }

        private async void ProductPurchased(StoreKit.SKPaymentTransaction transaction, InAppProduct product)
        {
            // update list to remove any non-consumable products that were
            var result = await App.Locator.ReloadCredit.InsertPurchase(product.ProductIdentifier
            , string.Empty
            , string.Empty
            , PlateformeVersionEnum.IOS);

            if (result == 0)
            {
                using (var alert = new UIAlertView(Application.LocalizedString("BuyPack")
                , string.Format(Application.LocalizedString("BuySuccess")
                , product.Title)
                , null
                , Application.LocalizedString("OK")
                , null))
                {
                    alert.Show();
                }
            }
        }

        #endregion

        #region ===== Interface Implementation ====================================================

        public void AttachToPurchaseManager(UIStoryboard Storyboard, InAppPurchaseManager purchaseManager)
        {
            _purchaseManager = purchaseManager;
            purchaseManager.ReceivedValidProducts += (products) =>
            {
                // received valid products from the iTunes App Store,
                // update the displ
                LoadData();
            };

            purchaseManager.InAppProductPurchased += (transaction, product) =>
            {
                ProductPurchased(transaction, product);
            };

            purchaseManager.InAppPurchasesRestored += (count) =>
            {
                // update list to remove any non-consumable products that were
                // purchased and restored
            };

            purchaseManager.InAppProductPurchaseFailed += (transaction, product) =>
            {
                //var tt = product.ProductIdentifier;
                //ProductPurchased(transaction, product);
                // Inform caller that the purchase of the requested product failed.
                // NOTE: The transaction will normally encode the reason for the failure but since
                // we are running in the simulated iTune App Store mode, no transaction will be returned.
                //Display Alert Dialog Box
                using (var alert = new UIAlertView(Application.LocalizedString("BuyPack")
                    , string.Format(Application.LocalizedString("BuyFailed")
                    , product.Title
                    , transaction.Error.ToString())
                    , null
                    , Application.LocalizedString("OK")
                    , null))
                {
                    alert.Show();
                }
                // InsertInAppPurchase(App.Locator.ReloadCredit.CreatePurchaseDTO(product.ProductIdentifier, empty, str, PlateformeVersionEnum.IOS)
                // Force a reload to clear any locked items
            };
        }

        #endregion

        #region ===== Event =======================================================================

        private void ObservationButton_TouchUpInside(object sender, EventArgs e)
        {
            var index = ListInAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.observation");
            BuyProducts(index);
        }

        private void DiscoveryButton_TouchUpInside(object sender, EventArgs e)
        {
            var index = ListInAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.discovery");
            BuyProducts(index);
        }

        private void ExplorationButton_TouchUpInside(object sender, EventArgs e)
        {
            var index = ListInAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.exploration");
            BuyProducts(index);
        }

        private void AdventureButton_TouchUpInside(object sender, EventArgs e)
        {
            var index = ListInAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.aventure");
            BuyProducts(index);
        }

        private void EpicButton_TouchUpInside(object sender, EventArgs e)
        {
            var index = ListInAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.epopee");
            BuyProducts(index);
        }

        #endregion
    }
}