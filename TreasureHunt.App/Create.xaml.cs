using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Geolocation;
using System.Diagnostics;
using System.Text;
using System.Net.Http;

namespace TreasureHunt.App
{
    public sealed partial class Create : Page
    {
        public Create()
        {
            InitializeComponent();
        }

        private  void Button_Click(object sender, RoutedEventArgs e)
        {
            //double[] position = GetPosition();
            double[] position = { 0.0, 0 };

            //var accessStatus = await Geolocator.RequestAccessAsync();
            //switch (accessStatus)
            //{
            //    case GeolocationAccessStatus.Allowed:

            //        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 1 };
                    
            //        Geoposition pos = await geolocator.GetGeopositionAsync();

            //        break;
            //    case GeolocationAccessStatus.Denied:
            //        break;
            //    case GeolocationAccessStatus.Unspecified:
            //        break;
            //    default:
            //        break;
            //}

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email.Text,
                Latitude = position[0],
                Longitude = position[1]
            };

            var content = JsonConvert.SerializeObject(user);
            Debug.WriteLine("Content: " + content);

            using (var client = new HttpClient())
            {

                // Send a POST
                Task task = Task.Run(async () =>
                {
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new ("application/json"));

                    var request = new StringContent(content.ToString(), Encoding.UTF8, "application/json");
     
                    Debug.WriteLine("Sent (Headers): " + request.Headers.ToString());

                    var response = await client.PostAsync(new Uri(App.BaseUri + "users"), request);
                    Debug.WriteLine("Response: " + response.StatusCode);

                });
                task.Wait();
            }

            App.RootFrame.Navigate(typeof(MainPage));
        }

        private double[] GetPosition()
        {
            Geolocator geo = new Geolocator();
            double lat = 0, longt = 0;

            Task getPosition = Task.Run(async () =>
            {
                try
                {
                    Geoposition pos = await geo.GetGeopositionAsync();
                    lat = pos.Coordinate.Point.Position.Latitude;
                    longt = pos.Coordinate.Point.Position.Longitude;
                }
                catch (Exception exp)
                {
                    Debug.WriteLine(exp);
                }

            });
            getPosition.Wait();

            double[] position= { lat, longt };

            return position;
        }
    }
}
