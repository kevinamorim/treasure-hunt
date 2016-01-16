using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TreasureHunt.App.Views
{
    public sealed partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs args)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Login":

                    DoLogin();

                    App.RootFrame.Navigate(typeof(MainPage));

                    break;
                default:
                    break;
            }
        }

        private void DoLogin()
        {
            User user = new User() { Username = UsernameTextBox.Text };

            if ((bool)LocalhostCheckBox.IsChecked)
            {
                App.BaseUri = new Uri("http://localhost:56856/api/");
            }

            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(user);

                HttpResponseMessage response = null;
                string userIdString = "";

                Task registerUser = Task.Run(async () =>
                {
                    var request = new StringContent(content, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(App.BaseUri + "users", request);
                    userIdString = await response.Content.ReadAsStringAsync();
                });

                LoginStackPanel.Visibility = Visibility.Collapsed;
                loadingProgressRing.IsActive = true;
                LoginStackPanel.Visibility = Visibility.Visible;

                registerUser.Wait();

                Guid userId = new Guid(userIdString.Trim('"'));

                SaveUserId(userId);

            }
        }

        private void SaveUserId(Guid userId)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[App.USER_ID] = userId;
        }
    }
}
