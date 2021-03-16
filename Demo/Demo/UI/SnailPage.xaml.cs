using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    public partial class SnailPage : ContentPage
    {
        private const float MAX_RADIUS_RATIO = 1f; //占用屏幕最大比例

        private const int DISPLAY_HOURS = 26;
        private const int INIT_RADIUS = 50;

        private const int NUMBER_MARK_RADIUS = 30;

        //Consts
        private const float RADIANS_ONE_DEGREE = (float)(Math.PI / 180);
        private const int MINUTES_ONE_DAY = 24 * 60;
        private const double RADIANS_ONE_DAY = 4 * Math.PI;
        private const double RADIANS_ONE_HOUR = Math.PI / 6;
        private const float RADIANS_ONE_MINUTE = (float)(Math.PI / 360);
        private const double ONE_RADIAN_DEGREES = 180 / Math.PI;
        private const double HALF_PI = 0.5 * Math.PI;
        private const float TWO_PI = (float)(2 * Math.PI);

        private const int DEGREES_ONE_HOUR = 30;
        private const float MARK_LENGTH_RATIO = 0.2f;

        //Draw Middle Data
        private float _maxRadius;
        private float _initRadius;
        private float _radiusGap;
        private int _roudCount;
        private int _totalDegrees;
        private float _totalRadian;
        private float _oneDegreeRadiusIncr;
        private float _oneRadianRadiusIncr;

        //小刻度的长度
        private float _markLength;

        public SnailPage()
        {
            InitializeComponent();
        }

        private bool _first = true;


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

            canvas.Clear();

            canvas.Translate(info.Width / 2f, info.Height / 2f);

            if (_first)
            {
                _maxRadius = Math.Min(info.Width, info.Height) / 2f * MAX_RADIUS_RATIO;

                _initRadius = INIT_RADIUS;

                int displayHours = DISPLAY_HOURS;

                //转几圈
                _roudCount = 1 + (int)Math.Floor(displayHours / 12f);

                int extraHoursDegrees = (displayHours % 12) * DEGREES_ONE_HOUR;

                //总共多少度
                _totalDegrees = _roudCount * 360 + extraHoursDegrees;

                //总共多少弧度
                _totalRadian = _totalDegrees * RADIANS_ONE_DEGREE;

                //每一度，增长的半径长度, 
                _oneDegreeRadiusIncr =
                    extraHoursDegrees > 75 //只是多出来两个多小时的话，不影响
                    ? (_maxRadius - _initRadius) / _totalDegrees
                    : (_maxRadius - _initRadius) / (_totalDegrees - extraHoursDegrees);

                //每一弧度，增长的半径长度
                _oneRadianRadiusIncr = 
                    extraHoursDegrees > 75 //只是多出来两个多小时的话，不影响
                    ? (_maxRadius - _initRadius) / _totalRadian 
                    :(_maxRadius - _initRadius) / (_totalRadian - extraHoursDegrees * RADIANS_ONE_DEGREE);

                //两根螺旋线间的距离
                _radiusGap = (GetRadiusByRadian(_roudCount * TWO_PI) - _initRadius) / _roudCount;

                _markLength = _radiusGap * MARK_LENGTH_RATIO;

                _first = false;
            }

            //Draw TimeBlock

            List<(Time24Hour, Time24Hour)> blocks = new List<(Time24Hour, Time24Hour)>
            {
                (new Time24Hour(0,0), new Time24Hour(3,50)),
                (new Time24Hour(4, 0), new Time24Hour(6, 15)),
                (new Time24Hour(6, 30), new Time24Hour(6, 55)),
                (new Time24Hour(6, 55), new Time24Hour(7, 0)),
                (new Time24Hour(7, 0), new Time24Hour(7, 45)),
                (new Time24Hour(8, 0), new Time24Hour(11, 55)),
                (new Time24Hour(12, 0), new Time24Hour(13, 0)),
                (new Time24Hour(13, 0), new Time24Hour(18, 0)),
                (new Time24Hour(20, 0), new Time24Hour(20, 30)),
                (new Time24Hour(21, 0), new Time24Hour(24, 0)),
            };


            foreach (var block in blocks)
            {
                SKPath tbPath = GetTimeBlockPath(block.Item1, block.Item2);

                paint2.Color = ColorUtil.RandomColor().Color.ToSKColor();

                canvas.DrawPath(tbPath, paint2);
            }

            //Snail

            DrawSnail(canvas);

            // Center


            canvas.DrawPoint(0, 0, paint2);

        }

        /// <summary>
        /// 表盘
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="initRadius"></param>
        /// <param name="diffRadius"></param>
        private void DrawSnail(SKCanvas canvas)
        {


            using SKPaint spiralPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 4,
                IsAntialias = true
            };

            using SKPaint hourPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 4,
                IsAntialias = true
            };

            using SKPaint littleCirclePatin = new SKPaint
            {

                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.Black,
                StrokeWidth = 1,
                IsAntialias = true
            };

            using SKPaint markPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 4,
                IsAntialias = true,
                StrokeCap = SKStrokeCap.Round,
                PathEffect = SKPathEffect.CreateDash(new float[] { 30, 30 }, 0)
            };

            using SKPaint numberPaint = new SKPaint
            {
                Color = SKColors.White,
            };

            float originalTextWidth = numberPaint.MeasureText("8");
            float textWithSizeRatio = originalTextWidth / numberPaint.TextSize;
            numberPaint.TextSize = NUMBER_MARK_RADIUS * 2 / MathF.Sqrt(2);
            float oneDigitalWidth = numberPaint.TextSize * textWithSizeRatio;

            //TODO: 可以只在有变动时生成
            using SKPath spiralPath = new SKPath();
            using SKPath hourPath = new SKPath();
            using SKPath markPath = new SKPath();

            int hourNumber = 0;

            for (float degree = 0; degree <= _totalDegrees; degree += 1)
            {
                float curRaius = GetRadiusByDegree(degree);
                double radians = degree * RADIANS_ONE_DEGREE - HALF_PI;

                float cos = (float)Math.Cos(radians);
                float sin = (float)Math.Sin(radians);

                float x = curRaius * cos;
                float y = curRaius * sin;

                float outterX = (curRaius + _radiusGap) * cos;
                float outterY = (curRaius + _radiusGap) * sin;

                if (degree == 0)
                {
                    spiralPath.MoveTo(x, y);
                }
                else
                {
                    spiralPath.LineTo(x, y);
                }

                if (hourNumber <= DISPLAY_HOURS)
                {
                    if (degree % 30 == 0)
                    {
                        //画小时线
                        canvas.DrawLine(x, y, outterX, outterY, spiralPaint);

                        hourNumber++;

                    }
                    else if (degree * 10 % 75 == 0)
                    {
                        //画15分钟线
                        //canvas.DrawLine(x, y, outterX, outterY, markPaint);
                    }
                }
            }

            canvas.DrawPath(spiralPath, spiralPaint);

            hourNumber = 0;

            for (float degree = 0; degree <= _totalDegrees; degree += 30)
            {
                float curRaius = GetRadiusByDegree(degree);
                double radians = degree * RADIANS_ONE_DEGREE - HALF_PI;

                float cos = (float)Math.Cos(radians);
                float sin = (float)Math.Sin(radians);

                float x = curRaius * cos;
                float y = curRaius * sin;

                float outterX = (curRaius + _radiusGap) * cos;
                float outterY = (curRaius + _radiusGap) * sin;



                if (hourNumber <= DISPLAY_HOURS)
                {
                    if (degree % 30 == 0)
                    {

                        canvas.DrawCircle(outterX, outterY, NUMBER_MARK_RADIUS, littleCirclePatin);

                        string text = (hourNumber % 24).ToString();

                        float textWidth = text.Length * oneDigitalWidth;

                        canvas.DrawText(hourNumber.ToString(), outterX - textWidth / 2f, outterY + numberPaint.TextSize / 2f, numberPaint);

                        hourNumber++;
                    }
                }
            }



            //return path;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxRadius">最大半径</param>
        /// <param name="initRadius">初始半径</param>
        /// <param name="radiusGap">螺旋环的间距</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        private SKPath GetTimeBlockPath(Time24Hour startTime, Time24Hour endTime)
        {
            SKPath tbPath = new SKPath();

            float startRadian = GetTimeRadian(startTime);
            float endRadian = GetTimeRadian(endTime);

            float startRadius = GetRadiusByRadian(startRadian);
            float startOutterRadius = startRadius + _radiusGap;

            float endRadius = GetRadiusByRadian(endRadian);
            float endOutterRadius = endRadius + _radiusGap;

            double degrees = (endRadian - startRadian) * ONE_RADIAN_DEGREES;  //180 / Math.PI;

            double oneDegreeRadiusIncr = (endRadius - startRadius) / degrees;

            for (int degree = 0; degree <= degrees; degree++)
            {
                double curRadian = startRadian + degree * RADIANS_ONE_DEGREE;
                double curRadius = startOutterRadius + degree * oneDegreeRadiusIncr;

                float curX = (float)(curRadius * Math.Cos(curRadian - HALF_PI));
                float curY = (float)(curRadius * Math.Sin(curRadian - HALF_PI));

                if (degree == 0)
                {
                    tbPath.MoveTo(curX, curY);
                }
                else
                {
                    tbPath.LineTo(curX, curY);
                }
            }

            tbPath.LineTo((float)(endOutterRadius * Math.Cos(endRadian - HALF_PI)), (float)(endOutterRadius * Math.Sin(endRadian - HALF_PI)));

            for (int degree = 0; degree <= degrees; degree++)
            {
                double curRadian = endRadian - degree * RADIANS_ONE_DEGREE;
                double curRadius = endRadius - degree * oneDegreeRadiusIncr;

                float curX = (float)(curRadius * Math.Cos(curRadian - HALF_PI));
                float curY = (float)(curRadius * Math.Sin(curRadian - HALF_PI));

                tbPath.LineTo(curX, curY);
            }

            tbPath.LineTo((float)(startRadius * Math.Cos(startRadian - HALF_PI)), (float)(startRadius * Math.Sin(startRadian - HALF_PI)));

            tbPath.Close();

            return tbPath;
        }

        /// <summary>
        /// 时间弧度，从0点开始
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static float GetTimeRadian(Time24Hour time)
        {
            //TODO: 提前把数字都计算出来
            return (time.Day * 24 * 60 + time.Hour * 60 + time.Minute) * RADIANS_ONE_MINUTE;
        }

        /// <summary>
        /// 根据弧度，算出半径
        /// </summary>
        /// <param name="radian"></param>
        /// <param name="initRadius"></param>
        /// <param name="maxRadius"></param>
        /// <returns></returns>
        public float GetRadiusByRadian(float radian)
        {
            return _initRadius + radian * _oneRadianRadiusIncr;
        }

        public float GetRadiusByDegree(float degree)
        {
            return _initRadius + degree * _oneDegreeRadiusIncr;
        }
    }
}