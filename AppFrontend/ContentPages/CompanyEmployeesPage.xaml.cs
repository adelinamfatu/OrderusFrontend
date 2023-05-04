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
    public partial class CompanyEmployeesPage : ContentPage
    {
        ObservableCollection<EmployeeDTO> employees = new ObservableCollection<EmployeeDTO>();
        public ObservableCollection<EmployeeDTO> Employees { get { return employees; } }

        ObservableCollection<ServiceDTO> services = new ObservableCollection<ServiceDTO>();
        public ObservableCollection<ServiceDTO> Services { get { return services; } }

        public GlobalService globalService { get; set; }

        public CompanyEmployeesPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
            this.BindingContext = this;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Company")
            {
                RetrieveEmployees();
                GetCompanyServices();
            }
        }

        private async void RetrieveEmployees()
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.CompanyURL + globalService.Company.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("company_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var employeeJSON = JsonConvert.DeserializeObject<List<EmployeeDTO>>(json);
                    BuildEmployeeList(employeeJSON);
                }
            }
        }

        private void BuildEmployeeList(IEnumerable<EmployeeDTO> employeeJSON)
        {
            foreach (var employee in employeeJSON)
            {
                employees.Add(employee);
            }
        }

        private async void GetCompanyServices()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.CompanyDetailsURL + globalService.Company.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("company_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var servicesJSON = JsonConvert.DeserializeObject<List<CompanyServiceOptionDTO>>(json);
                    foreach (var service in servicesJSON)
                    {
                        services.Add(service.Service);
                    }
                }
            }
        }

        private void RemoveServiceFromList(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Grid grid = (Grid)button.Parent;
            Label label = (Label)grid.Children[0];
            string service = label.Text;
            services.Remove(services.FirstOrDefault(s => s.Name == service));
        }
    }
}