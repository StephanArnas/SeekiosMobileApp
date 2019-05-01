using System;

namespace SeekiosApp.Helper
{
    public class DateHelper
    {
        public static DateTime GetDateTimeFromOneMonthAgo(DateTime? baseTime = null)
        {
            if (baseTime == null)
            {
                return GetSystemTime().AddMonths(-1);
            }
            else return baseTime.Value.AddMonths(-1);
        }

        public static DateTime GetDateTimeInOneMonthFromNow(DateTime? baseTime = null)
        {
            if (baseTime == null)
            {
                return GetSystemTime().AddMonths(1);
            }
            else return baseTime.Value.AddMonths(1);
        }

        public static DateTime GetSystemTime()
        {
            return DateTime.UtcNow; //this way no mistakes between UtcNow or Now
        }

        public static DateTime GetLastSecondOfDay(DateTime when)
        {
            return when.AddDays(1).AddSeconds(-1);
        }

        public static DateTime ComputeNextDayOfMonth(int dayOfMonth)
        {
            DateTime refillDate;
            {
                var baseDate = GetSystemTime();
                var year = baseDate.Year; int month = baseDate.Month;
                //if we decide to change the refill day to last day, we should check if day exists with above method ?
                var dateDeRenouvellementDuMois = new DateTime(year, month, dayOfMonth).ToUniversalTime();
                //we do not use the addMonth trick because we want a specific day
                if (dateDeRenouvellementDuMois > baseDate)
                {
                    refillDate = dateDeRenouvellementDuMois;
                }
                else
                {
                    if (month >= 12)
                    {
                        month = 1; year += 1;
                    }
                    else
                    {
                        month += 1;
                    }
                    refillDate = new DateTime(year, month, dayOfMonth); //if we decide to change the refill day to last day, we should check if day exists with above method ?
                }
            }
            return refillDate.ToLocalTime();
        }

        public static string TimeLeftUntilNextNoon()
        {
            var now = DateTime.Now;
            var nextNoon = new DateTime(now.Year, now.Month, now.Day, 12, 00, 00);
            if (now.Hour > 12)
            {
                nextNoon = nextNoon.AddDays(1);
            }
            var remain = nextNoon - now;
            if (remain.TotalMinutes > 60)
            {
                return string.Format("{0}h", (int)(remain.TotalMinutes / 60));
            }
            else return string.Format("{0}min", (int)remain.TotalMinutes);
        }
    }
}
