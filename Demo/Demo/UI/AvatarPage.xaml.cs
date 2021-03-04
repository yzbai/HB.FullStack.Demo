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

        public AvatarPage()
        {
            InitializeComponent();

            PickAvatarCommand = new AsyncCommand(PickAvatarAsync);

            BindingContext = this;

            _croppedFullPath = 
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls() => null;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (_fileHelper.IsFileExisted(USER_ID.ToString(), UserFileType.Avatar))
            {
                AvatarSource = ImageSource.FromFile(_fileHelper.GetAvatarFullPath(USER_ID));
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

            string avatarFullPath = Path.Combine(_fileHelper.GetDirectoryPath(UserFileType.Avatar), USER_ID.ToString());

            CropperPage cropperPage = new CropperPage(fileResult.FullPath, avatarFullPath);

            NavigationService.Current.Push(cropperPage);
        }
    }
}