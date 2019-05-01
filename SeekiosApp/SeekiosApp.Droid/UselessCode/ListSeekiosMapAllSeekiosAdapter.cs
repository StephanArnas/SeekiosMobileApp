//using System.Collections.Generic;

//using Android.App;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Model.DTO;
//using Android.Views.Animations;
//using GalaSoft.MvvmLight.Views;
//using Android.Graphics;
//using System.Linq;
//using System;
//using SeekiosApp.Extension;
//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
//using SeekiosApp.Droid.Helper;

//namespace SeekiosApp.Droid.CustomComponents
//{
//    public class ListSeekiosMapAllSeekiosAdapter : BaseAdapter<SeekiosDTO>
//    {
//        #region ===== Attributs ===================================================================

//        private Activity _context;
//        private Action<SeekiosDTO> _onSeekiosClick;
//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        #endregion

//        #region ===== Constructeur(s) =============================================================

//        public ListSeekiosMapAllSeekiosAdapter(Activity context, Action<SeekiosDTO> onSeekiosClick)
//            : base()
//        {
//            _onSeekiosClick = onSeekiosClick;
//            _context = context;
//        }

//        #endregion

//        #region ===== ListAdapter =================================================================

//        public override long GetItemId(int position)
//        {
//            return position;
//        }
//        public override SeekiosDTO this[int position]
//        {
//            get { return App.Locator.ListSeekios.LsSeekios[position]; }
//        }
//        public override int Count
//        {
//            get { return App.Locator.ListSeekios.LsSeekios.Count; }
//        }
//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            var item = this[position];

//            // pas de vue à utiliser ? création d'une vue
//            ListSeekiosMapPageViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(item.Idseekios))
//                _alreadyCreatedViews.Add(item.Idseekios, null);
//            Android.Views.View view = _alreadyCreatedViews[item.Idseekios];

//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as ListSeekiosMapPageViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new ListSeekiosMapPageViewHolder();
//                view = _context.LayoutInflater.Inflate(Resource.Drawable.SeekiosRowMapAllSeekios, null);
//                view.Tag = holder;

//                // récupération des objets de la vue
//                holder.ImageSeekiosRoundedImageView = view.FindViewById<RoundedImageView>(Resource.Id.seekiosRow_seekiosImage);
//                holder.LastPositionTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_lastPosition);
//                holder.LogoModeSvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_modeLogo);
//                holder.LogoBatterySvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_batteryLogo);
//                holder.LogoSignalSvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_signalLogo);
//                holder.LogoBatteryProgressSvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_batteryLogoProgress);
//                holder.LogoSignalProgressSvgImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.seekiosRow_signalLogoProgress);
//                holder.BatteryProgressRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.seekiosRow_batteryProgressLayout);
//                holder.SignalProgressRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.seekiosRow_signalProgressLayout);
//                holder.SeekiosNameTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_seekiosName);
//                holder.BatteryTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_battery);
//                holder.SignalTextView = view.FindViewById<TextView>(Resource.Id.seekiosRow_signal);

//                view.Click += (s, e) => _onSeekiosClick(item);

//                _alreadyCreatedViews[item.Idseekios] = view;
//            }
//            // changement de couleur si un seekios n'est pas celui de l'utilisateur connecté
//            if (item.User_iduser != App.CurrentUserEnvironment.User.IdUser)
//            {
//                holder.SeekiosNameTextView.SetTextColor(_context.Resources.GetColor(Resource.Color.communityBlue));
//                var hexCodeColorCommunity = ImageHelper.ConvertColorToHex(_context.Resources.GetColor(Resource.Color.communityBlue));
//                holder.LogoSignalProgressSvgImageView.SetSvg(_context, Resource.Drawable.Signal, "888888=" + hexCodeColorCommunity);
//                holder.LogoBatteryProgressSvgImageView.SetSvg(_context, Resource.Drawable.Battery, "36DA3E=" + hexCodeColorCommunity);
//                holder.LogoBatterySvgImageView.SetSvg(_context, Resource.Drawable.Battery, "36DA3E=888888");
//            }
//            else
//            {
//                holder.SeekiosNameTextView.SetTextColor(_context.Resources.GetColor(Resource.Color.primary));
//                var hexCodeColorPrimary = ImageHelper.ConvertColorToHex(_context.Resources.GetColor(Resource.Color.primary));
//                holder.LogoSignalProgressSvgImageView.SetSvg(_context, Resource.Drawable.Signal, "888888=" + hexCodeColorPrimary);
//                holder.LogoBatteryProgressSvgImageView.SetSvg(_context, Resource.Drawable.Battery, "36DA3E=" + hexCodeColorPrimary);
//                holder.LogoBatterySvgImageView.SetSvg(_context, Resource.Drawable.Battery, "36DA3E=888888");
//            }
//            // image du Seekios
//            if (!string.IsNullOrEmpty(item.SeekiosPicture))
//            {
//                var bytes = Convert.FromBase64String(item.SeekiosPicture);
//                var imageBitmap = BitmapFactory.DecodeByteArray(bytes, 0, bytes.Length);
//                holder.ImageSeekiosRoundedImageView.SetImageBitmap(imageBitmap);
//                imageBitmap.Dispose();
//            }
//            else holder.ImageSeekiosRoundedImageView.SetImageResource(Resource.Drawable.DefaultSeekios);

//            // nom du seekios
//            holder.SeekiosNameTextView.Text = item.SeekiosName;

//            // dernière localisation
//            if (item.LastKnownLocation_dateLocationCreation != null && item.LastKnownLocation_dateLocationCreation.Value.Year != 1)
//            {
//                holder.LastPositionTextView.Text = item.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow();
//            }

//            // mode 
//            var mode = App.CurrentUserEnvironment.LsMode.Where(el => el.Seekios_idseekios == item.Idseekios).FirstOrDefault();
//            if (mode != null)
//            {
//                switch (mode.ModeDefinition_idmodeDefinition)
//                {
//                    case 4: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeDontMove, "dddddd=888888"); break;
//                    case 5: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeZone, "dddddd=888888"); break;
//                    case 6: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeInTime, "dddddd=888888"); break;
//                    case 7: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeFollowMe, "dddddd=888888"); break;
//                    case 8: break;// Mode Keep it close
//                    case 9: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeDailyTrack, "dddddd=888888"); break;
//                    case 10: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeActivity, "dddddd=888888"); break;
//                    case 11: holder.LogoModeSvgImageView.SetSvg(_context, Resource.Drawable.ModeRoad, "dddddd=888888"); break;
//                    default:
//                        break;
//                }
//            }

//            // batterie
//            holder.BatteryTextView.Text = string.Format("{0}%", item.BatteryLife);
//            var totalSize = (double)holder.LogoBatterySvgImageView.LayoutParameters.Height;
//            totalSize *= 0.9;//pour ne pas prendre en compte le bout de l'image de la batterie
//            var sizeOfVisiblePart = (int)((totalSize / 100.0) * (double)item.BatteryLife);
//            holder.BatteryProgressRelativeLayout.LayoutParameters.Height = sizeOfVisiblePart;
//            ((RelativeLayout.LayoutParams)holder.LogoBatteryProgressSvgImageView.LayoutParameters).TopMargin = (int)(sizeOfVisiblePart - totalSize);

//            // signal
//            holder.SignalTextView.Text = string.Format("{0}%", item.SignalQuality);
//            totalSize = (double)holder.LogoSignalSvgImageView.LayoutParameters.Width;
//            sizeOfVisiblePart = (int)((totalSize / 100.0) * (double)item.SignalQuality);
//            holder.SignalProgressRelativeLayout.LayoutParameters.Width = sizeOfVisiblePart;
//            ((RelativeLayout.LayoutParams)holder.LogoSignalProgressSvgImageView.LayoutParameters).RightMargin = (int)(sizeOfVisiblePart - totalSize);

//            return view;
//        }

//        #endregion
//    }
//}