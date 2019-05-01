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
//    public class ShareSeekiosContextMenuListAdapter : BaseAdapter<ShortSeekiosDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public ShareSeekiosContextMenuListAdapter(Activity context)
//            : base()
//        {
//            _context = context;
//        }

//        public override ShortSeekiosDTO this[int position]
//        {
//            get
//            {
//                var lsSeekios = App.CurrentUserEnvironment.LsSeekios.Where(el => el.User_iduser == App.CurrentUserEnvironment.User.IdUser).OrderBy(el => el.SeekiosName).ToList();
//                var seekios = lsSeekios[position];
//                return new ShortSeekiosDTO()
//                {
//                    IdSeekios = seekios.Idseekios,
//                    SeekiosName = seekios.SeekiosName,
//                    SeekiosPicture = seekios.SeekiosPicture,
//                    User_iduser = seekios.User_iduser.ToString()
//                };
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.LsSeekios.Where(el => el.User_iduser == App.CurrentUserEnvironment.User.IdUser).OrderBy(el => el.SeekiosName).ToList().Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            return App.CurrentUserEnvironment.LsSeekios.Where(el => el.User_iduser == App.CurrentUserEnvironment.User.IdUser).OrderBy(el => el.SeekiosName).ToList()[position].Idseekios;
//        }

//        #endregion

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'item
//            var item = this[position];

//            // On essaie de récupérer la vue potentiellement déjà créé
//            ShareSeekiosContextMenuListViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(item.IdSeekios))
//                _alreadyCreatedViews.Add(item.IdSeekios, null);
//            Android.Views.View view = _alreadyCreatedViews[item.IdSeekios];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as ShareSeekiosContextMenuListViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new ShareSeekiosContextMenuListViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.SeekiosPicture = view.FindViewById<RoundedImageView>(Resource.Id.customListRow_image);
//                holder.SeekiosName = view.FindViewById<TextView>(Resource.Id.customListRow_text);
//                holder.CheckBox = view.FindViewById<CheckBox>(Resource.Id.customListRow_checkBox);
//                holder.CheckBox.SetBackgroundColor(Color.Gray);

//                if (!string.IsNullOrEmpty(item.SeekiosPicture))
//                {
//                    var bytes = Convert.FromBase64String(item.SeekiosPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    holder.SeekiosPicture.SetImageBitmap(imageBitmap);
//                    imageBitmap.Dispose();
//                }
//                else holder.SeekiosPicture.SetImageResource(Resource.Drawable.DefaultUser);

//                // nom du seekios
//                holder.SeekiosName.Text = item.SeekiosName;
//                // add object to dictionary : handling checked box
//                if (!App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary.ContainsKey(item)) App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary.Add(item, false);

//                _alreadyCreatedViews[item.IdSeekios] = view;
//            }
//            if (App.Locator.ShareSeekios.ShortCheckedSeekiosDictionary.ElementAt(position).Value)
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