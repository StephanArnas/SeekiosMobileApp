using GalaSoft.MvvmLight.Views;
using SeekiosApp.Enum;
using SeekiosApp.Interfaces;
using System.Linq;

namespace SeekiosApp.ViewModel
{
    public class MapAllSeekiosViewModel : MapViewModelBase
    {
        #region ===== Properties ==================================================================

        public bool HasRemovedButtonAnnotationView { get; set; }

        #endregion

        #region ===== Constructor =================================================================

        public MapAllSeekiosViewModel(IDispatchOnUIThread dispatcherService
            , IDataService dataService
            , Interfaces.IDialogService dialogService
            , ILocalNotificationService localNotificationService) 
            : base(dispatcherService, dataService, dialogService, localNotificationService) { }

        #endregion
        
        #region ===== Public Methods ==============================================================

        /// <summary>
        /// Allows to zoom on a seekios marker using its index in the list
        /// </summary>
        public void ZoomToSeekios()
        {
            if (Seekios == null) return;
            MapControlManager.CenterInMarker(Seekios.Idseekios.ToString(), true);
        }

        /// <summary>
        /// Initialise the map
        /// </summary>
        public override void InitMap()
        {
            MapControlManager.InitMap(22f, true);
            // display all seekios on the map
            foreach (var seekios in App.CurrentUserEnvironment.LsSeekios)
            {
                if (seekios != null && seekios.LastKnownLocation_dateLocationCreation.HasValue
                    && (seekios.LastKnownLocation_latitude != App.DefaultLatitude
                    && seekios.LastKnownLocation_longitude != App.DefaultLongitude))
                {
                    var mode = App.CurrentUserEnvironment.LsMode.FirstOrDefault(el => el.Seekios_idseekios == seekios.Idseekios);
                    var isDontMove = mode != null && mode.ModeDefinition_idmodeDefinition == (int)ModeDefinitionEnum.ModeDontMove;

                    MapControlManager.CreateSeekiosMarkerAsync(seekios.Idseekios.ToString()
                        , seekios.SeekiosName
                        , seekios.SeekiosPicture
                        , seekios.LastKnownLocation_dateLocationCreation.Value
                        , seekios.LastKnownLocation_latitude
                        , seekios.LastKnownLocation_longitude
                        , seekios.LastKnownLocation_accuracy
                        , isDontMove);
                }
            }
            MapControlManager.SeekiosMarkerClicked += OnSeekiosMarkerClicked;
        }

        /// <summary>
        /// Action performed when the marker is clicked
        /// </summary>
        private void OnSeekiosMarkerClicked(object sender, string idSeekios)
        {
            Seekios = App.CurrentUserEnvironment.LsSeekios.FirstOrDefault(el => el.Idseekios.ToString() == idSeekios);
        }

        #endregion
    }
}