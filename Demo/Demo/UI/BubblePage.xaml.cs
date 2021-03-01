using System;

using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Skia;
using HB.FullStack.XamarinForms.Styles;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace Demo.UI
{
    /// <summary>
    /// 尝试失败，请使用BubblePage2，将SKCanvasView放到AbsoluteLayout里面而不是覆盖
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BubblePage : ContentPage
    {
        private double _length;

        private float _skRadius;

        private Point _centerPoint;
        private View? _curBubbleView;

        private SKPoint? _curHitPoint;

        public BubblePage()
        {
            InitializeComponent();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            _skRadius = Math.Min(info.Width, info.Height) * 3 / 8f;

            _length = SKUtil.ToDp(_skRadius);

            SKPoint center = new SKPoint(info.Width / 2f, info.Height / 2f);

            _centerPoint = SKUtil.ToPoint(center);

            canvas.Clear();

            using SKPaint paint = new SKPaint() { Style = SKPaintStyle.StrokeAndFill, Color = SKColors.Yellow };
            canvas.DrawCircle(center, _skRadius, paint);

            if (_curHitPoint.HasValue)
            {
                using SKPaint dotPaint = new SKPaint() { Style = SKPaintStyle.Stroke, Color = SKColors.Black, StrokeWidth = 10 };
                canvas.DrawCircle(_curHitPoint.Value, 10, dotPaint);
            }
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            Point dpPoint = SKUtil.ToPoint(e.Location);

            if (_curBound.HasValue && _curBound.Value.Contains(dpPoint))
            {
                return;
            }

            DisplayBubblePop(dpPoint);

            _curHitPoint = e.Location;

            CanvasView.InvalidateSurface();
        }

        private View BuildView()
        {
            return new Grid
            {
                RowDefinitions = Rows.Define(Star, Star, Star, Star),
                ColumnDefinitions = Columns.Define(Star, Star, Star, Star),
                Children = {
                    new Label{Text="上午 08:00 ~ 上午 09:00"}.Row(0).Column(0).ColumnSpan(4),
                    new Label{Text = "背诵唐诗宋词" }.Row(1).Column(0).ColumnSpan(4),
                    new Label{Text="进度：第2章"}.Row(2).Column(0).ColumnSpan(4),
                    new ImageButton{
                        Source= new FontImageSource{ FontFamily = Consts.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarEdit },
                        Command = new Command(()=>{ DisplayAlert("ss","xx", "Ok"); })
                    }.Row(3).Column(0),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Consts.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarClock } }.Row(3).Column(1),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Consts.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarMultiple } }.Row(3).Column(2),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Consts.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarQuestion } }.Row(3).Column(3)
                }
            };
        }

        private double _thMargin = 2;
        private Rectangle? _curBound;

        private void DisplayBubblePop(Point point)
        {
            if (_curBubbleView != null)
            {
                ContainerView.Children.Remove(_curBubbleView);
            }

            _curBubbleView = BuildView();

            double x0 = _centerPoint.X;
            double y0 = _centerPoint.Y;

            double x = point.X;
            double y = point.Y;

            double tan = Math.Abs(y - y0) / Math.Abs(x - x0);

            double viewWidth, viewHeight, viewX, viewY;

            if (tan <= Math.Sqrt(3))
            {
                double offset = tan / Math.Sqrt(3) * 3 * _length / 8;

                viewWidth = 0.618 * _length;
                viewHeight = _length;

                if (x < x0)
                {
                    //左侧
                    viewX = x - viewWidth - 2 * _thMargin;

                    if (y < y0)
                    {
                        viewY = y - (viewHeight / 2 - offset) - _thMargin;
                    }
                    else
                    {
                        viewY = y - (viewHeight / 2 + offset) - _thMargin;
                    }
                }
                else
                {
                    //右侧
                    viewX = x;

                    if (y < y0)
                    {
                        viewY = y - (viewHeight / 2 - offset) - _thMargin;
                    }
                    else
                    {
                        viewY = y - (viewHeight / 2 + offset) - _thMargin;
                    }
                }
            }
            else
            {
                double offset = 1 / tan / Math.Sqrt(3) * 3 * _length / 8;

                viewWidth = _length;
                viewHeight = _length * 0.618;

                if (y < y0)
                {
                    //上测
                    viewY = y - viewHeight - 2 * _thMargin;

                    if (x < x0)
                    {
                        viewX = x - (viewWidth / 2 - offset) - _thMargin;
                    }
                    else
                    {
                        viewX = x - (viewWidth / 2 + offset) - +_thMargin;
                    }
                }
                else
                {
                    //下侧
                    viewY = y;

                    if (x < x0)
                    {
                        viewX = x - (viewWidth / 2 - offset) - _thMargin;
                    }
                    else
                    {
                        viewX = x - (viewWidth / 2 + offset) - +_thMargin;
                    }
                }
            }

            _curBubbleView.WidthRequest = viewWidth;
            _curBubbleView.HeightRequest = viewHeight;

            ContainerView.Children.Add(_curBubbleView);

            //检查是否溢出屏幕
            if (viewX < 0)
            {
                viewX = 0;
            }
            else if (viewX + viewWidth > ContainerView.Width)
            {
                viewX = ContainerView.Width - viewWidth;
            }

            if (viewY < 0)
            {
                viewY = 0;
            }
            else if (viewY + viewHeight > ContainerView.Height)
            {
                viewY = ContainerView.Height - viewHeight;
            }

            _curBound = new Rectangle(viewX, viewY, viewWidth, viewHeight);

            AbsoluteLayout.SetLayoutBounds(_curBubbleView, _curBound.Value);
        }
    }
}