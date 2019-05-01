using Android.Gms.Maps.Model;

namespace SeekiosApp.Droid.Messages
{
    public class MarkerZoneDeleteMessage
    {
        public MarkerZoneDeleteMessage(Marker marker)
        {
            MarkerToDelete = marker;
        }
        public Marker MarkerToDelete { get; set; }
    }
}