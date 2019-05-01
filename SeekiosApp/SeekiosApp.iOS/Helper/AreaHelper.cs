using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using SeekiosApp.iOS.Views.MapAnnotations;
using CoreLocation;
using MapKit;

namespace SeekiosApp.IOS.Helper
{
	public class AreaHelper
	{
        #region ===== Constructor =================================================================

        private static double EARTH_RADIUS = 6371000;// meters

        #endregion

        #region ===== Public Methodes =============================================================

        /// <summary>
        /// Calculates the area of GPSP olygon on earth in square meters.
        /// </summary>
        /// <returns>The area of GPSP olygon on earth in square meters.</returns>
        /// <param name="allPins">All pins.</param>
        public static double CalculateAreaOfGPSPolygonOnEarthInSquareMeters(List<MKAnnotation> locations)
		{
			return CalculateAreaOfGPSPolygonOnSphereInSquareMeters(locations, EARTH_RADIUS);
        }

        /// <summary>
        /// Serializes the area.
        /// </summary>
        /// <returns>The area.</returns>
        /// <param name="area">Area.</param>
        public static string SerializeArea(double area)
        {
            if (area < 1000) return string.Format("{0:0} m²", area);
            if (area < 10000) return string.Format("{0:0.00} km²", (area / 1000.0));
            if (area < 1000000) return string.Format("{0:0.00} ha", (area / 10000.0));
            return (area / 10000.0).ToString("G2", CultureInfo.InvariantCulture) + " ha";// string.Format("{0:### ### ##0} ha", (area / 10000.0));
        }

        #endregion

        #region ===== Public Methodes =============================================================

        /// <summary>
        /// Calculates the area of GPSP olygon on sphere in square meters.
        /// </summary>
        /// <returns>The area of GPSP olygon on sphere in square meters.</returns>
        /// <param name="locations">Locations.</param>
        /// <param name="radius">Radius.</param>
        private static double CalculateAreaOfGPSPolygonOnSphereInSquareMeters(List<MKAnnotation> locations, double radius)
		{
			if (locations.Count < 3)
			{
				return 0;
			}

			var diameter = radius * 2;
			var circumference = diameter * Math.PI;
			var listY = new List<double>();
			var listX = new List<double>();
			var listArea = new List<double>();
			// calculate segment x and y in degrees for each point
			var latitudeRef = locations[0].Coordinate.Latitude;
			var longitudeRef = locations[0].Coordinate.Longitude;
			for (int i = 1; i < locations.Count; i++)
			{
				var latitude = locations[i].Coordinate.Latitude;
				var longitude = locations[i].Coordinate.Longitude;
				listY.Add(calculateYSegment(latitudeRef, latitude, circumference));
				//Log.d(LOG_TAG, String.format("Y %s: %s", listY.size() - 1, listY.get(listY.size() - 1)));
				listX.Add(calculateXSegment(longitudeRef, longitude, latitude, circumference));
				//Log.d(LOG_TAG, String.format("X %s: %s", listX.size() - 1, listX.get(listX.size() - 1)));
			}

			// calculate areas for each triangle segment
			for (int i = 1; i < listX.Count; i++)
			{
				var x1 = listX[i - 1];
				var y1 = listY[i - 1];
				var x2 = listX[i];
				var y2 = listY[i];
				listArea.Add(calculateAreaInSquareMeters(x1, x2, y1, y2));
				//Log.d(LOG_TAG, String.format("area %s: %s", listArea.size() - 1, listArea.get(listArea.size() - 1)));
			}

			// sum areas of all triangle segments
			double areasSum = 0;
			foreach (var area in listArea)
			{
				areasSum = areasSum + area;
			}

			// get abolute value of area, it can't be negative
			return Math.Abs(areasSum);// Math.sqrt(areasSum * areasSum);
		}

		/// <summary>
		/// Calculates the area in square meters.
		/// </summary>
		/// <returns>The area in square meters.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="y2">The second y value.</param>
		private static double calculateAreaInSquareMeters(double x1, double x2, double y1, double y2)
		{
			return (y1 * x2 - x1 * y2) / 2;
		}

		/// <summary>
		/// Calculates the YS egment.
		/// </summary>
		/// <returns>The YS egment.</returns>
		/// <param name="latitudeRef">Latitude reference.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="circumference">Circumference.</param>
		private static double calculateYSegment(double latitudeRef, double latitude, double circumference)
		{
			return (latitude - latitudeRef) * circumference / 360.0;
		}

		/// <summary>
		/// Calculates the XS egment.
		/// </summary>
		/// <returns>The XS egment.</returns>
		/// <param name="longitudeRef">Longitude reference.</param>
		/// <param name="longitude">Longitude.</param>
		/// <param name="latitude">Latitude.</param>
		/// <param name="circumference">Circumference.</param>
		private static double calculateXSegment(double longitudeRef, double longitude, double latitude, double circumference)
		{
			return (longitude - longitudeRef) * circumference * Math.Cos(convertToRadians(latitude)) / 360.0;
		}

		/// <summary>
		/// Converts to radians.
		/// </summary>
		/// <returns>The to radians.</returns>
		/// <param name="angle">Angle.</param>
		private static double convertToRadians(double angle)
		{
			return (Math.PI / 180) * angle;
		}

        #endregion
    }
}