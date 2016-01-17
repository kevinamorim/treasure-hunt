using System;
using Windows.Storage;
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

            var localSettings = ApplicationData.Current.LocalSettings;

            if (App.BaseUri == new Uri("http://localhost:56856/api/"))
            {
                networkBtn_localhost.IsChecked = true;
            }
            else
            {
                networkBtn_azure.IsChecked = true;
            }

            if (localSettings.Values.ContainsKey(App.DESIRED_ACCURACY))
            {
                accuracySlider.Value = (double) localSettings.Values[App.DESIRED_ACCURACY];
            }

            if (localSettings.Values.ContainsKey("updateInterval"))
            {
                updateIntervalSlider.Value = (double)localSettings.Values["updateInterval"];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Save":

                    SaveAccuracy();
                    SaveUpdateInterval();

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
                case "Logout":

                    Logout();
                    App.RootFrame.Navigate(typeof(Login));

                    break;
                default:
                    break;
            }
        }

        private void SliderChanged(object sender, RoutedEventArgs e)
        {
            switch ((sender as Slider).Name)
            {
                case "accuracySlider":
                    accuracyTextBlock.Text = "Accuracy: " + accuracySlider.Value + " m";
                    break;
                case "updateIntervalSlider":
                    updateIntervalTextBlock.Text = "Update Interval: " + updateIntervalSlider.Value + " sec";
                    break;
                default:
                    break;
            }
        }

        private void SaveAccuracy()
        {
            double accuracy = accuracySlider.Value;

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[App.DESIRED_ACCURACY] = accuracy;
        }

        private void SaveUpdateInterval()
        {
            double updateInterval = updateIntervalSlider.Value;

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["updateInterval"] = updateInterval;
        }

        private void Logout()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(App.USER_ID);
        }

    }
}
