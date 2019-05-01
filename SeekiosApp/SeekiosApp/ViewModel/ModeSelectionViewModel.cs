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
using System.Threading.Tasks;
using SeekiosApp.Enum;
using SeekiosApp.Helper;
using Newtonsoft.Json;
using SeekiosApp.Model.APP;

namespace SeekiosApp.ViewModel
{
    public class ModeSelectionViewModel : ViewModelBase
    {
        #region ===== Attributs ===================================================================

        protected IDispatchOnUIThread _dispatcherService;
        private INavigationService _navigationService;
        private Interfaces.IDialogService _dialogService;
        private ISaveDataService _saveDataService;
        private IDataService _dataService;


        #endregion

        #region ===== Properties ==================================================================

        public SeekiosDTO SeekiosUpdated { get; set; }

        public List<TrackingSetting> LsTrackingSetting { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ModeSelectionViewModel(IDispatchOnUIThread dispatcherService
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ISaveDataService saveDataService)
        {
            _dispatcherService = dispatcherService;
            _dataService = dataService;
            _dialogService = dialogService;
            _saveDataService = saveDataService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            LsTrackingSetting = new List<TrackingSetting>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Select a mode
        /// </summary>
        public async Task<bool> SelectMode(ModeDefinitionEnum modeDefinition)
        {
            try
            {
                // User is out of requests
                if (int.Parse(CreditHelper.TotalCredits) <= 0)
                {
                    await _dialogService.ShowMessage(Resources.UserNoRequestLeft
                        , Resources.UserNoRequestLeftTitle);
                    return false;
                }

                // User has no internet
                if (!App.DeviceIsConnectedToInternet)
                {
                    await _dialogService.ShowMessage(Resources.NoInternetMessage
                        , Resources.NoInternetTitle);
                    return false;
                }

                // If a mode is already active on the seekios, we display custom popup
                var modeActive = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                if (modeActive != null)
                {
                    if (!await _dialogService.ShowChangeModePopup(Resources.ModeChangeTitle
                        , Resources.ModeChangePowerSaving
                        , modeActive.ModeDefinition_idmodeDefinition
                        , (int)modeDefinition
                        , SeekiosUpdated.IsInPowerSaving)) return false;
                }

                // Display loading layout
                _dialogService.ShowLoadingLayout();

                // Settings to save offline the preferences for mode zone / don't move
                var trackingSetting = App.Locator.ModeSelection.LsTrackingSetting.FirstOrDefault(x => x.IdSeekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios && x.ModeDefinition == modeDefinition);
                if (trackingSetting == null)
                {
                    trackingSetting = new TrackingSetting()
                    {
                        IdSeekios = SeekiosUpdated.Idseekios,
                        ModeDefinition = modeDefinition,
                        RefreshTime = MapViewModelBase.RefreshTime,
                    };
                    App.Locator.ModeSelection.LsTrackingSetting.Add(trackingSetting);
                }
                else trackingSetting.RefreshTime = MapViewModelBase.RefreshTime;

                // Create the new mode to add
                var modeToAddInDb = new ModeDTO
                {
                    Seekios_idseekios = SeekiosUpdated.Idseekios,
                    Trame = string.Empty,
                    Device_iddevice = App.CurrentUserEnvironment.Device != null ? App.CurrentUserEnvironment.Device.Iddevice : 0,
                };

                // Save the mode in database
                int idMode = 0;
                var timeDiffHours = Math.Ceiling((DateTime.Now - DateTime.UtcNow).TotalHours);
                switch (modeDefinition)
                {
                    case ModeDefinitionEnum.ModeDontMove:
                        trackingSetting.IsEnable = App.Locator.ModeDontMove.IsTrackingSettingEnable;
                        trackingSetting.IsPowerSavingEnabled = App.Locator.ModeDontMove.IsPowerSavingEnabled;
                        modeToAddInDb.Trame = string.Format("{0};{1}", timeDiffHours, MapViewModelBase.RefreshTime == 0 ? string.Empty : MapViewModelBase.RefreshTime.ToString());
                        modeToAddInDb.IsPowerSavingEnabled = App.Locator.ModeDontMove.IsPowerSavingEnabled;
                        idMode = await _dataService.InsertModeDontMove(modeToAddInDb, App.Locator.ModeDontMove.LsAlertsModeDontMove);
                        break;
                    case ModeDefinitionEnum.ModeZone:
                        trackingSetting.IsEnable = App.Locator.ModeZone.IsTrackingSettingEnable;
                        trackingSetting.IsPowerSavingEnabled = App.Locator.ModeZone.IsPowerSavingEnabled;
                        modeToAddInDb.Trame = string.Format("{0};{1}", timeDiffHours, App.Locator.ModeZone.CodeTrame(App.Locator.ModeZone.LsAreaCoordsMap));
                        modeToAddInDb.IsPowerSavingEnabled = App.Locator.ModeZone.IsPowerSavingEnabled;
                        idMode = await _dataService.InsertModeZone(modeToAddInDb, App.Locator.ModeZone.LsAlertsModeZone);
                        break;
                    case ModeDefinitionEnum.ModeTracking:
                        modeToAddInDb.Trame = string.Format("{0};{1}", timeDiffHours, MapViewModelBase.RefreshTime == 0 ? string.Empty : MapViewModelBase.RefreshTime.ToString());
                        modeToAddInDb.IsPowerSavingEnabled = App.Locator.ModeTracking.IsPowerSavingEnabled;
                        trackingSetting.IsPowerSavingEnabled = App.Locator.ModeTracking.IsPowerSavingEnabled;
                        idMode = await _dataService.InsertModeTracking(modeToAddInDb);
                        break;
                    default:
                        return false;
                }

                // Update power saving state only if the current state of power saving was off
                if (!App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == SeekiosUpdated.Idseekios).IsInPowerSaving)
                {
                    SeekiosUpdated.IsInPowerSaving = trackingSetting.IsPowerSavingEnabled;
                    App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == SeekiosUpdated.Idseekios).IsInPowerSaving = trackingSetting.IsPowerSavingEnabled;
                }

                // Save the setting tracking offline
                _saveDataService.SaveData(App.TrackingSetting, JsonConvert.SerializeObject(LsTrackingSetting));

                // Update the seekios
                if (idMode <= 0) return false;
                SeekiosUpdated.HasGetLastInstruction = false;
                if (MapViewModelBase.Seekios != null)
                {
                    MapViewModelBase.Seekios.HasGetLastInstruction = false;
                }

                // We locally delete the last mode and the alerts associate
                foreach (var mode in App.CurrentUserEnvironment.LsMode.Where(x => x.Seekios_idseekios == SeekiosUpdated.Idseekios))
                {
                    var idAlerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == mode.Idmode).Select(x => x.IdAlert).ToList();
                    foreach (var idAlert in idAlerts)
                    {
                        App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == idAlert);
                        App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdAlert == idAlert);
                    }
                }
                App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Seekios_idseekios == SeekiosUpdated.Idseekios);

                // Add the mode in local data
                var modeToAddInLocal = new ModeDTO()
                {
                    Idmode = idMode,
                    CountOfTriggeredAlert = 0,
                    DateModeCreation = DateTime.UtcNow,
                    Device_iddevice = modeToAddInDb.Device_iddevice,
                    Seekios_idseekios = modeToAddInDb.Seekios_idseekios,
                    StatusDefinition_idstatusDefinition = (int)StatutDefinitionEnum.RAS,
                    Trame = modeToAddInDb.Trame,
                    ModeDefinition_idmodeDefinition = (int)modeDefinition,
                    IsPowerSavingEnabled = modeToAddInDb.IsPowerSavingEnabled
                };
                App.CurrentUserEnvironment.LsMode.Add(modeToAddInLocal);

                // Handle alerts if it's required
                switch (modeDefinition)
                {
                    case ModeDefinitionEnum.ModeDontMove:
                        // configure a bool to execute 2 back actions 
                        App.Locator.ListAlert.Seekios = SeekiosUpdated;
                        var dontMoveAlerts = await _dataService.AlertsByMode(modeToAddInDb);
                        if (dontMoveAlerts != null)
                        {
                            foreach (AlertDTO a in dontMoveAlerts)
                            {
                                App.CurrentUserEnvironment.LsAlert.Add(a);
                            }
                        }
                        // Pourquoi on utilise le ModeZoneViewModel pour le DontMove???
                        App.Locator.ModeDontMove.LsAlertsModeDontMove?.Clear();
                        break;
                    case ModeDefinitionEnum.ModeZone:
                        // il faut aussi aller chercher les alertes pour le nouveau mode avec leur nouveaux ids...
                        // vider la liste dans ModeZoneVM + ajouter les nlles alertes dans LsAlertes
                        var zoneAlerts = await _dataService.AlertsByMode(modeToAddInDb);
                        if (zoneAlerts != null)
                        {
                            foreach (AlertDTO a in zoneAlerts)
                            {
                                App.CurrentUserEnvironment.LsAlert.Add(a);
                            }
                        }
                        App.Locator.ModeZone.LsAlertsModeZone?.Clear();
                        break;
                }

                // Delete the seekios tracking object because it's a new mode
                var seekiosOnTrackingToDelete = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == SeekiosUpdated.Idseekios);
                if (seekiosOnTrackingToDelete != null)
                {
                    if (seekiosOnTrackingToDelete.Timer.IsRunning) seekiosOnTrackingToDelete.Timer.Stop();
                    App.Locator.Map.LsSeekiosOnTracking.RemoveAll(x => x.Seekios.Idseekios == SeekiosUpdated.Idseekios);
                }

                // Setup a new seekios tracking object for a new timer
                if (modeToAddInLocal.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
                {
                    App.Locator.Map.AddSeekiosOnTracking(SeekiosUpdated, modeToAddInLocal);
                }

                // Hide loading layout
                _dialogService.HideLoadingLayout();
                return true;
            }
            catch (TimeoutException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (WebException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (Exception)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.UnexpectedError
                    , Resources.UnexpectedErrorTitle
                    , Resources.Close, null));
            }
            _dialogService.HideLoadingLayout();
            return false;
        }

        public void SelectModeSignalR(string uidDevice, string modeJson)
        {
            // Deserialize data
            if (string.IsNullOrEmpty(modeJson)) return;
            if (uidDevice == App.UidDevice) return;
            var modeToAdd = JsonConvert.DeserializeObject<ModeDTO>(modeJson);
            if (modeToAdd == null) return;
            modeToAdd.StatusDefinition_idstatusDefinition = (int)StatutDefinitionEnum.RAS;

            // Setup the current seekios
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == modeToAdd.Seekios_idseekios);
            if (seekios == null) return;

            // Settings to save offline the preferences for mode zone / don't move
            var trackingSetting = App.Locator.ModeSelection.LsTrackingSetting.FirstOrDefault(x => x.IdSeekios == seekios.Idseekios && x.ModeDefinition == (ModeDefinitionEnum)modeToAdd.ModeDefinition_idmodeDefinition);
            int refreshTime = 0;
            if (trackingSetting == null)
            {
                trackingSetting = new TrackingSetting()
                {
                    IdSeekios = modeToAdd.Seekios_idseekios,
                    ModeDefinition = (ModeDefinitionEnum)modeToAdd.ModeDefinition_idmodeDefinition,
                    RefreshTime = MapViewModelBase.RefreshTime,
                    IsPowerSavingEnabled = modeToAdd.IsPowerSavingEnabled
                };
                App.Locator.ModeSelection.LsTrackingSetting.Add(trackingSetting);
            }
            if (seekios.VersionEmbedded_idversionEmbedded < (int)VersionEmbeddedEnum.V1007)
            {
                // 1.006 et en dessous
                var param = modeToAdd.Trame.Split(';');
                int.TryParse(param[0], out refreshTime);
            }
            else
            {
                // 1.007 et au dessus
                var param = modeToAdd.Trame.Split(';');
                int.TryParse(param[1], out refreshTime);
            }
            if (refreshTime > 0)
            {
                trackingSetting.IsEnable = true;
                trackingSetting.RefreshTime = refreshTime;
            }
            else trackingSetting.RefreshTime = MapViewModelBase.RefreshTime;
            if (modeToAdd.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove)
            {
                App.Locator.ListAlert.Seekios = seekios;
                App.Locator.ModeDontMove.LsAlertsModeDontMove?.Clear();
            }
            else if (modeToAdd.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeZone)
            {
                App.Locator.ModeZone.LsAlertsModeZone?.Clear();
            }

            // Update power saving state only if the current state of power saving was off
            if (!App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == seekios.Idseekios).IsInPowerSaving)
            {
                seekios.IsInPowerSaving = trackingSetting.IsPowerSavingEnabled;
                App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == seekios.Idseekios).IsInPowerSaving = trackingSetting.IsPowerSavingEnabled;
            }

            // Save the setting tracking offline
            _saveDataService.SaveData(App.TrackingSetting, JsonConvert.SerializeObject(LsTrackingSetting));

            // Update the seekios
            if (App.Locator.DetailSeekios != null) App.Locator.DetailSeekios.SeekiosSelected.HasGetLastInstruction = false;
            if (SeekiosUpdated != null) SeekiosUpdated.HasGetLastInstruction = false;
            if (MapViewModelBase.Seekios != null) MapViewModelBase.Seekios.HasGetLastInstruction = false;
            seekios.HasGetLastInstruction = false;

            // We locally delete the last mode and the alerts associate
            foreach (var mode in App.CurrentUserEnvironment.LsMode.Where(x => x.Seekios_idseekios == seekios.Idseekios))
            {
                var idAlerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == mode.Idmode).Select(x => x.IdAlert).ToList();
                foreach (var idAlert in idAlerts)
                {
                    App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == idAlert);
                    App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdAlert == idAlert);
                }
            }
            App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Seekios_idseekios == seekios.Idseekios);
            
            // Add the mode in local data
            App.CurrentUserEnvironment.LsMode.Add(modeToAdd);

            // Delete the seekios tracking object because it's a new mode
            var seekiosOnTrackingToDelete = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == seekios.Idseekios);
            if (seekiosOnTrackingToDelete != null)
            {
                if (seekiosOnTrackingToDelete.Timer.IsRunning) seekiosOnTrackingToDelete.Timer.Stop();
                App.Locator.Map.LsSeekiosOnTracking.RemoveAll(x => x.Seekios.Idseekios == seekios.Idseekios);
            }

            // Setup a new seekios tracking object for a new timer
            if (modeToAdd.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeTracking)
            {
                App.Locator.Map.AddSeekiosOnTracking(seekios, modeToAdd);
            }

            // Refresh views
            if (_dispatcherService != null)
            {
                _dispatcherService.Invoke(() =>
                {
                    App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                    App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                    App.RaiseSeekiosInformationChangedEverywhere(seekios.Idseekios);
                });
            }
        }

        /// <summary>
        /// Navigate to a mode view
        /// </summary>
        public void GoToMode(ModeDefinitionEnum mode)
        {
            MapViewModelBase.Seekios = SeekiosUpdated;
            switch (mode)
            {
                case ModeDefinitionEnum.ModeTracking:
                    _navigationService.NavigateTo(App.MODE_TRACKING_PAGE);
                    break;
                case ModeDefinitionEnum.ModeDontMove:
                    App.Locator.Alert.ModeDefinition = ModeDefinitionEnum.ModeDontMove;
                    _navigationService.NavigateTo(App.MODE_DONT_MOVE_2_PAGE);
                    break;
                case ModeDefinitionEnum.ModeZone:
                    App.Locator.Alert.ModeDefinition = ModeDefinitionEnum.ModeZone;
                    App.Locator.ModeZone.IsOnEditMode = true;
                    _navigationService.NavigateTo(App.MODE_ZONE_PAGE);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Delete a mode
        /// </summary>
        public async Task<bool> DeleteMode(ModeDTO mode)
        {
            try
            {
                // Display the popup to confirm the action
                if (mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosMoved
                    || mode.StatusDefinition_idstatusDefinition == (int)StatutDefinitionEnum.SeekiosOutOfZone)
                {
                    if (await _dialogService.ShowMessage(string.Empty
                        , Resources.DeleteModeContent
                        , Resources.DeleteModeYes
                        , Resources.DeleteModeNo
                        , null) == false) return false;
                }
                else if (App.CurrentUserEnvironment.LsSeekios.First(x => x.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios).IsInPowerSaving == true)
                {
                    if (await _dialogService.ShowMessage(Resources.DeleteModePowerSavingOnContent
                        , Resources.DeleteModeContent
                        , Resources.DeleteModeYes
                        , Resources.DeleteModeNo
                        , null) == false) return false;
                }
                else
                {
                    if (await _dialogService.ShowMessage(string.Empty
                        , Resources.DeleteModeContent
                        , Resources.DeleteModeYes
                        , Resources.DeleteModeNo
                        , null) == false) return false;
                }

                // Stop the timer if the seekios is in tracking
                _dialogService.ShowLoadingLayout();
                var seekiosOnTracking = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                if (seekiosOnTracking != null)
                {
                    if (seekiosOnTracking.Timer.IsRunning) seekiosOnTracking.Timer.Stop();
                    App.Locator.Map.LsSeekiosOnTracking.Remove(seekiosOnTracking);
                    if (App.Locator.BaseMap.LsSeekiosAlertState.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                    {
                        App.Locator.BaseMap.LsSeekiosAlertState.Remove(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                    }
                }

                // Delete the mode in database
                var result = await _dataService.DeleteMode(mode.Idmode.ToString());
                _dialogService.HideLoadingLayout();
                if (result == 1)
                {
                    // Delete the last mode and the alerts associate in local memory
                    var idAlerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == mode.Idmode).Select(x => x.IdAlert);
                    foreach (var idAlert in idAlerts)
                    {
                        App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == idAlert);
                        App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdAlert == idAlert);
                    }
                    App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Idmode == mode.Idmode);
                    //await _dialogService.ShowMessage(Resources.DeleteModeSuccess, Resources.DeleteModeSuccessTitle);
                    return true;
                }
            }
            catch (TimeoutException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (WebException)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.TimeoutError
                    , Resources.TimeoutErrorTitle
                    , Resources.Close, null));
            }
            catch (Exception)
            {
                _dispatcherService.Invoke(async () => await _dialogService.ShowError(
                    Resources.UnexpectedError
                    , Resources.UnexpectedErrorTitle
                    , Resources.Close, null));
            }
            _dialogService.HideLoadingLayout();
            return false;
        }

        /// <summary>
        /// Delete a mode
        /// </summary>
        public void DeleteModeSignalR(string uidDevice, string idmodeStr)
        {
            if (string.IsNullOrEmpty(idmodeStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idmode = 0;
            if (!int.TryParse(idmodeStr, out idmode)) return;
            var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(x => x.Idmode == idmode);
            if (mode == null) return;
            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == mode.Seekios_idseekios);
            if (seekios == null) return;

            // Stop the timer if the seekios is in tracking
            var seekiosOnTracking = App.Locator.Map.LsSeekiosOnTracking.FirstOrDefault(x => x.Seekios.Idseekios == seekios.Idseekios);
            if (seekiosOnTracking != null)
            {
                if (seekiosOnTracking.Timer.IsRunning) seekiosOnTracking.Timer.Stop();
                App.Locator.Map.LsSeekiosOnTracking.Remove(seekiosOnTracking);
                if (App.Locator.BaseMap.LsSeekiosAlertState.Contains(App.Locator.DetailSeekios.SeekiosSelected.Idseekios))
                {
                    App.Locator.BaseMap.LsSeekiosAlertState.Remove(App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                }
            }
            // Delete the last mode and the alerts associate in local memory
            var idAlerts = App.CurrentUserEnvironment.LsAlert.Where(x => x.IdMode == idmode).Select(x => x.IdAlert);
            foreach (var idAlert in idAlerts)
            {
                App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == idAlert);
                App.CurrentUserEnvironment.LsAlert.RemoveAll(x => x.IdAlert == idAlert);
            }
            App.CurrentUserEnvironment.LsMode.RemoveAll(x => x.Idmode == mode.Idmode);
            // Update UI
            if (_dispatcherService != null)
            {
                _dispatcherService.Invoke(() =>
                {
                    App.Locator.ListSeekios.ActivityNeedsUIToBeUpdated = true;
                    App.Locator.DetailSeekios.ActivityNeedsUIToBeUpdated = true;
                    App.RaiseSeekiosInformationChangedEverywhere(0);
                });
            }

        }

        public string DisplayModeRefreshTimeTracking(ModeDTO mode)
        {
            if (mode != null && !string.IsNullOrEmpty(mode.Trame))
            {
                var param = mode.Trame.Split(';');
                if (param?.Count() >= 2)
                {
                    int refreshValue = 0;
                    int.TryParse(param[1], out refreshValue);
                    int[] values = new int[] { 1, 2, 3, 4, 5, 10, 15, 30, 60, 120, 180, 240, 300, 360, 720, 1440 };
                    if (values.Contains(refreshValue))
                    {
                        return string.Format(Resources.RefreshRate
                            , refreshValue < 60 ? refreshValue + " min" : refreshValue / 60 + " h");
                    }
                    else return Resources.No;
                }
                else return Resources.No;
            }
            else return Resources.No;
        }

        #endregion
    }
}
