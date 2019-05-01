using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Extension;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using SeekiosApp.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using SeekiosApp.Enum;
using Newtonsoft.Json;

namespace SeekiosApp.ViewModel
{
    public class AddSeekiosViewModel : ViewModelBase, IDisposable
    {
        #region ===== Attributs ===================================================================

        private Interfaces.IDialogService _dialogService = null;
        private IDataService _dataService = null;
        private IDispatchOnUIThread _dispatcherService = null;
        private INavigationService _navigationService = null;

        #endregion

        #region ===== Constructor =================================================================

        public AddSeekiosViewModel(INavigationService navigationService, IDataService dataService, Interfaces.IDialogService dialogService, IDispatchOnUIThread dispatcherService)
        {
            _navigationService = navigationService;
            _dataService = dataService;
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
        }

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Seekios to be updated</summary>
        public SeekiosDTO UpdatingSeekios { get; set; }

        /// <summary>Seekios name</summary>
        public string SeekiosName { get; set; }

        /// <summary>Seekios IMEI</summary>
        public string SeekiosIMEI { get; set; }

        /// <summary>Seekios PIN</summary>
        public string SeekiosPIN { get; set; }

        /// <summary>Seekios picture</summary>
        public byte[] SeekiosImage { get; set; }

        /// <summary>True : we add a seekios / False : we modify a seekios</summary>
        public bool IsAdding { get; set; }

        /// <summary>When a seekios is deleted, it's allowing to skipe the DetailView (only for iOS)</summary>
        public bool IsGoingBack { get; set; }

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Add a seekios 
        /// </summary>
        private async Task<bool> InsertSeekios(string seekiosName, string imei, string pin, byte[] seekiosPicture)
        {
            try
            {
                _dialogService.ShowLoadingLayout();
                var seekiosToAdd = new SeekiosDTO();
                seekiosToAdd.Imei = imei;
                seekiosToAdd.SeekiosName = seekiosName.ToUpperCaseFirst();
                seekiosToAdd.PinCode = pin;
                seekiosToAdd.SeekiosPicture = seekiosPicture == null ? null : Convert.ToBase64String(seekiosPicture);
                // Add seekios in the database
                seekiosToAdd = await _dataService.InsertSeekios(seekiosToAdd);
                if (seekiosToAdd == null || seekiosToAdd.Idseekios <= 0)
                {
                    _dialogService.HideLoadingLayout();
                    return false;
                }
                // Add seekios in local data
                App.CurrentUserEnvironment.LsSeekios.Add(seekiosToAdd);
                _dialogService.HideLoadingLayout();
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
            _dialogService.HideLoadingLayout();
            return false;
        }

        /// <summary>
        /// Update a seekios
        /// </summary>
        private async Task<bool> UpdateSeekios(string name)
        {
            try
            {
                if (UpdatingSeekios == null || UpdatingSeekios.Idseekios <= 0) return false;
                _dialogService.ShowLoadingLayout();
                UpdatingSeekios.SeekiosName = name.ToUpperCaseFirst();
                UpdatingSeekios.SeekiosPicture = SeekiosImage == null ? string.Empty : Convert.ToBase64String(SeekiosImage);
                UpdatingSeekios.User_iduser = App.CurrentUserEnvironment.User.IdUser;

                var index = App.CurrentUserEnvironment.LsSeekios.FindIndex(el => el.Idseekios == UpdatingSeekios.Idseekios);
                if (index < 0 || index >= App.CurrentUserEnvironment.LsSeekios.Count)
                {
                    _dialogService.HideLoadingLayout();
                    return false; // If not found, index equals -1. should not happen but it does according to hockeyapp
                }

                App.CurrentUserEnvironment.LsSeekios[index] = UpdatingSeekios;
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                // Update the seekios in database
                var result = await _dataService.UpdateSeekios(UpdatingSeekios);
                _dialogService.HideLoadingLayout();
                return result > 0;
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
            return false;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Insert or update a seekios
        /// </summary>
        public async Task<bool> InsertOrUpdateSeekios(string seekiosName
            , string imei
            , string pin
            , byte[] seekiosPicture)
        {
            if (IsAdding)
            {
                return await InsertSeekios(seekiosName, imei, pin, seekiosPicture);
            }
            else
            {
                return await UpdateSeekios(seekiosName);
            }
        }

        /// <summary>
        /// Insert a new seekios
        /// </summary>
        public void InsertSeekiosSignalR(string uidDevice, string seekiosJson)
        {
            if (string.IsNullOrEmpty(seekiosJson)) return;
            if (uidDevice == App.UidDevice) return;
            var seekiosToAdd = JsonConvert.DeserializeObject<SeekiosDTO>(seekiosJson);
            if (seekiosToAdd != null)
            {
                App.CurrentUserEnvironment.LsSeekios.Add(seekiosToAdd);
                App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                if (_dispatcherService != null)
                {
                    _dispatcherService.Invoke(() =>
                    {
                        App.RaiseSeekiosInformationChangedEverywhere(seekiosToAdd.Idseekios);
                    });
                }
            }
        }

        /// <summary>
        /// Update seekios
        /// </summary>
        public void UpdateSeekiosSignalR(string uidDevice, string seekiosJson)
        {
            if (string.IsNullOrEmpty(seekiosJson)) return;
            if (uidDevice == App.UidDevice) return;
            var seekiosToUpdate = JsonConvert.DeserializeObject<SeekiosDTO>(seekiosJson);
            if (seekiosToUpdate != null)
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == seekiosToUpdate.Idseekios);
                if (seekios != null)
                {
                    seekios.SeekiosName = seekiosToUpdate.SeekiosName;
                    seekios.SeekiosPicture = seekiosToUpdate.SeekiosPicture;
                    seekios.SendNotificationOnNewTrackingLocation = seekiosToUpdate.SendNotificationOnNewTrackingLocation;
                    seekios.SendNotificationOnNewOutOfZoneLocation = seekiosToUpdate.SendNotificationOnNewOutOfZoneLocation;
                    seekios.SendNotificationOnNewDontMoveLocation = seekiosToUpdate.SendNotificationOnNewDontMoveLocation;
                    App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                    App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                    if (_dispatcherService != null)
                    {
                        _dispatcherService.Invoke(() =>
                        {
                            App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Update the notification setting
        /// </summary>
        public async void UpdateNotificationSetting(bool notificationTracking
            , bool notificationZone
            , bool notificationDontMove)
        {
            if (UpdatingSeekios == null) return;
            if (notificationTracking != UpdatingSeekios.SendNotificationOnNewTrackingLocation
                || notificationZone != UpdatingSeekios.SendNotificationOnNewOutOfZoneLocation
                || notificationDontMove != UpdatingSeekios.SendNotificationOnNewDontMoveLocation)
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == UpdatingSeekios.Idseekios);
                seekios.SendNotificationOnNewTrackingLocation = notificationTracking;
                seekios.SendNotificationOnNewOutOfZoneLocation = notificationZone;
                seekios.SendNotificationOnNewDontMoveLocation = notificationDontMove;
                if (await _dataService.UpdateNotificationSetting(UpdatingSeekios) != 1)
                {
                    await _dialogService.ShowMessage(Resources.NotificationSettingErrorContent
                         , Resources.NotificationSettingErrorTitle);
                    Dispose();
                }
                else Dispose();
            }
            else Dispose();
        }

        /// <summary>
        /// Initialize properties to default or null 
        /// </summary>
        public void Dispose()
        {
            SeekiosImage = null;
            SeekiosName = string.Empty;
            UpdatingSeekios = null;
            SeekiosIMEI = string.Empty;
            SeekiosPIN = string.Empty;
        }

        #endregion
    }
}
