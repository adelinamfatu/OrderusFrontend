using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.ViewModels;
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
        ObservableCollection<EmployeeViewModel> employees = new ObservableCollection<EmployeeViewModel>();
        public ObservableCollection<EmployeeViewModel> Employees { get { return employees; } }

        List<ServiceDTO> services = new List<ServiceDTO>();

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
                GetDataForUI();
            }
        }

        private async void GetDataForUI()
        {
            await GetCompanyServices();
            RetrieveEmployees();
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
                if(employee.IsConfirmed == false)
                {
                    Employees.Add(new EmployeeViewModel(employee, services));
                }
                else
                {
                    Employees.Add(new EmployeeViewModel(employee));
                }
            }
        }

        private async Task GetCompanyServices()
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
            var service = (ServiceDTO)button.BindingContext;
            var viewCell = FindParentViewCell(button);

            if (viewCell != null)
            {
                EmployeeViewModel employee = (EmployeeViewModel)viewCell.BindingContext;
                employee.Services.Remove(service);
            }
        }

        private ViewCell FindParentViewCell(Element element)
        {
            Element parent = element.Parent;
            while (parent != null)
            {
                if (parent is ViewCell viewCell)
                {
                    return viewCell;
                }
                parent = parent.Parent;
            }
            return null;
        }

        private void ConfirmEmployeeAndServices(object sender, EventArgs e)
        {
            var result = UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.EmployeeConfirmationTitle,
                Message = ToastDisplayResources.EmployeeConfirmation,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });
            if(result.Result == true)
            {
                Button button = (Button)sender;
                EmployeeViewModel employee = (EmployeeViewModel)button.BindingContext;
                employee.IsConfirmed = true;
                EmployeeDTO employeeToSend = new EmployeeDTO() { Email = employee.Email };
                employeeToSend.Services = new List<ServiceDTO>();
                foreach (var service in employee.Services)
                {
                    employeeToSend.Services.Add(service);
                }
                SendEmployeeData(employeeToSend);
            }
        }

        private async void SendEmployeeData(EmployeeDTO employee)
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.UpdateURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("company_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);
            }
        }
    }
}