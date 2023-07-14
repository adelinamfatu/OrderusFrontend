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

        private HashSet<string> privateLessonsTypes = new HashSet<string>
        {
            ServiceType.Matematica.ToString(),
            ServiceType.Romana.ToString(),
            ServiceType.Franceza.ToString(),
            ServiceType.Chimie.ToString(),
            ServiceType.Fizica.ToString(),
            ServiceType.Engleza.ToString()
        };

        private HashSet<string> businessTypes = new HashSet<string>
        {
            ServiceType.Contabilitate.ToString(),
            ServiceType.Marketing_si_publicitate.ToString().Replace("_", " "),
            ServiceType.Consultanta_financiara.ToString().Replace("_", " ")
        };

        private HashSet<string> eventTypes = new HashSet<string>
        {
            ServiceType.Fotografie.ToString(),
            ServiceType.Planificare_si_coordonare.ToString().Replace("_", " ")
        };

        public BasketPage()
        {
            InitializeComponent();
            Order = new OrderViewModel();
            GetBasketInformation();
            globalService = DependencyService.Get<GlobalService>();
            MessagingCenter.Subscribe<CompanyPage, BasketItemMessage>(this, "BasketItemMessage", (sender, message) =>
            {
                CSO = message.CSO;
                SetUIServiceInformation();
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
            Order.CompanyName = CSO.Company.Name;
            Order.UnitOfMeasurement = CSO.Service.UnitOfMeasurement;
            Order.Price = CSO.Price;

            if(CSO.Service.Name == ServiceType.Curatenie.ToString())
            {
                surfaceLabel.IsVisible = true;
                surfaceEntry.IsVisible = true;
                noRoomsLabel.IsVisible = true;
                noRoomsEntry.IsVisible = true;
            }
            else if(privateLessonsTypes.Contains(CSO.Service.Name) || businessTypes.Contains(CSO.Service.Name)
                                        || eventTypes.Contains(CSO.Service.Name))
            {
                etaBtn.Text = "Calculeaza pret";
            }
            else if(CSO.Service.Name == ServiceType.Reparatii.ToString())
            {
                complexityLabel.IsVisible = true;
                complexitySlider.IsVisible = true;
                noRepairsLabel.IsVisible = true;
                noRepairsEntry.IsVisible = true;
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
                Order.Offers.Add(offer.Discount + (offer.Type == DiscountType.Percentage ? "%" : " lei"));
            }
            this.BindingContext = Order;
        }

        private void CalculateEstimatedDuration(object sender, EventArgs e)
        {
            if(CheckTimeAndDate())
            {
                CSO.DateTime = new DateTime(datePicker.Date.Year,
                                                    datePicker.Date.Month,
                                                    datePicker.Date.Day,
                                                    timePicker.Time.Hours,
                                                    timePicker.Time.Minutes,
                                                    timePicker.Time.Seconds);
                if (CSO.Service.Name == ServiceType.Curatenie.ToString())
                {
                    if (surfaceEntry.Text == null && noRoomsEntry.Text == null)
                    {
                        CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DataMissingError);
                    }
                    else
                    {
                        CSO.Surface = int.Parse(surfaceEntry.Text);
                        CSO.NbRooms = int.Parse(noRoomsEntry.Text);
                        SendServiceData(ConvertCSOToPO.Convert(CSO, globalService.Client.Email), RestResources.CleaningURL);
                    }
                }
                else if (privateLessonsTypes.Contains(CSO.Service.Name) || businessTypes.Contains(CSO.Service.Name)
                                                || eventTypes.Contains(CSO.Service.Name))
                {
                    ShowToast(60);
                }
                else if(CSO.Service.Name == ServiceType.Reparatii.ToString())
                {
                    if (noRepairsEntry.Text == null)
                    {
                        CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DataMissingError);
                    }
                    else
                    {
                        CSO.Complexity = (int)complexitySlider.Value;
                        CSO.NbRepairs = int.Parse(noRepairsEntry.Text);
                        SendServiceData(ConvertCSOToPO.Convert(CSO, globalService.Client.Email), RestResources.RepairingURL);
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

        private async void SendServiceData(PossibleOrderDTO po, string serviceName)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + serviceName;

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
            if(CSO.Service.Name == ServiceType.Curatenie.ToString() || CSO.Service.Name == ServiceType.Reparatii.ToString())
            {
                CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.OrderTimeEstimationSuccess);
            }
            CSO.Duration = duration;
            Order.Duration = duration;
            if(CSO.Service.Name == ServiceType.Curatenie.ToString())
            {
                Order.PaymentAmount = CSO.Price * int.Parse(surfaceEntry.Text);
            }
            else if(CSO.Service.Name == ServiceType.Reparatii.ToString())
            {
                Order.PaymentAmount = CSO.Price * int.Parse(noRepairsEntry.Text);
            }
            else if(privateLessonsTypes.Contains(CSO.Service.Name) || businessTypes.Contains(CSO.Service.Name)
                                    || eventTypes.Contains(CSO.Service.Name))
            {
                Order.PaymentAmount = CSO.Price * (CSO.Duration / 60);
            }
            Order.InitialPaymentAmount = Order.PaymentAmount;
            orderButton.IsEnabled = true;
        }

        private void PlaceOrder(object sender, EventArgs e)
        {
            var order = ConvertToOrder();
            AddOrder(order);
            if(offersPicker.SelectedItem != null)
            {
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
            var order = new OrderDTO()
            {
                StartTime = CSO.DateTime,
                Duration = CSO.Duration,
                ServiceID = CSO.Service.ID,
                ClientEmail = globalService.Client.Email,
                CompanyID = CSO.Company.ID,
                PaymentAmount = Order.PaymentAmount,
                Comment = commentEntry.Text
            };

            if (CSO.Service.Name == ServiceType.Curatenie.ToString())
            {
                order.Details = GetDetailsDictionary();
            }
            return order;
        }

        private Dictionary<string, string> GetDetailsDictionary()
        {
            Dictionary<string, string> details = new Dictionary<string, string>();
            if(CSO.Service.Name == ServiceType.Curatenie.ToString())
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