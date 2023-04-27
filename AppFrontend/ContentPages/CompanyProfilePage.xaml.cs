using AppFrontend.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompanyProfilePage : ContentPage
    {
        public GlobalService globalService { get; set; }

        public CompanyProfilePage()
        {
            InitializeComponent();
            globalService = DependencyService.Get<GlobalService>();
            globalService.PropertyChanged += GlobalService_PropertyChanged;
        }

        private void GlobalService_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Company")
            {
                this.BindingContext = globalService.Company;
            }
        }

        private void ChangePage(object sender, CheckedChangedEventArgs e)
        {
            if(page2Button.IsChecked == true)
            {
                name.IsVisible = false;
                city.IsVisible = false;
                street.IsVisible = false;
                streetNumber.IsVisible = false;
                building.IsVisible = false;
                staircase.IsVisible = false;
                apartmentNumber.IsVisible = false;
                functionEntryGrid.IsVisible = true;
                functionsUniformGrid.IsVisible = true;
            }
            else
            {
                name.IsVisible = true;
                city.IsVisible = true;
                street.IsVisible = true;
                streetNumber.IsVisible = true;
                building.IsVisible = true;
                staircase.IsVisible = true;
                apartmentNumber.IsVisible = true;
                functionEntryGrid.IsVisible = false;
                functionsUniformGrid.IsVisible = false;
            }
        }

        private void AddFunctionToList(object sender, EventArgs e)
        {
            if (globalService.Company.Functions == null)
            {
                globalService.Company.Functions = new List<string>();
            }
            globalService.Company.Functions.Add(function.Text);
        }
    }
}