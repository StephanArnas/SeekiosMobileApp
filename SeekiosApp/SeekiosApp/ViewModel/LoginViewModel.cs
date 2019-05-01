using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using Newtonsoft.Json;
using SeekiosApp.Helper;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using SeekiosApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        #region ===== Constructor =================================================================

        public LoginViewModel(IDataService dataService /*IDialogService dialogService,*/ /*ISaveDataService saveDataService*//*, IInternetConnectionService connectionService *//*,IDontMoveService dontMoveService*/)
        {
            _dataService = dataService;
            _dialogService = ServiceLocator.Current.GetInstance<Interfaces.IDialogService>();
            _saveDataService = ServiceLocator.Current.GetInstance<ISaveDataService>();
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            GetSaveData();
        }

        #endregion

        #region ===== Attributs ===================================================================

        private INavigationService _navigationService;
        private Interfaces.IDialogService _dialogService;
        private IDataService _dataService;
        private ISaveDataService _saveDataService;
        private string _email;
        private string _password;
        //private string _fbUser;
        private bool _isLoading = false;

        /// ***** Create account

        private string _userFirstName;
        private string _userLastName;
        private string _userEmail;
        private string _userPassword;
        private string _userConfirmedPassword;
        private string _versionApp;

        /// ***** Password forgotten

        private string _forgetPasswordEmail;
        private string _backgroundImage = string.Empty;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>User email</summary>
        public string Email
        {
            get { return _email; }
            set
            {
                Set(() => this.Email, ref _email, value);
                RaisePropertyChanged("CanConnect");
            }
        }

        /// <summary>User password</summary>
        public string Password
        {
            get { return _password; }
            set
            {
                Set(() => this.Password, ref _password, value);
                RaisePropertyChanged("CanConnect");
            }
        }

        /// <summary>User first name entered at account creation</summary>
        public string UserFirstName
        {
            get { return _userFirstName; }
            set
            {
                Set(() => this.UserFirstName, ref _userFirstName, value);
                RaisePropertyChanged("CanCreateAccount");
            }
        }

        /// <summary>User last name entered at account creation</summary>
        public string UserLastName
        {
            get { return _userLastName; }
            set
            {
                Set(() => this.UserLastName, ref _userLastName, value);
                RaisePropertyChanged("CanCreateAccount");
            }
        }

        /// <summary>User email entered at accoutn creation</summary>
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                Set(() => this.UserEmail, ref _userEmail, value);
                RaisePropertyChanged("CanCreateAccount");
            }
        }

        /// <summary>User password entered at account creation</summary>
        public string UserPassword
        {
            get { return _userPassword; }
            set
            {
                Set(() => this.UserPassword, ref _userPassword, value);
                RaisePropertyChanged("CanCreateAccount");
            }
        }

        /// <summary>Confirmed password</summary>
        public string UserConfirmedPassword
        {
            get { return _userConfirmedPassword; }
            set
            {
                Set(() => this.UserConfirmedPassword, ref _userConfirmedPassword, value);
                RaisePropertyChanged("CanCreateAccount");
            }
        }

        /// <summary>User email used to get his password back</summary>
        public string ForgetPasswordEmail
        {
            get { return _forgetPasswordEmail; }
            set
            {
                Set(() => this.ForgetPasswordEmail, ref _forgetPasswordEmail, value);
                RaisePropertyChanged("CanForgetPassword");
            }
        }

        /// <summary>True if can connect</summary>
        public bool CanConnect
        {
            get { return !string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password) && !IsLoading; }
        }

        /// <summary>True if the user can create an account</summary>
        public bool CanCreateAccount
        {
            get
            {
                return !string.IsNullOrEmpty(UserFirstName) &&
                  !string.IsNullOrEmpty(UserLastName) &&
                  !string.IsNullOrEmpty(UserPassword) &&
                  !string.IsNullOrEmpty(UserEmail) &&
                  !string.IsNullOrEmpty(UserConfirmedPassword) &&
                  !IsLoading;
            }
        }

        /// <summary>True if the user can get a new password by email</summary>
        public bool CanForgetPassword
        {
            get
            {
                return !string.IsNullOrEmpty(ForgetPasswordEmail);
            }
        }

        /// <summary>True if the app is loading</summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                Set(() => this.IsLoading, ref _isLoading, value);
                RaisePropertyChanged("CanConnect");
                RaisePropertyChanged("CanCreateAccount");
                RaisePropertyChanged("CanForgetPassword");
            }
        }

        /// <summary>Version of the application</summary>
        public string VersionApplication
        {
            get { return _versionApp; }
            set { _versionApp = value; }
        }

        public ITimer AppTimer { get; set; }

        public bool IsDeconnected { get; set; }

        #endregion

        #region ===== Public Methods ==============================================================

        #region Handle Connect / Disconnect / Create account / Forget pwd Methods

        public async Task<bool> Connect(string deviceModel
            , string platform
            , string version
            , string uniqueDeviceId
            , string countryCode)
        {
            App.Locator.ListSeekios.IsNotFromLogin = false;
            if (string.IsNullOrEmpty(Email)
                || string.IsNullOrEmpty(Password)
                || string.IsNullOrEmpty(deviceModel)
                || string.IsNullOrEmpty(version)
                || string.IsNullOrEmpty(uniqueDeviceId))
            {
                // TODO : handle custom alert msg
                return false;
            }
            var connectOk = false;
            IsLoading = true;
            try
            {
                var passHash = Password;
                passHash = CryptographyHelper.CalculatePasswordMD5Hash(Email, Password);
                DataService.Email = Email;
                DataService.Pass = passHash;
                App.CurrentUserEnvironment = await _dataService.GetUserEnvironment(App.Locator.Login.VersionApplication,
                    platform,
                    deviceModel,
                    version,
                    uniqueDeviceId,
                    countryCode);
                // The app need an update
                if (App.CurrentUserEnvironment == null)
                {
                    RemoveSavedCredentials();
                    App.IsAppNeedUpdate = true;
                    _navigationService.NavigateTo(App.NEED_UPDATE_PAGE);
                    return false;
                }
                // Authentication failed
                else if (App.CurrentUserEnvironment.User == null)
                {
                    return false;
                }
                // Authentication succeeded
                else
                {
                    InitTimers();
                    connectOk = true;
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
            finally
            {
                IsLoading = false;
            }
            // save data and navigate to seekios list if the user is correctly authenticated
            if (connectOk)
            {
                SaveCurrentCredentials();
                if (GetSavedFirstLaunchTuto())
                {
                    _navigationService.NavigateTo(App.LIST_SEEKIOS_PAGE);
                }
                else
                {
                    _navigationService.NavigateTo(App.TUTORIAL_BACKGROUND_FIRST_LAUNCH_PAGE);
                }
            }
            Password = string.Empty;
            AppTimer = ServiceLocator.Current.GetInstance<ITimer>();
            return connectOk;
        }

        public async Task<bool> AutoConnect(string deviceModel
            , string platform
            , string version
            , string uniqueDeviceId
            , string countryCode)
        {
            // get connection information to auto connect 
            LocalCredentials credentials = null;

            credentials = GetSavedCredentials();
            if (credentials != null)
            {
                DataService.Email = credentials.Email;
                DataService.Pass = credentials.Password;
                DataService.GeneratedToken = credentials.Token;
                try
                {
                    App.CurrentUserEnvironment = await _dataService.GetUserEnvironment(App.Locator.Login.VersionApplication,
                        platform,
                        deviceModel,
                        version,
                        uniqueDeviceId,
                        countryCode);
                }
                catch (TimeoutException)
                {
                    await _dialogService.ShowError(
                        Resources.TimeoutError
                        , Resources.TimeoutErrorTitle
                        , Resources.Close, null);
                    IsLoading = false;
                    return false;
                }
                catch (WebException)
                {
                    await _dialogService.ShowError(
                        Resources.TimeoutError
                        , Resources.TimeoutErrorTitle
                        , Resources.Close, null);
                    IsLoading = false;
                    return false;
                }
                catch (Exception)
                {
                    await _dialogService.ShowError(
                        Resources.UnexpectedError
                        , Resources.UnexpectedErrorTitle
                        , Resources.Close, null);
                    IsLoading = false;
                    return false;
                }
                if (App.CurrentUserEnvironment == null)
                {
                    App.IsAppNeedUpdate = true;
                    _navigationService.NavigateTo(App.NEED_UPDATE_PAGE);
                    return false;
                }
                else if (App.CurrentUserEnvironment.User == null)
                {
                    return false;
                }
                else
                {
                    InitTimers();
                    // AppTimer is used for blocking notifications when the app is in background
                    // It needs to be initialized when autoconnect and connect methods are called
                    AppTimer = ServiceLocator.Current.GetInstance<ITimer>();
                    _navigationService.NavigateTo(App.LIST_SEEKIOS_PAGE);
                    return true;
                }
            }
            else
            {
                return false;
                //else if (!string.IsNullOrEmpty(userEnvironment.User.SocialNetworkUserId))
                //{
                //    var facebookEmail = userEnvironment.User.Email;
                //    var facebookFirstName = userEnvironment.User.FirstName;
                //    var facebookLastName = userEnvironment.User.LastName;
                //    var facebookId = userEnvironment.User.SocialNetworkUserId;
                //    var facebookPicture = userEnvironment.User.UserPicture;
                //    connectOk = await ConnectWithFacebook(facebookEmail, facebookFirstName, facebookLastName, facebookId, facebookPicture, deviceModel, platform, version, uniqueDeviceId, countryCode, true);
                //}
            }
        }

        public async Task<int> Disconnect(string uidDevice)
        {
            try
            {
                if (!Plugin.Connectivity.CrossConnectivity.Current.IsConnected)
                {
                    await _dialogService.ShowMessage(Resources.NoInternetDeconnection
                        , string.Empty
                        , Resources.NoInternetDeconnectionButton
                        , null);
                    return -1;
                }
                int res = await _dataService.UnregisterDeviceForNotification(uidDevice);
                if (res != 1)
                {
                    await _dialogService.ShowMessage(Resources.NoWebserviceDeconnection
                        , string.Empty
                        , Resources.NoWebserviceDeconnectionButton
                        , null);
                    return -1;
                }
                App.Locator.Login.IsDeconnected = true; // bypass the autoconnection in LoginView
                RemoveSavedCredentials();
                _dataService.LogOut();
                return 1;
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

        public async Task<bool> CreateAccount(string firstName
            , string lastName
            , string deviceModel
            , string platform
            , string version
            , string uniqueDeviceId
            , string countryCode)
        {
            if (string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)
               || string.IsNullOrEmpty(UserPassword) || string.IsNullOrEmpty(deviceModel) || string.IsNullOrEmpty(version)
               || string.IsNullOrEmpty(uniqueDeviceId))
            {
                return false;
            }

            IsLoading = true;
            var passHash = CryptographyHelper.CalculatePasswordMD5Hash(UserEmail, UserPassword);

            try
            {
                App.Locator.ListSeekios.IsNotFromLogin = false;

                // TODO : plus d'infos à récupérer depuis Facebook
                var user = new UserDTO()
                {
                    Email = UserEmail,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = passHash,
                    IdCountryResource = ParserHelper.ParseCountryCode(countryCode)
                };

                int result = await _dataService.InsertUser(user);
                if (result == 1)
                    user.IdUser = result;
                else
                {
                    IsLoading = false;
                    return false;
                }

                await _dialogService.ShowMessage(Resources.CreateAccountSuccessText
                    , Resources.CreateAccountSuccessTitle);
                IsLoading = false;
                Email = UserEmail;
                Password = UserPassword;
                return await Connect(deviceModel, platform, version, uniqueDeviceId, countryCode);
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

        public async Task ForgetPassword(string email)
        {
            try
            {
                IsLoading = true;
                int result = await _dataService.AskForNewPassword(email);

                if (result == -10)
                {
                    await _dialogService.ShowMessage(Resources.EmailSyntaxErrorContent, Resources.EmailErrorTitle);
                }
                if (result == -11)
                {
                    await _dialogService.ShowMessage(Resources.UnexpectedError, Resources.UnexpectedErrorTitle);
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
            finally
            {
                IsLoading = false;
            }
        }

        //public async Task<bool> ConnectWithFacebook(string email
        //    , string firstname
        //    , string lastname
        //    , string fbUserId
        //    , string userPicture
        //    , string deviceModel
        //    , string platform
        //    , string version
        //    , string udeviceid
        //    , string countryCode
        //    , bool isAutoConnexion = false)
        //{
        //    bool connectOk = false;

        //    if (!string.IsNullOrEmpty(email)
        //        && !string.IsNullOrEmpty(firstname)
        //        && !string.IsNullOrEmpty(lastname)
        //        && userPicture != null
        //        && !string.IsNullOrEmpty(fbUserId) || string.IsNullOrEmpty(deviceModel) || string.IsNullOrEmpty(version) || string.IsNullOrEmpty(udeviceid))
        //    {
        //        try
        //        {
        //            var socialNetworkType = (int)SocialNetworkTypes.Facebook;
        //            var userExistState = await _dataService.CheckIfUserExists(email, socialNetworkType.ToString());

        //            // If an error occured
        //            if (userExistState < 0)
        //            {
        //                var msg = Resources.AuthenticationFailed;
        //                var title = Resources.AuthenticationFailedTitle;
        //                await _dialogService.ShowMessage(msg, title);
        //                return false;
        //            }

        //            // If an account already exists on another social network
        //            if (userExistState > 1)
        //            {
        //                var msg = Resources.AccountAlreadyExistsText;
        //                var title = Resources.AccountAlreadyExistsTitle;
        //                await _dialogService.ShowMessage(msg, title);
        //                return false;
        //            }

        //            try
        //            {
        //                // If the user's account doesn't exist
        //                if (userExistState == 0)
        //                {
        //                    UserDTO user = new UserDTO();
        //                    user.Email = email;
        //                    user.FirstName = firstname;
        //                    user.LastName = lastname;
        //                    user.DateLocation = DateTime.Now;
        //                    user.UserPicture = userPicture;
        //                    user.SocialNetworkType = socialNetworkType;
        //                    user.SocialNetworkUserId = fbUserId;

        //                    await _dataService.InsertUser(user);
        //                }

        //                DataService.Email = email;
        //                DataService.Pass = fbUserId;
        //                // At this point, the user is necessarily create
        //                var userEnvironment = await _dataService.GetUserEnvironmentForFacebook(email, fbUserId, deviceModel, platform, version, udeviceid, "noIpNeeded", "noTokenNeeded", countryCode);
        //                //If authentication failed
        //                if (userEnvironment == null)
        //                {
        //                    //Delete saved information as it might be incorrect
        //                    RemoveSavedUserEnvironment();
        //                    await _dialogService.ShowError(Resources.FacebookConnectionFailedButtonText, Resources.FacebookConnectionFailedTitle, Resources.FacebookConnectionFailedButtonText, null);

        //                    return false;
        //                }
        //                connectOk = true;

        //                App.ActualTheme = userEnvironment.User.DefaultTheme;
        //                //else we connect the user using the information given
        //                App.CurrentUserEnvironment = userEnvironment;
        //                App.CurrentUserEnvironment.LsModeDefinition = _dataService.GetModeDefinition();
        //            }
        //            catch (WebException)
        //            {
        //                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
        //                return connectOk;
        //            }
        //            catch (TimeoutException e)
        //            {
        //                await _dialogService.ShowError(e, Resources.FacebookConnectionFailedTitle, Resources.FacebookConnectionFailedButtonText, null);
        //                return connectOk;
        //            }
        //            catch (Exception e)
        //            {
        //                await _dialogService.ShowError(e, Resources.UnexpectedErrorTitle, Resources.UnexpectedError, null);
        //                return connectOk;
        //            }
        //        }
        //        catch (TimeoutException e)
        //        {
        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
        //            return connectOk;
        //        }
        //        catch (WebException e)
        //        {
        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
        //            return connectOk;
        //        }
        //        catch (Exception e)
        //        {
        //            await _dialogService.ShowError(e, Resources.UnexpectedErrorTitle, Resources.UnexpectedError, null);
        //            return connectOk;
        //        }
        //    }
        //    else
        //    {
        //        return false;
        //    }

        //    if (connectOk)
        //    {
        //        //Save informations to allow the AutoConnect for the futur
        //        SaveCurrentUserEnvironment();
        //        //TODO : check if there is some seekios in don't move / Reinitialize service if there is old remaining seekios which are note in don't move anymore
        //        //StartDontMoveServiceIfNeeded();
        //        _navigationService.NavigateTo(App.LIST_SEEKIOS_PAGE);
        //    }

        //    return connectOk;
        //}

        #endregion

        #region Handle Navigation Methods

        public void GoToNeedUpdate()
        {
            _navigationService.NavigateTo(App.NEED_UPDATE_PAGE);
        }

        #endregion

        #region Handle Datas Offline Methods

        public void GetSaveData()
        {
            // data from the state of the map (satellite / plan)
            var result = false;
            if (_saveDataService.Contains(App.MapChange))
            {
                bool.TryParse(_saveDataService.GetData(App.MapChange), out result);
                App.Locator.BaseMap.IsInNormalMode = result;
            }
            // data from the state of traking when seekios is out of zone
            if (_saveDataService.Contains(App.TrackingSetting))
            {
                App.Locator.ModeSelection.LsTrackingSetting = JsonConvert.DeserializeObject<List<TrackingSetting>>(_saveDataService.GetData(App.TrackingSetting));
            }
        }

        public LocalCredentials GetSavedCredentials()
        {
            if (!_saveDataService.Contains(App.LocalCredentials)) return null;
            var json = _saveDataService.GetData(App.LocalCredentials);
            return JsonConvert.DeserializeObject<LocalCredentials>(json);
        }

        public void SaveCurrentCredentials()
        {
            if (App.CurrentUserEnvironment == null || App.CurrentUserEnvironment.User == null) return;
            var credentials = new LocalCredentials()
            {
                Token = DataService.GeneratedToken,
                Email = App.CurrentUserEnvironment.User.Email,
                Password = App.CurrentUserEnvironment.User.Password
            };
            var json = JsonConvert.SerializeObject(credentials);
            _saveDataService.SaveData(App.LocalCredentials, json);
        }

        public void RemoveSavedCredentials()
        {
            _saveDataService.RemoveData(App.LocalCredentials);
        }

        public bool GetSavedFirstLaunchTuto()
        {
            if (!_saveDataService.Contains(App.IsFirstLaunchTutorial))
            {
                return false;
            }
            var json = _saveDataService.GetData(App.IsFirstLaunchTutorial);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public void SaveFirstLaunchTuto()
        {
            var json = JsonConvert.SerializeObject(true);
            _saveDataService.SaveData(App.IsFirstLaunchTutorial, json);
        }

        #endregion

        public string GetRamdomImageName()
        {
            if (string.IsNullOrEmpty(_backgroundImage))
            {
                _backgroundImage = App.LoginBackgrounds[new Random().Next(0, App.LoginBackgrounds.Length)];
            }
            return _backgroundImage;
        }

        public string CalculatePassword(string password)
        {
            return CryptographyHelper.CalculatePasswordMD5Hash(Email, password);
        }

        #endregion

        #region ===== Private Methods =============================================================

        #region Handle Timer for seekios

        private void InitTimers()
        {
            DateTime dateEndRefreshTimer;
            ModeDTO mode = null;
            foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
            {
                // seekios on demand
                if (seekios.DateLastOnDemandRequest.HasValue)
                {
                    dateEndRefreshTimer = seekios.DateLastOnDemandRequest.Value.AddSeconds(App.TIME_FOR_REFRESH_SEEKIOS_IN_SECOND);
                    if (dateEndRefreshTimer > DateTime.Now)
                    {
                        App.Locator.Map.AddSeekiosOnDemand(seekios, dateEndRefreshTimer);
                    }
                }
                // seekios on tracking mode
                mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(x => x.Seekios_idseekios == seekios.Idseekios);
                if (mode != null)
                {
                    if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeTracking)
                    {
                        App.Locator.Map.AddSeekiosOnTracking(seekios, mode);
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeZone
                            && mode.StatusDefinition_idstatusDefinition != (int)Enum.StatutDefinitionEnum.RAS)
                    {
                        App.Locator.Map.AddSeekiosOnTracking(seekios, mode);
                    }
                    else if (mode.ModeDefinition_idmodeDefinition == (int)Enum.ModeDefinitionEnum.ModeDontMove
                            && mode.StatusDefinition_idstatusDefinition != (int)Enum.StatutDefinitionEnum.RAS)
                    {
                        App.Locator.Map.AddSeekiosOnTracking(seekios, mode);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
