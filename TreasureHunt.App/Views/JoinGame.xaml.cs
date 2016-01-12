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
                List<GameView> gamesViews = new List<GameView>();

                string[] difficulties = { "Normal", "Hard", "Insane" };

                foreach (var item in games)
                {
                    GameView gameView = new GameView()
                    {
                        Id = item.Id,
                        Name = item.Username,
                        Difficulty = difficulties[item.Difficulty]
                    };

                    string finished = "";

                    if (item.Finished)
                    {
                        TimeSpan timespan = item.FinishedAt - item.StartedAt;

                        finished = timespan.Days + "d " + timespan.Hours + "h " + timespan.Minutes + "m " + timespan.Seconds + "s ";
                    }
                    else
                    {
                        finished = "Not finished";
                    }

                    gameView.Finished = finished;

                    gamesViews.Add(gameView);

                }


                listView.ItemsSource = gamesViews;
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
