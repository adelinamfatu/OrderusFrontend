using Acr.UserDialogs;
using AppFrontend.Resources.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyMenuPage : Xamarin.Forms.TabbedPage
    {
        public CompanyMenuPage()
        {
            InitializeComponent();
            TabStyling();
        }

        public void TabStyling()
        {
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            this.BarBackgroundColor = Color.FromHex("#6ba1f9");
            this.SelectedTabColor = Color.White;
            this.UnselectedTabColor = Color.FromHex("#667284");
        }

        private async void LogoutAccount(object sender, EventArgs e)
        {
            var result = await DisplayConfirmPopUp();
            if (result == true)
            {
                SecureStorage.Remove("company_token");
                Xamarin.Forms.Application.Current.MainPage = new LoginPage();
            }
        }

        private async Task<bool> DisplayConfirmPopUp()
        {
            return await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.LogoutConfirmationTitle,
                Message = ToastDisplayResources.LogoutConfirmation,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });
        }
    }
}