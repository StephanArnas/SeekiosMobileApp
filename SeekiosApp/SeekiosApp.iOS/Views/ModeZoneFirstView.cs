using Foundation;
using MapKit;
using SeekiosApp.IOS.Helper;
using SeekiosApp.ViewModel;
using System;
using System.Linq;
using UIKit;
using SeekiosApp.iOS.Helper;
using SeekiosApp.iOS.ControlManager;
using SeekiosApp.iOS.Views.BaseView;
using System.Collections.Generic;
using SeekiosApp.Model.APP;
using SeekiosApp.Enum;

namespace SeekiosApp.iOS
{
    public partial class ModeZoneFirstView : BaseViewControllerMap
    {
        #region ===== Attributs ===================================================================

        private bool _isNoPoint = true;
        public static bool IsOnModeZone = false;

        #endregion

        #region ===== Constructor =================================================================

        public ModeZoneFirstView(IntPtr handle) : base(handle) { }

        #endregion

        #region ===== Life Cycle ==================================================================

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            App.Locator.ModeZone.IsActivityFocused = true;
            //App.Locator.ModeZone.PropertyChanged += ModeZone_PropertyChanged;
            //App.Locator.ModeZone.OnSeekiosOutOfZoneNotified += ModeZone_OnSeekiosOutOfZoneNotified;
            //App.Locator.ModeZone.OnSeekiosBackInZoneNotified += ModeZone_OnSeekiosBackInZoneNotified;
            //App.Locator.ModeZone.OnNewZoneTrackingLocationAddedNotified += ModeZone_OnNewZoneTrackingLocationAddedNotified;
        }

        public override void ViewWillAppear(bool animated)
        {
            Title = Application.LocalizedString("ModeZoneTitle");
            IsOnModeZone = true;
            MapControlManager.IsOutOf = false;

            if (App.Locator.ModeZone.IsGoingBack)
            {
                GoBack(false);
                return;
            }

            if (_mapControlManager == null)
            {
                base.ViewWillAppear(animated);
            }

            NextButton.TouchUpInside += NextButton_TouchUpInside;
            if (_mapControlManager != null)
            {
                _mapControlManager.ZoneInformationUpdated += MapControlManager_ZoneInformationUpdated;
            }

            TimerLabel.Hidden = true;
            RefreshInProgressButton.Hidden = true;
        }

        public override void ViewWillDisappear(bool animated)
        {
            IsOnModeZone = false;
            if (App.Locator.ModeZone.IsGoingBack)
            {
                Title = "seekios";
            }
            else
            {
                Title = "1/3";
            }

            NextButton.TouchUpInside -= NextButton_TouchUpInside;
            if (_mapControlManager != null)
            {
                _mapControlManager.ZoneInformationUpdated -= MapControlManager_ZoneInformationUpdated;
            }

            base.ViewWillDisappear(animated);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            App.Locator.ModeZone.IsOnEditMode = false;
            App.Locator.ModeZone.IsActivityFocused = false;

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
            InitialiseAllStrings();

            NextButton.Layer.MasksToBounds = true;
            NextButton.Layer.CornerRadius = 4;
            FocusOnZoneButton.Hidden = true;
        }

        public override void SetMapControlManager()
        {
            // map initialisation
            _mapControlManager = new MapControlManager(MapViewControl
                , this
                , FocusOnSeekiosButton
                , FocusOnZoneButton
                , ChangeMapTypeButton
                , MapZoomInButton
                , MapZoomOutButton
                , null
                , UndoButton
                , NextButton
                , App.Locator.DetailSeekios.SeekiosSelected.Idseekios.ToString());

            App.Locator.ModeZone.MapControlManager = _mapControlManager;
            App.Locator.ModeZone.InitMap();
            App.Locator.ModeZone.InitMode();
            App.Locator.ModeZone.InitZone();

            if (App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_accuracy > 0)
            {
                // display the area for accuracy (triangulation)
                _mapControlManager.CreateAccuracyArea(App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_latitude
                    , App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_longitude
                    , App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_accuracy);
            }
            else
            {
                // big zoom
                _mapControlManager.CenterInLocalisation(App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_latitude
                    , App.Locator.DetailSeekios.SeekiosSelected.LastKnownLocation_longitude
                    , (float)ZoomLevelEnum.BigZoom
                    , true);
            }

            _mapControlManager.RegisterMethodes();
            _mapControlManager.IsOnPointAdding = true;
        }

        #endregion

        #region ===== Public Methodes =============================================================

        #endregion

        #region ===== Private Methodes ============================================================


        private void RefreshMarkers()
        {
            //if (_mapControlManager.ZonePolygon == null || _mapControlManager.ZonePolygon.Points.Count() == 0)
            //{
            //    NumberOfPointsLabel.Text = "0/" + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;
            //    SurfaceLabel.Text = string.Format("0 {0}", "km²");
            //    return;
            //}
            FocusOnZoneButton.Hidden = true;

            // 1 points
            if (_mapControlManager.ZonePolygon.Points.Count() == 2 && _isNoPoint)
            {
                _isNoPoint = false;
                //NumberOfPointsLabel.Text = "1 / " + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;
                NumberOfPointsLabel.Text = Application.LocalizedString("Need3PointsForZone");
                FocusOnZoneButton.Hidden = true;
            }
            // 0 points
            else if (_mapControlManager.ZonePolygon.Points.Count() == 2)
            {
                _isNoPoint = true;
                //NumberOfPointsLabel.Text = "0 / " + App.Locator.ModeZone.MAX_NUMBER_OF_POINTS;
                NumberOfPointsLabel.Text = Application.LocalizedString("ClickOnMap");
                FocusOnZoneButton.Hidden = true;
            }
            // 2 points and more
            else
            {
                _isNoPoint = true;
                NumberOfPointsLabel.Text = Application.LocalizedString("Need3PointsForZone");
                if ((_mapControlManager.ZonePolygon.Points.Count() - 1) == 10)
                {
                    var actionSheetAlert = AlertControllerHelper.CreatePopupMaximumPointZone();
                    PresentViewController(actionSheetAlert, true, null);
                }
                if (_mapControlManager.ZonePolygon.Points.Count() - 1 > 2)
                {
                    FocusOnZoneButton.Hidden = false;
                    NumberOfPointsLabel.Text = string.Empty;
                }
            }

            //var surface = AreaHelper.CalculateAreaOfGPSPolygonOnEarthInSquareMeters(_mapControlManager.PointsOfZone);
            //SurfaceLabel.Text = AreaHelper.SerializeArea(surface);
        }

        private void InitialiseAllStrings()
        {
            RefreshInProgressButton.SetTitle(Application.LocalizedString("InProgress"), UIControlState.Normal);
            NextButton.SetTitle(Application.LocalizedString("Next"), UIControlState.Normal);
            NumberOfPointsLabel.Text = Application.LocalizedString("ClickOnMap");
        }

        #endregion

        #region ===== Event =======================================================================

        private void NextButton_TouchUpInside(object sender, EventArgs e)
        {
            var zone = _mapControlManager.PointsOfZone.Select(el => new LatitudeLongitude (el.Coordinate.Latitude, el.Coordinate.Longitude)).ToList();
			var listOfPoints = new List<LatitudeLongitude>();
			var time = (DateTime.UtcNow - App.Locator.DetailSeekios.SeekiosSelected.DateLastCommunication).Value.TotalHours;
			//If last position known > 1 hour ago
			if (time > 1)
			{
                var popup = AlertControllerHelper.CreateAlertToInformSeekiosPositionMoreThan1Hour(() =>
                {
                    if (_mapControlManager.PointsOfZone != null)
                    {
                        App.Locator.ModeZone.GoToSecondPage(zone);
                    }
                    else App.Locator.ModeZone.GoToSecondPage(null);
                });
                PresentViewController(popup, true, null);
            }
            //else we verify if the last position known is in the zone or not
            else 
			{ 
				//verify zone validity
                if (_mapControlManager.PointsOfZone == null)
                {
                    App.Locator.ModeZone.GoToSecondPage(null);
                }
                App.Locator.ModeZone.GoToSecondPage(zone);
			}
        }

        private void ModeZone_OnNewZoneTrackingLocationAddedNotified(double lat, double lon, double altitude, double accuracy, DateTime dateCommunication)
        {

        }

        private void ModeZone_OnSeekiosBackInZoneNotified(double lat, double lon, double altitude, double accuracy, DateTime dateCommunication)
        {

        }

        private void ModeZone_OnSeekiosOutOfZoneNotified(double lat, double lon, double altitude, double accuracy, DateTime dateCommunication)
        {

        }

        private void ModeZone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

        }

        private void MapControlManager_ZoneInformationUpdated(object sender, EventArgs e)
        {
            RefreshMarkers();
        }

        partial void RefreshInProgressButton_Click(UIButton sender)
        {
            var actionSheetAlert = AlertControllerHelper.CreateAlertOnClickRefreshButton(MapViewModelBase.Seekios.SeekiosName);
            PresentViewController(actionSheetAlert, true, null);
        }

        #endregion
    }
}