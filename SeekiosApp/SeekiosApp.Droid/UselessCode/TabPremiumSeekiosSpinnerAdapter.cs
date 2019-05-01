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
//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
//using SeekiosApp.Model.DTO;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class TabPremiumSeekiosSpinnerAdapter : BaseAdapter<SeekiosDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private XamSvg.SvgImageView _lastConnectedButtonClicked;

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public TabPremiumSeekiosSpinnerAdapter(Activity context) : base()
//        {
//            _context = context;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override SeekiosDTO this[int position]
//        {
//            get { return App.CurrentUserEnvironment.LsSeekios[position]; }
//        }

//        public override int Count
//        {
//            get { return App.CurrentUserEnvironment.LsSeekios.Count; }
//        }

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // pas de vue à utiliser ? création d'une vue
//            Android.Views.View view = convertView;
//            //TabPremiumSeekiosSpinnerViewHolder holder = null;
//            //// Si on a récupéré la vue on récupère le holder dans son tag
//            //if (view != null) holder = view.Tag as TabPremiumSeekiosSpinnerViewHolder;
//            //// Si le holder n'est pas défini, on le fait et on crée la vue
//            //if (holder == null)
//            //{
//            //    holder = new TabPremiumSeekiosSpinnerViewHolder();

//            //}



//            return convertView;
//        }

//        #endregion
//    }
//}