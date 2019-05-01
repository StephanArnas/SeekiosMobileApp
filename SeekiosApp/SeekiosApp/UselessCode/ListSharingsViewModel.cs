//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using SeekiosApp.Interfaces;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;
//using System;

//namespace SeekiosApp.ViewModel
//{
//    public class ListSharingsViewModel : ViewModelBase
//    {
//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="dataService">data service</param>
//        /// <param name="dialogService">dialog service</param>
//        /// <param name="bluetoothManager"></param>
//        public ListSharingsViewModel(IDataService dataService, IDialogService dialogService)
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

//        #region ===== Propriétés =================================================================

//        /// <summary>
//        /// Whole sharings list
//        /// </summary>
//        public List<SharingDTO> LsSharing
//        {
//            get
//            {
//                if (SelectedFriend != null)
//                    return App.CurrentUserEnvironment.LsSharing;
//                else return new List<SharingDTO>();
//            }
//        }

//        /// <summary>
//        /// List of sharings done by the authenticated user
//        /// </summary>
//        public List<SharingDTO> LsOwnSharing
//        {
//            get
//            {
//                if (SelectedFriend != null)
//                    return LsSharing.Where(el => (el.User_IdUser == SelectedFriend.IdUser && el.IsUserOwner == false)
//                                                       || (el.Friend_IdUser == SelectedFriend.IdUser && el.IsUserOwner == true)).ToList();
//                else return new List<SharingDTO>();
//            }

//        }

//        /// <summary>
//        /// List of sharings done by current user's firends
//        /// </summary>
//        public List<SharingDTO> LsOtherSharing
//        {
//            get
//            {
//                if (SelectedFriend != null)
//                    return LsSharing.Where(el => (el.User_IdUser == SelectedFriend.IdUser && el.IsUserOwner == true)
//                                                       || (el.Friend_IdUser == SelectedFriend.IdUser && el.IsUserOwner == false)).ToList();
//                else return new List<SharingDTO>();
//            }
//        }

//        ///<summary>Friend selected from the friend list</summary>
//        public FriendUserDTO SelectedFriend { get; set; }


//        /// <summary>SelectedFriend formatted Name + FirstName in a string</summary>
//        public string FormattedFriendName
//        {
//            get
//            {
//                if (SelectedFriend != null)
//                    return string.Format("{0} {1}", SelectedFriend.FirstName, SelectedFriend.LastName);
//                else return string.Empty;
//            }
//        }

//        #endregion

//        #region ===== Méthodes Publiques ==========================================================

//        /// <summary>
//        /// Navigate to ShareSeekios page
//        /// </summary>
//        public void GoToShareSeekios()
//        {
//            _navigationService.NavigateTo(App.SHARE_SEEKIOS_PAGE);
//        }

//        /// <summary>
//        /// Delete a sharing from the context menu
//        /// </summary>
//        public async void DeleteSharing(SharingDTO sharing)
//        {
//            try
//            {
//                if (sharing == null) return;

//                App.CurrentUserEnvironment.LsSharing.Remove(sharing);
//                var result = await _dataService.DeleteSharing(sharing.User_IdUser, sharing.Friend_IdUser, sharing.Seekios_IdSeekios);
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            return;
//        }

//        /// <summary>
//        /// Update a sharing
//        /// </summary>
//        /// <param name="sharing"></param>
//        public async Task<int> UpdateSharing(SharingDTO sharing)
//        {
//            try
//            {
//                if (sharing == null) return -1;
//                return await _dataService.UpdateSharing(sharing);
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
        
//        #region ===== Méthodes Privées ============================================================

//        #endregion
//    }
//}
