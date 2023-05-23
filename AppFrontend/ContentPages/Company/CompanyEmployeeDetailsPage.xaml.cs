using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Company
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyEmployeeDetailsPage : ContentPage
    {
        public EmployeeDTO Employee { get; set; }

        public CompanyEmployeeDetailsPage(string employeeEmail)
        {
            InitializeComponent();
            GetEmployeeDetails(employeeEmail);
        }

        private async void GetEmployeeDetails(string employeeEmail)
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + employeeEmail;

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
                    Employee = JsonConvert.DeserializeObject<EmployeeDTO>(json);
                    this.BindingContext = this;
                }
            }
        }
    }
}