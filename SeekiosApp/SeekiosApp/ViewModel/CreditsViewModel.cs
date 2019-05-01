using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;

namespace SeekiosApp.ViewModel
{
    public class CreditsViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService;

        #endregion

        #region ===== Constructor =================================================================

        public CreditsViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void GoToCreditHistoric()
        {
            if (_navigationService.CurrentPageKey != App.TRANSACTION_HISTORIC_PAGE)
            {
                _navigationService.NavigateTo(App.TRANSACTION_HISTORIC_PAGE);
            }
        }

        public void GoToReloadCredit()
        {
            if (_navigationService.CurrentPageKey != App.RELOAD_CREDIT_PAGE)
            {
                _navigationService.NavigateTo(App.RELOAD_CREDIT_PAGE);
            }
        }
        
        #endregion
    }
}
