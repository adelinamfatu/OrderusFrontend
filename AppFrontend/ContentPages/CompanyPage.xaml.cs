using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        private double listViewHeight = 0;

        private int listViewNbRows = 0;

        public CompanyPage(CompanyDTO company)
        {
            InitializeComponent();
            this.Company = company;
            BuildingPrompt = company.Building == null ? null : DisplayPrompts.Building;
            StaircasePrompt = company.Staircase is null ? null : DisplayPrompts.Staircase;
            ApartmentPrompt = company.ApartmentNumber is null ? null : DisplayPrompts.Apartment;
            FloorPrompt = company.Floor is null ? null : DisplayPrompts.Floor;
            /*serviceOptions.Add(new CompanyServiceOptionDTO
            {
                Service = new ServiceDTO
                {
                    ID = 1,
                    Name = "Tamplarie"
                },
                Price = 5.2F
            });
            serviceOptions.Add(new CompanyServiceOptionDTO
            {
                Service = new ServiceDTO
                {
                    ID = 2,
                    Name = "Montare"
                },
                Price = 6.5F
            });*/
            RetrieveCompanyDetails(Company.ID);
            RetrieveComments(Company.ID);
            this.BindingContext = this;
        }

        private async void RetrieveCompanyDetails(int companyId)
        {
            string url = RestResources.ConnectionURL + RestResources.CompaniesURL + RestResources.CompanyDetailsURL + companyId;

            using (HttpClient client = new HttpClient())
            {
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

            using (HttpClient client = new HttpClient())
            {
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
                    ClientEmail = comment.ClientEmail,
                    Content = comment.Content,
                    Score = comment.Score
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void CalculateListViewHeight(object sender, EventArgs e)
        {
            
            if (sender is Grid)
            {
                Grid grid = (Grid)sender;

                listViewHeight += grid.Height - 20;
                listViewHeight += grid.Margin.Top;
                listViewHeight += grid.Margin.Bottom;

                if (++listViewNbRows == serviceOptions.Count)
                {
                    Device.BeginInvokeOnMainThread(() => servicesListView.MinimumHeightRequest = listViewHeight);
                }
            }
        }
    }
}