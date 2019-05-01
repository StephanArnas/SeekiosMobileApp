using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Views;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using Android.Graphics;

namespace SeekiosApp.Droid.CustomComponents
{
    public class MapWrapperLayout : RelativeLayout
    {
        #region ===== Attributs ===================================================================

        private GoogleMap _map;
        private Marker _marker;
        private Android.Views.View _infoWindow;

        private const int _heightSeekios = 130;
        private const int _heightMarker = 35;

        #endregion

        #region ===== Propri�t�s =================================================================

        public static MapWrapperLayout Instance { get; set; }

        #endregion

        #region ===== Constructeur(s) =============================================================

        public MapWrapperLayout(Context context) : base (context) {}

        public MapWrapperLayout(Context context, IAttributeSet attrs) : base(context, attrs) {}

        public MapWrapperLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) {}

        #endregion

        #region ===== M�thodes Public =============================================================

        /// <summary>
        /// Doit �tre appel� avant de configurer les �v�nements de gesture
        /// </summary>
        public void Init(GoogleMap map)
        {
            this._map = map;
            Instance = this;
        }


        /// <summary>
        /// Doit �tre appel� par InfoWindowAdapter.GetInfoContents ou InfoWindowAdapter.GetInfoWindow
        /// </summary>
        public void SetMarkerWithInfoWindow(Marker marker, Android.Views.View infoWindow)
        {
            this._marker = marker;
            this._infoWindow = infoWindow;
        }

        #endregion

        #region ===== �v�nements ==================================================================

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            bool ret = false;

            // v�rifi que le InfoWindows est affich�
            if (_marker != null && _marker.IsInfoWindowShown && _map != null && _infoWindow != null)
            {
                // obtient la position du marker � l'�cran
                Point point = _map.Projection.ToScreenLocation(_marker.Position);

                // ajustement du clique pour un pin Seekios
                MotionEvent seekiosRefresh = MotionEvent.ObtainNoHistory(e);
                seekiosRefresh.OffsetLocation(
                    -point.X + (_infoWindow.Width / 2),
                    -point.Y + _infoWindow.Height + _heightSeekios);   // 130 = la hauteur du pin seekios

                // ajustement du clique pour un pin Marker
                MotionEvent copyEvMarker = MotionEvent.ObtainNoHistory(e);
                copyEvMarker.OffsetLocation(
                    -point.X + (_infoWindow.Width / 2),
                    -point.Y + _infoWindow.Height + _heightMarker);    // 35 = la hauteur du pin marker

                // dispatch le MotionEvent ajust� vers le InfoWindow
                ret = _infoWindow.DispatchTouchEvent(seekiosRefresh);
                ret = _infoWindow.DispatchTouchEvent(copyEvMarker);
            }

            return ret || base.DispatchTouchEvent(e);
        }

        #endregion
    }
}