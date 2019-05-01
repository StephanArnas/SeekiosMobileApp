using SeekiosApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeekiosApp.Model.APP;
using SeekiosApp.Model.DTO;

namespace SeekiosDataServiceUnitTest.Services
{
    public class MapControlManager : IMapControlManager
    {
        public event Func<object, EventArgs, Task> OnInitTrackingRouteComplete;
        public event EventHandler<string> SeekiosMarkerClicked;
        public event EventHandler UserLocationChanged;

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
            throw new NotImplementedException();
        }

        public void ChangeSelectedLocation(LocationDTO coordonate)
        {
            
        }

        public void ChangeSelectedLocationHistory(LocationDTO coord)
        {
            
        }

        public void CreateRoute(List<LocationDTO> coordonates)
        {
            
        }

        public void CreateRouteBackground(List<LocationDTO> coordonates, bool notOnlyGps = true)
        {
        }

        public void CreateRouteForeground(List<LocationDTO> coordonates)
        {
            
        }

        public string CreateSeekiosMarker(string seekiosId, string title, string imageBase64, DateTime? lastLocationDate, double latitude, double longitude, double accuracy, bool isDontMove, bool isInAlert = false)
        {
            return seekiosId;
        }

        public void CreateUndoActionsToRemoveActualZone()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
    }
}
