using HB.FullStack.Mobile.Base;
using HB.FullStack.Mobile.Effects.Touch;
using HB.FullStack.Mobile.Platforms;
using HB.FullStack.Mobile.Skia;

using SkiaSharp;
using SkiaSharp.Views.Forms;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CropperPage : BaseContentPage
    {
        private readonly IFileHelper _fileHelper = DependencyService.Resolve<IFileHelper>();
        private CropperFrameFigure? _cropperFrameFigure;
        private BitmapFigure? _bitmapFigure;

        public ObservableRangeCollection<SKFigure> Figures { get; } = new ObservableRangeCollection<SKFigure>();

        public ICommand RotateCommand { get; }

        public ICommand CancelCommand { get; }

        public ICommand ResetCommand { get; }

        public ICommand CropCommand { get; }

        public CropperPage()
        {
            InitializeComponent();

            CropCommand = new Command(Crop);
            RotateCommand = new Command(Rotate);
            CancelCommand = new Command(Cancel);
            ResetCommand = new Command(Reset);

            BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ResumeFigures();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            RemoveFigures();
        }

        private void ResumeFigures()
        {
            _bitmapFigure = new BitmapFigure(0.9f, 0.9f, null)
            {
                EnableTwoFingers = true,
                ManipulationMode = TouchManipulationMode.IsotropicScale
            };

            _cropperFrameFigure = new CropperFrameFigure(0.5f, 0.5f, 0.9f, 0.9f);

            Figures.AddRange(new SKFigure[] { _bitmapFigure, _cropperFrameFigure });

            _fileHelper.GetResourceStreamAsync("bg_test.png").ContinueWith(async t =>
            {
                using Stream stream = await t;

                _bitmapFigure.SetBitmap(stream);

            }).Fire();
        }

        private void RemoveFigures()
        {
            foreach (SKFigure f in Figures)
            {
                f.Dispose();
            }

            Figures.Clear();

            _cropperFrameFigure = null;
            _bitmapFigure = null;
        }

        protected override IList<IBaseContentView?>? GetAllCustomerControls() => new List<IBaseContentView?> { FigureCanvas };

        private void Reset()
        {
            RemoveFigures();
            ResumeFigures();
        }

        private void Rotate()
        {
            _bitmapFigure?.Rotate90(false);
        }

        private void Cancel()
        {

        }

        private void Crop()
        {
            if (_cropperFrameFigure == null || _bitmapFigure == null)
            {
                return;
            }

            SKRect cropRect = _cropperFrameFigure.CropRect;

            _bitmapFigure.Crop(cropRect);
            _bitmapFigure.EnableTouch = false;

            Figures.Remove(_cropperFrameFigure);

        }
    }

    public enum CornerType
    {
        None,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom
    }

    public class CropperFrameFigure : SKFigure
    {
        private const double CORNER_LENGTH_DP = 10;
        private const double CORNER_TOUCH_RADIUS_DP = 20;
        private const float CROP_RECT_MINIMUM_LENGTH_DP = 40;

        private readonly float _initCropperWidthRatio;
        private readonly float _initCropperHeightRatio;
        private readonly float _outterWidthRatio;
        private readonly float _outterHeightRatio;

        private readonly SKPaint _outterRectPaint;
        private readonly SKPaint _cropperRectPaint;
        private readonly SKPaint _cornerPaint;
        private readonly float _cornerLength;
        private readonly float _cornerTouchRadius;
        private readonly float _cropRectMinimumLength;

        private bool _firstDraw = true;
        private CornerType _hittedCorner = CornerType.None;
        private SKRect _cropRect;
        private SKRect _outterRect;

        public float Transparency { get; set; } = 0.8f;

        public SKRect CropRect => _cropRect;

        public bool IsSquare { get; set; } = true;

        /// <summary>
        /// CropperFrameFigure
        /// </summary>
        /// <param name="initCroperWidthRatio">初始Crop框的比例</param>
        /// <param name="initCropperHeightRatio">初始Crop框的比例</param>
        /// <param name="outterWidthRatio">最大外围</param>
        /// <param name="outterHeightRatio">最大外围</param>
        public CropperFrameFigure(float initCroperWidthRatio, float initCropperHeightRatio, float outterWidthRatio, float outterHeightRatio)
        {
            _cornerLength = (float)SKUtil.ToPx(CORNER_LENGTH_DP);
            _cornerTouchRadius = (float)SKUtil.ToPx(CORNER_TOUCH_RADIUS_DP);
            _cropRectMinimumLength = (float)SKUtil.ToPx(CROP_RECT_MINIMUM_LENGTH_DP);

            _initCropperWidthRatio = initCroperWidthRatio;
            _initCropperHeightRatio = initCropperHeightRatio;
            _outterWidthRatio = outterWidthRatio;
            _outterHeightRatio = outterHeightRatio;

            _outterRectPaint = new SKPaint { Style = SKPaintStyle.Fill, Color = SKColors.DimGray.WithAlpha((byte)(0xFF * (1 - Transparency))) };
            _cropperRectPaint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 2, Color = SKColors.Black };
            _cornerPaint = new SKPaint { Style = SKPaintStyle.Stroke, StrokeWidth = 15, Color = SKColors.Black };


            OneFingerDragged += CropperFrameFigure_OneFingerDragged;
        }

        public void Reset()
        {
            _firstDraw = true;

            CanvasView?.InvalidateSurface();
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {
            if (CanvasSizeChanged)
            {
                _outterRect = SKRect.Create(
                    _outterWidthRatio * info.Width / -2,
                    _outterHeightRatio * info.Height / -2,
                    _outterWidthRatio * info.Width,
                    _outterHeightRatio * info.Height);
            }

            if (_firstDraw)
            {
                if (IsSquare)
                {
                    float cropRectLength = Math.Min(_initCropperWidthRatio * info.Width, _initCropperHeightRatio * info.Height);

                    _cropRect = SKRect.Create(cropRectLength / -2, cropRectLength / -2, cropRectLength, cropRectLength);
                }
                else
                {
                    _cropRect = SKRect.Create(
                        _initCropperWidthRatio * info.Width / -2,
                        _initCropperHeightRatio * info.Height / -2,
                        _initCropperWidthRatio * info.Width,
                        _initCropperHeightRatio * info.Height);
                }
                _firstDraw = false;
            }

            canvas.DrawRect(_outterRect, _outterRectPaint);
            canvas.DrawRect(_cropRect, _cropperRectPaint);

            //Draw Corner

            using SKPath cornerPath = new SKPath();

            //左上角
            cornerPath.MoveTo(_cropRect.Left + _cornerLength, _cropRect.Top);
            cornerPath.LineTo(_cropRect.Left, _cropRect.Top);
            cornerPath.LineTo(_cropRect.Left, _cropRect.Top + _cornerLength);

            //右上角
            cornerPath.MoveTo(_cropRect.Right - _cornerLength, _cropRect.Top);
            cornerPath.LineTo(_cropRect.Right, _cropRect.Top);
            cornerPath.LineTo(_cropRect.Right, _cropRect.Top + _cornerLength);

            //左下角
            cornerPath.MoveTo(_cropRect.Left + _cornerLength, _cropRect.Bottom);
            cornerPath.LineTo(_cropRect.Left, _cropRect.Bottom);
            cornerPath.LineTo(_cropRect.Left, _cropRect.Bottom - _cornerLength);

            //右下角
            cornerPath.MoveTo(_cropRect.Right - _cornerLength, _cropRect.Bottom);
            cornerPath.LineTo(_cropRect.Right, _cropRect.Bottom);
            cornerPath.LineTo(_cropRect.Right, _cropRect.Bottom - _cornerLength);

            canvas.DrawPath(cornerPath, _cornerPaint);
        }

        public override bool OnHitTest(SKPoint skPoint, long touchId)
        {
            SKPoint hitPoint = GetNewCoordinatedPoint(skPoint);

            //左上角
            SKRect rect = SKRect.Create(_cropRect.Left - _cornerTouchRadius, _cropRect.Top - _cornerTouchRadius, _cornerTouchRadius * 2, _cornerTouchRadius * 2);

            if (rect.Contains(hitPoint))
            {
                _hittedCorner = CornerType.LeftTop;
                return true;
            }

            //右上角
            rect = SKRect.Create(_cropRect.Right - _cornerTouchRadius, _cropRect.Top - _cornerTouchRadius, _cornerTouchRadius * 2, _cornerTouchRadius * 2);

            if (rect.Contains(hitPoint))
            {
                _hittedCorner = CornerType.RightTop;
                return true;
            }

            //左下角
            rect = SKRect.Create(_cropRect.Left - _cornerTouchRadius, _cropRect.Bottom - _cornerTouchRadius, _cornerTouchRadius * 2, _cornerTouchRadius * 2);

            if (rect.Contains(hitPoint))
            {
                _hittedCorner = CornerType.LeftBottom;
                return true;
            }

            //右下角
            rect = SKRect.Create(_cropRect.Right - _cornerTouchRadius, _cropRect.Bottom - _cornerTouchRadius, _cornerTouchRadius * 2, _cornerTouchRadius * 2);

            if (rect.Contains(hitPoint))
            {
                _hittedCorner = CornerType.RightBottom;
                return true;
            }

            _hittedCorner = CornerType.None;
            return false;
        }

        private void CropperFrameFigure_OneFingerDragged(object sender, SKFigureTouchEventArgs e)
        {
            float xOffset = e.CurrentPoint.X - e.PreviousPoint.X;
            float yOffset = e.CurrentPoint.Y - e.PreviousPoint.Y;

            //if (IsSquare)
            //{
            //    xOffset = Math.Max(xOffset, yOffset);
            //    yOffset = xOffset;
            //}

            //不能超出边界
            //不能太小
            switch (_hittedCorner)
            {
                case CornerType.RightBottom:
                    {
                        float newRight = _cropRect.Right + xOffset;
                        float newBottom = _cropRect.Bottom + yOffset;

                        newRight = Math.Max(Math.Min(newRight, _outterRect.Right), _cropRect.Left + _cropRectMinimumLength);
                        newBottom = Math.Max(Math.Min(newBottom, _outterRect.Bottom), _cropRect.Top + _cropRectMinimumLength);

                        if (IsSquare)
                        {
                            float offset = Math.Min(newRight - _cropRect.Right, newBottom - _cropRect.Bottom);
                            _cropRect.Right = _cropRect.Right + offset;
                            _cropRect.Bottom = _cropRect.Bottom + offset;
                        }
                        else
                        {
                            _cropRect.Right = newRight;
                            _cropRect.Bottom = newBottom;
                        }

                        break;
                    }
                case CornerType.LeftBottom:
                    {
                        float newLeft = _cropRect.Left + xOffset;
                        float newBottom = _cropRect.Bottom + yOffset;

                        newLeft = Math.Min(Math.Max(newLeft, _outterRect.Left), _cropRect.Right - _cropRectMinimumLength);
                        newBottom = Math.Max(Math.Min(newBottom, _outterRect.Bottom), _cropRect.Top + _cropRectMinimumLength);

                        if (IsSquare)
                        {
                            float offset = Math.Min(Math.Abs(newLeft - _cropRect.Left), Math.Abs(newBottom - _cropRect.Bottom));
                            _cropRect.Left = _cropRect.Left + offset * Math.Sign(xOffset);
                            _cropRect.Bottom = _cropRect.Bottom + offset * Math.Sign(yOffset);
                        }
                        else
                        {
                            _cropRect.Left = newLeft;
                            _cropRect.Bottom = newBottom;
                        }
                        break;
                    }
                case CornerType.LeftTop:
                    {
                        float newLeft = _cropRect.Left + xOffset;
                        float newTop = _cropRect.Top + yOffset;

                        newLeft = Math.Min(Math.Max(newLeft, _outterRect.Left), _cropRect.Right - _cropRectMinimumLength);
                        newTop = Math.Min(Math.Max(newTop, _outterRect.Top), _cropRect.Bottom - _cropRectMinimumLength);

                        if (IsSquare)
                        {
                            float offset = Math.Min(newLeft - _cropRect.Left, newTop - _cropRect.Top);
                            _cropRect.Left = _cropRect.Left + offset;
                            _cropRect.Top = _cropRect.Top + offset;
                        }
                        else
                        {
                            _cropRect.Left = newLeft;
                            _cropRect.Top = newTop;
                        }
                        break;
                    }
                case CornerType.RightTop:
                    {
                        float newRight = _cropRect.Right + xOffset;
                        float newTop = _cropRect.Top + yOffset;

                        newRight = Math.Max(Math.Min(newRight, _outterRect.Right), _cropRect.Left + _cropRectMinimumLength);
                        newTop = Math.Min(Math.Max(newTop, _outterRect.Top), _cropRect.Bottom - _cropRectMinimumLength);

                        if (IsSquare)
                        {
                            float offset = Math.Min(newRight - _cropRect.Right, newTop - _cropRect.Top);
                            _cropRect.Right = _cropRect.Right + offset;
                            _cropRect.Top = _cropRect.Top + offset;
                        }
                        else
                        {
                            _cropRect.Right = newRight;
                            _cropRect.Top = newTop;
                        }

                        break;
                    }
                case CornerType.None:
                    break;
            }
        }

        #region Dispose Pattern

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_disposed)
            {
                if (disposing)
                {
                    // managed
                    _cropperRectPaint.Dispose();
                    _outterRectPaint.Dispose();
                    _cornerPaint.Dispose();
                }

                //unmanaged

                _disposed = true;
            }
        }

        #endregion Dispose Pattern
    }

    /// <summary>
    /// 默认以NewCoordinateOriginalRatioPoint为DestRect中心
    /// </summary>
    public class BitmapFigure : SKFigure
    {
        private readonly float _widthRatio;
        private readonly float _heightRatio;

        private SKRect _destRect;
        private SKBitmap? _bitmap;
        private DrawBitmapResult? _drawBitmapResult;

        public SKStretchMode StretchMode { get; set; } = SKStretchMode.AspectFit;

        public SKAlignment HorizontalAlignment { get; set; } = SKAlignment.Center;

        public SKAlignment VerticalAlignment { get; set; } = SKAlignment.Center;

        public TouchManipulationMode ManipulationMode { get; set; } = TouchManipulationMode.IsotropicScale;

        public BitmapFigure(float widthRatio, float heightRatio, Stream? bitmapStream)
        {
            _widthRatio = widthRatio;
            _heightRatio = heightRatio;

            if (bitmapStream != null)
            {
                SetBitmap(bitmapStream);
            }

            OneFingerDragged += OnOneFingerDragged;
            TwoFingerDragged += OnTwoFingerDragged;
        }

        public void SetBitmap(Stream stream)
        {
            SKBitmap bitmap = SKBitmap.Decode(stream);

            SetBitmap(bitmap);
        }

        public void SetBitmap(SKBitmap bitmap)
        {
            _bitmap?.Dispose();

            _bitmap = bitmap;

            Reset();
        }

        public void Reset()
        {
            _rotatedDegrees = 0;

            Matrix = SKMatrix.CreateIdentity();

            CanvasView?.InvalidateSurface();
        }

        private float _rotatedDegrees;

        public void Rotate90(bool left)
        {
            float degree = left ? -90 : 90;
            _rotatedDegrees += degree;

            Matrix = Matrix.PostConcat(SKMatrix.CreateRotationDegrees(degree));

            CanvasView?.InvalidateSurface();
        }

        public void Crop(SKRect cropRect)
        {
            SKMatrix invertedMatrix = Matrix.Invert();

            SKRect mappedCropRect = invertedMatrix.MapRect(cropRect);

            //由于任意旋转下，矩阵旋转后，不能再用SKRect表示，这里不用任意旋转

            float sourceX = (mappedCropRect.Left - _drawBitmapResult.DisplayRect.Left) / _drawBitmapResult.WidthScale;
            float sourceY = (mappedCropRect.Top - _drawBitmapResult.DisplayRect.Top) / _drawBitmapResult.HeightScale;
            float sourceWidth = mappedCropRect.Width / _drawBitmapResult.WidthScale;
            float sourceHeight = mappedCropRect.Height / _drawBitmapResult.HeightScale;

            //得到原始图片上的原始区域
            SKRect sourceRect = SKRect.Create(sourceX, sourceY, sourceWidth, sourceHeight);

            //将SourceRect区域投射到新的canvas

            SKBitmap croppedBitmap = new SKBitmap((int)cropRect.Width, (int)cropRect.Height);

            using SKCanvas newCanvas = new SKCanvas(croppedBitmap);

            newCanvas.RotateDegrees(_rotatedDegrees, croppedBitmap.Width / 2f, croppedBitmap.Height / 2f);

            SKRect newDestRect = SKRect.Create(0, 0, croppedBitmap.Width, croppedBitmap.Height);

            newCanvas.DrawBitmap(_bitmap, sourceRect, newDestRect);

            SetBitmap(croppedBitmap);
        }

        protected override void OnDraw(SKImageInfo info, SKCanvas canvas)
        {
            if (CanvasSizeChanged)
            {
                //新坐标系下的。
                _destRect = SKRect.Create(
                    _widthRatio * info.Width / -2,
                    _heightRatio * info.Height / -2,
                    _widthRatio * info.Width,
                    _heightRatio * info.Height);
            }

            if (_bitmap != null)
            {
                SKRect mappedDestRect = Matrix.Invert().MapRect(_destRect);

                SKPaint paint = new SKPaint { IsStroke = true, StrokeWidth = 10, Color = Color.Red.ToSKColor() };

                canvas.DrawRect(mappedDestRect, paint);

                _drawBitmapResult = canvas.DrawBitmap(_bitmap, _destRect, StretchMode, HorizontalAlignment, VerticalAlignment);

                paint.Dispose();
            }
        }

        protected override void OnUpdateHitTestPath(SKImageInfo info)
        {
            if (CanvasSizeChanged)
            {
                HitTestPath = new SKPath();

                HitTestPath.AddRect(_destRect);
            }
        }

        private void OnTwoFingerDragged(object sender, SKFigureTouchEventArgs args)
        {
            SKPoint previousPivotedPoint = args.PreviousPoint;
            SKPoint currentPivotedPoint = args.CurrentPoint;
            SKPoint pivotPivotedPoint = args.PivotPoint;

            SKMatrix changedMatrix = SKUtil.CaculateTwoFingerDraggedMatrix(previousPivotedPoint, currentPivotedPoint, pivotPivotedPoint, ManipulationMode);

            Matrix = Matrix.PostConcat(changedMatrix);
        }

        private void OnOneFingerDragged(object sender, SKFigureTouchEventArgs args)
        {
            SKPoint previousPivotedPoint = args.PreviousPoint;
            SKPoint currentPivotedPoint = args.CurrentPoint;
            SKPoint pivotPivotedPoint = Matrix.MapPoint(0, 0); //新坐标系下，图片的原点

            SKMatrix changedMatrix = SKUtil.CaculateOneFingerDraggedMatrix(previousPivotedPoint, currentPivotedPoint, pivotPivotedPoint, ManipulationMode);

            Matrix = Matrix.PostConcat(changedMatrix);
        }


        #region Dispose Pattern

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!_disposed)
            {
                if (disposing)
                {
                    // managed
                    _bitmap?.Dispose();
                }

                //unmanaged

                _disposed = true;
            }
        }

        #endregion Dispose Pattern
    }
}