using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class MapViewModel : MapViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService;

        #endregion

        #region ===== Properties ==================================================================

        public bool IsSeekiosInAlert
        {
            get
            {
                return Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved
                    || Mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone;
            }
        }


        #endregion

        #region ===== Constructor =================================================================

        public MapViewModel(IDispatchOnUIThread dispatcher
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ILocalNotificationService localNotificationService)
            : base(dispatcher, dataService, dialogService, localNotificationService)
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void GoToHistoric()
        {
            _navigationService.NavigateTo(App.MAP_HISTORIC_PAGE);
        }

        #endregion
    }
}
