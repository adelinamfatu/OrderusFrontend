using App.DTO;
using AppFrontend.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public ClientDTO Client { get; set; }

        public ProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            Client = globalService.Client;
            this.BindingContext = this;
        }
    }
}