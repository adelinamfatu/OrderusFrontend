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
            var token = SecureStorage.GetAsync("orderus_token").Result;
            if(string.IsNullOrEmpty(token))
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new MenuPage();
            }
        }
    }
}
