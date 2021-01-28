using HB.FullStack.Mobile.Base;
using HB.FullStack.Mobile.Effects.Touch;
using HB.FullStack.Mobile.Skia;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.CommunityToolkit.Markup;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace Demo.Cropper
{
    public class AvatarCropper : BaseSkiaContentView
    {
        private SKFigureCanvasView _canvas;
        public AvatarCropper()
        {
            Content = new Grid
            {
                RowDefinitions = Rows.Define(GridLength.Star),
                ColumnDefinitions = Columns.Define(GridLength.Star),
                Children = {
                    new SKFigureCanvasView { EnableTimeTick = false }
                        .Bind(SKFigureCanvasView.FiguresProperty, nameof(Figures))
                        .Row(0).Column(0)
                        .Assign(out _canvas)
                }
            }.Invoke(v => v.BindingContext = this);
        }

        public ObservableRangeCollection<SKFigure> Figures { get; private set; } = new ObservableRangeCollection<SKFigure>();

        public override IList<IBaseContentView?>? GetAllCustomerControls() => new List<IBaseContentView?> { _canvas };

        protected override void ReAddFigures()
        {

        }

        protected override void DisposeFigures()
        {

        }
    }

    public class BitmapFigure : SKFigure
    {
        SKBitmap _bitmap;


        public override bool OnHitTest(SKPoint skPoint, long touchId)
        {
            return base.OnHitTest(skPoint, touchId);
        }

        public override void ProcessTouchAction(TouchActionEventArgs args)
        {
            base.ProcessTouchAction(args);
        }

        protected override void OnCaculateOutput()
        {
            throw new NotImplementedException();
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info)
        {
            throw new NotImplementedException();
        }
    }
}