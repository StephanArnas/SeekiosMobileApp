using System;
using System.Collections.Generic;
using Android.Gms.Maps.Model;
using System.Globalization;

namespace SeekiosApp.Droid.Helper
{
    public class AreaHelper
    {
        private static double EARTH_RADIUS = 6371000;// meters

        public static double CalculateAreaOfGPSPolygonOnEarthInSquareMeters(List<LatLng> locations)
        {
            return CalculateAreaOfGPSPolygonOnSphereInSquareMeters(locations, EARTH_RADIUS);
        }

        private static double CalculateAreaOfGPSPolygonOnSphereInSquareMeters(List<LatLng> locations, double radius)
        {
            if (locations.Count < 3)
            {
                return 0;
            }

            var diameter = radius * 2;
            var circumference = diameter * Math.PI;
            var listY = new List<Double>();
            var listX = new List<Double>();
            var listArea = new List<Double>();
            // calculate segment x and y in degrees for each point
            var latitudeRef = locations[0].Latitude;
            var longitudeRef = locations[0].Longitude;
            for (int i = 1; i < locations.Count; i++)
            {
                var latitude = locations[i].Latitude;
                var longitude = locations[i].Longitude;
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

        private static Double calculateAreaInSquareMeters(double x1, double x2, double y1, double y2)
        {
            return (y1 * x2 - x1 * y2) / 2;
        }

        private static double calculateYSegment(double latitudeRef, double latitude, double circumference)
        {
            return (latitude - latitudeRef) * circumference / 360.0;
        }

        private static double calculateXSegment(double longitudeRef, double longitude, double latitude,
             double circumference)
        {
            return (longitude - longitudeRef) * circumference * Math.Cos(convertToRadians(latitude)) / 360.0;
        }

        private static double convertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static string SerializeArea(double area)
        {
            if (area < 1000) return string.Format("{0:0} m²", area);
            if (area < 10000) return string.Format("{0:0.00} km²", (area / 1000.0));
            if (area < 1000000) return string.Format("{0:0.00} ha", (area / 10000.0));
            return (area / 10000.0).ToString("G2", CultureInfo.InvariantCulture) + " ha";
        }
    }
}