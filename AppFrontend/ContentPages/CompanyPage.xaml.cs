using App.DTO;
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
    public partial class CompanyPage : ContentPage
    {
        private CompanyDTO company { get; set; }

        public CompanyPage(CompanyDTO company)
        {
            InitializeComponent();
            this.BindingContext = this;
            this.company = company;
        }
    }
}