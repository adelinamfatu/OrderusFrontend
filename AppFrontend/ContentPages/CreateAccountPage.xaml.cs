using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccountPage : ContentPage
    {
        public List<string> StreetTypes { get; set; }

        const string onlyLettersRegex = @"^[a-zA-Z]+$";

        const string lettersAndNumbersRegex = @"^[a-zA-Z0-9]+$";

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

        private void CreateAccount(object sender, EventArgs e)
        {
            VerifyAccountDetails();
        }

        private void VerifyAccountDetails()
        {
            if (email.Text.Contains("@"))
            {
                if(password.Text == confirmPassword.Text)
                {
                    if(streetTypePicker.SelectedItem != null)
                    {
                        if(phoneNumber.Text.Length == 10)
                        {
                            SaveAccount();
                        }
                        else
                        {
                            CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.PhoneNumberError);
                        }
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.StreetTypeError);
                    }
                }
                else
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.PasswordError);
                }
            }
            else
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.EmailError);
            }
        }

        private ClientDTO GetClientFromUI()
        {
            return new ClientDTO
            {
                Email = email.Text,
                Phone = phoneNumber.Text,
                Password = password.Text,
                Name = char.ToUpper(name.Text[0]) + name.Text.Substring(1),
                Surname = char.ToUpper(surname.Text[0]) + surname.Text.Substring(1),
                City = city.Text,
                Street = streetTypePicker.SelectedItem + street.Text,
                StreetNumber = streetNumber.Text,
                Building = !string.IsNullOrEmpty(building.Text) ? building.Text : null,
                Staircase = !string.IsNullOrEmpty(staircase.Text) ? staircase.Text : null,
                ApartmentNumber = !string.IsNullOrEmpty(apartmentNumber.Text) ? int.Parse(apartmentNumber.Text) : (int?)null,
                Floor = !string.IsNullOrEmpty(floor.Text) ? int.Parse(floor.Text) : (int?)null
            };
        }

        private async void SaveAccount()
        {
            ClientDTO client = GetClientFromUI();
            string url = RestResources.ConnectionURL + RestResources.ClientsURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var json = JsonConvert.SerializeObject(client);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.CreateAccountSuccess);
                }
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.CreateAccountError);
                }
            }
        }

        private void ValidateTextInput(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                var IsValid = Regex.IsMatch(e.NewTextValue, onlyLettersRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                ((Entry)sender).Text = IsValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
            }
        }

        private void ValidateTextAndNumbersInput(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.NewTextValue))
            {
                var IsValid = Regex.IsMatch(e.NewTextValue, lettersAndNumbersRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                ((Entry)sender).Text = IsValid ? e.NewTextValue : e.NewTextValue.Remove(e.NewTextValue.Length - 1);
            }
        }
    }
}