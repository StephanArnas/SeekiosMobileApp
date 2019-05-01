using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Properties;
using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SeekiosApp.Extension;
using SeekiosApp.Services;
using SeekiosApp.Helper;
using SeekiosApp.Model.DTO;
using Newtonsoft.Json;

namespace SeekiosApp.ViewModel
{
    public class ParameterViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDataService _dataService = null;
        private IDispatchOnUIThread _dispatcherService = null;

        private bool _isUpdating = false;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Old password</summary>
        public string OldPassword { get; set; }

        /// <summary>New password</summary>
        public string NewPassword { get; set; }

        /// <summary>New password to confirm</summary>
        public string NewPasswordReenter { get; set; }

        /// <summary>User picture</summary>
        public byte[] UserPicture { get; set; }

        /// <summary>True if profile is currently being updated</summary>
        public bool IsUpdating
        {
            get { return _isUpdating; }
            set { Set(() => IsUpdating, ref _isUpdating, value); }
        }

        #endregion

        #region ===== Constructor =================================================================

        public ParameterViewModel(IDataService dataService, Interfaces.IDialogService dialogService, IDispatchOnUIThread dispatcherService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region Navigation

        /// <summary>
        /// Navigate to list of tutorial
        /// </summary>
        public void GoToTutorialBackground()
        {
            _navigationService.NavigateTo(App.TUTORIAL_BACKGROUND_FIRST_LAUNCH_PAGE);
        }

        public void GoToTutorial()
        {
            _navigationService.NavigateTo(App.TUTORIAL_PAGE);
        }

        /// <summary>
        /// Navigate to list of tutorial
        /// </summary>
        public void GoToListTutorial()
        {
            _navigationService.NavigateTo(App.LIST_TUTORIAL_PAGE);
        }

        /// <summary>
        /// Navigate to seekios Led View
        /// </summary>
        public void GoToSeekiosLed()
        {
            _navigationService.NavigateTo(App.TUTORIAL_SEEKIOS_LED_PAGE);
        }

        /// <summary>
        /// Navigate to tutorial Power Saving
        /// </summary>
        public void GoToTutorialPowerSaving()
        {
            _navigationService.NavigateTo(App.TUTORIAL_POWERSAVING_PAGE);
        }

        /// <summary>
        /// Navigate to tutorial Power Saving
        /// </summary>
        public void GoToTutorialCreditCost()
        {
            _navigationService.NavigateTo(App.TUTORIAL_CREDITCOST_PAGE);
        }

        /// <summary>
        /// Navigate to payment mode page
        /// </summary>
        public void GoToPaymentMode()
        {
            _navigationService.NavigateTo(App.RELOAD_CREDIT_PAGE);
        }

        /// <summary>
        /// Navigate to the about page
        /// </summary>
        public void GoToAboutPage()
        {
            _navigationService.NavigateTo(App.ABOUT_PAGE);
        }

        /// <summar
        /// Navigate to buy credits page 
        /// </summary>
        private void GoToBuyCredits()
        {
            _navigationService.NavigateTo(App.ADD_CREDITS_PAGE);
        }

        #endregion

        #region User Logic

        /// <summary>
        ///  Update user
        /// </summary>
        public async Task<int> UpdateUser(string email, string phoneNumber, string firstName, string lastName)
        {
            try
            {
                // Check if internet is available
                if (!App.DeviceIsConnectedToInternet)
                {
                    await _dialogService.ShowMessage(Resources.WebErrorTitle, Resources.WebErrorButtonText);
                    return -2;
                }

                // Update user data
                App.CurrentUserEnvironment.User.Email = email;
                App.CurrentUserEnvironment.User.FirstName = firstName.ToUpperCaseFirst();
                App.CurrentUserEnvironment.User.LastName = lastName.ToUpperCaseFirst();
                App.CurrentUserEnvironment.User.UserPicture = UserPicture == null ? App.CurrentUserEnvironment.User.UserPicture : Convert.ToBase64String(UserPicture);

                // Show the loading layout
                _dialogService.ShowLoadingLayout();

                if (await _dataService.UpdateUser(App.CurrentUserEnvironment.User) == 1)
                {
                    // Hide the loading layout
                    _dialogService.HideLoadingLayout();
                    return 1;
                }
                else
                {
                    // Error message
                    await _dialogService.ShowMessage(Resources.UpdateUserFailedTitle, Resources.UpdateUserFailedContent);
                    // Hide the loading layout
                    _dialogService.HideLoadingLayout();
                    return -1;
                }
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
            _dialogService.HideLoadingLayout();
            return -1;
        }

        /// <summary>
        /// Update the user
        /// </summary>
        public void UpdateUserSignalR(string uidDevice, string userJson)
        {
            if (string.IsNullOrEmpty(userJson)) return;
            if (uidDevice == App.UidDevice) return;
            var user = JsonConvert.DeserializeObject<UserDTO>(userJson);
            if (user != null)
            {
                App.CurrentUserEnvironment.User = user;
                if (_dispatcherService != null)
                {
                    _dispatcherService.Invoke(() =>
                    {
                        OnUserChanged?.Invoke(null, null);
                    });
                }
            }
        }

        /// <summary>
        /// Update user password in database
        /// </summary>
        public async Task<bool> UpdateNewPasswordChanged()
        {
            var previousPassword = string.Empty;
            try
            {
                IsUpdating = true;
                var hash = CryptographyHelper.CalculatePasswordMD5Hash(App.CurrentUserEnvironment.User.Email, OldPassword);

                if (App.CurrentUserEnvironment.User.Password != hash)
                {
                    var msg = Resources.BadPassword;
                    var title = Resources.BadPasswordTitle;
                    await _dialogService.ShowMessage(msg, title);
                    IsUpdating = false;
                    return false;
                }
                if (NewPassword != NewPasswordReenter)
                {
                    var msg = Resources.PasswordDoesntMatch;
                    var title = Resources.PasswordDoesntMatchTitle;
                    await _dialogService.ShowMessage(msg, title);
                    IsUpdating = false;
                    return false;
                }
                if (NewPassword.Length < 6)
                {
                    //var msg = Resources.PasswordTooShort;
                    //var title = Resources.PasswordTooShortTitle;
                    //await _dialogService.ShowMessage(msg, title);
                    IsUpdating = false;
                    return false;
                }
                previousPassword = App.CurrentUserEnvironment.User.Password;
                App.CurrentUserEnvironment.User.Password = CryptographyHelper.CalculatePasswordMD5Hash(App.CurrentUserEnvironment.User.Email, NewPassword);

                if (await _dataService.UpdateUser(App.CurrentUserEnvironment.User) >= 1)
                {
                    // Save new password for auto reconnection
                    App.Locator.Login.SaveCurrentCredentials();
                    DataService.Pass = App.CurrentUserEnvironment.User.Password;
                    await _dialogService.ShowMessage(Resources.PasswordUpdated, Resources.PasswordUpdateTitle);
                }
                else
                {
                    App.CurrentUserEnvironment.User.Password = previousPassword;
                    await _dialogService.ShowError(Resources.PasswordUpdateFailed, Resources.PasswordUpdateTitle, Resources.WebErrorButtonText, null);
                }

                IsUpdating = false;
                return true;
            }
            catch (WebException)
            {
                App.CurrentUserEnvironment.User.Password = previousPassword;
                await _dialogService.ShowError(Resources.PasswordUpdateFailed
                    , Resources.PasswordUpdateTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException)
            {
                App.CurrentUserEnvironment.User.Password = previousPassword;
                await _dialogService.ShowError(Resources.PasswordUpdateFailed
                    , Resources.PasswordUpdateTitle
                    , Resources.WebErrorButtonText, null);
            }

            App.Locator.Parameter.OldPassword = null;
            App.Locator.Parameter.NewPassword = null;
            App.Locator.Parameter.NewPasswordReenter = null;
            IsUpdating = false;

            return false;
        }

        #endregion

        #endregion

        #region ===== Events ======================================================================

        public event EventHandler OnUserChanged;

        #endregion
    }
}