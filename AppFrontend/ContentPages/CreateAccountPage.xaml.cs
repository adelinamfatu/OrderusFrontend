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
        public CreateAccountPage()
        {
            InitializeComponent();
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