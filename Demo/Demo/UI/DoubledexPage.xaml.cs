using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.Common;
using HB.FullStack.XamarinForms;

using SkiaSharp;
using SkiaSharp.Extended;
using SkiaSharp.Views.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DoubledexPage : ContentPage
    {

        private const int DISPLAY_HOURS = 26;

        private const float MAX_RADIUS_RATIO = 1f;

        private const float INIT_RADIUS = 100f;

        private float _initRadius;
        private float _maxRadius;
        private float _radiusGap;
        private int _fullRoudCount;
        private int _extraHours;
        private bool _first = true;

        public DoubledexPage()
        {
            InitializeComponent();
        }

        private void CanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            using SKPaint paint2 = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
                StrokeWidth = 20
            };

            if (_first)
            {

                _initRadius = INIT_RADIUS;
                _maxRadius = Math.Min(info.Width, info.Height) / 2f * MAX_RADIUS_RATIO;

                int displayHours = DISPLAY_HOURS;

                //转几整圈
                _fullRoudCount = 1 + (int)Math.Floor(displayHours / 12f);

                //整圈后剩下的小时
                _extraHours = displayHours % 12;

                //每环之间等距
                _radiusGap = (_maxRadius - _initRadius) / (_extraHours <= 2 ? _fullRoudCount - 1 : _fullRoudCount); //只是多出来两个多小时的话，不影响


                _first = false;
            }

            canvas.Clear();
            canvas.Translate(info.Width / 2f, info.Height / 2f);



            List<(Time24Hour, Time24Hour)> blocks = new List<(Time24Hour, Time24Hour)>
            {
                (new Time24Hour(0,0), new Time24Hour(3,50)),
                (new Time24Hour(4, 0), new Time24Hour(6, 15)),
                (new Time24Hour(6, 30), new Time24Hour(6, 55)),
                (new Time24Hour(6, 55), new Time24Hour(7, 0)),
                (new Time24Hour(7, 0), new Time24Hour(7, 45)),
                (new Time24Hour(8, 0), new Time24Hour(10, 55)),
                (new Time24Hour(11, 30), new Time24Hour(12, 30)),
                (new Time24Hour(13, 0), new Time24Hour(18, 0)),
                (new Time24Hour(20, 0), new Time24Hour(20, 30)),
                (new Time24Hour(21, 0), new Time24Hour(24, 0)),
            };


            foreach (var block in blocks)
            {
                paint2.Color = ColorUtil.RandomColor().Color.ToSKColor();
                
                var tbPaths = GetTimeBlockPath(block.Item1, block.Item2);

                foreach (SKPath path in tbPaths)
                {
                    canvas.DrawPath(path, paint2);
                    path.Dispose();
                }
            }

            //Snail
            DrawDial(canvas);

            // Center
            canvas.DrawPoint(0, 0, paint2);

        }

        private List<SKPath> GetTimeBlockPath(Time24Hour startTime, Time24Hour endTime)
        {
            float start = GetTimePercent(startTime);

            float end = GetTimePercent(endTime);

            //Same Ring
            int startRing = (int)Math.Floor(start);
            int endRing = (int)Math.Floor(end) == end ? (int)(end - 1) : (int)Math.Floor(end);

            float innerRadius, outterRadius;

            if (startRing == endRing)
            {
                innerRadius = GetRadiusByRingCount(startRing);
                outterRadius = GetRadiusByRingCount(startRing + 1);

                return new List<SKPath> { SKGeometry.CreateSectorPath(start, end, outterRadius, innerRadius) };
            }

            List<SKPath> paths = new List<SKPath>();

            //先画完当前ring
            innerRadius = GetRadiusByRingCount(startRing);
            outterRadius = GetRadiusByRingCount(startRing + 1);

            paths.Add(SKGeometry.CreateSectorPath(start, startRing + 1, outterRadius, innerRadius));

            //画整圈

            int curRing = startRing + 1;

            while (curRing < endRing)
            {
                innerRadius = GetRadiusByRingCount(curRing);
                outterRadius = GetRadiusByRingCount(curRing + 1);
                paths.Add(SKGeometry.CreateSectorPath(curRing, curRing + 1, outterRadius, innerRadius));

                curRing++;
            }

            //画剩下的
            innerRadius = GetRadiusByRingCount(endRing);
            outterRadius = GetRadiusByRingCount(endRing + 1);
            paths.Add(SKGeometry.CreateSectorPath(endRing, end, outterRadius, innerRadius));

            return paths;
        }

        /// <summary>
        /// 第几圈, 从0 开始
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private float GetRadiusByRingCount(int count)
        {
            return count * _radiusGap + _initRadius;
        }

        private static float GetTimePercent(Time24Hour time)
        {
            return (time.Day * 24 * 60 + time.Hour * 60 + time.Minute) / (12 * 60f);
        }

        private void DrawDial(SKCanvas canvas)
        {
            using SKPaint paint = new SKPaint { Style = SKPaintStyle.Stroke, Color = SKColors.Blue, StrokeWidth = 10 };

            float curRadius = 0;

            for (int i = 0; i < _fullRoudCount; i++)
            {
                curRadius = _initRadius + i * _radiusGap;

                canvas.DrawCircle(0, 0, curRadius, paint);
            }

            //画剩余extraHours
            curRadius += _radiusGap;

            canvas.DrawArc(SKRect.Create(-curRadius, -curRadius, 2 * curRadius, 2 * curRadius), -90, _extraHours * 30, false, paint);
        }
    }
}