using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class TransactionHistoricViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private Interfaces.IDialogService _dialogService;
        private IDataService _dataService;

        #endregion

        #region ===== Properties ==================================================================

        public List<OperationDTO> LsOperation { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public TransactionHistoricViewModel(IDataService dataService, Interfaces.IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            LsOperation = new List<OperationDTO>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public async Task<bool> GetTransactionHistoricByUser()
        {
            try
            {
                var response = await _dataService.OperationHistoric();
                if (response == null)
                {
                    await _dialogService.ShowMessage(Resources.UnexpectedError, Resources.UnexpectedErrorTitle);
                }
                LsOperation = response != null ? response : LsOperation;
                return true;
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
            return false;
        }

        #endregion
    }
}
