using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Controls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageTestPage : BaseContentPage
    {
        public ImageTestPage()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public AuthUriImageSource AuthImageSource { get; } = new AuthUriImageSource { Uri = new Uri( "http://brlite.com/qrcode.jpg") };

        public string ImageUrl { get; } = "http://brlite.com/qrcode.jpg";

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return null;
        }
    }
}