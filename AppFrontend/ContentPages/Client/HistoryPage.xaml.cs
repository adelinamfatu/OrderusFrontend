using App.DTO;
using AppFrontend.ContentPages.Client;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        ObservableCollection<OrderDTO> orders = new ObservableCollection<OrderDTO>();
        public ObservableCollection<OrderDTO> Orders { get { return orders; } }

        public GlobalService globalService { get; set; }

        public HistoryPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Client")
            {
                GetDataForUI();
                this.BindingContext = this;
            }
        }

        private async void GetDataForUI()
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.HistoryURL + globalService.Client.Email;

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
                    var ordersJSON = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    BuildHistory(ordersJSON);
                }
            }
        }

        private void BuildHistory(IEnumerable<OrderDTO> ordersJSON)
        {
            foreach (var order in ordersJSON)
            {
                this.Orders.Add(order);
            }
        }

        private void OpenReceipt(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            OrderDTO order = (OrderDTO)button.BindingContext;
            Navigation.PushAsync(new OrderReceiptPage(order));
        }
    }
}