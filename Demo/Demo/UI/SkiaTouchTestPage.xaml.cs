using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Base;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaTouchTestPage : BaseContentPage
    {
        public SkiaTouchTestPage()
        {
            InitializeComponent();
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return new IBaseContentView?[] { };
        }
    }
}