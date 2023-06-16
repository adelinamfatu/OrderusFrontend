using App.DTO;
using AppFrontend.ContentPages.Client;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public partial class ClientProfilePage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public ClientProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "Client")
            {
                this.BindingContext = globalService.Client;
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
                string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.PhotoURL;

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    var token = SecureStorage.GetAsync("client_token").Result;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var multipartContent = new MultipartFormDataContent();
                    var imageContent = new StreamContent(stream);

                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "logo",
                        FileName = globalService.Client.Email + ".png"
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

        private void OpenAccountInformationPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ClientAddressPage());
        }

        private void OpenSpinningWheelPage(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ClientSpinningWheelPage());
        }
    }
}