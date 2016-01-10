using TreasureHunt.App.Models;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Core;
using Windows.UI;

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

        private MapIcon UserLocationIcon
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

        public Play()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Game game = (Game)e.Parameter;

            LoadSettings();

            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    InitializeGeolocator();

                    Geopoint currentPosition = new Geopoint(new BasicGeoposition() { Latitude = game.OriginalLatitude, Longitude = game.OriginalLongitude });
                    TargetPosition = new Geopoint(new BasicGeoposition() { Latitude = game.TargetLatitude, Longitude = game.TargetLongitude });

                    DrawMap(currentPosition, 15);
                    DrawUserPositionIcon(currentPosition);
                    DrawTargetCircle(TargetPosition);

                    DoneLoadingMap();

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Exit":
                    App.RootFrame.Navigate(typeof(MainPage));
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
                Geopoint currentPosition = new Geopoint(new BasicGeoposition() { Latitude = args.Position.Coordinate.Latitude, Longitude = args.Position.Coordinate.Longitude });
                SetCurrentPosition(currentPosition);
            });
        }

        private void SetCurrentPosition(Geopoint currentPosition)
        {
            MainMap.Center = currentPosition;
            UserLocationIcon.Location = currentPosition;

            var currentBasicGeoposition = new BasicGeoposition() { Latitude = currentPosition.Position.Latitude, Longitude = currentPosition.Position.Longitude };
            var targetBasicGeoposition = new BasicGeoposition() { Latitude = TargetPosition.Position.Latitude, Longitude = TargetPosition.Position.Longitude };

            var distanceInMeters = GeoHelper.CalculateDistance(currentBasicGeoposition, targetBasicGeoposition) * 1000;

            if (distanceInMeters <= DesiredAccuracyInMeters)
            {
                GameOver();
            }

            distanceTextBlock.Text = "Distance: " + distanceInMeters + " meters ";
        }

        private void DrawMap(Geopoint center, int zoomLevel, bool landmarksVisible = false)
        {
            MainMap.Center = center;
            MainMap.ZoomLevel = zoomLevel;
            MainMap.LandmarksVisible = landmarksVisible;
        }

        private void DrawUserPositionIcon(Geopoint currentPosition)
        {
            UserLocationIcon = new MapIcon();
            UserLocationIcon.Location = currentPosition;
            UserLocationIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5);
            UserLocationIcon.ZIndex = 0;

            MainMap.MapElements.Add(UserLocationIcon);
        }

        private void DrawTargetCircle(Geopoint targetPosition)
        {
            MapPolygon targetPositionCircle = MapHelper.DrawCircle(GeoHelper.CalculateCircle(targetPosition.Position, (int)DesiredAccuracyInMeters), Colors.Chocolate, Colors.BurlyWood);
            MainMap.MapElements.Add(targetPositionCircle);
        }

        private void DoneLoadingMap()
        {
            progressRing.IsActive = false;
            progressRing.Visibility = Visibility.Collapsed;
            progressMessage.Visibility = Visibility.Collapsed;
            distanceTextBlock.Visibility = Visibility.Visible;
            MainMap.Visibility = Visibility.Visible;
            nextUpdateTextBlock.Visibility = Visibility.Visible;
            progressBar.Visibility = Visibility.Visible;
        }

        private void GameOver()
        {
            gameOverTextBlock.Visibility = Visibility.Visible;
        }

        private async void updateProgress(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Value -= 1;
            });
        }

        private void LoadSettings()
        {

            uint defaultDesiredAccuracy = 50;
            uint defaultUpdateInterval = 10;

            var localSettings = ApplicationData.Current.LocalSettings;

            var desiredAccuracy = localSettings.Values["desiredAccuracy"];
            DesiredAccuracyInMeters = (desiredAccuracy == null) ? defaultDesiredAccuracy : Convert.ToUInt32(desiredAccuracy);

            var updateInterval = localSettings.Values["updateInterval"];
            UpdateIntervalInSeconds = (updateInterval == null) ? defaultUpdateInterval : Convert.ToUInt32(updateInterval);

        }
    }
}
