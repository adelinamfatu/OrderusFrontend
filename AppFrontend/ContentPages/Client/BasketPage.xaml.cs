using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using AppFrontend.ViewModels;
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

        public OrderViewModel Order { get; set; }

        public List<OfferDTO> OffersJSON { get; set; }

        public List<string> Offers { get; set; }

        public BasketPage()
        {
            InitializeComponent();
            Offers = new List<string>();
            GetBasketInformation();
            globalService = DependencyService.Get<GlobalService>();
            MessagingCenter.Subscribe<CompanyPage, BasketItemMessage>(this, "BasketItemMessage", (sender, message) =>
            {
                CSO = message.CSO;
            });
            if(Preferences.ContainsKey("BasketItem"))
            {
                SetUIServiceInformation();
            }
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Client")
            {
                GetOffers();
            }
        }

        private void GetBasketInformation()
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                var basketItemJson = Preferences.Get("BasketItem", "");
                CSO = JsonConvert.DeserializeObject<CompanyServiceOptionDTO>(basketItemJson);
                Order = new OrderViewModel();
                Order.Duration = 0;
            }
            else
            {
                serviceDetailsFrame.IsVisible = false;
                serviceOrderFrame.IsVisible = false;
                priceInfoStackLayout.IsVisible = false;
                emptyBasketImg.IsVisible = true;
                emptyBasketLabel.IsVisible = true;
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
                priceInfoStackLayout.IsVisible = false;
                emptyBasketImg.IsVisible = true;
                emptyBasketLabel.IsVisible = true;
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

        private async void GetOffers()
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.OfferURL + RestResources.AvailableURL + globalService.Client.Email;

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
                    OffersJSON = JsonConvert.DeserializeObject<List<OfferDTO>>(json);
                    BuildOfferPicker();
                }
            }
        }

        private void BuildOfferPicker()
        {
            foreach(var offer in OffersJSON)
            {
                Offers.Add(offer.Discount + (offer.Type == DiscountType.Percentage ? "%" : " lei"));
            }
            this.BindingContext = this;
        }

        private void CalculateEstimatedDuration(object sender, EventArgs e)
        {
            if(CheckTimeAndDate())
            {
                if (CSO.Service.Name == ServiceType.Curatare.ToString())
                {
                    if (surfaceEntry.Text == null && noRoomsEntry.Text == null)
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

                //other orders
            }
        }

        private bool CheckTimeAndDate()
        {
            DateTime currentDate = DateTime.Now.Date;
            if(datePicker.Date < currentDate || datePicker.Date == currentDate)
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.PastDateOrderError);
                return false;
            }
            else if(datePicker.Date.DayOfWeek == DayOfWeek.Saturday || datePicker.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.WeekendOrderError);
                return false;
            }
            else if(timePicker.Time < TimeSpan.FromHours(8) || timePicker.Time > TimeSpan.FromHours(20))
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.OutOfOfficeOrderError);
                return false;
            }
            return true;
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
                else if(response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.OrderTimeEstimationError);
                }
            }
        }

        private void ShowToast(int duration)
        {
            CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.OrderTimeEstimationSuccess);
            CSO.Duration = duration;
            Order.Duration = duration;
            Order.PaymentAmount = CSO.Price * (CSO.Duration / 60.0f);
            Order.InitialPaymentAmount = Order.PaymentAmount;
            orderButton.IsEnabled = true;
        }

        private void PlaceOrder(object sender, EventArgs e)
        {
            var order = ConvertToOrder();
            AddOrder(order);
            var offerID = 1;
            var selectedOffer = offersPicker.SelectedItem.ToString();
            if (selectedOffer.Contains("%"))
            {
                int numericValue = int.Parse(selectedOffer.Substring(0, selectedOffer.IndexOf("%")));
                offerID = OffersJSON.Where(o => o.Type == DiscountType.Percentage && o.Discount == numericValue).FirstOrDefault().ID;
            }
            else
            {
                int numericValue = int.Parse(selectedOffer.Substring(0, selectedOffer.IndexOf(" lei")));
                offerID = OffersJSON.Where(o => o.Type == DiscountType.Value && o.Discount == numericValue).FirstOrDefault().ID;
            }
            DeleteOffer(offerID);
        }

        private async void DeleteOffer(int offerID)
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.OfferURL + RestResources.DeleteURL + offerID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(url);
            }
        }

        private async void AddOrder(OrderDTO order)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(order);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.PlaceOrderSuccess);
                    RemoveOrderFromBasket(null, EventArgs.Empty);
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.PlaceOrderError);
                }
            }
        }

        private OrderDTO ConvertToOrder()
        {
            return new OrderDTO()
            {
                StartTime = CSO.DateTime,
                Duration = CSO.Duration,
                ServiceID = CSO.Service.ID,
                ClientEmail = globalService.Client.Email,
                CompanyID = CSO.Company.ID,
                PaymentAmount = Order.PaymentAmount,
                Details = GetDetailsDictionary(),
                Comment = commentEntry.Text
            };
        }

        private Dictionary<string, string> GetDetailsDictionary()
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            if(CSO.Service.Name == ServiceType.Curatare.ToString())
            {
                details.Add(noRoomsLabel.Text, noRoomsEntry.Text);
                details.Add(surfaceLabel.Text, surfaceEntry.Text);
            }
            return details;
        }

        private void AddOfferToOrder(object sender, EventArgs e)
        {
            this.Order.PaymentAmount = this.Order.InitialPaymentAmount;
            var selectedOffer = offersPicker.SelectedItem.ToString();

            if (selectedOffer.Contains("%"))
            {
                string numericValue = selectedOffer.Substring(0, selectedOffer.IndexOf("%"));
                int value = int.Parse(numericValue);
                Order.PaymentAmount -= this.Order.PaymentAmount / 100 * value;
            }
            else if (selectedOffer.Contains(" lei"))
            {
                string numericValue = selectedOffer.Substring(0, selectedOffer.IndexOf(" lei"));
                int value = int.Parse(numericValue);
                Order.PaymentAmount -= value;
            }

            CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.OrderOfferSuccess);
        }

        private void datePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            orderButton.IsEnabled = false;
        }
    }
}