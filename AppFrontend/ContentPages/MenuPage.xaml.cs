using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace AppFrontend.ContentPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : Xamarin.Forms.TabbedPage
    {
        public MenuPage()
        {
            InitializeComponent();

            TabStyling();
        }

        public void TabStyling()
        {
            On<Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            this.BarBackgroundColor = Color.FromHex("#26364d");
            this.SelectedTabColor = Color.White;
            this.UnselectedTabColor = Color.Gray;
        }
    }
}