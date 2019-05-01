//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Graphics;
//using System.Globalization;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Model.DTO;
//using SeekiosApp.Droid.CustomComponents.Adapter;
//using SeekiosApp.Droid.CustomComponents;
//using Android.Telephony;

//namespace SeekiosApp.Droid.View.FragmentView
//{
//    public class TabCreditPremiumFragment : Android.Support.V4.App.Fragment
//    {
//        ReloadCreditActivity _context;
//        public TabCreditPremiumFragment(ReloadCreditActivity context)
//        {
//            _context = context;
//        }

//        #region ===== Attributs ===================================================================

//        /// <summary>Il est la mise en page qui affiche la liste des seekios avec prime ou freemium</summary>

//        public LinearLayout SeekiosListLayout { get; set; }

//        /// <summary>il affiche le nom de seekios</summary>
//        public TextView SeekiosName { get; set; }

//        /// <summary>Afficher la seekios pictaure</summary>
//        public RoundedImageView SeekiosImageView { get; set; }

//        ///<summary>Liste des seekios de la popup</summary>
//        public ListView ShortSeekiosListView { get; set; }

//        public Button SeekiosSubscriptionButton { get; set; }

//        /// <summary></summary>
//        private Android.Views.View _view = null;
//        /// <summary></summary>
//        private Context context = null;

//        ///<summary>Adapter de la listview de la popup</summary>
//        private SubscriptionTabPremiumMenuListAdapter _adapter;

//        /// <summary>Nom du seekios sélectionné à afficher dans le texte informatif</summary>
//        private string _seekiosSelectedName = string.Empty;

//        private bool stateButtonSubscribe;

//        #endregion


//        #region ===== Cycle De Vie ========================

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="inflater"></param>
//        /// <param name="container"></param>
//        /// <param name="savedInstanceState"></param>
//        /// <returns></returns>
//        /// 
//        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
//        {
//            var view = inflater.Inflate(Resource.Layout.TabCreditPremium, container, false);
//            GetObjectsFromView(view);
//            //DisplaySeekios();
//            return view;
//        }

//        private void GetObjectsFromView(Android.Views.View view)
//        {
//            SeekiosListLayout = view.FindViewById<LinearLayout>(Resource.Id.seekios_layout);
//            SeekiosImageView = view.FindViewById<RoundedImageView>(Resource.Id.seekiosRow_seekiosImage);
//            SeekiosName = view.FindViewById<TextView>(Resource.Id.seekios_nametextview);
//            SeekiosSubscriptionButton = view.FindViewById<Button>(Resource.Id.subscription_button);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        public override void OnResume()
//        {
//            base.OnResume();
//            SeekiosSubscriptionButton.Click += UpdateSeekiosSubscriptionClick;
//            SeekiosListLayout.Click += onSeekiosLayoutClick;


//        }

//        private void UpdateSeekiosSubscriptionClick(object sender, EventArgs e)
//        {
//            App.Locator.ReloadCredit.UpdateSeekiosSubscription();

//            if (SeekiosSubscriptionButton.Text == "Unsubscribe")
//            {
//                SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.btn_bg);
//                SeekiosSubscriptionButton.SetText(Resource.String.subscription_btnpremium);
//                SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                SeekiosSubscriptionButton.SetTextSize(0, 40);
//            }
//            else
//            {
//                SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.unsubscribebtn_bg);
//                SeekiosSubscriptionButton.SetText(Resource.String.unsubscription);
//                SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                SeekiosSubscriptionButton.SetTextSize(0, 40);
//            }
//        }

//        private void onSeekiosLayoutClick(object sender, EventArgs e)
//        {

//            Dialog listSeekiosDialog = null;
//            var listSeekiosSeekiosDialog = new Android.Support.V7.App.AlertDialog.Builder(this.Activity); var inflater = (LayoutInflater)this.Activity.GetSystemService(Context.LayoutInflaterService);
//            var view = inflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListLayout, null);

//            ShortSeekiosListView = view.FindViewById<ListView>(Resource.Id.shortSeekios_listView);

//            _adapter = new SubscriptionTabPremiumMenuListAdapter(this.Activity);

//            ShortSeekiosListView.Adapter = _adapter;
//            ShortSeekiosListView.ChoiceMode = ChoiceMode.Single;
//            ShortSeekiosListView.ItemsCanFocus = true;

//            ShortSeekiosListView.ItemClick += ((o, p) =>
//            {
//                var idSeekios = (int)_adapter.GetItemId(p.Position);
//                var selectedSeekios = App.CurrentUserEnvironment.GetSeekiosOfUsers().FirstOrDefault(el => el.Idseekios == idSeekios);
//                if (selectedSeekios == null) return;


//                if (!string.IsNullOrEmpty(selectedSeekios.SeekiosPicture))
//                {
//                    var bytes = Convert.FromBase64String(selectedSeekios.SeekiosPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    SeekiosImageView.SetImageBitmap(imageBitmap);

//                    imageBitmap.Dispose();
//                }
//                else SeekiosImageView.SetImageResource(Resource.Drawable.DefaultSeekios);

//                if (selectedSeekios.Subscription_idsubscription == (int)Enum.SubscriptionDefinitionEnum.Freemium)
//                {
//                    SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.btn_bg);
//                    SeekiosSubscriptionButton.SetText(Resource.String.subscription_btnpremium);
//                    SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                    SeekiosSubscriptionButton.SetTextSize(0, 40);
//                }
//                else
//                {
//                    SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.unsubscribebtn_bg);
//                    SeekiosSubscriptionButton.SetText(Resource.String.unsubscription);
//                    SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                    SeekiosSubscriptionButton.SetTextSize(0, 40);
//                }

//                SeekiosName.Text = selectedSeekios.SeekiosName;
//                App.Locator.ReloadCredit.SelectedSeekiosSubscription = selectedSeekios;

//                listSeekiosDialog.Dismiss();
//            });



//            listSeekiosSeekiosDialog.SetView(view);
//            listSeekiosDialog = listSeekiosSeekiosDialog.Create();
//            listSeekiosDialog.Show();


//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private void DisplaySeekios()
//        {
//            var inflater = (LayoutInflater)this.Activity.GetSystemService(Context.LayoutInflaterService);
//            var view = inflater.Inflate(Resource.Layout.ShareSeekiosContextMenuListLayout, null);

//            ShortSeekiosListView = view.FindViewById<ListView>(Resource.Id.shortSeekios_listView);

//            _adapter = new SubscriptionTabPremiumMenuListAdapter(this.Activity);

//            ShortSeekiosListView.Adapter = _adapter;
//            ShortSeekiosListView.ChoiceMode = ChoiceMode.Single;
//            ShortSeekiosListView.ItemsCanFocus = true;

//            if (!ShortSeekiosListView.Adapter.IsEmpty)
//            {
//                var idSeekios = (int)_adapter.GetItemId(0);

//                var selectedSeekios = App.CurrentUserEnvironment.GetSeekiosOfUsers().FirstOrDefault(el => el.Idseekios == idSeekios);
//                if (selectedSeekios == null) return;


//                if (!string.IsNullOrEmpty(selectedSeekios.SeekiosPicture))
//                {
//                    var bytes = Convert.FromBase64String(selectedSeekios.SeekiosPicture);
//                    var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                    SeekiosImageView.SetImageBitmap(imageBitmap);

//                    imageBitmap.Dispose();
//                }
//                else SeekiosImageView.SetImageResource(Resource.Drawable.DefaultSeekios);

//                if (selectedSeekios.Subscription_idsubscription == (int)Enum.SubscriptionDefinitionEnum.Freemium)
//                {
//                    SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.btn_bg);
//                    SeekiosSubscriptionButton.SetText(Resource.String.subscription_btnpremium);
//                    SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                    SeekiosSubscriptionButton.SetTextSize(0, 40);
//                }
//                else
//                {
//                    SeekiosSubscriptionButton.SetBackgroundResource(Resource.Drawable.unsubscribebtn_bg);
//                    SeekiosSubscriptionButton.SetText(Resource.String.unsubscription);
//                    SeekiosSubscriptionButton.SetPadding(20, 30, 20, 30);
//                    SeekiosSubscriptionButton.SetTextSize(0, 40);
//                }

//                SeekiosName.Text = selectedSeekios.SeekiosName;
//                App.Locator.ReloadCredit.SelectedSeekiosSubscription = selectedSeekios;
//            }
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        public override void OnPause()
//        {
//            base.OnPause();
//            SeekiosSubscriptionButton.Click -= UpdateSeekiosSubscriptionClick;
//            SeekiosListLayout.Click -= onSeekiosLayoutClick;
//        }

//        #endregion

//    }
//}