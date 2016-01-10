using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TreasureHunt.App.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace TreasureHunt.App.Views
{
    public sealed partial class ViewGames : Page
    {
        public ViewGames()
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
                    response = await client.GetStringAsync(App.BaseUri + "games");
                });
                task.Wait();
                listView.ItemsSource = JsonConvert.DeserializeObject<List<Game>>(response);
            }
        }

    }
}
