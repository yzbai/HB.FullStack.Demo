using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Base;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZipTestPage : ContentPage
    {
        public ZipTestPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            using Stream stream = await FileSystem.OpenAppPackageFileAsync("InitDatas.zip");

            await BaseApplication.PlatformHelper.UnZipAsync(stream, Path.Combine(FileSystem.AppDataDirectory));

            string testFullPath = Path.Combine(FileSystem.AppDataDirectory, "Default", "Sub", "Painters.png");

            TestImage.Source = ImageSource.FromFile(testFullPath);
        }
    }
}