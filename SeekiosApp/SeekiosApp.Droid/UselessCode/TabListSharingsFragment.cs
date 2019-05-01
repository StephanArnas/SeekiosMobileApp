//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Util;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Model.DTO;
//using com.refractored.fab;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabListSharingsFragment : Android.Support.V4.App.Fragment
//    {

//        public TabListSharingsFragment(List<SharingDTO> LsSharing, bool IsTabOwner)
//        {
//            _lsSharing = LsSharing;
//            _isTabOwner = IsTabOwner;
//        }
//        #region ===== Attributs ===================================================================

//        /// <summary>Adapter de la liste d'amis</summary>
//        private ListSharingsAdapter _adapter = null;

//        /// <summary>List des sharings : owned or shared with me</summary>
//        private List<SharingDTO> _lsSharing = null;

//        /// <summary>True si la table créée est celle de Mes partages, false si c'est celle des seekios partagés avec moi</summary>
//        private bool _isTabOwner = false;

//        #endregion

//        #region ===== Propriétés ==================================================================
//        /// <summary>Liste des Amis de l'utilisateur</summary>
//        public ListView SharingListView { get; set; }

//        /// <summary>Layout utilisé lorsque l'utilisateur n'a aucun ami</summary>&²
//        public LinearLayout EmptyRequestListLinearLayout { get; set; }

//        /// <summary>Bouton floatant d'ajout</summary>
//        public FloatingActionButton ShareFloatingActionButton { get; set; }

//        /// <summary>TextView affiché quand la liste est vide</summary>
//        public TextView EmptyTextView { get; set; }

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
//            var view = inflater.Inflate(Resource.Layout.TabListSharingsLayout, container, false);

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

//            //// On pourrait simplifier tout ça sans vérifier au préalable quelle liste on met à jour et en faisant simplement
//            //// un appel à notifyDataSetChanged
//            //if (App.Locator.TabMyListSharings.HasBeenUpdated == true && App.Locator.ShareSeekios.IsOwnSharing == true && _isTabOwner == true)
//            //{
//            //    _lsSharing = App.Locator.ListSharings.LsOwnSharing;
//            //    _adapter.NotifyDataSetChanged();
//            //    App.Locator.TabMyListSharings.HasBeenUpdated = false;
//            //}
//            //else if (App.Locator.TabMyListSharings.HasBeenUpdated == true && App.Locator.ShareSeekios.IsOwnSharing == false && _isTabOwner == false)
//            //{
//            //    _lsSharing = App.Locator.ListSharings.LsOtherSharing;
//            //    _adapter.NotifyDataSetChanged();
//            //    App.Locator.TabMyListSharings.HasBeenUpdated = false;
//            //}
//            _adapter.NotifyDataSetChanged();
//            if (_isTabOwner)
//                RegisterForContextMenu(SharingListView);
//            ShareFloatingActionButton.Click += OnShareFloatingActionButtonClick;
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre le ContextMenu auprès de la vue
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            if (_isTabOwner)
//                UnregisterForContextMenu(SharingListView);
//            ShareFloatingActionButton.Click -= OnShareFloatingActionButtonClick;
//        }

//        #endregion

//        #region ===== Liste Seekios ContextMenu ===================================================

//        /// <summary>
//        /// Création du contexte menu
//        /// </summary>
//        public override void OnCreateContextMenu(IContextMenu menu, Android.Views.View v, IContextMenuContextMenuInfo menuInfo)
//        {
//            base.OnCreateContextMenu(menu, v, menuInfo);
//            MenuInflater inflater = Activity.MenuInflater;
//            inflater.Inflate(Resource.Menu.ListSharingsMenu, menu);

//            if (v.Id == Resource.Id.listSharing_listView)
//            {
//                AdapterView.AdapterContextMenuInfo info = (AdapterView.AdapterContextMenuInfo)menuInfo;
//                menu.SetHeaderTitle(string.Format("{0} : {1}", App.Locator.ListSharings.FormattedFriendName,
//                    App.Locator.ListSeekios.LsSeekios.FirstOrDefault(el => el.Idseekios == App.Locator.ShareSeekios.SharingSelected.Seekios_IdSeekios).SeekiosName));
//            }
//        }

//        /// <summary>
//        /// Sélection d'un Seekios dans le contexte menu
//        /// </summary>
//        public override bool OnContextItemSelected(IMenuItem item)
//        {
//            switch (item.ItemId)
//            {
//                case Resource.Id.listSharings_deleteSharing:
//                    var sharingSelected = App.Locator.ShareSeekios.SharingSelected;
//                    App.Locator.ListSharings.DeleteSharing(sharingSelected);
//                    _adapter.NotifyDataSetChanged();
//                    return true;
//                default:
//                    return base.OnContextItemSelected(item);
//            }
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            EmptyRequestListLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.listSharing_emptyLayout);
//            SharingListView = view.FindViewById<ListView>(Resource.Id.listSharing_listView);
//            EmptyTextView = view.FindViewById<TextView>(Resource.Id.listSharing_emptyListTextView);
//            ShareFloatingActionButton = view.FindViewById<FloatingActionButton>(Resource.Id.listSharing_floatingActionButton);
//        }

//        /// <summary>
//        /// Initialise la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            InitializeListView();

//            if (_isTabOwner)
//            {
//                ShareFloatingActionButton.Visibility = ViewStates.Visible;
//                EmptyTextView.Text = Resources.GetString(Resource.String.tabListOwnSharings_emptyList);
//            }
//            else
//            {
//                ShareFloatingActionButton.Visibility = ViewStates.Gone;
//                EmptyTextView.Text = Resources.GetString(Resource.String.tabListOtherSharings_emptyList);
//            }
//        }

//        /// <summary>
//        /// Initialise la liste des amis
//        /// </summary>
//        private void InitializeListView()
//        {
//            _adapter = new ListSharingsAdapter(Activity, _isTabOwner);
//            SharingListView.Adapter = _adapter;
//            SharingListView.EmptyView = EmptyRequestListLinearLayout;
//            SharingListView.ChoiceMode = ChoiceMode.Single;
//            SharingListView.ItemsCanFocus = true;
//        }

//        #endregion

//        #region ===== Méthodes privées ============================================================


//        #endregion

//        #region ===== Evènement ===================================================================

//        /// <summary>
//        /// Gère le click sur le floating button
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnShareFloatingActionButtonClick(object sender, EventArgs e)
//        {
//            App.Locator.ListSharings.GoToShareSeekios();
//        }


//        #endregion
//    }
//}