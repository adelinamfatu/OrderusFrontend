using App.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchCategoryPage : ContentPage, INotifyPropertyChanged
    {
        ObservableCollection<CategoryDTO> categories = new ObservableCollection<CategoryDTO>();
        public ObservableCollection<CategoryDTO> Categories { get { return categories; } }
        public SearchCategoryPage()
        {
            InitializeComponent();
            categoriesListView.SelectedItem = null;
            this.BindingContext = this;
            /*categories.Add(new CategoryDTO
            {
                ID = 1,
                Name = "Casa"
            });
            categories.Add(new CategoryDTO
            {
                ID = 2,
                Name = "Evenimente"
            });
            categories.Add(new CategoryDTO
            {
                ID = 3,
                Name = "Meditatii"
            });
            categories.Add(new CategoryDTO
            {
                ID = 4,
                Name = "Afaceri"
            });*/
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