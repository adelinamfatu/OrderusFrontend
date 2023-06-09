using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class ClientAddressPage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public ClientViewModel client { get; set; }

        public ClientAddressPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            client = new ClientViewModel(globalService.Client);
            this.BindingContext = client;
        }

        private void GetClientAccountUpdatesFromUI(object sender, EventArgs e)
        {
            var Client = new ClientDTO()
            {
                Email = client.Email,
                Phone = client.Phone,
                City = client.City,
                Street = client.Street,
                StreetNumber = client.StreetNumber,
                Building = client.Building,
                Staircase = client.Staircase,
                ApartmentNumber = client.ApartmentNumber,
                Floor = client.Floor
            };
            SendClientAccountUpdates(Client);
        }

        private async void SendClientAccountUpdates(ClientDTO Client)
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.DetailsURL + RestResources.UpdateURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(Client);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.DataUpdateSuccess);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DataUpdateFail);
                }
            }
        }
    }
}