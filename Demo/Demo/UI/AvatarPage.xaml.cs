using System.Collections.Generic;
using System.IO;
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
        private ImageSource? _avatarSource;

        public ImageSource? AvatarSource { get => _avatarSource; set { _avatarSource = value; OnPropertyChanged(nameof(AvatarSource)); } }

        public ICommand PickAvatarCommand { get; }

        private string? _croppedFullPath;

        private string _defaultAvatarPath = "bg_test.png";

        public AvatarPage()
        {
            InitializeComponent();

            PickAvatarCommand = new AsyncCommand(PickAvatarAsync);

            BindingContext = this;

            _croppedFullPath = Path.Combine(FileSystem.AppDataDirectory, "avatar", "testCropped.xx");
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls() => null;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (File.Exists(_croppedFullPath))
            {
                AvatarSource = ImageSource.FromFile(_croppedFullPath);
            }
            else
            {
                AvatarSource = ImageSource.FromFile(_defaultAvatarPath);
            }
        }

        private async Task PickAvatarAsync()
        {
            FileResult? fileResult = await MediaPicker.PickPhotoAsync().ConfigureAwait(false);

            if (fileResult == null)
            {
                return;
            }

            CropperPage cropperPage = new CropperPage(fileResult.FullPath, _croppedFullPath, _=> { });

            NavigationService.Current.Push(cropperPage);
        }
    }
}