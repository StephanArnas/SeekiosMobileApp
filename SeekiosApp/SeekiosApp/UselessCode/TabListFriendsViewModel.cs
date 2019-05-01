//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using SeekiosApp.Interfaces;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System;
//using System.Threading.Tasks;

//namespace SeekiosApp.ViewModel
//{
//    public class TabListFriendsViewModel : ViewModelBase
//    {
//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="dataService"></param>
//        /// <param name="dialogService"></param>
//        public TabListFriendsViewModel(IDataService dataService, IDialogService dialogService)
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

//        /// <summary>True if method GetPictureByUser() has been called</summary>
//        public bool IsPictureImageLoaded
//        {
//            get
//            {
//                return _isPictureImageLoaded;
//            }
//            set
//            {
//                _isPictureImageLoaded = value;
//            }
//        }
//        private bool _isPictureImageLoaded = false;

//        /// <summary>
//        /// Friend list
//        /// </summary>
//        public List<FriendUserDTO> LsFriends
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.LsFriend.OrderBy(x => x.FirstName).ToList();
//            }
//        }

//        /// <summary>
//        /// Sharing list
//        /// </summary>
//        public List<SharingDTO> LsSharing
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.LsSharing;
//            }
//        }

//        #endregion

//        #region ===== Méthodes Publiques ==========================================================

//        /// <summary>
//        /// Navigate to Add Friend page
//        /// </summary>
//        public void GoToAddFriend()
//        {
//            _navigationService.NavigateTo(App.ADD_FRIEND_PAGE);
//        }

//        /// <summary>
//        /// Navigate to sharing list page
//        /// </summary>
//        public void GoToListShareSeekios()
//        {
//            _navigationService.NavigateTo(App.LIST_SHARINGS_PAGE);
//        }

//        /// <summary>
//        /// Get users picture asynchronously
//        /// </summary>
//        public async Task<bool> GetPictureByUser()
//        {
//            try
//            {
//                if (!IsPictureImageLoaded)
//                {
//                    IsPictureImageLoaded = true;
//                    if (App.CurrentUserEnvironment.LsFriend != null)
//                    {
//                        foreach (var friend in App.CurrentUserEnvironment.LsFriend)
//                        {
//                            var idFriend = friend.User_IdUser != App.CurrentUserEnvironment.User.IdUser ? friend.User_IdUser : friend.Friend_IdUser;
//                            var image = await _dataService.GetPictureByUser(idFriend);
//                            if (!string.IsNullOrEmpty(image) && image.Length > 10)
//                            {
//                                friend.UserPicture = image.Replace("\"", "").Replace("\\", ""); //TODO: tavucbo
//                                RaisePropertyChanged("LsFriend");
//                            }
//                        }
//                    }
//                }
//                return true;
//            }
//            catch (WebException e)
//            {
//                await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            catch (TimeoutException e)
//            {
//                await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//            }
//            return false;
//        }

//        /// <summary>
//        /// Delete friendship
//        /// </summary>
//        public async void DeleteFriendship(int idu, int idf)
//        {
//            try
//            {
//                if (idu <= 0 || idf <= 0) return;

//                var friendshipToDelete = App.CurrentUserEnvironment.LsFriend.Where(el => el.IdUser == idf).FirstOrDefault();
//                if (friendshipToDelete != null)
//                {
//                    App.CurrentUserEnvironment.LsFriend.Remove(friendshipToDelete);
//                    var result = await _dataService.DeleteFriendship(idu, idf);
//                }
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

//        #endregion

//        #region ===== Méthodes Privées ============================================================

//        #endregion
//    }
//}
