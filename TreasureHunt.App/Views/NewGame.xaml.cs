using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App.Views
{
    public sealed partial class NewGame : Page
    {

        public NewGame()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            GameNameInput.KeyUp += TextBox_KeyUp;
        }

        private async void Button_Click(object sender, RoutedEventArgs args) 
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Start":

                    GameNameInput.IsEnabled = false;
                    DifficultyInput.IsEnabled = false;

                    startBtn.Visibility = Visibility.Collapsed;
                    progressRing.IsActive = true;

                    progressRing.Visibility = Visibility.Visible;
                    loadingTextBlock.Visibility = Visibility.Visible;
                    Game game = await CreateGame();

                    App.RootFrame.Navigate(typeof(Play), game.Id);

                    break;
                default:
                    break;
            }
        }

        private async Task<Game> CreateGame()
        {

            List<int> distanceInMeters = new List<int>(new int[] { 100, 500, 1000 });

            var localSettings = ApplicationData.Current.LocalSettings;
            uint accuracyInMeters = localSettings.Values.ContainsKey("desiredAccuracy") ? Convert.ToUInt32(localSettings.Values["desiredAccuracy"]) : 50;

            var game = new Game()
            {
                Id = Guid.NewGuid(),
                Name = GameNameInput.Text,
                Difficulty = DifficultyInput.SelectedIndex,
                Finished = false,
                StartedAt = DateTime.Now
            };

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = accuracyInMeters };

                    loadingTextBlock.Text = "Getting your location...";
                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    BasicGeoposition basicGeoPosition = new BasicGeoposition();
                    basicGeoPosition.Latitude = pos.Coordinate.Latitude;
                    basicGeoPosition.Longitude = pos.Coordinate.Longitude;

                    BasicGeoposition destinationBasicGeoPosition = GeoHelper.PickRandomLocation(basicGeoPosition, 
                        (int)(distanceInMeters[game.Difficulty] + accuracyInMeters), 
                        (int)(distanceInMeters[game.Difficulty] + accuracyInMeters));

                    Geopoint destination = new Geopoint(destinationBasicGeoPosition);

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

                Task registerGame = Task.Run(async () =>
                {
                    var request = new StringContent(content, Encoding.UTF8, "application/json");
                    await client.PostAsync(App.BaseUri + "games", request);
                });

                loadingTextBlock.Text = "Creating game...";
                registerGame.Wait();
            }

            return game;
        }

        private void TextBox_KeyUp(object sender, KeyRoutedEventArgs args)
        {
            if (args.Key == Windows.System.VirtualKey.Enter)
            {
                Windows.UI.ViewManagement.InputPane.GetForCurrentView().TryHide();
            }
        }

    }
}
