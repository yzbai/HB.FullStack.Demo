using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Platforms;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Svg;
using Svg.Skia;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SvgTestPage : ContentPage
    {
        private SKSvg? _svg;

        public SvgTestPage()
        {
            InitializeComponent();
        }

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;

            canvas.Clear();

            if (_svg != null)
            {
                canvas.DrawPicture(_svg.Picture);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            _svg?.Dispose();

            using Stream stream = await FileSystem.OpenAppPackageFileAsync("Test.svg").ConfigureAwait(false);

            _svg = new SKSvg();
            _svg.Load(stream);

            CanvasView.InvalidateSurface();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _svg?.Dispose();
        }
    }
}