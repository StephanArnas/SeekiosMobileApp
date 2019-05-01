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
//using System.Threading;
//using Android.Text;
//using Android.Text.Style;
//using System.Threading.Tasks;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class TabRequestsAdapter : BaseAdapter<FriendshipDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Android.Support.V4.App.Fragment _fragment;
//        private List<FriendshipDTO> _lsFriendship;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public TabRequestsAdapter(Android.Support.V4.App.Fragment fragment)
//            : base()
//        {
//            _fragment = fragment;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override FriendshipDTO this[int position]
//        {
//            get
//            {
//                return App.Locator.TabRequests.LsPendingFriendship[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.Locator.TabRequests.LsPendingFriendship.Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'objet FriendshipDTO qui représente une demande dans la liste
//            var item = this[position];

//            // récupération de l'ami concerné par la requête
//            ShortUserDTO user = new ShortUserDTO();
//            if (item.Friend_IdUser == App.CurrentUserEnvironment.User.IdUser) user = App.Locator.TabRequests.LsUsers.Where(el => item.User_IdUser == el.IdUser).FirstOrDefault();
//            else user = App.Locator.TabRequests.LsUsers.Where(el => item.Friend_IdUser == el.IdUser).FirstOrDefault();

//            // pas de vue à utiliser ? création d'une vue
//            TabRequestsViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(user.IdUser))
//                _alreadyCreatedViews.Add(user.IdUser, null);
//            Android.Views.View view = _alreadyCreatedViews[user.IdUser];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as TabRequestsViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new TabRequestsViewHolder();
//                view = _fragment.Activity.LayoutInflater.Inflate(Resource.Layout.TabRequestsRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.TabRequestUserPicture = view.FindViewById<RoundedImageView>(Resource.Id.tabRequestRow_userImage);
//                holder.TabRequestInformationTextView = view.FindViewById<TextView>(Resource.Id.tabRequestRow_information);
//                holder.TabRequestAcceptImageView = view.FindViewById<SvgImageView>(Resource.Id.tabRequest_acceptRequestImgActionClick);
//                holder.TabRequestRefuseImageView = view.FindViewById<SvgImageView>(Resource.Id.tabRequest_refuseRequestImgActionClick);

//                // acceptation de la demande d'ami, on change l'état de pending à false
//                holder.TabRequestAcceptImageView.Click += (async (o, e) =>
//                {
//                    await App.Locator.TabRequests.AcceptFriendship(item);
//                });

//                // refus de la demande d'ami, on retire la demande de la bdd
//                holder.TabRequestRefuseImageView.Click += ((o, e) =>
//                {
//                    if (App.Locator.TabRequests.LsPendingFriendship.FirstOrDefault(el => el.User_IdUser == item.User_IdUser && el.Friend_IdUser == item.Friend_IdUser) == null) return;
//                    App.Locator.TabRequests.DeleteFriendship(item.User_IdUser, item.Friend_IdUser);
//                });

//                // gestion du texte informatif et de la visibilité du bouton d'acceptation
//                if (item != null && user != null && item.User_IdUser == App.CurrentUserEnvironment.User.IdUser)
//                {
//                    var userName = string.Format("{0} {1}", user.FirstName, user.LastName);
//                    var infoString = _fragment.Resources.GetString(Resource.String.tabRequest_ownFriendshipRequest);
//                    var infoText = string.Format(infoString, userName);

//                    var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(infoString, userName);
//                    var formattedinfoText = new SpannableString(infoText);
//                    formattedinfoText.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.CommunityColor)), resultTuple.Item1, resultTuple.Item2, 0);
//                    holder.TabRequestInformationTextView.SetText(formattedinfoText, TextView.BufferType.Spannable);

//                    holder.TabRequestAcceptImageView.Visibility = ViewStates.Gone;
//                }
//                else
//                {
//                    var userName = string.Format("{0} {1}", user.FirstName, user.LastName);
//                    var infoString = _fragment.Resources.GetString(Resource.String.tabRequest_otherFriendshipRequest);
//                    var infoText = string.Format(infoString, string.Format("{0} {1}", user.FirstName, user.LastName));

//                    var resultTuple = SeekiosApp.Helper.StringHelper.GetStartAndEndIndexOfStringInString(infoString, userName);
//                    var formattedinfoText = new SpannableString(infoText);
//                    formattedinfoText.SetSpan(new ForegroundColorSpan(Color.ParseColor(App.CommunityColor)), resultTuple.Item1, resultTuple.Item2, 0);
//                    holder.TabRequestInformationTextView.SetText(formattedinfoText, TextView.BufferType.Spannable);

//                    holder.TabRequestAcceptImageView.Visibility = ViewStates.Visible;
//                }
//                _alreadyCreatedViews[user.IdUser] = view;
//            }

//            // image de l'ami
//            if (user != null && !string.IsNullOrEmpty(user.UserPicture))
//            {
//                var bytes = Convert.FromBase64String(user.UserPicture);
//                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                holder.TabRequestUserPicture.SetImageBitmap(imageBitmap);
//                imageBitmap.Dispose();
//            }
//            else holder.TabRequestUserPicture.SetImageResource(Resource.Drawable.DefaultCommunityUser);

//            return view;
//        }
        
//        #endregion

//    }
//}