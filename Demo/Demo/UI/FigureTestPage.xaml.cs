using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Controls.Clock;
using HB.FullStack.XamarinForms.Skia;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FigureTestPage : BaseContentPage
    {
        public FigureTestPage()
        {
            InitializeComponent();
            BindingContext = this;

            HourHandFigure hourHandFigure = new HourHandFigure(0.5f, 6);

            MinuteHandFigure minuteHandFigure = new MinuteHandFigure(0.8f, 19, hourHandFigure);

            TimeBlockFigure tb1 = new TimeBlockFigure(
                0.3f,
                0.5f,
                new TimeBlockDrawInfo
                {
                    Color = Color.Red.ToSKColor(),
                    StartTime = new Time24Hour(6, 30),
                    EndTime = new Time24Hour(9, 45)
                });

            TimeBlockFigure tb2 = new TimeBlockFigure(
                0.3f,
                0.5f,
                new TimeBlockDrawInfo
                {
                    Color = Color.Blue.ToSKColor(),
                    StartTime = new Time24Hour(10, 30),
                    EndTime = new Time24Hour(13, 45)
                });

            TimeBlockFigureGroup tbGroup = new TimeBlockFigureGroup() { EnableMultipleSelected = true };
            tbGroup.AddFigures(tb1, tb2);

            Figures.AddRange(new SKFigure[] { hourHandFigure, minuteHandFigure, tbGroup });
        }

        public ObservableRangeCollection<SKFigure> Figures { get; } = new ObservableRangeCollection<SKFigure>();
        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return new List<IBaseContentView?> { Canvas };
        }
    }
}