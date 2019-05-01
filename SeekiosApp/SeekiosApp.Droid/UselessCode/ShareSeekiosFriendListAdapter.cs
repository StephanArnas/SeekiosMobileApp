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
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
//using Android.Graphics;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class ShareSeekiosFriendListAdapter : BaseAdapter<ShortUserDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public ShareSeekiosFriendListAdapter(Activity context)
//            : base()
//        {
//            _context = context;
//        }

//        public override ShortUserDTO this[int position]
//        {
//            get
//            {
//                return App.Locator.ShareSeekios.ShortCheckedUserList[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.Locator.ShareSeekios.ShortCheckedUserList.Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            return App.Locator.ShareSeekios.ShortCheckedUserList[position].IdUser;
//        }

//        #endregion

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'item
//            var user = this[position];

//            // On essaie de récupérer la vue potentiellement déjà créé
//            ShareSeekiosFriendListViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(user.IdUser))
//                _alreadyCreatedViews.Add(user.IdUser, null);
//            Android.Views.View view = _alreadyCreatedViews[user.IdUser];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as ShareSeekiosFriendListViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new ShareSeekiosFriendListViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.ShareSeekiosFriendListRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.FriendPicture = view.FindViewById<RoundedImageView>(Resource.Id.shareSeekiosList_image);
//                holder.FriendFormattedName = view.FindViewById<TextView>(Resource.Id.shareSeekiosList_text);

//                if (!string.IsNullOrEmpty(user.UserPicture))
//                {
//                    var bytes = Convert.FromBase64String(user.UserPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    holder.FriendPicture.SetImageBitmap(imageBitmap);
//                    imageBitmap.Dispose();
//                }
//                else holder.FriendPicture.SetImageResource(Resource.Drawable.DefaultSeekios);

//                // nom du seekios
//                holder.FriendFormattedName.Text = string.Format("{0} {1}", user.FirstName, user.LastName);

//                _alreadyCreatedViews[user.IdUser] = view;
//            }
//            return view;
//        }
//    }
//}
