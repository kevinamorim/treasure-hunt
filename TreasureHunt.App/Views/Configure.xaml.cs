using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App.Views
{
    public sealed partial class Configure : Page
    {
        public Configure()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (App.BaseUri == new Uri("http://localhost:56856/api/"))
            {
                networkBtn_localhost.IsChecked = true;
            }
            else
            {
                networkBtn_azure.IsChecked = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Save":

                    if ((bool) networkBtn_localhost.IsChecked)
                    {
                        App.BaseUri = new Uri("http://localhost:56856/api/");
                    }
                    else if ((bool) networkBtn_azure.IsChecked)
                    {
                        App.BaseUri = new Uri("http://treasure-hunt.azurewebsites.net/api/");
                    }
                    else
                    {
                        App.BaseUri = new Uri("http://localhost:56856/api/");
                    }

                    App.RootFrame.Navigate(typeof(MainPage));

                    break;
                case "Go Back":
                    App.RootFrame.Navigate(typeof(MainPage));
                    break;
                default:
                    break;
            }
        }
    }
}
