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

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabRequestsFragment : Android.Support.V4.App.Fragment
//    {
//        #region ===== Attributs ===================================================================

//        /// <summary>Adapter de la liste d'amis</summary>
//        private TabRequestsAdapter _requestAdapter = null;

//        #endregion

//        #region ===== Propriétés =================================================================
//        /// <summary>Liste des Amis de l'utilisateur</summary>
//        public ListView RequestListView { get; set; }

//        /// <summary>Layout utilisé lorsque l'utilisateur n'a aucun ami</summary>
//        public LinearLayout EmptyRequestListLinearLayout { get; set; }

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
//            var view = inflater.Inflate(Resource.Layout.TabRequestsFragmentLayout, container, false);

//            GetObjectsFromView(view);
//            LoadData();

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
//            if (_requestAdapter != null) _requestAdapter.NotifyDataSetChanged();
//            else _requestAdapter = new TabRequestsAdapter(this);
//            App.Locator.TabRequests.PropertyChanged += OnRequestDataLoaded;
//            RegisterForContextMenu(RequestListView);
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre le ContextMenu auprès de la vue
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            App.Locator.TabRequests.PropertyChanged -= OnRequestDataLoaded;
//            UnregisterForContextMenu(RequestListView);
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            EmptyRequestListLinearLayout = view.FindViewById<LinearLayout>(Resource.Id.tabRequest_emptyLayout);
//            RequestListView = view.FindViewById<ListView>(Resource.Id.tabRequest_requestList);
//        }

//        /// <summary>
//        /// Initialise la liste des amis
//        /// </summary>
//        private void InitializeListView()
//        {
//            _requestAdapter = new TabRequestsAdapter(this);
//            RequestListView.Adapter = _requestAdapter;
//            RequestListView.EmptyView = EmptyRequestListLinearLayout;
//            RequestListView.ChoiceMode = ChoiceMode.Single;
//            RequestListView.ItemsCanFocus = true;
//        }

//        #endregion

//        #region ===== Méthodes privées ============================================================


//        /// <summary>
//        /// Load friendship requests
//        /// </summary>
//        private void LoadData()
//        {
//            InitializeListView();
//        }

//        /// <summary>
//        /// Met à jour la liste view quand les données sont chargées
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private void OnRequestDataLoaded(object sender, System.ComponentModel.PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == "RequestDataLoaded" || e.PropertyName == "RequestDeleted"
//                    || e.PropertyName == "RequestAccepted" || e.PropertyName == "RequestSent")
//                _requestAdapter.NotifyDataSetChanged();
//        }

//        #endregion
//    }
//}