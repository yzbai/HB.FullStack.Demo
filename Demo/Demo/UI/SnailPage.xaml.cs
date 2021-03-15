using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SnailPage : ContentPage
    {
        public SnailPage()
        {
            InitializeComponent();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
{
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            canvas.Translate(info.Width / 2f, info.Height / 2f);
            //canvas.RotateDegrees(-90);

            float maxRadius = Math.Min(info.Width, info.Height) / 2f;

            float initRadius = 50;

            float gapRadius = maxRadius - initRadius;

            SKPoint initPoint = new SKPoint(0,0);

            using (SKPath path = new SKPath())
            {
                int count = 0;
                for (float angle = 0; angle < 360 * 3; angle += 1, count++)
                {
                    float curRaius = initRadius + gapRadius  / (36*30)  * count;
                    double radians = Math.PI * angle / 180;
                    float x = curRaius * (float)Math.Cos(radians);
                    float y = curRaius * (float)Math.Sin(radians);
                    
                    SKPoint point = new SKPoint(x, y);

                    if (angle == 0)
                    {
                        initPoint = point;
                        path.MoveTo(point);
                    }
                    else
                    {
                        path.LineTo(point);
                    }
                }

                path.LineTo(initPoint);

                SKPaint paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Red,
                    StrokeWidth = 5
                };

                canvas.DrawPath(path, paint);

                SKPaint paint2 = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Blue,
                    StrokeWidth = 20
                };

                canvas.DrawPoint(0, 0, paint2);
            }



        }
    }
}