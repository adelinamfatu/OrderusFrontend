using AppFrontend.ContentPages;
using AppFrontend.Resources;
using Microsoft.Extensions.DependencyInjection;
using Plugin.FirebasePushNotification;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DependencyService.Register<GlobalService>();
            var clientToken = SecureStorage.GetAsync("client_token").Result;
            var employeeToken = SecureStorage.GetAsync("employee_token").Result;
            var companyToken = SecureStorage.GetAsync("company_token").Result;
            if (!string.IsNullOrEmpty(clientToken))
            {
                MainPage = new ClientMenuPage();
            }
            else if(!string.IsNullOrEmpty(employeeToken))
            {
                MainPage = new EmployeeMenuPage();
            }
            else if(!string.IsNullOrEmpty(companyToken))
            {
                MainPage = new CompanyMenuPage();
            }
            else
            {
                MainPage = new LoginPage();
            }
            CrossFirebasePushNotification.Current.OnTokenRefresh += Current_OnTokenRefresh;
            CrossFirebasePushNotification.Current.OnNotificationError += Current_OnNotificationError;
            System.Diagnostics.Debug.WriteLine($"Token: {CrossFirebasePushNotification.Current.Token}");
        }

        private void Current_OnNotificationError(object source, FirebasePushNotificationErrorEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Eroare: {e.Message}");
        }

        private void Current_OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"Token: {e.Token}");
        }
    }
}
