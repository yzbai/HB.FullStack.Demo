using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Controls.Clock;

using Microsoft.Extensions.Logging;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaTouchPage : BaseContentPage
    {
        public ClockDescription Description { get; set; }

        public ObservableRangeCollection<TimeBlockDrawInfo> TimeBlockDrawInfos { get; private set; } = new ObservableRangeCollection<TimeBlockDrawInfo>();

        public SkiaTouchPage()
        {
            InitializeComponent();

            Description = new ClockDescription
            {
                //DialBackgroundGifResourceName = "Newtons_cradle_animation_book_2.gif",
                DialBackgroundRatio = 0.6f,
                TicksRatio = 0.6f,
                DialHandRatio = 0.6f,
                TimeBlockRatio = 0.6f
            };

            TimeBlockDrawInfos.Add(new TimeBlockDrawInfo { Color = SKColors.Red, StartTime = new Time24Hour(6, 30), EndTime = new Time24Hour(9, 30) });
            
            BindingContext = this;
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return new List<IBaseContentView?> {Clock, ClockPicker};
        }

        private void SKCanvasView_Touch(object sender, SKTouchEventArgs e)
        {
            GlobalSettings.Logger.LogInformation(SerializeUtil.ToJson(e));
        }

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {

        }
    }
}