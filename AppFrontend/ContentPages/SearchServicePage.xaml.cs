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
                Name = "Reparatii"
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
            string url = "http://192.168.2.39:9000/api/services/categories/" + categoryID;

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
    }
}