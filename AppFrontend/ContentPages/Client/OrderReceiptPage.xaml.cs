using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderReceiptPage : ContentPage
    {
        public OrderViewModel Order { get; set; }

        public CompanyDTO Company { get; set; }

        public GlobalService globalService { get; set; }

        public OrderReceiptPage(OrderViewModel order)
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            this.Order = order;
            GetDataForUI();
        }

        private async void GetDataForUI()
        {
            await GetOrderMaterials();
            await GetCompanyData();
            this.BindingContext = this;
        }

        private async Task GetOrderMaterials()
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.MaterialsURL + Order.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
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

        private async Task GetCompanyData()
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.CompaniesURL + Order.ID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Company = JsonConvert.DeserializeObject<CompanyDTO>(json);
                }
            }
        }

        private async void DownloadOrderAsPDF(object sender, EventArgs e)
        {
            try
            {
                var pdfGenerator = new ReceiptPdfGenerator();
                await pdfGenerator.GeneratePdf(Order, Company);
            }
            catch (Exception ex)
            {

            }
        }
    }
}