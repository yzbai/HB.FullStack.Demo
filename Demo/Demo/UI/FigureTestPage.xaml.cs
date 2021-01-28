using HB.FullStack.Mobile.Base;
using HB.FullStack.Mobile.Controls.Clock;
using HB.FullStack.Mobile.Skia;

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
        public ObservableRangeCollection<SKFigure> Figures { get; } = new ObservableRangeCollection<SKFigure>();

        public FigureTestPage()
        {
            InitializeComponent();
            BindingContext = this;

            HourHandFigure hourHandFigure = new HourHandFigure(0.5f, 6) {  PivotRatioPoint = new SKRatioPoint(0.5f, 0.5f)};

            MinuteHandFigure minuteHandFigure = new MinuteHandFigure(0.8f, 19, hourHandFigure) { PivotRatioPoint = new SKRatioPoint(0.5f, 0.5f)};

            Figures.AddRange(new SKFigure[] { hourHandFigure, minuteHandFigure});
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls()
        {
            return new List<IBaseContentView?> { Canvas };
        }
    }
}