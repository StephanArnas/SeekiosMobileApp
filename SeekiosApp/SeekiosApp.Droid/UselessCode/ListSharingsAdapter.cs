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
//using System.Threading;
//using SeekiosApp.Droid.View;
//using System.Globalization;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class ListSharingsAdapter : BaseAdapter<SharingDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private bool _isTabowner;
//        ///<summary>Format de l'affichage des dates</summary>
//        private string _dateFormat = "MM/dd/yy";
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public ListSharingsAdapter(Activity context, bool isTabOwner)
//            : base()
//        {
//            _context = context;
//            _isTabowner = isTabOwner;
//        }

//        public override SharingDTO this[int position]
//        {
//            get
//            {
//                if (_isTabowner)
//                    return App.Locator.ListSharings.LsOwnSharing[position];
//                else return App.Locator.ListSharings.LsOtherSharing[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                if (_isTabowner)
//                    return App.Locator.ListSharings.LsOwnSharing.Count();
//                else return App.Locator.ListSharings.LsOtherSharing.Count();
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            if (_isTabowner)
//                return App.Locator.ListSharings.LsOwnSharing[position].Seekios_IdSeekios;
//            else return App.Locator.ListSharings.LsOtherSharing[position].Seekios_IdSeekios;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            // récupération de l'item
//            var item = this[position];

//            // récupération du seekios concerné
//            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == item.Seekios_IdSeekios);

//            // On essaie de récupérer la vue potentiellement déjà créé
//            ListSharingsViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(item.Seekios_IdSeekios))
//                _alreadyCreatedViews.Add(item.Seekios_IdSeekios, null);
//            Android.Views.View view = _alreadyCreatedViews[item.Seekios_IdSeekios];
//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as ListSharingsViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new ListSharingsViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Layout.ListSharingsRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.SeekiosRoundedImageview = view.FindViewById<RoundedImageView>(Resource.Id.sharingRow_seekiosImage);
//                holder.SeekiosNameTextView = view.FindViewById<TextView>(Resource.Id.sharingRow_seekiosName);
//                holder.ShareDateTextView = view.FindViewById<TextView>(Resource.Id.sharingRow_shareText);
//                holder.MapLayout = view.FindViewById<LinearLayout>(Resource.Id.sharingRow_mapLayout);

//                view.SetOnTouchListener(new OnTouchListener(holder, _context, item, seekios));

//                // bouton map
//                holder.MapLayout.Click += new EventHandler((o, e) => App.Locator.MySeekiosDetail.GoToMap(App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios == item.Seekios_IdSeekios)));

//                _alreadyCreatedViews[item.Seekios_IdSeekios] = view;
//            }

//            if (!string.IsNullOrEmpty(seekios.SeekiosPicture))
//            {
//                var bytes = Convert.FromBase64String(seekios.SeekiosPicture);
//                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                holder.SeekiosRoundedImageview.SetImageBitmap(imageBitmap);
//                imageBitmap.Dispose();
//            }
//            else holder.SeekiosRoundedImageview.SetImageResource(Resource.Drawable.DefaultSeekios);

//            // nom du seekios
//            holder.SeekiosNameTextView.Text = seekios.SeekiosName;

//            // date de partage
//            if (item.DateEndSharing == null) holder.ShareDateTextView.Text = string.Format(_context.Resources.GetString(Resource.String.listSharing_shareDateUnlimited), item.DateBeginSharing.ToString(_dateFormat, CultureInfo.CurrentCulture));
//            else holder.ShareDateTextView.Text = string.Format(_context.Resources.GetString(Resource.String.listSharing_shareDateOnPeriod), item.DateBeginSharing.ToString(_dateFormat, CultureInfo.CurrentCulture), item.DateEndSharing?.ToString(_dateFormat, CultureInfo.CurrentCulture));

//            return view;
//        }

//        #endregion

//        public class OnTouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
//        {
//            private ListSharingsViewHolder _holder = null;
//            private Activity _context;
//            private SharingDTO _item;
//            private SeekiosDTO _seekiosSelected;
//            private Thread _longClickThread = null;
//            private bool _isLongClickCompleted = false;

//            public OnTouchListener(ListSharingsViewHolder holder, Activity context, SharingDTO item, SeekiosDTO seekios)
//            {
//                _holder = holder;
//                _context = context;
//                _item = item;
//                _seekiosSelected = seekios;
//            }

//            public bool OnTouch(Android.Views.View v, MotionEvent e)
//            {
//                App.Locator.ShareSeekios.SharingSelected = _item;

//                // On vérifie l'appartenance du partage
//                if (App.CurrentUserEnvironment.User.IdUser == _item.User_IdUser && _item.IsUserOwner == true
//                        || App.CurrentUserEnvironment.User.IdUser == _item.Friend_IdUser && _item.IsUserOwner == false)
//                    App.Locator.ShareSeekios.IsOwnSharing = true;
//                else App.Locator.ShareSeekios.IsOwnSharing = false;

//                if (App.Locator.ShareSeekios.IsOwnSharing)
//                {
//                    //Lorsque l'on appuie sur la vue du seekios on lance le timer pour le longClick
//                    if (e.Action == MotionEventActions.Down)
//                    {
//                        if (_longClickThread != null && _longClickThread.IsAlive)
//                            _longClickThread.Abort();
//                        _isLongClickCompleted = false;
//                        //Thread timer pour le long click
//                        _longClickThread = new Thread(new ThreadStart(() =>
//                        {
//                            Thread.Sleep(Android.Views.ViewConfiguration.LongPressTimeout);
//                        //execution du longclick sur la vue des amis
//                        _context.RunOnUiThread(() => v.PerformLongClick());
//                            _isLongClickCompleted = true;
//                        }));
//                        _longClickThread.Start();
//                        return true;
//                    }
//                    //Si on quitte la vue du seekios on annule le longclick
//                    if (e.Action == MotionEventActions.Cancel)
//                    {
//                        if (_longClickThread != null && _longClickThread.IsAlive)
//                            _longClickThread.Abort();
//                        return true;
//                    }
//                    //Dans le cas d'une autre action que up (notamment move), on quitte
//                    if (e.Action != MotionEventActions.Up)
//                        return true;

//                    //Lorsque l'on relache le click, si le longclick a été réalisé on quitte
//                    if (_isLongClickCompleted)
//                        return true;

//                    //Sinon on annule le longClick
//                    if (_longClickThread != null && _longClickThread.IsAlive)
//                        _longClickThread.Abort();

//                    var idUser = App.CurrentUserEnvironment.User.IdUser;
//                    // Edit Mode : si l'on clique sur un seekios que l'on partage
//                    if ((idUser == _item.User_IdUser && _item.IsUserOwner == true)
//                    || (idUser == _item.Friend_IdUser && _item.IsUserOwner == false))
//                        App.Locator.ShareSeekios.IsInEditMode = true;
//                    // Display Mode : si on clique sur le seekios qu'un ami nous partage
//                    else App.Locator.ShareSeekios.IsInEditMode = false;

//                    App.Locator.ShareSeekios.SeekiosSelected = new ShortSeekiosDTO()
//                    {
//                        IdSeekios = _seekiosSelected.Idseekios,
//                        SeekiosName = _seekiosSelected.SeekiosName,
//                        SeekiosPicture = _seekiosSelected.SeekiosPicture,
//                        User_iduser = _seekiosSelected.User_iduser.ToString()
//                    };
//                    App.Locator.ListSharings.GoToShareSeekios();
//                }
//                return true;
//            }
//        }
//    }
//}
