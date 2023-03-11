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
    public partial class SearchCompanyPage : ContentPage
    {
        public SearchCompanyPage(int serviceId)
        {
            InitializeComponent();
            this.BindingContext = this;
        }
    }
}