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
            globalService = DependencyService.Get<GlobalService>();
            this.BindingContext = this;
            /*companies.Add(new CompanyDTO
            {
                ID = 1,
                Name = "Reparatot",
                Logo = "reparatot.ro/wp-content/uploads/2015/03/head-logo.png"
            });
            companies.Add(new CompanyDTO
            {
                ID = 2,
                Name = "Reparathor",
                Logo = "reparathor.ro/wp-content/uploads/2021/01/cropped-hammer-3-1.png"
            });*/
            RetrieveCompaniesByService(serviceId);
        }

        private async void RetrieveCompaniesByService(int serviceId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.ServicesURL + serviceId;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", globalService.token);
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