using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
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

        protected override void OnNavigatedTo(NavigationEventArgs args)
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

            Debug.WriteLine("Clicked.");

            User user = new User() { Username = "kevin" };

            GameView gameView = args.ClickedItem as GameView;

            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(user);
                var request = new StringContent(content, Encoding.UTF8, "application/json");
                Task task = Task.Run(async () =>
                {
                    response = await client.PutAsync(App.BaseUri + "games/JoinGame/" + gameView.Id, request);
                });
                task.Wait();
            }

            Debug.WriteLine("Status: " + response.StatusCode.ToString() + " Response: " + response.Content.ToString());

        }
    }
}
