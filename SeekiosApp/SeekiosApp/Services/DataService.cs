using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ModernHttpClient;
using Newtonsoft.Json;
using SeekiosApp.Extension;
using SeekiosApp.Helper;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.Services
{
    public class DataService : IDataService, IDisposable
    {
        #region ===== Attributs ===================================================================

        public static bool UseHttps = true;
        public static bool UseStaging = false;
        private static string _baseUrl = "http" + (UseHttps ? "s" : "") + "://" + (UseStaging ? "staging" : "production") + ".seekios.com/SeekiosService.svc/";
        private const int _DEFAULT_HTTP_TIMEOUT = 60;
        //private const int _DEFAULT_TOKEN_TIMEOUT = 15;
        private const string _TOKEN_HEADER = "token";
        public static TokenDTO GeneratedToken = null;
        private static Interfaces.IDialogService _dialogService = null;

        //Staging (avant 52.164.245.90)
        //Production (avant seekios.cloudapp.net)
        //only one HttpClient instance for the whole app
        private static HttpClient _httpClient = new HttpClient(new NativeMessageHandler());
        private readonly string _DATE_STRING_FORMAT = "dd-MM-yyyy_HH!mm!ss";

        #region Webservice Methods 

        /// <summary>Get methodes</summary>
        private static string _getUserByCredential = string.Format("{0}GetUserByCredential", _baseUrl);
        private static string _getSubscriptionsByUser = string.Format("{0}GetSubscriptionsByUser", _baseUrl);
        private static string _getSeekiosProductionByImei = string.Format("{0}GetSeekiosProductionByImei", _baseUrl);
        private static string _getSeekiosProductionByUser = string.Format("{0}GetSeekiosProductionByUser", _baseUrl);
        private static string _isIMEIAndPinValid = string.Format("{0}IsIMEIAndPinValid", _baseUrl);
        private static string _getSeekios = string.Format("{0}GetSeekios", _baseUrl);
        private static string _getFriendsByUser = string.Format("{0}GetFriendsByUser", _baseUrl);
        private static string _getPictureByUser = string.Format("{0}GetPictureByUser", _baseUrl);
        private static string _getCurrentMonthLocationsByUser = string.Format("{0}GetCurrentMonthLocationsByUser", _baseUrl);
        private static string _getLastMonthLocationsByUser = string.Format("{0}GetLastMonthLocationsByUser", _baseUrl);
        private static string _getLocations = string.Format("{0}Locations", _baseUrl);
        private static string _getLowerDateAndUpperDate = string.Format("{0}LowerDateAndUpperDate", _baseUrl);
        private static string _locationsByMode = string.Format("{0}LocationsByMode", _baseUrl);
        private static string _getModeBySeekios = string.Format("{0}GetModeBySeekios", _baseUrl);
        private static string _getPackCredit = string.Format("{0}CreditPacksByLanguage", _baseUrl);
        private static string _getUserEnvironment = string.Format("{0}UserEnvironment", _baseUrl);
        private static string _getUserEnvironmentForFacebook = string.Format("{0}GetUserEnvironmentForFacebook", _baseUrl);
        private static string _refreshSeekios = string.Format("{0}RefreshSeekiosLocation", _baseUrl);
        private static string _getFavoritesAreaByUser = string.Format("{0}GetFavoritesAreaByUser", _baseUrl);
        private static string _generateUniqueCodeAndSendMessage = string.Format("{0}GenerateUniqueCodeAndSendMessage", _baseUrl);
        private static string _getSharingByUser = string.Format("{0}GetSharingByUser", _baseUrl);
        private static string _getUserByEmail = string.Format("{0}GetUserByEmail", _baseUrl);
        private static string _getUserByPhone = string.Format("{0}GetUserByPhone", _baseUrl);
        private static string _getUsersByNameSearch = string.Format("{0}GetUsersByNameSearch", _baseUrl);
        private static string _getPendingFriendship = string.Format("{0}GetPendingFriendshipByUser", _baseUrl);
        private static string _getShortUsersByPendingFriendship = string.Format("{0}GetShortUsersByPendingFriendship", _baseUrl);
        private static string _notifyBLEConnexionLost = string.Format("{0}NotifyBLEConnexionLost", _baseUrl);
        private static string _checkIfUserExists = string.Format("{0}CheckIfUserExists", _baseUrl);
        private static string _getAlertsByMode = string.Format("{0}AlertsByMode", _baseUrl);
        private static string _isSeekiosVersionApplicationNeedForceUpdate = string.Format("{0}IsSeekiosVersionApplicationNeedForceUpdate", _baseUrl);
        private static string _requestBatteryLevel = string.Format("{0}RefreshSeekiosBatteryLevel", _baseUrl);
        private static string _getTransactionHistoric = string.Format("{0}OperationHistoric", _baseUrl);
        private static string _getStoreHistoric = string.Format("{0}OperationFromStore", _baseUrl);
        private static string _getDataForInAppBilling = string.Format("{0}GetDataForInAppBilling", _baseUrl);
        private static string _askForNewPassword = string.Format("{0}AskForNewPassword", _baseUrl);
        private static string _getAlerts = string.Format("{0}Alerts", _baseUrl);
        private static string _login = string.Format("{0}Login", _baseUrl);
        private static string _registerDevice = string.Format("{0}RegisterDevice", _baseUrl);
        private static string _unregisterDevice = string.Format("{0}UnregisterDevice", _baseUrl);
        private static string _updateNotificationSetting = string.Format("{0}UpdateNotificationSetting", _baseUrl);
        //private string _getSharingByFriend = string.Format("{0}GetSharingByFriend", _baseUrl);

        /// <summary>POST methods</summary>
        private static string _insertSeekios = string.Format("{0}InsertSeekios", _baseUrl);
        private static string _insertModeTracking = string.Format("{0}InsertModeTracking", _baseUrl);
        private static string _insertModeZone = string.Format("{0}InsertModeZone", _baseUrl);
        private static string _insertModeDontMove = string.Format("{0}InsertModeDontMove", _baseUrl);
        private static string _insertAlert = string.Format("{0}InsertAlert", _baseUrl);
        private static string _insertAlertRecipient = string.Format("{0}InsertAlertRecipient", _baseUrl);
        private static string _insertAlertWithRecipient = string.Format("{0}InsertAlertSOSWithRecipient", _baseUrl);
        private static string _insertAlertFavorite = string.Format("{0}InsertAlertFavorite", _baseUrl);
        private static string _insertFavoriteArea = string.Format("{0}InsertFavoriteArea", _baseUrl);
        private static string _insertFriendship = string.Format("{0}InsertFriendship", _baseUrl);
        private static string _insertSharing = string.Format("{0}InsertSharing", _baseUrl);
        private static string _insertUser = string.Format("{0}InsertUser", _baseUrl);
        private static string _insertMultipleSharing = string.Format("{0}InsertMultipleSharing", _baseUrl);
        private static string _insertInAppPurchase = string.Format("{0}InsertInAppPurchase", _baseUrl);

        /// <summary>PUT methods</summary>
        private static string _updateSeekios = string.Format("{0}UpdateSeekios", _baseUrl);
        private static string _updateUser = string.Format("{0}UpdateUser", _baseUrl);
        private static string _updateAlert = string.Format("{0}UpdateAlert", _baseUrl);
        private static string _updateAlertRecipient = string.Format("{0}UpdateAlertRecipient", _baseUrl);
        private static string _updateAlertWithRecipient = string.Format("{0}UpdateAlertSOSWithRecipient", _baseUrl);
        private static string _updateFavoriteArea = string.Format("{0}UpdateFavoriteArea", _baseUrl);
        private static string _updateFriendshipPendingState = string.Format("{0}UpdateFriendshipPendingState", _baseUrl);
        private static string _updateSharing = string.Format("{0}UpdateSharing", _baseUrl);
        private static string _notifyAlertSOSHasBeenRead = string.Format("{0}AlertSOSHasBeenRead", _baseUrl);

        /// <summary>DELETE methods</summary>
        private static string _deleteSeekios = string.Format("{0}DeleteSeekios", _baseUrl);
        private static string _deleteMode = string.Format("{0}DeleteMode", _baseUrl);
        private static string _deleteAlert = string.Format("{0}DeleteAlert", _baseUrl);
        private static string _deleteAlertRecipient = string.Format("{0}DeleteAlertRecipient", _baseUrl);
        private static string _deleteAlertFavorite = string.Format("{0}DeleteAlertFavorite", _baseUrl);
        private static string _deleteFavoriteArea = string.Format("{0}DeleteFavoriteArea", _baseUrl);
        private static string _deleteFriendship = string.Format("{0}DeleteFriendship", _baseUrl);
        private static string _deleteSharing = string.Format("{0}DeleteSharing", _baseUrl);
        private static string _deleteUser = string.Format("{0}DeleteUser", _baseUrl);
        private static string _sendLogData = string.Format("{0}LogData", _baseUrl);

        #endregion

        #endregion

        #region ===== Properties ==================================================================

        public static string Email { get; set; }
        public static string Pass { get; set; }
        public JsonSerializerSettings JsonSetting
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };
            }
        }

        #endregion

        #region ===== Constructor =================================================================
        public sealed class PreserveAttribute : Attribute
        {
            public bool AllMembers;
            public bool Conditional;
        }

        [Preserve]
        public DataService() : base()
        {
            _httpClient.Timeout = new TimeSpan(0, 0, _DEFAULT_HTTP_TIMEOUT);
            _dialogService = ServiceLocator.Current.GetInstance<Interfaces.IDialogService>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region ===== UserEnvironment ==========

        /// <summary>
        /// Get the whole user environment
        /// </summary>
        public async Task<UserEnvironmentDTO> GetUserEnvironment(string idapp, string platform, string deviceModel, string version, string uidDevice, string countryCode)
        {
            var url = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", _getUserEnvironment, idapp, platform, deviceModel, version, uidDevice, countryCode);
            var json = await GetRequestAsync(url);
            var currentDateTime = DateHelper.GetSystemTime();

            if (string.IsNullOrEmpty(json) || !json.IsJson()) return new UserEnvironmentDTO();
            var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
            if (error != null && !string.IsNullOrEmpty(error.ErrorCode) && (error.ErrorCode == "0x0102" || error.ErrorCode == "0x0103"))
            {
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleUserEnvironmentError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return null;
            }
            else GeneratedToken = null;

            var userEnvironment = new UserEnvironmentDTO();
            userEnvironment = JsonConvert.DeserializeObject<UserEnvironmentDTO>(json);

            if (userEnvironment.LsAlert == null) userEnvironment.LsAlert = new List<AlertDTO>();
            if (userEnvironment.LsAlertFavorites == null) userEnvironment.LsAlertFavorites = new List<AlertFavoriteDTO>();
            if (userEnvironment.LsAlertRecipient == null) userEnvironment.LsAlertRecipient = new List<AlertRecipientDTO>();
            if (userEnvironment.LsFavoriteArea == null) userEnvironment.LsFavoriteArea = new List<FavoriteAreaDTO>();
            if (userEnvironment.LsFriend == null) userEnvironment.LsFriend = new List<FriendUserDTO>();
            if (userEnvironment.LsSharing == null) userEnvironment.LsSharing = new List<SharingDTO>();
            if (userEnvironment.LsMode == null) userEnvironment.LsMode = new List<ModeDTO>();
            if (userEnvironment.LsSeekios == null) userEnvironment.LsSeekios = new List<SeekiosDTO>();
            if (userEnvironment.LsLocations == null) userEnvironment.LsLocations = new List<LocationDTO>();

            userEnvironment.ClientSynchronisationDate = currentDateTime;
            return userEnvironment;
        }

        /// <summary>
        /// Log-out a user
        /// </summary>
        public void LogOut()
        {
            GeneratedToken = null;
            _httpClient.DefaultRequestHeaders.Clear();
        }

        #endregion

        #region ===== User =====================

        /// <summary>
        /// Get the list of users who share a pending friendship with a certain user
        /// </summary>
        public async Task<List<ShortUserDTO>> GetShortUsersByPendingFriendship(string id)
        {
            if (string.IsNullOrEmpty(id)) return null;

            var url = string.Format("{0}/{1}", _getShortUsersByPendingFriendship, id);
            var json = await GetRequestAsync(url);

            if (string.IsNullOrEmpty(json) || !json.IsJson()) return await Task.FromResult(new List<ShortUserDTO>());
            return JsonConvert.DeserializeObject<List<ShortUserDTO>>(json);
        }

        /// <summary>
        /// Update user information
        /// </summary>
        public async Task<int> UpdateUser(UserDTO user)
        {
            int result = 0;
            var json = JsonConvert.SerializeObject(user, JsonSetting);
            var response = await PutRequestAsync(string.Format("{0}/{1}", _updateUser, App.UidDevice), json);

            if (int.TryParse(response, out result)) return result;
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleUpdateUserError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowError(content, title, Resources.Close, null);
                return 0;
            }
        }

        /// <summary>
        /// Add a user in database
        /// </summary>
        public async Task<int> InsertUser(UserDTO user)
        {
            if (user == null) return 0;
            int result;

            var json = JsonConvert.SerializeObject(user, JsonSetting);
            var response = await PostRequestAsync(_insertUser, json, false);

            if (int.TryParse(response, out result)) return result;
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleInsertUserError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowError(content, title, Resources.Close, null);
                return 0;
            }
        }

        /// <summary>
        /// Ask for a new password
        /// </summary>
        /// <param name="email">user email</param>
        /// <returns>
        /// -2 if the email syntax is incorrect
        /// -1 if no user matches the email
        /// 0 if it fails
        /// 1 it it success
        /// </returns>
        public async Task<int> AskForNewPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return -10;
            int result = 0;

            var url = string.Format("{0}/{1}", _askForNewPassword, email);
            var response = await GetRequestAsync(url, false);
            if (!int.TryParse(response, out result)) return -11;

            var title = string.Empty;
            var content = string.Empty;
            ErrorMessageHelper.HandleAskNewPasswordError(ref title, ref content, result);
            await _dialogService.ShowMessage(content, title);

            return result;
        }

        #endregion

        #region ===== DeviceForNotification ====

        /// <summary>
        /// Register device for mobile notifications
        /// </summary>
        public async Task<int> RegisterDeviceForNotification(string deviceModel
            , string platform
            , string version
            , string uidDevice
            , string countryCode)
        {
            if (string.IsNullOrEmpty(deviceModel)
                || string.IsNullOrEmpty(platform)
                || string.IsNullOrEmpty(version)
                || string.IsNullOrEmpty(uidDevice)
                || string.IsNullOrEmpty(countryCode)) return -1;

            var url = string.Format("{0}/{1}/{2}/{3}/{4}/{5}", _registerDevice, deviceModel, platform, version, uidDevice, countryCode);
            var json = await GetRequestAsync(url);
            if (!json.Equals("1")) return -1;

            return 1;
        }

        /// <summary>
        /// Unregister device from mobile notifications
        /// </summary>
        public async Task<int> UnregisterDeviceForNotification(string uidDevice)
        {
            if (string.IsNullOrEmpty(uidDevice)) return -1;

            var url = string.Format("{0}/{1}", _unregisterDevice, uidDevice);
            var json = await GetRequestAsync(url);

            if (!json.Equals("1")) return -1;
            return 1;
        }

        #endregion

        #region ===== Seekios ==================

        /// <summary>
        /// Return the seekios list of the chosen user
        /// </summary>
        public async Task<List<SeekiosDTO>> GetSeekios()
        {
            var json = await GetRequestAsync(string.Format("{0}", _getSeekios));
            if (string.IsNullOrEmpty(json) || !json.IsJson()) return await Task.FromResult(new List<SeekiosDTO>());
            return JsonConvert.DeserializeObject<List<SeekiosDTO>>(json);
        }

        /// <summary>
        /// Add a Seekios : create a Seekios for a user, add it in database
        /// </summary>
        public async Task<SeekiosDTO> InsertSeekios(SeekiosDTO seekiosToAdd)
        {
            var json = JsonConvert.SerializeObject(seekiosToAdd, JsonSetting);
            var response = await PostRequestAsync(string.Format("{0}/{1}", _insertSeekios, App.UidDevice), json);
            if (string.IsNullOrEmpty(response) || !response.IsJson()) return null;

            var seekios = JsonConvert.DeserializeObject<SeekiosDTO>(response);
            if (seekios == null || string.IsNullOrEmpty(seekios.SeekiosName))
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleInsertSeekiosError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return null;
            }
            return seekios;
        }

        /// <summary>
        /// Update the Seekios in database with new information
        /// </summary>
        public async Task<int> UpdateSeekios(SeekiosDTO seekios)
        {
            int countOfUpdates = -1;
            var json = JsonConvert.SerializeObject(seekios, JsonSetting);
            var response = await PutRequestAsync(string.Format("{0}/{1}", _updateSeekios, App.UidDevice), json);

            if (int.TryParse(response, out countOfUpdates))
            {
                return countOfUpdates;
            }
            return -1;
        }

        /// <summary>
        /// Delete a Seekios
        /// </summary>
        public async Task<int> DeleteSeekios(int id)
        {
            int result = 0;

            var url = string.Format("{0}/{1}/{2}", _deleteSeekios, App.UidDevice, id);
            var response = await DeleteRequestAsync(url);

            if (int.TryParse(response, out result)) return result;
            else if (!response.IsJson()) return -1;
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleDeletSeekiosError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
            }
            return 0;
        }

        /// <summary>
        /// Refresh the Seekios informationRafraichit les informations du seekios
        /// </summary>
        public async Task<int> RefreshSeekiosLocation(int idSeekios)
        {
            if (idSeekios <= 0) return 0;
            int result = 0;

            var url = string.Format("{0}/{1}/{2}", _refreshSeekios, App.UidDevice, idSeekios);
            var json = await GetRequestAsync(url);

            if (int.TryParse(json, out result) && result == 1) return 1;
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleRefreshSeekiosLocationError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
            }
            return 0;
        }

        /// <summary>
        /// Request the level of battery
        /// </summary>
        public async Task<int> RefreshSeekiosBatteryLevel(int id)
        {
            int answer = 0;

            var url = string.Format("{0}/{1}/{2}", _requestBatteryLevel, App.UidDevice, id);
            var response = await GetRequestAsync(url);

            if (int.TryParse(response, out answer) && answer == 1) return answer;
            else
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleRefreshSeekiosBatteryError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
            }
            return 0;
        }

        #endregion

        #region ===== Location =================

        /// <summary>
        /// Get seekios locations within start date and end date
        /// </summary>
        public async Task<List<LocationDTO>> Locations(int id, DateTime lowerDate, DateTime upperDate)
        {
            if (id <= 0) return null;

            var json = await GetRequestAsync(string.Format("{0}/{1}/{2}/{3}", _getLocations
                , id
                , lowerDate.ToString(_DATE_STRING_FORMAT)
                , upperDate.ToString(_DATE_STRING_FORMAT)));

            if (string.IsNullOrEmpty(json) || !json.IsJson())
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleLocationsError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowError(content, title, Resources.Close, null);
                return null;
            }

            var locations = JsonConvert.DeserializeObject<List<LocationDTO>>(json);
            // check if the list is correct
            locations.RemoveAll(x => x.Idlocation == 0);
            return locations;
        }

        /// <summary>
        /// Get locations by mode
        /// </summary>
        public async Task<List<LocationDTO>> LocationsByMode(int id)
        {
            var json = await GetRequestAsync(string.Format("{0}/{1}", _locationsByMode, id));
            if (string.IsNullOrEmpty(json) || !json.IsJson()) return null;

            return JsonConvert.DeserializeObject<List<LocationDTO>>(json);
        }

        /// <summary>
        /// Get seekios locations within start date and end date
        /// </summary>
        public async Task<LocationUpperLowerDates> LowerDateAndUpperDate(int id)
        {
            if (id <= 0) return null;

            var json = await GetRequestAsync(string.Format("{0}/{1}", _getLowerDateAndUpperDate, id));
            if (string.IsNullOrEmpty(json) || !json.IsJson() || json.Length <= 36) return await Task.FromResult(new LocationUpperLowerDates());

            return JsonConvert.DeserializeObject<LocationUpperLowerDates>(json);
        }

        #endregion

        #region ===== Friend ===================

        /// <summary>
        /// Delete a friendship
        /// </summary>
        public async Task<int> DeleteFriendship(int idUser, int idFriend)
        {
            if (idUser <= 0 || idFriend <= 0) return -1;
            int result = -1;

            var url = string.Format("{0}/{1}/{2}", _deleteFriendship, idUser, idFriend);
            var response = await DeleteRequestAsync(url);

            if (int.TryParse(response, out result))
            {
                return result;
            }
            return -1;
        }

        /// <summary>
        /// Add a friendship which is pending between two users
        /// </summary>
        public async Task<int> InsertFriendship(FriendshipDTO friendship)
        {
            if (friendship == null) return -1;
            int result;

            var json = JsonConvert.SerializeObject(friendship, JsonSetting);
            var response = await PostRequestAsync(_insertFriendship, json);

            if (int.TryParse(response, out result)) return result;
            return -1;
        }

        /// <summary>
        /// Get all pending friend requests related to a user
        /// </summary>
        public async Task<List<FriendshipDTO>> GetPendingFriendshipByUser(int id)
        {
            if (id <= 0) return null;

            var url = string.Format("{0}/{1}", _getPendingFriendship, id);
            var json = await GetRequestAsync(url);

            if (string.IsNullOrEmpty(json) || !json.IsJson()) return await Task.FromResult(new List<FriendshipDTO>());
            return JsonConvert.DeserializeObject<List<FriendshipDTO>>(json);
        }

        /// <summary>
        /// Accept a friendship : pending state changing from true to false
        /// </summary>
        public async Task<int> AcceptFriendship(FriendshipDTO friendship)
        {
            if (friendship == null) return -1;
            int result = -1;

            var json = JsonConvert.SerializeObject(friendship, JsonSetting);
            var response = await PutRequestAsync(_updateFriendshipPendingState, json);

            if (int.TryParse(response, out result))
            {
                return result;
            }
            return -1;
        }

        #endregion

        #region ===== Sharing ==================

        /// <summary>
        /// Get sharings by user
        /// </summary>
        public async Task<List<SharingDTO>> GetSharingByUser(int id)
        {
            if (id <= 0) return null;

            var url = string.Format("{0}/{1}", _getSharingByUser, id);
            var json = await GetRequestAsync(url);

            if (string.IsNullOrEmpty(json) || !json.IsJson()) return null;
            return JsonConvert.DeserializeObject<List<SharingDTO>>(json);
        }

        /// <summary>
        /// Delete a sharing
        /// </summary>
        public async Task<int> DeleteSharing(int idu, int idf, int idseekios)
        {
            if (idu <= 0 || idf <= 0 || idseekios <= 0) return -1;
            int result = -1;

            var url = string.Format("{0}/{1}/{2}/{3}", _deleteSharing, idu, idf, idseekios);
            var response = await DeleteRequestAsync(url);

            if (int.TryParse(response, out result)) return result;
            else return -1;
        }

        /// <summary>
        /// Add a sharing between two users
        /// </summary>
        public async Task<int> InsertSharing(SharingDTO sharing)
        {
            if (sharing == null) return -1;
            int result = 0;

            var json = JsonConvert.SerializeObject(sharing, JsonSetting);
            json = json.Replace("\\", "").Replace("\\", "");
            var response = await PostRequestAsync(_insertSharing, json);

            if (int.TryParse(response, out result)) return result;
            else return -1;
        }

        /// <summary>
        /// Insert multiple sharing
        /// </summary>
        public async Task<int> InsertMultipleSharing(List<SharingDTO> lsSharing)
        {
            if (lsSharing == null && lsSharing.Count <= 0) return -1;
            int result = 0;

            var json = JsonConvert.SerializeObject(lsSharing, JsonSetting);
            var response = await PostRequestAsync(_insertMultipleSharing, json);

            if (int.TryParse(response, out result)) return result;
            else return -1;
        }

        /// <summary>
        /// Update a sharing
        /// </summary>
        public async Task<int> UpdateSharing(SharingDTO sharing)
        {
            if (sharing == null) return -1;
            int result = 0;

            var json = JsonConvert.SerializeObject(sharing, JsonSetting);
            var response = await PutRequestAsync(_updateSharing, json);

            if (int.TryParse(response, out result)) return result;
            else return -1;
        }

        #endregion

        #region ===== Mode =====================

        /// <summary>
        /// Add the mode of a seekios in database
        /// </summary>
        public async Task<int> InsertModeTracking(ModeDTO modeToAdd)
        {
            int idModeAdded = -1;

            var json = JsonConvert.SerializeObject(modeToAdd, JsonSetting);
            var response = await PostRequestAsync(string.Format("{0}/{1}", _insertModeTracking, App.UidDevice), json);

            if (!int.TryParse(response, out idModeAdded))
            {
                await _dialogService.ShowMessage(Resources.ModeNotConfiguredTitle, Resources.UnexpectedErrorTitle);
                return 0;
            }

            if (idModeAdded <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleInsertModeTrackingError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return 0;
            }

            return idModeAdded;
        }

        /// <summary>
        /// Insert a mode don't move with alert(s) and recipient(s)
        /// </summary>
        public async Task<int> InsertModeDontMove(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes)
        {
            int idModeAdded = 0;

            var jsonMode = JsonConvert.SerializeObject(modeToAdd, JsonSetting);
            var jsonAlert = JsonConvert.SerializeObject(alertes, JsonSetting);
            var json = "{ \"modeToAdd\":" + jsonMode + ",\"alerts\":" + jsonAlert + "}";
            var response = await PostRequestAsync(string.Format("{0}/{1}", _insertModeDontMove, App.UidDevice), json);

            if (!int.TryParse(response, out idModeAdded))
            {
                await _dialogService.ShowMessage(Resources.ModeNotConfiguredTitle, Resources.UnexpectedErrorTitle);
                return 0;
            }

            if (idModeAdded <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleInsertModeDontMoveError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return 0;
            }
            return idModeAdded;
        }

        /// <summary>
        /// Insert a mode zone with alert(s) and recipient(s)
        /// </summary>
        public async Task<int> InsertModeZone(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes)
        {
            int idModeAdded = 0;

            var jsonMode = JsonConvert.SerializeObject(modeToAdd, JsonSetting);
            var jsonAlert = JsonConvert.SerializeObject(alertes, JsonSetting);
            var json = "{ \"modeToAdd\":" + jsonMode + ",\"alerts\":" + jsonAlert + "}";
            var response = await PostRequestAsync(string.Format("{0}/{1}", _insertModeZone, App.UidDevice), json);

            if (!int.TryParse(response, out idModeAdded))
            {
                await _dialogService.ShowMessage(Resources.ModeNotConfiguredTitle, Resources.UnexpectedErrorTitle);
                return 0;
            }

            if (idModeAdded <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleInsertModeZoneError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return 0;
            }
            return idModeAdded;
        }

        /// <summary>
        /// Delete the mode of a seekios in database
        /// </summary>
        public async Task<int> DeleteMode(string id)
        {
            int isSucceed = 0;
            var url = string.Format("{0}/{1}/{2}", _deleteMode, App.UidDevice, id);
            var response = await DeleteRequestAsync(url);

            if (!int.TryParse(response, out isSucceed))
            {
                await _dialogService.ShowMessage(Resources.ModeNotConfiguredTitle, Resources.UnexpectedErrorTitle);
                return 0;
            }

            if (isSucceed <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleDeleteModeError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return 0;
            }
            return isSucceed;
        }

        #endregion

        #region ===== Alert ====================

        /// <summary>
        /// Get alerts by mode
        /// </summary>
        public async Task<List<AlertDTO>> AlertsByMode(ModeDTO mode)
        {
            if (mode.Idmode <= 0) return null;
            var url = string.Format("{0}/{1}", _getAlertsByMode, mode.Idmode);
            var json = await GetRequestAsync(url);
            if (string.IsNullOrEmpty(json) || !json.IsJson()) return null;

            var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
            if (string.IsNullOrEmpty(error.ErrorCode))
            {
                return JsonConvert.DeserializeObject<List<AlertDTO>>(json);
            }
            else
            {
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleAlertByModeError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
            }
            return null;
        }

        /// <summary>
        /// Change state of IsRead in database
        /// </summary>
        public async Task<int> AlertSOSHasBeenRead(string idSeekios)
        {
            if (string.IsNullOrEmpty(idSeekios)) return 0;
            int result = 0;

            var response = await PutRequestAsync(string.Format("{0}/{1}/{2}", _notifyAlertSOSHasBeenRead, App.UidDevice, idSeekios), string.Empty);
            if (!int.TryParse(response, out result) && !response.IsJson())
            {
                await _dialogService.ShowMessage(Resources.UnexpectedErrorTitle, Resources.AlertErrorTitle);
            }

            if (int.TryParse(response, out result) && result != 1)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleAlertSOSHasBeenReadError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return 0;
            }
            return result;
        }

        /// <summary>
        /// Insert an alert SOS
        /// </summary>
        public async Task<int> InsertAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert)
        {
            int idAlert = -1;
            var json = JsonConvert.SerializeObject(alert, JsonSetting);
            var response = await PostRequestAsync(string.Format("{0}/{1}/{2}", _insertAlertWithRecipient, App.UidDevice, idSeekios), json);
            if (int.TryParse(response, out idAlert) && idAlert <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleInsertAlertError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return -1;
            }
            return idAlert;
        }

        /// <summary>
        /// Update an alert SOS
        /// </summary>
        public async Task<int> UpdateAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert)
        {
            int idAlert = -1;
            var json = JsonConvert.SerializeObject(alert, JsonSetting);
            var response = await PutRequestAsync(string.Format("{0}/{1}/{2}", _updateAlertWithRecipient, App.UidDevice, idSeekios), json);
            if (int.TryParse(response, out idAlert) && idAlert <= 0)
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(response);
                var content = string.Empty;
                var title = string.Empty;
                ErrorMessageHelper.HandleUpdateAlertError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowMessage(content, title);
                return -1;
            }
            return idAlert;
        }

        #endregion

        #region ===== AlertFavorite ============

        /// <summary>
        /// Add an alert to favorite
        /// </summary>
        public async Task<int> InsertAlertFavorite(AlertFavoriteDTO alertFavorite)
        {
            int idAlertFavorite = -1;
            var json = JsonConvert.SerializeObject(alertFavorite, JsonSetting);
            var response = await PostRequestAsync(_insertAlertFavorite, json);

            if (int.TryParse(response, out idAlertFavorite))
            {
                return idAlertFavorite;
            }
            return -1;
        }

        /// <summary>
        /// Delete an alert from favorite
        /// </summary>
        public async Task<int> DeleteAlertFavorite(int id)
        {
            int countOfAlertFavoriteDeleted = -1;
            var url = string.Format("{0}/{1}", _deleteAlertFavorite, id);
            var response = await DeleteRequestAsync(url);

            if (int.TryParse(response, out countOfAlertFavoriteDeleted))
            {
                return countOfAlertFavoriteDeleted;
            }
            return -1;
        }

        #endregion

        #region ===== FavoriteArea =============

        /// <summary>
        /// Get a user's favorite areas
        /// </summary>
        public async Task<List<FavoriteAreaDTO>> GetFavoritesAreaByUser(int id)
        {
            if (id <= 0) return null;

            var url = string.Format("{0}/{1}", _getFavoritesAreaByUser, id);
            var json = await GetRequestAsync(url);

            if (string.IsNullOrEmpty(json) || !json.IsJson()) return null;
            return JsonConvert.DeserializeObject<List<FavoriteAreaDTO>>(json);
        }

        /// <summary>
        /// Add an area to favorite
        /// </summary>
        public async Task<int> InsertFavoriteArea(FavoriteAreaDTO area)
        {
            int idAreaAdded = -1;
            var json = JsonConvert.SerializeObject(area, JsonSetting);
            var response = await PostRequestAsync(_insertFavoriteArea, json);

            if (int.TryParse(response, out idAreaAdded))
            {
                return idAreaAdded;
            }
            return -1;
        }

        /// <summary>
        /// Update a favorite area
        /// </summary>
        public async Task<int> UpdateFavoriteArea(FavoriteAreaDTO area)
        {
            int countOfFavoriteAreaUpdated = -1;
            var json = JsonConvert.SerializeObject(area, JsonSetting);
            var response = await PutRequestAsync(_updateFavoriteArea, json);

            if (int.TryParse(response, out countOfFavoriteAreaUpdated))
            {
                return countOfFavoriteAreaUpdated;
            }
            return -1;
        }

        /// <summary>
        /// Delete an area from favorites
        /// </summary>
        public async Task<int> DeleteFavoriteArea(int id)
        {
            int countOfFavoriteAreaDeleted = -1;
            var url = string.Format("{0}/{1}", _deleteFavoriteArea, id);
            var response = await DeleteRequestAsync(url);

            if (int.TryParse(response, out countOfFavoriteAreaDeleted))
            {
                return countOfFavoriteAreaDeleted;
            }
            return -1;
        }

        #endregion

        #region ===== Credit ===================

        /// <summary>
        /// Get the credit packs
        /// </summary>
        public async Task<List<PackCreditDTO>> CreditPacksByLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) return null;
            var url = string.Format("{0}/{1}", _getPackCredit, language);
            var json = await GetRequestAsync(url);

            if (string.IsNullOrEmpty(json) || !json.IsJson())
            {
                var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
                var title = string.Empty;
                var content = string.Empty;
                ErrorMessageHelper.HandleCreditPacksError(ref title, ref content, error.ErrorCode);
                await _dialogService.ShowError(content, title, Resources.Close, null);
                return null;
            }
            return JsonConvert.DeserializeObject<List<PackCreditDTO>>(json);
        }

        /// <summary>
        /// Get all transactions made by the user with the Seekios
        /// </summary>
        public async Task<List<OperationDTO>> OperationHistoric()
        {
            var json = await GetRequestAsync(_getTransactionHistoric);
            if (string.IsNullOrEmpty(json) || !json.IsJson()) return null;
            return JsonConvert.DeserializeObject<List<OperationDTO>>(json);
        }

        /// <summary>
        /// Gets the necessary data for in-app billing (e.g. pubkey google store)
        /// X-Platform, works for Apple Store and Play Store
        /// </summary>
        public async Task<string> GetDatForInAppBilling(string versionApp)
        {
            return await GetRequestAsync(string.Format("{0}/{1}", _getDataForInAppBilling, versionApp));
        }

        /// <summary>
        /// Validates an in-app purchase through our servers.
        /// X-Platform, works for Apple Store and Play Store
        /// </summary>
        public async Task<int> InsertInAppPurchase(PurchaseDTO purchase)
        {
            int isValid = -1;
            var json = JsonConvert.SerializeObject(purchase, JsonSetting);
            var response = await PostRequestAsync(_insertInAppPurchase, json);

            if (int.TryParse(response, out isValid))
            {
                return isValid;
            }
            return -1;
        }

        #endregion

        #region ===== Mode Notifications =======

        /// <summary>
        /// Update the setting for seekios
        /// </summary>
        public async Task<int> UpdateNotificationSetting(SeekiosDTO seekios)
        {
            int isValid = -1;
            var response = await GetRequestAsync(string.Format("{0}/{1}/{2}/{3}/{4}/{5}"
                ,  _updateNotificationSetting
                , seekios.Idseekios
                , App.UidDevice
                , seekios.SendNotificationOnNewTrackingLocation
                , seekios.SendNotificationOnNewOutOfZoneLocation
                , seekios.SendNotificationOnNewDontMoveLocation));

            if (int.TryParse(response, out isValid))
            {
                return isValid;
            }
            return -1;
        }

        #endregion

        #region ===== HANDLE HTTP ==============

        /// <summary>
        /// Make a GET HTTP request
        /// </summary>
        private static async Task<string> GetRequestAsync(string url, bool authenticationNeeded = true)
        {
            //bypass authentication when in test mode
            //authenticationNeeded = !App.IsTestMode;
            var authenticationSuccess = true;

            if (!App.DeviceIsConnectedToInternet) throw new WebException(Resources.NoInternetMessage);

            var result = string.Empty;
            if (authenticationNeeded) authenticationSuccess = await AddAuthenticationHeaders();
            HttpResponseMessage response = null;
            if (authenticationSuccess)
            {
                response = await _httpClient.GetAsync(url);
                result = response.Content.ReadAsStringAsync().Result;
            }
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.Contains("Unauthorized access"))
                {
                    throw new Exception("Unauthorized access");
                }
            }
            return result;
        }

        /// <summary>
        /// Make a POST HTTP request
        /// </summary>
        private static async Task<string> PostRequestAsync(string url, string json, bool authenticationNeeded = true)
        {
            if (!App.DeviceIsConnectedToInternet) throw new WebException(Properties.Resources.WebError);
            var result = string.Empty;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (authenticationNeeded) await AddAuthenticationHeaders();
            var param = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await _httpClient.PostAsync(new Uri(url), param);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.Contains("Unauthorized access"))
                {
                    throw new Exception("Unauthorized access");
                }
            }
            using (HttpContent content = response.Content)
            {
                result = content.ReadAsStringAsync().Result;
            }
            return result;
        }

        /// <summary>
        /// Make a PUT HTTP request
        /// </summary>
        private static async Task<string> PutRequestAsync(string url, string json, bool authenticationNeeded = true)
        {
            if (!App.DeviceIsConnectedToInternet) throw new WebException(Properties.Resources.WebError);
            var result = string.Empty;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (authenticationNeeded) await AddAuthenticationHeaders();
            //client.MaxResponseContentBufferSize = 9999999;
            var param = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;
            response = await _httpClient.PutAsync(new Uri(url), param);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.Contains("Unauthorized access"))
                {
                    throw new Exception("Unauthorized access");
                }
            }
            using (HttpContent content = response.Content)
            {
                result = content.ReadAsStringAsync().Result;
            }
            return result;
        }

        /// <summary>
        /// Make a DELETE HTTP request
        /// </summary>
        private static async Task<string> DeleteRequestAsync(string url, bool authenticationNeeded = true)
        {
            if (!App.DeviceIsConnectedToInternet) throw new WebException(Properties.Resources.WebError);
            string result = string.Empty;

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (authenticationNeeded) await AddAuthenticationHeaders();
            HttpResponseMessage response = null;
            response = await _httpClient.DeleteAsync(new Uri(url));
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                if (result.Contains("Unauthorized access"))
                {
                    throw new Exception("Unauthorized access");
                }
            }
            using (HttpContent content = response.Content)
            {
                result = content.ReadAsStringAsync().Result;
            }
            return result;
        }

        private async static Task<bool> AddAuthenticationHeaders()
        {
            var success = true;
            //We remove all headers
            _httpClient.DefaultRequestHeaders.Clear();
            //We reconnect the user by giving a new token if the session has lasted mroe than 15mins
            if (GeneratedToken == null || (DateTime.UtcNow > GeneratedToken.ExpirationDate))
            {
                success = await Login(Email, Pass);
            }
            if (success)
            {
                if (GeneratedToken != null)
                {
                    _httpClient.DefaultRequestHeaders.Add(_TOKEN_HEADER, GeneratedToken.AuthToken);
                    return true;
                }
                else return false;
            }
            return false;
        }

        public static async Task<bool> Login(string email, string pwd)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd)) return false;
                var url = string.Format("{0}/{1}/{2}", _login, email, pwd);
                var json = await GetRequestAsync(url, false);

                GeneratedToken = JsonConvert.DeserializeObject<TokenDTO>(json);
                if (GeneratedToken?.AuthToken == null)
                {
                    GeneratedToken = null;
                    var error = JsonConvert.DeserializeObject<ErrorDTO>(json);
                    var content = string.Empty;
                    var title = string.Empty;
                    ErrorMessageHelper.HandlerLoginError(ref title, ref content, error.ErrorCode);
                    var saveDataService = GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<ISaveDataService>();
                    saveDataService.RemoveData(App.LocalCredentials);
                    await _dialogService.ShowMessage(content, title);
                    return false;
                }
            }
            catch (Exception e)
            {
                var error = e.InnerException + "\n" + e.Message;
            }
            return true;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        #endregion

        #endregion
    }
}