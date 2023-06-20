using Acr.UserDialogs;
using App.DTO;
using AppFrontend.ContentPages.Client;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using AppFrontend.ViewModels;
using Newtonsoft.Json;
using Plugin.Toast;
using SegmentedControl.FormsPlugin.Abstractions;
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

namespace AppFrontend.ContentPages.Employee
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EmployeeSchedulePage : ContentPage
    {
        ObservableCollection<OrderViewModel> orders = new ObservableCollection<OrderViewModel>();
        public ObservableCollection<OrderViewModel> Orders { get { return orders; } }

        public GlobalService globalService { get; set; }

        public EmployeeSchedulePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Employee")
            {
                GetDataForUI();
                this.BindingContext = this;
            }
        }

        private async void GetDataForUI()
        {
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.ScheduleURL + globalService.Employee.Email;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var ordersJSON = JsonConvert.DeserializeObject<List<OrderDTO>>(json);
                    BuildSchedule(ordersJSON);
                }
            }
        }

        private void BuildSchedule(IEnumerable<OrderDTO> ordersJSON)
        {
            foreach(var order in ordersJSON)
            {
                this.Orders.Add(new OrderViewModel(order));
            }
            OnDayOptionsSelectedIndexChanged(null, null);
        }

        private void OnDayOptionsSelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            string selectedOption = dayOptionsSC.Children[dayOptionsSC.SelectedSegment].Text;
            if (selectedOption == DisplayPrompts.Today)
            {
                var filteredOrders = Orders.Where(order => order.StartTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
            else if(selectedOption == DisplayPrompts.Tomorrow)
            {
                currentTime = currentTime.AddDays(1);
                var filteredOrders = Orders.Where(order => order.StartTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
            else
            {
                currentTime = currentTime.AddDays(2);
                var filteredOrders = Orders.Where(order => order.StartTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
        }

        private async void DelayOrders(object sender, EventArgs e)
        {
            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.DelayOrderTitle,
                Message = ToastDisplayResources.DelayOrder,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if(response)
            {
                SendDelayInformation();
                UpdateUIInformation();
            }
        }

        private async void SendDelayInformation()
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.DelayURL + globalService.Employee.Email;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(globalService.Employee.Email, Encoding.UTF8, "text/plain");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.DelayOrderSuccess);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.DelayOrderError);
                }
            }
        }

        private void UpdateUIInformation()
        {
            foreach(var Order in Orders)
            {
                if(Order.StartTime.Date == DateTime.Now.Date)
                {
                    if(Order.StartTime <= DateTime.Now && DateTime.Now <= Order.FinishTime)
                    {
                        Order.Duration += 10;
                    }
                    else
                    {
                        Order.StartTime = Order.StartTime.AddMinutes(10);
                    }
                    Order.FinishTime = Order.FinishTime.AddMinutes(10);
                }
            }
        }

        private async void FinishOrder(object sender, EventArgs e)
        {
            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.FinishOrderTitle,
                Message = ToastDisplayResources.FinishOrder,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if (response)
            {
                Button button = (Button)sender;
                int orderId = (int)button.CommandParameter;
                SendFinishingOrderConfirmation(orderId);
            }
        }

        private async void SendFinishingOrderConfirmation(int orderID)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + orderID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var content = new StringContent(globalService.Employee.Email, Encoding.UTF8, "text/plain");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.FinishOrderSuccess);
                    Orders.Where(o => o.ID == orderID).FirstOrDefault().IsCurrentOrder = false;
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.FinishOrderError);
                }
            }
        }

        private void MakeOrderDateChangeRequest(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TimePicker timePicker = (TimePicker)button.Parent.FindByName("myTimePicker");

            if (timePicker != null)
            {
                timePicker.Unfocused += TimePickerUnfocused;
                timePicker.Focus();
            }
        }

        private async void TimePickerUnfocused(object sender, FocusEventArgs e)
        {
            TimePicker timePicker = (TimePicker)sender;
            var selectedTime = timePicker.Time;

            if(selectedTime.Seconds == 0)
            {
                int orderId = (int)timePicker.Parent.FindByName<Button>("btnChangeOrderHour").CommandParameter;

                var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Title = ToastDisplayResources.ChangeOrderTimeTitle,
                    Message = ToastDisplayResources.ChangeOrderTime,
                    OkText = ToastDisplayResources.PromptYes,
                    CancelText = ToastDisplayResources.PromptCancel
                });

                if(response)
                {
                    SendOrderTimeChangeRequest(selectedTime, orderId);
                }
                
                timePicker.Unfocused -= TimePickerUnfocused;
            }
        }

        private async void SendOrderTimeChangeRequest(TimeSpan time, int orderID)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.TimeURL + orderID;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("employee_token").Result;
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var timeJson = JsonConvert.SerializeObject(time);
                var content = new StringContent(timeJson, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.ChangeOrderSuccess);
                }
                if (response.StatusCode == HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.ChangeOrderError);
                }
            }
        }
    }
}