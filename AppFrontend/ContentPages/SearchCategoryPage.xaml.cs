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
        private GlobalService globalService { get; set; }

        public SearchCategoryPage()
        {
            InitializeComponent();
            //globalService = DependencyService.Get<GlobalService>();
            categoriesListView.SelectedItem = null;
            this.BindingContext = this;
            RetrieveCategories();
        }

        private async void RetrieveCategories()
        {
            string url = RestResources.ConnectionURL + RestResources.CategoriesURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("orderus_token").Result;
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

        private void OpenServiceSearchPage(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;
            var parentGrid = (Grid)button.Parent;
            var label = (Label)parentGrid.Children.FirstOrDefault(l => l is Label);
            var text = label.Text;
            CategoryDTO category = Categories.FirstOrDefault(c => c.Name == text);
            Navigation.PushAsync(new SearchServicePage(category.ID));
        }
    }
}