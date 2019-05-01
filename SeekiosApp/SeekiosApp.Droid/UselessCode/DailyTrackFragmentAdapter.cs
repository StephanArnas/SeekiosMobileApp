//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using SeekiosApp.Model.APP;
//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
//using Android.Text.Format;
//using SeekiosApp.Droid.View.FragmentView;
//using Android.Support.V4.App;
//using SeekiosApp.Helper;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class DailyTrackFragmentAdapter : BaseAdapter<Model.APP.Time>
//    {
//        #region ===== Attributs ===================================================================

//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        private Android.Support.V4.App.Fragment _context;

//        #endregion

//        #region ===== Constructeur ================================================================

//        public DailyTrackFragmentAdapter(Android.Support.V4.App.Fragment context)
//            : base()
//        {
//            _context = context;
//            if (App.Locator.ModeDailyTrack.TimePickerList == null)
//                App.Locator.ModeDailyTrack.TimePickerList = new List<Model.APP.Time>();
//        }

//        #endregion

//        #region ===== Propriétés ==================================================================

//        public override Model.APP.Time this[int position]
//        {
//            get
//            {
//                return App.Locator.ModeDailyTrack.TimePickerList[position];
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                return App.Locator.ModeDailyTrack.TimePickerList.Count;
//            }
//        }

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        #endregion

//        #region ===== List Adapter =================================================================

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            //get item
//            var time = this[position];

//            // pas de vue à utiliser ? création d'une vue
//            DailyTrackFragmentViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(position))
//                _alreadyCreatedViews.Add(position, null);
//            Android.Views.View view = _alreadyCreatedViews[position];

//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as DailyTrackFragmentViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new DailyTrackFragmentViewHolder();
//                view = _context.Activity.LayoutInflater.Inflate(Resource.Layout.DailyTrackMetaDataFragmentRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.TimePickedLayout = view.FindViewById<LinearLayout>(Resource.Id.dailyTrackRow_timeLayout);
//                holder.TimePickedTextView = view.FindViewById<TextView>(Resource.Id.dailyTrackRow_time);
//                holder.DeleteImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.dailyTrackRow_deleteImage);

//                //initialise textview
//                holder.TimePickedTextView.Text = time.ToString();

//                //delete action on item click
//                holder.DeleteImageView.Click += ((e, o) =>
//                {
//                    App.Locator.ModeDailyTrack.DeleteTimeItem(position);
//                });

//                //open a time picker when an item is clicked
//                holder.TimePickedLayout.Click += ((e, o) =>
//                {
//                    TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (Model.APP.Time selectedTime)
//                    {
//                        //Work to do with the element
//                        if(selectedTime != time)
//                        {
//                            App.Locator.ModeDailyTrack.TimePickerList.Remove(time);
//                            App.Locator.ModeDailyTrack.TimePickerList.Add(selectedTime);
//                        }
//                        holder.TimePickedTextView.Text = selectedTime.ToString();                        
//                    }, DateTime.Now);
//                    frag.Show(_context.FragmentManager, DatePickerFragment.TAG);
//                });
//            }

//            return view;
//        }

//        #endregion
//    }
//}