using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XctTouchTestPage : ContentPage
    {
        public ICommand BgTapCommand { get; set; }
        public XctTouchTestPage()
        {
            InitializeComponent();

            BgTapCommand = new Command(async () => {

                await this.DisplayToastAsync("Background Clicked");

            });

            BindingContext = this;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Button", "Clicked", "Ok");
        }
    }
}