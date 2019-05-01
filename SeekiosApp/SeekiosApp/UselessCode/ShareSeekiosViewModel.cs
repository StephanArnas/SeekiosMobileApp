//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using SeekiosApp.Interfaces;
//using System;
//using System.Net;
//using System.Threading.Tasks;
//using System.Collections.Generic;

//namespace SeekiosApp.ViewModel
//{
//    public class ShareSeekiosViewModel : ViewModelBase, IDisposable
//    {

//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="dataService"></param>
//        /// <param name="dialogService"></param>
//        public ShareSeekiosViewModel(IDataService dataService, IDialogService dialogService, IBluetoothService bluetoothManager)
//        {
//            _dataService = dataService;
//            _dialogService = dialogService;
//            _navigationService = ServiceLocator.Current.GetInstance<INavigationService>();

//        }

//        #endregion

//        #region ===== Attributs ===================================================================

//        /// <summary>Navigation service</summary>
//        private INavigationService _navigationService;

//        /// <summary>Dialog service</summary>
//        private IDialogService _dialogService;

//        /// <summary>Data access service</summary>
//        private IDataService _dataService;


//        #endregion

//        #region ===== Propriétés ==================================================================

//        /// <summary>True if a sharing is being updated (not created)</summary>
//        public bool IsInEditMode
//        {
//            get
//            {
//                return _isInEditMode;
//            }

//            set
//            {
//                _isInEditMode = value;
//            }
//        }
//        private bool _isInEditMode = false;

//        /// <summary>
//        /// Selected sharing from the sharing list
//        /// </summary>
//        public SharingDTO SharingSelected
//        {
//            get
//            {
//                return _sharingSelected;
//            }

//            set
//            {
//                _sharingSelected = value;
//            }
//        }
//        private SharingDTO _sharingSelected;

//        /// <summary>
//        /// Seekios selected in the list when chosing a seekios to share
//        /// </summary>
//        public ShortSeekiosDTO SeekiosSelected
//        {
//            get
//            {
//                return _seekiosSelected;
//            }
//            set
//            {
//                _seekiosSelected = value;
//            }
//        }
//        private ShortSeekiosDTO _seekiosSelected;

//        /// <summary>
//        /// Dictionary representing the friend users linked with a bool value
//        /// isChecked = true if I want to share with this user
//        /// </summary>
//        public Dictionary<ShortSeekiosDTO, bool> ShortCheckedSeekiosDictionary
//        {
//            get
//            {
//                if (_shortCheckedSeekiosDictionary == null) _shortCheckedSeekiosDictionary = new Dictionary<ShortSeekiosDTO, bool>();
//                return _shortCheckedSeekiosDictionary;
//            }
//            set
//            {
//                _shortCheckedSeekiosDictionary = value;
//            }
//        }
//        private Dictionary<ShortSeekiosDTO, bool> _shortCheckedSeekiosDictionary;

//        /// <summary>
//        /// List of seekios I want to share
//        /// </summary>
//        public List<ShortSeekiosDTO> SeekiosToShareList
//        {
//            get
//            {
//                if (_seekiosToShareList == null) _seekiosToShareList = new List<ShortSeekiosDTO>();
//                return _seekiosToShareList;
//            }
//            set
//            {
//                _seekiosToShareList = value;
//            }
//        }
//        private List<ShortSeekiosDTO> _seekiosToShareList;

//        /// <summary>
//        /// True if its a sharing from the current authenticated user, false if its a sharing from a friend
//        /// </summary>
//        public bool IsOwnSharing
//        {
//            get
//            {
//                return _isOwnSharing;
//            }
//            set
//            {
//                _isOwnSharing = value;
//            }
//        }
//        private bool _isOwnSharing;

//        /// <summary>
//        /// Dictionary representing the friend users linked with a bool value
//        /// isChecked = true if I want to share with this user
//        /// </summary>
//        public Dictionary<ShortUserDTO, bool> ShortCheckedUserDictionary
//        {
//            get
//            {
//                if (_shortCheckedUserDictionary == null) _shortCheckedUserDictionary = new Dictionary<ShortUserDTO, bool>();
//                return _shortCheckedUserDictionary;
//            }
//            set
//            {
//                _shortCheckedUserDictionary = value;
//            }
//        }
//        private Dictionary<ShortUserDTO, bool> _shortCheckedUserDictionary;

//        /// <summary>
//        /// List of users I want to share my seekios with
//        /// </summary>
//        public List<ShortUserDTO> ShortCheckedUserList
//        {
//            get
//            {
//                if (_shortCheckedUserList == null) _shortCheckedUserList = new List<ShortUserDTO>();
//                return _shortCheckedUserList;
//            }
//        }
//        private List<ShortUserDTO> _shortCheckedUserList;

//        #endregion

//        #region ===== Méthodes Privées ============================================================


//        #endregion

//        #region ===== Méthodes Publiques ==========================================================

//        /// <summary>
//        /// Create multiple sharing in database
//        /// </summary>
//        /// <param name="sharing"></param>
//        public async Task<int> InsertMultipleSharing(List<SharingDTO> seekiosSharing)
//        {
//            try
//            {
//                return await _dataService.InsertMultipleSharing(seekiosSharing);
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            return 0;
//        }

//        /// <summary>
//        /// create a sharing in database
//        /// </summary>
//        /// <param name="sharing"></param>
//        public async Task<int> InsertSharing(SharingDTO sharing)
//        {
//            try
//            {
//                return await _dataService.InsertSharing(sharing);
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            return 0;
//        }

//        public void Dispose()
//        {
//            SeekiosSelected = null;
//            SharingSelected = null;
//            IsInEditMode = false;
//        }

//        /// <summary>
//        /// Delete a sharing
//        /// </summary>
//        /// <param name="sharing"></param>
//        /// <returns></returns>
//        public async Task<int> DeleteSharing(SharingDTO sharing)
//        {
//            try
//            {
//                return await _dataService.DeleteSharing(sharing.User_IdUser, sharing.Friend_IdUser, sharing.Seekios_IdSeekios);
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            return 0;
//        }

//        #endregion
//    }
//}
