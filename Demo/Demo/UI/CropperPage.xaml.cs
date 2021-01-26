using HB.FullStack.Mobile.Base;

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
    public partial class CropperPage : BaseContentPage
    {
        public CropperPage()
        {
            InitializeComponent();
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            throw new NotImplementedException();
        }
    }
}