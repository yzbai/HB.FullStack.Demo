using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

using HB.FullStack.Common;
using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Base;
using HB.FullStack.XamarinForms.Skia;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SkiaTouchTestPage : BaseContentPage
    {
        #region 单个

        private RectangleFigure? _newRectangelFigure;

        private RectangleDrawData? _drawData;
        private RectangleResultData? _resultData;

        public RectangleDrawData? DrawData { get => _drawData; set { _drawData = value; OnPropertyChanged(); } }
        public RectangleResultData? ResultData { get => _resultData; set { _resultData = value; OnPropertyChanged(); } }

        #endregion

        #region Group

        private RectangleCollectionFigure? _rectangleCollectionFigure;

        private ObservableRangeCollection<RectangleDrawData>? _drawDatas;
        private IList<RectangleResultData?>? _resultDatas;

        public ObservableRangeCollection<RectangleDrawData>? DrawDatas { get => _drawDatas; set { _drawDatas = value; OnPropertyChanged(); } }

        public IList<RectangleResultData?>? ResultDatas
        {
            get => _resultDatas;
            set
            {
                if (_resultDatas is ObservableCollection<RectangleResultData> oldCollection)
                {
                    oldCollection.CollectionChanged -= OnResultDatasCollectionChanged;
                }
                _resultDatas = value;

                OnPropertyChanged();

                CheckOnIfSignalNewSelectedIds();

                if (_resultDatas is ObservableCollection<RectangleResultData> newCollection)
                {
                    newCollection.CollectionChanged += OnResultDatasCollectionChanged;
                }
            }
        }

        private void CheckOnIfSignalNewSelectedIds()
        {
            //通过比较前后两次选择状态，不相同的时候，再发射Notify
            OnPropertyChanged(nameof(SelectedIds));
            OnPropertyChanged(nameof(LongSelectedIds));
        }

        public string? SelectedIds
        {
            get
            {
                if (ResultDatas == null)
                {
                    return null;
                }

                var step1 = ResultDatas.Where(d => d != null && d.State == FigureState.Selected).ToList();
                var step2 = step1.Select(d => d.Id).ToList();
                var step3 = step2.ToJoinedString(" # ");

                return step3;
            }
        }

        public string? LongSelectedIds
        {
            get
            {
                if (ResultDatas == null)
                {
                    return null;
                }

                BaseApplication.LogDebug("输出长按选择Ids");

                var step1 = ResultDatas.Where(d => d != null && d.State == FigureState.LongSelected).ToList();
                var step2 = step1.Select(d => d.Id).ToList();
                var step3 = step2.ToJoinedString(" # ");

                return step3;
            }
        }

        private void OnResultDatasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CheckOnIfSignalNewSelectedIds();
        }

        #endregion

        public ICommand RandomCommand { get; set; }

        public SkiaTouchTestPage()
        {
            RandomCommand = new Command(() =>
            {
                DrawData = new RectangleDrawData { Id = 101, Rect = GetRandomRect(), Color = ColorUtil.RandomColor().Color.ToSKColor() };

                var initDrawDatas = new List<RectangleDrawData>();

                for (int i = 0; i < 2; ++i)
                {
                    initDrawDatas.Add(new RectangleDrawData { Id = 101 + i, Rect = GetRandomRect(), Color = ColorUtil.RandomColor().Color.ToSKColor() });
                }

                DrawDatas = new ObservableRangeCollection<RectangleDrawData>(initDrawDatas);

            });

            InitializeComponent();

            BindingContext = this;
        }
        protected override IList<IBaseContentView?>? GetAllCustomerControls() => new IBaseContentView?[] { FigureCanvasView };

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _newRectangelFigure = new RectangleFigure();
            _newRectangelFigure.SetBinding(RectangleFigure.DrawDataProperty, new Binding(nameof(DrawData), source: this));
            _newRectangelFigure.SetBinding(RectangleFigure.ResultDataProperty, new Binding(nameof(ResultData), source: this));
            FigureCanvasView.Figures.Add(_newRectangelFigure);

            _rectangleCollectionFigure = new RectangleCollectionFigure();
            _rectangleCollectionFigure.SetBinding(RectangleCollectionFigure.DrawDatasProperty, new Binding(nameof(DrawDatas), source: this));
            _rectangleCollectionFigure.SetBinding(RectangleCollectionFigure.ResultDatasProperty, new Binding(nameof(ResultDatas), source: this));
            FigureCanvasView.Figures.Add(_rectangleCollectionFigure);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            //Remove Figures
            FigureCanvasView.Figures.Clear();
            _newRectangelFigure?.Dispose();
        }

        private static SKRect GetRandomRect()
        {
            Random random = new Random();

            int x = random.Next(-300, 300);
            int y = random.Next(-600, 600);

            int width = random.Next(10, 300);
            int height = random.Next(10, 400);

            return new SKRect(x, y, x + width, y + height);
        }
    }

    public class RectangleCollectionFigure : SKFigureGroup<RectangleFigure, RectangleDrawData, RectangleResultData>
    {
        public RectangleCollectionFigure()
        {
            EnableLongTap = true;
            EnableMultipleSelected = true;
            EnableMultipleLongSelected = true;
        }

        protected override void OnDrawDatasChanged()
        {
             
        }

        protected override void OnDrawDatasCollectionChanged()
        {
             
        }
    }

    public class RectangleFigure : SKFigure<RectangleDrawData, RectangleResultData>
    {
        public RectangleFigure()
        {
            OneFingerDragged += OnOneFingerDragged;
        }

        protected override void OnDrawDataChanged()
        {
            ReCaculateMiddleCache();
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas, RectangleDrawData initDrawData, FigureState state)
        {
            if (CanvasSizeChanged)
            {
                ReCaculateMiddleCache();
            }

            using SKPaint paint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 5, Color = initDrawData.Color };

            if (state == FigureState.Selected)
            {
                paint.Style = SKPaintStyle.StrokeAndFill;
                paint.StrokeWidth = 10;
                paint.StrokeCap = SKStrokeCap.Round;
            }
            else if (state == FigureState.LongSelected)
            {
                paint.Style = SKPaintStyle.StrokeAndFill;
                paint.StrokeWidth = 20;
                paint.StrokeCap = SKStrokeCap.Square;
            }

            canvas.DrawRect(initDrawData.Rect, paint);
        }

        /// <summary>
        /// 位置，大小，Matrix，形态等，不包含数据
        /// </summary>
        private void ReCaculateMiddleCache()
        {
            //这里放一些计算出来的位置，matrix等，只有在InitData和CanvasSize变化时变化，
            //其他情况不变，节省算力

            /*

             有两大变量：

            InitDrawDataChanged 和 CanvasSizeChanged, 这两者一个在OnInitDrawDataChanged中，一个在Draw中判断，要把所有的数据计算缓存重新计算一边

             */
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info, RectangleDrawData initDrawData)
        {
            //HitTestPath.Reset();
            HitTestPath.AddRect(initDrawData.Rect);
        }

        protected override void OnCaculateOutput(out RectangleResultData? newResultDrawData, RectangleDrawData initDrawData)
        {
            newResultDrawData = new RectangleResultData { Rect = Matrix.MapRect(initDrawData.Rect), Id = initDrawData.Id, State = initDrawData.State };
        }

        private void OnOneFingerDragged(object sender, SKFigureTouchEventArgs e)
        {
            SKMatrix transMatrix = SKMatrix.CreateTranslation(e.CurrentPoint.X - e.PreviousPoint.X, e.CurrentPoint.Y - e.PreviousPoint.Y);

            Matrix = Matrix.PostConcat(transMatrix);
        }
    }

    public class RectangleDrawData : FigureData
    {
        public long Id { get; set; }

        public SKRect Rect { get; set; }

        public SKColor Color { get; set; }

        protected override bool EqualsImpl(FigureData other)
        {
            if (other is RectangleDrawData data)
            {

                return Id == data.Id && Rect == data.Rect && Color == data.Color;
            }

            return false;
        }

        protected override HashCode GetHashCodeImpl()
        {
            HashCode hashCode = new HashCode();

            hashCode.Add(Id);
            hashCode.Add(Rect);
            hashCode.Add(Color);

            return hashCode;
        }
    }

    public class RectangleResultData : FigureData
    {
        public long Id { get; set; }

        public SKRect Rect { get; set; }

        protected override bool EqualsImpl(FigureData other)
        {
            if (other is RectangleDrawData data)
            {

                return Id == data.Id && Rect == data.Rect;
            }

            return false;
        }

        protected override HashCode GetHashCodeImpl()
        {
            HashCode hashCode = new HashCode();

            hashCode.Add(Id);
            hashCode.Add(Rect);

            return hashCode;
        }
    }
}