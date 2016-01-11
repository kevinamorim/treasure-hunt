using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App.Views
{
    public sealed partial class GameOver : Page
    {
        public GameOver()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Game game = (Game) e.Parameter;

            TimeSpan timeSpan = (game.FinishedAt - game.StartedAt);

            string timespanString = timeSpan.Days + "d " + timeSpan.Hours + "h " + timeSpan.Minutes + "m " + timeSpan.Seconds + "s";

            timeTextBlock.Text += " " + timespanString;

            string[] difficulties = { "Normal", "Hard", "Insane" };
            difficultyTextBlock.Text += " " + difficulties[game.Difficulty];

            using (var client = new HttpClient())
            {
                var content = JsonConvert.SerializeObject(game);

                // Send a POST
                Task task = Task.Run(async () =>
                {
                    var request = new StringContent(content, Encoding.UTF8, "application/json");
                    await client.PutAsync(App.BaseUri + "games", request);
                });
                task.Wait();
            }



        }

    }
}
