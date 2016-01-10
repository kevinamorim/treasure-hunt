using TreasureHunt.App.Views;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TreasureHunt.App
{
    public sealed partial class MainPage : Page
    {
        const string apiUrl = @"localhost:51397/api/";

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            switch ((sender as Button).Content.ToString())
            {
                case "New Game":
                    App.RootFrame.Navigate(typeof(NewGame));
                    break;
                case "View Games":
                    App.RootFrame.Navigate(typeof(ViewGames));
                    break;
                case "Configure":
                    App.RootFrame.Navigate(typeof(Configure));
                    break;
                default:
                    break;
            }
        }


    }
}
