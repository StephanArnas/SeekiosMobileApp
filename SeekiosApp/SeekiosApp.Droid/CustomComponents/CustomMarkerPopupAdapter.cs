using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using GalaSoft.MvvmLight.Helpers;

namespace SeekiosApp.Droid.CustomComponents
{
    public class CustomMarkerPopupAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private LayoutInflater _layoutInflater = null;

        public CustomMarkerPopupAdapter(LayoutInflater inflater)
        {
            _layoutInflater = inflater;
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            if (!marker.Title.StartsWith("p"))
            {
                var customMarkerPopup = _layoutInflater.Inflate(Resource.Drawable.PopupMarkerSeekios, null);
                var seekiosRefreshSvgImageView = customMarkerPopup.FindViewById<XamSvg.SvgImageView>(Resource.Id.customMarker_refresh);
                var seekiosNameTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_seekiosName);
                var seekiosLastPositionTextView = customMarkerPopup.FindViewById<TextView>(Resource.Id.customMarker_lastPosition);

                //seekiosRefreshSvgImageView.Clickable = true;
                //seekiosRefreshSvgImageView.Click += (o, e) =>
                //{
                //    App.Locator.ModeDefinition.RefreshSeekiosPosition(marker.Title);
                //};
                seekiosNameTextView.Text = marker.Title;
                seekiosLastPositionTextView.Text = marker.Snippet;

                return customMarkerPopup;
            }

            return null;
        }
    }
}