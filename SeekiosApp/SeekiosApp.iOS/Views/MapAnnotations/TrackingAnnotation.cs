using CoreLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeekiosApp.iOS.Views.MapAnnotations
{
    public class TrackingAnnotation : BaseAnnotation
    {
        public bool IsInAlert { get; set; }

        #region ===== Constructor =================================================================

        public TrackingAnnotation(CLLocationCoordinate2D coordinate, string title, string subtitle) : base (coordinate, title, subtitle) { IsInAlert = false; }

        public TrackingAnnotation(CLLocationCoordinate2D coordinate) : this (coordinate, string.Empty, string.Empty) { }

        #endregion
    }
}