using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI;
using Windows.UI.Core;
using TreasureHunt.App.Models;
using Windows.Storage;

namespace TreasureHunt.App.Views
{
    public sealed partial class TestMap : Page
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

        private MapPolygon UserLocationCircle
        {
            get; set;
        }

        public TestMap()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            LoadSettings();
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:

                    Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = DesiredAccuracyInMeters };

                    Geoposition pos = await geolocator.GetGeopositionAsync();
                    Geopoint myLocation = pos.Coordinate.Point;

                    geolocator.PositionChanged += PositionChanged;
                    geolocator.ReportInterval = UpdateIntervalInSeconds * 1000;

                    MainMap.Center = myLocation;
                    MainMap.ZoomLevel = 15;
                    MainMap.LandmarksVisible = true;

                    UserLocationIcon = new MapIcon();
                    UserLocationIcon.Location = myLocation;
                    UserLocationIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5);
                    
                    UserLocationIcon.ZIndex = 0;
         
                    MainMap.MapElements.Add(UserLocationIcon);

                    BasicGeoposition basicGeoPosition = new BasicGeoposition();
                    basicGeoPosition.Latitude = pos.Coordinate.Latitude;
                    basicGeoPosition.Longitude = pos.Coordinate.Longitude;

                    UserLocationCircle = MapHelper.DrawCircle(GeoHelper.CalculateCircle(myLocation.Position, 50), Colors.Blue, Colors.Red);
                    MainMap.MapElements.Add(UserLocationCircle);

                    MapIcon destinationIcon = new MapIcon();
                    BasicGeoposition destinationBasicGeoPosition = GeoHelper.PickRandomLocation(basicGeoPosition, 1, 50);
                    Geopoint destination = new Geopoint(destinationBasicGeoPosition);
                    destinationIcon.Location = destination;
                    destinationIcon.ZIndex = 0;

                   // MainMap.MapElements.Add(destinationIcon);

                    progressRing.IsActive = false;
                    progressRing.Visibility = Visibility.Collapsed;
                    progressMessage.Visibility = Visibility.Collapsed;

                    MainMap.Visibility = Visibility.Visible;

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

        private async void updateProgress(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Value -= 1;
            });
        }

        async void PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
            {
                progressBar.Value = UpdateIntervalInSeconds;
                UserLocationIcon.Location = args.Position.Coordinate.Point;
                UserLocationCircle = MapHelper.DrawCircle(GeoHelper.CalculateCircle(args.Position.Coordinate.Point.Position, 50), Colors.Blue, Colors.Red);
            });
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
