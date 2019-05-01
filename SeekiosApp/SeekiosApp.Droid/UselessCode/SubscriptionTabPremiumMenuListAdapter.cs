//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Graphics;
//using SeekiosApp.Model.DTO;

//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;

//using Android.Widget;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    class SubscriptionTabPremiumMenuListAdapter : BaseAdapter<SeekiosDTO>
//    {

//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public SubscriptionTabPremiumMenuListAdapter(Activity context)
//            : base()
//        {
//            _context = context;
//        }


//        public override SeekiosDTO this[int position]
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.GetSeekiosOfUsers()[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.CurrentUserEnvironment.GetSeekiosOfUsers().Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//                return App.CurrentUserEnvironment.GetSeekiosOfUsers().ToList()[position].Idseekios;
//        }

//        #endregion

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            var item = this[position];
//            if (!_alreadyCreatedViews.ContainsKey(item.Idseekios))
//                _alreadyCreatedViews.Add(item.Idseekios, null);

//            // If we get the view, we get the holder
//            Android.Views.View view = _alreadyCreatedViews[item.Idseekios];
//            SubscriptionTabPremiumMenuListViewHolder holder = null;
//            if (view != null) holder = view.Tag as SubscriptionTabPremiumMenuListViewHolder;

//            if (holder == null)
//            {
//                holder = new SubscriptionTabPremiumMenuListViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.SunbscriptionSeekiosContextMenuListRow, null);
//                view.Tag = holder;

//                // get the objects from the view
//                holder.SeekiosPicture = view.FindViewById<RoundedImageView>(Resource.Id.customListRow_image);
//                holder.SeekiosName = view.FindViewById<TextView>(Resource.Id.customListRow_text);
//                holder.PremiumImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_coindollar);

//                // setup the name of the seekios
//                holder.SeekiosName.Text = item.SeekiosName;

//                // display a picture with a dollar when it's a premium seekios
//                if (item.Subscription_idsubscription == (int)Enum.SubscriptionDefinitionEnum.Freemium)
//                {
//                    holder.PremiumImageView.Visibility = ViewStates.Gone;
//                }                

//                // set up the picture of the seekios
//                if (!string.IsNullOrEmpty(item.SeekiosPicture))
//                {
//                    var bytes = Convert.FromBase64String(item.SeekiosPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    holder.SeekiosPicture.SetImageBitmap(imageBitmap);
//                    imageBitmap.Dispose();
//                }
//                else holder.SeekiosPicture.SetImageResource(Resource.Drawable.DefaultSeekios);

//                _alreadyCreatedViews[item.Idseekios] = view;
//            }

//            return view;
//        }
//    }
//}