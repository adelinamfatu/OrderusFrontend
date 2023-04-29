using AppFrontend.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Net;
using System.Net.Http;
using AppFrontend.Resources.Files;
using Plugin.Toast;
using System.IO;
using System.ComponentModel;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using App.DTO;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyProfilePage : ContentPage, INotifyPropertyChanged
    {
        public GlobalService globalService { get; set; }

        public CompanyViewModel company { get; set; }

        public CompanyProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Company")
            {
                this.company = new CompanyViewModel(globalService.Company);
                GetAllServices();
                this.BindingContext = company;
            }
        }

        private async void GetAllServices()
        {
            string url = RestResources.ConnectionURL + RestResources.ServicesURL;

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
                    var servicesJSON = JsonConvert.DeserializeObject<List<ServiceDTO>>(json);
                    foreach(var service in servicesJSON)
                    {
                        company.Services.Add(service);
                    }
                }
            }
        }

        private void ChangePage(object sender, CheckedChangedEventArgs e)
        {
            if(page2Button.IsChecked == true)
            {
                name.IsVisible = false;
                city.IsVisible = false;
                street.IsVisible = false;
                streetNumber.IsVisible = false;
                building.IsVisible = false;
                staircase.IsVisible = false;
                apartmentNumber.IsVisible = false;
                floor.IsVisible = false;
                site.IsVisible = false;
                description.IsVisible = false;
                servicesLabel.IsVisible = false;
                saveButton.IsVisible = false;
                functionEntryGrid.IsVisible = true;
                functionsUniformGrid.IsVisible = true;
                multiListViewServices.IsVisible = false;
            }
            else if(page1Button.IsChecked == true)
            {
                name.IsVisible = true;
                city.IsVisible = true;
                street.IsVisible = true;
                streetNumber.IsVisible = true;
                building.IsVisible = true;
                staircase.IsVisible = true;
                apartmentNumber.IsVisible = true;
                floor.IsVisible = true;
                site.IsVisible = true;
                description.IsVisible = true;
                servicesLabel.IsVisible = false;
                saveButton.IsVisible = false;
                functionEntryGrid.IsVisible = false;
                functionsUniformGrid.IsVisible = false;
                multiListViewServices.IsVisible = false;
            }
            else
            {
                name.IsVisible = false;
                city.IsVisible = false;
                street.IsVisible = false;
                streetNumber.IsVisible = false;
                building.IsVisible = false;
                staircase.IsVisible = false;
                apartmentNumber.IsVisible = false;
                floor.IsVisible = false;
                site.IsVisible = false;
                description.IsVisible = false;
                servicesLabel.IsVisible = true;
                saveButton.IsVisible = true;
                functionEntryGrid.IsVisible = false;
                functionsUniformGrid.IsVisible = false;
                multiListViewServices.IsVisible = true;
            }
        }

        private void AddFunctionToList(object sender, EventArgs e)
        {
            if(company.Functions.Contains(function.Text))
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.FunctionError);
            }
            else
            {
                company.Functions.Add(function.Text);
            }
        }

        private async void ChoosePhotoFromGallery(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Selectati o image"
                });

                if (result != null)
                {
                    var selectedPhotoPath = result.FullPath;
                    SendPhoto(selectedPhotoPath);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private async void SendPhoto(string selectedPhotoPath)
        {
            using (var stream = new FileStream(selectedPhotoPath, FileMode.Open, FileAccess.Read))
            {
                string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.PhotoURL;

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    var multipartContent = new MultipartFormDataContent();
                    var imageContent = new StreamContent(stream);

                    imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                    imageContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                    {
                        Name = "logo",
                        FileName = globalService.Company.Name + ".png"
                    };

                    multipartContent.Add(imageContent);

                    HttpResponseMessage response = await httpClient.PostAsync(url, multipartContent);

                    if (response.IsSuccessStatusCode)
                    {
                        CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.SavePhotoSuccess);
                    }
                    if (response.StatusCode == HttpStatusCode.Conflict)
                    {
                        CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.SavePhotoFail);
                    }
                }
            }
        }

        private void SaveCompanyAccountUpdates(object sender, EventArgs e)
        {

        }
    }
}