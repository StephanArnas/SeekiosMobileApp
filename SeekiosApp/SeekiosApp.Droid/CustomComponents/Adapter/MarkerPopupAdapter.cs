using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using SeekiosApp.Droid.Gestures;
using System.Linq;
using SeekiosApp.ViewModel;
using System.Collections.Generic;

namespace SeekiosApp.Droid.CustomComponents
{
    public class MarkerPopupAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        #region ===== Attributs ===================================================================

        private LayoutInflater _inflater = null;
        private Context _context = null;

        #endregion

        #region ===== Constructeur(s) =============================================================

        public MarkerPopupAdapter(Context context, LayoutInflater inflater)
        {
            _inflater = inflater;
            _context = context;
        }

        #endregion

        #region ===== Méthodes Public =============================================================

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            // marqueur Zone
            if (marker.Snippet == "MarkerZone")
            {
                var customMarkerPopup = _inflater.Inflate(Resource.Drawable.PopupMarkerZone, null);
                var seekiosDeleteSvgImageView = customMarkerPopup.FindViewById<XamSvg.SvgImageView>(Resource.Id.customMarker_delete);
                seekiosDeleteSvgImageView.Clickable = true;
                var deleteButtonListener = new OnInfoWindowMarkerTouchListener(_context, marker, OnInfoWindowMarkerTouchListener.PopupMarkerActionType.MarkerZone);
                seekiosDeleteSvgImageView.SetOnTouchListener(deleteButtonListener);
                MapWrapperLayout.Instance.SetMarkerWithInfoWindow(marker, customMarkerPopup);

                return customMarkerPopup;
            }

            // marqueur Tracking
            if (marker.Snippet == "MarkerRoute")
            {
                var customMarkerPopup = _inflater.Inflate(Resource.Drawable.PopupMarkerRoute, null);
                var dateTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarkerRoute_date);
                dateTextView.Text = marker.Title;
                MapWrapperLayout.Instance.SetMarkerWithInfoWindow(marker, customMarkerPopup);

                return customMarkerPopup;
            }

            // marqueur locationHistory
            if (marker.Snippet == "LocationHistory")
            {
                var customMarkerPopup = _inflater.Inflate(Resource.Drawable.PopupMarkerRoute, null);
                var dateTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarkerRoute_date);
                dateTextView.Text = marker.Title;
                MapWrapperLayout.Instance.SetMarkerWithInfoWindow(marker, customMarkerPopup);

                return customMarkerPopup;
            }
            //InTime marker
            if (marker.Snippet == "InTimeMarker")
            {
                var customMarkerPopup = _inflater.Inflate(Resource.Drawable.PopupMarkerRoute, null);
                var dateTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarkerRoute_date);
                dateTextView.Text = marker.Title;
                MapWrapperLayout.Instance.SetMarkerWithInfoWindow(marker, customMarkerPopup);

                return customMarkerPopup;
            }
            // marqueur Seekios
            else
            {
                var isAlert = MapViewModelBase.Mode != null && MapViewModelBase.Mode.StatusDefinition_idstatusDefinition != 1;
                var isSeekiosOnDemand = false;
                var seekiosOnDemand = App.Locator.Map.LsSeekiosOnDemand.FirstOrDefault(x => x.Seekios.Idseekios == App.Locator.DetailSeekios.SeekiosSelected.Idseekios);
                if (seekiosOnDemand != null) isSeekiosOnDemand = seekiosOnDemand.Seekios.IsOnDemand;

                var customMarkerPopup = ((LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService)).Inflate(Resource.Drawable.PopupMarkerSeekios, null);
                var seekiosRefreshSvgImageView = customMarkerPopup.FindViewById<XamSvg.SvgImageView>(Resource.Id.customMarker_refresh);
                var seekiosRefreshTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_refreshText);
                var seekiosMoreSvgImageView = customMarkerPopup.FindViewById<XamSvg.SvgImageView>(Resource.Id.customMarker_share);
                var seekiosMoreTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_shareText);
                var seekiosNameTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_seekiosName);
                var seekiosLastPositionTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_lastPosition);

                if (!isAlert)
                {
                    seekiosRefreshSvgImageView.SetSvg(_context, Resource.Drawable.RefreshSeekios, !App.DeviceIsConnectedToInternet || isSeekiosOnDemand || App.Locator.Historic.IsHistoryActivated ? "62da73=888888" : string.Empty);
                    seekiosRefreshTextView.SetTextColor(!App.DeviceIsConnectedToInternet || isSeekiosOnDemand || App.Locator.Historic.IsHistoryActivated ? _context.Resources.GetColor(Resource.Color.textColorTitle) : _context.Resources.GetColor(Resource.Color.primary));
                    seekiosMoreSvgImageView.SetSvg(_context, Resource.Drawable.More, !App.DeviceIsConnectedToInternet ? "62da73=888888" : string.Empty);
                    seekiosMoreTextView.SetTextColor(!App.DeviceIsConnectedToInternet ? _context.Resources.GetColor(Resource.Color.textColorTitle) : _context.Resources.GetColor(Resource.Color.primary));
                }
                else
                {
                    seekiosRefreshSvgImageView.SetSvg(_context, Resource.Drawable.RefreshSeekios, !App.DeviceIsConnectedToInternet || isSeekiosOnDemand ? "62da73=888888" : "62da73=da2e2e");
                    seekiosRefreshTextView.SetTextColor(!App.DeviceIsConnectedToInternet || isSeekiosOnDemand ? _context.Resources.GetColor(Resource.Color.textColorTitle) : _context.Resources.GetColor(Resource.Color.color_red));
                    seekiosMoreSvgImageView.SetSvg(_context, Resource.Drawable.More, !App.DeviceIsConnectedToInternet ? "62da73=888888" : "62da73=da2e2e");
                    seekiosMoreTextView.SetTextColor(!App.DeviceIsConnectedToInternet ? _context.Resources.GetColor(Resource.Color.textColorTitle) : _context.Resources.GetColor(Resource.Color.color_red));
                }

                seekiosRefreshSvgImageView.Clickable = !App.Locator.Historic.IsHistoryActivated;
                seekiosMoreSvgImageView.Clickable = true;

                var infoButtonListener = new OnInfoWindowMarkerTouchListener(_context, marker, OnInfoWindowMarkerTouchListener.PopupMarkerActionType.MarkerSeekios);
                seekiosRefreshSvgImageView.SetOnTouchListener(infoButtonListener);
                var moreSeekiosButtonListener = new OnInfoWindowMarkerTouchListener(_context, marker, OnInfoWindowMarkerTouchListener.PopupMarkerActionType.MarkerSeekiosMore);
                seekiosMoreSvgImageView.SetOnTouchListener(moreSeekiosButtonListener);

                MapWrapperLayout.Instance.SetMarkerWithInfoWindow(marker, customMarkerPopup);
                seekiosNameTextView.Text = marker.Title;
                seekiosLastPositionTextView.Text = marker.Snippet;

                return customMarkerPopup;
            }
        }

        #endregion
    }
}