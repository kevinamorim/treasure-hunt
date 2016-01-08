using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
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

        private void Button_Click(object sender, RoutedEventArgs args) 
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Start":
                    CreateGame();
                    break;
                case "Go Back":
                    App.RootFrame.Navigate(typeof(MainPage));
                    break;
                default:
                    break;
            }
        }

        private void CreateGame()
        {
            var game = new Game()
            {
                Id = Guid.NewGuid(),
                Username = UsernameInput.Text,
                Difficulty = DifficultyInput.SelectedIndex,
                Finished = false,
                StartedAt = DateTime.Now
            };

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
        }
    }
}
