using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources;
using AppFrontend.Resources.Files;
using AppFrontend.Resources.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyPage : ContentPage, INotifyPropertyChanged
    {
        public CompanyDTO Company { get; set; }

        ObservableCollection<CompanyServiceOptionDTO> serviceOptions = new ObservableCollection<CompanyServiceOptionDTO>();

        public ObservableCollection<CompanyServiceOptionDTO> ServiceOptions { get { return serviceOptions; } }

        ObservableCollection<CommentDTO> comments = new ObservableCollection<CommentDTO>();

        public ObservableCollection<CommentDTO> Comments { get { return comments; } }

        public string BuildingPrompt { get; set; }

        public string StaircasePrompt { get; set; }

        public string ApartmentPrompt { get; set; }

        public string FloorPrompt { get; set; }

        private GlobalService globalService { get; set; }

        public CompanyPage(CompanyDTO company)
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            this.Company = company;
            BuildingPrompt = company.Building == null ? null : DisplayPrompts.Building;
            StaircasePrompt = company.Staircase is null ? null : DisplayPrompts.Staircase;
            ApartmentPrompt = company.ApartmentNumber is null ? null : DisplayPrompts.Apartment;
            FloorPrompt = company.Floor is null ? null : DisplayPrompts.Floor;
            RetrieveCompanyDetails(Company.ID);
            RetrieveComments(Company.ID);
            this.BindingContext = this;
        }

        private async void RetrieveCompanyDetails(int companyId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.CompanyDetailsURL + companyId;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<CompanyServiceOptionDTO> csoJSON = JsonConvert.DeserializeObject<List<CompanyServiceOptionDTO>>(json);
                    BuildCompanyDetails(csoJSON);
                }
            }
        }

        private void BuildCompanyDetails(List<CompanyServiceOptionDTO> csoJSON)
        {
            foreach (var cso in csoJSON)
            {
                serviceOptions.Add(new CompanyServiceOptionDTO
                {
                    Service = cso.Service,
                    Price = cso.Price
                });
            }
        }

        private async void RetrieveComments(int companyId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.CommentsURL + companyId;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<CommentDTO> commentsJSON = JsonConvert.DeserializeObject<List<CommentDTO>>(json);
                    BuildComments(commentsJSON);
                }
            }
        }

        private void BuildComments(List<CommentDTO> commentsJSON)
        {
            foreach (var comment in commentsJSON)
            {
                comments.Add(new CommentDTO
                {
                    ClientName = comment.ClientName,
                    Content = comment.Content,
                    Score = comment.Score
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void ServiceSelectedEvent(object sender, ItemTappedEventArgs e)
        {
            var service = (CompanyServiceOptionDTO)e.Item;
            service.Company = Company;
            var message = $"{string.Format(ToastDisplayResources.ServiceSelected, service.Service.Name)}";

            var response = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
            {
                Title = ToastDisplayResources.ServiceSelectedTitle,
                Message = message,
                OkText = ToastDisplayResources.PromptYes,
                CancelText = ToastDisplayResources.PromptCancel
            });

            if(response)
            {
                Preferences.Set("BasketItem", JsonConvert.SerializeObject(service));
                var basketItemMessage = new BasketItemMessage { CSO = service };
                MessagingCenter.Send(this, "BasketItemMessage", basketItemMessage);
            }
        }
    }
}