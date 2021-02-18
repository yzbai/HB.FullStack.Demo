using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    public class UserInfo
    {
        public string? Name { get; set; }

        public int Age { get; set; }
    }

    /// <summary>
    /// 测试Live Data
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RepoTestPage : ContentPage
    {

        public string TestField = "Can`t Binding Field";

        public string TestProperty { get; set; } = "Only Can Binding Property";

        public string Name { get; set; } = "Name";

        private ObservableTask<UserInfo>? _userLive;

        public ObservableTask<UserInfo>? UserLive
        {
            get => _userLive;
            set { _userLive = value; OnPropertyChanged(nameof(UserLive)); }
        }

        public RepoTestPage()
        {
            InitializeComponent();

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            UserLive = GetUser();

            base.OnAppearing();
        }

        private ObservableTask<UserInfo>? GetUser()
        {
            UserInfo initialInfo = new UserInfo { Name = "Get From Local", Age = -100 };

            return new ObservableTask<UserInfo>(async () =>
            {
                await Task.Delay(3000);

                return new UserInfo { Name = "Get From Remote", Age = 99 };

            }, initialInfo);
        }
    }
}