using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class DetailSeekiosViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDataService _dataService = null;
        private ISaveDataService _saveDataService = null;
        private IDispatchOnUIThread _dispatcherService = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Seekios selected when navigating from the Seekios list</summary>
        public SeekiosDTO SeekiosSelected { get; set; }

        /// <summary>Return the version of the seekios firmware</summary>
        public string SeekiosEmbeddedVersion
        {
            get
            {
                switch (SeekiosSelected.VersionEmbedded_idversionEmbedded)
                {
                    case (int)VersionEmbeddedEnum.V1009:
                        return "1.009";
                    case (int)VersionEmbeddedEnum.V1008:
                        return "1.008";
                    case (int)VersionEmbeddedEnum.V1007:
                        return "1.007";
                    case (int)VersionEmbeddedEnum.V1006:
                        return "1.006";
                    case (int)VersionEmbeddedEnum.V1005:
                        return "1.005";
                    case (int)VersionEmbeddedEnum.V1004:
                        return "1.004";
                    case (int)VersionEmbeddedEnum.V1003:
                        return "1.003";
                    case (int)VersionEmbeddedEnum.V1002:
                        return "1.002";
                    case (int)VersionEmbeddedEnum.V1001:
                        return "1.001";
                    case (int)VersionEmbeddedEnum.v1000:
                        return "1.000";
                    case (int)VersionEmbeddedEnum.V0016:
                        return "1.016";
                    case (int)VersionEmbeddedEnum.V0012:
                        return "0.012";
                    default:
                        return Resources.NoVersionEmbedded;
                }
            }
        }

        /// <summary>True if the Activity needs to have its UI updated (use only in Android)</summary>
        public bool ActivityNeedsUIToBeUpdated { get; set; }

        /// <summary>(use only in Android)</summary>
        public bool IsSeekiosDeleted { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public DetailSeekiosViewModel(IDataService dataService
            , Interfaces.IDialogService dialogService
            , ISaveDataService saveDataService
            , INavigationService navigationService
            , IDispatchOnUIThread dispatcherService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _navigationService = navigationService;
            _saveDataService = saveDataService;
            _dispatcherService = dispatcherService;
            ActivityNeedsUIToBeUpdated = false;
            IsSeekiosDeleted = false;
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region Navigation

        /// <summary>
        /// Navigate to Seekios parameters 
        /// </summary>
        public void GoToParameter(SeekiosDTO seekios)
        {
            App.Locator.AddSeekios.UpdatingSeekios = seekios;
            App.Locator.AddSeekios.SeekiosIMEI = seekios.Imei;
            App.Locator.AddSeekios.SeekiosName = seekios.SeekiosName;
            if (seekios.SeekiosPicture != null)
            {
                App.Locator.AddSeekios.SeekiosImage = Convert.FromBase64String(seekios.SeekiosPicture);
            }
            App.Locator.AddSeekios.IsAdding = false;
            _navigationService.NavigateTo(App.ADD_SEEKIOS_PAGE);
        }

        /// <summary>
        /// Navigate to the alert sos page
        /// </summary>
        public void GoToAlertSOS()
        {
            App.Locator.AlertSOS.CurrentAlertSOS = App.CurrentUserEnvironment.LsAlert.FirstOrDefault(el => el.IdAlert == App.Locator.DetailSeekios.SeekiosSelected.AlertSOS_idalert);
            _navigationService.NavigateTo(App.ALERT_SOS_PAGE);
        }

        /// <summary>
        /// Navigate to alert list
        /// </summary>
        public void GoToSeekiosAlert()
        {
            _navigationService.NavigateTo(App.MODE_ZONE_3_PAGE);
        }

        /// <summary>
        /// Navigate to Map
        /// </summary>
        public void GoToMap(SeekiosDTO seekios)
        {
            MapViewModelBase.Seekios = seekios;
            if (_navigationService != null && _navigationService.CurrentPageKey != App.MAP_PAGE)
            {
                _navigationService.NavigateTo(App.MAP_PAGE);
            }
        }

        /// <summary>
        /// Navigate to reload credit page
        /// </summary>
        public void GoToBuyCredits()
        {
            if (_navigationService.CurrentPageKey != App.RELOAD_CREDIT_PAGE)
            {
                _navigationService.NavigateTo(App.RELOAD_CREDIT_PAGE);
            }
        }

        #endregion

        #region Seekios Logic

        /// <summary>
        /// Delete a seekios
        /// </summary>
        public async Task<int> DeleteSeekios()
        {
            if (SeekiosSelected == null) return -1;
            try
            {
                int res = await _dataService.DeleteSeekios(SeekiosSelected.Idseekios);
                if (res == 1)
                {
                    var modes = App.CurrentUserEnvironment.LsMode.Where(x => x.Seekios_idseekios == SeekiosSelected.Idseekios);
                    if (modes?.Count() > 0)
                    {
                        foreach (var mode in modes)
                        {
                            var alerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == mode.Idmode);
                            if (alerts?.Count() > 0)
                            {
                                foreach (var alert in alerts)
                                {
                                    // Remove recipients
                                    App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == alert.IdAlert);
                                }
                            }
                            // Remove alerts
                            App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdMode == mode.Idmode);
                        }
                        // Remove modes
                        App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Seekios_idseekios == SeekiosSelected.Idseekios);
                    }
                    // Remove locations
                    App.CurrentUserEnvironment.LsLocations.RemoveAll(x => x.Seekios_idseekios == SeekiosSelected.Idseekios);
                    // Remove seekios
                    App.CurrentUserEnvironment.LsSeekios.Remove(SeekiosSelected);
                    App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                    await _dialogService.ShowMessage(Resources.DeleteSeekiosSuccess, Resources.DeleteSeekiosTitle);
                    return 1;
                }
                else if (res == -1)
                {
                    await _dialogService.ShowMessage(Resources.MySeekios_DeleteSeekios_Title, Resources.UnexpectedErrorTitle);
                    return -1;
                }
                return -1;
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

        /// <summary>
        /// Delete seekios
        /// </summary>
        public void DeleteSeekiosSignalR(string uidDevice, string idseekiosStr)
        {
            if (string.IsNullOrEmpty(idseekiosStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idseekios = 0;
            if (!int.TryParse(idseekiosStr, out idseekios)) return;
            if (!App.CurrentUserEnvironment.LsSeekios.Any(x => x.Idseekios == idseekios)) return;

            var modes = App.CurrentUserEnvironment.LsMode.Where(x => x.Seekios_idseekios == idseekios);
            if (modes?.Count() > 0)
            {
                foreach (var mode in modes)
                {
                    var alerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == mode.Idmode);
                    if (alerts?.Count() > 0)
                    {
                        foreach (var alert in alerts)
                        {
                            // Remove recipients
                            App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == alert.IdAlert);
                        }
                    }
                    // Remove alerts
                    App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdMode == mode.Idmode);
                }
                // Remove modes
                App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Seekios_idseekios == idseekios);
            }
            // Remove locations
            App.CurrentUserEnvironment.LsLocations.RemoveAll(x => x.Seekios_idseekios == idseekios);
            // Remove seekios
            App.CurrentUserEnvironment.LsSeekios.RemoveAll(x => x.Idseekios == idseekios);
            App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
            App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
            if (_dispatcherService != null)
            {
                _dispatcherService.Invoke(() =>
                {
                    if (_navigationService.CurrentPageKey != "-- ROOT --")
                    {
                        // Navigate to the list seekios
                        _navigationService.NavigateTo(App.LIST_SEEKIOS_PAGE);
                    }
                    else App.RaiseSeekiosInformationChangedEverywhere(0);
                });
            }
        }

        /// <summary>
        /// Request battery level
        /// </summary>
        public async Task<bool> RequestBatteryLevel()
        {
            try
            {
                // If user has not enough credits, cancel the request
                int creditsDispo = 0;
                if (!int.TryParse(Helper.CreditHelper.TotalCredits, out creditsDispo)) return false;
                if (creditsDispo <= 0)
                {
                    await _dialogService.ShowMessage(Resources.UserNoRequestLeft
                        , Resources.UserNoRequestLeftTitle);
                    return false;
                }

                var seekios = App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == SeekiosSelected.Idseekios);
                // If the seekios is in power saving, cancel the request
                if (seekios.IsInPowerSaving == true)
                {
                    await _dialogService.ShowMessage(Resources.PowerSavingOn
                        , Resources.PowerSavingOnTitle
                        , Resources.PowerSavingTuto
                        , Resources.Close, (result) => { if (result) App.Locator.Parameter.GoToTutorialPowerSaving(); });
                    return false;
                }

                // Request on the webservice to update the battery level
                _dialogService.ShowLoadingLayout();
                if (await _dataService.RefreshSeekiosBatteryLevel(SeekiosSelected.Idseekios) != -1)
                {
                    seekios.HasGetLastInstruction = false;
                    seekios.IsRefreshingBattery = true;
                    App.Locator.DetailSeekios.SeekiosSelected = seekios;
                    _dialogService.HideLoadingLayout();
                    return true;
                }
                _dialogService.HideLoadingLayout();
                return false;
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

        /// <summary>
        /// 
        /// </summary>
        public void RequestBatteryLevelSignalR(string uidDevice, string idSeekiosStr)
        {
            if (string.IsNullOrEmpty(idSeekiosStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idSeekios = 0;
            if (int.TryParse(idSeekiosStr, out idSeekios))
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == idSeekios);
                if (seekios != null)
                {
                    seekios.HasGetLastInstruction = false;
                    seekios.IsRefreshingBattery = true;
                    if (_dispatcherService != null)
                    {
                        _dispatcherService.Invoke(() =>
                        {
                            App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                            App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                            App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Update the state of the alert SOS
        /// </summary>
        public async Task<int> NotifyAlertSOSHasBeenRead(int idSeekios)
        {
            if (idSeekios <= 0) return -1;
            try
            {
                if (await _dataService.AlertSOSHasBeenRead(idSeekios.ToString()) == 1)
                {
                    App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == idSeekios).IsLastSOSRead = true;
                    return 1;
                }
                else return -1;
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

        /// <summary>
        /// Update the state of the alert SOS
        /// </summary>
        public void NotifyAlertSOSHasBeenReadSignalR(string uidDevice, string idseekiosStr)
        {
            if (string.IsNullOrEmpty(idseekiosStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idseekios = 0;
            if (int.TryParse(idseekiosStr, out idseekios))
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == idseekios);
                if (seekios != null)
                {
                    seekios.IsLastSOSRead = true;
                    if (_dispatcherService != null)
                    {
                        _dispatcherService.Invoke(() =>
                        {
                            App.RaiseSeekiosInformationChangedEverywhere(idseekios);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// State if an alert has to be shown and read
        /// </summary>
        public bool AlertNeedsToBeRead()
        {
            if (SeekiosSelected != null)
            {
                return !SeekiosSelected.IsLastSOSRead;
            }
            return false;
        }

        #endregion

        #endregion
    }
}