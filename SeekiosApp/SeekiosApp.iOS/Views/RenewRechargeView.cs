using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.Views;
using CoreAnimation;
using Xamarin.InAppPurchase;
using System.Collections.Generic;
using SeekiosApp.Enum.FromDataBase;

namespace SeekiosApp.iOS
{
	public partial class RenewRechargeView : UIViewController
    {
        #region ===== Properties ==================================================================

        private InAppPurchaseManager PurchaseManager = new InAppPurchaseManager();
		private List<InAppProduct> InAppProducts = new List<InAppProduct>();
		public static UIStoryboard StoryboardVal = UIStoryboard.FromName("ListSeekios", null);

		#endregion

		#region ===== Constructor =================================================================

		public RenewRechargeView (IntPtr handle) : base (handle) { }

		#endregion

		//#region ===== Life Cycle ==================================================================

		//public override void ViewDidLoad()
		//{
		//	base.ViewDidLoad();

		//	DiscoveryButton.TouchUpInside += DiscoveryButton_TouchUpInside;
		//	ExplorationButton.TouchUpInside += ExplorationButton_TouchUpInside;
		//	AdventureButton.TouchUpInside += AdventureButton_TouchUpInside;
		//	EpicButton.TouchUpInside += EpicButton_TouchUpInside;

		//	InformationTextView.Editable = false;

		//	SetupInApp();
		//}

		//public override void ViewWillAppear(bool animated)
		//{
		//	base.ViewWillAppear(animated);

		//	this.ParentViewController.NavigationItem.Title = @"Recharge";

		//	DiscoveryButton.Layer.CornerRadius = 4;
		//	DiscoveryButton.Layer.MasksToBounds = true;

		//	ExplorationButton.Layer.CornerRadius = 4;
		//	ExplorationButton.Layer.MasksToBounds = true;

		//	AdventureButton.Layer.CornerRadius = 4;
		//	AdventureButton.Layer.MasksToBounds = true;

		//	EpicButton.Layer.CornerRadius = 4;
		//	EpicButton.Layer.MasksToBounds = true;


		//	CALayer profileImageCircle1 = DiscoveryDiscountView.Layer;
		//	profileImageCircle1.CornerRadius = DiscoveryDiscountView.Frame.Size.Width / 2;
		//	DiscoveryDiscountView.ClipsToBounds = true;

		//	CALayer profileImageCircle2 = ExplorationDiscountView.Layer;
		//	profileImageCircle2.CornerRadius = ExplorationDiscountView.Frame.Size.Width / 2;
		//	ExplorationDiscountView.ClipsToBounds = true;

		//	CALayer profileImageCircle3 = AdventureDiscountView.Layer;
		//	profileImageCircle3.CornerRadius = AdventureDiscountView.Frame.Size.Width / 2;
		//	AdventureDiscountView.ClipsToBounds = true;

		//	CALayer profileImageCircle4 = EpicDiscountView.Layer;
		//	profileImageCircle4.CornerRadius = EpicDiscountView.Frame.Size.Width / 2;
		//	EpicDiscountView.ClipsToBounds = true; ;
		//}

  //      #endregion

  //      #region ===== In Purchase =================================================================

  //      /// <summary>
  //      /// Attachs to purchase manager.
  //      /// </summary>
  //      /// <param name="purchaseManager">Purchase manager.</param>
  //      public void AttachToPurchaseManager(UIStoryboard storyboard, InAppPurchaseManager purchaseManager)
		//{
		//	// Respond to events
		//	PurchaseManager.ReceivedValidProducts += (products) =>
		//	{
		//		// Received valid products from the iTunes App Store,
		//		// Update the displ
		//		InPurchaseInfo();
		//	};

		//	purchaseManager.InAppProductPurchased += (transaction, product) =>
		//	{
		//		purchaseMethod(transaction, product);
		//	};

		//	purchaseManager.InAppPurchasesRestored += (count) =>
		//	{
		//		// Update list to remove any non-consumable products that were
		//		// purchased and restored

		//		using (
		//			var alert = new UIAlertView("Seekios.InAppPurchase"
		//			, string.Format("Attempt to Restaurer {0} prodcut has Successful", count)
		//			, null
		//			, "OK"
		//			, null))
		//		{
		//			alert.Show();
		//		}

		//	};

		//	purchaseManager.InAppProductPurchaseFailed += (transaction, product) =>
		//	{
		//		// Inform caller that the purchase of the requested product failed.
		//		// NOTE: The transaction will normally encode the reason for the failure but since
		//		// we are running in the simulated iTune App Store mode, no transaction will be returned.
		//		//Display Alert Dialog Box
		//		using (var alert = new UIAlertView("Seekios.InAppPurchase"
  //                  , string.Format("Attempt to purchase {0} has failed: {1}"
  //                  , product.Title
  //                  , transaction.Error.ToString())
  //                  , null
  //                  , "OK"
  //                  , null))
		//		{
		//			alert.Show();
		//		}

		//		//InsertInAppPurchase(App.Locator.ReloadCredit.CreatePurchaseDTO(product.ProductIdentifier, empty, str, PlateformeVersionEnum.IOS)
		//		// Force a reload to clear any locked items
		//	};
		//}

		//private async void purchaseMethod(StoreKit.SKPaymentTransaction transaction, InAppProduct product)
		//{
		//	// update list to remove any non-consumable products that were
		//	var result = await App.Locator.ReloadCredit.InsertPurchase(product.ProductIdentifier
		//	, string.Empty
		//	, string.Empty
		//	, PlateformeVersionEnum.IOS);

		//	if (result == 1)
		//	{
		//		using (var alert = new UIAlertView("Achat Pack"
		//		, string.Format("L'achat du pack {0} à réussi"
		//		, product.Title)
		//		, null
		//		, "OK"
		//		, null))
		//		{
		//			alert.Show();
		//		}
		//	}
		//}

		//public void SetupInApp()
		//{
		//	// Assembly public key
		//	string value = Xamarin.InAppPurchase.Utilities.Security.Unify(
		//		new string[] { "1322f985c2",
		//			"a34166b24",
		//			"ab2b367",
		//			"851cc6" },
		//		new int[] { 0, 1, 2, 3 });

		//	PurchaseManager.PublicKey = value;
		//	PurchaseManager.ApplicationUserName = "Seekios";

		//	// Warn user that the store is not available
		//	if (PurchaseManager.CanMakePayments)
		//	{
		//		Console.WriteLine("Seekios.InAppBilling: User can make payments to iTunes App Store.");
		//	}
		//	else {
		//		//Display Alert Dialog Box
		//		using (var alert = new UIAlertView("Seekios.InAppBilling"
  //                  , "Sorry but you cannot make purchases from the In App Billing store. Please try again later."
  //                  , null
  //                  , "OK"
  //                  , null))
		//		{
		//			alert.Show();
		//		}
		//	}

		//	// Warn user if the Purchase Manager is unable to connect to
		//	// the network.
		//	PurchaseManager.NoInternetConnectionAvailable += () =>
		//	{
		//		//Display Alert Dialog Box
		//		using (var alert = new UIAlertView("Seekios.InAppBilling"
  //                  , "No open internet connection is available."
  //                  , null
  //                  , "OK"
  //                  , null))
		//		{
		//			alert.Show();
		//		}
		//	};

		//	// Show any invalid product queries
		//	PurchaseManager.ReceivedInvalidProducts += (productIDs) =>
		//	{
		//		// Display any invalid product IDs to the console
		//		Console.WriteLine("The following IDs were rejected by the iTunes App Store:");
		//		foreach (string ID in productIDs)
		//		{
		//			Console.WriteLine(ID);
		//		}
		//		Console.WriteLine(" ");
		//	};

		//	// Setup automatic purchase persistance and load any previous purchases
		//	PurchaseManager.AutomaticPersistenceType = InAppPurchasePersistenceType.LocalFile;
		//	PurchaseManager.PersistenceFilename = "AtomicData";
		//	PurchaseManager.ShuffleProductsOnPersistence = false;
		//	PurchaseManager.RestoreProducts();

		//	PurchaseManager.QueryInventory(new string[] {
		//		"com.thingsoftomorrow.seekios.discovery.subscription",
		//		"com.thingsoftomorrow.seekios.exploration.subscription",
		//		"com.thingsoftomorrow.seekios.aventure.subscription",
		//		"com.thingsoftomorrow.seekios.epopee.subscription"
		//	});

		//	InAppProducts.Clear();
		//	AttachToPurchaseManager(StoryboardVal, PurchaseManager);
		//}

  //      #endregion

  //      #region ===== Event =======================================================================

  //      private void DiscoveryButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	var index = InAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.discovery.subscription");
		//	BuyProducts(index);
		//}

		//private void ExplorationButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	var index = InAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.exploration.subscription");
		//	BuyProducts(index);
		//}

		//private void AdventureButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	var index = InAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.aventure.subscription");
		//	BuyProducts(index);
		//}

		//private void EpicButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	var index = InAppProducts.FindIndex(a => a.ProductIdentifier == "com.thingsoftomorrow.seekios.epopee.subscription");
		//	BuyProducts(index);
		//}

		//private void BuyProducts(int index)
		//{
		//	if (InAppProducts.Count <= 0 || index == -1) return;

		//	var alert = new UIAlertView("Renouvellement Automatique"
  //              , string.Format("Voulez-vous acheter le pack {0} pour {1}?"
  //              , InAppProducts[index].Title
		//	    , InAppProducts[index].FormattedPrice)
  //              , null, "Annuler", "Acheter","Restaurer");
		//	//Wireup events
		//	alert.CancelButtonIndex = 0;
		//	alert.Clicked += (caller, buttonArgs) =>
		//	{
		//		// Does the user want to purchase?
		//		if (buttonArgs.ButtonIndex == 1)
		//		{
		//			//Yes, request purchase item	
		//			PurchaseManager.BuyProduct(InAppProducts[index]);
		//		}
		//		else if (buttonArgs.ButtonIndex == 2){
		//			PurchaseManager.RestorePreviousPurchases();
		//		}
		//	};

		//	//Display dialog
		//	alert.Show();
		//}

		//private void InPurchaseInfo()
		//{
		//	// Find all purchased product
		//	foreach (InAppProduct product in PurchaseManager)
		//	{
		//		var alert = new UIAlertView("Renouvellement Automatique"
  //                  , string.Format("Do you want to buy {0} for {1}?"
  //                  , product.ProductIdentifier
  //                  , product.FormattedPrice)
  //                  , null, "Cancel", "Buy");

		//		// Take action based on the product typee
		//		switch (product.ProductType)
		//		{
		//			case InAppProductType.NonConsumable:
		//				if (product.Downloadable)
		//				{
		//					InAppProducts.Add(product);
		//				}
		//				else {
		//					InAppProducts.Add(product);
		//				}
		//				break;
		//			case InAppProductType.Consumable:
		//				InAppProducts.Add(product);
		//				break;
		//			case InAppProductType.AutoRenewableSubscription:
		//				InAppProducts.Add(product);
		//				break;
		//			case InAppProductType.FreeSubscription:
		//				InAppProducts.Add(product);
		//				break;
		//			case InAppProductType.NonRenewingSubscription:
		//				InAppProducts.Add(product);
		//				break;
		//			case InAppProductType.Unknown:
		//				InAppProducts.Add(product);
		//				break;

		//		}
		//	}
		//}

		//#endregion
	}
}