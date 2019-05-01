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
//using Java.Lang;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class ShareSeekiosFriendListContextMenuAdapter : BaseAdapter<ShortUserDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public ShareSeekiosFriendListContextMenuAdapter(Activity context)
//            : base()
//        {
//            _context = context;
//        }

//        public override ShortUserDTO this[int position]
//        {
//            get
//            {
//                var user = App.CurrentUserEnvironment.LsFriend[position];
//                return new ShortUserDTO()
//                {
//                    FirstName = user.FirstName,
//                    IdUser = user.IdUser,
//                    LastName = user.LastName,
//                    UserPicture = user.UserPicture
//                };
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.LsFriend.Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            return App.CurrentUserEnvironment.LsFriend[position].IdUser;
//        }

//        #endregion

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'item
//            var user = this[position];

//            // On essaie de récupérer la vue potentiellement déjà créé
//            ShareSeekiosFriendListContextMenuViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(user.IdUser))
//                _alreadyCreatedViews.Add(user.IdUser, null);
//            Android.Views.View view = _alreadyCreatedViews[user.IdUser];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as ShareSeekiosFriendListContextMenuViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new ShareSeekiosFriendListContextMenuViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.UserPicture = view.FindViewById<RoundedImageView>(Resource.Id.customListRow_image);
//                holder.UserFormattedName = view.FindViewById<TextView>(Resource.Id.customListRow_text);
//                holder.CheckBox = view.FindViewById<CheckBox>(Resource.Id.customListRow_checkBox);
//                holder.CheckBox.SetBackgroundColor(Color.Gray);

//                if (!string.IsNullOrEmpty(user.UserPicture))
//                {
//                    var bytes = Convert.FromBase64String(user.UserPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    holder.UserPicture.SetImageBitmap(imageBitmap);
//                    imageBitmap.Dispose();
//                }
//                else holder.UserPicture.SetImageResource(Resource.Drawable.DefaultUser);

//                // nom du seekios
//                holder.UserFormattedName.Text = string.Format("{0} {1}", user.FirstName, user.LastName);
//                // add object to dictionary : handling checked box
//                if (!App.Locator.ShareSeekios.ShortCheckedUserDictionary.ContainsKey(user)) App.Locator.ShareSeekios.ShortCheckedUserDictionary.Add(user, false);

//                _alreadyCreatedViews[user.IdUser] = view;
//            }
//            if (App.Locator.ShareSeekios.ShortCheckedUserDictionary.ElementAt(position).Value)
//            {
//                holder.CheckBox.Checked = true;
//            }
//            else
//            {
//                holder.CheckBox.Checked = false;
//            }

//            return view;
//        }
//    }
//}