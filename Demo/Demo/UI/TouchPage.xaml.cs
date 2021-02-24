using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HB.FullStack.XamarinForms.Effects.Touch;
using HB.FullStack.XamarinForms.Skia;

using SkiaSharp;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TouchPage : ContentPage
    {
        public TouchPage()
        {
            InitializeComponent();

            TouchEffect touchEffect2 = new TouchEffect();
            touchEffect2.TouchAction += TouchEffect2_TouchAction;

            
            //CanvasView2.Effects.Add(touchEffect2);


            TouchEffect touchEffect1 = new TouchEffect();
            touchEffect1.TouchAction += TouchEffect1_TouchAction;


            CanvasView.Effects.Add(touchEffect1);


        }

        private void TouchEffect2_TouchAction(object sender, TouchActionEventArgs e)
        {
            label2.Text = $"2 : x:{e.Location.X}, y:{e.Location.Y}";
        }

        private void TouchEffect1_TouchAction(object sender, TouchActionEventArgs e)
        {
            SKPoint sKPoint = SKUtil.ToSKPoint(e.Location);
            label2.Text = $"2 : x:{e.Location.X}, y:{e.Location.Y}";
        }

        private void CanvasView_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            //label2.Text = $"Canvas HiTTTT, Too Happy!";
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {

        }

        private void CanvasView_Touch2(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {

        }
    }
}