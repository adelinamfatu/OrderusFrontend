using Acr.UserDialogs;
using App.DTO;
using AppFrontend.ContentPages.Client;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class HistoryPage : ContentPage
    {
        ObservableCollection<OrderViewModel> orders = new ObservableCollection<OrderViewModel>();
        public ObservableCollection<OrderViewModel> Orders { get { return orders; } }

        public GlobalService globalService { get; set; }

        public HistoryPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Client")
            {
                GetDataForUI();
                this.BindingContext = this;
            }
        }

        private async void GetDataForUI()
        {
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.HistoryURL + globalService.Client.Email;

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
                    var ordersJSON = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    BuildHistory(ordersJSON);
                }
            }
        }

        private void BuildHistory(IEnumerable<OrderDTO> ordersJSON)
        {
            foreach (var order in ordersJSON)
            {
                this.Orders.Add(new OrderViewModel(order));
            }
        }

        private void OpenReceipt(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            OrderViewModel order = (OrderViewModel)button.BindingContext;
            Navigation.PushAsync(new OrderReceiptPage(order));
        }

        private async void CancelOrder(object sender, EventArgs e)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.CancelOrderTitle,
                Message = ToastDisplayResources.CancelOrder,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if(result)
            {
                var ID = int.Parse(((MenuItem)sender).CommandParameter.ToString());
                SendCancelOrderRequest(ID);
            }
        }

        private async void SendCancelOrderRequest(int orderID)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.DeleteURL + orderID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.CancelOrderSuccess);
                    CancelOrderOnUI(orderID);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.CancelOrderError);
                }
            }
        }

        private void CancelOrderOnUI(int orderID)
        {
            var order = Orders.Where(o => o.ID == orderID).FirstOrDefault();
            order.IsFinished = true;
            order.Color = Color.PaleVioletRed;
        }
    }
}