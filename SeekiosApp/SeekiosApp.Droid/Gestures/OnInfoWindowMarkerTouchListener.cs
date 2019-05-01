using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Views;
using Android.Gms.Maps.Model;
using SeekiosApp.Droid.CustomComponents;
using Android.App;
using SeekiosApp.ViewModel;
using System.Globalization;
using SeekiosApp.Droid.View;

namespace SeekiosApp.Droid.Gestures
{
    public class OnInfoWindowMarkerTouchListener : Activity, Android.Views.View.IOnTouchListener
    {
        #region ===== Attributs ===================================================================

        private Context _context;
        private Marker _marker;
        private PopupMarkerActionType _markerType;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public OnInfoWindowMarkerTouchListener(Context context, Marker marker, PopupMarkerActionType markerType)
        {
            _context = context;
            _marker = marker;
            _markerType = markerType;
        }

        #endregion

        #region ===== Implementation Interface ====================================================

        public bool OnTouch(Android.Views.View v, MotionEvent e)
        {
            if (e.Action == MotionEventActions.Cancel)
            {
                if (App.DeviceIsConnectedToInternet && _markerType == PopupMarkerActionType.MarkerSeekios)
                {
                    if (!IsSeekiosOnDemand())
                    {
                        _marker.ShowInfoWindow();
                        AskOnDemandRequest();
                    }
                }
                else if (App.DeviceIsConnectedToInternet && _markerType == PopupMarkerActionType.MarkerSeekiosMore)
                {
                    if (App.DeviceIsConnectedToInternet)
                    {
                        if (_context is MapActivity)
                        {
                            ((MapActivity)_context).DisplayModeSelection();
                        }
                        else if (_context is MapHistoricActivity)
                        {
                            ((MapHistoricActivity)_context).DisplayModeSelection();
                        }
                    }
                }
            }
            else if (e.Action == MotionEventActions.Up)
            {
                if (_markerType == PopupMarkerActionType.MarkerZone)
                {
                    try
                    {
                        var markerToDelete = new Messages.MarkerZoneDeleteMessage(_marker);
                        GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(markerToDelete);
                    }
                    catch (Exception) { }
                }
                return true;
            }
            return false;
        }

        private bool IsSeekiosOnDemand()
        {
            var isSeekiosOnDemand = false;
            var seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
            if (seekiosOnDemand != null) isSeekiosOnDemand = seekiosOnDemand.Seekios.IsOnDemand;
            return isSeekiosOnDemand;
        }

        private async void AskOnDemandRequest()
        {
            if (_context is MapActivity)
            {
                await App.Locator.BaseMap.RefreshSeekiosPosition();
            }
        }

        #endregion

        public enum PopupMarkerActionType
        {
            MarkerSeekios,
            MarkerSeekiosMore,
            MarkerZone
        }
    }
}