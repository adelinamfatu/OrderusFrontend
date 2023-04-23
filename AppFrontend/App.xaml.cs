using AppFrontend.ContentPages;
using AppFrontend.Resources;
using Microsoft.Extensions.DependencyInjection;
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
                MainPage = new LoginPage();
            }
            else if(!string.IsNullOrEmpty(employeeToken))
            {
                
            }
            else if(!string.IsNullOrEmpty(companyToken))
            {
                MainPage = new CompanyMenuPage();
            }
            else
            {
                MainPage = new MenuPage();
            }
        }
    }
}
