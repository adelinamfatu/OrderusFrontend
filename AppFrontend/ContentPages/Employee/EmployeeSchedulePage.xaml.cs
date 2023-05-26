using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using SegmentedControl.FormsPlugin.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        ObservableCollection<OrderDTO> orders = new ObservableCollection<OrderDTO>();
        public ObservableCollection<OrderDTO> Orders { get { return orders; } }

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
                this.Orders.Add(order);
            }
        }

        private void OnDayOptionsSelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            string selectedOption = dayOptionsSC.Children[dayOptionsSC.SelectedSegment].Text;
            if (selectedOption == DisplayPrompts.Today)
            {
                var filteredOrders = Orders.Where(order => order.DateTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
            else if(selectedOption == DisplayPrompts.Tomorrow)
            {
                currentTime = currentTime.AddDays(1);
                var filteredOrders = Orders.Where(order => order.DateTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
            else
            {
                currentTime = currentTime.AddDays(2);
                var filteredOrders = Orders.Where(order => order.DateTime.Date == currentTime.Date).ToList();
                scheduledOrdersListView.ItemsSource = filteredOrders;
            }
        }

        private void scheduledOrdersListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.Item == null)
                return;

            var item = e.Item as OrderDTO;
            DateTime currentTime = DateTime.Now;
            DateTime startTime = item.DateTime;
            DateTime finishTime = item.FinishTime; 

            if (currentTime >= startTime && currentTime < finishTime)
            {
                item.AreButtonsVisible = true;
            }
            else
            {
                item.AreButtonsVisible = false;
            }
        }
    }
}