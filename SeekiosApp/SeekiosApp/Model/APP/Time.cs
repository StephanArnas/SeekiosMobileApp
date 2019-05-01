using System;

namespace SeekiosApp.Model.APP
{
    public class Time
    {
        public Time()
        {
            Hour = DateTime.Now.Hour;
            Minute = DateTime.Now.Minute;
        }

        public Time(int hour, int minute)
        {
            Hour = hour;
            Minute = minute;
        }
        public int Hour { get; set; }
        public int Minute { get; set; }

        public override string ToString()
        {
            return string.Format("{0:00}:{1:00}", Hour, Minute);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Time time = obj as Time;
            if (time.Hour == Hour && time.Minute == Minute) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
