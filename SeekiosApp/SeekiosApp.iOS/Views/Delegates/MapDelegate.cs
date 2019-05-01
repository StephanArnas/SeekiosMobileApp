using System;
using MapKit;
using UIKit;
using CoreGraphics;
using System.Drawing;
using SeekiosApp.ViewModel;
using SeekiosApp.iOS.Helper;
using Foundation;
using SeekiosApp.Extension;
using SeekiosApp.iOS.Views.MapAnnotations;
using CoreLocation;
using System.Linq;
using SeekiosApp.Model.DTO;
using System.Globalization;
using SeekiosApp.iOS.ControlManager;
using System.Text;

namespace SeekiosApp.iOS.ModeZone
{
    public class MapDelegate : MKMapViewDelegate
    {
        #region ===== Attributs ===================================================================

        private const string _idModeZoneAnotation = "ModeZoneAnotation";
        private const string _idHistoricAnnotation = "HistoricAnnotation";
        private const string _idTrackingAnnotation = "TrackingAnnotation";
        private const string _idSeekiosAnnotation = "SeekiosAnnotation";
        private UIImageView _venueView;
        private UIImage _venueImage;
        private ModeZoneFirstView _modeZonecontroller;
        private MapView _mapViewcontroller;
        private MapHistoricView _mapHistoriccontroller;
        private MapAllSeekiosView _mapAllSeekios;

        #endregion

        #region ===== Constructor =================================================================

        public MapDelegate(ModeZoneFirstView controller)
        {
            _modeZonecontroller = controller;
        }

        public MapDelegate(MapView controller)
        {
            _mapViewcontroller = controller;
        }

        public MapDelegate(MapHistoricView controller)
        {
            _mapHistoriccontroller = controller;
        }

        public MapDelegate(MapAllSeekiosView controller)
        {
            _mapAllSeekios = controller;
        }

        #endregion

        #region ===== Public Override Methodes ====================================================

        public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // we don't handle the annotation for the user
            if (annotation is MKUserLocation)
            {
                return null;
            }
            // annotation for the mode zone map
            else if (annotation is ModeZoneAnnotation)
            {
                return CreateModeZoneAnnotation(mapView, annotation);
            }
            // annotation for historic map
            else if (annotation is HistoricAnnotation)
            {
                return CreateHistoricAnnotation(mapView, annotation);
            }
            // annotation to display the seekios
            else if (annotation is SeekiosAnnotation)
            {
                return CreateSeekiosAnnotation(mapView, annotation);
            }
            // annotation to display the tracking points
            else if (annotation is TrackingAnnotation)
            {
                return CreateTrackingAnnotation(mapView, annotation);
            }
            else return null;
        }

        public override void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            // show an image view when the conference annotation view is selected
            if (view.Annotation is ModeZoneAnnotation)
            {
                _venueView = new UIImageView();
                _venueView.ContentMode = UIViewContentMode.ScaleAspectFit;
                _venueImage = UIImage.FromFile("venue.png");
                _venueView.Image = _venueImage;
                view.AddSubview(_venueView);
                UIView.Animate(0.4, () =>
                {
                    _venueView.Frame = new CGRect(-75, -75, 200, 200);
                });
            }
        }

        public override void DidDeselectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            // remove the image view when the conference annotation is deselected
            if (view.Annotation is ModeZoneAnnotation)
            {
                _venueView.RemoveFromSuperview();
                _venueView.Dispose();
                _venueView = null;
            }
        }

        public override void CalloutAccessoryControlTapped(MKMapView mapView, MKAnnotationView view, UIControl control)
        {
            if (view.Annotation != null)
            {
                //this.controller.RemovePIN((MapKit.MKAnnotation)view.Annotation);
            }
        }

        public override MKOverlayRenderer OverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (overlay is MKPolygon)
            {
                var polygon = overlay as MKPolygon;
                var polygonView = new MKPolygonRenderer(polygon);
                if (MapControlManager.IsOutOf && !ModeZoneFirstView.IsOnModeZone)
                {
                    polygonView.FillColor = UIColor.FromRGB(255, 76, 57).ColorWithAlpha((nfloat)0.4);
                    polygonView.StrokeColor = UIColor.FromRGB(255, 76, 57).ColorWithAlpha((nfloat)0.2);
                }
                else
                {
                    polygonView.FillColor = UIColor.FromRGB(98, 218, 115).ColorWithAlpha((nfloat)0.4);
                    polygonView.StrokeColor = UIColor.FromRGB(98, 218, 115).ColorWithAlpha((nfloat)0.2);
                }
                polygonView.LineWidth = 1;
                return polygonView;
            }
            else if (overlay is MKCircle)
            {
                var circle = overlay as MKCircle;
                var circleView = new MKCircleRenderer(circle);
                circleView.FillColor = UIColor.Blue;
                circleView.Alpha = 0.2f;
                return circleView;
            }
            else if (overlay is MKPolyline)
            {
                MKPolylineRenderer polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline);
                if (MapControlManager.IsOutOf && !ModeZoneFirstView.IsOnModeZone)
                {
                    polylineRenderer.FillColor = UIColor.FromRGB(255, 76, 57);
                    polylineRenderer.StrokeColor = UIColor.FromRGB(255, 76, 57);
                }
                else
                {
                    polylineRenderer.FillColor = UIColor.FromRGB(98, 218, 115);
                    polylineRenderer.StrokeColor = UIColor.FromRGB(98, 218, 115);
                }
                polylineRenderer.LineWidth = 2;
                polylineRenderer.Alpha = 0.8f;
                return polylineRenderer;
            }
            return null;
        }

        public override void ChangedDragState(MKMapView mapView, MKAnnotationView annotationView, MKAnnotationViewDragState newState, MKAnnotationViewDragState oldState)
        {
            base.ChangedDragState(mapView, annotationView, newState, oldState);
            if (newState == MKAnnotationViewDragState.Starting)
            {
                annotationView.DragState = MKAnnotationViewDragState.Dragging;
            }
            else if (newState == MKAnnotationViewDragState.Ending || newState == MKAnnotationViewDragState.Canceling)
            {
                annotationView.DragState = MKAnnotationViewDragState.None;
            }
        }

        #endregion

        #region ===== Private Methodes ============================================================

        private MKAnnotationView CreateModeZoneAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // queue for reuse the pin if it's already exist
            var annotationView = mapView.DequeueReusableAnnotation(_idModeZoneAnotation);
            if (annotationView == null)
            {
                // not existing, we create a new one 
                annotationView = new MKAnnotationView(annotation, _idModeZoneAnotation);
            }
            // image of the annotation display on the map
            if ((annotation as ModeZoneAnnotation).IsLastAnnotation)
            {
                annotationView.Image = UIImage.FromBundle("MapPinLast");
            }
            else annotationView.Image = UIImage.FromBundle("MapPin");
            annotationView.CanShowCallout = true;
            annotationView.Selected = true;
            annotationView.Draggable = true;

            return annotationView;
        }

        private MKAnnotationView CreateHistoricAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // queue for reuse the pin if it's already exist
            var annotationView = mapView.DequeueReusableAnnotation(_idHistoricAnnotation);
            if (annotationView == null)
            {
                // not existing, we create a new one 
                annotationView = new MKAnnotationView(annotation, _idHistoricAnnotation);
            }
            // image of the annotation display on the map
            annotationView.Image = UIImage.FromBundle("MapPin");
            annotationView.CanShowCallout = true;

            var seekiosLastLocationLabel = new UILabel();
            seekiosLastLocationLabel.LineBreakMode = UILineBreakMode.WordWrap;
            seekiosLastLocationLabel.Lines = new nint(1);
            seekiosLastLocationLabel.Text = ((HistoricAnnotation)annotation).Content;
            seekiosLastLocationLabel.Font = UIFont.FromName("Helvetica", 14f);
            seekiosLastLocationLabel.TextColor = UIColor.LightGray;
            seekiosLastLocationLabel.SizeToFit();
            seekiosLastLocationLabel.SetNeedsDisplay();

            // add the view in the annotation view
            annotationView.LeftCalloutAccessoryView = seekiosLastLocationLabel;

            return annotationView;
        }

        private MKAnnotationView CreateSeekiosAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // queue for reuse the pin if it's already exist
            var annotationView = mapView.DequeueReusableAnnotation(_idSeekiosAnnotation);
            if (annotationView == null)
            {
                // not existing, we create a new one 
                annotationView = new MKAnnotationView(annotation, _idSeekiosAnnotation);
            }

            // image of the annotation display on the map
            if (((SeekiosAnnotation)annotation).ImageName != null)
            {
                annotationView.Image = ((SeekiosAnnotation)annotation).ImageName;
            }
            else annotationView.Image = UIImage.FromBundle("PinSeekiosGreen");
            annotationView.CanShowCallout = true;
            annotationView.CenterOffset = new CGPoint(0, -annotationView.Frame.Size.Height * 0.5);//-imageHeight / 2);

            SeekiosDTO seekios = null;
            if (((SeekiosAnnotation)annotation).IdSeekios != 0)
            {
                seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.Idseekios == ((SeekiosAnnotation)annotation).IdSeekios);
            }
            else return null;

            // image of the seekios inside the annotation
            var imageView = new UIImageView();
            imageView.Frame = new CGRect(17.9, 4.2, 40, 40);

            imageView.Layer.CornerRadius = imageView.Frame.Size.Width / 2;
            imageView.ClipsToBounds = true;

            if (string.IsNullOrEmpty(seekios.SeekiosPicture))
            {
                imageView.Image = UIImage.FromBundle("DefaultSeekios");
            }
            else
            {
                using (var dataDecoded = new NSData(seekios.SeekiosPicture
                        , NSDataBase64DecodingOptions.IgnoreUnknownCharacters))
                {
                    imageView.Image = new UIImage(dataDecoded);
                }
            }
            annotationView.AddSubview(imageView);

            // wrap all element in a view
            var customView = new UIView();
            float xValue = 5f;

            if (App.Locator.ModeZone.IsActivityFocused)
            {
                customView.Frame = new RectangleF(0f, 0f, 160f, 75f);
            }
            else if (App.Locator.MapAllSeekios.HasRemovedButtonAnnotationView)
            {
                customView.Frame = new RectangleF(0f, 0f, 220f, 70f);
                customView.AddSubview(CreateMoreButton(10, seekios.LastKnownLocation_latitude, seekios.LastKnownLocation_longitude));
                customView.AddSubview(CreateMoreLabel(0));
                xValue = 65;
            }
            else
            {
                bool isAlert = ((SeekiosAnnotation)annotation).IsAlert;
                customView.Frame = new RectangleF(0f, 0f, 270f, 70f);
                customView.AddSubview(CreateRefreshButton(isAlert, seekios.IsInPowerSaving));
                customView.AddSubview(CreateMoreButton(70
                    , seekios.LastKnownLocation_latitude
                    , seekios.LastKnownLocation_longitude
                    , isAlert));
                customView.AddSubview(CreateRefreshLabel(isAlert));
                customView.AddSubview(CreateMoreLabel(60, isAlert));
                xValue = 115f;
            }
            customView.AddSubview(CreateSeekiosNameLabel(seekios.SeekiosName, xValue));

            if (seekios.LastKnownLocation_dateLocationCreation.HasValue)
            {
                customView.AddSubview(CreateLastPositionLabel(seekios.LastKnownLocation_dateLocationCreation.Value.FormatDateFromNow(), xValue));
            }
            else customView.AddSubview(CreateLastPositionLabel(Application.LocalizedString("DateNotAvailable"), xValue));

            // add the view in the annotation view
            annotationView.LeftCalloutAccessoryView = customView;

            return annotationView;
        }

        private MKAnnotationView CreateTrackingAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            // queue for reuse the pin if it's already exist
            var annotationView = mapView.DequeueReusableAnnotation(_idTrackingAnnotation);
            if (annotationView == null)
            {
                // not existing, we create a new one 
                annotationView = new MKAnnotationView(annotation, _idTrackingAnnotation);
            }
            // image of the annotation display on the map
            if (((TrackingAnnotation)annotation).IsInAlert)
            {
                annotationView.Image = UIImage.FromBundle("MapPinRed");
            }
            else
            {
                annotationView.Image = UIImage.FromBundle("MapPin");
            }
            annotationView.CanShowCallout = true;

            var seekiosLastLocationLabel = new UILabel();
            seekiosLastLocationLabel.LineBreakMode = UILineBreakMode.WordWrap;
            seekiosLastLocationLabel.Lines = new nint(1);
            seekiosLastLocationLabel.Text = ((TrackingAnnotation)annotation).Content;
            seekiosLastLocationLabel.Font = UIFont.FromName("Helvetica", 14f);
            seekiosLastLocationLabel.TextColor = UIColor.LightGray;
            seekiosLastLocationLabel.SizeToFit();
            seekiosLastLocationLabel.SetNeedsDisplay();

            // add the view in the annotation view
            annotationView.LeftCalloutAccessoryView = seekiosLastLocationLabel;

            return annotationView;
        }

        #region CREATE UI

        private UILabel CreateSeekiosNameLabel(string text, float x)
        {
            var seekiosNameLabel = new UILabel();
            seekiosNameLabel.Frame = new RectangleF(x, 4f, 115f, 25f);
            seekiosNameLabel.Text = text;
            seekiosNameLabel.Font = UIFont.FromName("Helvetica", 20f);
            seekiosNameLabel.AdjustsFontSizeToFitWidth = true;
            seekiosNameLabel.TextColor = UIColor.DarkGray;
            return seekiosNameLabel;
        }

        private UILabel CreateLastPositionLabel(string text, float x)
        {
            var seekiosLastLocationLabel = new UILabel();
            seekiosLastLocationLabel.Frame = new RectangleF(x, 31f, 150f, 20f);
            seekiosLastLocationLabel.Text = text;
            seekiosLastLocationLabel.Font = UIFont.FromName("Helvetica", 11f);
            seekiosLastLocationLabel.TextColor = UIColor.LightGray;
            if (MapView.IsRefreshEnable)
            {
                seekiosLastLocationLabel.Enabled = true;
            }
            else
            {
                seekiosLastLocationLabel.Enabled = false;
            }
            return seekiosLastLocationLabel;
        }

        private UIButton CreateRefreshButton(bool isAlert = false, bool isInPowerSaving = false)
        {
            var refreshButton = new UIButton(UIButtonType.Custom);
            refreshButton.Frame = new RectangleF(17f, 6f, 26f, 26f);

            if (MapView.IsRefreshEnable) refreshButton.Enabled = true;
            else refreshButton.Enabled = false;

            if (isAlert) refreshButton.SetImage(UIImage.FromBundle("RefreshSeekiosAlert"), UIControlState.Normal);
            else refreshButton.SetImage(UIImage.FromBundle("RefreshSeekios"), UIControlState.Normal);

            refreshButton.TouchUpInside += async (s, e) =>
            {
                if (_mapViewcontroller != null)
                {
                    if (await App.Locator.BaseMap.RefreshSeekiosPosition())
                    {
                        _mapViewcontroller.SetTimer(s, e);
                    }
                }
            };
            return refreshButton;
        }

        private UILabel CreateRefreshLabel(bool isAlert = false)
        {
            var refreshLabel = new UILabel();
            refreshLabel.TextAlignment = UITextAlignment.Center;
            refreshLabel.Frame = new RectangleF(0f, 31f, 60f, 20f);
            refreshLabel.Text = Application.LocalizedString("Update");
            refreshLabel.Font = UIFont.FromName("Helvetica", 11f);
            if (isAlert)
            {
                refreshLabel.TextColor = UIColor.FromRGB(255, 76, 57);
            }
            else refreshLabel.TextColor = UIColor.FromRGB(98, 218, 115);
            return refreshLabel;
        }

        private UIButton CreateMoreButton(float x, double latitude, double longitude, bool isAlert = false)
        {
            var moreButton = new UIButton(UIButtonType.Custom);
            moreButton.Frame = new RectangleF(x, 4f, 30f, 30f);
            if (isAlert) moreButton.SetImage(UIImage.FromBundle("MoreAlert"), UIControlState.Normal);
            else moreButton.SetImage(UIImage.FromBundle("More"), UIControlState.Normal);
            moreButton.TouchUpInside += (s, e) =>
            {
                var alert = AlertControllerHelper.CreateAlertOnMarkerMap(() =>
                {
                    var coordinate = new CLLocationCoordinate2D(latitude, longitude);
                    var mapItem = new MKMapItem(new MKPlacemark(coordinate));
                    mapItem.OpenInMaps(new MKLaunchOptions() { MapCenter = coordinate });
                }
                , () =>
                {
                    var item = FromObject(SeekiosApp.Helper.StringHelper.GoogleMapLinkShare(latitude, longitude));
                    var activityItems = new NSObject[] { item };
                    UIActivity[] applicationActivities = null;
                    var activityController = new UIActivityViewController(activityItems, applicationActivities);
                    if (_mapViewcontroller != null) _mapViewcontroller.PresentViewController(activityController, true, null);
                    else if (_mapHistoriccontroller != null) _mapHistoriccontroller.PresentViewController(activityController, true, null);
                    else if (_mapAllSeekios != null) _mapAllSeekios.PresentViewController(activityController, true, null);
                    else if (_modeZonecontroller != null) _modeZonecontroller.PresentViewController(activityController, true, null);
                }
                , () =>
                {
                    var geoCode2 = new CLGeocoder();
                    geoCode2.ReverseGeocodeLocation(new CLLocation(latitude, longitude),
                        (placemarks, error) =>
                        {
                            if (placemarks?.Count() > 0)
                            {
                                UIPasteboard.General.String = FormatSeekiosAdress(placemarks.Last());
                            }
                            else
                            {
                                var alert2 = AlertControllerHelper.CreateAlertOnMarkerMapNoAdress();
                                if (_mapViewcontroller != null)
                                {
                                    _mapViewcontroller.PresentViewController(alert2, true, null);
                                }
                                else if (_mapHistoriccontroller != null)
                                {
                                    _mapHistoriccontroller.PresentViewController(alert2, true, null);
                                }
                                else if (_mapAllSeekios != null)
                                {
                                    _mapAllSeekios.PresentViewController(alert2, true, null);
                                }
                                else if (_modeZonecontroller != null)
                                {
                                    _modeZonecontroller.PresentViewController(alert2, true, null);
                                }
                            }
                        });
                });

                var geoCode = new CLGeocoder();
                geoCode.ReverseGeocodeLocation(new CLLocation(latitude, longitude),
                    (placemarks, error) =>
                    {
                        if (placemarks?.Count() > 0)
                        {
                            alert.Title = FormatSeekiosAdress(placemarks.Last());
                        }
                        else alert.Title = Application.LocalizedString("NoAdressSeekios");
                    });

                if (_mapViewcontroller != null)
                {
                    _mapViewcontroller.PresentViewController(alert, true, null);
                }
                else if (_mapHistoriccontroller != null)
                {
                    _mapHistoriccontroller.PresentViewController(alert, true, null);
                }
                else if (_mapAllSeekios != null)
                {
                    _mapAllSeekios.PresentViewController(alert, true, null);
                }
                else if (_modeZonecontroller != null)
                {
                    _modeZonecontroller.PresentViewController(alert, true, null);
                }
            };
            return moreButton;
        }

        private UILabel CreateMoreLabel(float x, bool isAlert = false)
        {
            var shareLabel = new UILabel();
            shareLabel.TextAlignment = UITextAlignment.Center;
            shareLabel.Frame = new RectangleF(x, 31f, 50f, 20f);
            shareLabel.Text = Application.LocalizedString("More");
            shareLabel.Font = UIFont.FromName("Helvetica", 11f);
            if (isAlert)
            {
                shareLabel.TextColor = UIColor.FromRGB(255, 76, 57);
            }
            else shareLabel.TextColor = UIColor.FromRGB(98, 218, 115);
            return shareLabel;
        }

        private string FormatSeekiosAdress(CLPlacemark placemark)
        {
            StringBuilder result = new StringBuilder();

            if (!string.IsNullOrEmpty(placemark.SubThoroughfare))
            {
                result.Append(placemark.SubThoroughfare + ", ");
            }
            if (!string.IsNullOrEmpty(placemark.Thoroughfare))
            {
                result.Append(placemark.Thoroughfare + ", ");
            }
            if (!string.IsNullOrEmpty(placemark.PostalCode))
            {
                result.Append(placemark.PostalCode + ", ");
            }
            if (!string.IsNullOrEmpty(placemark.Locality))
            {
                result.Append(placemark.Locality + ", ");
            }
            if (!string.IsNullOrEmpty(placemark.AdministrativeArea))
            {
                result.Append(placemark.AdministrativeArea + ", ");
            }

            result.Append(placemark.Country);

            return result.ToString();
        }

        #endregion

        #endregion
    }
}