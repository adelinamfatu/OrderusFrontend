using App.DTO;
using AppFrontend.Resources.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public BasketPage()
        {
            InitializeComponent();
            GetBasketInformation();
            MessagingCenter.Subscribe<CompanyPage, BasketItemMessage>(this, "BasketItemMessage", (sender, message) =>
            {
                CSO = message.CSO;
            });
            this.BindingContext = this;
        }

        private void GetBasketInformation()
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                var basketItemJson = Preferences.Get("BasketItem", "");
                CSO = JsonConvert.DeserializeObject<CompanyServiceOptionDTO>(basketItemJson);
            }
        }
    }
}