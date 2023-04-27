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

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyProfilePage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public CompanyProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Company")
            {
                this.BindingContext = globalService.Company;
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
                functionEntryGrid.IsVisible = true;
                functionsUniformGrid.IsVisible = true;
            }
            else
            {
                name.IsVisible = true;
                city.IsVisible = true;
                street.IsVisible = true;
                streetNumber.IsVisible = true;
                building.IsVisible = true;
                staircase.IsVisible = true;
                apartmentNumber.IsVisible = true;
                functionEntryGrid.IsVisible = false;
                functionsUniformGrid.IsVisible = false;
            }
        }

        private void AddFunctionToList(object sender, EventArgs e)
        {
            if (globalService.Company.Functions == null)
            {
                globalService.Company.Functions = new List<string>();
            }
            globalService.Company.Functions.Add(function.Text);
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
    }
}