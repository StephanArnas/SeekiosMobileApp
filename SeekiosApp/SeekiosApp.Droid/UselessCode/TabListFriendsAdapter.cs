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
//using XamSvg;
//using Android.Graphics;
//using Android.Database;
//using com.refractored.fab;
//using System.Threading;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class TabListFriendsAdapter : BaseAdapter<FriendUserDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Android.Support.V4.App.Fragment _fragment;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();
//        private int[] _indexStates;

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public TabListFriendsAdapter(Android.Support.V4.App.Fragment fragment)
//            : base()
//        {
//            _fragment = fragment;
//            if (Count > 0)
//                RefreshHeaders();
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override FriendUserDTO this[int position]
//        {
//            get
//            {
//                return App.Locator.TabListFriends.LsFriends[position];
//            }
//        }

//        public override int Count
//        {
//            get { return App.Locator.TabListFriends.LsFriends.Count; }

//        }

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'objet UserDTO qui représente un ami dans la liste
//            var item = App.Locator.TabListFriends.LsFriends[position];

//            // pas de vue à utiliser ? création d'une vue
//            TabListFriendsViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(item.IdUser))
//                _alreadyCreatedViews.Add(item.IdUser, null);
//            Android.Views.View view = _alreadyCreatedViews[item.IdUser];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as TabListFriendsViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new TabListFriendsViewHolder();
//                view = _fragment.Activity.LayoutInflater.Inflate(Resource.Layout.TabListFriendsRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.FriendAvatarRoundedImageView = view.FindViewById<RoundedImageView>(Resource.Id.friendRow_userImage);
//                holder.FriendNameTextView = view.FindViewById<TextView>(Resource.Id.friendRow_nameText);
//                holder.FriendShareTextView = view.FindViewById<TextView>(Resource.Id.friendRow_shareText);
//                holder.SectionHeaderLayout = view.FindViewById<LinearLayout>(Resource.Id.listFriend_headerLayout);
//                holder.FriendListHeaderTextView = view.FindViewById<TextView>(Resource.Id.listFriend_header_title);
//                holder.FriendRowLayout = view.FindViewById<RelativeLayout>(Resource.Id.listFriend_rowLayout);

//                // on gère les clicks avec le listener
//                view.SetOnTouchListener(new OnTouchListener(holder, _fragment.Activity, item));

//                _alreadyCreatedViews[item.IdUser] = view;
//            }

//            // image de l'ami
//            if (!string.IsNullOrEmpty(item.UserPicture))
//            {
//                var bytes = Convert.FromBase64String(item.UserPicture);
//                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                holder.FriendAvatarRoundedImageView.SetImageBitmap(imageBitmap);
//                imageBitmap.Dispose();
//            }
//            else holder.FriendAvatarRoundedImageView.SetImageResource(Resource.Drawable.DefaultCommunityUser);

//            // Nom et prénom de l'ami
//            holder.FriendNameTextView.Text = string.Format("{0} {1}", item.FirstName, item.LastName);

//            // Partages des seekios
//            // récupération des seekios partagés avec l'ami qui vient d'être défini

//            string sharingsText = string.Empty;
//            var sharingsNumber = App.Locator.TabListFriends.LsSharing.Where(el => el.Friend_IdUser == item.IdUser || el.User_IdUser == item.IdUser).Count();

//            if (sharingsNumber == 0) sharingsText = _fragment.Resources.GetString(Resource.String.listFriend_noSeekiosShared);
//            else if (sharingsNumber == 1) sharingsText = string.Format("{0} {1}", sharingsNumber, _fragment.Resources.GetString(Resource.String.listFriend_singleSeekiosShared));
//            else sharingsText = string.Format("{0} {1}", sharingsNumber, _fragment.Resources.GetString(Resource.String.listFriend_multipleSeekiosShared));

//            holder.FriendShareTextView.Text = sharingsText;

//            holder.FriendListHeaderTextView.Text = item.FirstName.Substring(0, 1).ToUpper();
//            if (position <= _indexStates.Length - 1)
//                holder.SectionHeaderLayout.Visibility = _indexStates[position] == (int)HeaderState.HeaderVisible ? ViewStates.Visible : ViewStates.Gone;

//            return view;
//        }

//        /// <summary>
//        /// Permet d'initialiser et de refresh les headers quand on supprime un ami
//        /// </summary>
//        public void RefreshHeaders()
//        {
//            _indexStates = new int[Count];
//            var i = 0;
//            foreach (var groupByName in App.Locator.TabListFriends.LsFriends.GroupBy(el => el.FirstName[0]))
//            {
//                var first = true;
//                foreach (var user in groupByName)
//                {
//                    if (first)
//                    {
//                        _indexStates[i] = (int)HeaderState.HeaderVisible;
//                        first = false;
//                    }
//                    else
//                    {
//                        _indexStates[i] = (int)HeaderState.HeaderGone;
//                    }
//                    i++;
//                }
//            }
//        }

//    }
//    #endregion

//    public class OnTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
//    {
//        private TabListFriendsViewHolder _holder = null;
//        private Activity _context;
//        private FriendUserDTO _item;
//        private Thread _longClickThread = null;
//        private bool _isLongClickCompleted = false;

//        public OnTouchListener(TabListFriendsViewHolder holder, Activity context, FriendUserDTO item)
//        {
//            _holder = holder;
//            _context = context;
//            _item = item;
//        }

//        public bool OnTouch(Android.Views.View v, MotionEvent e)
//        {
//            //Lorsque l'on appuie sur un item de la liste des amis, on lance le timer pour le longClick
//            if (e.Action == MotionEventActions.Down)
//            {
//                if (_longClickThread != null && _longClickThread.IsAlive)
//                    _longClickThread.Abort();
//                _isLongClickCompleted = false;
//                //Thread timer pour le long click
//                _longClickThread = new Thread(new ThreadStart(() =>
//                {
//                    var longPressTimeout = 2000;
//                    try
//                    {
//                        longPressTimeout = ViewConfiguration.LongPressTimeout;
//                    }
//                    catch (Exception)
//                    {
//                        longPressTimeout = 2000;
//                    }
//                    Thread.Sleep(longPressTimeout);
//                    //execution du longclick sur la vue du seekios
//                    _context.RunOnUiThread(() => v.PerformLongClick());
//                    _isLongClickCompleted = true;
//                }));
//                _longClickThread.Start();
//                return true;
//            }
//            //Si on quitte la vue du seekios on annule le longclick
//            if (e.Action == MotionEventActions.Cancel)
//            {
//                if (_longClickThread != null && _longClickThread.IsAlive)
//                    _longClickThread.Abort();
//                return true;
//            }
//            //Dans le cas d'une autre action que up (notamment move), on quitte
//            if (e.Action != MotionEventActions.Up)
//                return true;

//            //Lorsque l'on relache le click, si le longclick a été réalisé on quitte
//            if (_isLongClickCompleted)
//                return true;

//            //Sinon on annule le longClick
//            if (_longClickThread != null && _longClickThread.IsAlive)
//                _longClickThread.Abort();

//            App.Locator.ListSharings.SelectedFriend = _item;
//            App.Locator.TabListFriends.GoToListShareSeekios();

//            return true;
//        }
//    }

//    public enum HeaderState
//    {
//        HeaderVisible = 0,
//        HeaderGone = 1,
//    }
//}