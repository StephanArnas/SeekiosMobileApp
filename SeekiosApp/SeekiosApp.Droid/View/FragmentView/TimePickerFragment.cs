using System;

using Android.App;
using Android.OS;
using Android.Widget;
using SeekiosApp.Model.APP;

namespace SeekiosApp.Droid.View.FragmentView
{
    class TimePickerFragment : Android.Support.V4.App.DialogFragment, TimePickerDialog.IOnTimeSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(TimePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<Time> _dateSelectedHandler = delegate { };
        Action<TimeDay> _dateDaySelectedHandler = delegate { };
        Time _dateSelected = new Time();
        TimeDay _dateDaySelected = new TimeDay();
        bool _isDateDay = false;

        public static TimePickerFragment NewInstance(Action<Time> onDateSelected, DateTime defaultDate)
        {
            TimePickerFragment frag = new TimePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            return frag;
        }
        public static TimePickerFragment NewInstance(Action<TimeDay> onDateSelected, DateTime defaultDate)
        {
            TimePickerFragment frag = new TimePickerFragment();
            frag._isDateDay = true;
            frag._dateDaySelectedHandler = onDateSelected;
            return frag;
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Android.App.TimePickerDialog dialog = new Android.App.TimePickerDialog(Activity,
                                                           this,
                                                           DateTime.Now.Hour,
                                                           DateTime.Now.Minute,
                                                           true);
            return dialog;
        }

        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            //we display the LCOAL time for the user
            if(_isDateDay)
            {
                _dateDaySelected = new TimeDay(hourOfDay, minute);
                _dateDaySelectedHandler(_dateDaySelected);
            }
            else
            {
                _dateSelected = new Time(hourOfDay, minute);
                _dateSelectedHandler(_dateSelected);
            }
        }

        internal void Show(Android.App.FragmentManager fragmentManager, string tAG)
        {

        }
    }
}