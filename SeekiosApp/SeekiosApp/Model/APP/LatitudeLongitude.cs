namespace SeekiosApp.Model.APP
{
    public class LatitudeLongitude
    {
        public LatitudeLongitude(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}", Latitude, Longitude);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var l = obj as LatitudeLongitude;
            if (l.Latitude == Latitude && l.Longitude == Longitude) return true;
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
