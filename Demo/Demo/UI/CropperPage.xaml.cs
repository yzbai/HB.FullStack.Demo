using HB.FullStack.Mobile.Base;
using HB.FullStack.Mobile.Skia;

using SkiaSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CropperPage : BaseContentPage
    {
        public ObservableRangeCollection<SKFigure> Figures { get; } = new ObservableRangeCollection<SKFigure>();

        public CropperPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            BitmapFigure bitmapFigure = new BitmapFigure();

            Figures.Add(bitmapFigure);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            foreach (SKFigure f in Figures)
            {
                f.Dispose();
            }

            Figures.Clear();
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls() => new List<IBaseContentView?> { FigureCanvas };
    }

    public class BitmapFigure : SKFigure
    {
        private readonly float _widthRatio;
        private readonly float _heightRatio;

        private SKBitmap _bitmap;

        public BitmapFigure(float widthRatio, float heightRatio)
        {
            _widthRatio = widthRatio;
            _heightRatio = heightRatio;
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {


            SKRect destRect = SKRect.Create()

            canvas.DrawBitmap(_bitmap, );
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info)
        {
            base.OnUpdateHitTestPath(info);
        }

        #region Dispose Pattern

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_disposed)
            {
                if (disposing)
                {
                    // managed
                    _bitmap?.Dispose();
                }

                //unmanaged

                _disposed = true;
            }
        }

        #endregion Dispose Pattern
    }
}