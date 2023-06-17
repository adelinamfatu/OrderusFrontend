using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
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
    public partial class ClientOffersPage : ContentPage
    {
        public GlobalService globalService { get; set; }

        ObservableCollection<OfferDTO> offers = new ObservableCollection<OfferDTO>();
        public ObservableCollection<OfferDTO> Offers { get { return offers; } }

        public ClientOffersPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            RetrieveOffers();
            this.BindingContext = this;
        }

        private async void RetrieveOffers()
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.OfferURL + globalService.Client.Email;

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
                    List<OfferDTO> offersJSON = JsonConvert.DeserializeObject<List<OfferDTO>>(json);
                    BuildOfferList(offersJSON);
                }
            }
        }

        private void BuildOfferList(List<OfferDTO> offersJSON)
        {
            foreach(var offer in offersJSON)
            {
                offers.Add(offer);
            }
        }
    }
}