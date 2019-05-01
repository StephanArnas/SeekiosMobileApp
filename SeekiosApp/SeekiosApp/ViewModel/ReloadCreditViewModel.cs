using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SeekiosApp.Enum.FromDataBase;

namespace SeekiosApp.ViewModel
{
    public class ReloadCreditViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDataService _dataService = null;

        #endregion

        #region ===== Constructor =================================================================

        public ReloadCreditViewModel(IDataService dataService, Interfaces.IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================
        
        public async Task<int> InsertPurchase(string key, string data, string signature, PlateformeVersionEnum platform)
        {
            try
            {
                var purchase = new PurchaseDTO()
                {
                    IdUser = App.CurrentUserEnvironment.User.IdUser,
                    InnerData = data,
                    Signature = signature,
                    StoreId = (int)platform,
                    VersionApp = App.Locator.Login.VersionApplication,
                    KeyProduct = key
                };
                return await _dataService.InsertInAppPurchase(purchase);
            }
            catch (TimeoutException)
            {
                await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null);
            }
            catch (WebException)
            {
                await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null);
            }
            catch (Exception)
            {
                await _dialogService.ShowError(
                    Resources.UnexpectedError
                    , Resources.UnexpectedErrorTitle
                    , Resources.Close, null);
            }
            return -1;
        }

        public async Task ShowMessage(string message, string title)
        {
            await _dialogService.ShowMessage(message, title);
        }

        #endregion
    }
}
