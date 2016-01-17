using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App.Views
{
    public sealed partial class JoinGame : Page
    {
        public JoinGame()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (var client = new HttpClient())
            {
                var response = "";
                Task task = Task.Run(async () =>
                {
                    response = await client.GetStringAsync(App.BaseUri + "games/" + false);
                });
                task.Wait();

                List<Game> games = JsonConvert.DeserializeObject<List<Game>>(response);
                List<JoinGameViewModel> joinGameViewModels = new List<JoinGameViewModel>();

                string[] difficulties = { "Normal", "Hard", "Insane" };

                foreach (var item in games)
                {
                    JoinGameViewModel gameView = new JoinGameViewModel()
                    {
                        Id = item.Id,
                        GameName = item.Name,
                        Difficulty = difficulties[item.Difficulty]
                    };

                    joinGameViewModels.Add(gameView);

                }


                listView.ItemsSource = joinGameViewModels;
            }
        }

        private void List_ItemClick(object sender, ItemClickEventArgs args)
        {

            JoinGameViewModel gameView = args.ClickedItem as JoinGameViewModel;

            var localSettings = ApplicationData.Current.LocalSettings;
            string userId = localSettings.Values[App.USER_ID].ToString();

            HttpResponseMessage response = new HttpResponseMessage();

            var jsonString = JsonConvert.SerializeObject(new
            {
                GameId = gameView.Id,
                UserId = userId
            });

            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            var str = "";
            using (var client = new HttpClient())
            {
                Task task = Task.Run(async () =>
                {
                    response = await client.PutAsync(App.BaseUri + "games/JoinGame", content);
                    str = await response.Content.ReadAsStringAsync();
                });
                task.Wait();
            }

            Debug.WriteLine("ID: " + str);

            App.RootFrame.Navigate(typeof(Play), gameView.Id);

        }
    }
}
