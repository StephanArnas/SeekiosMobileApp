
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SeekiosApp.ViewModel
{
    public class ListAlertsViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService;
        private Interfaces.IDialogService _dialogService;
        private IDataService _dataService;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Seekios on which we configure an alert</summary>
        public SeekiosDTO Seekios { get; set; }

        /// <summary>True: notification push activated on the current seekios mode</summary>
        public bool NotificationPushMode
        {
            get
            {
                if (Seekios != null) return Convert.ToBoolean(App.CurrentUserEnvironment.GetModeFromSeekios(Seekios).NotificationPush);
                else return false;
            }
        }

        /// <summary>Selected Seekios alert list</summary>
        public List<AlertDTO> LsSeekiosAlerts
        {
            get
            {
                return App.CurrentUserEnvironment.GetAlertsFromSeekios(Seekios);
            }
        }

        /// <summary>Whole alert list</summary>
        public List<AlertDTO> LsAllAlerts
        {
            get
            {
                return App.CurrentUserEnvironment.GetAlertsFromSeekios();
            }
        }

        #endregion

        #region ===== Constructor =================================================================

        public ListAlertsViewModel(IDataService dataService, Interfaces.IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void GoToAlert(AlertDTO item)
        {
            App.Locator.Alert.IsNew = false;
            App.Locator.Alert.ConvertAlertDTOToClassMember(item);
            _navigationService.NavigateTo(App.ALERT_PAGE);
        }

        #endregion
    }
}