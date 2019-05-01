using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeekiosApp.Interfaces
{
    public interface IDataService
    {
        #region ===== UserEnvironment ==========

        Task<UserEnvironmentDTO> GetUserEnvironment(string idapp, string platform, string deviceModel, string version, string uidDevice, string countryCode);

        void LogOut();

        #endregion

        #region ===== User =====================

        Task<List<ShortUserDTO>> GetShortUsersByPendingFriendship(string id);

        Task<int> UpdateUser(UserDTO user);

        Task<int> InsertUser(UserDTO user);

        Task<int> AskForNewPassword(string email);

        #endregion

        #region ===== DeviceForNotification ====

        Task<int> RegisterDeviceForNotification(string deviceModel, string platform, string version, string uidDevice, string countryCode);

        Task<int> UnregisterDeviceForNotification(string uidDevice);

        #endregion

        #region ===== Seekios ==================

        Task<List<SeekiosDTO>> GetSeekios();

        Task<SeekiosDTO> InsertSeekios(SeekiosDTO seekios);

        Task<int> UpdateSeekios(SeekiosDTO seekios);

        Task<int> RefreshSeekiosLocation(int idSeekios);

        Task<int> DeleteSeekios(int id);

        Task<int> RefreshSeekiosBatteryLevel(int idSeekios);

        #endregion

        #region ===== Location =================

        Task<List<LocationDTO>> Locations(int id, DateTime lowerDate, DateTime upperDate);

        Task<LocationUpperLowerDates> LowerDateAndUpperDate(int id);

        Task<List<LocationDTO>> LocationsByMode(int id);

        #endregion

        #region ===== Friend ===================

        /// <summary>
        /// Delete a friendship
        /// </summary>
        /// <param name="id">Seekios id</param>
        /// <returns>1 if it worked, -1 otherwise</returns>
        Task<int> DeleteFriendship(int idi, int idf);

        /// <summary>
        /// Add a friendship which is pending between two users
        /// </summary>
        /// <param name="friendship">friendship to create</param>
        /// <returns></returns>
        Task<int> InsertFriendship(FriendshipDTO friendship);

        /// <summary>
        /// Get all pending friend requests related to a user
        /// </summary>
        /// <param name="idUser">user id</param>
        /// <returns>pending friendship list</returns>
        Task<List<FriendshipDTO>> GetPendingFriendshipByUser(int idUser);

        /// <summary>
        /// Accept a friendship : pending state changing from true to false
        /// </summary>
        /// <param name="friendship">the friendship to update</param>
        /// <returns></returns>
        Task<int> AcceptFriendship(FriendshipDTO friendship);

        #endregion

        #region ===== Sharing ==================

        /// <summary>
        /// Delete a sharing
        /// </summary>
        /// <param name="idu">user id</param>
        /// <param name="idf">friend id</param>
        /// <param name="idseekios">seekios id</param>
        /// <returns>-1 if failed</returns>
        Task<int> DeleteSharing(int idu, int idf, int idseekios);

        /// <summary>
        /// Add a sharing between two users
        /// </summary>
        /// <param name="sharing">sharing to add</param>
        /// <returns></returns>
        Task<int> InsertSharing(SharingDTO sharing);

        /// <summary>
        /// Add multiple seekios and sharing in database
        /// </summary>
        /// <param name="seekiosSharing"></param>
        /// <returns></returns>
        Task<int> InsertMultipleSharing(List<SharingDTO> seekiosSharing);

        /// <summary>
        /// Update a sharing
        /// </summary>
        /// <param name="sharing">sharing to update</param>
        /// <returns></returns>
        Task<int> UpdateSharing(SharingDTO sharing);

        /// <summary>
        /// get sharings by user
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns>sharing list as a Task</returns>
        Task<List<SharingDTO>> GetSharingByUser(int id);

        #endregion

        #region ===== Mode =====================

        Task<int> InsertModeTracking(ModeDTO modeToAdd);

        Task<int> InsertModeZone(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes);

        Task<int> InsertModeDontMove(ModeDTO modeToAdd, List<AlertWithRecipientDTO> alertes);

        Task<int> DeleteMode(string id);

        #endregion

        #region ===== Alert ====================

        Task<List<AlertDTO>> AlertsByMode(ModeDTO mode);

        Task<int> AlertSOSHasBeenRead(string idSeekios);

        Task<int> InsertAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert);

        Task<int> UpdateAlertSOSWithRecipient(int idSeekios, AlertWithRecipientDTO alert);

        #endregion

        #region ===== AlertFavorite ============

        Task<int> InsertAlertFavorite(AlertFavoriteDTO alertFavorite);

        Task<int> DeleteAlertFavorite(int id);

        #endregion

        #region ===== FavoriteArea =============

        Task<List<FavoriteAreaDTO>> GetFavoritesAreaByUser(int id);

        Task<int> InsertFavoriteArea(FavoriteAreaDTO area);

        Task<int> UpdateFavoriteArea(FavoriteAreaDTO area);

        Task<int> DeleteFavoriteArea(int id);

        #endregion

        #region ===== Credit ===================

        Task<List<PackCreditDTO>> CreditPacksByLanguage(string language);

        Task<List<OperationDTO>> OperationHistoric();

        //Task<List<OperationFromStore>> OperationFromStoreHistoric(string idUserStr);

        Task<int> InsertInAppPurchase(PurchaseDTO purchase);

        Task<string> GetDatForInAppBilling(string versionApp);

        #endregion

        #region ===== Mode Notifications =======

        Task<int> UpdateNotificationSetting(SeekiosDTO seekios);

        #endregion
    }
}
