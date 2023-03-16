using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
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

        public SearchCompanyPage(int serviceId)
        {
            InitializeComponent();
            this.BindingContext = this;
            companies.Add(new CompanyDTO
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
            });
            //RetrieveCompaniesByService(serviceId);
        }

        private async void RetrieveCompaniesByService(int serviceId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + serviceId;

            using (HttpClient client = new HttpClient())
            {
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
                    Logo = company.Logo
                });
            }
        }
    }
}