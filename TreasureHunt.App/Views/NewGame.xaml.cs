﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            uint accuracyInMeters = localSettings.Values.ContainsKey(App.DESIRED_ACCURACY) ? Convert.ToUInt32(localSettings.Values[App.DESIRED_ACCURACY]) : 50;

            var game = new Game()
            {
                Name = GameNameInput.Text,
                Difficulty = DifficultyInput.SelectedIndex,
                Finished = false,
            };

            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = accuracyInMeters };

                    loadingTextBlock.Text = "Getting your location (+/-" + accuracyInMeters + " m) ...";
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
                    loadingTextBlock.Text = "Location access denied! :(";
                    break;
                case GeolocationAccessStatus.Unspecified:
                    loadingTextBlock.Text = "Unspecified error with location :/";
                    break;
                default:
                    break;
            }

            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(game);
                var gameId = "";

                Task registerGame = Task.Run(async () =>
                {
                    var request = new StringContent(content, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(App.BaseUri + "games", request);
                    gameId = await response.Content.ReadAsStringAsync();
                });

                loadingTextBlock.Text = "Creating game...";
                registerGame.Wait();

                game.Id = new Guid(gameId.Trim('"'));

                JoinGame(game.Id);
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

        private void JoinGame(Guid gameId)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            Guid userId = new Guid(localSettings.Values[App.USER_ID].ToString());

            var jsonString = JsonConvert.SerializeObject(new
            {
                GameId = gameId,
                UserId = userId
            });

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var str = "";
            using (var client = new HttpClient())
            {
                Task task = Task.Run(async () =>
                {
                    var response = await client.PutAsync(App.BaseUri + "games/JoinGame", content);
                    str = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("JoinGame(): " + str);
                });
                task.Wait();
            }
        }

    }
}
