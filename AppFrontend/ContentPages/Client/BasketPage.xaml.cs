using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
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
    public partial class BasketPage : ContentPage
    {
        public CompanyServiceOptionDTO CSO { get; set; }

        public GlobalService globalService { get; set; }

        public BasketPage()
        {
            InitializeComponent();
            GetBasketInformation();
            globalService = DependencyService.Get<GlobalService>();
            MessagingCenter.Subscribe<CompanyPage, BasketItemMessage>(this, "BasketItemMessage", (sender, message) =>
            {
                CSO = message.CSO;
            });
            SetUIServiceInformation();
            this.BindingContext = this;
        }

        private void GetBasketInformation()
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                var basketItemJson = Preferences.Get("BasketItem", "");
                CSO = JsonConvert.DeserializeObject<CompanyServiceOptionDTO>(basketItemJson);
            }
            else
            {
                serviceDetailsFrame.IsVisible = false;
                serviceOrderFrame.IsVisible = false;
            }
        }

        private void RemoveOrderFromBasket(object sender, EventArgs e)
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                Preferences.Remove("BasketItem");
                CSO = null;
                serviceDetailsFrame.IsVisible = false;
                serviceOrderFrame.IsVisible = false;
            }
        }

        private void SetUIServiceInformation()
        {
            if(CSO.Service.Name == ServiceType.Curatare.ToString())
            {
                surfaceLabel.IsVisible = true;
                surfaceEntry.IsVisible = true;
                noRoomsLabel.IsVisible = true;
                noRoomsEntry.IsVisible = true;
            }
        }

        private void CalculateEstimatedDuration(object sender, EventArgs e)
        {
            if (CSO.Service.Name == ServiceType.Curatare.ToString())
            {
                if(surfaceEntry.Text == null && noRoomsEntry.Text == null)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DataMissingError);
                }
                else
                {
                    CSO.Surface = int.Parse(surfaceEntry.Text);
                    CSO.NbRooms = int.Parse(noRoomsEntry.Text);
                    CSO.DateTime = new DateTime(datePicker.Date.Year,
                                                datePicker.Date.Month,
                                                datePicker.Date.Day,
                                                timePicker.Time.Hours,
                                                timePicker.Time.Minutes,
                                                timePicker.Time.Seconds);
                    SendCleaningServiceData(ConvertCSOToPO.Convert(CSO, globalService.Client.Email));
                }
            }
            

        }

        private async void SendCleaningServiceData(PossibleOrderDTO po)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.CleaningURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(po);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var contentString = await response.Content.ReadAsStringAsync();
                    var duration = int.Parse(contentString);
                    ShowToast(duration);
                }
            }
        }

        private void ShowToast(int duration)
        {
            if (duration == -1)
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.OrderTimeEstimationError);
            }
            else
            {
                CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.OrderTimeEstimationSuccess);
                CSO.Duration = duration;
                orderButton.IsEnabled = true;
            }
        }
    }
}