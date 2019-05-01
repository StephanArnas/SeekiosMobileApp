//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Com.Android.Vending.Billing;
//using Xamarin.InAppBilling;
//using SeekiosApp.Droid.Helper;
//using SeekiosApp.Interfaces;

//namespace SeekiosApp.Droid.Services
//{
//    // read tuto : http://developer.android.com/google/play/billing/api.html
//    // & https://components.xamarin.com/gettingstarted/xamarin.inappbilling
//    public class InAppBillingServiceConnection : Java.Lang.Object, IServiceConnection, IJavaObject, IDisposable
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context = null;
//        private IDataService _dataService = null;

//        #endregion

//        #region ===== Properties ==================================================================

//        /// <summary>Gets the Google Play <c>InAppBillingService</c> interface</summary>
//        public IInAppBillingService Service { get; private set; }

//        /// <summary>Gets the <see cref="T:Xamarin.InAppBilling.InAppBillingHandler" /> used to communicate with the Google Play Service</summary>
//        public IInAppBillingHandler BillingHandler { get; private set; }

//        /// <summary>Gets a value indicating whether this <see cref="T:Xamarin.InAppBilling.InAppBillingServiceConnection" /> is connected to the Google Play service</summary>
//        public bool Connected { get; private set; }

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public InAppBillingServiceConnection(Activity activity, IDataService dataService)
//        {
//            _context = activity;
//            _dataService = dataService;
//        }

//        #endregion

//        #region ===== Public Methodes =============================================================

//        /// <summary>
//        /// Connect this instance to the Google Play service to support In-App Billing in your application
//        /// </summary>
//        public void Connect()
//        {
//            Intent intent = new Intent("com.android.vending.billing.InAppBillingService.BIND");
//            intent.SetPackage("com.android.vending");
//            if (_context.PackageManager.QueryIntentServices(intent, 0).Count != 0)
//            {
//                _context.BindService(intent, this, Bind.AutoCreate);
//            }
//            else
//            {
//                // Don't put any async/await
//                App.Locator.ReloadCredit.ShowMessage(_context.Resources.GetString(Resource.String.packOnPlayStoreUnavailable)
//                    , _context.Resources.GetString(Resource.String.packOnErrorTitle));
//                _context.Finish();
//            }
//        }

//        /// <summary>
//        /// Disconnects this instance from the Google Play service.
//        /// </summary>
//        /// <remarks>Important: Remember to unbind from the In-app Billing service when you are done with your activity.
//        /// If you don’t unbind, the open service connection could cause your device’s performance to degrade. To unbind
//        /// and free your system resources, call the <c>Disconnect</c> method when your Activity gets destroyed.</remarks>
//        public void Disconnect(bool unbind)
//        {
//            if(unbind) _context.UnbindService(this);
//            Connected = false;
//            Service = null;
//            BillingHandler = null;
//            App.Locator.ReloadCredit.OnDisconnected?.Invoke(null, null);
//        }

//        /// <summary>
//        /// The connection is successful
//        /// Callback from IServiceConnection
//        /// </summary>
//        public void OnServiceConnected(ComponentName name, IBinder service)
//        {
//            Service = IInAppBillingServiceStub.AsInterface(service);
//            var packageName = _context.PackageName;
//            try
//            {
//                var num = Service.IsBillingSupported(Billing.APIVersion, packageName, ItemType.Product);
//                if (num == BillingResult.OK)
//                {
//                    num = Service.IsBillingSupported(Billing.APIVersion, packageName, ItemType.Subscription);
//                    if (num != BillingResult.OK)
//                    {
//                        App.Locator.ReloadCredit.OnError(string.Format("Subscriptions NOT AVAILABLE. Response: {0}", num), InAppBillingErrorType.SubscriptionsNotSupported);
//                        Connected = false;
//                    }
//                    else
//                    {
//                        Connected = true;
//                        BillingHandler = new InAppBillingHandler(_context, Service, _dataService);
//                        App.Locator.ReloadCredit.OnConnected?.Invoke(null, null);
//                    }
//                }
//                else
//                {
//                    App.Locator.ReloadCredit.OnError(string.Format("In-app billing version 3 NOT supported for {0}", packageName), InAppBillingErrorType.BillingNotSupported);
//                    Connected = false;
//                }
//            }
//            catch (Exception exception1)
//            {
//                Exception exception = exception1;
//                App.Locator.ReloadCredit.OnError(exception.ToString(), InAppBillingErrorType.UnknownError);
//                Connected = false;
//            }
//        }

//        /// <summary>
//        /// Raises the service disconnected event.
//        /// </summary>
//        public void OnServiceDisconnected(ComponentName name)
//        {
//            /*this.Connected = false;
//            this.Service = null;
//            this.BillingHandler = null;
//            //this.RaiseOnDisconnected();
//            App.Locator.ReloadCredit.OnDisconnected.Invoke(null, null);*/
//            Disconnect(false);
//        }

//        #endregion
//    }
//}