using App.DTO;
using AppFrontend.Resources.Helpers;
using AppFrontend.Resources.Helpers.Strategy;
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
            SetUIServiceInformation();
            this.BindingContext = this;
        }

        private void GetBasketInformation()
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                var basketItemJson = Preferences.Get("BasketItem", "");
                CSO = JsonConvert.DeserializeObject<CompanyServiceOptionDTO>(basketItemJson);
            }
            else
            {
                serviceDetailsFrame.IsVisible = false;
                serviceOrderFrame.IsVisible = false;
            }
        }

        private void RemoveOrderFromBasket(object sender, EventArgs e)
        {
            if (Preferences.ContainsKey("BasketItem"))
            {
                Preferences.Remove("BasketItem");
                CSO = null;
                serviceDetailsFrame.IsVisible = false;
                serviceOrderFrame.IsVisible = false;
            }
        }

        private void SetUIServiceInformation()
        {
            if(CSO.Service.Name == ServiceType.Curatare.ToString())
            {
                surfaceLabel.IsVisible = true;
                surfaceEntry.IsVisible = true;
                noRoomsLabel.IsVisible = true;
                noRoomsEntry.IsVisible = true;
            }
        }

        private void CalculateEstimatedDuration(object sender, EventArgs e)
        {
            if (CSO.Service.Name == ServiceType.Curatare.ToString())
            {
                //CSO.Surface = int.Parse(surfaceEntry.Text);
                //CSO.NbRooms = int.Parse(noRoomsEntry.Text);
                IService service = new CleaningService();
                service.SendServiceData(CSO);
            }
            

        }
    }
}