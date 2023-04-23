using App.DTO;
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
    public partial class SearchCategoryPage : ContentPage, INotifyPropertyChanged
    {
        ObservableCollection<CategoryDTO> categories = new ObservableCollection<CategoryDTO>();
        public ObservableCollection<CategoryDTO> Categories { get { return categories; } }

        ObservableCollection<string> companies = new ObservableCollection<string>();
        public ObservableCollection<string> Companies { get { return companies; } }

        private List<CompanyDTO> companiesJSON { get; set; }

        public Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm 
            { get; } = (text, values) => values.Where(x => x.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                                                .OrderBy(x => x).ToList();

        public SearchCategoryPage()
        {
            InitializeComponent();
            categoriesListView.SelectedItem = null;
            this.BindingContext = this;
            RetrieveCategories();
            RetrieveCompanies();
        }

        private async void RetrieveCategories()
        {
            string url = RestResources.ConnectionURL + RestResources.CategoriesURL;

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
                    List<CategoryDTO> categoriesJSON = JsonConvert.DeserializeObject<List<CategoryDTO>>(json);
                    BuildCategorySearch(categoriesJSON);
                }
            }
        }

        private async void RetrieveCompanies()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL;

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
                    companiesJSON = JsonConvert.DeserializeObject<List<CompanyDTO>>(json);
                    BuildCompanySearch(companiesJSON);
                }
            }
        }

        private void BuildCategorySearch(List<CategoryDTO> categoriesJSON)
        {
            foreach(var category in categoriesJSON)
            {
                categories.Add(new CategoryDTO 
                { 
                    ID = category.ID,
                    Name = category.Name
                });
            }
        }

        private void BuildCompanySearch(List<CompanyDTO> companiesJSON)
        {
            foreach (var company in companiesJSON)
            {
                companies.Add(company.Name);
            }
        }

        private void OpenServiceSearchPage(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;
            var parentGrid = (Grid)button.Parent;
            var label = (Label)parentGrid.Children.FirstOrDefault(l => l is Label);
            var text = label.Text;
            CategoryDTO category = Categories.FirstOrDefault(c => c.Name == text);
            Navigation.PushAsync(new SearchServicePage(category.ID));
        }

        private void OpenCompanyPage(object sender, EventArgs e)
        {
            if(searchComboBox.SelectedItem != null)
            {
                var companyName = searchComboBox.SelectedItem;
                var company = companiesJSON.Where(comp => comp.Name == (string)companyName).FirstOrDefault();
                Navigation.PushAsync(new CompanyPage(company));
            }
        }
    }
}