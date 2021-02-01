using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using HB.FullStack.Mobile.Base;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AvatarPage : BaseContentPage
    {
        private string _avatarName;

        public string AvatarName { get => _avatarName; set { _avatarName = value; } }

        public ICommand PickAvatarCommand { get; }

        public AvatarPage()
        {
            InitializeComponent();
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        => new List<IBaseContentView?> { };
    }
}