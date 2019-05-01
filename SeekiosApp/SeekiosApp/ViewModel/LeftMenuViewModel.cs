using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Enum;
using System;

namespace SeekiosApp.ViewModel
{
    public class LeftMenuViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        /// <summary>Navigation service</summary>
        private INavigationService _navigationService;

        #endregion

        #region ===== Properties ==================================================================

        public string UserFullName
        {
            get
            {
                if (App.CurrentUserEnvironment != null && App.CurrentUserEnvironment.User != null)
                    return string.Format("{0} {1}"
                        , App.CurrentUserEnvironment.User.FirstName
                        , App.CurrentUserEnvironment.User.LastName);
                else return string.Empty;
            }
        }

        #endregion

        #region ===== Constructor =================================================================

        public LeftMenuViewModel()
        {
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Navigate to the page login
        /// </summary>
        public void GoToLogin()
        {
            if (_navigationService.CurrentPageKey != App.LOGIN_PAGE)
            {
                _navigationService.NavigateTo(App.LOGIN_PAGE);
            }
        }

        /// <summary>
        /// Navigate to the page add seekios
        /// </summary>
        public void GoToAddSeekios()
        {
            if (_navigationService.CurrentPageKey != App.ADD_SEEKIOS_PAGE)
            {
                App.Locator.AddSeekios.IsAdding = true;
                _navigationService.NavigateTo(App.ADD_SEEKIOS_PAGE);
            }
        }

        /// <summary>
        /// Navigate to global seekios map (only on iOS)
        /// </summary>
        public void GoToSeekiosMapAllSeekios()
        {
            if (_navigationService.CurrentPageKey != App.MAP_ALL_SEEKIOS_PAGE)
            {
                _navigationService.NavigateTo(App.MAP_ALL_SEEKIOS_PAGE);
            }
        }

        /// <summary>
        /// Navigate to the page community 
        /// </summary>
        [Obsolete("community feature is not enable yet")]
        public void GoToCommunity()
        {
            if (_navigationService.CurrentPageKey != App.COMMUNITY_PAGE)
            {
                _navigationService.NavigateTo(App.COMMUNITY_PAGE);
            }
        }

        /// <summary>
        /// Navigate to the page user parameter
        /// </summary>
        public void GoToParameter()
        {
            if (_navigationService.CurrentPageKey != App.PARAMETER_PAGE)
            {
                _navigationService.NavigateTo(App.PARAMETER_PAGE);
            }
        }

        /// <summary>
        /// Navigate to the page of Condition general for use (only on Android, on iOS it's opening a web browser)
        /// </summary>
        public void GoToCGU()
        {
            if (_navigationService.CurrentPageKey != App.CGU_PAGE)
            {
                _navigationService.NavigateTo(App.CGU_PAGE);
            }
        }

        /// <summary>
        /// Navigate to the page tutorial
        /// </summary>
        public void GoToListTutorial()
        {
            if (_navigationService.CurrentPageKey != App.LIST_TUTORIAL_PAGE)
            {
                _navigationService.NavigateTo(App.LIST_TUTORIAL_PAGE);
            }
        }

        #endregion
    }
}