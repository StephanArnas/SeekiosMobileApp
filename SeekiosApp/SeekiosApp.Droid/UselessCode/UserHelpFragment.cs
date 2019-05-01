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

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class UserHelpFragment : Android.Support.V4.App.Fragment
//    {
//        #region ===== Constructeur ================================================================

//        public UserHelpFragment(Android.Graphics.Drawables.Drawable drawable)
//        {
//            _drawable = drawable;
//        }

//        #endregion

//        #region ===== Attributs ===================================================================

//        private Android.Graphics.Drawables.Drawable _drawable;

//        #endregion

//        #region ===== Propriétés =================================================================
//        /// <summary>Liste des Amis de l'utilisateur</summary>
//        public ImageView UserHelpImageView { get; set; }

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
//            var view = inflater.Inflate(Resource.Layout.UserHelperImageView, container, false);
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
//        }

//        /// <summary>
//        /// Suspension de la page
//        ///  - Désenregistre le ContextMenu auprès de la vue
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//        }

//        #endregion

//        #region ===== Initialisation Vue ==========================================================

//        /// <summary>
//        /// Récupère les objets de la vue
//        /// </summary>
//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            UserHelpImageView = view.FindViewById<ImageView>(Resource.Id.userHelperImage);
//        }

//        /// <summary>
//        /// Initialise la vue avec les données
//        /// </summary>
//        private void SetDataToView()
//        {
//            UserHelpImageView.SetImageDrawable(_drawable);
//        }

//        #endregion
//    }
//}