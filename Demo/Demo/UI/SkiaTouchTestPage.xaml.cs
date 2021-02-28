using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

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
        private RectangleFigure? _newRectangelFigure;

        private RectangleDrawData? _initData, _resultData;

        public RectangleDrawData? InitDrawData { get => _initData; set { _initData = value; OnPropertyChanged(); } }
        public RectangleDrawData? ResultDrawData { get => _resultData; set { _resultData = value; OnPropertyChanged(); } }


        private RectangleCollectionFigure? _rectangleCollectionFigure;

        private ObservableRangeCollection<RectangleDrawData>? _initDrawDatas;
        private IList<RectangleDrawData?>? _resultDrawDatas;

        public ObservableRangeCollection<RectangleDrawData>? InitDrawDatas { get => _initDrawDatas; set { _initDrawDatas = value; OnPropertyChanged(); } }

        public IList<RectangleDrawData?>? ResultDrawDatas
        {
            get => _resultDrawDatas;
            set
            {
                if (_resultDrawDatas is ObservableCollection<RectangleDrawData> oldCollection)
                {
                    oldCollection.CollectionChanged -= OnResultDrawDatasCollectionChanged;
                }
                _resultDrawDatas = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(SelectedIds));
                OnPropertyChanged(nameof(LongSelectedIds));

                if (_resultDrawDatas is ObservableCollection<RectangleDrawData> newCollection)
                {
                    newCollection.CollectionChanged += OnResultDrawDatasCollectionChanged;
                }
            }
        }

        public string? SelectedIds
        {
            get
            {
                if (ResultDrawDatas == null)
                {
                    return null;
                }

                var step1 = ResultDrawDatas.Where(d => d != null && d.State == FigureState.Selected).ToList();
                var step2 = step1.Select(d => d.Id).ToList();
                var step3 = step2.ToJoinedString(" # ");

                return step3;
            }
        }

        public string? LongSelectedIds
        {
            get
            {
                if (ResultDrawDatas == null)
                {
                    return null;
                }

                BaseApplication.LogDebug("输出长按选择Ids");

                var step1 = ResultDrawDatas.Where(d => d != null && d.State == FigureState.LongSelected).ToList();
                var step2 = step1.Select(d => d.Id).ToList();
                var step3 = step2.ToJoinedString(" # ");

                return step3;
            }
        }

        private void OnResultDrawDatasCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            OnPropertyChanged(nameof(SelectedIds));
            OnPropertyChanged(nameof(LongSelectedIds));
        }

        public ICommand RandomCommand { get; set; }

        public SkiaTouchTestPage()
        {
            RandomCommand = new Command(() =>
            {
                InitDrawData = new RectangleDrawData { Id = 101, Rect = GetRandomRect(), Color = ColorUtil.RandomColor().Color.ToSKColor() };

                var initDrawDatas = new List<RectangleDrawData>();

                for (int i = 0; i < 2; ++i)
                {
                    initDrawDatas.Add(new RectangleDrawData { Id = 101 + i, Rect = GetRandomRect(), Color = ColorUtil.RandomColor().Color.ToSKColor() });
                }

                InitDrawDatas = new ObservableRangeCollection<RectangleDrawData>(initDrawDatas);

            });

            InitializeComponent();

            BindingContext = this;
        }
        protected override IList<IBaseContentView?>? GetAllCustomerControls() => new IBaseContentView?[] { FigureCanvasView };

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _newRectangelFigure = new RectangleFigure();
            _newRectangelFigure.SetBinding(RectangleFigure.InitDrawDataProperty, new Binding(nameof(InitDrawData), source: this));
            _newRectangelFigure.SetBinding(RectangleFigure.ResultDrawDataProperty, new Binding(nameof(ResultDrawData), source: this));
            FigureCanvasView.Figures.Add(_newRectangelFigure);

            _rectangleCollectionFigure = new RectangleCollectionFigure();
            _rectangleCollectionFigure.SetBinding(RectangleCollectionFigure.InitDrawDatasProperty, new Binding(nameof(InitDrawDatas), source: this));
            _rectangleCollectionFigure.SetBinding(RectangleCollectionFigure.ResultDrawDatasProperty, new Binding(nameof(ResultDrawDatas), source: this));
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

    public class RectangleCollectionFigure : SKFigureGroup<RectangleFigure, RectangleDrawData>
    {
        public RectangleCollectionFigure()
        {
            EnableLongTap = true;
            EnableMultipleSelected = true;
            EnableMultipleLongSelected = true;
        }
    }

    public class RectangleFigure : SKFigure<RectangleDrawData>
    {
        public RectangleFigure()
        {
            OneFingerDragged += OnOneFingerDragged;
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas, RectangleDrawData initDrawData, FigureState state)
        {
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

        protected override void OnUpdateHitTestPath(SKImageInfo info, RectangleDrawData initDrawData)
        {
            //HitTestPath.Reset();
            HitTestPath.AddRect(initDrawData.Rect);
        }

        protected override void OnCaculateOutput(out RectangleDrawData? newResultDrawData, RectangleDrawData initDrawData)
        {
            newResultDrawData = new RectangleDrawData { Rect = Matrix.MapRect(initDrawData.Rect), Color = initDrawData.Color, Id = initDrawData.Id, State = initDrawData.State };
        }

        private void OnOneFingerDragged(object sender, SKFigureTouchInfo e)
        {
            SKMatrix transMatrix = SKMatrix.CreateTranslation(e.CurrentPoint.X - e.PreviousPoint.X, e.CurrentPoint.Y - e.PreviousPoint.Y);

            Matrix = Matrix.PostConcat(transMatrix);
        }
    }

    public class RectangleDrawData : SKFigureDrawData
    {
        public long Id { get; set; }

        public SKRect Rect { get; set; }

        public SKColor Color { get; set; }

        protected override bool EqualsImpl(SKFigureDrawData other)
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

    public class OldRectangleFigure : SKFigure
    {
        public static BindableProperty InitDrawDataProperty = BindableProperty.Create(
                    nameof(InitDrawData),
                    typeof(RectangleDrawData),
                    typeof(OldRectangleFigure),
                    null,
                    BindingMode.OneWay,
                    propertyChanged: (b, oldValue, newValue) => ((OldRectangleFigure)b).OnInitDrawDataChanged((RectangleDrawData)oldValue, (RectangleDrawData)newValue));

        public static BindableProperty ResultDrawDataProperty = BindableProperty.Create(
                    nameof(ResultDrawData),
                    typeof(RectangleDrawData),
                    typeof(OldRectangleFigure),
                    null,
                    BindingMode.OneWayToSource);

        public RectangleDrawData? InitDrawData { get => (RectangleDrawData?)GetValue(InitDrawDataProperty); set => SetValue(InitDrawDataProperty, value); }

        public RectangleDrawData? ResultDrawData { get => (RectangleDrawData?)GetValue(ResultDrawDataProperty); set => SetValue(ResultDrawDataProperty, value); }

        private bool _hitPathNeedUpdate = false;

        public OldRectangleFigure()
        {
            OneFingerDragged += RectangleFigure_OneFingerDragged;
        }

        private void OnInitDrawDataChanged(RectangleDrawData oldValue, RectangleDrawData newValue)
        {
            _hitPathNeedUpdate = true;

            InvalidateMatrixAndSurface();
        }

        private void RectangleFigure_OneFingerDragged(object sender, SKFigureTouchInfo e)
        {
            SKMatrix transMatrix = SKMatrix.CreateTranslation(e.CurrentPoint.X - e.PreviousPoint.X, e.CurrentPoint.Y - e.PreviousPoint.Y);

            Matrix = Matrix.PostConcat(transMatrix);
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {
            if (InitDrawData == null)
            {
                return;
            }

            using SKPaint paint = new SKPaint { Style = SKPaintStyle.Fill, Color = InitDrawData.Color };

            canvas.DrawRect(InitDrawData.Rect, paint);
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info)
        {
            if (InitDrawData == null)
            {
                return;
            }

            if (CanvasSizeChanged || _hitPathNeedUpdate)
            {
                _hitPathNeedUpdate = false;

                HitTestPath = new SKPath();
                HitTestPath.AddRect(InitDrawData.Rect);
            }
        }

        protected override void OnCaculateOutput()
        {
            if (InitDrawData == null)
            {
                return;
            }

            //这样没用，因为DrawData的值（地址）并没有变，只是他成员的值变了，OnPropertyChanged时，会因为引用值相同而不通知
            //ResultDrawData.Rect = Matrix.MapRect(_currentDrawData.Rect);
            //OnPropertyChanged(nameof(DrawData));

            ResultDrawData = new RectangleDrawData { Rect = Matrix.MapRect(InitDrawData.Rect), Color = InitDrawData.Color };
        }
    }


}