using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeekiosApp.Interfaces
{
    public interface IMapControlManager
    {
        /// <summary>
        /// Method allowing to initialize the map options
        /// </summary>
        /// <param name="zoomLevel">float zoom level</param>
        /// <param name="zoom">bool zoom activated or not</param>
        /// <param name="compass">bool compass displayed or not</param>
        /// <param name="mapToolbar">bool map toolbar displayed or not</param>
        void InitMap(float zoomLevel, bool zoom = false, bool compass = true, bool mapToolbar = true);

        /// <summary>
        /// Create a Seekios marker with a refresh button and the Seekios image in base64
        /// return the marker id map
        /// </summary>
        void CreateSeekiosMarkerAsync(string seekiosId, string title, string imageBase64, DateTime? lastLocationDate, double latitude, double longitude, double accuracy, bool isDontMove, bool isInAlert = false);

        /// <summary>
        /// Center the map on the marker id. If the id doesn't exist, the method returns false
        /// </summary>
        bool CenterInMarker(string seekiosId, bool showInfoWindow = false, bool withAnimation = false);

        /// <summary>
        /// Center the map on a position. This action can be animated or not (not animated by default)
        /// </summary>
        void CenterInLocalisation(double latitude, double longitude, float zoomLevel, bool withAnimation = false);
        
        /// <summary>
        /// Center the map view on the list of locations
        /// </summary>
        /// <param name="locations">List of locations</param>
        void CenterOnLocations(List<LatitudeLongitude> locations, bool with_animation = false);

        /// <summary>
        /// Center the map view on the current zone
        /// </summary>
        void CenterOnZone();

        /// <summary>
        /// Method used to dispose of the mapcontrol and the markers
        /// </summary>
        void Dispose();

        /// <summary>
        /// Remove all markers
        /// </summary>
        void RemoveAllMarkers();

        /// <summary>
        /// Create a zone on the map
        /// </summary>
        void CreateZone(List<LatitudeLongitude> coordonates, bool isInAlert, bool isLastMarker = false, int index = 0);

        /// <summary>
        /// Create all undo actions necessary to remove actual zone
        /// </summary>
        void CreateUndoActionsToRemoveActualZone();

        /// <summary>
        /// Delete zone 
        /// </summary>
        void DeleteZone();

        /// <summary>
        /// Create a route on the map
        /// </summary>
        void CreateRoute(List<LocationDTO> coordonates);

        /// <summary>
        /// Load marker for tracking mode in foreground
        /// </summary>
        void CreateRouteForeground(List<LocationDTO> coordonates);

        /// <summary>
        /// Load marker for tracking mode in background
        /// </summary>
        void CreateRouteBackground(List<LocationDTO> coordonates);


        void ChangeMapType(bool isInNormalMode);
        
        /// <summary>
        /// Modify the point selected within the location history
        /// </summary>
        void ChangeSelectedLocationHistory(LocationDTO coord);

        /// <summary>
        /// Delete the point selected within the location history
        /// </summary>
        void RemoveSelectedHistoryMarker();

        /// <summary>
        /// Register methodes
        /// </summary>
        void RegisterMethodes();

        /// <summary>
        /// Unregister methodes
        /// </summary>
        void UnregisterMethodes();

        bool IsSeekiosInZone(LatitudeLongitude seekiosPosition, List<LatitudeLongitude> zone);

        #region Event

        /// <summary>
        /// Event fired when a marker is clicked
        /// </summary>
        event EventHandler<string> SeekiosMarkerClicked;

        /// <summary>
        /// Event fired when the user location changed
        /// </summary>
        event EventHandler UserLocationChanged;

        /// <summary>
        /// Event fired when the maker option has all been set up (only android require)
        /// </summary>
        event Func<object, EventArgs, Task> OnInitTrackingRouteComplete;

        #endregion

        #region Useless Code

        /// <summary>
        /// Center the map on a position. This action can be animated or not (not animated by default)
        /// </summary>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //void GoToLocation(LatitudeLongitude latlong, string title, bool withAnimation = false);

        /// <summary>
        /// Get address form latlong
        /// </summary>
        /// <param name="context"></param>
        /// <param name="latlong"></param>
        /// <returns></returns>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //Task<Address> GetAddressFromLatLong(LatitudeLongitude latlong);

        /// <summary>
        /// Get latlong from address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //LatitudeLongitude GetLatLongFromAdress(string address);

        /// <summary>
        /// Get address from address
        /// </summary>
        /// <param name="context"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //Task<List<string>> GetAddressList(string address);

        /// <summary>
        /// Remove a marker thanks to its position
        /// </summary>
        /// <param name="latlong"></param>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //void RemoveMarker(LatitudeLongitude latlong);

        /// <summary>
        /// Center the map view on the current user location
        /// </summary>
        /// <param name="withAnimation">do translation with animation or not</param>
        //WARNING : Not use in version 1.0 of the app, It's require for mode Follow Me or In Time
        //bool CenterOnMyLocation(bool withAnimation = false);

        #endregion

    }
}
