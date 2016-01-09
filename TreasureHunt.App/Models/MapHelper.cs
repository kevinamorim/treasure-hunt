using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Xaml.Controls.Maps;

namespace TreasureHunt.App.Models
{
    public class MapHelper
    {

        public static MapPolygon DrawCircle(List<BasicGeoposition> points, Color fillColor, Color strokeColor, int strokeThickness = 2, byte alpha = 80)
        {
            fillColor.A = alpha;
            strokeColor.A = alpha;

            var circle = new MapPolygon
            {
                StrokeThickness = strokeThickness,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                Path = new Geopath(points)
            };

            return circle;
        }

    }
}
