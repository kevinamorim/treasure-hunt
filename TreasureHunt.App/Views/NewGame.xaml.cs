using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TreasureHunt.App.Views
{
    public sealed partial class NewGame : Page
    {
        public NewGame()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs args) 
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Start":
                    startBtn.Visibility = Visibility.Collapsed;
                    goBackBtn.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = true;
                    await CreateGame();
                    break;
                case "Go Back":
                    App.RootFrame.Navigate(typeof(MainPage));
                    break;
                default:
                    break;
            }
        }

        private async Task<Game> CreateGame()
        {
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                Username = UsernameInput.Text,
                Difficulty = DifficultyInput.SelectedIndex,
                Finished = false,
                StartedAt = DateTime.Now
            };

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 1 };

                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    BasicGeoposition basicGeoPosition = new BasicGeoposition();
                    basicGeoPosition.Latitude = pos.Coordinate.Latitude;
                    basicGeoPosition.Longitude = pos.Coordinate.Longitude;

                    BasicGeoposition destinationBasicGeoPosition = PickRandomLocation(basicGeoPosition, 1, 50);
                    Geopoint destination = new Geopoint(destinationBasicGeoPosition);

                    game.OriginalLatitude = myLocation.Position.Latitude;
                    game.OriginalLongitude = myLocation.Position.Longitude;

                    game.TargetLatitude = destination.Position.Latitude;
                    game.TargetLongitude = destination.Position.Longitude;

                    break;
                case GeolocationAccessStatus.Denied:
                    break;
                case GeolocationAccessStatus.Unspecified:
                    break;
                default:
                    break;
            }

            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(game);

                // Send a POST
                Task task = Task.Run(async () =>
                {
                    var request = new StringContent(content, Encoding.UTF8, "application/json");
                    await client.PostAsync(App.BaseUri + "games", request);
                });
                task.Wait();
            }

            App.RootFrame.Navigate(typeof(MainPage));

            return game;
        }

        private BasicGeoposition PickRandomLocation(BasicGeoposition center, int minRadius, int maxRadius)
        {
            Random random = new Random();
            int radius = random.Next(minRadius, maxRadius);

            List<BasicGeoposition> positions = CalculateCircle(center, radius);

            int index = random.Next(positions.Count);

            return positions[index];
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

    }
}
