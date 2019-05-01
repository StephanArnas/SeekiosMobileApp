using CoreLocation;

namespace SeekiosApp.iOS.Views.MapAnnotations
{
    public class ModeZoneAnnotation : BaseAnnotation
    {
        #region ===== Properties ==================================================================

        public bool IsLastAnnotation { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public ModeZoneAnnotation(CLLocationCoordinate2D coordinate
            , string title
            , string subtitle
            , bool isLastAnnotation = false) 
            : base (coordinate, title, subtitle)
        {
            IsLastAnnotation = isLastAnnotation;
        }

        public ModeZoneAnnotation(CLLocationCoordinate2D coordinate
            , bool isLastAnnotation = false) 
            : this (coordinate, string.Empty, string.Empty, isLastAnnotation) {  }

        #endregion
    }
}