using Acr.UserDialogs;
using App.DTO;
using AppFrontend.Resources.Files;
using Newtonsoft.Json;
using Plugin.Toast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages.Client
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientReviewPage : ContentPage
    {
        OrderDTO order { get; set; }

        public ClientReviewPage(OrderDTO order)
        {
            InitializeComponent();
            this.order = order;
        }

        private async void SendReviewData(object sender, EventArgs e)
        {
            if(ratingBar.SelectedStarValue == 0)
            {
                CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.StarReviewError);
            }
            else
            {
                var result = await UserDialogs.Instance.ConfirmAsync(new ConfirmConfig
                {
                    Title = ToastDisplayResources.StarReviewTitle,
                    Message = ToastDisplayResources.StarReview,
                    OkText = ToastDisplayResources.PromptYes,
                    CancelText = ToastDisplayResources.PromptCancel
                });

                if(result)
                {
                    var comment = new CommentDTO()
                    {
                        OrderID = order.ID,
                        ClientEmail = order.ClientEmail,
                        CompanyID = order.CompanyID,
                        Date = DateTime.Now,
                        Score = ratingBar.SelectedStarValue,
                        Content = commentEntry.Text
                    };
                    SaveOrderComment(comment);
                }
            }
        }

        private async void SaveOrderComment(CommentDTO comment)
        {
            string url = RestResources.ConnectionURL + RestResources.OrdersURL + RestResources.CommentsURL + RestResources.CreateAccountURL;

            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback += (send, cert, chain, sslPolicyErrors) => true;
            using (HttpClient client = new HttpClient(handler))
            {
                var token = SecureStorage.GetAsync("client_token").Result;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(comment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    CrossToastPopUp.Current.ShowToastSuccess(ToastDisplayResources.StarReviewSuccess);
                    await Navigation.PopAsync();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    CrossToastPopUp.Current.ShowToastError(ToastDisplayResources.StarReviewFail);
                }
            }
        }
    }
}