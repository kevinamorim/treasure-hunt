using System;
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

        }

    }
}
