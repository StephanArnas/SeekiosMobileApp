//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.View.FragmentView;
//using SeekiosApp.Droid.CustomComponents;
//using SeekiosApp.Droid.Services;
//using SeekiosApp.Droid.Helper;
//using Android.Graphics;
//using System.Globalization;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Model.DTO;
//using System.Threading.Tasks;
//using System.Threading;
//using Android.Text;
//using Android.Text.Style;

//namespace SeekiosApp.Droid.View
//{
//    //[Activity(Theme = "@style/Theme.Community")]
//    //public class ShareSeekiosActivity : AppCompatActivityBase
//    //{

//    //    #region ===== Attribut ====================================================================

//    //    ///<summary>Adapter de la listview de la popup friend</summary>
//    //    private ShareSeekiosFriendListContextMenuAdapter _popupFriendAdapter;

//    //    /// <summary>Adapter de la listview de la popup seekios</summary>
//    //    private ShareSeekiosContextMenuListAdapter _popupSeekiosAdapter;

//    //    /// <summary>Adapter de la listview des amis sélectionnés</summary>
//    //    private ShareSeekiosFriendListAdapter _friendListAdapter;

//    //    /// <summary>Adapter de la listview des amis sélectionnés</summary>
//    //    private ShareSeekiosListSeekiosAdapter _seekiosListAdapter;

//    //    #endregion

//    //    #region ===== Propriétés ==================================================================

//    //    ///<summary>Liste des friends de la popup</summary>
//    //    public ListView ShortFriendPopupListView { get; set; }

//    //    /// <summary>Friend list displayed in the page, showing with which friends I will share a seekios</summary>
//    //    public ListView ShortFriendListView { get; set; }

//    //    ///<summary>Empty Layout de la popup de choix du Seekios</summary>
//    //    public LinearLayout EmptyListLayout { get; set; }

//    //    /// <summary>Friend information text for the sharing</summary>
//    //    public TextView FriendInformationTextView { get; set; }

//    //    /// <summary>Button add friend to share the seekios with</summary>
//    //    public TextView FriendAddButton { get; set; }

//    //    /// <summary>Button add seekios to chose seekios to share</summary>
//    //    public TextView SeekiosAddButton { get; set; }

//    //    /// <summary>List displaying the seekios selected for sharing</summary>
//    //    public ListView ShortSeekiosListView { get; set; }

//    //    ///<summary>Liste des seekios de la popup</summary>
//    //    public ListView ShortSeekiosPopupListView { get; set; }

//    //    /// <summary>Information about the Seekios shared in the information text</summary>
//    //    public TextView SeekiosInformationTextView { get; set; }

//    //    public List<SharingDTO> SharingList { get; set; }

//    //    #endregion

//    //    #region ===== Cycle De Vie ================================================================

//    //    /// <summary>
//    //    /// Création de la page
//    //    /// </summary>
//    //    protected override void OnCreate(Bundle bundle)
//    //    {
//    //        SetContentView(Resource.Layout.ShareSeekiosLayout);
//    //        base.OnCreate(bundle);

//    //        GetObjectsFromView();
//    //        SetDataToView();

//    //        if (ToolbarPage != null)
//    //        {
//    //            SetSupportActionBar(ToolbarPage);
//    //            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// Reprise de la page
//    //    ///  - Enregistre le ContextMenu
//    //    ///  - Rafraîchit les données de la liste
//    //    /// </summary>
//    //    protected override void OnResume()
//    //    {
//    //        base.OnResume();
//    //        FriendAddButton.Click += OnAddFriendButtonClicked;
//    //        SeekiosAddButton.Click += OnAddSeekiosButtonClicked;
//    //    }

//    //    /// <summary>
//    //    /// Suspension de la page
//    //    ///  - Désenregistre le ContextMenu auprès de la vue
//    //    /// </summary>
//    //    protected override void OnPause()
//    //    {
//    //        base.OnPause();
//    //        FriendAddButton.Click -= OnAddFriendButtonClicked;
//    //    }

//    //    /// <summary>
//    //    /// Suppression de la page
//    //    /// </summary>
//    //    protected override void OnDestroy()
//    //    {

//    //        App.ActualTheme = App.THEME_LIGHT;
//    //        App.Locator.ShareSeekios.Dispose();
//    //        App.Locator.ShareSeekios.ShortCheckedUserList.Clear();
//    //        base.OnDestroy();
//    //    }

//    //    #endregion

//    //    #region ===== ActionBar ===================================================================

//    //    /// <summary>
//    //    /// Création de l'action bar
//    //    /// </summary>
//    //    public override bool OnCreateOptionsMenu(IMenu menu)
//    //    {
//    //        MenuInflater.Inflate(Resource.Menu.ShareSeekiosActionBar, menu);
//    //        SetTitle(Resource.String.shareSeekios_title);

//    //        return base.OnCreateOptionsMenu(menu);
//    //    }

//    //    /// <summary>
//    //    /// Sélection d'un bouton de l'action bar
//    //    /// </summary>
//    //    public override bool OnOptionsItemSelected(IMenuItem item)
//    //    {
//    //        // on sauvegarde et créé le partage en bdd
//    //        if (item.ItemId == Resource.Id.menu_save)
//    //        {
//    //            InsertMultipleSharing();
//    //        }
//    //        // on ferme la page (bouton retour)
//    //        else
//    //        {
//    //            Finish();
//    //        }
//    //        return true;
//    //    }

//    //    #endregion

//    //    #region ===== Initialisation Vue ==========================================================

//    //    /// <summary>
//    //    /// Récupère les objets de la vue
//    //    /// </summary>
//    //    private void GetObjectsFromView()
//    //    {
//    //        ToolbarPage = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
//    //        SeekiosInformationTextView = FindViewById<TextView>(Resource.Id.shareSeekios_informationSeekios);
//    //        FriendInformationTextView = FindViewById<TextView>(Resource.Id.shareSeekios_informationFriend);
//    //        FriendAddButton = FindViewById<TextView>(Resource.Id.shareSeekios_friendAddButton);
//    //        ShortFriendListView = FindViewById<ListView>(Resource.Id.shareSeekios_shortFriendList);
//    //        ShortSeekiosListView = FindViewById<ListView>(Resource.Id.shareSeekios_shortSeekiosList);
//    //        SeekiosAddButton = FindViewById<TextView>(Resource.Id.shareSeekios_seekiosAddButton);
//    //    }

//    //    /// <summary>
//    //    /// Initialise les objets de la vue avec les données
//    //    /// </summary>
//    //    private void SetDataToView()
//    //    {
//    //        ToolbarPage.SetBackgroundColor(Resources.GetColor(Resource.Color.communityDarkBlue));

//    //        InitialiseInformativeSeekiosText();
//    //        //InitialiseInformativeFriendText();
//    //        InitialiseFriendListView();
//    //        InitialiseSeekiosListView();
//    //    }

//    //    /// <summary>Initialise la liste des amis liés au partage</summary>
//    //    private void InitialiseFriendListView()
//    //    {
//    //        _friendListAdapter = new ShareSeekiosFriendListAdapter(this);
//    //        ShortFriendListView.Adapter = _friendListAdapter;
//    //        ShortFriendListView.ChoiceMode = ChoiceMode.Single;
//    //        ShortFriendListView.ItemsCanFocus = true;
//    //    }

//    //    /// <summary>Initialise la liste des seekios à partager</summary>
//    //    private void InitialiseSeekiosListView()
//    //    {
//    //        _seekiosListAdapter = new ShareSeekiosListSeekiosAdapter(this);
//    //        ShortSeekiosListView.Adapter = _seekiosListAdapter;
//    //        ShortSeekiosListView.ChoiceMode = ChoiceMode.Single;
//    //        ShortSeekiosListView.ItemsCanFocus = true;
//    //    }

//    //    /// <summary>
//    //    /// Initialise Seekios info textview
//    //    /// </summary>
//    //    private void InitialiseInformativeSeekiosText()
//    //    {
//    //        //Color seekios name
//    //        //var seekiosName = string.Empty;
//    //        //if (string.IsNullOrEmpty(_seekiosSelectedName)) seekiosName = "???";
//    //        //else seekiosName = _seekiosSelectedName;
//    //        //var infoStringSeekios = Resources.GetString(Resource.String.shareSeekios_informationSeekios);
//    //        //var infoTextSeekios = string.Format(infoStringSeekios, seekiosName);

//    //        //var resultTupleSeekios = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(infoStringSeekios, seekiosName);
//    //        //var formattedinfoTextSeekios = new SpannableString(infoTextSeekios);
//    //        //formattedinfoTextSeekios.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.CommunityMainColor)), resultTupleSeekios.Item1, resultTupleSeekios.Item2, 0);
//    //        //SeekiosInformationTextView.SetText(formattedinfoTextSeekios, TextView.BufferType.Spannable);
//    //    }

//    //    /// <summary>
//    //    /// Initialise Friend info textview
//    //    /// </summary>
//    //    private void InitialiseInformativeFriendText()
//    //    {
//    //    }

//    //    #endregion

//    //    #region ===== Méthodes privées ============================================================

//    //    private async void InsertSharing()
//    //    {
//    //        if (App.Locator.ShareSeekios.SeekiosToShareList == null || App.Locator.ShareSeekios.SeekiosToShareList.Count <= 0)
//    //        {
//    //            Toast.MakeText(this, Resource.String.shareSeekios_listSeekiosEmpty, ToastLength.Short);
//    //            return;
//    //        }
//    //        if (App.Locator.ShareSeekios.ShortCheckedUserList == null || App.Locator.ShareSeekios.ShortCheckedUserList.Count <= 0)
//    //        {
//    //            Toast.MakeText(this, Resource.String.shareSeekios_listFriendEmpty, ToastLength.Short);
//    //            return;
//    //        }
//    //        SharingList = new List<SharingDTO>();

//    //        var seekios = App.Locator.ShareSeekios.SeekiosToShareList.FirstOrDefault();
//    //        var user = App.Locator.ShareSeekios.ShortCheckedUserList.FirstOrDefault();
//    //        var friendship = App.CurrentUserEnvironment.LsFriend.FirstOrDefault(el => el.User_IdUser == user.IdUser
//    //                                                                            || el.Friend_IdUser == user.IdUser);
//    //        var sharing = new SharingDTO()
//    //        {
//    //            DateBeginSharing = DateTime.UtcNow,
//    //            DateEndSharing = null,
//    //            Friend_IdUser = friendship.Friend_IdUser,
//    //            User_IdUser = friendship.User_IdUser,
//    //            IsUserOwner = true ? (App.CurrentUserEnvironment.User.IdUser == friendship.User_IdUser) : (App.CurrentUserEnvironment.User.IdUser == friendship.Friend_IdUser),
//    //            Seekios_IdSeekios = seekios.IdSeekios
//    //        };

//    //        switch (await App.Locator.ShareSeekios.InsertSharing(sharing))
//    //        {
//    //            default:
//    //                break;
//    //            case 1:
//    //                Finish();
//    //                break;
//    //            case -1:
//    //                Toast.MakeText(this, Resource.String.shareSeekios_shareNotValid, ToastLength.Short);
//    //                break;
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// 
//    //    /// </summary>
//    //    private async void InsertMultipleSharing()
//    //    {
//    //        if (App.Locator.ShareSeekios.SeekiosToShareList == null || App.Locator.ShareSeekios.SeekiosToShareList.Count <= 0)
//    //        {
//    //            Toast.MakeText(this, Resource.String.shareSeekios_listSeekiosEmpty, ToastLength.Short);
//    //            return;
//    //        }
//    //        if (App.Locator.ShareSeekios.ShortCheckedUserList == null || App.Locator.ShareSeekios.ShortCheckedUserList.Count <= 0)
//    //        {
//    //            Toast.MakeText(this, Resource.String.shareSeekios_listFriendEmpty, ToastLength.Short);
//    //            return;
//    //        }
//    //        SharingList = new List<SharingDTO>();

//    //        foreach (var seekios in App.Locator.ShareSeekios.SeekiosToShareList)
//    //        {
//    //            foreach (var user in App.Locator.ShareSeekios.ShortCheckedUserList)
//    //            {
//    //                var friendship = App.CurrentUserEnvironment.LsFriend.FirstOrDefault(el => el.User_IdUser == user.IdUser
//    //                                                                                    || el.Friend_IdUser == user.IdUser);
//    //                SharingList.Add(new SharingDTO()
//    //                {
//    //                    DateBeginSharing = DateTime.UtcNow,
//    //                    DateEndSharing = null,
//    //                    Friend_IdUser = friendship.Friend_IdUser,
//    //                    User_IdUser = friendship.User_IdUser,
//    //                    IsUserOwner = true ? (App.CurrentUserEnvironment.User.IdUser == friendship.User_IdUser) : (App.CurrentUserEnvironment.User.IdUser == friendship.Friend_IdUser),
//    //                    Seekios_IdSeekios = seekios.IdSeekios
//    //                });
//    //            }
//    //        }

//    //        //var lsSeekios = new List<ShortSeekiosDTO>();
//    //        //foreach (var seekios in App.Locator.ShareSeekios.SeekiosToShareList)
//    //        //{
//    //        //    lsSeekios.Add(new ShortSeekiosDTO()
//    //        //    {
//    //        //        IdSeekios = seekios.IdSeekios,
//    //        //        SeekiosPicture = string.Empty,
//    //        //        SeekiosName = seekios.SeekiosName,
//    //        //        User_iduser = seekios.User_iduser
//    //        //    });
//    //        //}
//    //        //var seekiosSharing = new SeekiosSharingDTO()
//    //        //{
//    //        //    LsSeekios = lsSeekios,
//    //        //    LsSharing = SharingList
//    //        //};

//    //        switch (await App.Locator.ShareSeekios.InsertMultipleSharing(SharingList))
//    //        {
//    //            default:
//    //                break;
//    //            case 1:
//    //                Finish();
//    //                break;
//    //            case -1:
//    //                Toast.MakeText(this, Resource.String.shareSeekios_shareNotValid, ToastLength.Short);
//    //                break;
//    //        }
//    //    }

//    //    /// <summary>
//    //    /// On supprime un partage en bdd
//    //    /// </summary>
//    //    private void DeleteSharing()
//    //    {
//    //        AlertDialog.Builder builder = new AlertDialog.Builder(this);
//    //        builder.SetTitle(Resources.GetString(Resource.String.popup_confirmedChoiceTitle));
//    //        builder.SetMessage(Resources.GetString(Resource.String.listSharing_confirmDelete));
//    //        // Add the buttons
//    //        builder.SetPositiveButton(Resources.GetString(Resource.String.shareSeekios_confirmChoice), new EventHandler<DialogClickEventArgs>(async (o, e) =>
//    //        {
//    //            var sharingSelected = App.Locator.ShareSeekios.SharingSelected;

//    //            if (sharingSelected != null)
//    //            {
//    //                switch (await App.Locator.ShareSeekios.DeleteSharing(sharingSelected))
//    //                {
//    //                    default:
//    //                        Toast.MakeText(this, Resource.String.shareSeekios_sharingCannotBeDeleted, ToastLength.Short).Show();
//    //                        break;
//    //                    case 0:
//    //                        break;
//    //                    case 1:
//    //                        App.Locator.ListSharings.LsSharing.Remove(sharingSelected);
//    //                        Toast.MakeText(this, Resource.String.shareSeekios_sharingDeleted, ToastLength.Short).Show();
//    //                        Finish();
//    //                        break;
//    //                }
//    //            }
//    //            else
//    //            {
//    //                Toast.MakeText(this, Resource.String.shareSeekios_sharingCannotBeDeleted, ToastLength.Short).Show();
//    //                Finish();
//    //            }
//    //        }));
//    //        builder.SetNegativeButton(Resources.GetString(Resource.String.shareSeekios_cancelChoice), new EventHandler<DialogClickEventArgs>((o, e) =>
//    //        {
//    //            Toast.MakeText(this, Resource.String.shareSeekios_deleteCancelled, ToastLength.Short);
//    //        }));

//    //        // Create the AlertDialog
//    //        AlertDialog dialog = builder.Create();
//    //        dialog.Show();
//    //    }

//    //    #endregion

//    //    #region ===== Handlers ====================================================================

//    //    /// <summary>
//    //    /// Add friend to list through a ContextMenu (popup)
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnAddFriendButtonClicked(object sender, EventArgs e)
//    //    {
//    //        if (App.CurrentUserEnvironment.LsFriend.Count > 0)
//    //        {
//    //            RunOnUiThread(() =>
//    //            {
//    //                Dialog listFriendDialog = null;
//    //                var listFriendBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
//    //                var inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
//    //                var view = inflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListLayout, null);

//    //                ShortFriendPopupListView = view.FindViewById<ListView>(Resource.Id.shareSeekiosPopup_shortList);

//    //                var cancelButton = view.FindViewById<TextView>(Resource.Id.shareSeekiosPopup_cancel);
//    //                var acceptButton = view.FindViewById<TextView>(Resource.Id.shareSeekiosPopup_accept);

//    //                cancelButton.Click += (o, ev) =>
//    //                {
//    //                    listFriendDialog.Dismiss();
//    //                };

//    //                acceptButton.Click += (o, ev) =>
//    //                {
//    //                    foreach (var item in App.Locator.ShareSeekios.ShortCheckedUserDictionary)
//    //                    {
//    //                        if (item.Value && !App.Locator.ShareSeekios.ShortCheckedUserList.Contains(item.Key))
//    //                            App.Locator.ShareSeekios.ShortCheckedUserList.Add(item.Key);
//    //                        if (!item.Value && App.Locator.ShareSeekios.ShortCheckedUserList.Contains(item.Key))
//    //                            App.Locator.ShareSeekios.ShortCheckedUserList.Remove(item.Key);
//    //                    }
//    //                    _friendListAdapter.NotifyDataSetChanged();
//    //                    listFriendDialog.Dismiss();
//    //                };

//    //                _popupFriendAdapter = new ShareSeekiosFriendListContextMenuAdapter(this);
//    //                ShortFriendPopupListView.Adapter = _popupFriendAdapter;
//    //                ShortFriendPopupListView.ChoiceMode = ChoiceMode.Single;
//    //                ShortFriendPopupListView.ItemsCanFocus = true;
//    //                ShortFriendPopupListView.Clickable = true;
//    //                ShortFriendPopupListView.ItemClick += OnItemPopupFriendListItemclicked;

//    //                listFriendBuilder.SetView(view);
//    //                listFriendDialog = listFriendBuilder.Create();
//    //                listFriendDialog.Show();
//    //            });
//    //        }
//    //        else Toast.MakeText(this, Resources.GetString(Resource.String.shareSeekios_listFriendEmpty), ToastLength.Short);
//    //    }

//    //    /// <summary>
//    //    /// Add friend to list through a ContextMenu (popup)
//    //    /// </summary>
//    //    /// <param name="sender"></param>
//    //    /// <param name="e"></param>
//    //    private void OnAddSeekiosButtonClicked(object sender, EventArgs e)
//    //    {
//    //        if (App.CurrentUserEnvironment.LsSeekios.Count > 0)
//    //        {
//    //            RunOnUiThread(() =>
//    //            {
//    //                Dialog listSeekiosDialog = null;
//    //                var listSeekiosBuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
//    //                var inflater = (LayoutInflater)this.GetSystemService(Context.LayoutInflaterService);
//    //                var view = inflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListLayout, null);

//    //                ShortSeekiosPopupListView = view.FindViewById<ListView>(Resource.Id.shareSeekiosPopup_shortList);

//    //                var cancelButton = view.FindViewById<TextView>(Resource.Id.shareSeekiosPopup_cancel);
//    //                var acceptButton = view.FindViewById<TextView>(Resource.Id.shareSeekiosPopup_accept);

//    //                cancelButton.Click += (o, ev) =>
//    //                {
//    //                    listSeekiosDialog.Dismiss();
//    //                };

//    //                acceptButton.Click += (o, ev) =>
//    //                {
//    //                    foreach (var item in App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary)
//    //                    {
//    //                        if (item.Value && !App.Locator.ShareSeekios.SeekiosToShareList.Contains(item.Key))
//    //                            App.Locator.ShareSeekios.SeekiosToShareList.Add(item.Key);
//    //                        if (!item.Value && App.Locator.ShareSeekios.SeekiosToShareList.Contains(item.Key))
//    //                            App.Locator.ShareSeekios.SeekiosToShareList.Remove(item.Key);
//    //                    }
//    //                    _seekiosListAdapter.NotifyDataSetChanged();
//    //                    listSeekiosDialog.Dismiss();
//    //                };

//    //                _popupSeekiosAdapter = new ShareSeekiosContextMenuListAdapter(this);
//    //                ShortSeekiosPopupListView.Adapter = _popupSeekiosAdapter;
//    //                ShortSeekiosPopupListView.ChoiceMode = ChoiceMode.Single;
//    //                ShortSeekiosPopupListView.ItemsCanFocus = true;
//    //                ShortSeekiosPopupListView.Clickable = true;
//    //                ShortSeekiosPopupListView.ItemClick += OnItemPopupSeekiosListItemclicked;

//    //                listSeekiosBuilder.SetView(view);
//    //                listSeekiosDialog = listSeekiosBuilder.Create();
//    //                listSeekiosDialog.Show();
//    //            });
//    //        }
//    //        else Toast.MakeText(this, Resources.GetString(Resource.String.shareSeekios_listFriendEmpty), ToastLength.Short);
//    //    }

//    //    private void OnItemPopupFriendListItemclicked(object sender, AdapterView.ItemClickEventArgs e)
//    //    {
//    //        var item = App.Locator.ShareSeekios.ShortCheckedUserDictionary.ElementAt(e.Position);
//    //        App.Locator.ShareSeekios.ShortCheckedUserDictionary[item.Key] = !item.Value;
//    //        _popupFriendAdapter.NotifyDataSetChanged();
//    //    }

//    //    private void OnItemPopupSeekiosListItemclicked(object sender, AdapterView.ItemClickEventArgs e)
//    //    {
//    //        var item = App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary.ElementAt(e.Position);
//    //        App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary[item.Key] = !item.Value;
//    //        _popupSeekiosAdapter.NotifyDataSetChanged();
//    //    }
//    //    #endregion
//    //}
//}