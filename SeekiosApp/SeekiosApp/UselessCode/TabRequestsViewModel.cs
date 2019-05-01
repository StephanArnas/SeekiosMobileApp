//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using SeekiosApp.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Threading.Tasks;

//namespace SeekiosApp.ViewModel
//{
//    public class TabRequestsViewModel : ViewModelBase, IDisposable
//    {
//        #region ===== Constructeur ================================================================

//        /// <summary>
//        /// Constructeur
//        /// </summary>
//        /// <param name="dataService">service d'accès aux données</param>
//        /// <param name="dialogService">service de dialogue</param>
//        public TabRequestsViewModel(IDataService dataService, IDialogService dialogService)
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

//        /// <summary>
//        /// True if a friend request has been sent
//        /// </summary>
//        public bool RequestHasBeenSent
//        {
//            get
//            {
//                return _requestHasBeenSent;
//            }
//            set
//            {
//                _requestHasBeenSent = value;
//            }
//        }
//        private bool _requestHasBeenSent = false;

//        /// <summary>Friendship requests associated to the current user</summary>
//        public List<FriendshipDTO> LsPendingFriendship
//        {
//            get
//            {
//                if (_lsPendingFriendship == null)
//                    _lsPendingFriendship = new List<FriendshipDTO>();

//                return _lsPendingFriendship;
//            }
//            set
//            {
//                _lsPendingFriendship = value;
//            }
//        }
//        private List<FriendshipDTO> _lsPendingFriendship;

//        /// <summary>User list linked to the pending friendship associated to the current logged-in user</summary>
//        public List<ShortUserDTO> LsUsers
//        {
//            get
//            {
//                if (_lsUsers == null)
//                    _lsUsers = new List<ShortUserDTO>();

//                return _lsUsers;
//            }
//            set
//            {
//                _lsUsers = value;
//            }
//        }
//        private List<ShortUserDTO> _lsUsers;

//        /// <summary>True if data is loaded</summary>
//        public bool IsDataLoaded = false;

//        #endregion

//        #region ===== Méthodes Publiques ==========================================================

//        /// <summary>
//        /// Delete pending friendship
//        /// </summary>
//        /// <param name="idUser"></param>
//        /// <param name="idFriend"></param>
//        public async void DeleteFriendship(int idUser, int idFriend)
//        {
//            try
//            {
//                await _dataService.DeleteFriendship(idUser, idFriend);
//                var friendship = LsPendingFriendship.Where(el => el.User_IdUser == idUser && el.Friend_IdUser == idFriend).FirstOrDefault();
//                LsPendingFriendship.Remove(friendship);
//                var user = new ShortUserDTO();
//                if (friendship.Friend_IdUser == App.CurrentUserEnvironment.User.IdUser) user = LsUsers.Where(el => el.IdUser == friendship.User_IdUser).FirstOrDefault();
//                else user = LsUsers.Where(el => el.IdUser == friendship.Friend_IdUser).FirstOrDefault();
//                LsUsers.Remove(user);
//                App.Locator.TabRequests.IsDataLoaded = false;
//                RaisePropertyChanged("RequestDeleted");
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
//        /// Update view when a friend request is sent
//        /// </summary>
//        public void UpdateUI(ShortUserDTO user)
//        {
//            IsDataLoaded = false;
//            RequestHasBeenSent = true;
//            GetUserPicture(user.IdUser);
//            RaisePropertyChanged("RequestSent");
//        }

//        /// <summary>
//        /// Accept friendship request
//        /// </summary>
//        /// <param name="friendship"></param>
//        public async Task<int> AcceptFriendship(FriendshipDTO friendship)
//        {
//            try
//            {
//                await _dataService.AcceptFriendship(friendship);
//                LsPendingFriendship.Remove(friendship);
//                ShortUserDTO user = new ShortUserDTO();

//                if (friendship.Friend_IdUser == App.CurrentUserEnvironment.User.IdUser) user = LsUsers.Where(el => el.IdUser == friendship.User_IdUser).FirstOrDefault();
//                else user = LsUsers.Where(el => el.IdUser == friendship.Friend_IdUser).FirstOrDefault();

//                LsUsers.Remove(user);
//                App.CurrentUserEnvironment.LsFriend.Add(new FriendUserDTO()
//                {
//                    IdUser = App.CurrentUserEnvironment.User.IdUser,
//                    FirstName = user.FirstName,
//                    LastName = user.LastName,
//                    Friend_IdUser = friendship.Friend_IdUser,
//                    User_IdUser = friendship.User_IdUser,
//                    UserPicture = user.UserPicture
//                });
//                App.Locator.TabRequests.IsDataLoaded = false;
//                RaisePropertyChanged("RequestAccepted");

//                return 1;
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

//        /// <summary>Data initialization</summary>
//        public async Task<int> InitialiseData()
//        {
//            if (!IsDataLoaded)
//            {
//                IsDataLoaded = true;
//                try
//                {
//                    LsPendingFriendship = await _dataService.GetPendingFriendshipByUser(App.CurrentUserEnvironment.User.IdUser);
//                    // Update user list
//                    LsUsers = await _dataService.GetShortUsersByPendingFriendship(App.CurrentUserEnvironment.User.IdUser.ToString());
//                    RaisePropertyChanged("RequestDataLoaded");
//                }
//                catch (TimeoutException e)
//                {
//                    await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//                    return -1;
//                }
//                catch (WebException e)
//                {
//                    await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//                    return -1;
//                }
//            }
//            return 1;
//        }

//        /// <summary>
//        /// Dipsose method
//        /// </summary>
//        public void Dispose()
//        {
//            LsPendingFriendship = null;
//            LsUsers = null;
//            IsDataLoaded = false;
//        }

//        /// <summary>
//        /// Get user picture asynchronously
//        /// </summary>
//        /// <param name="user"></param>
//        public async void GetUserPicture(int id)
//        {
//            try
//            {
//                var image = await _dataService.GetPictureByUser(id);
//                var newImage = image.Replace("\"", "").Replace("\\", "");
//                if (!string.IsNullOrEmpty(image))
//                {
//                    var userToModify = LsUsers.FirstOrDefault(el => el.IdUser == id);
//                    var userToAdd = new ShortUserDTO()
//                    {
//                        FirstName = userToModify.FirstName,
//                        LastName = userToModify.LastName,
//                        IdUser = userToModify.IdUser,
//                        UserPicture = newImage
//                    };
//                    LsUsers.Remove(userToModify);
//                    LsUsers.Add(userToAdd);
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
