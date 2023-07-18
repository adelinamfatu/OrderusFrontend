using App.DTO;
using AppFrontend.CustomControls;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
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
    public partial class CompanyStockPage : ContentPage
    {
        public GlobalService globalService { get; set; }

        ObservableCollection<MaterialDTO> materials = new ObservableCollection<MaterialDTO>();
        public ObservableCollection<MaterialDTO> Materials { get { return materials; } }

        public CompanyStockPage()
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
                GetMaterials();
            }
        }

        private async void GetMaterials()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.MaterialsURL + globalService.Company.ID;

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
                    var materialsJSON = JsonConvert.DeserializeObject<List<MaterialDTO>>(json);
                    foreach (var material in materialsJSON)
                    {
                        materials.Add(material);
                    }
                }
            }
        }

        private void AddMaterialToList(object sender, EventArgs e)
        {
            var material = new MaterialDTO() 
            { 
                Name = materialName.Text, 
                Price = float.Parse(materialPrice.Text), 
                Quantity = int.Parse(materialQuantity.Text),
                CompanyID = globalService.Company.ID
            };
            materials.Add(material);
            AddMaterialData(material);
        }

        private async void AddMaterialData(MaterialDTO material)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.MaterialsURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("company_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(material);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);
            }
        }

        private void SaveMaterial(object sender, EventArgs e)
        {
            Button saveButton = (Button)sender;
            var grid = (Grid)saveButton.Parent;
            MaterialDTO material = (MaterialDTO)grid.BindingContext;
            material.CompanyID = globalService.Company.ID;
            SendUpdatedMaterialData(material);
        }

        private async void SendUpdatedMaterialData(MaterialDTO material)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.MaterialsURL + RestResources.UpdateURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("company_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(material);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.MaterialStockUpdateSuccess);
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.MaterialStockUpdateError);
                }
            }
        }
    }
}