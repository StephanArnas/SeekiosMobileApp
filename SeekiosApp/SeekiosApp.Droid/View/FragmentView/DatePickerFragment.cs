using System;
using Android.OS;
using Android.Widget;
using Android.Support.V4.App;

namespace SeekiosApp.Droid.View.FragmentView
{
    class DatePickerFragment : DialogFragment, Android.App.DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> _dateSelectedHandler = delegate { };
        DateTime _defaultDate = DateTime.Now;
        DateTime? _minDate = null;
        DateTime? _maxDate = null;

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected, DateTime defaultDate, DateTime minDate, DateTime maxDate)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag._dateSelectedHandler = onDateSelected;
            frag._defaultDate = defaultDate;
            frag._minDate = new DateTime(minDate.Year, minDate.Month, minDate.Day).AddDays(-1);
            frag._maxDate = maxDate;
            return frag;
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            Android.App.DatePickerDialog dialog = new Android.App.DatePickerDialog(Activity
                                                    , Resource.Style.Theme_AppCompat_Light_Dialog
                                                    , this
                                                    , _defaultDate.Year
                                                    , _defaultDate.Month
                                                    , _defaultDate.Day);

            dialog.UpdateDate(_defaultDate);

            if (_minDate.HasValue) dialog.DatePicker.MinDate = UnixTimestampFromDateTime(_minDate.Value);
            if (_maxDate.HasValue) dialog.DatePicker.MaxDate = UnixTimestampFromDateTime(_maxDate.Value);

            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            _dateSelectedHandler(selectedDate);
        }

        public static long UnixTimestampFromDateTime(DateTime date)
        {
            // Convert the alarm time to UTC
            var utcAlarmTime = TimeZoneInfo.ConvertTimeToUtc(date);
            // Work out the difference between epoch (Java) and ticks (.NET)
            var t2 = new DateTime(1970, 1, 1) - DateTime.MinValue;
            var epochDifferenceInSeconds = t2.TotalSeconds;
            // Convert from ticks to milliseconds
            return utcAlarmTime.AddSeconds(-epochDifferenceInSeconds).Ticks / 10000;
        }
    }
}