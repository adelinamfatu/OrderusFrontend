using AppFrontend.ContentPages;
using AppFrontend.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            MainPage = new LoginPage();
        }
    }
}
