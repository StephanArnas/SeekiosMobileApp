using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;
using Android.Gms.Maps;
using SeekiosApp.Interfaces;
using Android.Gms.Maps.Model;
using XamSvg;
using Android.Graphics;
using SeekiosApp.Droid.Helper;
using SeekiosApp.Model;
using SeekiosApp.Model.APP;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Ioc;
using SeekiosApp.Extension;
using SeekiosApp.Model.DTO;
using System.Globalization;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using Android.Util;
using System.IO;
using SeekiosApp.Enum;

namespace SeekiosApp.Droid.ControlManager
{
    public class MapControlManager : IMapControlManager, IDisposable
    {
        #region ===== Constants ===================================================================

        private const int IN_TIME_ACCURACY = 20;
        private const int NUMBER_OF_ADDRESSES = 10;
        private const int INTIME_ZOOM_LEVEL = 18;

        private const string HEX_GREEN = "62DA73";
        private const string HEX_RED = "DA2E2E";
        private const string HEX_ORANGE = "DF912F";
        private const string COLOR_GREEN = "#" + HEX_GREEN;
        private const string COLOR_RED = "#" + HEX_RED;
        private const string COLOR_ORANGE = "#" + HEX_ORANGE;

        #endregion

        #region ===== Attributs ===================================================================

        private GoogleMap _map;
        private Context _activityContext;
        private List<Marker> _markers;
        private Circle _accuracyArea;
        private List<SeekiosMarkerIdAssociation> _markerIdAssociation;
        private Dictionary<string, Circle> _circleIdAssociation;//todo: unify _accuracyArea and this, no need to keep two references to same objects
        private Polygon _updatingAreaPolygon = null;
        private bool _isOnPointAdding = false;
        private string _idSeekios = string.Empty;

        private SvgImageView _addPointSvgImageView = null;
        private SvgImageView _undoSvgImageView = null;
        private SvgImageView _centerSeekios = null;

        // Color of elements in alert
        private int _alertColor = int.Parse("44" + HEX_RED, NumberStyles.HexNumber);
        // Color of elements when ther is no problems
        private int _normalColor = int.Parse("44" + HEX_GREEN, NumberStyles.HexNumber);//FF21b928

        #endregion

        #region ===== Properties ==================================================================

        /// <summary>Représente la zone sur la map</summary>
        public Polygon ZonePolygon { get; set; }

        /// <summary>Représente le trajet sur la map</summary>
        public Polyline RoutePolyline { get; set; }

        public PolylineOptions RouteOptions { get; set; }

        public List<MarkerOptions> MarkerOptions { get; set; }

        /// <summary>Marqueur sélectionné par l'utilisateur</summary>
        public Marker SelectedMarker { get; set; }

        /// <summary>Points de la zone</summary>
        public List<Marker> PointsOfZone { get; set; }

        /// <summary>Points du trajet</summary>
        public List<Marker> PointsOfRoute { get; set; }

        /// <summary>Marqueur du trajet sélectionné par l'utilisateur</summary>
        public Marker SelectedPointsOfRoute { get; set; }

        /// <summary>Liste d'action à annuler pour revenir en arrière</summary>
        public ObservableCollection<Action> UndoActions { get; set; }

        /// <summary>Vrai si la vue est en mode d'ajout de points</summary>
        public bool IsOnPointAdding
        {
            get { return _isOnPointAdding; }
            set
            {
                _isOnPointAdding = value;
                _addPointSvgImageView.SetSvg(_activityContext, Resource.Drawable.ModeZoneEdit,
                    _isOnPointAdding ?
                    "#fafafa=" + COLOR_GREEN : COLOR_GREEN + "=#fafafa", "88B3B3B3=" + HEX_GREEN);
            }
        }

        public static bool IsInAlert = false;

        /// <summary>Marqueur de l'historique de localisation</summary>
        public Marker SelectedLocationHistory { get; set; }

        public event EventHandler ZoneInformationUpdated;
        public event EventHandler<string> SeekiosMarkerClicked;
        public event EventHandler UserLocationChanged;
        public event Func<object, EventArgs, Task> OnInitTrackingRouteComplete;

        #endregion

        #region ===== Constructeurs ===============================================================

        /// <summary>
        /// Initialise la nouvelle instance de MapControlManager.
        /// Il est nécessaire de passer le contrôle GoogleMap et le context pour fonctionner.
        /// GoogleMap : (MapFragment)FragmentManager.FindFragmentById(...) | mapGragment.Map;
        /// ActivityContext : BaseContext
        /// </summary>
        [PreferredConstructorAttribute]
        public MapControlManager(GoogleMap mapControl, Context activityContext)
        {
            // Initialisation
            _markers = new List<Marker>();
            _markerIdAssociation = new List<SeekiosMarkerIdAssociation>();
            _circleIdAssociation = new Dictionary<string, Circle>();
            UndoActions = new ObservableCollection<Action>();
            PointsOfZone = new List<Marker>();
            PointsOfRoute = new List<Marker>();

            _map = mapControl ?? throw new Exception("Le contrôle GoogleMap ne peut être null");
            _activityContext = activityContext ?? throw new Exception("Le context de l'activité ne peut être null");

            // Set the mapType according to what the user chose
            _map.MapType = App.Locator.BaseMap.IsInNormalMode ? GoogleMap.MapTypeNormal : GoogleMap.MapTypeSatellite;

            // Customize markers
            LayoutInflater inflater = (LayoutInflater)_activityContext.GetSystemService(Context.LayoutInflaterService);
            _map.SetInfoWindowAdapter(new CustomComponents.MarkerPopupAdapter(_activityContext, (LayoutInflater)_activityContext.GetSystemService(Context.LayoutInflaterService)));

            // Events subscription
            _map.MarkerDragStart += OnMarkerDragStart;
            _map.MarkerDrag += OnMarkerDrag;
            _map.MarkerDragEnd += OnMarkerDragEnd;
            _map.MapClick += OnMapClick;
            _map.MarkerClick += OnMarkerClick;
        }

        /// <summary>
        /// Initialise la nouvelle instance de MapControlManager.
        /// Il est nécessaire de passer le contrôle GoogleMap et le context pour fonctionner.
        /// GoogleMap : (MapFragment)FragmentManager.FindFragmentById(...) | mapGragment.Map;
        /// ActivityContext : BaseContext
        /// CenterSeekios : centrer la map sur le seekios
        /// IdSeekios : centrer la map sur le seekios
        /// </summary>
        public MapControlManager(GoogleMap mapControl
            , Context activityContext
            , SvgImageView centerSeekios
            , string idSeekios)
            : this(mapControl, activityContext)
        {
            _centerSeekios = centerSeekios;
            _idSeekios = idSeekios;
        }

        /// <summary>
        /// Initialise la nouvelle instance de MapControlManager.
        /// Il est nécessaire de passer le contrôle GoogleMap et le context pour fonctionner.
        /// GoogleMap : (MapFragment)FragmentManager.FindFragmentById(...) | mapGragment.Map;
        /// ActivityContext : BaseContext
        /// Add
        /// 
        /// : AddPointSvgImageView
        /// UndoButton : UndoSvgImageView
        /// CenterSeekios : centrer la map sur le seekios
        /// IdSeekios : centrer la map sur le seekios
        /// </summary>
        public MapControlManager(GoogleMap mapControl
            , Context activityContext
            , SvgImageView addButton
            , SvgImageView undoButton
            , SvgImageView centerSeekios
            , string idSeekios)
            : this(mapControl, activityContext)
        {
            _addPointSvgImageView = addButton;
            _undoSvgImageView = undoButton;
            _centerSeekios = centerSeekios;
            _idSeekios = idSeekios;
        }

        #endregion

        #region ===== Méthodes Publiques ==========================================================

        /// <summary>
        /// Permet d'initialiser les options de la map
        /// </summary>
        /// <param name="zoomLevel">Zoom de démarrage de la map</param>
        public void InitMap(float zoomLevel, bool zoom = false, bool compass = true, bool mapToolbar = true)
        {
            _map.UiSettings.ZoomControlsEnabled = zoom;
            _map.UiSettings.CompassEnabled = compass;
            _map.UiSettings.MapToolbarEnabled = true;
            _map.SetPadding(0, 0, 0, 0);
            _map.MyLocationChange += OnMyLocationChange;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<Messages.MarkerZoneDeleteMessage>(this, message =>
            {
                RemovePointOfZone(message.MarkerToDelete, false, false);
            });

            if (_addPointSvgImageView != null) IsOnPointAdding = true;//active le mode edition des le debut

            _map.MoveCamera(CameraUpdateFactory.ZoomTo(zoomLevel));
        }

        /// <summary>
        /// Supprime le MapControl ainsi que les markers
        /// </summary>
        public void Dispose()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Unregister<Messages.MarkerZoneDeleteMessage>(this);
            _map.MyLocationChange -= OnMyLocationChange;

            RemoveAllMarkers();

            _activityContext = null;
            _map = null;
        }

        /// <summary>
        /// Supprime tous les markers
        /// </summary>
        public void RemoveAllMarkers()
        {
            _map.Clear();
            foreach (Marker marker in _markers)
            {
                marker.Dispose();
            }
            _markers.Clear();
            _markerIdAssociation.Clear();
            _circleIdAssociation.Clear();
        }

        /// <summary>
        /// Permet de centrer la map sur l'id d'un marker. Si l'id n'existe pas, la méthode retourne False
        /// </summary>
        public bool CenterInMarker(string seekiosId, bool showInfoWindow = false, bool withAnimation = true)
        {
            SeekiosMarkerIdAssociation idMarkerAsso = _markerIdAssociation.FirstOrDefault(id => id.SeekiosId == seekiosId);
            if (idMarkerAsso == null) return false;

            Marker marker = _markers.FirstOrDefault(m => m.Id.Equals(idMarkerAsso.MarkerId));
            if (marker == null) return false;

            Circle c = null;
            if (_circleIdAssociation.TryGetValue(idMarkerAsso.MarkerId, out c))
            {
                CenterOnUnaccurateLocation(c, marker.Position.Latitude, marker.Position.Longitude, (float)ZoomLevelEnum.BigZoom, withAnimation);
            }
            else CenterInLocalisation(marker.Position.Latitude, marker.Position.Longitude, (float)ZoomLevelEnum.BigZoom, withAnimation);
            if (showInfoWindow)
            {
                SelectedMarker = marker;
                marker.ShowInfoWindow();
            }

            return true;
        }

        /// <summary>
        /// Centre la carte sur une position. Il est possible de le faire de façon animé ou non (par défaut pas d'animation)
        /// </summary>
        public void CenterInLocalisation(double latitude, double longitude, float zoomLevel, bool withAnimation = false)
        {
            var location = new LatLng(latitude, longitude);
            if (withAnimation)
            {
                _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(location, zoomLevel));
            }
            else _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(location, zoomLevel));
        }

        /// <summary>
        /// 
        /// </summary>
        public void CenterOnLocations(List<LatitudeLongitude> locations, bool with_animation = false)
        {
            if (locations == null || locations.Count == 0) return;

            var minLat = locations.Select(el => el.Latitude).Min();
            var minLong = locations.Select(el => el.Longitude).Min();

            var maxLat = locations.Select(el => el.Latitude).Max();
            var maxLong = locations.Select(el => el.Longitude).Max();
            CenterCamera(minLat, minLong, maxLat, maxLong, with_animation);
        }

        public void CenterCamera(double minLat, double minLong, double maxLat, double maxLong, bool with_animation)
        {
            //handle crash : Android google maps,google places, northern latitude exceeds southern latitiude
            //source: http://stackoverflow.com/questions/24610957/android-google-maps-google-places-northern-latitude-exceeds-southern-latitiude
            var builder = new LatLngBounds.Builder();
            builder.Include(new LatLng(minLat, minLong)).Include(new LatLng(maxLat, maxLong));
            if (with_animation)
            {
                _map.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 150));
            }
            else
            {
                _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(builder.Build(), 150));
            }
        }

        public void CenterOnLocations(List<Marker> locations, bool with_animation = false)
        {
            if (locations == null || locations.Count == 0) return;
            var minLat = locations.Select(el => el.Position.Latitude).Min();
            var minLong = locations.Select(el => el.Position.Longitude).Min();
            var maxLat = locations.Select(el => el.Position.Latitude).Max();
            var maxLong = locations.Select(el => el.Position.Longitude).Max();
            CenterCamera(minLat, minLong, maxLat, maxLong, with_animation);
        }

        /// <summary>
        /// Center the map on the area of the zone
        /// </summary>
        public void CenterOnZone()
        {
            if (ZonePolygon != null && ZonePolygon.Points != null)
                CenterOnLocations(ZonePolygon.Points.Select(el => new LatitudeLongitude(el.Latitude, el.Longitude)).ToList(), true);
        }

        /// <summary>
        /// Remove a marker
        /// </summary>
        public void RemoveMarker(Marker marker)
        {
            if (marker == null) return;

            var markerAsso = _markerIdAssociation.FirstOrDefault(id => id.MarkerId == marker.Id);
            if (markerAsso != null) _markerIdAssociation.Remove(markerAsso);
            {
                Circle circle = null;
                if (_circleIdAssociation.TryGetValue(marker.Id, out circle))
                {
                    circle.Remove();//deletes the circle if it exists
                    _circleIdAssociation.Remove(marker.Id);
                }
            }

            var markerToRemove = _markers.FirstOrDefault(el => el == marker);
            if (markerToRemove == null)
            {
                markerToRemove = _markers.FirstOrDefault();
            }
            markerToRemove.Remove();
            _markers.Remove(markerToRemove);

            return;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveMarker(string seekiosId)
        {
            SeekiosMarkerIdAssociation idMarkerAsso = _markerIdAssociation.FirstOrDefault(id => id.SeekiosId == seekiosId);
            if (idMarkerAsso == null) return;

            Marker marker = _markers.FirstOrDefault(m => m.Id.Equals(idMarkerAsso.MarkerId));
            if (marker == null) return;

            RemoveMarker(marker);
        }

        /// <summary>
        /// Create a seekios marker 
        /// </summary>
        public void CreateSeekiosMarkerAsync(string seekiosId
            , string title
            , string imageBase64
            , DateTime? lastLocationDate
            , double latitude
            , double longitude
            , double accuracy
            , bool isDontMove
            , bool isInAlert = false)
        {
            Bitmap seekiosMarkerBitmap = null;

            // If the seekios does not have a picture, loading of the default seekios picture
            if (string.IsNullOrEmpty(imageBase64))
            {
                if (_activityContext == null) throw new Exception("CreateSeekiosMarkerAsync: _activityContext can not be null");
                var defaultSeekios = _activityContext.Resources.GetDrawable(Resource.Drawable.DefaultSeekios);
                using (var bitmap = (defaultSeekios as BitmapDrawable).Bitmap)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bitmap.Compress(Bitmap.CompressFormat.Png, 100, stream);
                        imageBase64 = Base64.EncodeToString(stream.ToArray(), Base64Flags.Default);
                    }
                }
            }

            // Create bitmap marker
            if (string.IsNullOrEmpty(imageBase64)) throw new Exception("CreateSeekiosMarkerAsync: imageBase64 remains null");
            seekiosMarkerBitmap = CreateSeekiosBitmap(imageBase64, isDontMove, accuracy, isInAlert);
            // Place the marker
            Marker seekiosMarker = CreateMarker(title, seekiosMarkerBitmap, latitude, longitude);
            seekiosMarkerBitmap.Dispose();
            if (lastLocationDate.HasValue)
            {
                seekiosMarker.Snippet = lastLocationDate.Value.FormatDateFromNow();
            }
            _markerIdAssociation.Add(new SeekiosMarkerIdAssociation(seekiosId, seekiosMarker.Id));

            if (accuracy != 0)
            {
                Circle c = CreateAccuracyArea(latitude, longitude, accuracy);
                _circleIdAssociation.Add(seekiosMarker.Id, c);
                CenterOnUnaccurateLocation(c, latitude, longitude, accuracy, true);
            }
            else CenterInMarker(seekiosId);

            //TODO: on map all seekios, do not show info for each marker
            SelectedMarker = seekiosMarker;
            if (App.Locator.ModeZone.IsOnEditMode && App.Locator.ModeZone.IsActivityFocused)
            {
                seekiosMarker.HideInfoWindow();
            }
            else seekiosMarker.ShowInfoWindow();
        }

        /// <summary>
        /// Move the marker
        /// </summary>
        public void ChangeMarkerLocation(string seekiosId, double latitude, double longitude, double accuracy)
        {
            SeekiosMarkerIdAssociation idMarkerAsso = _markerIdAssociation.FirstOrDefault(id => id.SeekiosId == seekiosId);

            if (idMarkerAsso != null)
            {
                Marker marker = _markers.FirstOrDefault(m => m.Id.Equals(idMarkerAsso.MarkerId));
                if (marker == null && _markers.Count > 0)
                    marker = _markers.FirstOrDefault();

                marker.HideInfoWindow();
                marker.Position = new LatLng(latitude, longitude);
                marker.ShowInfoWindow();
                CenterOnUnaccurateLocation(CreateAccuracyArea(latitude, longitude, accuracy), latitude, longitude, accuracy, true);
            }
        }

        /// <summary>
        /// Création d'un marker avec la latitude, longitude et l'icon en Bitmap
        /// </summary>
        public Marker CreateMarker(string title, Bitmap markerIcon, double latitude, double longitude)
        {
            var markerSeekios = new MarkerOptions();
            markerSeekios.SetPosition(new LatLng(latitude, longitude));

            if (!string.IsNullOrEmpty(title))
            {
                markerSeekios.SetTitle(title);
            }

            if (markerIcon != null)
            {
                using (var bitmapDescriptor = BitmapDescriptorFactory.FromBitmap(markerIcon))
                {
                    if (bitmapDescriptor != null)
                    {
                        markerSeekios.SetIcon(bitmapDescriptor);
                    }
                    else markerSeekios.SetSnippet("");
                }
            }
            else markerSeekios.SetSnippet("");

            if (_map == null) throw new Exception("CreateMarker: map can not be null");
            var marker = _map.AddMarker(markerSeekios);
            if (_markers == null) _markers = new List<Marker>();
            _markers.Add(marker);
            return marker;
        }

        public Circle CreateAccuracyArea(double latitude, double longitude, double accuracy)
        {
            if (_accuracyArea != null)
            {
                _accuracyArea.Remove();
            }
            if (accuracy <= 0) return null;
            var circleOptions = new CircleOptions();
            circleOptions.InvokeCenter(new LatLng(latitude, longitude));
            circleOptions.InvokeRadius(accuracy);
            circleOptions.InvokeFillColor(0X44159ED3);
            circleOptions.InvokeStrokeColor(0X7F159ED3);
            circleOptions.InvokeStrokeWidth(3);
            _accuracyArea = _map.AddCircle(circleOptions);
            return _accuracyArea;
        }

        /// <summary>
        /// 
        /// </summary>
        public Circle CreateInTimeAccuracyArea(double latitude, double longitude, double accuracy)
        {
            //WARNING : Not use in version 1.0 of the app, It's require for mode InTime

            //if (_inTimeAccuracyArea != null)
            //{
            //    _inTimeAccuracyArea.Remove();
            //}
            //if (accuracy <= 0) return null;
            //var circleOptions = new CircleOptions();
            //circleOptions.InvokeCenter(new LatLng(latitude, longitude));
            //circleOptions.InvokeRadius(accuracy);
            //circleOptions.InvokeFillColor(0X44159ED3);
            //circleOptions.InvokeStrokeColor(0X7F159ED3);
            //circleOptions.InvokeStrokeWidth(3);
            //_inTimeAccuracyArea = _map.AddCircle(circleOptions);
            //return _inTimeAccuracyArea;
            return null;
        }

        #region Zone

        /// <summary>
        /// Create a zone on the map
        /// </summary>
        public void CreateZone(List<LatitudeLongitude> coords, bool isInAlert, bool isLastMarker = false, int index = 0)
        {
            // Delete previous marker
            DeleteZone();

            if (coords == null || coords.Count <= 0) return;
            if (PointsOfZone == null) PointsOfZone = new List<Marker>();
            if (_map == null) throw new Exception("CreateZone: map can not be null");

            var rectOptions = new PolygonOptions();
            var i = 1;
            var count = coords.Count + index;
            var state = false;
            foreach (var coord in coords)
            {
                // If the point already exist, we do not need to draw it again
                if (rectOptions.Points != null
                    && rectOptions.Points.Where(el => el.Latitude == coord.Latitude && el.Longitude == el.Longitude).Count() > 0)
                {
                    continue;
                }

                var latLong = new LatLng(coord.Latitude, coord.Longitude);
                rectOptions.Add(latLong);

                // If we are in edition mode, we add markers for each point of the zone
                if (!App.Locator.ModeZone.IsOnEditMode)
                {
                    continue;
                }

                if (isLastMarker && i == count)
                {
                    state = true;
                }
                else state = false;

                var markerZone = CreateMarkerZoneMap(latLong, state);
                PointsOfZone.Add(_map.AddMarker(markerZone));
                i++;
            }
            rectOptions.InvokeStrokeWidth(2);
            ZonePolygon = _map.AddPolygon(rectOptions);
            SetZoneInAlert(isInAlert);
        }

        /// <summary>
        /// 
        /// </summary>
        public void CreateUndoActionsToRemoveActualZone()
        {
            if (ZonePolygon == null) return;
            var lsPointAlreadyHandled = new List<LatLng>();
            foreach (var point in ZonePolygon.Points)
            {
                if (lsPointAlreadyHandled.Any(el => el.Latitude == point.Latitude
                    && el.Longitude == point.Longitude))
                    continue;
                lsPointAlreadyHandled.Add(point);
                var marker = PointsOfZone.FirstOrDefault(
                    el => el.Position.Latitude == point.Latitude
                        && el.Position.Longitude == point.Longitude);
                if (marker == null) continue;
                UndoActions.Add(() =>
                {
                    RemovePointOfZone(marker, false, true);
                });
            }
            _undoSvgImageView.SetSvg(_activityContext
                , Resource.Drawable.ModeZoneGoBack
                , UndoActions.Count == 0 ? string.Empty : "#666666=#36da3e"
                /*, UndoActions.Count > 0 ? string.Empty : string.Empty*/);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isInAlert"></param>
        public void SetZoneInAlert(bool isInAlert)
        {
            //bool isInAlertScenario = App.Locator.BaseMap.LsSeekiosAlertState.Contains(App.Locator.MySeekiosDetail.SeekiosSelected.Idseekios);
            if (ZonePolygon != null)
            {
                ZonePolygon.FillColor = isInAlert ? _alertColor : _normalColor;
                ZonePolygon.StrokeColor = isInAlert ? _alertColor : _normalColor;
            }
        }

        /// <summary>
        /// Converti le type LatLng en objet LatitudeLongitude manipulable dans le ViewModel
        /// </summary>
        public List<LatitudeLongitude> ConvertLatitudeLongitudeToLatLng(List<LatLng> coords)
        {
            var result = new List<LatitudeLongitude>();
            foreach (var coord in coords)
            {
                result.Add(new LatitudeLongitude(coord.Latitude, coord.Longitude));
            }
            return result;
        }

        /// <summary>
        /// Supprime la zone de la map 
        /// </summary>
        public void DeleteZone()
        {
            if (PointsOfZone != null)
            {
                foreach (var marker in PointsOfZone)
                {
                    marker.Remove();
                }
                PointsOfZone.Clear();
            }

            if (ZonePolygon != null) ZonePolygon.Remove();
            if (SelectedMarker != null && !App.Locator.ModeZone.IsOnEditMode)
            {
                SelectedMarker.HideInfoWindow();
                SelectedMarker.ShowInfoWindow();
            }
            ZonePolygon = null;
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
            LatLng p1 = new LatLng(0, 0);
            LatLng p2 = new LatLng(0, 0);
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

        #region Register

        /// <summary>
        /// Enregistre les actions
        /// </summary>
        public void RegisterMethodes()
        {
            if (_addPointSvgImageView != null) _addPointSvgImageView.Click += OnAddPoint;
            if (_undoSvgImageView != null) _undoSvgImageView.Click += OnUndoChanges;
            if (_centerSeekios != null) _centerSeekios.Click += OnCenterSeekios;
            if (UndoActions != null) UndoActions.CollectionChanged += OnUndoActionsCollectionChanged;    // grise le bouton undo si la collection de modification est vide
        }

        /// <summary>
        /// Désenregistre les actions
        /// </summary>
        public void UnregisterMethodes()
        {
            if (_addPointSvgImageView != null) _addPointSvgImageView.Click -= OnAddPoint;
            if (_undoSvgImageView != null) _undoSvgImageView.Click -= OnUndoChanges;
            if (_centerSeekios != null) _centerSeekios.Click -= OnCenterSeekios;
            if (UndoActions != null) UndoActions.CollectionChanged -= OnUndoActionsCollectionChanged;    // grise le bouton undo si la collection de modification est vide
        }

        #endregion

        #region Tracking

        public void CreateRoute(List<LocationDTO> coords)
        {
            CreateRouteBackground(coords);
            CreateRouteForeground(coords);
        }

        public void CreateRouteBackground(List<LocationDTO> coords)
        {
            if (RoutePolyline != null) RoutePolyline.Remove();
            if (SelectedPointsOfRoute != null) SelectedPointsOfRoute.Remove();
            PointsOfRoute.ForEach(el => el.Remove());
            PointsOfRoute.Clear();

            if (coords.Count != 0)
            {
                RouteOptions = new PolylineOptions();
                MarkerOptions = new List<MarkerOptions>();
                MarkerOptions markerRoute = null;
                LatLng latLong = null;
                foreach (var coord in coords)
                {
                    if (coord.Accuracy == 0)
                    {
                        latLong = new LatLng(coord.Latitude, coord.Longitude);
                        RouteOptions.Add(latLong);
                        markerRoute = CreateMarkerRouteMap(latLong, coord.DateLocationCreation);
                        if (markerRoute != null)
                        {
                            MarkerOptions.Add(markerRoute);
                        }
                    }
                }
                // il faut que l'interaction avec google maps soit dans UI Thread. si on fait du asynchrone, ce n'est plus le cas...
            }
            OnInitTrackingRouteComplete?.Invoke(null, null);
        }

        public void CreateRouteForeground(List<LocationDTO> coords)
        {
            if (MarkerOptions != null && RouteOptions != null && _map != null && PointsOfRoute != null && RouteOptions != null)
            {
                foreach (MarkerOptions mark in MarkerOptions)
                {
                    Marker marqueur = _map.AddMarker(mark);
                    PointsOfRoute.Add(marqueur);
                }
                RouteOptions.InvokeColor(int.Parse("50" + (IsInAlert ? HEX_RED : HEX_GREEN), NumberStyles.HexNumber));
                RouteOptions.InvokeWidth(10);
                RoutePolyline = _map.AddPolyline(RouteOptions);

                // this new way is much better, no filters and no memory allocation and no object creation
                int maxNumberOfPositions = 20;
                double minLat = double.MaxValue;
                double minLong = double.MaxValue;
                double maxLat = double.MinValue;
                double maxLong = double.MinValue;
                int last = PointsOfRoute.Count != 0 ? PointsOfRoute.Count - 1 : 0;
                int limit = PointsOfRoute.Count > maxNumberOfPositions ? PointsOfRoute.Count - maxNumberOfPositions : 0;
                LatLng tmp = null;
                for (int i = last; i != limit; --i)
                {
                    tmp = PointsOfRoute[i].Position;
                    if (tmp.Latitude < minLat) minLat = tmp.Latitude;
                    if (tmp.Longitude < minLong) minLong = tmp.Longitude;
                    if (tmp.Latitude > maxLat) maxLat = tmp.Latitude;
                    if (tmp.Longitude > maxLong) maxLong = tmp.Longitude;
                }
                if (coords.Count > 1) CenterCamera(minLat, minLong, maxLat, maxLong, true);
            }
        }

        #endregion

        #region Location history

        /// <summary>
        /// Sélectionne un historique de localisations sur la map
        /// </summary>
        public void ChangeSelectedLocationHistory(LocationDTO coordonate)
        {
            // On commence par supprimer l'ancien historique sélectionné s'il existe
            if (SelectedLocationHistory != null) SelectedLocationHistory.Remove();

            // Si on ne demande rien à sélectionner on quite la fonction
            if (coordonate == null) return;

            // On le remplace par un nouveau markeur mais sélectionné cette fois ci
            SelectedLocationHistory = _map.AddMarker(CreateSelectedMarkerHistory(new LatLng(coordonate.Latitude, coordonate.Longitude)
                , coordonate.DateLocationCreation
                , coordonate.Accuracy));

            // On centre la vue dessus le nouveau markeur sélectionné
            if (!App.Locator.ModeZone.IsOnEditMode)
            {
                if (coordonate.Accuracy == 0)
                {
                    _map.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(coordonate.Latitude, coordonate.Longitude)));
                }
                else _map.MoveCamera(CameraUpdateFactory.NewLatLng(CreateAccuracyArea(coordonate.Latitude, coordonate.Longitude, coordonate.Accuracy).Center));
            }
            // On affiche l'infobulle
            SelectedLocationHistory.ShowInfoWindow();
        }

        /// <summary>
        /// Supprime l'historique de localisations sélectionné affiché sur la map
        /// </summary>
        public void RemoveSelectedHistoryMarker()
        {
            if (SelectedLocationHistory == null) return;
            SelectedLocationHistory.HideInfoWindow();
            SelectedLocationHistory.Remove();
        }

        #endregion

        #region Useless Code

        //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
        public Marker CreateInTimeMarker(string title, double latitude, double longitude)
        {
            //MarkerOptions markerSeekios = new MarkerOptions();
            //markerSeekios.SetPosition(new LatLng(latitude, longitude));
            //if (!string.IsNullOrEmpty(title))
            //{
            //    markerSeekios.SetTitle(title);
            //}
            //markerSeekios.SetSnippet("InTimeMarker");
            //Marker marker = _map.AddMarker(markerSeekios);
            //SelectedMarker = marker;
            //marker.ShowInfoWindow();
            //_markers.Add(marker);
            //_lastInTimeMarker = marker;
            //return marker;
            return null;
        }

        //WARNING : Not use in version 1.0 of the app, It's has been required in MapBaseActivity
        public LatLng GetLastMarkerPosition()
        {
            //return _markers.LastOrDefault().Position;
            return null;
        }

        //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
        public async Task<Model.APP.Address> GetAddressFromLatLong(LatitudeLongitude latlong)
        {
            //try
            //{
            //    if (_geocoder == null) _geocoder = new Geocoder(Application.Context, Locale.ForLanguageTag(CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            //    var result = await _geocoder.GetFromLocationAsync(latlong.Latitude, latlong.Longitude, 1);
            //    var address = result.FirstOrDefault();
            //    return new Model.APP.Address(address.Latitude, address.Longitude, string.Format("{0}, {1}", address.GetAddressLine(0), address.GetAddressLine(1)));
            //}
            //catch (Exception)
            //{
            //    //App.Locator.ModeInTime.ShowErrorMessage(Enum.ErrorCode.GeocoderError, null, null);
            //    return null;
            //}
            return null;
        }

        //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
        public LatitudeLongitude GetLatLongFromAdress(string address)
        {
            //try
            //{
            //    Android.Locations.Address addressFound = null;

            //    if (_geocoder == null) _geocoder = new Geocoder(Application.Context, Locale.ForLanguageTag(CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            //    var result = _geocoder.GetFromLocationName(address, 1).ToList();
            //    if (result.Count > 0)
            //    {
            //        addressFound = result.FirstOrDefault();
            //        return new LatitudeLongitude(addressFound.Latitude, addressFound.Longitude);
            //    }
            //    else return null;
            //}
            //catch (Exception)
            //{
            //    //App.Locator.ModeInTime.ShowErrorMessage(Enum.ErrorCode.GeocoderError, null, null);
            //    return null;
            //}
            return null;
        }

        //WARNING : Not use in version 1.0 of the app, It's require for mode InTime
        public async Task<List<string>> GetAddressList(string address)
        {
            //try
            //{
            //    List<string> addressStrList = null;
            //    if (_geocoder == null) _geocoder = new Geocoder(Application.Context, Locale.ForLanguageTag(CultureInfo.CurrentCulture.TwoLetterISOLanguageName));
            //    var locList = await _geocoder.GetFromLocationNameAsync(address, NUMBER_OF_ADDRESSES);
            //    if (locList != null) addressStrList = new List<string>();
            //    var addressList = locList.ToList();
            //    foreach (var addressItem in addressList)
            //    {
            //        addressStrList.Add(string.Format("{0}, {1}", addressItem.GetAddressLine(0), addressItem.GetAddressLine(1)));
            //    }
            //    return addressStrList;
            //}
            //catch (Exception)
            //{
            //    //App.Locator.ModeInTime.ShowErrorMessage(Enum.ErrorCode.GeocoderError, null, null);
            //    return null;
            //}
            return null;
        }

        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me
        public bool CenterOnMyLocation(bool withAnimation = false)
        {
            //if (_map.MyLocation == null) return false;

            //var location = new LatLng(_map.MyLocation.Latitude, _map.MyLocation.Longitude);
            //if (withAnimation)
            //    _map.MoveCamera(CameraUpdateFactory.NewLatLng(location));
            //else
            //    _map.AnimateCamera(CameraUpdateFactory.NewLatLng(location));
            //return true;
            return false;
        }

        #endregion

        #endregion

        #region ===== Private Methods =============================================================

        /// <summary>
        /// Création du Bitmap de l'icon pin Seekios avec une image base 64
        /// </summary>
        private Bitmap CreateSeekiosBitmap(string imageBase64, bool isDontMove, double accuracy = 0, bool isInAlert = false)
        {
            Bitmap bmp = null;

            int pinWidth = 105;
            int pinHeight = 130;
            int circleWidth = 80;
            int offset_left = (pinWidth - circleWidth);

            if (isDontMove)
            {
                bmp = Bitmap.CreateBitmap(pinWidth + offset_left, pinHeight, Bitmap.Config.Argb8888);
            }
            else
            {
                pinWidth = 80;
                pinHeight = 130;
                bmp = Bitmap.CreateBitmap(pinWidth, pinHeight, Bitmap.Config.Argb8888);
            }

            using (Canvas markerCanvas = new Canvas(bmp))
            {
                using (var seekiosImage = ImageHelper.Base64ToBitmap(imageBase64))
                {
                    // Add the seekios image in the canvas
                    if (seekiosImage == null) return bmp;

                    using (var seekiosImageCircle = ImageHelper.GetCroppedBitmap(seekiosImage))
                    {
                        if (isDontMove)
                        {
                            markerCanvas.DrawBitmap(Bitmap.CreateScaledBitmap(seekiosImageCircle
                                , circleWidth - 2
                                , circleWidth - 2
                                , false), offset_left + 1, 1, null);
                        }
                        else
                        {
                            markerCanvas.DrawBitmap(Bitmap.CreateScaledBitmap(seekiosImageCircle
                                , pinWidth - 2
                                , pinWidth - 2
                                , false), 1, 1, null);
                        }
                    }
                }

                // Image of seekios marker
                using (SvgBitmapDrawable seekiosMarkerSvgImage = isDontMove
                    ? SvgFactory.GetDrawable(_activityContext.Resources, Resource.Drawable.pinmodedontmove)
                    : SvgFactory.GetDrawable(_activityContext.Resources, Resource.Drawable.PinSeekios, pinWidth, pinHeight))
                {
                    // If the color of the marker must change
                    if (isInAlert || accuracy != 0)
                    {
                        var color = Color.ParseColor(isInAlert
                            ? COLOR_RED
                            : (accuracy != 0 ? COLOR_ORANGE : COLOR_GREEN));
                        DrawPictureOnCanvasWithAnotherColor(markerCanvas
                            , seekiosMarkerSvgImage.Picture
                            , color
                            , isDontMove ? new Rect(offset_left, 0, (offset_left << 1) + circleWidth, pinHeight) : null);
                    }
                    else
                    {
                        var rect = isDontMove
                            ? new Rect(offset_left, 0, (offset_left << 1) + circleWidth, pinHeight)
                            : new Rect(0, 0, markerCanvas.Width, markerCanvas.Height);
                        markerCanvas.DrawPicture(seekiosMarkerSvgImage.Picture, rect);
                    }
                }
            }
            return bmp;
        }

        private void DrawPictureOnCanvasWithAnotherColor(Canvas canvas, Picture picture, Color color, Rect customRectToDrawIn = null)
        {
            var rect = new RectF(0, 0, canvas.Width, canvas.Height);
            Bitmap changedMarker = Bitmap.CreateBitmap(canvas.Width, canvas.Height, Bitmap.Config.Argb8888);

            Canvas changeColor = new Canvas(changedMarker);
            changeColor.DrawPicture(picture, rect);
            changeColor.DrawColor(color, PorterDuff.Mode.SrcAtop);
            changeColor.Dispose();

            if (customRectToDrawIn == null) canvas.DrawBitmap(changedMarker, null, rect, null);
            else canvas.DrawBitmap(changedMarker, null, customRectToDrawIn, null);
            changedMarker.Dispose();
        }

        /// <summary>
        /// Ajoute un point à la zone
        /// </summary>
        private Marker AddPointToZone(LatLng point, bool isInAlert)
        {
            var markerZone = CreateMarkerZoneMap(point, true);
            // redessine la map pour mettre à jour la couleur des marker
            CreateZone(ConvertLatitudeLongitudeToLatLng(PointsOfZone.Select(el => el.Position).ToList()), isInAlert);
            var markerToAdd = _map.AddMarker(markerZone);
            PointsOfZone.Add(markerToAdd);

            if (ZonePolygon == null)
            {
                CreateZone(ConvertLatitudeLongitudeToLatLng(PointsOfZone.Select(el => el.Position).ToList()), isInAlert);
                RefreshZone();
                return PointsOfZone.Where(el => el.Position.Latitude == point.Latitude && el.Position.Longitude == point.Longitude).FirstOrDefault();
            }
            ZonePolygon.Points = PointsOfZone.Select(el => el.Position).ToList();
            RefreshZone();

            return markerToAdd;
        }

        /// <summary>
        /// Initialise un marker pour la zone
        /// </summary>
        private MarkerOptions CreateMarkerZoneMap(LatLng point, bool isLastPoint)
        {
            MarkerOptions markerZone = new MarkerOptions();
            markerZone.Draggable(true);
            markerZone.SetSnippet("MarkerZone");
            markerZone.SetPosition(point);
            markerZone.Anchor(0.5f, 0.5f);

            SvgBitmapDrawable markerDrawable = SvgFactory.GetDrawable(
                _activityContext.Resources
                , Resource.Drawable.MarkerZonev2);
            Bitmap markerBitmap = Bitmap.CreateBitmap(
                markerDrawable.Picture.Width
                , markerDrawable.Picture.Height
                , Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(markerBitmap);
            canvas.DrawPicture(markerDrawable.Picture);
            if (isLastPoint) canvas.DrawColor(Color.Green, PorterDuff.Mode.SrcAtop);

            BitmapDescriptor bitMapDescr = BitmapDescriptorFactory.FromBitmap(
                Bitmap.CreateScaledBitmap(markerBitmap
                , (int)(markerBitmap.Width * 0.75)
                , (int)(markerBitmap.Height * 0.75)
                , false));

            markerZone.SetIcon(bitMapDescr);

            markerDrawable.Dispose();
            markerBitmap.Dispose();
            bitMapDescr.Dispose();
            canvas.Dispose();

            return markerZone;
        }

        /// <summary>
        /// Supprime un point de la zone
        /// </summary>
        private void RemovePointOfZone(Marker point, bool isInAlert, bool isUndo)
        {
            if (point == null) return;

            var pointsToRestore = PointsOfZone.Select(el => el.Position).ToList();
            var pointToRemove = PointsOfZone.FirstOrDefault(
                    el => el.Position.Latitude == point.Position.Latitude
                        && el.Position.Longitude == point.Position.Longitude);

            if (pointToRemove == null) return;

            pointToRemove.Remove();
            PointsOfZone.Remove(pointToRemove);

            if (PointsOfZone.Count == 0)
            {
                ZonePolygon.Remove();
                ZonePolygon = null;
                RefreshZone();
                return;
            }

            ZonePolygon.Points = PointsOfZone.Select(el => el.Position).ToList();
            CreateZone(ConvertLatitudeLongitudeToLatLng(ZonePolygon.Points.ToList()), isInAlert, true, -1);
            RefreshZone();

            SelectedMarker = null;

            if (!isUndo)
                UndoActions.Add(new Action(() =>
                {
                    CreateZone(ConvertLatitudeLongitudeToLatLng(pointsToRestore), true);
                    RefreshZone();
                }));
        }

        /// <summary>
        /// Initialise un marker pour le parcours
        /// </summary>
        private MarkerOptions CreateMarkerRouteMap(LatLng point, DateTime dateLocation)
        {
            return CreateMarkerRouteMap(point, DateExtension.FormatDateTimeFromNow(dateLocation));
        }

        /// <summary>
        /// Initialise un marker pour le parcours
        /// </summary>
        private MarkerOptions CreateMarkerRouteMap(LatLng point, string dateLocation)
        {
            MarkerOptions markerRoute = new MarkerOptions();
            markerRoute.Draggable(false);
            markerRoute.SetSnippet("MarkerRoute");
            markerRoute.SetPosition(point);
            markerRoute.SetTitle(dateLocation);
            BitmapDescriptor bitMapDescr = null;

            if (_activityContext != null)
            {
                SvgBitmapDrawable markerDrawable = SvgFactory.GetDrawable(_activityContext.Resources, IsInAlert ? Resource.Drawable.MarkerZonev2Red : Resource.Drawable.MarkerZonev2);
                Bitmap markerBitmap = Bitmap.CreateBitmap(markerDrawable.Picture.Width, markerDrawable.Picture.Height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(markerBitmap);
                canvas.DrawPicture(markerDrawable.Picture);

                bitMapDescr = BitmapDescriptorFactory.FromBitmap(Bitmap.CreateScaledBitmap(markerBitmap
                    , (int)(markerBitmap.Width * 0.6)
                    , (int)(markerBitmap.Height * 0.6)
                    , false));
                markerRoute.Anchor(0.5f, 0.5f);

                markerRoute.SetIcon(bitMapDescr);
                bitMapDescr.Dispose();
            }
            else return null;

            return markerRoute;
        }

        /// <summary>
        /// Initialise un marker avec un style sélectionné pour le parcours
        /// </summary>
        private MarkerOptions CreateSelectedMarkerRouteMap(LatLng point, DateTime dateLocation)
        {
            MarkerOptions markerRoute = new MarkerOptions();
            markerRoute.Draggable(false);
            markerRoute.SetSnippet("MarkerRoute");
            markerRoute.SetPosition(point);
            markerRoute.SetTitle(DateExtension.FormatDateTimeFromNow(dateLocation));

            var bitMapDescr = BitmapDescriptorFactory.FromResource(Resource.Drawable.MarkerZonev2);
            markerRoute.SetIcon(bitMapDescr);
            bitMapDescr.Dispose();

            return markerRoute;
        }

        /// <summary>
        /// Initialise un marker avec un style sélectionné pour le parcours
        /// </summary>
        private MarkerOptions CreateSelectedMarkerHistory(LatLng point, DateTime dateLocation, double accuracy = 0)
        {
            MarkerOptions markerRoute = new MarkerOptions();
            markerRoute.Draggable(false);
            markerRoute.SetSnippet("LocationHistory");
            markerRoute.SetPosition(point);
            markerRoute.SetTitle(DateExtension.FormatDateTimeFromNow(dateLocation));

            SvgBitmapDrawable markerDrawable = SvgFactory.GetDrawable(
                    _activityContext.Resources
                    , Resource.Drawable.MarkerZonev2);
            Bitmap markerBitmap = Bitmap.CreateBitmap(
                    markerDrawable.Picture.Width
                    , markerDrawable.Picture.Height
                    , Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(markerBitmap);
            canvas.DrawPicture(markerDrawable.Picture);

            if (accuracy != 0)
                canvas.DrawColor(Color.ParseColor(COLOR_ORANGE), PorterDuff.Mode.SrcAtop);

            BitmapDescriptor bitMapDescr = BitmapDescriptorFactory.FromBitmap(
                Bitmap.CreateScaledBitmap(markerBitmap
                , (int)(markerBitmap.Width * 0.75)
                , (int)(markerBitmap.Height * 0.75)
                , false));

            markerRoute.SetIcon(bitMapDescr);
            markerRoute.Anchor(0.5f, 0.5f);//important pour bien placer l'img par rapport au point

            bitMapDescr.Dispose();
            markerBitmap.Dispose();
            markerDrawable.Dispose();
            canvas.Dispose();

            return markerRoute;
        }

        /// <summary>
        /// Lève un event pour notifier qu'il faut mettre à jour les données de la zone sur la vue
        /// </summary>
        private void RefreshZone()
        {
            ZoneInformationUpdated?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region ===== Event =======================================================================

        #region Collection Changed

        /// <summary>
        /// Modification de la collection qui gère le retour en arrière
        /// </summary>
        private void OnUndoActionsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (App.Locator.ModeZone.SelectedFavoriteArea != null) App.Locator.ModeZone.SelectedFavoriteArea = null;
            _undoSvgImageView.SetSvg(_activityContext
                , Resource.Drawable.ModeZoneGoBack
                , UndoActions.Count == 0 ? string.Empty : "#666666=#36da3e"
                /*, UndoActions.Count > 0 ? string.Empty : string.Empty*/);
        }

        #endregion

        #region Map

        /// <summary>
        /// Déplacement d'un marker
        /// </summary>
        private void OnMarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
        {
            PolygonOptions rectOptions = new PolygonOptions();
            foreach (var point in PointsOfZone)
            {
                rectOptions.Add(point.Position);
            }
            rectOptions.InvokeFillColor(int.Parse("444dbea0", System.Globalization.NumberStyles.HexNumber));
            rectOptions.InvokeStrokeColor(int.Parse("994dbea0", System.Globalization.NumberStyles.HexNumber));
            rectOptions.InvokeStrokeWidth(2);
            _updatingAreaPolygon = _map.AddPolygon(rectOptions);
        }

        /// <summary>
        /// Déplacement d'un marker
        /// </summary>
        private void OnMarkerDrag(object sender, GoogleMap.MarkerDragEventArgs e)
        {
            _updatingAreaPolygon.Points = PointsOfZone.Select(el => el.Position).ToList();
        }

        /// <summary>
        /// Déplacement d'un marker
        /// </summary>
        private void OnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            _updatingAreaPolygon.Remove();

            var savedPoints = ZonePolygon.Points.ToList();

            ZonePolygon.Points = PointsOfZone.Select(el => el.Position).ToList();
            RefreshZone();

            UndoActions.Add(new Action(() =>
            {
                CreateZone(ConvertLatitudeLongitudeToLatLng(savedPoints), false);
                RefreshZone();
            }));
        }

        /// <summary>
        /// Clique sur la map
        /// </summary>
        private void OnMapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if (!IsOnPointAdding) return;
            if (ZonePolygon != null && ZonePolygon.Points.Count() > App.Locator.ModeZone.MAX_NUMBER_OF_POINTS) return;

            var pointsToRestore = PointsOfZone.Select(el => el.Position).ToList();
            var markerToAdd = AddPointToZone(e.Point, false);

            UndoActions.Add(new Action(() =>
            {
                CreateZone(ConvertLatitudeLongitudeToLatLng(pointsToRestore), false, true);
                RefreshZone();
            }));

        }

        /// <summary>
        /// Clique sur un marker
        /// </summary>
        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            if (!App.Locator.ModeZone.IsOnEditMode)
            {
                SelectedMarker = e.Marker;
                e.Marker.ShowInfoWindow();
                e.Handled = true;
                if (SeekiosMarkerClicked == null) return;
                var markerAsso = _markerIdAssociation.FirstOrDefault(el => el.MarkerId == e.Marker.Id);
                if (markerAsso == null) return;
                SeekiosMarkerClicked.Invoke(sender, markerAsso.SeekiosId);
            }
        }

        private void OnMyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            if (UserLocationChanged == null) return;
            UserLocationChanged.Invoke(sender, null);
        }

        #endregion

        #region Button Action

        /// <summary>
        /// Ajoute un point de zone sur la map
        /// </summary>
        private void OnAddPoint(object sender, EventArgs e)
        {
            IsOnPointAdding = !IsOnPointAdding;
        }

        /// <summary>
        /// Revient en arrière
        /// </summary>
        private void OnUndoChanges(object sender, EventArgs e)
        {
            if (UndoActions.Count <= 0) return;
            UndoActions.Last().Invoke();
            UndoActions.Remove(UndoActions.Last());
        }

        /// <summary>
        /// Centre la map sur le seekios
        /// </summary>
        private void OnCenterSeekios(object sender, EventArgs e)
        {
            CenterInMarker(_idSeekios);
        }

        private void CenterOnUnaccurateLocation(Circle cercle, double lattitude, double longitude, double accuracy, bool with_animation = true)
        {
            if (cercle == null) return;
            try
            {
                var circleOptions = new CircleOptions();
                circleOptions.InvokeCenter(new LatLng(lattitude, longitude));
                circleOptions.InvokeRadius(accuracy);
                if (with_animation)
                {
                    _map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(circleOptions.Center, GetZoomLevel(cercle)));
                }
                else _map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(circleOptions.Center, GetZoomLevel(cercle)));
            }
            catch (Exception) { }
        }

        private int GetZoomLevel(Circle circle)
        {
            int zoomLevel = 11;
            if (circle != null)
            {
                double radius = circle.Radius + (circle.Radius / 2);
                double scale = radius / 500;
                zoomLevel = (int)(16 - (Math.Log(scale) / Math.Log(2)));
            }
            return zoomLevel;
        }

        /// <summary>
        /// switch between satellite and normal mode
        /// </summary>
        public void ChangeMapType(bool isInNormalMode)
        {
            App.Locator.BaseMap.SaveDataChangeMap(isInNormalMode);
            if (isInNormalMode)
            {
                _map.MapType = GoogleMap.MapTypeNormal;
            }
            else _map.MapType = GoogleMap.MapTypeSatellite;
        }

        #endregion

        #endregion
    }
}