using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using System.Linq;
using SeekiosApp.Enum;
using SeekiosApp.Model.DTO;
using SeekiosApp.Interfaces;
using System.Threading.Tasks;
using SeekiosApp.Properties;
using System.Collections.Generic;
using System;
using SeekiosApp.Extension;
using System.Net;

namespace SeekiosApp.ViewModel
{
    public class AlertViewModel : ViewModelBase, IDisposable
    {
        #region ===== Attributs ===================================================================

        private INavigationService _navigationService = null;
        private Interfaces.IDialogService _dialogService = null;
        private IDataService _dataService = null;
        private List<AlertRecipientDTO> _lsRecipients = null;

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Alert id</summary>
        public int IdAlert { get; set; }

        /// <summary>Mode id</summary>
        public int IdMode { get; set; }

        /// <summary>Enum: alert type id (NotificationPush = 1, SMS = 2, Email = 3, MessageCall = 4)</summary>
        public AlertDefinitionEnum? IdAlertTypeEnum { get; set; }

        /// <summary>Alert title</summary>
        public string TitleAlert { get; set; }

        /// <summary>Alert content</summary>
        public string ContentAlert { get; set; }

        /// <summary>Recipient list</summary>
        public List<AlertRecipientDTO> LsRecipients
        {
            get
            {
                if (_lsRecipients == null) _lsRecipients = new List<AlertRecipientDTO>();
                return _lsRecipients;
            }
            set
            {
                if (value != _lsRecipients)
                {
                    _lsRecipients = value;
                }
            }
        }

        /// <summary>True: the alert is new</summary>
        public bool IsNew { get; set; }

        /// <summary>Type of the mode we need to define the alert</summary>
        public ModeDefinitionEnum ModeDefinition { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public AlertViewModel(IDataService dataService, Interfaces.IDialogService dialogService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
            LsRecipients = App.CurrentUserEnvironment.GetRecipientsFromAlert(IdAlert);
        }

        #endregion

        #region ===== Public Methods ==============================================================

        #region Methods ViewModel

        public void ConvertAlertDTOToClassMember(AlertDTO alert)
        {
            if (alert != null)
            {
                IdAlert = alert.IdAlert;
                IdAlertTypeEnum = (AlertDefinitionEnum)System.Enum.Parse(typeof(AlertDefinitionEnum), alert.IdAlertType.ToString());
                TitleAlert = alert.Title;
                if (alert.IdMode != null) IdMode = alert.IdMode.Value;
                ContentAlert = alert.Content;
            }
        }

        public AlertWithRecipientDTO ConvertClassMemberToAlertWithRecipientDTO()
        {
            var source = new AlertWithRecipientDTO();
            if (ModeDefinition == ModeDefinitionEnum.ModeZone)
            {
                source.IdAlert = App.Locator.ModeZone.LsAlertsModeZone.Count();
            }
            else if (ModeDefinition == ModeDefinitionEnum.ModeDontMove)
            {
                source.IdAlert = App.Locator.ModeDontMove.LsAlertsModeDontMove.Count();
            }
            source.IdMode = IdMode;
            source.IdAlertType = (int)AlertDefinitionEnum.Email;
            source.Content = ContentAlert.ToUpperCaseFirst();
            source.Title = TitleAlert.ToUpperCaseFirst();
            source.LsRecipients = new List<AlertRecipientDTO>();
            foreach (AlertRecipientDTO recipient in LsRecipients)
            {
                source.LsRecipients.Add(recipient);
            }
            return source;
        }

        public void Dispose()
        {
            IdAlert = 0;
            IdMode = 0;
            IdAlertTypeEnum = null;
            TitleAlert = string.Empty;
            ContentAlert = string.Empty;
            if (LsRecipients?.Count() > 0) LsRecipients = new List<AlertRecipientDTO>();
        }

        #endregion

        #region Methods Alert

        public async Task<bool> InsertAlert(string title, string content)
        {
            TitleAlert = title;
            ContentAlert = content;
            if (await CheckIfRecipients() && !await CheckIfEmptyField())
            {
                if (ModeDefinition == ModeDefinitionEnum.ModeZone)
                {
                    App.Locator.ModeZone.LsAlertsModeZone.Add(ConvertClassMemberToAlertWithRecipientDTO());
                }
                else if (ModeDefinition == ModeDefinitionEnum.ModeDontMove)
                {
                    App.Locator.ModeDontMove.LsAlertsModeDontMove.Add(ConvertClassMemberToAlertWithRecipientDTO());
                }
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAlert(string title, string content)
        {
            TitleAlert = title;
            ContentAlert = content;
            if (await CheckIfRecipients() && !await CheckIfEmptyField())
            {
                var alertToUpdate = ConvertClassMemberToAlertWithRecipientDTO();
                AlertWithRecipientDTO alertOld = null;
                if (ModeDefinition == ModeDefinitionEnum.ModeZone)
                {
                    alertOld = App.Locator.ModeZone.LsAlertsModeZone.FirstOrDefault(el => el.IdAlert == IdAlert);
                }
                else if (ModeDefinition == ModeDefinitionEnum.ModeDontMove)
                {
                    alertOld = App.Locator.ModeDontMove.LsAlertsModeDontMove.FirstOrDefault(el => el.IdAlert == IdAlert);
                }
                alertOld.Title = alertToUpdate.Title;
                alertOld.Content = alertToUpdate.Content;
                alertOld.LsRecipients.Clear();
                alertOld.LsRecipients.AddRange(alertToUpdate.LsRecipients);
                return true;
            }
            else return false;
        }

        #endregion

        #region Methodes AlertFavorite

        public async Task AddFavoriteAlert(AlertFavoriteDTO alertFavorite)
        {
            try
            {
                if (alertFavorite == null) return;

                if (!await CheckIfEmptyFieldInAlertFavorite(alertFavorite))
                {
                    if (await CheckIfAlertFavoriteAlreadyExist(alertFavorite))
                    {
                        alertFavorite.IdUser = App.CurrentUserEnvironment.User.IdUser;
                        App.CurrentUserEnvironment.LsAlertFavorites.Add(alertFavorite);
                        var id = await _dataService.InsertAlertFavorite(alertFavorite);
                        alertFavorite.IdAlertFavorite = id;
                    }
                }
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText
                    , null);
            }
            return;
        }

        public async Task<bool> DeleteAlertFavorite(AlertFavoriteDTO alertFavorite)
        {
            try
            {
                if (alertFavorite == null) return false;

                App.CurrentUserEnvironment.LsAlertFavorites.Remove(alertFavorite);
                await _dataService.DeleteAlertFavorite(alertFavorite.IdAlertFavorite);

                return true;
            }
            catch (WebException)
            {
                await _dialogService.ShowError(Resources.TimeoutError
                    , Resources.WebErrorTitle
                    , Resources.WebErrorButtonText, null);
            }
            catch (TimeoutException e)
            {
                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle
                    , Resources.WebErrorButtonText
                    , null);
            }
            return false;
        }

        #endregion

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Check if at least one addressee is configured
        /// </summary>
        private async Task<bool> CheckIfRecipients()
        {
            if (LsRecipients == null || LsRecipients.Count == 0)
            {
                await _dialogService.ShowMessage(
                    Resources.AlertActivity_MissingRecipientContent,
                    Resources.AlertActivity_MissingRecipientTitle);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check if empty fields exist
        /// </summary>
        private async Task<bool> CheckIfEmptyField()
        {
            if (string.IsNullOrEmpty(TitleAlert))
            {
                await _dialogService.ShowMessage(
                    Resources.AlertActivity_MissingTitle_Content,
                    Resources.AlertActivity_MissingTitle_Title);
                return true;
            }
            else if (string.IsNullOrEmpty(ContentAlert))
            {
                await _dialogService.ShowMessage(
                    Resources.AlertActivity_MissingContent_Content,
                    Resources.AlertActivity_MissingContent_Title);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check if the alert favorite has empty fields
        /// </summary>
        private async Task<bool> CheckIfEmptyFieldInAlertFavorite(AlertFavoriteDTO source)
        {
            //TODO : handle error message
            var isDisplayError = false;
            var msg = string.Empty;
            var title = string.Empty;
            // message error if empty fields
            switch (IdAlertTypeEnum)
            {
                case AlertDefinitionEnum.SMS:
                    msg = Resources.ContentEmptySMS;
                    title = Resources.UnexpectedErrorTitle;
                    if (string.IsNullOrEmpty(source.Content)) isDisplayError = true;
                    break;
                case AlertDefinitionEnum.Email:
                    msg = Resources.ContentEmptyEmail;
                    title = Resources.UnexpectedErrorTitle;
                    if (string.IsNullOrEmpty(source.EmailObject) || string.IsNullOrEmpty(source.Content)) isDisplayError = true;
                    break;
                case AlertDefinitionEnum.MessageCall:
                    msg = Resources.ContentEmptyEmail;
                    title = Resources.UnexpectedErrorTitle;
                    if (source.Record == null) isDisplayError = true;
                    break;
            }

            // error message
            if (isDisplayError)
            {
                await _dialogService.ShowMessage(msg, title);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Check if the alert already exists in favorite
        /// </summary>
        private async Task<bool> CheckIfAlertFavoriteAlreadyExist(AlertFavoriteDTO source)
        {
            if (App.CurrentUserEnvironment.LsAlertFavorites.Any(el => el.Content == source.Content))
            {
                await _dialogService.ShowMessage(Resources.FavoriteAlertAlreadyExist
                    , Resources.FavoriteAlertAlreadyExistTitle);
                return false;
            }
            return true;
        }

        #endregion
    }
}