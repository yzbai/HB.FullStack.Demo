using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Skia;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaTouchTestPage : BaseContentPage
    {
        private RectangleFigure? _rectangelFigure1;
        private RectangleFigure? _rectangelFigure2;
        public SkiaTouchTestPage()
        {
            InitializeComponent();

            FigureCanvasView2.EnableTouchEventPropagation = true;
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return new IBaseContentView?[] { };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            SKRect rect1 = new SKRect(-200, -200, 200, 200);
            SKRect rect2 = new SKRect(200, 200, 300, 300);

            _rectangelFigure1 = new RectangleFigure(rect1, SKColors.Red);
            _rectangelFigure2 = new RectangleFigure(rect2, SKColors.Blue);


            FigureCanvasView.Figures = new ObservableCollection<SKFigure> { _rectangelFigure1, _rectangelFigure2 };
            //FigureCanvasView.Figures.Add(_rectangelFigure1);


            //FigureCanvasView.Figures.Add(_rectangelFigure2);
            //FigureCanvasView2.Figures.Add(_rectangelFigure2);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Remove Figures
            FigureCanvasView.Figures.Clear();
            //FigureCanvasView2.Figures.Clear();

            _rectangelFigure1?.Dispose();
            _rectangelFigure2?.Dispose();
        }
    }

    public class RectangleFigure : SKFigure
    {
        private readonly SKRect _rect;
        private readonly SKColor _color;

        public RectangleFigure(SKRect initRect, SKColor color)
        {
            OneFingerDragged += RectangleFigure_OneFingerDragged;
            _rect = initRect;
            _color = color;
        }

        private void RectangleFigure_OneFingerDragged(object sender, SKFigureTouchInfo e)
        {
            SKMatrix transMatrix = SKMatrix.CreateTranslation(e.CurrentPoint.X - e.PreviousPoint.X, e.CurrentPoint.Y - e.PreviousPoint.Y);

            Matrix = Matrix.PostConcat(transMatrix);
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {
            using SKPaint paint = new SKPaint { Style = SKPaintStyle.Fill, Color = _color };
            canvas.DrawRect(_rect, paint);
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info)
        {
            if (CanvasSizeChanged)
            {
                HitTestPath = new SKPath();
                HitTestPath.AddRect(_rect);
            }
        }
    }
}