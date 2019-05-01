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
//using SeekiosApp.Model.APP;
//using SeekiosApp.Droid.CustomComponents.Adapter.ViewHolderAdapter;
//using SeekiosApp.Droid.View.FragmentView;
//using Android.Content.Res;

//namespace SeekiosApp.Droid.CustomComponents.Adapter
//{
//    public class InTimeFragmentAdapter : BaseAdapter<TimeDay>
//    {
//        #region ===== Attributs ===================================================================

//        private Dictionary<int, Android.Views.View> _alreadyCreatedViews = new Dictionary<int, Android.Views.View>();

//        private Android.Support.V4.App.Fragment _context;

//        #endregion

//        #region ===== Constructeur ================================================================

//        public InTimeFragmentAdapter(Android.Support.V4.App.Fragment context)
//            : base()
//        {
//            _context = context;
//            //if (App.Locator.ModeInTime.TimePickerList == null)
//            //    App.Locator.ModeInTime.TimePickerList = new List<TimeDay>();
//        }

//        public override TimeDay this[int position]
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public override int Count
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        #endregion

//        #region ===== Propriétés ==================================================================

//        //public override TimeDay this[int position]
//        //{
//        //    get
//        //    {
//        //        return App.Locator.ModeInTime.TimePickerList[position];
//        //    }
//        //}

//        //public override int Count
//        //{
//        //    get
//        //    {
//        //        return App.Locator.ModeInTime.TimePickerList.Count;
//        //    }
//        //}

//        public override long GetItemId(int position)
//        {
//            return position;
//        }

//        #endregion

//        #region ===== List Adapter =================================================================

//        public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
//        {
//            //get item
//            var timeDay = this[position];

//            // pas de vue à utiliser ? création d'une vue
//            InTimeFragmentViewHolder holder = null;
//            if (!_alreadyCreatedViews.ContainsKey(timeDay.GetHashCode()))
//                _alreadyCreatedViews.Add(timeDay.GetHashCode(), null);
//            Android.Views.View view = _alreadyCreatedViews[timeDay.GetHashCode()];

//            // Si on a récupéré la vue on récupère le holder dans son tag
//            if (view != null) holder = view.Tag as InTimeFragmentViewHolder;
//            // Si le holder n'est pas défini, on le fait et on crée la vue
//            if (holder == null)
//            {
//                holder = new InTimeFragmentViewHolder();
//                view = _context.Activity.LayoutInflater.Inflate(Resource.Layout.InTimeMetaDataFragmentRow, null);
//                view.Tag = holder;
//                // récupération des objets de la vue
//                holder.TimePickedLayout = view.FindViewById<LinearLayout>(Resource.Id.inTimeRow_clickabletimeLayout);
//                holder.TimePickedTextView = view.FindViewById<TextView>(Resource.Id.inTimeRow_time);
//                holder.DeleteImageView = view.FindViewById<XamSvg.SvgImageView>(Resource.Id.inTimeRow_deleteImage);
//                //Day Relative Layout
//                holder.MondayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_mondayLayout);
//                holder.TuesdayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_tuesdayLayout);
//                holder.WednesdayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_wednesdayLayout);
//                holder.ThursdayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_thursdayLayout);
//                holder.FridayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_fridayLayout);
//                holder.SaturdayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_saturdayLayout);
//                holder.SundayRelativeLayout = view.FindViewById<RelativeLayout>(Resource.Id.inTime_sundayLayout);
//                //Day Text View
//                holder.MondayTextView = view.FindViewById<TextView>(Resource.Id.inTime_monday);
//                holder.TuesdayTextView = view.FindViewById<TextView>(Resource.Id.inTime_tuesday);
//                holder.WednesdayTextView = view.FindViewById<TextView>(Resource.Id.inTime_wednesday);
//                holder.ThursdayTextView = view.FindViewById<TextView>(Resource.Id.inTime_thursday);
//                holder.FridayTextView = view.FindViewById<TextView>(Resource.Id.inTime_friday);
//                holder.SaturdayTextView = view.FindViewById<TextView>(Resource.Id.inTime_saturday);
//                holder.SundayTextView = view.FindViewById<TextView>(Resource.Id.inTime_sunday);

//                //Text for the rounded layout
//                holder.MondayTextView.Text = _context.GetString(Resource.String.inTime_mondayLetter);
//                holder.TuesdayTextView.Text = _context.GetString(Resource.String.inTime_tuesdayLetter);
//                holder.WednesdayTextView.Text = _context.GetString(Resource.String.inTime_wednesdayLetter);
//                holder.ThursdayTextView.Text = _context.GetString(Resource.String.inTime_thursdayLetter);
//                holder.FridayTextView.Text = _context.GetString(Resource.String.inTime_fridayLetter);
//                holder.SaturdayTextView.Text = _context.GetString(Resource.String.inTime_saturdayLetter);
//                holder.SundayTextView.Text = _context.GetString(Resource.String.inTime_sundayLetter);

//                #region DAY LAYOUT : INITIALISE COLORS BEFORE CLICK

//                if (timeDay.Monday)
//                {
//                    holder.MondayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.MondayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.MondayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.MondayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Tuesday)
//                {
//                    holder.TuesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.TuesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.TuesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.TuesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Wednesday)
//                {
//                    holder.WednesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.WednesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.WednesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.WednesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Thursday)
//                {
//                    holder.ThursdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.ThursdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.ThursdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.ThursdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Friday)
//                {
//                    holder.FridayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.FridayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.FridayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.FridayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Saturday)
//                {
//                    holder.SaturdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.SaturdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.SaturdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.SaturdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }

//                if (timeDay.Sunday)
//                {
//                    holder.SundayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                    holder.SundayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                }
//                else
//                {
//                    holder.SundayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                    holder.SundayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                }
//                #endregion

//                #region DAY LAYOUT CLICK : MODIFY COLOR IF ACTIVE OR INACTIVE

//                holder.MondayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Monday = !timeDay.Monday;
//                    //if new state = true
//                    if (timeDay.Monday)
//                    {
//                        holder.MondayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.MondayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.MondayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.MondayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.TuesdayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Tuesday = !timeDay.Tuesday;
//                    //if new state = true
//                    if (timeDay.Tuesday)
//                    {
//                        holder.TuesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.TuesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.TuesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.TuesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.WednesdayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Wednesday = !timeDay.Wednesday;
//                    //if new state = true
//                    if (timeDay.Wednesday)
//                    {
//                        holder.WednesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.WednesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.WednesdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.WednesdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.ThursdayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Thursday = !timeDay.Thursday;
//                    //if new state = true
//                    if (timeDay.Thursday)
//                    {
//                        holder.ThursdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.ThursdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.ThursdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.ThursdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.FridayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Friday = !timeDay.Friday;
//                    //if new state = true
//                    if (timeDay.Friday)
//                    {
//                        holder.FridayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.FridayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.FridayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.FridayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.SaturdayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Saturday = !timeDay.Saturday;
//                    //if new state = true
//                    if (timeDay.Saturday)
//                    {
//                        holder.SaturdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.SaturdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.SaturdayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.SaturdayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });
//                holder.SundayRelativeLayout.Click += ((e, o) =>
//                {
//                    //change the actual state of the bool
//                    timeDay.Sunday = !timeDay.Sunday;
//                    //if new state = true
//                    if (timeDay.Sunday)
//                    {
//                        holder.SundayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisFillColor);
//                        holder.SundayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonColorSelector)); ; // _context.Resources.GetColor(Resource.Color.textColorPrimary);
//                    }
//                    else
//                    {
//                        holder.SundayRelativeLayout.SetBackgroundResource(Resource.Drawable.EllipsisNoFill);
//                        holder.SundayTextView.SetTextColor(_context.Resources.GetColorStateList(Resource.Drawable.ButtonNoColorSelector));
//                    }
//                });

//                #endregion

//                //open a time picker when an item is clicked
//                holder.TimePickedLayout.Click += ((e, o) =>
//                {
//                    TimePickerFragment frag = TimePickerFragment.NewInstance(delegate (TimeDay selectedTime)
//                    {
//                        //Work to do with the element
//                        if (selectedTime != timeDay)
//                        {
//                            //var timerToModify = App.Locator.ModeInTime.TimePickerList.First(el => el.Equals(timeDay));
//                            //timerToModify.Hour = selectedTime.Hour;
//                            //timerToModify.Minute = selectedTime.Minute;
//                        }
//                        holder.TimePickedTextView.Text = selectedTime.ToString();
//                    }, DateTime.Now);
//                    frag.Show(_context.FragmentManager, DatePickerFragment.TAG);
//                });


//                //delete action on item click
//                holder.DeleteImageView.Click += ((e, o) =>
//                {
//                    //App.Locator.ModeInTime.DeleteTimeItem(timeDay);
//                });

//                _alreadyCreatedViews[timeDay.GetHashCode()] = view;
//            }

//            //initialise textview
//            holder.TimePickedTextView.Text = timeDay.ToString();

//            return view;
//        }

//        #endregion
//    }
//}