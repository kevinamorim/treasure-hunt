using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
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
                        Username = item.Username,
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
    }
}
