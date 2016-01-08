using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App
{
    public sealed partial class View : Page
    {
        public View()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            using (var client = new HttpClient())
            {
                var response = "";
                Task task = Task.Run(async () =>
                {
                    response = await client.GetStringAsync(App.BaseUri + "users");
                });
                task.Wait();
                listView.ItemsSource = JsonConvert.DeserializeObject<List<User>>(response);

                //App.ActiveUser = JsonConvert.DeserializeObject<List<User>>(response)[0];
            }
        }

        private void Button_Click(Object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Content.ToString())
            {
                case "Go Back":
                    App.RootFrame.Navigate(typeof (MainPage), true);
                    break;
                default:
                    break;
            }
        }
    }
}
