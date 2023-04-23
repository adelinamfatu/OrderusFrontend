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

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchCompanyPage : ContentPage
    {
        ObservableCollection<CompanyDTO> companies = new ObservableCollection<CompanyDTO>();
        public ObservableCollection<CompanyDTO> Companies { get { return companies; } }
        private GlobalService globalService { get; set; }

        public SearchCompanyPage(int serviceId)
        {
            InitializeComponent();
            //globalService = DependencyService.Get<GlobalService>();
            this.BindingContext = this;
            RetrieveCompaniesByService(serviceId);
        }

        private async void RetrieveCompaniesByService(int serviceId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.ServicesURL + serviceId;

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
                    List<CompanyDTO> companiesJSON = JsonConvert.DeserializeObject<List<CompanyDTO>>(json);
                    BuildCompanySearch(companiesJSON);
                }
            }
        }

        private void BuildCompanySearch(List<CompanyDTO> companiesJSON)
        {
            foreach (var company in companiesJSON)
            {
                companies.Add(new CompanyDTO
                {
                    ID = company.ID,
                    Name = company.Name,
                    City = company.City,
                    Street = company.Street,
                    StreetNumber = company.StreetNumber,
                    Building = company.Building,
                    Staircase = company.Staircase,
                    ApartmentNumber = company.ApartmentNumber,
                    Floor = company.Floor,
                    Logo = company.Logo,
                    Site = company.Site
                });
            }
        }

        private void OpenCompanyPage(object sender, SelectedItemChangedEventArgs e)
        {
            //var company = (CompanyDTO)e.SelectedItem;
            var company = (CompanyDTO)companiesListView.SelectedItem;
            Navigation.PushAsync(new CompanyPage(company));
        }
    }
}