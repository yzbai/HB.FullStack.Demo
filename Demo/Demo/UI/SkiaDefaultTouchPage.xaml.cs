using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaDefaultTouchPage : ContentPage
    {
        public SkiaDefaultTouchPage()
        {
            InitializeComponent();
        }

        private async void View1_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            await this.DisplayToastAsync("View1_Touch");
        }

        private void View1_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {

        }

        private async void View2_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            await this.DisplayPromptAsync("View2_Touch","xx");
        }

        private void View2_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {

        }
    }
}