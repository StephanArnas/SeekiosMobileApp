using SeekiosApp.Helper;
using SeekiosApp.Properties;
using System;

namespace SeekiosApp.Extension
{
    public static class DateExtension
    {
        public static string FormatDateFromNow(this DateTime dateLocation)
        {
            if (dateLocation == null) return Resources.ListSeekios_LastPositionNone;
            string displayDate = string.Empty;
            DateTime now = DateHelper.GetSystemTime();
            TimeSpan span = now - dateLocation;
            if (span.Days <= 365) //less than one year ago (approximately, not always exact)
            {
                if (span.TotalHours < now.Hour) //today
                {
                    displayDate = string.Format(Resources.ListSeekios_LastPositionToday
                        , dateLocation.ToString("HH:mm"));
                }
                else if (span.TotalHours < (now.Hour + 24)) //yesterday
                {
                    displayDate = string.Format(Resources.ListSeekios_LastPositionYesterday
                        , dateLocation.ToString("HH:mm"));
                }
                else //before yesterday
                {
                    displayDate = string.Format(Resources.ListSeekios_LastPositionPlurial
                        , span.Days
                        , dateLocation.ToString("HH:mm"));
                }
            }
            else displayDate = Resources.ListSeekios_LastPositionMoreThanOneYear;
            return displayDate;
        }

        public static string FormatDateFromNowAddSeekios(this DateTime dateLocation)
        {
            if (dateLocation == null) return Resources.ListSeekios_LastPositionNone;
            string displayDate = string.Empty;
            DateTime now = DateHelper.GetSystemTime();
            TimeSpan span = now - dateLocation;
            if (span.Days <= 365) //less than one year ago (approximately, not always exact)
            {
                if (span.TotalHours < now.Hour) //today
                {
                    displayDate = string.Format(Resources.AddedTodayAt, dateLocation.ToString("HH:mm"));
                }
                else if (span.TotalHours < (now.Hour + 24)) //yesterday
                {
                    displayDate = string.Format(Resources.AddedYesterdayAt, dateLocation.ToString("HH:mm"));
                }
                else //before yesterday
                {
                    displayDate = string.Format(Resources.AddedSinceAt, span.Days, dateLocation.ToString("HH:mm"));
                }
            }
            else displayDate = Resources.ListSeekios_LastPositionMoreThanOneYear;
            return displayDate;
        }

        public static string FormatDateFromNowAlertSOS(this DateTime dateLocation)
        {
            if (dateLocation == null) return Resources.ListSeekios_LastPositionNone;
            string displayDate = string.Empty;
            DateTime now = DateHelper.GetSystemTime();
            TimeSpan span = now - dateLocation;
            if (span.Days <= 365) //less than one year ago (approximately, not always exact)
            {
                if (span.TotalHours < now.Hour) //today
                {
                    displayDate = string.Format(Resources.SOSAlertTodayAt, dateLocation.ToString("HH:mm"));
                }
                else if (span.TotalHours < (now.Hour + 24)) //yesterday
                {
                    displayDate = string.Format(Resources.SOSAlertYesterdayAt, dateLocation.ToString("HH:mm"));
                }
                else //before yesterday
                {
                    displayDate = string.Format(Resources.SOSAlertSinceAt, span.Days, dateLocation.ToString("HH:mm"));
                }
            }
            else displayDate = Resources.ListSeekios_LastPositionMoreThanOneYear;
            return displayDate;
        }

        public static string FormatDateTimeFromNow(this DateTime dateLocation, bool withSeconds = true)
        {
            if (dateLocation == null) return string.Empty;
            string displayDate = string.Empty;
            // Today
            if (dateLocation.Date == DateTime.Now.Date)
            {
                displayDate = withSeconds ?
                    string.Format(Resources.TodayDateTime, dateLocation.Hour, dateLocation.Minute, dateLocation.Second) :
                    string.Format(Resources.TodayDateTimeNoSeconds, dateLocation.Hour, dateLocation.Minute);
            }
            // Yesterday
            else if (dateLocation.Date == DateTime.Now.AddDays(-1).Date)
            {
                displayDate = withSeconds ?
                    string.Format(Resources.YesterdayDateTime, dateLocation.Hour, dateLocation.Minute, dateLocation.Second) :
                    string.Format(Resources.YesterdayDateTimeNoSeconds, dateLocation.Hour, dateLocation.Minute); ;
            }
            // More than 2 days ago
            else
            {
                var totalDays = (int)(DateTime.Now.Date - dateLocation.Date).TotalDays;
                displayDate = withSeconds ?
                    string.Format(Resources.OneDayDateTime, dateLocation.Hour, dateLocation.Minute, dateLocation.Second, totalDays) :
                    string.Format(Resources.OneDayDateTimeNoSeconds, dateLocation.Hour, dateLocation.Minute, dateLocation.Second, totalDays);
            }
            return displayDate;
        }

        public static DateTime FormatJsonDateToDateTime(string jsondate)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            if (jsondate.Length >= 21)
            {
                dtDateTime = dtDateTime.AddSeconds(double.Parse(jsondate.Substring(6, jsondate.Length - 11))).ToLocalTime();
            }
            else dtDateTime = DateTime.Now.ToLocalTime();
            return dtDateTime;
        }
    }
}
