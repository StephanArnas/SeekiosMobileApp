using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Newtonsoft.Json;
using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using SeekiosApp.Model.DTO;
using SeekiosApp.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SeekiosApp.ViewModel
{
    public class AlertSOSViewModel : ViewModelBase, IDisposable
    {
        #region ===== Attributs ===================================================================

        private IDataService _dataService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDispatchOnUIThread _dispatcherService = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>The user defines an alert to be sent when an SOS has to be sent</summary>
        public AlertDTO CurrentAlertSOS { get; set; }

        /// <summary>Recipient list</summary>
        public List<AlertRecipientDTO> LsRecipients { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public AlertSOSViewModel(IDataService dataService, Interfaces.IDialogService dialogService, IDispatchOnUIThread dispatcherService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _dispatcherService = dispatcherService;
            LsRecipients = new List<AlertRecipientDTO>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Modify SOS alert, depending of the SOSAlert variable
        /// </summary>
        public async Task<bool> InsertOrUpdateAlertSOS(string title, string content)
        {
            try
            {
                // Adding alert sos 
                if (CurrentAlertSOS == null)
                {
                    // Create a new alert with recipient to add
                    var alertWithRecipientToAdd = new AlertWithRecipientDTO
                    {
                        IdAlertType = (int)AlertDefinitionEnum.Email,
                        IdMode = null,
                        Title = title,
                        Content = content,
                        LsRecipients = LsRecipients,
                    };
                    // Add the alert with recipient in the database
                    var alertId = await _dataService.InsertAlertSOSWithRecipient(App.Locator.DetailSeekios.SeekiosSelected.Idseekios, alertWithRecipientToAdd);
                    // Something wrong
                    if (alertId <= 0)
                    {
                        return false;
                    }
                    else
                    {
                        // Update the local values 
                        App.Locator.AddSeekios.UpdatingSeekios.AlertSOS_idalert = alertId;
                        alertWithRecipientToAdd.IdAlert = alertId;
                        App.CurrentUserEnvironment.LsAlert.Add(alertWithRecipientToAdd);
                        App.CurrentUserEnvironment.LsSeekios[App.CurrentUserEnvironment.LsSeekios.IndexOf(App.Locator.AddSeekios.UpdatingSeekios)].AlertSOS_idalert = alertId;
                        foreach (var recipient in LsRecipients) recipient.IdAlert = alertId;
                        App.CurrentUserEnvironment.LsAlertRecipient.AddRange(LsRecipients);
                        CurrentAlertSOS = alertWithRecipientToAdd;
                        return true;
                    }
                }
                // Modify alert sos
                else
                {
                    // Create a new alert with recipient to update
                    var alertWithRecipientToUpdate = new AlertWithRecipientDTO
                    {
                        IdAlert = CurrentAlertSOS.IdAlert,
                        IdAlertType = (int)AlertDefinitionEnum.Email,
                        IdMode = null,
                        Title = title,
                        Content = content,
                        LsRecipients = LsRecipients,
                    };
                    foreach (var recipient in alertWithRecipientToUpdate.LsRecipients)
                    {
                        recipient.IdAlert = alertWithRecipientToUpdate.IdAlert;
                    }
                    // Update the alert with recipient in the database
                    if (await _dataService.UpdateAlertSOSWithRecipient(App.Locator.DetailSeekios.SeekiosSelected.Idseekios, alertWithRecipientToUpdate) > 0)
                    {
                        CurrentAlertSOS.Title = title;
                        CurrentAlertSOS.Content = content;
                        App.CurrentUserEnvironment.LsAlert.RemoveAll(r => r.IdAlert == CurrentAlertSOS.IdAlert);
                        App.CurrentUserEnvironment.LsAlert.Add(alertWithRecipientToUpdate);
                        App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(r => r.IdAlert == CurrentAlertSOS.IdAlert);
                        App.CurrentUserEnvironment.LsAlertRecipient.AddRange(LsRecipients);
                    }
                    return true;
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
            return false;
        }

        /// <summary>
        /// Modify SOS alert, depending of the SOSAlert variable
        /// </summary>
        public void InsertOrUpdateAlertSOSSignalR(string uidDevice, string idseekiosStr, string alertWithRecipientJson)
        {
            if (string.IsNullOrEmpty(idseekiosStr)) return;
            if (uidDevice == App.UidDevice) return;
            int idseekios = 0;
            if (!int.TryParse(idseekiosStr, out idseekios)) return;
            if (string.IsNullOrEmpty(alertWithRecipientJson)) return;
            var alertWithRecipient = JsonConvert.DeserializeObject<AlertWithRecipientDTO>(alertWithRecipientJson);
            if (alertWithRecipient != null)
            {
                var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(x => x.Idseekios == idseekios);
                if (seekios != null)
                {
                    // Update Alert SOS
                    if (seekios.AlertSOS_idalert.HasValue)
                    {
                        // Remove old data
                        var alert = App.CurrentUserEnvironment.LsAlert.FirstOrDefault(x => x.IdAlert == seekios.AlertSOS_idalert);
                        App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(x => x.IdAlert == alert.IdAlert);
                        App.CurrentUserEnvironment.LsAlert.Remove(alert);
                        App.Locator.AlertSOS.LsRecipients.Clear();

                        // Add new data
                        CurrentAlertSOS = alertWithRecipient;
                        App.CurrentUserEnvironment.LsAlert.Add(alertWithRecipient);
                        seekios.AlertSOS_idalert = alertWithRecipient.IdAlert;
                        foreach (var recipient in alertWithRecipient.LsRecipients) recipient.IdAlert = alertWithRecipient.IdAlert;
                        App.CurrentUserEnvironment.LsAlertRecipient.AddRange(alertWithRecipient.LsRecipients);
                    }
                    // Insert Alert SOS
                    else
                    {
                        CurrentAlertSOS = alertWithRecipient;
                        seekios.AlertSOS_idalert = alertWithRecipient.IdAlert;
                        App.CurrentUserEnvironment.LsAlert.RemoveAll(r => r.IdAlert == alertWithRecipient.IdAlert);
                        App.CurrentUserEnvironment.LsAlert.Add(alertWithRecipient);
                        App.CurrentUserEnvironment.LsAlertRecipient.RemoveAll(r => r.IdAlert == alertWithRecipient.IdAlert);
                        App.Locator.AlertSOS.LsRecipients.Clear();
                        if (alertWithRecipient.LsRecipients?.Count > 0)
                        {
                            foreach (var recipient in alertWithRecipient.LsRecipients) recipient.IdAlert = alertWithRecipient.IdAlert;
                            App.CurrentUserEnvironment.LsAlertRecipient.AddRange(alertWithRecipient.LsRecipients);
                        }
                    }
                    // Update the view
                    if (_dispatcherService != null)
                    {
                        _dispatcherService.Invoke(() =>
                        {
                            OnAlertSosChanged?.Invoke(null, null);
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Initialize properties to default or null 
        /// </summary>
        public void Dispose()
        {
            CurrentAlertSOS = null;
            if (LsRecipients != null)
            {
                LsRecipients = new List<AlertRecipientDTO>();
            }
        }

        #endregion

        #region ===== Events ======================================================================

        public event EventHandler OnAlertSosChanged;

        #endregion
    }
}
