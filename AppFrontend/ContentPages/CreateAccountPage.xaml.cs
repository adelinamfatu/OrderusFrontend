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
    public partial class CreateAccountPage : ContentPage
    {
        public List<string> StreetTypes { get; set; }
        public CreateAccountPage()
        {
            InitializeComponent();
            StreetTypes = new List<string> { "Bd.", "Str.", "Aleea" };
            this.BindingContext = this;
        }

        private void OpenLoginPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LoginPage());
        }

        private void VerifyAccountDetails(object sender, EventArgs e)
        {
            SaveAccount();
        }

        private void SaveAccount()
        {

        }
    }
}