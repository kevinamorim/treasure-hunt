﻿using TreasureHunt.App.Models;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Core;
using Windows.UI;
using Windows.System.Display;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace TreasureHunt.App.Views
{
    public sealed partial class Play : Page
    {

        private uint DesiredAccuracyInMeters
        {
            get; set;
        }

        private uint UpdateIntervalInSeconds
        {
            get; set;
        }

        private Geolocator MyGeolocator
        {
            get; set;
        }

        private Geopoint TargetPosition
        {
            get; set;
        }

        private Game Game
        {
            get; set;
        }

        private DisplayRequest KeepScreenOnRequest
        {
            get; set;
        }

        public Play()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            KeepScreenOnRequest = new DisplayRequest();
            KeepScreenOnRequest.RequestActive();

            Guid gameId = (Guid)e.Parameter;

            GetGame(gameId);

            LoadSettings();

            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    InitializeGeolocator();

                    TargetPosition = new Geopoint(new BasicGeoposition() { Latitude = Game.TargetLatitude, Longitude = Game.TargetLongitude });

                    progressBar.Maximum = UpdateIntervalInSeconds;
                    progressBar.Value = UpdateIntervalInSeconds;

                    DispatcherTimer dispatcherTimer = new DispatcherTimer();
                    dispatcherTimer.Tick += updateProgress;
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    dispatcherTimer.Start();

                    break;

                case GeolocationAccessStatus.Denied:
                    break;

                case GeolocationAccessStatus.Unspecified:
                    break;

                default:
                    break;
            }


        }

        private void GetGame(Guid gameId)
        {
            using (var client = new HttpClient())
            {
                Task getGame = Task.Run(async () =>
                {
                    var response = await client.GetAsync(App.BaseUri + "games/" + gameId.ToString());
                    var str = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("GetGame(): " + str);
                    Game = await response.Content.ReadAsAsync<Game>();
                });

                getGame.Wait();
            }
        }

        private ICollection<User> GetPlayers()
        {

            List<User> users = null;

            using (var client = new HttpClient())
            {
                Task getPlayers = Task.Run(async () =>
                {
                    var response = await client.GetAsync(App.BaseUri + "games/GetPlayers/" + Game.Id.ToString());
                    var str = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine("GetPlayers(): " + str);

                    users = await response.Content.ReadAsAsync<List<User>>();
                });

                 getPlayers.Wait();

            }

            return users;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Finish":

                    GameOver();

                    break;
                default:
                    break;
            }
        }

        private void InitializeGeolocator()
        {
            MyGeolocator = new Geolocator { DesiredAccuracyInMeters = DesiredAccuracyInMeters };

            MyGeolocator.PositionChanged += PositionChanged;
            MyGeolocator.ReportInterval = UpdateIntervalInSeconds * 1000;
        }

        async void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Value = UpdateIntervalInSeconds;
                progressBar.Visibility = Visibility.Visible;
                updateProgressRing.Visibility = Visibility.Collapsed;
                updateProgressRing.IsActive = false;
                Geopoint currentPosition = new Geopoint(new BasicGeoposition() { Latitude = args.Position.Coordinate.Latitude, Longitude = args.Position.Coordinate.Longitude });
                SetCurrentPosition(currentPosition);

                SetPlayersList();
            });
        }

        private void SetPlayersList()
        {
            var players = GetPlayers();

            if (players != null)
            {
                listView.ItemsSource = players.ToList();
            }
        }

        private void SetCurrentPosition(Geopoint currentPosition)
        {

            var currentBasicGeoposition = new BasicGeoposition() { Latitude = currentPosition.Position.Latitude, Longitude = currentPosition.Position.Longitude };
            var targetBasicGeoposition = new BasicGeoposition() { Latitude = TargetPosition.Position.Latitude, Longitude = TargetPosition.Position.Longitude };

            var distanceInMeters = (GeoHelper.CalculateDistance(currentBasicGeoposition, targetBasicGeoposition) * 1000) - DesiredAccuracyInMeters;

            if (distanceInMeters <= DesiredAccuracyInMeters)
            {
                GameOver();
            }

            DistanceTextBlock.Text = Math.Round(distanceInMeters, 0) + " meters";

        }

        private void GameOver()
        {
            KeepScreenOnRequest.RequestRelease();
            Game.FinishedAt = DateTime.Now;
            App.RootFrame.Navigate(typeof(GameOver), Game);
        }

        private async void updateProgress(object sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TimeSpan span = (DateTime.Now - Game.StartedAt);
                TimeTextBlock.Text = span.Days + "d " + span.Hours + "h " + span.Minutes + "m " + span.Seconds + "s";

                if (progressBar.Value <= 0)
                {
                    progressBar.Visibility = Visibility.Collapsed;
                    updateProgressRing.Visibility = Visibility.Visible;
                    updateProgressRing.IsActive = true;
                }
                else
                {
                    progressBar.Value -= 1;
                }
            });
        }

        private void LoadSettings()
        {

            uint defaultDesiredAccuracy = 50;
            uint defaultUpdateInterval = 10;

            var localSettings = ApplicationData.Current.LocalSettings;

            var desiredAccuracy = localSettings.Values[App.DESIRED_ACCURACY];
            DesiredAccuracyInMeters = (desiredAccuracy == null) ? defaultDesiredAccuracy : Convert.ToUInt32(desiredAccuracy);

            var updateInterval = localSettings.Values["updateInterval"];
            UpdateIntervalInSeconds = (updateInterval == null) ? defaultUpdateInterval : Convert.ToUInt32(updateInterval);

        }

        private void Page_Loaded(object sender, RoutedEventArgs args)
        {
            LoadingStackPanel.Visibility = Visibility.Collapsed;
            LoadingProgressRing.IsActive = false;

            GameTitleTextBlock.Text = Game.Name;
            DistanceTextBlock.Text = 0 + " meters";

            GameStackPanel.Visibility = Visibility.Visible;
        }
    }
}
