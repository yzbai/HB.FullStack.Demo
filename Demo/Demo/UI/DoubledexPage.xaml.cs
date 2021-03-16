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
        private const int HOURS_ONE_ROUND = 12;

        private const float MAX_RADIUS_RATIO = 0.9f;
        private const float INIT_RADIUS = 100f;

        private const int HOUR_MARK_CIRCLE_RADIUS = 30; //小时标记的小圆圈

        private const float SHRINK_EXTRA_HOUR_RADIUS_RATIO = 0.5f;

        //COnsts
        private const float RADIANS_ONE_DEGREE = (float)(Math.PI / 180);
        private const double HALF_PI = 0.5 * Math.PI;

        private float _initRadius;
        private float _maxRadius;
        private float _radiusGap;
        private int _fullRoudCount;
        private int _extraHours;
        private bool _shrinkExtraHourRadius = false;
        private bool _first = true;

        private float _degrees_one_hour;

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
                _degrees_one_hour = 360 / HOURS_ONE_ROUND;

                _initRadius = INIT_RADIUS;
                _maxRadius = Math.Min(info.Width, info.Height) / 2f * MAX_RADIUS_RATIO;

                int displayHours = DISPLAY_HOURS;

                //转几整圈
                _fullRoudCount = 1 + (int)Math.Floor((float)displayHours / HOURS_ONE_ROUND);

                //整圈后剩下的小时
                _extraHours = displayHours % HOURS_ONE_ROUND;

                //每环之间等距

                int hourTolerance = HOURS_ONE_ROUND / 6; //只是多出来两个多小时的话，不影响原有间距

                _shrinkExtraHourRadius = _extraHours <= hourTolerance;

                _radiusGap = (_maxRadius - _initRadius) / (_shrinkExtraHourRadius ? _fullRoudCount - 1 : _fullRoudCount);

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
                (new Time24Hour(21, 0), new Time24Hour(1, 30, 1)),
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

            //同一圈
            int startRound = (int)Math.Floor(start);
            int endRound = (int)Math.Floor(end) == end ? (int)(end - 1) : (int)Math.Floor(end);

            float innerRadius, outterRadius;

            if (startRound == endRound)
            {
                innerRadius = GetRadiusByRoundCount(startRound);
                outterRadius = GetRadiusByRoundCount(startRound + 1);

                return new List<SKPath> { SKGeometry.CreateSectorPath(start, end, outterRadius, innerRadius) };
            }

            //不同圈
            List<SKPath> paths = new List<SKPath>();

            //先画完当前ring
            innerRadius = GetRadiusByRoundCount(startRound);
            outterRadius = GetRadiusByRoundCount(startRound + 1);

            paths.Add(SKGeometry.CreateSectorPath(start, startRound + 1, outterRadius, innerRadius));

            //画整圈
            int curRing = startRound + 1;

            while (curRing < endRound)
            {
                innerRadius = GetRadiusByRoundCount(curRing);
                outterRadius = GetRadiusByRoundCount(curRing + 1);
                paths.Add(SKGeometry.CreateSectorPath(curRing, curRing + 1, outterRadius, innerRadius));

                curRing++;
            }

            //画剩下的
            innerRadius = GetRadiusByRoundCount(endRound);
            outterRadius = GetRadiusByRoundCount(endRound + 1);
            paths.Add(SKGeometry.CreateSectorPath(endRound, end, outterRadius, innerRadius));

            return paths;
        }

        /// <summary>
        /// 第几圈, 从0 开始
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private float GetRadiusByRoundCount(int count)
        {
            if (count < _fullRoudCount)
            {
                return count * _radiusGap + _initRadius;
            }

            return _initRadius + (_fullRoudCount - 1) * _radiusGap + (_shrinkExtraHourRadius ? _radiusGap * SHRINK_EXTRA_HOUR_RADIUS_RATIO : _radiusGap);
        }

        private static float GetTimePercent(Time24Hour time)
        {
            return (time.Day * 24 * 60 + time.Hour * 60 + time.Minute) / (HOURS_ONE_ROUND * 60f);
        }

        private void DrawDial(SKCanvas canvas)
        {
            using SKPaint paint = new SKPaint { Style = SKPaintStyle.Stroke, Color = SKColors.Black, StrokeWidth = 8 };
            using SKPaint hourLinePaint = new SKPaint { Style = SKPaintStyle.Stroke, Color = SKColors.Black, StrokeWidth = 3 };
            using SKPaint hourCirclePaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.Black };
            using SKPaint hourTextPaint = new SKPaint { Color = SKColors.White };

            float originalTextWidth = hourTextPaint.MeasureText("4");
            float textWithSizeRatio = originalTextWidth / hourTextPaint.TextSize;
            hourTextPaint.TextSize = HOUR_MARK_CIRCLE_RADIUS * 2 / MathF.Sqrt(2);
            float oneDigitalWidth = hourTextPaint.TextSize * textWithSizeRatio;

            //画轮廓线
            float curRadius = 0;
            float innerRadius = 0;
            int hourTextNumber = 0;

            for (int i = 0; i < _fullRoudCount; i++)
            {
                curRadius = GetRadiusByRoundCount(i);

                canvas.DrawCircle(0, 0, curRadius, paint);

                if (i == 0)
                {
                    continue;
                }

                //画小时

                innerRadius = GetRadiusByRoundCount(i - 1);

                for (float degree = 0; degree < 360; degree += _degrees_one_hour)
                {
                    double radian = degree * RADIANS_ONE_DEGREE - HALF_PI;

                    float cos = (float)Math.Cos(radian);
                    float sin = (float)Math.Sin(radian);

                    float innerX = innerRadius * cos;
                    float innerY = innerRadius * sin;

                    float x = curRadius * cos;
                    float y = curRadius * sin;

                    //line
                    canvas.DrawLine(innerX, innerY, x, y, hourLinePaint);

                    //circle
                    canvas.DrawCircle(x, y, HOUR_MARK_CIRCLE_RADIUS, hourCirclePaint);

                    //text

                    string text = (hourTextNumber % 24).ToString();

                    float textWidth = text.Length * oneDigitalWidth;

                    canvas.DrawText(text, x - textWidth / 2f, y + hourTextPaint.TextSize / 2f, hourTextPaint);

                    hourTextNumber++;
                }
            }

            //画剩余extraHours
            //curRadius += _shrinkExtraHourRadius ? _radiusGap * SHRINK_EXTRA_HOUR_RADIUS_RATIO : _radiusGap;
            curRadius = GetRadiusByRoundCount(_fullRoudCount);

            float extraDegrees = _extraHours * _degrees_one_hour;
            canvas.DrawArc(SKRect.Create(-curRadius, -curRadius, 2 * curRadius, 2 * curRadius), -90, extraDegrees, false, paint);

            innerRadius = GetRadiusByRoundCount(_fullRoudCount - 1);

            for (float degree = 0; degree <= extraDegrees; degree += _degrees_one_hour)
            {
                double radian = degree * RADIANS_ONE_DEGREE - HALF_PI;

                float cos = (float)Math.Cos(radian);
                float sin = (float)Math.Sin(radian);

                float innerX = innerRadius * cos;
                float innerY = innerRadius * sin;

                float x = curRadius * cos;
                float y = curRadius * sin;

                //line
                canvas.DrawLine(innerX, innerY, x, y, hourLinePaint);

                //circle
                canvas.DrawCircle(x, y, HOUR_MARK_CIRCLE_RADIUS, hourCirclePaint);

                //text

                string text = (hourTextNumber % 24).ToString();

                float textWidth = text.Length * oneDigitalWidth;

                canvas.DrawText(text, x - textWidth / 2f, y + hourTextPaint.TextSize / 2f, hourTextPaint);

                hourTextNumber++;
            }
            //line

            //
        }
    }
}