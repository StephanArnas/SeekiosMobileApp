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
//using Android.Graphics.Drawables;
//using Xamarin.InAppBilling;
//using Android.Support.V4.App;
//using SeekiosApp.Droid.View;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class ListPackCreditAdapter : BaseAdapter<PackCreditDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Action<string> _buyProduct;
//        private FragmentActivity _activity;
//        private Action<string> p;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        /// <summary>
//        /// Initialise le adapter pour afficher les packs de credit
//        /// </summary>
//        /// <param name="context">activité</param>
//        public ListPackCreditAdapter(Activity context, Action<string> buyProduct) : base()
//        {
//            _context = context;
//            _buyProduct = buyProduct;
//        }

//        public ListPackCreditAdapter(FragmentActivity activity, Action<string> buyProduct)
//        {
//            _activity = activity;
//            _buyProduct = buyProduct;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override PackCreditDTO this[int position]
//        {
//            get { return ((ReloadCreditActivity)_context).LsProducts[position]; }
//        }

//        public override int Count
//        {
//            get { return App.Locator.ReloadCredit.LsPackCredit == null ? 0 : App.Locator.ReloadCredit.LsPackCredit.Count; }
//        }

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//           // // item courant
//           // var item = App.Locator.ReloadCredit.LsPackCredit[position];
//           // // initialisation de la vue (si première fois) ou récupération de la vue
//           // ListPackCreditViewHolder holder = null;
//           // if (!_alreadyCreatedViews.ContainsKey(item.IdPackCredit))
//           //     _alreadyCreatedViews.Add(item.IdPackCredit, null);
//           // Android.Views.View view = _alreadyCreatedViews[item.IdPackCredit];


//           // if (holder != null) holder = view.Tag as ListPackCreditViewHolder;
//           // if (holder == null)
//           // {
//           //     holder = new ListPackCreditViewHolder();
//           //     view = _activity.LayoutInflater.Inflate(Resource.Drawable.TabCreditPackItemGridView, null);
//           //     holder.TitleTextView = view.FindViewById<TextView>(Resource.Id.tabCreditPackItem_title);
//           //     holder.DescriptionTextView = view.FindViewById<TextView>(Resource.Id.tabCreditPackItem_description);
//           //     holder.PriceTextView = view.FindViewById<TextView>(Resource.Id.tabCreditPackItem_price);
//           //     holder.RewardingCreditTextView = view.FindViewById<TextView>(Resource.Id.tabCreditPackItem_rewarding);
//           //     holder.ContainerLayout = view.FindViewById<LinearLayout>(Resource.Id.rootLayout);
//           //     holder.SecondLayout = view.FindViewById<RelativeLayout>(Resource.Id.secondLayout);
//           //     holder.ArcCircleSvgImage = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.tabCreditPackItem_arcCircle);
//           //     view.Tag = holder;

//           //     _alreadyCreatedViews[item.IdPackCredit] = view;
//           // }

//           // // titre du pack
//           // holder.TitleTextView.Text = item.Title;
//           // // prix du pack
//           // holder.PriceTextView.Text = item.Price;
//           // // description du pack
//           // holder.DescriptionTextView.Text = item.Description;
//           // holder.DescriptionTextView.SetTextColor(Android.Graphics.Color.ParseColor(item.ColorHeaderBackground));
//           // // nombre de crédit du pack
//           // holder.RewardingCreditTextView.Text = item.RewardingCredit;
//           // // couleur du titre
//           // holder.SecondLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor(item.ColorHeaderBackground));
//           // // container
//           // using (StateListDrawable states = new StateListDrawable())
//           // {
//           //     states.AddState(new int[] { Android.Resource.Attribute.StatePressed }, new ColorDrawable(Android.Graphics.Color.ParseColor(item.ColorHeaderBackground)));
//           //     states.AddState(new int[] { Android.Resource.Attribute.NormalScreens }, new ColorDrawable(Android.Graphics.Color.ParseColor(item.ColorBackground)));
//           //     states.AddState(new int[] { }, new ColorDrawable(Android.Graphics.Color.ParseColor(item.ColorBackground)));
//           //     holder.ContainerLayout.Background = states;
//           // }
//           // // couleur du svg
//           // var colorMapping = "FFFFFF=" + item.ColorHeaderBackground.Substring(1);
//           //// holder.ArcCircleSvgImage.SetSvg(_context, Resource.Drawable.ArcCircle, colorMapping, string.Empty);
//           // // achat InApp du pack de credit 
//           // holder.ContainerLayout.Click += (o, e) =>
//           // {
//           //     Console.WriteLine("Achat InApp Pack Credit");
//           //     if (_buyProduct != null)
//           //     {
//           //         _buyProduct.Invoke(item.IdProduct);
//           //     }
//           // };
//            // retourne la view 
//            return convertView; // view 
//        }

//        #endregion
//    }
//}