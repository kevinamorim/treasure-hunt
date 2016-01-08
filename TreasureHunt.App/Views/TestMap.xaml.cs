using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI;
using System.Collections.Generic;

namespace TreasureHunt.App.Views
{
    public sealed partial class TestMap : Page
    {
        public TestMap()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 1 };

                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    MainMap.Center = myLocation;
                    MainMap.ZoomLevel = 20;
                    MainMap.LandmarksVisible = true;
                    MapIcon mapIcon1 = new MapIcon();
                    mapIcon1.Location = myLocation;
                    mapIcon1.ZIndex = 0;

                    MainMap.MapElements.Add(mapIcon1);

                    BasicGeoposition basicGeoPosition = new BasicGeoposition();
                    basicGeoPosition.Latitude = pos.Coordinate.Latitude;
                    basicGeoPosition.Longitude = pos.Coordinate.Longitude;

                    MainMap.MapElements.Add(DrawCircle(basicGeoPosition, 50));

                    MapIcon destinationIcon = new MapIcon();
                    BasicGeoposition destinationBasicGeoPosition = PickRandomLocation(basicGeoPosition, 1, 50);
                    Geopoint destination = new Geopoint(destinationBasicGeoPosition);
                    destinationIcon.Location = destination;
                    destinationIcon.ZIndex = 0;

                    MainMap.MapElements.Add(destinationIcon);

                    break;
                case GeolocationAccessStatus.Denied:
                    break;
                case GeolocationAccessStatus.Unspecified:
                    break;
                default:
                    break;
            }

        }

        private BasicGeoposition PickRandomLocation(BasicGeoposition center, int minRadius, int maxRadius)
        {
            Random random = new Random();
            int radius = random.Next(minRadius, maxRadius);

            List<BasicGeoposition> positions = CalculateCircle(center, radius);

            int index = random.Next(positions.Count);

            return positions[index];
        }

        private MapPolygon DrawCircle(BasicGeoposition center, int radius)
        {
            Color fillColor = Colors.Peru;
            Color strokeColor = Colors.Red;

            fillColor.A = 80;
            strokeColor.A = 80;

            var circle = new MapPolygon
            {
                StrokeThickness = 2,
                FillColor = fillColor,
                StrokeColor = strokeColor,
                Path = new Geopath(CalculateCircle(center, radius))
            };

            return circle;
        }

        private List<BasicGeoposition> CalculateCircle(BasicGeoposition center, int radius)
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

        private double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Go Back":
                    App.RootFrame.Navigate(typeof(MainPage));
                    break;
                default:
                    break;
            }
        }


    }
}
