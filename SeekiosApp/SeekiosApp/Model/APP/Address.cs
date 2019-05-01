namespace SeekiosApp.Model.APP
{
    public class Address
    {
        public Address(double lat, double longitude, string address)
        {
            Latitude = lat;
            Longitude = longitude;
            AddressName = address;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AddressName { get; set; }
    }
}
