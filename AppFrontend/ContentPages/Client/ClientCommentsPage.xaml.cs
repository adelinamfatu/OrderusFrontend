using Acr.UserDialogs;
using App.DTO;
using AppFrontend.CustomControls;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using RatingBarControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientCommentsPage : ContentPage
    {
        public GlobalService globalService { get; set; }

        ObservableCollection<OrderDTO> orders = new ObservableCollection<OrderDTO>();
        public ObservableCollection<OrderDTO> Orders { get { return orders; } }

        public ClientCommentsPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            RetrieveOrders();
            this.BindingContext = this;
        }

        private async void RetrieveOrders()
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.OrdersURL + RestResources.CommentsURL + globalService.Client.Email;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<OrderDTO> ordersJSON = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    BuildOrderList(ordersJSON);
                }
            }
        }

        private void BuildOrderList(List<OrderDTO> ordersJSON)
        {
            foreach (var order in ordersJSON)
            {
                orders.Add(order);
            }
        }

        private void ReviewOrder(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            var order = (OrderDTO)button.BindingContext;
            Navigation.PushAsync(new ClientReviewPage(order));
            Orders.Remove(order);
        }
    }
}