using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientMenuPage : Xamarin.Forms.TabbedPage
    {
        private GlobalService globalService { get; set; }

        public ClientMenuPage()
        {
            InitializeComponent();
            TabStyling();
            globalService = DependencyService.Get<GlobalService>();
            SetClientData();
        }

        public void TabStyling()
        {
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            this.BarBackgroundColor = Color.FromHex("#6ba1f9");
            this.SelectedTabColor = Color.White;
            this.UnselectedTabColor = Color.FromHex("#667284");
        }

        private async void SetClientData()
        {
            var token = SecureStorage.GetAsync("client_token").Result;
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.TokenURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    ClientDTO user = JsonConvert.DeserializeObject<ClientDTO>(json);
                    globalService.Client = user;
                }
            }
        }

        private async void LogoutAccount(object sender, EventArgs e)
        {
            var result = await DisplayConfirmPopUp();
            if(result == true)
            {
                SecureStorage.Remove("client_token");
                Application.Current.MainPage = new LoginPage();
            }
        }

        private async Task<bool> DisplayConfirmPopUp()
        {
            return await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.LogoutConfirmationTitle,
                Message = ToastDisplayResources.LogoutConfirmation,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });
        }
    }
}