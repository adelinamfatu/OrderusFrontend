﻿using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Microcharts;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
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
    public partial class CompanyStatisticsPage : ContentPage
    {
        List<ChartEntry> ServicesData = new List<ChartEntry>();

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
                    ValueLabelColor = color,
                    LabelTextSize = 
                });
            }
            servicesPieChart.Chart = new PieChart { Entries = ServicesData };
        }
    }
}