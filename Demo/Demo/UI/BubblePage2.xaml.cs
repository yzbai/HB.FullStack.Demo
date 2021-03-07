using System;

using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Skia;
using HB.FullStack.XamarinForms.Styles;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;

using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

using Rectangle = Xamarin.Forms.Rectangle;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BubblePage2 : ContentPage
    {
        private double _length;

        private float _skRadius;

        private Point _centerPoint;
        private View? _curBubbleView;

        private Rectangle? _curBubbleViewBound;
        double _padding = 10;

        public BubblePage2()
        {
            InitializeComponent();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            _skRadius = Math.Min(info.Width, info.Height) * 3 / 8f;

            _length = SKUtil.ToDp(_skRadius) * 4 / 3;

            SKPoint center = new SKPoint(info.Width / 2f, info.Height / 2f);

            _centerPoint = SKUtil.ToPoint(center);


            canvas.Clear();

            using SKPaint paint = new SKPaint() { Style = SKPaintStyle.StrokeAndFill, Color = SKColors.Yellow };
            canvas.DrawCircle(center, _skRadius, paint);
        }

        private void CanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            Point dpPoint = SKUtil.ToPoint(e.Location);

            if (_curBubbleViewBound.HasValue && _curBubbleViewBound.Value.Contains(dpPoint))
            {
                return;
            }

            DisplayBubbleView(dpPoint);

            CanvasView.InvalidateSurface();
        }

        private void DisplayBubbleView(Point occurPoint)
        {
            if (_curBubbleView != null)
            {
                ContainerView.Children.Remove(_curBubbleView);
            }

            _curBubbleViewBound = GetBubbleViewBound(_centerPoint, ref occurPoint, _length, 0.622 * _length, ContainerView.Width, ContainerView.Height);

            _curBubbleView = BuildBubbleView(_padding);
            _curBubbleView.BackgroundColor = Color.Blue;
            _curBubbleView.WidthRequest = _curBubbleViewBound.Value.Width;
            _curBubbleView.HeightRequest = _curBubbleViewBound.Value.Height;
            _curBubbleView.Clip = GetBubbleViewClip(_curBubbleViewBound.Value, occurPoint, _padding);

            AbsoluteLayout.SetLayoutBounds(_curBubbleView, _curBubbleViewBound.Value);

            ContainerView.Children.Add(_curBubbleView);
        }

        private View BuildBubbleView(double padding)
        {
            return new Grid
            {
                Padding = new Thickness(padding),
                RowDefinitions = Rows.Define(Star, Star, Star, Star),
                ColumnDefinitions = Columns.Define(Star, Star, Star, Star),
                Children = {
                    new Label{Text="上午 08:00 ~ 上午 09:00"}.Row(0).Column(0).ColumnSpan(4),
                    new Label{Text = "背诵唐诗宋词" }.Row(1).Column(0).ColumnSpan(4),
                    new Label{Text="进度：第2章"}.Row(2).Column(0).ColumnSpan(4),
                    new ImageButton{
                        Source= new FontImageSource{ FontFamily = Conventions.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarEdit },
                        Command = new Command(()=>{ DisplayAlert("ss","xx", "Ok"); })
                    }.Row(3).Column(0),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Conventions.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarClock } }.Row(3).Column(1),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Conventions.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarMultiple } }.Row(3).Column(2),
                    new ImageButton{Source= new FontImageSource{ FontFamily = Conventions.Material_Icon_Font_Family, Glyph = MaterialFont.CalendarQuestion } }.Row(3).Column(3)
                }
            };
        }


        /// <summary>
        /// 得到Bubble的剪切轮廓
        /// </summary>
        /// <param name="curBound"></param>
        /// <param name="occurPoint"></param>
        /// <param name="margin"></param>
        /// <returns></returns>
        private static Geometry? GetBubbleViewClip(Rectangle curBound, Point occurPoint, double margin)
        {
            //小三角形
            PathFigure tranglePathFigure;
            double x, y; //小三角形的顶点

            if (Math.Abs(occurPoint.X - curBound.X) < 0.1)
            {
                //左侧<
                x = 0;
                y = occurPoint.Y - curBound.Y;

                tranglePathFigure = new PathFigure { IsClosed = true, StartPoint = new Point(x, y) };
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x + margin, y - margin)));
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x + margin, y + margin)));
            }
            else if (Math.Abs(occurPoint.X - (curBound.X + curBound.Width)) < 0.1)
            {
                //右侧>
                x = curBound.Width;
                y = occurPoint.Y - curBound.Y;

                tranglePathFigure = new PathFigure { IsClosed = true, StartPoint = new Point(x, y) };
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x - margin, y - margin)));
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x - margin, y + margin)));
            }
            else if (Math.Abs(occurPoint.Y - curBound.Y) < 0.1)
            {
                //上测^
                x = occurPoint.X - curBound.X;
                y = 0;

                tranglePathFigure = new PathFigure { IsClosed = true, StartPoint = new Point(x, y) };
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x - margin, y + margin)));
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x + margin, y + margin)));
            }
            else
            {
                //下侧v
                x = occurPoint.X - curBound.X;
                y = curBound.Height;

                tranglePathFigure = new PathFigure { IsClosed = true, StartPoint = new Point(x, y) };
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x - margin, y - margin)));
                tranglePathFigure.Segments.Add(new LineSegment(new Point(x + margin, y - margin)));
            }


            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(tranglePathFigure);

            //圆角矩形
            Rect rect = new Rect(margin, margin, curBound.Width - 2 * margin, curBound.Height - 2 * margin);

            RoundRectangleGeometry roundRectangleGeometry = new RoundRectangleGeometry(5, rect);

            roundRectangleGeometry.Children.Add(pathGeometry);

            return roundRectangleGeometry;
        }

        /// <summary>
        /// 计算得到Pop的外边框,小三角形包含在边框内
        /// </summary>
        /// <param name="centerPoint">中心点</param>
        /// <param name="occurpoint">点击的地方，即气泡小尖尖对准的地方, 会因为溢出检测而调整</param>
        /// <param name="viewLength">外边框的长边</param>
        /// <param name="viewWidthHeightRatio">外边框的短边</param>
        /// <param name="containerWidth">容器宽度</param>
        /// <param name="containerHeight">容器高度</param>
        /// <returns></returns>
        private static Rectangle GetBubbleViewBound(Point centerPoint, ref Point occurpoint, double viewLongSideLength, double viewShortSideLength, double containerWidth, double containerHeight)
        {
            double x0 = centerPoint.X;
            double y0 = centerPoint.Y;

            double occurX = occurpoint.X;
            double occurY = occurpoint.Y;

            double tan = Math.Abs(occurY - y0) / Math.Abs(occurX - x0);

            double viewX, viewY, viewWidth, viewHeight;

            if (tan <= Math.Sqrt(3))
            {
                double offset = tan / Math.Sqrt(3) * 3 * viewLongSideLength / 8;

                viewWidth = viewShortSideLength;
                viewHeight = viewLongSideLength;

                if( (occurX < x0 && occurX - viewWidth >= 0) || (occurX>= x0 && occurX + viewWidth > containerWidth))
                {
                    //左侧
                    viewX = occurX - viewWidth;
                }
                else
                {
                    //右侧
                    viewX = occurX;
                }

                if (occurY < y0)
                {
                    viewY = occurY - (viewHeight / 2 - offset);
                }
                else
                {
                    viewY = occurY - (viewHeight / 2 + offset);
                }
            }
            else
            {
                double offset = 1 / tan / Math.Sqrt(3) * 3 * viewLongSideLength / 8;

                viewWidth = viewLongSideLength;
                viewHeight = viewShortSideLength;

                if ((occurY < y0 && occurY - viewHeight >= 0) || (occurY >= y0 && occurY + viewHeight > containerHeight))
                {
                    //上侧
                    viewY = occurY - viewHeight;
                }
                else
                {
                    //下侧
                    viewY = occurY;
                }

                if (occurX < x0)
                {
                    viewX = occurX - (viewWidth / 2 - offset);
                }
                else
                {
                    viewX = occurX - (viewWidth / 2 + offset);
                }
            }
           
            return new Rectangle(viewX, viewY, viewWidth, viewHeight);
        }
    }
}