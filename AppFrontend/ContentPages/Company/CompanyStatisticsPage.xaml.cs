using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    public partial class CompanyStatisticsPage : ContentPage
    {
        List<ChartEntry> ServicesData = new List<ChartEntry>();
        List<ChartEntry> EarningsData = new List<ChartEntry>();

        public GlobalService globalService { get; set; }

        public CompanyStatisticsPage()
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
                GetServicesData();
                GetMonthlyEarnings();
            }
        }

        private async void GetServicesData()
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.ServicesURL + globalService.Company.ID;

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
                    var orderCountJSON = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
                    BuildPieChart(orderCountJSON);
                }
            }
        }

        private async void GetMonthlyEarnings()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.EarningsURL + globalService.Company.ID;

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
                    var earningsJSON = JsonConvert.DeserializeObject<Dictionary<string, float>>(json);
                    BuildBarChart(earningsJSON);
                }
            }
        }

        private void BuildPieChart(Dictionary<string, int> orderCountJSON)
        {
            Random random = new Random();

            foreach (var service in orderCountJSON)
            {
                byte r = (byte)random.Next(256);
                byte g = (byte)random.Next(256);
                byte b = (byte)random.Next(256);
                var color = new SKColor(r, g, b);

                ServicesData.Add(new ChartEntry(service.Value)
                {
                    Color = color,
                    Label = service.Key,
                    ValueLabel = service.Value.ToString(),
                    ValueLabelColor = color
                });
            }
            servicesPieChart.Chart = new PieChart { Entries = ServicesData };
        }

        private void BuildBarChart(Dictionary<string, float> earningsJSON)
        {
            Random random = new Random();

            foreach (var monthlyEarning in earningsJSON)
            {
                byte r = (byte)random.Next(256);
                byte g = (byte)random.Next(256);
                byte b = (byte)random.Next(256);
                var color = new SKColor(r, g, b);

                EarningsData.Add(new ChartEntry(monthlyEarning.Value)
                {
                    Color = color,
                    Label = monthlyEarning.Key,
                    ValueLabel = monthlyEarning.Value.ToString(),
                    ValueLabelColor = color
                });
            }
            earningsBarChart.Chart = new BarChart { Entries = EarningsData };
        }

        private async void DownloadCSVFile(object sender, EventArgs e)
        {
            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.DownloadCSVRaportTitle,
                Message = ToastDisplayResources.DownloadCSVRaport,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if(response)
            {
                GetCompanyData();
            }
        }

        private async void GetCompanyData()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + globalService.Company.ID;

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
                    var ordersJSON = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    BuildCSVFile(ordersJSON);
                }
            }
        }

        private async void BuildCSVFile(List<OrderDTO> orders)
        {
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("EmailAngajat,Serviciu,TimpIncepere,Durata,SumaPlata,Anulat");

            foreach (var order in orders)
            {
                csvContent.AppendLine($"{order.EmployeeEmail},{order.ServiceName},{order.StartTime},{order.Duration},{order.PaymentAmount},{order.IsCancelled}");
            }

            string fileName = $"date_{globalService.Company.Name}.csv";
            var file = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllText(file, csvContent.ToString());

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(file)
            });
        }
    }
}