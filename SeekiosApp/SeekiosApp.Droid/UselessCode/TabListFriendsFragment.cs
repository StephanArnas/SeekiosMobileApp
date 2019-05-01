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
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Model.DTO;
//using com.refractored.fab;
//using SeekiosApp.Droid.Helper;
//using Android.Text;
//using Android.Text.Style;
//using Android.Graphics;
//using Android.Database;
//using Android.Support.V7.Widget;
//using Java.Lang;
//using System.Net;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabListFriendsFragment : Android.Support.V4.App.Fragment
//    {

//        #region ===== Attributs ===================================================================
//        /// <summary>Adapter de la liste d'amis</summary>
//        private TabListFriendsAdapter _friendAdapter = null;

//        /// <summary>Ami sélectionné dans la liste des amis</summary>
//        private FriendUserDTO _selectedUser = null;

//        #endregion

//        #region ===== Propriétés =================================================================
//        /// <summary>Liste des Amis de l'utilisateur</summary>
//        public ListView FriendsListView { get; set; }

//        /// <summary>Layout utilisé lorsque l'utilisateur n'a aucun ami</summary>
//        public LinearLayout EmptyFriendListLinearLayout { get; set; }

//        ///<summary>Bouton flottant pour ajouter un ami</summary>
//        public FloatingActionButton AddFriendFloatingButton { get; set; }

//        ///<summary>TextView Title (nombres d'amis total)</summary>
//        public TextView TitleTextView { get; set; }

//        #endregion

//        #region ===== Cycle De Vie ================================================================
//        /// <summary>
//        /// Création de la page
//        /// </summary>
//        public override void OnCreate(Bundle savedInstanceState)
//        {
//            base.OnCreate(savedInstanceState);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="inflater"></param>
//        /// <param name="container"></param>
//        /// <param name="savedInstanceState"></param>
//        /// <returns></returns>
//        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            var view = inflater.Inflate(Resource.Layout.TabListFriendsFragmentLayout, container, false);

//            GetObjectsFromView(view);
//            SetDataToView();

//            return view;
//        }

//        /// <summary>
//        /// Reprise de la page
//        ///  - Enregistre le ContextMenu
//        ///  - Rafraîchit les données de la liste
//        /// </summary>
//        public override void OnResume()
//        {
//            base.OnResume();
//            LoadData();
//            EmptyFriendListLinearLayout.Click += OnEmptyFriendListClick;
//            AddFriendFloatingButton.Click += OnAddFriendFloatingButtonClick;
//            App.Locator.TabRequests.PropertyChanged += OnFriendshipRequestAccepted;
//            App.Locator.TabListFriends.PropertyChanged += OnFriendListPictureLoaded;
//            RegisterForContextMenu(FriendsListView);
//            // mise à jour de la liste
//            _friendAdapter.NotifyDataSetChanged();
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre le ContextMenu auprès de la vue
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            EmptyFriendListLinearLayout.Click -= OnEmptyFriendListClick;
//            AddFriendFloatingButton.Click -= OnAddFriendFloatingButtonClick;
//            App.Locator.TabRequests.PropertyChanged -= OnFriendshipRequestAccepted;
//            App.Locator.TabListFriends.PropertyChanged -= OnFriendListPictureLoaded;
//            UnregisterForContextMenu(FriendsListView);
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            EmptyFriendListLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.friend_emptyLayout);
//            FriendsListView = view.FindViewById<ListView>(Resource.Id.user_friendList);
//            AddFriendFloatingButton = view.FindViewById<FloatingActionButton>(Resource.Id.listFriend_floatingActionButton);
//            TitleTextView = view.FindViewById<TextView>(Resource.Id.listFriend_title);
//        }

//        /// <summary>
//        /// Initialise la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            InitializeListView();
//            InitializeNumberOfFriendsTextView();

//            // rend visible le bouton flottant
//            AddFriendFloatingButton.AttachToListView(FriendsListView);
//            AddFriendFloatingButton.Show();

//        }

//        /// <summary>
//        /// Initialise la liste des amis
//        /// </summary>
//        private void InitializeListView()
//        {
//            _friendAdapter = new TabListFriendsAdapter(this);
//            FriendsListView.Adapter = _friendAdapter;
//            FriendsListView.EmptyView = EmptyFriendListLinearLayout;
//            FriendsListView.ChoiceMode = ChoiceMode.Single;
//            FriendsListView.ItemsCanFocus = true;
//        }

//        /// <summary>
//        /// Initialise le TextView du nombre d'amis
//        /// </summary>
//        private void InitializeNumberOfFriendsTextView()
//        {
//            var friendNbr = App.Locator.TabListFriends.LsFriends.Count;
//            var textFriendNbr = string.Empty;

//            if (friendNbr < 2)
//                textFriendNbr = string.Format(Resources.GetString(Resource.String.listFriend_friendsNbrSingular), friendNbr);

//            else textFriendNbr = string.Format(Resources.GetString(Resource.String.listFriend_friendsNbrPlural), friendNbr);

//            var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfNumberInString(textFriendNbr);
//            var formattedTextFriendNbr = new SpannableString(textFriendNbr);
//            formattedTextFriendNbr.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.CommunityColor)), resultTuple.Item1, resultTuple.Item2, 0);
//            TitleTextView.SetText(formattedTextFriendNbr, TextView.BufferType.Spannable);
//        }

//        #endregion

//        #region ===== Liste Friends ContextMenu ===================================================

//        /// <summary>
//        /// Création du contexte menu
//        /// </summary>
//        public override void OnCreateContextMenu(IContextMenu menu, Android.Views.View v, IContextMenuContextMenuInfo menuInfo)
//        {
//            base.OnCreateContextMenu(menu, v, menuInfo);
//            MenuInflater inflater = base.Activity.MenuInflater;
//            inflater.Inflate(Resource.Menu.ListFriendMenu, menu);

//            if (v.Id == Resource.Id.user_friendList)
//            {
//                AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo)menuInfo;
//                _selectedUser = App.Locator.TabListFriends.LsFriends[info.Position];
//                menu.SetHeaderTitle(string.Format("{0} {1}", _selectedUser.FirstName, _selectedUser.LastName));
//            }
//        }

//        /// <summary>
//        /// Sélection d'un Seekios dans le contexte menu
//        /// </summary>
//        public override bool OnContextItemSelected(IMenuItem item)
//        {
//            switch (item.ItemId)
//            {
//                case Resource.Id.listFriendMenu_deleteFriendship:
//                    App.Locator.TabListFriends.DeleteFriendship(App.CurrentUserEnvironment.User.IdUser, _selectedUser.IdUser);
//                    InitializeNumberOfFriendsTextView();
//                    _friendAdapter.RefreshHeaders();
//                    _friendAdapter.NotifyDataSetChanged();
//                    return true;
//                default:
//                    return base.OnContextItemSelected(item);
//            }
//        }

//        #endregion

//        #region ===== Méthodes privées ============================================================


//        /// <summary>
//        /// Charge les images des amis en asynchrone
//        /// </summary>
//        private async void LoadData()
//        {
//            await App.Locator.TabListFriends.GetPictureByUser();
//            await App.Locator.TabRequests.InitialiseData();
//        }

//        /// <summary>
//        ///Aller à ListShareSeekios quand un item est cliqué
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnFriendsListViewClick(object sender, EventArgs e)
//        {
//            var item = sender as ShortUserDTO;
//            App.Locator.TabListFriends.GoToListShareSeekios();
//        }

//        #endregion

//        #region ===== Évènement ===================================================================

//        /// <summary>
//        /// Aller à l'ajout d'un ami si on n'en a pas
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnEmptyFriendListClick(object sender, EventArgs e)
//        {
//            App.Locator.TabListFriends.GoToAddFriend();
//        }

//        /// <summary>
//        /// Aller à l'ajout d'un ami si on n'en a pas
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnAddFriendFloatingButtonClick(object sender, EventArgs e)
//        {
//            App.Locator.TabListFriends.GoToAddFriend();
//        }

//        /// <summary>
//        /// Evenement click dans la popup pour supprimer un Seekios
//        /// </summary>
//        private EventHandler<DialogClickEventArgs> OnPopupDeleteConfirmed()
//        {
//            EventHandler<DialogClickEventArgs> confirmedPopupClick = new EventHandler<DialogClickEventArgs>((s, args) =>
//            {
//                App.Locator.TabListFriends.DeleteFriendship(App.CurrentUserEnvironment.User.IdUser, _selectedUser.IdUser);
//                _friendAdapter.NotifyDataSetChanged();
//            });
//            return confirmedPopupClick;
//        }

//        /// <summary>
//        /// Update la liste d'amis si une requête d'ami est acceptée
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnFriendshipRequestAccepted(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == "RequestAccepted")
//            {
//                _friendAdapter.RefreshHeaders();
//                InitializeNumberOfFriendsTextView();
//                _friendAdapter.NotifyDataSetChanged();
//            }
//        }

//        /// <summary>
//        /// Notify pictures have been loaded
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnFriendListPictureLoaded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == "LsFriend")
//                _friendAdapter.NotifyDataSetChanged();
//        }

//        #endregion

//        #region ===== Popups ======================================================================

//        /// <summary>
//        /// Ouvre une popup qui demande de confirmer le choix sélectionné
//        /// </summary>
//        private void ConfirmedDeleteChoice()
//        {
//            Dialog ChoiceConfirmedDialog = null;
//            var ChoiceConfirmedConfirmedDialog = new Android.Support.V7.App.AlertDialog.Builder(this.Activity);

//            // titre de la popup
//            ChoiceConfirmedConfirmedDialog.SetTitle(Resources.GetString(Resource.String.popup_confirmedChoiceTitle));

//            // bouton confirmer 
//            ChoiceConfirmedConfirmedDialog.SetPositiveButton(Resources.GetString(Resource.String.popup_confirmed), OnPopupDeleteConfirmed());

//            // bouton annuler 
//            ChoiceConfirmedConfirmedDialog.SetNegativeButton(Resources.GetString(Resource.String.popup_cancelled), delegate
//            {
//                ChoiceConfirmedDialog.Dismiss();
//            });

//            ChoiceConfirmedDialog = ChoiceConfirmedConfirmedDialog.Create();
//            ChoiceConfirmedDialog.Show();
//        }

//        #endregion
//    }
//}