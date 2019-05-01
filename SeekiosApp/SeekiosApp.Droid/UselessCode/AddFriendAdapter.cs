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
//using SeekiosApp.Droid.Helper;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class AddFriendAdapter : BaseAdapter<ShortUserDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        private Activity _context;

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public AddFriendAdapter(Activity context)
//        {
//            _context = context;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override ShortUserDTO this[int position]
//        {
//            get
//            {
//                return App.Locator.AddFriend.FriendSearchList[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.Locator.AddFriend.FriendSearchList.Count;
//            }
//        }

//        /// <summary>
//        /// Returns the user ID at the index [position] of the list
//        /// </summary>
//        /// <param name="position">index</param>
//        /// <returns></returns>
//        public override long GetItemId(int position)
//        {
//            return this[position].IdUser;
//        }

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            //get item
//            var item = this[position];

//            // pas de vue à utiliser ? création d'une vue
//            AddFriendViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(item.IdUser))
//                _alreadyCreatedViews.Add(item.IdUser, null);
//            Android.Views.View view = _alreadyCreatedViews[item.IdUser];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as AddFriendViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new AddFriendViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.AddFriendRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.FriendPicture = view.FindViewById<RoundedImageView>(Resource.Id.addFriend_userImage);
//                holder.FriendFormattedName = view.FindViewById<TextView>(Resource.Id.addFriend_nameText);
//                holder.InformationImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.addFriendRow_image);
//                holder.InformationTextView = view.FindViewById<TextView>(Resource.Id.addfriend_infoTextView);

//                holder.InformationImageView.Click += new EventHandler(async (e, o) =>
//                {
//                    var friendship = new FriendshipDTO()
//                    {
//                        User_IdUser = App.CurrentUserEnvironment.User.IdUser,
//                        Friend_IdUser = item.IdUser,
//                        DateFriendshipCreation = DateTime.Now,
//                        IsPending = true
//                    };
//                    var result = await App.Locator.AddFriend.SendFriendshipRequest(friendship, item);

//                });

//                _alreadyCreatedViews[item.IdUser] = view;
//            }
//            // image de l'utilisateur
//            if (!string.IsNullOrEmpty(item.UserPicture))
//            {
//                var bytes = Convert.FromBase64String(item.UserPicture);
//                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                holder.FriendPicture.SetImageBitmap(imageBitmap);
//                imageBitmap.Dispose();
//            }
//            else holder.FriendPicture.SetImageResource(Resource.Drawable.DefaultCommunityUser);

//            // Nom et prénom de l'ami
//            holder.FriendFormattedName.Text = string.Format("{0} {1}", item.FirstName, item.LastName);

//            // Etat de la relation entre l'utilisateur connecté et l'utilisateur affiché
//            // if is friend
//            if (App.CurrentUserEnvironment.LsFriend.Exists((e) => e.IdUser == item.IdUser))
//            {
//                var hexColorGray = ImageHelper.ConvertColorToHex(_context.Resources.GetColor(Resource.Color.communityGray));
//                holder.InformationImageView.Clickable = false;
//                holder.InformationImageView.SetSvg(_context, Resource.Drawable.CommunityShare, "555555=" + hexColorGray);
//                holder.InformationTextView.Text = _context.Resources.GetString(Resource.String.addFriend_alreadyFriend);
//                holder.InformationTextView.SetTextColor(_context.Resources.GetColor(Resource.Color.communityGray));
//            }
//            // if pending request
//            else if(App.Locator.TabRequests.LsPendingFriendship
//                .Exists((e) =>
//                e.Friend_IdUser == item.IdUser && e.User_IdUser == App.CurrentUserEnvironment.User.IdUser
//                || e.User_IdUser == item.IdUser && e.Friend_IdUser == App.CurrentUserEnvironment.User.IdUser))
//            {
//                var hexColorGray = ImageHelper.ConvertColorToHex(_context.Resources.GetColor(Resource.Color.communityGray));
//                holder.InformationImageView.Clickable = false;
//                holder.InformationImageView.SetSvg(_context, Resource.Drawable.CommunityShare, "555555=" + hexColorGray);
//                holder.InformationTextView.Text = _context.Resources.GetString(Resource.String.addFriend_pendingRequest);
//                holder.InformationTextView.SetTextColor(_context.Resources.GetColor(Resource.Color.communityGray));
//            }
//            else
//            {
//                var hexCodeColorCommunity = ImageHelper.ConvertColorToHex(_context.Resources.GetColor(Resource.Color.communityBlue));
//                holder.InformationImageView.Clickable = true;
//                holder.InformationImageView.SetSvg(_context, Resource.Drawable.AddFriend, "555555=" + hexCodeColorCommunity);
//                holder.InformationTextView.Text = _context.Resources.GetString(Resource.String.addFriend_sendRequest);
//                holder.InformationTextView.SetTextColor(_context.Resources.GetColor(Resource.Color.communityBlue));
//            }

//            return view;
//        }

//        #endregion
//    }
//}