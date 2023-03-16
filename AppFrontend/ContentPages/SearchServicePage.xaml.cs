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
    public partial class SearchServicePage : ContentPage
    {
        ObservableCollection<ServiceDTO> services = new ObservableCollection<ServiceDTO>();
        public ObservableCollection<ServiceDTO> Services { get { return services; } }

        public SearchServicePage() { }

        public SearchServicePage(int categoryId)
        {
            InitializeComponent();
            this.BindingContext = this;
            services.Add(new ServiceDTO
            {
                ID = 1,
                Name = "Tamplarie"
            });
            services.Add(new ServiceDTO
            {
                ID = 2,
                Name = "Dulgherie"
            });
            services.Add(new ServiceDTO
            {
                ID = 3,
                Name = "Montare"
            });
            services.Add(new ServiceDTO
            {
                ID = 4,
                Name = "Reparare"
            });
            services.Add(new ServiceDTO
            {
                ID = 5,
                Name = "Curatare"
            });
            //RetrieveServicesByCategory(categoryId);
        }

        private void BuildServiceSearch(List<ServiceDTO> servicesJSON)
        {
            foreach (var service in servicesJSON)
            {
                services.Add(new ServiceDTO
                {
                    ID = service.ID,
                    Name = service.Name
                });
            }
        }

        private async void RetrieveServicesByCategory(int categoryID)
        {
            string url = RestResources.ConnectionURL + RestResources.CategoriesURL + categoryID;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<ServiceDTO> servicesJSON = JsonConvert.DeserializeObject<List<ServiceDTO>>(json);
                    BuildServiceSearch(servicesJSON);
                }
            }
        }

        private void OpenCompanySearchPage(object sender, EventArgs e)
        {
            ImageButton button = (ImageButton)sender;
            var parentGrid = (Grid)button.Parent;
            var label = (Label)parentGrid.Children.FirstOrDefault(l => l is Label);
            var text = label.Text;
            ServiceDTO service = Services.FirstOrDefault(c => c.Name == text);
            Navigation.PushAsync(new SearchCompanyPage(service.ID));
        }
    }
}