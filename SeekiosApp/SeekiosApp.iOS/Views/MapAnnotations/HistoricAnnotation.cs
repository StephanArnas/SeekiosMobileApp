using CoreLocation;

namespace SeekiosApp.iOS.Views.MapAnnotations
{
    public class HistoricAnnotation : BaseAnnotation
    {
        #region ===== Constructor =================================================================

        public HistoricAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle) : base (coordinate, title, subtitle) { }

        public HistoricAnnotation(CLLocationCoordinate2D coordinate) : this (coordinate, string.Empty, string.Empty) { }

        #endregion
    }
}