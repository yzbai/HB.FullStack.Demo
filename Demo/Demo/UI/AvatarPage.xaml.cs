using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Controls.Cropper;
using HB.FullStack.XamarinForms.Platforms;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AvatarPage : BaseContentPage
    {
        private const long USER_ID = 123456789;

        private ImageSource? _avatarSource;

        private readonly IFileHelper _fileHelper = DependencyService.Resolve<IFileHelper>();

        public ImageSource? AvatarSource { get => _avatarSource; set { _avatarSource = value; OnPropertyChanged(nameof(AvatarSource)); } }

        public ICommand PickAvatarCommand { get; }

        public AvatarPage()
        {
            InitializeComponent();

            PickAvatarCommand = new AsyncCommand(PickAvatarAsync);

            BindingContext = this;
        }
        protected override IList<IBaseContentView?>? GetAllCustomerControls() => null;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_fileHelper.IsFileExisted(USER_ID.ToString(), UserFileType.Avatar))
            {
                AvatarSource = ImageSource.FromFile(_fileHelper.GetAvatarFilePath(USER_ID));
            }
            else
            {
                AvatarSource = ImageSource.FromFile("bg_test.png");
            }
        }
        private async Task PickAvatarAsync()
        {
            FileResult? fileResult = await MediaPicker.PickPhotoAsync().ConfigureAwait(false);

            if (fileResult == null)
            {
                return;
            }

            CropperPage cropperPage = new CropperPage(fileResult.FullPath, USER_ID.ToString(), UserFileType.Avatar);

            NavigationService.Current.Push(cropperPage);
        }

    }
}