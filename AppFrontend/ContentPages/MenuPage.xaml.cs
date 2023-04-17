using System;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : Xamarin.Forms.TabbedPage
    {
        public MenuPage()
        {
            InitializeComponent();

            TabStyling();
        }

        public void TabStyling()
        {
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            this.BarBackgroundColor = Color.FromHex("#26364d");
            this.SelectedTabColor = Color.White;
            this.UnselectedTabColor = Color.Gray;
        }

        private void LogoutAccount(object sender, EventArgs e)
        {
            SecureStorage.Remove("orderus_token");
            Application.Current.MainPage = new LoginPage();
        }
    }
}