using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Employee
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeUnconfirmedOrderDetailsPage : ContentPage
    {
        public OrderViewModel Order { get; set; }

        public GlobalService globalService { get; set; }

        public EmployeeUnconfirmedOrderDetailsPage(OrderDTO order)
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            this.Order = new OrderViewModel(order);
            PrepareDataForUIAsync();
            this.BindingContext = this;
        }

        private async void PrepareDataForUIAsync()
        {
            await GetOrderDetails();
            GetMaterials();
        }

        private async Task GetOrderDetails()
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.OrdersURL + Order.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Order.Details = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
        }

        private async void GetMaterials()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.MaterialsURL + globalService.Employee.CompanyID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var materialsJSON = JsonConvert.DeserializeObject<List<MaterialDTO>>(json);
                    foreach (var material in materialsJSON)
                    {
                        Order.Materials.Add(material);
                    }
                }
            }
        }

        private async void ConfirmOrder(object sender, EventArgs e)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.OrderConfirmationTitle,
                Message = ToastDisplayResources.OrderConfirmation,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });
            if(result == true)
            {
                List<MaterialDTO> materials = new List<MaterialDTO>();
                foreach(var material in Order.Materials)
                {
                    if(material.Quantity != 0)
                    {
                        materials.Add(material);
                    }
                }
                SendOrderAndMaterialData(materials);
            }
        }

        private async void SendOrderAndMaterialData(List<MaterialDTO> materials)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.UpdateURL + Order.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(materials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.DataUpdateSuccess);
                    NavigateToPreviousPageAsync();
                }
            }
        }

        private async void NavigateToPreviousPageAsync()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Navigation.PopAsync();
        }

        private void RequireMaterialToggled(object sender, ToggledEventArgs e)
        {
            if(requireMaterialSw.IsToggled)
            {
                materialsListView.IsVisible = true;
            }
            else
            {
                materialsListView.IsVisible = false;
            }
        }
    }
}