using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreateAccountPage : ContentPage
    {
        public List<string> StreetTypes { get; set; }

        const string onlyLettersRegex = @"^[a-zA-Z\s]+$";

        const string lettersAndNumbersRegex = @"^[a-zA-Z0-9\s]+$";

        ObservableCollection<string> companies = new ObservableCollection<string>();
        public ObservableCollection<string> Companies { get { return companies; } }

        public Func<string, ICollection<string>, ICollection<string>> SortingAlgorithm
        { get; } = (text, values) => values.Where(x => x.StartsWith(text, StringComparison.CurrentCultureIgnoreCase))
                                            .OrderBy(x => x).ToList();

        public List<CompanyDTO> companiesJSON { get; set; }

        public CreateAccountPage()
        {
            InitializeComponent();
            StreetTypes = new List<string> { "Bd.", "Str.", "Aleea" };
            RetrieveCompanyNames();
            this.BindingContext = this;
        }

        private async void RetrieveCompanyNames()
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    companiesJSON = JsonConvert.DeserializeObject<List<CompanyDTO>>(json);
                    BuildCompanySearch(companiesJSON);
                }
            }
        }

        private void BuildCompanySearch(List<CompanyDTO> companiesJSON)
        {
            foreach (var company in companiesJSON)
            {
                companies.Add(company.Name);
            }
        }

        private void OpenLoginPage(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LoginPage());
        }

        private void CreateAccount(object sender, EventArgs e)
        {
            string selectedOption = createAccountOptionsSC.Children[createAccountOptionsSC.SelectedSegment].Text;
            if (selectedOption == DisplayPrompts.Client)
            {
                VerifyClientAccountDetails();
            }
            else if(selectedOption == DisplayPrompts.Employee)
            {
                VerifyEmployeeAccountDetails();
            }
            else if(selectedOption == DisplayPrompts.Company)
            {
                VerifyCompanyAccountDetails();
            }
        }

        private void VerifyEmployeeAccountDetails()
        {
            if (employeeEmail.Text.Contains("@"))
            {
                if (employeePassword.Text == employeeConfirmPassword.Text)
                {
                    if (employeePhoneNumber.Text.Length == 10)
                    {
                        if(searchComboBox.Text != null)
                        {
                            SaveEmployeeAccount();
                        }
                        else
                        {
                            CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.CompanyChoiceError);
                        }
                    }
                    else
                    {
                        CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.PhoneNumberError);
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

        private void VerifyClientAccountDetails()
        {
            if (clientEmail.Text.Contains("@"))
            {
                if(clientPassword.Text == clientConfirmPassword.Text)
                {
                    if(clientStreetTypePicker.SelectedItem != null)
                    {
                        if(clientPhoneNumber.Text.Length == 10)
                        {
                            SaveClientAccount();
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

        private void VerifyCompanyAccountDetails()
        {
            if(companyEmail.Text.Contains("@"))
            {
                if(companyPassword.Text == companyConfirmPassword.Text)
                {
                    if(companyStreetTypePicker.SelectedItem != null)
                    {
                        if(companyPhoneNumber.Text.Length == 10)
                        {
                            SaveCompanyAccount();
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
                Email = clientEmail.Text,
                Phone = clientPhoneNumber.Text,
                Password = clientPassword.Text,
                Name = char.ToUpper(clientName.Text[0]) + clientName.Text.Substring(1),
                Surname = char.ToUpper(clientSurname.Text[0]) + clientSurname.Text.Substring(1),
                City = clientCity.Text,
                Street = clientStreetTypePicker.SelectedItem + " " + clientStreet.Text,
                StreetNumber = clientStreetNumber.Text,
                Building = !string.IsNullOrEmpty(clientBuilding.Text) ? clientBuilding.Text : null,
                Staircase = !string.IsNullOrEmpty(clientStaircase.Text) ? clientStaircase.Text : null,
                ApartmentNumber = !string.IsNullOrEmpty(clientApartmentNumber.Text) ? int.Parse(clientApartmentNumber.Text) : (int?)null,
                Floor = !string.IsNullOrEmpty(clientFloor.Text) ? int.Parse(clientFloor.Text) : (int?)null
            };
        }

        private CompanyDTO GetCompanyFromUI()
        {
            return new CompanyDTO
            {
                RepresentativeEmail = companyEmail.Text,
                RepresentativePassword = companyPassword.Text,
                RepresentativeName = companyName.Text,
                RepresentativeSurname = companySurname.Text,
                Name = companyTitle.Text,
                City = companyCity.Text,
                Street = companyStreetTypePicker.SelectedItem + companyStreet.Text,
                StreetNumber = companyStreetNumber.Text,
                Building = !string.IsNullOrEmpty(companyBuilding.Text) ? companyBuilding.Text : null,
                Staircase = !string.IsNullOrEmpty(companyStaircase.Text) ? companyStaircase.Text : null,
                ApartmentNumber = !string.IsNullOrEmpty(companyApartmentNumber.Text) ? int.Parse(companyApartmentNumber.Text) : (int?)null,
                Floor = !string.IsNullOrEmpty(companyFloor.Text) ? int.Parse(companyFloor.Text) : (int?)null,
                Site = companySite.Text,
                Description = companyDescription.Text
            };
        }

        private EmployeeDTO GetEmployeeFromUI()
        {
            return new EmployeeDTO
            {
                Email = employeeEmail.Text,
                Phone = employeePhoneNumber.Text,
                Password = employeePassword.Text,
                Name = char.ToUpper(employeeName.Text[0]) + employeeName.Text.Substring(1),
                Surname = char.ToUpper(employeeSurname.Text[0]) + employeeSurname.Text.Substring(1),
                CompanyID = companiesJSON.FirstOrDefault(c => c.Name == searchComboBox.Text).ID,
                IsConfirmed = false
            };
        }

        private async void SaveClientAccount()
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

        private async void SaveCompanyAccount()
        {
            CompanyDTO company = GetCompanyFromUI();
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var json = JsonConvert.SerializeObject(company);
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

        private async void SaveEmployeeAccount()
        {
            EmployeeDTO employee = GetEmployeeFromUI();
            string url = RestResources.ConnectionURL + RestResources.EmployeesURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                var json = JsonConvert.SerializeObject(employee);
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

        private void OnCreateAccountOptionChanged(object sender, SegmentedControl.FormsPlugin.Abstractions.ValueChangedEventArgs e)
        {
            string selectedOption = createAccountOptionsSC.Children[createAccountOptionsSC.SelectedSegment].Text;
            if(selectedOption == DisplayPrompts.Client)
            {
                clientGrid.IsVisible = true;
                employeeGrid.IsVisible = false;
                companyGrid.IsVisible = false;
            }
            else
            {
                if(selectedOption == DisplayPrompts.Employee)
                {
                    employeeGrid.IsVisible = true;
                    clientGrid.IsVisible = false;
                    companyGrid.IsVisible = false;
                }
                else if(selectedOption == DisplayPrompts.Company)
                {
                    companyGrid.IsVisible = true;
                    clientGrid.IsVisible = false;
                    employeeGrid.IsVisible = false;
                }
            }
        }
    }
}