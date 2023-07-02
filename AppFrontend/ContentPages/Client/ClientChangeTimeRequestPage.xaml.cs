using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientChangeTimeRequestPage : ContentPage
    {
        public GlobalService globalService { get; set; }

        ObservableCollection<OrderChangeDTO> orders = new ObservableCollection<OrderChangeDTO>();
        public ObservableCollection<OrderChangeDTO> Orders { get { return orders; } }

        public ClientChangeTimeRequestPage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            RetrieveOrders();
            this.BindingContext = this;
        }

        private async void RetrieveOrders()
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.ClientsURL + globalService.Client.Email;

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
                    List<OrderChangeDTO> ordersJSON = JsonConvert.DeserializeObject<List<OrderChangeDTO>>(json);
                    BuildOrderList(ordersJSON);
                }
            }
        }

        private void BuildOrderList(List<OrderChangeDTO> ordersJSON)
        {
            foreach (var order in ordersJSON)
            {
                orders.Add(order);
            }
        }

        private async void ConfirmOrderTimeChangeRequest(object sender, EventArgs e)
        {
            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.ConfirmOrderTimeChangeRequestTitle,
                Message = ToastDisplayResources.ConfirmOrderTimeChangeRequest,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if(response)
            {
                Button button = (Button)sender;
                OrderChangeDTO order = (OrderChangeDTO)button.BindingContext;
                ChangeOrderTime(order.OrderID);
            }
        }

        private async void ChangeOrderTime(int orderID)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.ChangeURL + orderID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var email = JsonConvert.SerializeObject(globalService.Client.Email);
                var content = new StringContent(email, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.ConfirmOrderTimeChangeRequestSuccess);
                    RemoveOrderFromUI(orderID);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.ConfirmOrderTimeChangeRequestError);
                }
            }
        }

        private async void DeclineOrderTimeChangeRequest(object sender, EventArgs e)
        {
            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.DeclineOrderTimeChangeRequestTitle,
                Message = ToastDisplayResources.DeclineOrderTimeChangeRequest,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if (response)
            {
                Button button = (Button)sender;
                OrderChangeDTO order = (OrderChangeDTO)button.BindingContext;
                DeclineOrderTimeChange(order.OrderID);
            }
        }

        private async void DeclineOrderTimeChange(int orderID)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.ChangeURL + RestResources.DeleteURL + orderID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.DeclineOrderTimeChangeRequestSuccess);
                    RemoveOrderFromUI(orderID);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DeclineOrderTimeChangeRequestError);
                }
            }
        }

        private void RemoveOrderFromUI(int orderID)
        {
            Orders.Remove(Orders.Where(o => o.OrderID == orderID).FirstOrDefault());
        }
    }
}