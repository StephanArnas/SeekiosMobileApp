using Foundation;
using System;
using UIKit;
using SeekiosApp.iOS.Views.BaseView;
using SeekiosApp.iOS.ControlManager;
using SeekiosApp.ViewModel;
using SeekiosApp.Model.APP;
using System.Collections.Generic;
using CoreLocation;

namespace SeekiosApp.iOS
{
    public partial class MapAllSeekiosView : BaseViewControllerMap
    {
        #region ===== Constructor =================================================================

        public MapAllSeekiosView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewWillAppear(bool animated)
        {
            App.Locator.MapAllSeekios.HasRemovedButtonAnnotationView = true;
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            App.Locator.MapAllSeekios.HasRemovedButtonAnnotationView = false;
            base.ViewWillDisappear(animated);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            if (MapViewControl != null)
            {
                MapViewControl.MapType = MapKit.MKMapType.Hybrid;
                MapViewControl.Delegate = null;
                MapViewControl.RemoveFromSuperview();
                MapViewControl.Dispose();
                MapViewControl = null;
            }
        }

        #endregion

        #region ===== Initialisze View ============================================================

        public override void SetDataAndStyleToView()
        {
            Title = Application.LocalizedString("MapAllSeekiosTitle");
        }

        public override void SetMapControlManager()
        {
            _mapControlManager = new MapControlManager(MapViewControl
                , this
                , MapZoomInButton
                , MapZoomOutButton);

            App.Locator.MapAllSeekios.MapControlManager = _mapControlManager;
            App.Locator.MapAllSeekios.InitMap();

            if (App.CurrentUserEnvironment.LsSeekios.Count != 0)
            {
                _mapControlManager.RegisterMethodes();
				var listLatLong = _mapControlManager.ConvertLatitudeLongitudeToLatLng(GetSeekiosListCordinates());
				_mapControlManager.CenterOnLocations(listLatLong, true);
            }
        }

        #endregion

        #region ===== Private Methods =============================================================

        private List<CLLocationCoordinate2D> GetSeekiosListCordinates()
        {
            var coordonates = new List<CLLocationCoordinate2D>();
            foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
            {
                if (seekios != null && seekios.LastKnownLocation_dateLocationCreation.HasValue)
                {
                    if (seekios.LastKnownLocation_latitude != App.DefaultLatitude
                        && seekios.LastKnownLocation_longitude != App.DefaultLongitude)
                    {
                        coordonates.Add(new CLLocationCoordinate2D(seekios.LastKnownLocation_latitude, seekios.LastKnownLocation_longitude));
                    }
                }
            }
            return coordonates;
        }

        #endregion
    }
}