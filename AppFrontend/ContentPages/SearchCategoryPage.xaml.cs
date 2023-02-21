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
    public partial class SearchCategoryPage : ContentPage
    {
        ObservableCollection<CategoryDTO> categories = new ObservableCollection<CategoryDTO>();
        public ObservableCollection<CategoryDTO> Categories { get { return categories; } }

        public SearchCategoryPage()
        {
            InitializeComponent();
            categoriesListView.SelectedItem = null;
            this.BindingContext = this;
            RetrieveCategories();
        }

        private async void RetrieveCategories()
        {
            string url = "http://192.168.2.39:9000/api/services/categories";

            using (HttpClient client = new HttpClient())
            {
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

        private async void RetrieveServicesByCategory(int categoryID)
        {
            string url = "http://192.168.2.39:9000/api/services/categories/" + categoryID;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<ServiceDTO> servicesJSON = JsonConvert.DeserializeObject<List<ServiceDTO>>(json);
                    
                }
            }
        }

        private void OpenServiceSearchPage(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            /*StackLayout listViewItem = (StackLayout)button.Parent;
            Label label = (Label)listViewItem.Children[0];
            var text = label.Text;
            CategoryDTO category = Categories.FirstOrDefault(c => c.Name == text);
            RetrieveServicesByCategory(category.ID);*/
        }
    }
}