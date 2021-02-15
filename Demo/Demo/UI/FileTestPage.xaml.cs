using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Platforms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileTestPage : BaseContentPage
    {
        private IFileHelper _fileHelper = DependencyService.Resolve<IFileHelper>();

        public FileTestPage()
        {
            InitializeComponent();
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls() => null;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            TestImage.Source = ImageSource.FromStream(token => _fileHelper.GetResourceStreamAsync("bg_test", ResourceType.Drawable, null, token));
        }
    }
}