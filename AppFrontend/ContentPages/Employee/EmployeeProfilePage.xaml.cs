using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AppFrontend.ContentPages.Employee
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeProfilePage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public EmployeeViewModel employee { get; set; }

        public EmployeeProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Employee")
            {
                this.employee = new EmployeeViewModel(globalService.Employee);
                this.BindingContext = employee;
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
                string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.PhotoURL;

                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
                using (HttpClient httpClient = new HttpClient(handler))
                {
                    var token = SecureStorage.GetAsync("employee_token").Result;
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    var multipartContent = new MultipartFormDataContent();
                    var imageContent = new StreamContent(stream);

                    imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                    imageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "logo",
                        FileName = globalService.Employee.Email + ".png"
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

        private void GetEmployeeAccountUpdatesFromUI(object sender, EventArgs e)
        {
            var Employee = new EmployeeDTO()
            {
                Email = employee.Email,
                Phone = employee.Phone
            };
            SendEmployeeAccountUpdates(Employee);
        }

        private async void SendEmployeeAccountUpdates(EmployeeDTO Employee)
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.DetailsURL + RestResources.UpdateURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(Employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.DataUpdateSuccess);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DataUpdateFail);
                }
            }
        }
    }
}