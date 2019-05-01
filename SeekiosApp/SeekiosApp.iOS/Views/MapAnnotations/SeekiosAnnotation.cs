using CoreLocation;
using UIKit;
namespace SeekiosApp.iOS.Views.MapAnnotations
{
    public class SeekiosAnnotation : BaseAnnotation
    {
		public UIImage ImageName { get; set; }
        public int IdSeekios { get; set; }
		public bool IsAlert { get; set; }

        #region ===== Constructor =================================================================

        public SeekiosAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle) : base(coordinate, title, subtitle) { this.ImageName = null; }

        public SeekiosAnnotation(CLLocationCoordinate2D coordinate): this (coordinate, " ", string.Empty) { this.ImageName = null; }

		public SeekiosAnnotation(CLLocationCoordinate2D coordinate, UIImage imgSeekios ): this (coordinate, " ", string.Empty) { this.ImageName = imgSeekios; }

        #endregion
    }
}