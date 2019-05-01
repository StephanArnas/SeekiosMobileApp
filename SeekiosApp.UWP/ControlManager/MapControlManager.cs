using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using System.IO;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.FileProperties;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Shapes;
using SeekiosApp.UWP.UserControls;
using SeekiosApp.Extension;

namespace SeekiosApp.UWP.ControlManager
{
    public class MapControlManager : IMapControlManager
    {
        #region ===== Constants ===================================================================

        private const int IN_TIME_ACCURACY = 20;
        private const int NUMBER_OF_ADDRESSES = 10;
        private const int INTIME_ZOOM_LEVEL = 18;

        #endregion

        #region ===== Attributs ===================================================================

        private Button _focusOnSeekiosButton = null;
        private Button _focusOnZoneButton = null;
        private Button _changeMapTypeButton = null;
        private bool markerSeekiosOverlayShown = false;
        private MapSeekiosMarkerOverlay _mapSeekiosMarkerOverlay = null;
        private Geopoint _seekiosMarkerGeoPoint = null;

        #endregion

        #region ===== Properties ==================================================================

        public MapControl Map { get; set; }

        public static bool IsOutOf { get; set; }

        public event EventHandler<string> SeekiosMarkerClicked;
        public event EventHandler UserLocationChanged;
        public event Func<object, EventArgs, Task> OnInitTrackingRouteComplete;

        #endregion

        #region ===== Constructors ================================================================

        [PreferredConstructor]
        public MapControlManager(MapControl mapControl
            , Button focusOnSeekiosButton
            , Button changeMapTypeButton)
        {
            Map = mapControl;
            _focusOnSeekiosButton = focusOnSeekiosButton;
            _changeMapTypeButton = changeMapTypeButton;
            IsOutOf = false;
        }

        #endregion

        #region ===== Public Methods ==============================================================


        public void CenterInLocalisation(double latitude, double longitude, float zoomLevel, bool withAnimation = false)
        {

        }

        public bool CenterInMarker(string seekiosId, bool showInfoWindow = false, bool withAnimation = false)
        {
            return true;
        }

        public void CenterOnLocations(List<LatitudeLongitude> locations, bool with_animation = false)
        {

        }

        public void CenterOnZone()
        {

        }

        public void ChangeMapType(bool isInNormalMode)
        {

        }

        public void ChangeSelectedLocationHistory(LocationDTO coord)
        {

        }

        public void CreateRoute(List<LocationDTO> coordonates)
        {

        }

        public void CreateRouteBackground(List<LocationDTO> coordonates)
        {

        }

        public void CreateRouteForeground(List<LocationDTO> coordonates)
        {

        }

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
            // Clear all the marker
            if (Map.Children.Count > 0)
            {
                foreach (var child in Map.Children)
                {
                    Map.Children.Remove(child);
                }
            }

            // Remove the previous marker overlay
            if (markerSeekiosOverlayShown)
            {
                Map.Children.Remove(_mapSeekiosMarkerOverlay);
                markerSeekiosOverlayShown = false;
            }

            // Specify a known location.
            var seekiosMarkerPosition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            _seekiosMarkerGeoPoint = new Geopoint(seekiosMarkerPosition);

            //// Setup the seekios green marker as writableBitmap
            //var markerSeekiosFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Icons/MarkerSeekios.png"));
            //var markerSeekiosBitmap = new WriteableBitmap(92, 150);
            //using (var fileStream = await markerSeekiosFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            //{
            //    markerSeekiosBitmap.SetSource(fileStream);
            //}

            //// Setup the picture seekios as writableBitmap
            //var pictureSeekiosBitmap = new WriteableBitmap(90, 90);
            //using (var memoryStream = new InMemoryRandomAccessStream())
            //using (var dataWriter = new DataWriter(memoryStream.GetOutputStreamAt(0)))
            //{
            //    dataWriter.WriteBytes(Convert.FromBase64String(imageBase64));
            //    dataWriter.StoreAsync().GetResults();
            //    pictureSeekiosBitmap.SetSource(memoryStream);
            //}

            //// Merge the two bitmap 
            //markerSeekiosBitmap.Blit(new Rect(5, 5, pictureSeekiosBitmap.PixelWidth, pictureSeekiosBitmap.PixelHeight)
            //    , pictureSeekiosBitmap
            //    , new Rect(0, 0, pictureSeekiosBitmap.PixelWidth, pictureSeekiosBitmap.PixelHeight));

            // Create the seekios green marker
            var markerSeekiosImage = new Image();
            markerSeekiosImage.Height = 150;
            markerSeekiosImage.Width = 92;
            markerSeekiosImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Icons/MarkerSeekios.png"));

            // Create the seekios picture
            var pictureSeekiosEllipse = new Ellipse();
            pictureSeekiosEllipse.Height = 80;
            pictureSeekiosEllipse.Width = 80;
            pictureSeekiosEllipse.Margin = new Windows.UI.Xaml.Thickness(6, 8, 0, 0);
            using (InMemoryRandomAccessStream memoryStream = new InMemoryRandomAccessStream())
            using (DataWriter dataWriter = new DataWriter(memoryStream.GetOutputStreamAt(0)))
            {
                dataWriter.WriteBytes(Convert.FromBase64String(imageBase64));
                dataWriter.StoreAsync().GetResults();
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(memoryStream);
                pictureSeekiosEllipse.Fill = new ImageBrush() { ImageSource = bitmapImage };
            }

            // Create Layout that combine seekios green marker and seekios picture
            var seekiosMarkerLayout = new Canvas();
            seekiosMarkerLayout.Height = 150;
            seekiosMarkerLayout.Width = 92;
            seekiosMarkerLayout.Children.Add(markerSeekiosImage);
            seekiosMarkerLayout.Children.Add(pictureSeekiosEllipse);

            //Map.Children.Clear();
            Map.MapTapped -= Map_MapTapped;
            Map.MapTapped += Map_MapTapped;
            seekiosMarkerLayout.Tapped += PictureSeekiosEllipse_Tapped;
            //using (var memoryRandomAccessStream = new InMemoryRandomAccessStream())
            //{
            //    await markerSeekiosBitmap.ToStream(memoryRandomAccessStream, BitmapEncoder.PngEncoderId);

            //    // Create a MapIcon.
            //    var seekiosMapIcon = new MapIcon();

            //    seekiosMapIcon.Image = RandomAccessStreamReference.CreateFromStream(memoryRandomAccessStream);
            //    seekiosMapIcon.Location = geoPoint;
            //    seekiosMapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            //    seekiosMapIcon.Title = title;
            //    seekiosMapIcon.ZIndex = 0;
            //    seekiosMapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;

            // Add the MapIcon to the map.
            //Map.MapElements.Add(seekiosMapIcon);

            // Center the map over the POI.
            Map.Center = _seekiosMarkerGeoPoint;
            Map.ZoomLevel = 14;
            Map.Children.Add(seekiosMarkerLayout);
            MapControl.SetLocation(seekiosMarkerLayout, _seekiosMarkerGeoPoint);
            MapControl.SetNormalizedAnchorPoint(seekiosMarkerLayout, new Point(0.5, 1));
            _mapSeekiosMarkerOverlay = new MapSeekiosMarkerOverlay(title, lastLocationDate.Value.FormatDateFromNow());
            PictureSeekiosEllipse_Tapped(seekiosMarkerLayout, null);
            //}
        }

        private void Map_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            if (markerSeekiosOverlayShown)
            {
                Map.Children.Remove(_mapSeekiosMarkerOverlay);
                markerSeekiosOverlayShown = false;
            }
        }

        private void PictureSeekiosEllipse_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var seekiosMarker = sender as Canvas;
            if (seekiosMarker != null && _mapSeekiosMarkerOverlay != null)
            {
                if (!markerSeekiosOverlayShown)
                {
                    Map.Children.Add(_mapSeekiosMarkerOverlay);
                    MapControl.SetLocation(_mapSeekiosMarkerOverlay, _seekiosMarkerGeoPoint);
                    MapControl.SetNormalizedAnchorPoint(_mapSeekiosMarkerOverlay, new Point(0.5, 1));

                    markerSeekiosOverlayShown = true;
                }
                else
                {
                    Map.Children.Remove(_mapSeekiosMarkerOverlay);
                    markerSeekiosOverlayShown = false;
                }
            }
        }

        public void CreateUndoActionsToRemoveActualZone()
        {

        }

        public void CreateZone(List<LatitudeLongitude> coordonates, bool isInAlert, bool isLastMarker = false, int index = 0)
        {

        }

        public void DeleteZone()
        {

        }

        public void Dispose()
        {

        }

        public void InitMap(float zoomLevel, bool zoom = false, bool compass = true, bool mapToolbar = true)
        {

        }

        public bool IsSeekiosInZone(LatitudeLongitude seekiosPosition, List<LatitudeLongitude> zone)
        {
            return true;
        }

        public void RegisterMethodes()
        {

        }

        public void RemoveAllMarkers()
        {

        }

        public void RemoveSelectedHistoryMarker()
        {

        }

        public void UnregisterMethodes()
        {

        }

        #endregion
    }
}
