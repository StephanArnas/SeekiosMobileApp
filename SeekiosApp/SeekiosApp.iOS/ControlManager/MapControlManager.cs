using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using System.Threading.Tasks;
using MapKit;
using UIKit;
using SeekiosApp.ViewModel;
using CoreLocation;
using SeekiosApp.iOS.ModeZone;
using SeekiosApp.Enum;
using SeekiosApp.Model;
using System.Linq;
using SeekiosApp.iOS.Views.MapAnnotations;
using SeekiosApp.Extension;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using CoreGraphics;
using SeekiosApp.iOS.Helper;

namespace SeekiosApp.iOS.ControlManager
{
    public class MapControlManager : IMapControlManager, IDisposable
    {
        #region ===== Constants ===================================================================

        private const int IN_TIME_ACCURACY = 20;
        private const int NUMBER_OF_ADDRESSES = 10;
        private const int INTIME_ZOOM_LEVEL = 18;

        #endregion

        #region ===== Attributs ===================================================================

        private List<BaseAnnotation> _annotations;
        private List<SeekiosMarkerIdAssociation> _annotationIdAssociation;

        private MapDelegate _mapDelegate = null;
        private UIViewController _controller = null;
        private UILongPressGestureRecognizer _recognizer = null;
        private UIButton _focusOnSeekiosButton = null;
        private UIButton _focusOnZoneButton = null;
        private UIButton _changeMapTypeButton = null;
        private UIButton _zoomInButton = null;
        private UIButton _zoomOutButton = null;
        private UIButton _editZoneButton = null;
        private UIButton _undoButton = null;
        private UIButton _nextButton = null;
        private MKCircle _accuracyArea = null;
        private string _idSeekios = string.Empty;
        private bool _isOnPointAdding = false; //_dragStarted = false;

        #endregion

        #region ===== Properties ==================================================================

        public MKMapView MapViewControl { get; set; }

        public MKAnnotation SelectedAnnotation { get; set; }

        public MKAnnotation SelectedLocationHistory { get; set; }

        public MKAnnotation SelectedPointsOfRoute { get; set; }

        public MKPolyline RoutePolyline { get; set; }

        public MKPolygon ZonePolygon { get; set; }

        public List<MKAnnotation> PointsOfRoute { get; set; }

        public List<MKAnnotation> PointsOfZone { get; set; }

        public ObservableCollection<Action> UndoActions { get; set; }

        public bool IsOnPointAdding
        {
            get { return _isOnPointAdding; }
            set
            {
                _isOnPointAdding = value;
                //if (_isOnPointAdding)
                //{
                //    _editZoneButton.SetImage(UIImage.FromBundle("EditZoneSelected"), UIControlState.Normal);
                //}
                //else
                //{
                //    _editZoneButton.SetImage(UIImage.FromBundle("EditZone"), UIControlState.Normal);
                //}
            }
        }

        public static bool IsOutOf = false;

        public event Func<object, EventArgs, Task> OnInitTrackingRouteComplete;
        public event EventHandler<string> SeekiosMarkerClicked;
        public event EventHandler UserLocationChanged;
        public event EventHandler ZoneInformationUpdated;

        #endregion

        #region ===== Constructors ================================================================

        [PreferredConstructor]
        public MapControlManager(MKMapView mapViewControl
            , UIViewController controller
            , UIButton focusOnSeekiosButton
            , UIButton changeMapTypeButton
            , UIButton zoomInButton
            , UIButton zoomOutButton
            , string idSeekios)
        {
            MapViewControl = mapViewControl;
            _controller = controller;
            _focusOnSeekiosButton = focusOnSeekiosButton;
            _changeMapTypeButton = changeMapTypeButton;
            _zoomInButton = zoomInButton;
            _zoomOutButton = zoomOutButton;
            _idSeekios = idSeekios;

            if (_changeMapTypeButton != null)
            {
                if (App.Locator.BaseMap.IsInNormalMode)
                {
                    _changeMapTypeButton.SetImage(UIImage.FromBundle("MapTypeSatellite"), UIControlState.Normal);
                }
                else
                {
                    _changeMapTypeButton.SetImage(UIImage.FromBundle("MapTypePlan"), UIControlState.Normal);
                }
                ChangeMapType(App.Locator.BaseMap.IsInNormalMode);
            }

            _annotations = new List<BaseAnnotation>();
            _annotationIdAssociation = new List<SeekiosMarkerIdAssociation>();

            try
            {
                SetShadows(_focusOnSeekiosButton);
                SetShadows(_changeMapTypeButton);
                SetShadows(_zoomInButton);
                SetShadows(_zoomOutButton);
            }
            catch (Exception) { }

            PointsOfRoute = new List<MKAnnotation>();
        }

        // All Seekios Constructor 
        public MapControlManager(MKMapView mapViewControl
            , UIViewController controller
            , UIButton zoomInButton
            , UIButton zoomOutButton)
        {
            MapViewControl = mapViewControl;
            _controller = controller;
            _zoomInButton = zoomInButton;
            _zoomOutButton = zoomOutButton;

            _annotations = new List<BaseAnnotation>();
            _annotationIdAssociation = new List<SeekiosMarkerIdAssociation>();

            try
            {
                SetShadows(_focusOnSeekiosButton);
                SetShadows(_zoomInButton);
                SetShadows(_zoomOutButton);
            }
            catch (Exception) { }

            PointsOfRoute = new List<MKAnnotation>();
        }

        public MapControlManager(MKMapView mapViewControl
            , UIViewController controller
            , UIButton focusOnSeekiosButton
            , UIButton focusOnZoneButton
            , UIButton changeMapTypeButton
            , UIButton zoomInButton
            , UIButton zoomOutButton
            , string idSeekios)
            : this(mapViewControl
            , controller
            , focusOnSeekiosButton
            , changeMapTypeButton
            , zoomInButton
            , zoomOutButton
            , idSeekios)
        {
            _focusOnZoneButton = focusOnZoneButton;

            try
            {
                SetShadows(_focusOnZoneButton);
            }
            catch (Exception) { }
        }

        public MapControlManager(MKMapView mapViewControl
            , UIViewController controller
            , UIButton focusOnSeekiosButton
            , UIButton focusOnZoneButton
            , UIButton changeMapTypeButton
            , UIButton zoomInButton
            , UIButton zoomOutButton
            , UIButton editZoneButton
            , UIButton undoButton
            , UIButton nextButton
            , string idSeekios)
            : this(mapViewControl
            , controller
            , focusOnSeekiosButton
            , changeMapTypeButton
            , zoomInButton
            , zoomOutButton
            , idSeekios)
        {
            _focusOnZoneButton = focusOnZoneButton;
            //_editZoneButton = editZoneButton;
            _undoButton = undoButton;
            _nextButton = nextButton;
            _recognizer = new UILongPressGestureRecognizer(AddPointToZone);
            MapViewControl.AddGestureRecognizer(_recognizer);
            _recognizer.MinimumPressDuration = 0.08;

            try
            {
                //SetShadows(_editZoneButton);
                SetShadows(_undoButton);
                SetShadows(_focusOnZoneButton);
            }
            catch (Exception) { }

            PointsOfZone = new List<MKAnnotation>();
            UndoActions = new ObservableCollection<Action>();
        }

        #endregion

        #region ===== Public Methods ==============================================================

        public void InitMap(float zoomLevel
            , bool zoom = false
            , bool compass = true
            , bool mapToolbar = true)
        {
            if (_controller.GetType() == typeof(MapView))
            {
                _mapDelegate = new MapDelegate(_controller as MapView);
            }
            else if (_controller.GetType() == typeof(ModeZoneFirstView))
            {
                _mapDelegate = new MapDelegate(_controller as ModeZoneFirstView);
            }
            else if (_controller.GetType() == typeof(MapHistoricView))
            {
                _mapDelegate = new MapDelegate(_controller as MapHistoricView);
            }
            else if (_controller.GetType() == typeof(MapAllSeekiosView))
            {
                _mapDelegate = new MapDelegate(_controller as MapAllSeekiosView);
            }

            MapViewControl.Delegate = _mapDelegate;
        }

        public void CenterInLocalisation(double latitude
            , double longitude
            , float zoomLevel
            , bool withAnimation = false)
        {
            var mapCenter = new CLLocationCoordinate2D(latitude, longitude);
            if (zoomLevel != 0)
            {
                var span = new MKCoordinateSpan(0, 360 / Math.Pow(2, zoomLevel) * MapViewControl.Frame.Size.Width / 256);
                var mapRegion = new MKCoordinateRegion(mapCenter, span);
                MapViewControl.CenterCoordinate = mapCenter;
                MapViewControl.SetRegion(mapRegion, withAnimation);
            }
            else
            {
                MapViewControl.CenterCoordinate = mapCenter;
            }
        }

        public void ChangeAnnotationLocation(string seekiosId
            , double latitude
            , double longitude
            , double accuracy)
        {
            var idMarkerAsso = _annotationIdAssociation.FirstOrDefault(id => id.SeekiosId == seekiosId);
            if (idMarkerAsso != null)
            {
                var annotation = _annotations.FirstOrDefault(m => m.Id.Equals(idMarkerAsso.MarkerId));
                if (annotation != null)
                {
                    var mapCenter = new CLLocationCoordinate2D(latitude, longitude);
                    annotation.SetCoordinate(mapCenter);
                    MapViewControl.SelectAnnotation(annotation, true);
                    MapViewControl.SetCenterCoordinate(mapCenter, true);
                }
            }
        }

        public bool CenterInMarker(string seekiosId
            , bool showAnnotation = false
            , bool withAnimation = false)
        {
            var idAnnotationAsso = _annotationIdAssociation.FirstOrDefault(id => id.SeekiosId == seekiosId);
            if (idAnnotationAsso == null) return false;
            var seekiosAnnotation = _annotations.FirstOrDefault(annotation => annotation.Id.Equals(idAnnotationAsso.MarkerId));
            if (seekiosAnnotation == null) return false;

            var seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(f => f.Idseekios.ToString() == seekiosId);
            if (seekios != null)
            {
                if (seekios.LastKnownLocation_accuracy > 0)
                {
                    var centerPostion = new CLLocationCoordinate2D(seekios.LastKnownLocation_latitude
                        , seekios.LastKnownLocation_longitude);
                    var circle = MKCircle.Circle(centerPostion
                        , seekios.LastKnownLocation_accuracy);
                    MapViewControl.SetVisibleMapRect(circle.BoundingMapRect, new UIEdgeInsets(50, 20, 100, 20), true);
                }
                else
                {
                    CenterInLocalisation(seekios.LastKnownLocation_latitude
                        , seekios.LastKnownLocation_longitude
                        , (float)ZoomLevelEnum.BigZoom
                        , withAnimation);
                }
            }

            if (showAnnotation)
            {
                SelectedAnnotation = seekiosAnnotation;
                MapViewControl.SelectAnnotation(seekiosAnnotation, withAnimation);
            }

            return true;
        }

        public void CenterOnLocations(List<LatitudeLongitude> locations
            , bool with_animation = false)
        {
            if (locations == null || locations.Count == 0) return;

            var minLat = locations.Select(el => el.Latitude).Min();
            var minLong = locations.Select(el => el.Longitude).Min();

            var maxLat = locations.Select(el => el.Latitude).Max();
            var maxLong = locations.Select(el => el.Longitude).Max();

            var coordinateMin = new CLLocationCoordinate2D(minLat, minLong);
            var coordinateMax = new CLLocationCoordinate2D(maxLat, maxLong);

            var upperLeft = MKMapPoint.FromCoordinate(coordinateMin);
            var lowerRight = MKMapPoint.FromCoordinate(coordinateMax);

            var mapRect = new MKMapRect(upperLeft.X
                , upperLeft.Y
                , lowerRight.X - upperLeft.X
                , lowerRight.Y - upperLeft.Y);

            var region = MKCoordinateRegion.FromMapRect(mapRect);

            MapViewControl.SetVisibleMapRect(mapRect
                , new UIEdgeInsets(50, 20, 100, 20)
                , true);

            try
            {
                if (region.Center.Latitude > -89 && region.Center.Latitude < 89 && region.Center.Longitude > -179 && region.Center.Longitude < 179)
                {
                    if (region.Span.LatitudeDelta < 0)
                        region.Span.LatitudeDelta = 0.0;
                    if (region.Span.LongitudeDelta < 0)
                        region.Span.LongitudeDelta = 0.0;

                    MapViewControl.SetRegion(region, with_animation);
                }
            }
            catch (Exception)
            {
                //UIAlertView alert = new UIAlertView("Exception", String.Format("{0} \n {1}?", e.Message, e.StackTrace), null, "Cancel");
                //alert.Show();
            }
        }

        public void CenterOnZone()
        {
            if (ZonePolygon != null && ZonePolygon.Points != null)
            {
                MapViewControl.SetVisibleMapRect(ZonePolygon.BoundingMapRect, new UIEdgeInsets(50, 20, 100, 20), true);
            }
        }

        public void CreateSeekiosMarkerAsync(string seekiosId
            , string title
            , string imageBase64
            , DateTime? lastLocationDate
            , double latitude
            , double longitude
            , double accuracy
            , bool isDontMove = false
            , bool isInAlert = false)
        {
            var seekiosLocation = new CLLocationCoordinate2D(latitude, longitude);
            var seekiosImg = CreateSeekiosBitmap(imageBase64, accuracy, isInAlert, isDontMove);

            var seekiosAnnotation = new SeekiosAnnotation(seekiosLocation, seekiosImg);
            seekiosAnnotation.IdSeekios = int.Parse(seekiosId);
            MapViewControl.AddAnnotations(seekiosAnnotation);
            SelectedAnnotation = seekiosAnnotation;
            CreateAccuracyArea(latitude, longitude, accuracy);
            seekiosAnnotation.IsAlert = isInAlert;

            _annotations.Add(seekiosAnnotation);
            _annotationIdAssociation.Add(new SeekiosMarkerIdAssociation(seekiosId, seekiosAnnotation.Id));

            if (App.Locator.ModeZone.IsOnEditMode && App.Locator.ModeZone.IsActivityFocused)
            {
            }
            else
            {
                MapViewControl.SelectAnnotation(seekiosAnnotation, true);
            }
        }

        public MKCircle CreateAccuracyArea(double latitude
            , double longitude
            , double accuracy)
        {
            if (_accuracyArea != null)
            {
                MapViewControl.RemoveOverlay(_accuracyArea);
            }
            if (accuracy <= 0) return null;
            var centerPostion = new CLLocationCoordinate2D(latitude, longitude);
            var circle = MKCircle.Circle(centerPostion, accuracy);
            MapViewControl.AddOverlay(circle);
            MapViewControl.SetVisibleMapRect(circle.BoundingMapRect, new UIEdgeInsets(50, 20, 100, 20), true);
            _accuracyArea = circle;

            return _accuracyArea;
        }

        public void RemoveAllMarkers()
        {
            foreach (var annotation in _annotations)
            {
                annotation.Dispose();
            }
            _annotations.Clear();
            _annotationIdAssociation.Clear();
        }

        public void ChangeMapType(bool isInNormalMode)
        {
            App.Locator.BaseMap.SaveDataChangeMap(isInNormalMode);
            if (isInNormalMode)
            {
                MapViewControl.MapType = MKMapType.Standard;
            }
            else
            {
                MapViewControl.MapType = MKMapType.Satellite;
            }
        }

        #region Zone

        public void CreateUndoActionsToRemoveActualZone()
        {

        }

        public void CreateZone(List<LatitudeLongitude> coordonates
            , bool isInAlert
            , bool isLastMarker = false
            , int index = 0)
        {
            if (coordonates?.Count < 1) return;
            DeleteZone();

            var listLocation = new List<CLLocationCoordinate2D>();
            coordonates.ForEach(coord => listLocation.Add(new CLLocationCoordinate2D { Latitude = coord.Latitude, Longitude = coord.Longitude }));

            // Create an array of coordinates from allPins
            var finalCoordonates = new CLLocationCoordinate2D[listLocation.Count + 1];

            int i = 0;
            foreach (var currentPin in listLocation)
            {
                finalCoordonates[i] = currentPin;
                i++;
            }
            finalCoordonates[i] = finalCoordonates[0];

            // Add an overlay of the Zone
            ZonePolygon = MKPolygon.FromCoordinates(finalCoordonates);
            MapViewControl.AddOverlay(ZonePolygon);
        }

        public void DeleteZone()
        {
            if (ZonePolygon != null)
            {
                MapViewControl.RemoveOverlay(ZonePolygon);
            }
            ZonePolygon = null;
        }

        public List<LatitudeLongitude> ConvertLatitudeLongitudeToLatLng(List<CLLocationCoordinate2D> coords)
        {
            var result = new List<LatitudeLongitude>();
            foreach (var coord in coords)
            {
                result.Add(new LatitudeLongitude(coord.Latitude, coord.Longitude));
            }
            return result;
        }

        /// <summary>
        /// Verify if a Seekios is in a zone when configuring a mode zone
        /// </summary>
        /// <param name="seekiosPosition"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        public bool IsSeekiosInZone(LatitudeLongitude seekiosPosition, List<LatitudeLongitude> zone)
        {
            int i = 0;
            double angle = 0;
            //	LatLng p1 = new LatLng(0, 0);
            //	LatLng p2 = new LatLng(0, 0);

            var p1 = new CLLocationCoordinate2D(0, 0);
            var p2 = new CLLocationCoordinate2D(0, 0);

            int n = zone.Count();

            for (i = 0; i < n; i++)
            {
                p1.Latitude = zone[i].Latitude - seekiosPosition.Latitude;
                p1.Longitude = zone[i].Longitude - seekiosPosition.Longitude;
                p2.Latitude = zone[(i + 1) % n].Latitude - seekiosPosition.Latitude;
                p2.Longitude = zone[(i + 1) % n].Longitude - seekiosPosition.Longitude;
                angle += GetAngle2D(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);
            }

            if (Math.Abs(angle) < PI)
                return (false);
            else
                return (true);
        }

        private const double TWOPI = 2 * Math.PI;
        private const double PI = Math.PI;

        private double GetAngle2D(double x1, double y1, double x2, double y2)
        {
            double dtheta, theta1, theta2;

            theta1 = Math.Atan2(y1, x1);
            theta2 = Math.Atan2(y2, x2);
            dtheta = theta2 - theta1;
            while (dtheta > PI)
                dtheta -= TWOPI;
            while (dtheta < -PI)
                dtheta += TWOPI;

            return (dtheta);
        }

        #endregion

        #region Location history

        public void ChangeSelectedLocationHistory(LocationDTO coordonate)
        {
            // remove the old position if its exist
            if (SelectedLocationHistory != null) MapViewControl.RemoveAnnotation(SelectedLocationHistory);

            // if we don't need to select, exit
            if (coordonate == null) return;

            // remplacing with the new annotation
            var location = new CLLocationCoordinate2D(coordonate.Latitude, coordonate.Longitude);
            SelectedLocationHistory = new HistoricAnnotation(location
                , " "
                , string.Empty);
            ((HistoricAnnotation)SelectedLocationHistory).Content = coordonate.DateLocationCreation.FormatDateFromNow();
            MapViewControl.AddAnnotation(SelectedLocationHistory);

            // display accuracy if necessary
            CreateAccuracyArea(coordonate.Latitude
                , coordonate.Longitude
                , coordonate.Accuracy);

            //// center the new annotation on the map
            //if (!App.Locator.ModeZone.IsOnEditMode)
            //{
            //    if (coordonate.Accuracy == 0)
            //    {
            //        CenterInLocalisation(coordonate.Latitude
            //            , coordonate.Longitude
            //            , 0
            //            , true);
            //    }
            //    else
            //    {
            //        CenterInLocalisation(coordonate.Latitude
            //            , coordonate.Longitude
            //            , 0
            //            , true);
            //        //// display the annotation with accurency and focus on the circle
            //        //_mapViewControl.MapRectThatFits(CreateAccuracyArea(coordonate.Latitude
            //        //, coordonate.Longitude
            //        //, coordonate.Accuracy).BoundingMapRect);
            //    }
            //}

            // display the annotation view
            MapViewControl.SelectAnnotation(SelectedLocationHistory, true);
            //CreateAccuracyArea(coordonate.Latitude
            //    , coordonate.Longitude
            //    , coordonate.Accuracy);
        }

        public void RemoveSelectedHistoryMarker()
        {
            if (SelectedLocationHistory == null) return;
            MapViewControl.RemoveAnnotation(SelectedLocationHistory);
        }

        #endregion

        #region Mode Tracking

        public void CreateRoute(List<LocationDTO> coordonates)
        {
            CreateRouteBackground(coordonates);
            CreateRouteForeground(coordonates);
        }

        public void CreateRouteBackground(List<LocationDTO> coordonates)
        {
            _controller.InvokeOnMainThread(() =>
            {
                // Clean the map
                if (RoutePolyline != null)
                {
                    MapViewControl.RemoveOverlay(RoutePolyline);
                }
                if (PointsOfRoute.Count > 0)
                {
                    MapViewControl.RemoveAnnotations(PointsOfRoute.ToArray());
                    PointsOfRoute.Clear();
                }
                if (SelectedPointsOfRoute != null)
                {
                    MapViewControl.RemoveAnnotation(SelectedPointsOfRoute);
                }
            });

            if (coordonates.Count != 0)
            {
                foreach (var coordonate in coordonates)
                {
                    if (IsOutOf)
                    {
                        var trackingAnnotation = new TrackingAnnotation(new CLLocationCoordinate2D(coordonate.Latitude, coordonate.Longitude)
                            , " ", string.Empty);
                        trackingAnnotation.Content = coordonate.DateLocationCreation.FormatDateFromNow();
                        trackingAnnotation.IsInAlert = IsOutOf;
                        PointsOfRoute.Add(trackingAnnotation);
                    }
                    else
                    {
                        var trackingAnnotation = new TrackingAnnotation(new CLLocationCoordinate2D(coordonate.Latitude, coordonate.Longitude)
                            , " ", string.Empty);
                        trackingAnnotation.Content = coordonate.DateLocationCreation.FormatDateFromNow();
                        trackingAnnotation.IsInAlert = IsOutOf;
                        PointsOfRoute.Add(trackingAnnotation);
                    }
                }
            }
            OnInitTrackingRouteComplete?.Invoke(null, null);
        }

        public void CreateRouteForeground(List<LocationDTO> coordonates)
        {
            if (PointsOfRoute != null && PointsOfRoute.Count != 0)
            {
                foreach (var annotation in PointsOfRoute)
                {
                    MapViewControl.AddAnnotation(annotation);
                }

                var positionWithSeekios = PointsOfRoute.Select(s => s.Coordinate).ToList();
                if (SelectedAnnotation != null && !positionWithSeekios.Contains(SelectedAnnotation.Coordinate))
                {
                    positionWithSeekios.Add(SelectedAnnotation.Coordinate);
                }
                var polygon = MKPolyline.FromCoordinates(positionWithSeekios.ToArray());
                MapViewControl.AddOverlay(polygon);
                MapViewControl.SetVisibleMapRect(polygon.BoundingMapRect, new UIEdgeInsets(50, 40, 100, 40), true);
            }
        }

        #endregion

        public void RegisterMethodes()
        {
            if (_focusOnSeekiosButton != null) _focusOnSeekiosButton.TouchUpInside += FocusOnSeekiosButton_TouchUpInside;
            if (_focusOnZoneButton != null) _focusOnZoneButton.TouchUpInside += FocusOnZoneButton_TouchUpInside;
            if (_changeMapTypeButton != null) _changeMapTypeButton.TouchUpInside += ChangeMapTypeButton_TouchUpInside;
            if (_zoomInButton != null) _zoomInButton.TouchUpInside += ZoomInButton_TouchUpInside;
            if (_zoomOutButton != null) _zoomOutButton.TouchUpInside += ZoomOutButton_TouchUpInside;
            if (_editZoneButton != null) _editZoneButton.TouchUpInside += EditZoneButton_TouchUpInside;
            if (_undoButton != null) _undoButton.TouchUpInside += UndoButton_TouchUpInside;
        }

        public void UnregisterMethodes()
        {
            if (_focusOnSeekiosButton != null) _focusOnSeekiosButton.TouchUpInside -= FocusOnSeekiosButton_TouchUpInside;
            if (_focusOnZoneButton != null) _focusOnZoneButton.TouchUpInside -= FocusOnZoneButton_TouchUpInside;
            if (_changeMapTypeButton != null) _changeMapTypeButton.TouchUpInside -= ChangeMapTypeButton_TouchUpInside;
            if (_zoomInButton != null) _zoomInButton.TouchUpInside -= ZoomInButton_TouchUpInside;
            if (_zoomOutButton != null) _zoomOutButton.TouchUpInside -= ZoomOutButton_TouchUpInside;
            if (_editZoneButton != null) _editZoneButton.TouchUpInside -= EditZoneButton_TouchUpInside;
            if (_undoButton != null) _undoButton.TouchUpInside -= UndoButton_TouchUpInside;
        }

        public void Dispose()
        {
            RemoveAllMarkers();

            if (_focusOnSeekiosButton != null) _focusOnSeekiosButton.Dispose();
            if (_focusOnZoneButton != null) _focusOnZoneButton.Dispose();
            if (_zoomInButton != null) _zoomInButton.Dispose();
            if (_zoomOutButton != null) _zoomOutButton.Dispose();
            if (_editZoneButton != null) _editZoneButton.Dispose();
            if (_undoButton != null) _undoButton.Dispose();
            if (_accuracyArea != null) _accuracyArea.Dispose();
            if (_mapDelegate != null) _mapDelegate.Dispose();
            if (_controller != null) _controller.Dispose();
            if (MapViewControl != null)
            {
                MapViewControl.MapType = MKMapType.Hybrid;
                MapViewControl.Delegate = null;
                MapViewControl.RemoveFromSuperview();
                MapViewControl = null;
                MapViewControl.Dispose();
            }

            if (SelectedAnnotation != null) SelectedAnnotation.Dispose();
            if (SelectedLocationHistory != null) SelectedLocationHistory.Dispose();
            if (SelectedPointsOfRoute != null) SelectedPointsOfRoute.Dispose();
            if (RoutePolyline != null) RoutePolyline.Dispose();
            PointsOfRoute.Clear();
            PointsOfRoute = null;
        }

        #region Useless Code

        public bool CenterOnMyLocation(bool withAnimation = false)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me
            return false;
        }

        public Task<Address> GetAddressFromLatLong(LatitudeLongitude latlong)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
            return null;
        }

        public Task<List<string>> GetAddressList(string address)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
            return null;
        }

        public LatitudeLongitude GetLatLongFromAdress(string address)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
            return null;
        }

        public void GoToLocation(LatitudeLongitude latlong, string title, bool withAnimation = false)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
        }

        #endregion

        #endregion

        #region ===== Private Methodes ============================================================

        private UIImage CreateSeekiosBitmap(string imageBase64
            , double accuracy = 0
            , bool isInAlert = false
            , bool isInDontMove = false)
        {
            UIImage seekiosImage = null;

            if (isInDontMove)
            {

                if (accuracy != 0 && !isInAlert)
                {
                    seekiosImage = UIImage.FromBundle("PinModeDontMoveOrange");
                }
                else if (isInAlert)
                {
                    seekiosImage = UIImage.FromBundle("PinModeDontMoveRed");

                }
                else
                {
                    seekiosImage = UIImage.FromBundle("PinModeDontMove");
                }
                return seekiosImage;
            }
            else
            {
                if (accuracy != 0 && !isInAlert)
                {
                    seekiosImage = UIImage.FromBundle("PinSeekiosOrange");
                }
                else if (isInAlert)
                {
                    seekiosImage = UIImage.FromBundle("PinSeekiosRed");
                }
                else
                {
                    seekiosImage = UIImage.FromBundle("PinSeekiosGreen");
                }
                return seekiosImage;
            }
        }


        private void AddPointToZone()
        {
            //if (_recognizer.State == UIGestureRecognizerState.Began)
            //{
            //	_dragStarted = false;
            //             Console.WriteLine("Began !");
            //             return;
            //}
            //else if (_recognizer.State == UIGestureRecognizerState.Changed)
            //{
            //	_dragStarted = true;
            //             Console.WriteLine("Drag !");
            //	// Do dragging stuff here
            //}
            //else if (_recognizer.State == UIGestureRecognizerState.Ended){
            //	if (_dragStarted)
            //	{
            //                 Console.WriteLine("Ended 1!");
            //                 _dragStarted = false;
            //		return;
            //	}
            //	else { 
            //             Console.WriteLine("Ended !");
            //             }
            //         }
            if (_recognizer.State == UIGestureRecognizerState.Began)
            {
                //if (_recognizer.State == UIGestureRecognizerState.Began || !IsOnPointAdding) return;
                if (ZonePolygon != null && ZonePolygon.Points.Count() > App.Locator.ModeZone.MAX_NUMBER_OF_POINTS) return;

                // convert touched position to map coordinate
                var userTouch = _recognizer.LocationInView(MapViewControl);
                var mapPoint = MapViewControl.ConvertPoint(userTouch, MapViewControl);
                var newAnnotation = new ModeZoneAnnotation(mapPoint, true);

                // change the previous annotation to green
                var lastAnnotation = PointsOfZone.LastOrDefault();
                if (lastAnnotation != null)
                {
                    MapViewControl.RemoveAnnotation(lastAnnotation);
                    ((ModeZoneAnnotation)lastAnnotation).IsLastAnnotation = false;
                    MapViewControl.AddAnnotation(lastAnnotation);
                }

                // refresh the polygone
                List<LatitudeLongitude> zone = PointsOfZone.Select(el => new LatitudeLongitude(el.Coordinate.Latitude, el.Coordinate.Longitude)).ToList();
                zone.Add(new LatitudeLongitude(mapPoint.Latitude, mapPoint.Longitude));
                if (!App.Locator.ModeZone.CheckCorrectAreaFormat(zone)) return;

                var pointsToRestore = PointsOfZone.Select(el => el.Coordinate).ToList();
                PointsOfZone.Add(newAnnotation);

                MapViewControl.AddAnnotation(newAnnotation);

                var isInAlert = MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1;

                CreateZone(PointsOfZone.Select(s => new LatitudeLongitude(s.Coordinate.Latitude, s.Coordinate.Longitude)).ToList(), isInAlert, true, 0);
                RefreshZone();

                // reverse the action
                UndoActions.Add(new Action(() =>
                {
                    MapViewControl.RemoveAnnotation(newAnnotation);
                    PointsOfZone.Remove(PointsOfZone.Last());
                    var lastAnnotationUndo = PointsOfZone.LastOrDefault();
                    if (lastAnnotationUndo != null)
                    {
                        MapViewControl.RemoveAnnotation(lastAnnotationUndo);
                        ((ModeZoneAnnotation)lastAnnotationUndo).IsLastAnnotation = true;
                        MapViewControl.AddAnnotation(lastAnnotationUndo);
                    }
                    CreateZone(pointsToRestore.Select(s => new LatitudeLongitude(s.Latitude, s.Longitude)).ToList(), isInAlert, true, 0);
                    RefreshZone();
                    if (ZonePolygon == null || ZonePolygon.Points == null || PointsOfZone.Count() < 3) _nextButton.Enabled = false;
                    else _nextButton.Enabled = true;
                }));

                // if the zone contain 3 points, enable the next button
                if (PointsOfZone.Count < 3) _nextButton.Enabled = false;
                else _nextButton.Enabled = true;
            }
        }

        private void RefreshZone()
        {
            ZoneInformationUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void SetShadows(UIButton button)
        {
            if (button == null) return;
            button.Layer.MasksToBounds = false;
            button.Layer.ShadowColor = UIColor.Black.CGColor;
            button.Layer.ShadowOffset = new CGSize(0.5, 0.5);
            button.Layer.ShadowOpacity = 0.3f;
            button.Layer.ShadowRadius = 5;
            button.Layer.ShadowPath = CGPath.FromRect(button.Bounds);
        }

        #endregion

        #region ===== Event =======================================================================

        private void FocusOnSeekiosButton_TouchUpInside(object sender, EventArgs e)
        {
            CenterInMarker(_idSeekios, false, true);
        }

        private void FocusOnZoneButton_TouchUpInside(object sender, EventArgs e)
        {
            CenterOnZone();
        }

        private void ChangeMapTypeButton_TouchUpInside(object sender, EventArgs e)
        {
            App.Locator.BaseMap.IsInNormalMode = !App.Locator.BaseMap.IsInNormalMode;
            if (App.Locator.BaseMap.IsInNormalMode)
            {
                _changeMapTypeButton.SetImage(UIImage.FromBundle("MapTypeSatellite"), UIControlState.Normal);
            }
            else
            {
                _changeMapTypeButton.SetImage(UIImage.FromBundle("MapTypePlan"), UIControlState.Normal);
            }
            ChangeMapType(App.Locator.BaseMap.IsInNormalMode);
        }

        private void ZoomInButton_TouchUpInside(object sender, EventArgs e)
        {
            _zoomInButton.UserInteractionEnabled = false;
            var region = MapViewControl.Region;
            var span = MapViewControl.Region.Span;

            region.Center = MapViewControl.Region.Center;
            span.LatitudeDelta = MapViewControl.Region.Span.LatitudeDelta / 2.0002;
            span.LongitudeDelta = MapViewControl.Region.Span.LongitudeDelta / 2.0002;
            region.Span = span;

            MapViewControl.SetRegion(region, true);
            _zoomInButton.UserInteractionEnabled = true;
        }

        private void ZoomOutButton_TouchUpInside(object sender, EventArgs e)
        {
            _zoomOutButton.UserInteractionEnabled = false;
            var region = MapViewControl.Region;
            var span = MapViewControl.Region.Span;

            region.Center = MapViewControl.Region.Center;
            span.LatitudeDelta = MapViewControl.Region.Span.LatitudeDelta * 2;
            span.LongitudeDelta = MapViewControl.Region.Span.LongitudeDelta * 2;
            region.Span = span;

            if (span.LatitudeDelta > 200 || span.LongitudeDelta > 200) return;
            MapViewControl.SetRegion(region, true);

            _zoomOutButton.UserInteractionEnabled = true;
        }

        private void EditZoneButton_TouchUpInside(object sender, EventArgs e)
        {
            IsOnPointAdding = !IsOnPointAdding;
        }

        private void UndoButton_TouchUpInside(object sender, EventArgs e)
        {
            if (UndoActions.Count <= 0) return;
            UndoActions.Last().Invoke();
            UndoActions.Remove(UndoActions.Last());
        }

        #endregion
    }
}
