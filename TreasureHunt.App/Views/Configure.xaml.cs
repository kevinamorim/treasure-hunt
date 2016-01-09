using System;
using System.Collections.Generic;
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
            if (App.BaseUri == new Uri("http://localhost:56856/api/"))
            {
                networkBtn_localhost.IsChecked = true;
            }
            else
            {
                networkBtn_azure.IsChecked = true;
            }

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("accuracy"))
            {
                accuracySlider.Value = (double) ApplicationData.Current.LocalSettings.Values["accuracy"];
            }

            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("updateInterval"))
            {
                updateIntervalSlider.Value = (double) ApplicationData.Current.LocalSettings.Values["updateInterval"];
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
                case "Go Back":
                    App.RootFrame.Navigate(typeof(MainPage));
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
            localSettings.Values["accuracy"] = accuracy;
        }

        private void SaveUpdateInterval()
        {
            double updateInterval = updateIntervalSlider.Value;

            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["updateInterval"] = updateInterval;
        }
    }
}
