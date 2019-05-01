//using GalaSoft.MvvmLight;
//using GalaSoft.MvvmLight.Views;
//using Microsoft.Practices.ServiceLocation;
//using SeekiosApp.Interfaces;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Properties;
//using SeekiosApp.Services;
//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SeekiosApp.ViewModel
//{
//    public class AddFriendViewModel : ViewModelBase
//    {
//        //    #region ===== Constructeur ================================================================

//        public AddFriendViewModel(IDataService dataService
//            , IDialogService dialogService)
//        {
//            //_dataService = dataService;
//            //_dialogService = dialogService;
//            //_navigationService = ServiceLocator.Current.GetInstance<INavigationService>();
//            //FriendSearchList = new List<ShortUserDTO>();
//        }

//        //    #endregion

//        //    #region ===== Attributs ===================================================================

//        //    /// <summary>Navigation service</summary>
//        //    private INavigationService _navigationService;

//        //    /// <summary>Dialog service</summary>
//        //    private IDialogService _dialogService;

//        //    /// <summary>Data access service</summary>
//        //    private IDataService _dataService;

//        //    /// <summary>Short user search list</summary>
//        //    private List<ShortUserDTO> _friendSearchList;

//        //    /// <summary>User found and picture loaded register</summary>
//        //    private Dictionary<int, string> _userDictionnary = new Dictionary<int, string>();

//        //    #endregion

//        //    #region ===== Propriétés ==================================================================

//        //    /// <summary>
//        //    /// Friend list created after searching
//        //    /// </summary>
//        //    public List<ShortUserDTO> FriendSearchList
//        //    {
//        //        get
//        //        {
//        //            return _friendSearchList;
//        //        }
//        //        set
//        //        {
//        //            _friendSearchList = value;
//        //        }
//        //    }

//        //    /// <summary>
//        //    /// User get after researching by email or phone
//        //    /// </summary>
//        //    public ShortUserDTO UserSearchResult { get; set; }

//        //    #endregion

//        //    #region ===== Méthodes Privées ============================================================

//        //    #endregion

//        //    #region ===== Méthodes Publiques ==========================================================

//        //    /// <summary>
//        //    /// Get users picture asynchronously
//        //    /// </summary>
//        //    public async void GetPictureByUser()
//        //    {
//        //        try
//        //        {
//        //            if (App.Locator.AddFriend.FriendSearchList != null)
//        //            {
//        //                foreach (var friend in App.Locator.AddFriend.FriendSearchList)
//        //                {
//        //                    if (!_userDictionnary.ContainsKey(friend.IdUser))
//        //                    {
//        //                        _userDictionnary.Add(friend.IdUser, string.Empty);
//        //                        try
//        //                        {
//        //                            var image = await _dataService.GetPictureByUser(friend.IdUser);
//        //                            if (!string.IsNullOrEmpty(image) && image.Length > 10)
//        //                            {
//        //                                friend.UserPicture = image.Replace("\"", "").Replace("\\", ""); //TODO: tavucbo
//        //                                RaisePropertyChanged("PictureLoaded");
//        //                            }
//        //                            _userDictionnary[friend.IdUser] = friend.UserPicture;
//        //                        }
//        //                        catch (WebException e)
//        //                        {
//        //                            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //                        }
//        //                        catch (TimeoutException e)
//        //                        {
//        //                            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //                        }
//        //                    }
//        //                    else
//        //                    {
//        //                        var imageRegistered = string.Empty;
//        //                        _userDictionnary.TryGetValue(friend.IdUser, out imageRegistered);
//        //                        friend.UserPicture = imageRegistered;
//        //                        RaisePropertyChanged("PictureLoaded");
//        //                    }
//        //                }
//        //            }
//        //        }
//        //        catch (WebException e)
//        //        {
//        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (TimeoutException e)
//        //        {
//        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        return;
//        //    }

//        //    /// <summary>
//        //    /// Send friendship request
//        //    /// </summary>
//        //    /// <param name="friendship">friendship to add</param>
//        //    public async Task<int> SendFriendshipRequest(FriendshipDTO friendship, ShortUserDTO user)
//        //    {
//        //        try
//        //        {
//        //            var result = await _dataService.InsertFriendship(friendship);
//        //            if (result == -1)
//        //                RaisePropertyChanged("RequestNotSent");
//        //            else if (result == 1)
//        //            {
//        //                App.Locator.TabRequests.LsPendingFriendship.Add(friendship);
//        //                App.Locator.TabRequests.LsUsers.Add(user);
//        //                RaisePropertyChanged("RequestSent");
//        //            }
//        //            return result;
//        //        }
//        //        catch (WebException e)
//        //        {
//        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (TimeoutException e)
//        //        {
//        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (UnauthorizedAccessException e)
//        //        {
//        //            await _dialogService.ShowError(e, e.Source, e.Message, null);
//        //        }
//        //        return 0;
//        //    }

//        //    /// <summary>
//        //    /// Search users by name to add one of them as friend in database
//        //    /// </summary>
//        //    /// <param name="search">search string</param>
//        //    public async Task<List<ShortUserDTO>> GetUsersByNameSearch(string search)
//        //    {
//        //        try
//        //        {
//        //            if (string.IsNullOrEmpty(search) || search.Length <= 2)
//        //                return null;

//        //            var result = await _dataService.GetUsersByNameSearch(search);
//        //            foreach (var item in result)
//        //            {
//        //                if (item.IdUser == App.CurrentUserEnvironment.User.IdUser)
//        //                {
//        //                    result.Remove(item);
//        //                    break;
//        //                }
//        //            }
//        //            return result;
//        //        }
//        //        catch (WebException e)
//        //        {
//        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (TimeoutException e)
//        //        {
//        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (UnauthorizedAccessException e)
//        //        {
//        //            await _dialogService.ShowError(e, e.Source, e.Message, null);
//        //        }
//        //        return null;
//        //    }

//        //    /// <summary>
//        //    /// Get a user thanks to his email
//        //    /// </summary>
//        //    /// <param name="search">email searched</param>
//        //    public async Task<ShortUserDTO> GetUserByEmail(string search)
//        //    {
//        //        try
//        //        {
//        //            if (string.IsNullOrEmpty(search) || search.Length <= 2)
//        //                return null;

//        //            var result = await _dataService.GetUserByEmail(search);
//        //            if (result == null || result.IdUser == App.CurrentUserEnvironment.User.IdUser) return null;
//        //            else return result;
//        //        }
//        //        catch (WebException e)
//        //        {
//        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (TimeoutException e)
//        //        {
//        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (UnauthorizedAccessException e)
//        //        {
//        //            await _dialogService.ShowError(e, e.Source, e.Message, null);
//        //        }
//        //        return null;
//        //    }

//        //    /// <summary>
//        //    /// Get a user thanks to his phone number
//        //    /// </summary>
//        //    /// <param name="search">phone number searched</param>
//        //    public async Task<ShortUserDTO> GetUserByPhone(string search)
//        //    {
//        //        try
//        //        {
//        //            if (string.IsNullOrEmpty(search) || search.Length <= 2)
//        //                return null;

//        //            var result = await _dataService.GetUserByPhone(search);
//        //            if (result == null || result.IdUser == App.CurrentUserEnvironment.User.IdUser) return null;
//        //            else return result;
//        //        }
//        //        catch (WebException e)
//        //        {
//        //            await _dialogService.ShowError(Resources.TimeoutError, Resources.WebErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (TimeoutException e)
//        //        {
//        //            await _dialogService.ShowError(e, Resources.TimeoutErrorTitle, Resources.WebErrorButtonText, null);
//        //        }
//        //        catch (UnauthorizedAccessException e)
//        //        {
//        //            await _dialogService.ShowError(e, e.Source, e.Message, null);
//        //        }
//        //        return null;
//        //    }
//        //    #endregion
//    }
//}
