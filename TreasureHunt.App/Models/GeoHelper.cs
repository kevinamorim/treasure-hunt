using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace TreasureHunt.App.Models
{
    public class GeoHelper
    {
        public static BasicGeoposition PickRandomLocation(BasicGeoposition center, int minRadius, int maxRadius)
        {
            Random random = new Random();
            int radius = random.Next(minRadius, maxRadius);

            List<BasicGeoposition> positions = GeoHelper.CalculateCircle(center, radius);

            int index = random.Next(positions.Count);

            return positions[index];
        }

        public static List<BasicGeoposition> CalculateCircle(BasicGeoposition center, int radius)
        {

            List<BasicGeoposition> geoPositions = new List<BasicGeoposition>();

            double earthRadius = 6371000D;
            double circunference = 2D * Math.PI * earthRadius;

            for (int i = 0; i <= 360; i++)
            {
                double bearing = ToRad(i);
                double circunferenceLatitudeCorrected = 2D * Math.PI * Math.Cos(ToRad(center.Latitude)) * earthRadius;
                double lat1 = circunference / 360D * center.Latitude;
                double lon1 = circunferenceLatitudeCorrected / 360D * center.Longitude;
                double lat2 = lat1 + Math.Sin(bearing) * radius;
                double lon2 = lon1 + Math.Cos(bearing) * radius;

                BasicGeoposition newBasicPosition = new BasicGeoposition();
                newBasicPosition.Latitude = lat2 / (circunference / 360D);
                newBasicPosition.Longitude = lon2 / (circunferenceLatitudeCorrected / 360D);
                geoPositions.Add(newBasicPosition);
            }

            return geoPositions;

        }

        public static double CalculateDistance(BasicGeoposition source, BasicGeoposition target)
        {
            var r = 6371;
            var dLat = ToRad((target.Latitude - source.Latitude));
            var dLon = ToRad((target.Longitude - source.Longitude));

            var lat1 = ToRad(source.Latitude);
            var lat2 = ToRad(target.Latitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = r * c;

            return d;
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }


}
