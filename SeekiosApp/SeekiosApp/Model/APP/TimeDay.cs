using System;

namespace SeekiosApp.Model.APP
{
    public class TimeDay
    {
        public TimeDay()
        {
            Hour = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;
        }

        public TimeDay(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }

        public TimeDay(int hour
            , int minute
            , bool isActiveMonday
            , bool isActiveTuesday
            , bool isActiveWednesday
            , bool isActiveThursday
            , bool isActiveFriday
            , bool isActiveSaturday
            , bool isActiveSunday)
        {
            Hour = hour;
            Minute = minute;
            Monday = isActiveMonday;
            Tuesday = isActiveTuesday;
            Wednesday = isActiveWednesday;
            Thursday = isActiveThursday;
            Friday = isActiveFriday;
            Saturday = isActiveSaturday;
            Sunday = isActiveSunday;
        }

        public int Hour { get; set; }

        public int Minute { get; set; }

        public bool Monday
        {
            get
            {
                return _monday;
            }
            set
            {
                _monday = value;
            }
        }
        private bool _monday = true;

        public bool Tuesday
        {
            get
            {
                return _tuesday;
            }
            set
            {
                _tuesday = value;
            }
        }
        private bool _tuesday = true;

        public bool Wednesday
        {
            get
            {
                return _wednesday;
            }
            set
            {
                _wednesday = value;
            }
        }
        private bool _wednesday = true;

        public bool Thursday
        {
            get
            {
                return _thursday;
            }
            set
            {
                _thursday = value;
            }
        }
        private bool _thursday = true;

        public bool Friday
        {
            get
            {
                return _friday;
            }
            set
            {
                _friday = value;
            }
        }
        private bool _friday = true;

        public bool Saturday
        {
            get
            {
                return _saturday;
            }
            set
            {
                _saturday = value;
            }
        }
        private bool _saturday = true;

        public bool Sunday
        {
            get
            {
                return _sunday;
            }
            set
            {
                _sunday = value;
            }
        }
        private bool _sunday = true;

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}", Hour, Minute);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            TimeDay time = obj as TimeDay;
            if (time.Hour == Hour && time.Minute == Minute) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
