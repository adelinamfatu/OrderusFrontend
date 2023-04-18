using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private GlobalService globalService;
        public LoginPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
        }

        private void OpenCreateAccountPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new CreateAccountPage());
        }

        private async void SendUserInfo(object sender, EventArgs e)
        {
            ClientDTO client = GetClientFromUI();
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.LoginURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var json = JsonConvert.SerializeObject(client);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    await ShowSuccessToastAndWaitForDismissal(ToastDisplayResources.LoginSuccess);
                    var token = await response.Content.ReadAsStringAsync();
                    await SecureStorage.SetAsync("orderus_token", token);
                    OpenCategoryPage();
                }
                if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.LoginError);
                }
            }
        }

        private async Task ShowSuccessToastAndWaitForDismissal(string successMessage)
        {
            CrossToastPopUp.Current.ShowToastSuccess(successMessage);
            await Task.Delay(1000);
        }

        private ClientDTO GetClientFromUI()
        {
            return new ClientDTO
            {
                Email = email.Text,
                Password = password.Text
            };
        }

        private void OpenCategoryPage()
        {
            Application.Current.MainPage = new MenuPage();
        }
    }
}