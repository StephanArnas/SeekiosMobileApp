using MapKit;
using System;
using CoreLocation;
using Foundation;

namespace SeekiosApp.iOS.Views.MapAnnotations
{
    public class BaseAnnotation : MKAnnotation
    {
        #region ===== Attributs ===================================================================

        private CLLocationCoordinate2D _coordinate;
        private string _title, _subtitle;

        #endregion

        #region ===== Properties ==================================================================

        public string Id { get; set; }

        public string Content { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public BaseAnnotation()
        {
        }

        public BaseAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle)
        {
            SetCoordinate(coordinate);
            _title = title;
            _subtitle = subtitle;
            Id = Guid.NewGuid().ToString();
        }

        public BaseAnnotation(CLLocationCoordinate2D coordinate) : this(coordinate, string.Empty, string.Empty) { }

        #endregion

        #region ===== Public Overrides Methodes ===================================================

        public override CLLocationCoordinate2D Coordinate
        {
            get { return _coordinate; }
        }

        [Export("_original_setCoordinate:")]
        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            WillChangeValue("coordinate");
            _coordinate = value;
            DidChangeValue("coordinate");
        }

        public override string Title { get { return _title; } }

        public override string Subtitle { get { return _subtitle; } }

        #endregion
    }
}
