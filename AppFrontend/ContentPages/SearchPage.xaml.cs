using App.DTO;
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
    public partial class SearchPage : ContentPage
    {
        ObservableCollection<CategoryDTO> categories = new ObservableCollection<CategoryDTO>();
        public ObservableCollection<CategoryDTO> Categories { get { return categories; } }

        public SearchPage()
        {
            InitializeComponent();
            categories.Add(new CategoryDTO { Name = "Hehe" });
            categories.Add(new CategoryDTO { Name = "Hehehe" });
            //retrieveCategories();
            categoriesListView.ItemsSource = categories;
        }

        private async void retrieveCategories()
        {
            string url = "http://172.18.96.1:9000/api/services/categories";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<CategoryDTO> categoriesJSON = JsonConvert.DeserializeObject<List<CategoryDTO>>(json);
                    buildCategorySearch(categoriesJSON);
                }
            }
        }
        
        private void buildCategorySearch(List<CategoryDTO> categoriesJSON)
        {
            foreach(var category in categoriesJSON)
            {
                categories.Add(new CategoryDTO { Name = category.Name });
            }
        }
    }
}