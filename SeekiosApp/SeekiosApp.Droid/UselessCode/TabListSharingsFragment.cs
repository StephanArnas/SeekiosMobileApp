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

//        /// <summary>True si la table cr��e est celle de Mes partages, false si c'est celle des seekios partag�s avec moi</summary>
//        private bool _isTabOwner = false;

//        #endregion

//        #region ===== Propri�t�s ==================================================================
//        /// <summary>Liste des Amis de l'utilisateur</summary>
//        public ListView SharingListView { get; set; }

//        /// <summary>Layout utilis� lorsque l'utilisateur n'a aucun ami</summary>&�
//        public LinearLayout EmptyRequestListLinearLayout { get; set; }

//        /// <summary>Bouton floatant d'ajout</summary>
//        public FloatingActionButton ShareFloatingActionButton { get; set; }

//        /// <summary>TextView affich� quand la liste est vide</summary>
//        public TextView EmptyTextView { get; set; }

//        #endregion

//        #region ===== Cycle De Vie ================================================================
//        /// <summary>
//        /// Cr�ation de la page
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
//        ///  - Rafra�chit les donn�es de la liste
//        /// </summary>
//        public override void OnResume()
//        {
//            base.OnResume();

//            //// On pourrait simplifier tout �a sans v�rifier au pr�alable quelle liste on met � jour et en faisant simplement
//            //// un appel � notifyDataSetChanged
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
//        ///  - D�senregistre le ContextMenu aupr�s de la vue
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
//        /// Cr�ation du contexte menu
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
//        /// S�lection d'un Seekios dans le contexte menu
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
//        /// R�cup�re les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            EmptyRequestListLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.listSharing_emptyLayout);
//            SharingListView = view.FindViewById<ListView>(Resource.Id.listSharing_listView);
//            EmptyTextView = view.FindViewById<TextView>(Resource.Id.listSharing_emptyListTextView);
//            ShareFloatingActionButton = view.FindViewById<FloatingActionButton>(Resource.Id.listSharing_floatingActionButton);
//        }

//        /// <summary>
//        /// Initialise la vue avec les donn�es
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

//        #region ===== M�thodes priv�es ============================================================


//        #endregion

//        #region ===== Ev�nement ===================================================================

//        /// <summary>
//        /// G�re le click sur le floating button
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