// ******************************************************************
// Copyright (c) Tomasz Romaszkiewicz. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THE CODE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
// DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE CODE OR THE USE OR OTHER DEALINGS IN THE CODE.
// ******************************************************************

namespace DXCharts.Controls.Charts
{
    using System;
    using ChartElements.Interfaces;
    using Classes;
    using Windows.Foundation;
    using Windows.UI.Xaml;
    using System.Collections.Specialized;
    using System.Threading;

    public sealed class CartesianChart : ChartBase
    {
        /// <summary>
        /// 水平坐标
        /// </summary>
        public IChartAxis HorizontalAxis
        {
            get { return (IChartAxis)GetValue(HorizontalAxisProperty); }
            set { SetValue(HorizontalAxisProperty, value); }
        }

        public static readonly DependencyProperty HorizontalAxisProperty =
            DependencyProperty.Register(nameof(HorizontalAxis), typeof(IChartAxis), typeof(CartesianChart), new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// 垂直坐标
        /// </summary>
        public IChartAxis VerticalAxis
        {
            get { return (IChartAxis)GetValue(VerticalAxisProperty); }
            set { SetValue(VerticalAxisProperty, value); }
        }

        public static readonly DependencyProperty VerticalAxisProperty =
            DependencyProperty.Register(nameof(VerticalAxis), typeof(IChartAxis), typeof(CartesianChart), new PropertyMetadata(null, OnPropertyChangedStatic));

        /// <summary>
        /// 当横纵坐标变化后，触发事件重绘界面
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnPropertyChangedStatic(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CartesianChart)?.OnPropertyChanged(d, e.Property);
        }

        /// <summary>
        /// 横纵坐标轴位置
        /// </summary>
        public Point DataOrigin
        {
            get { return (Point)GetValue(DataOriginProperty); }
            set { SetValue(DataOriginProperty, value); }
        }

        public static readonly DependencyProperty DataOriginProperty =
            DependencyProperty.Register(nameof(DataOrigin), typeof(Point), typeof(CartesianChart), new PropertyMetadata(new Point(0, 0), OnPropertyChangedStatic));
        
        /// <summary>
        /// 是否需要重绘界面
        /// </summary>
        public bool NeedToRedraw
        {
            get { return (bool)GetValue(NeedToRedrawProperty); }
            set { SetValue(NeedToRedrawProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NeedToRedraw.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NeedToRedrawProperty =
            DependencyProperty.Register("NeedToRedraw", typeof(bool), typeof(CartesianChart), new PropertyMetadata(false, OnNeedToRedrawChanged));

        public event NotifyCollectionChangedEventHandler NeedToRedrawPropertyChanged;

        private static void OnNeedToRedrawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool bAutoRedraw;
            bAutoRedraw = (d == null)? false:(d as CartesianChart).AutoRedraw;

            // 如果启动了自动重绘界面就不启动手动绘制功能
            if (bAutoRedraw)
            {
                d.SetValue(NeedToRedrawProperty, false); // 重置标志位
                return;
            }

            if ((bool)(e.NewValue) == true)
            {
                (d as CartesianChart)?.NeedToRedrawPropertyChanged?.Invoke(null, null);
                d.SetValue(NeedToRedrawProperty, false); // 重绘后，重置标志位
            }
        }

        /// <summary>
        /// 是否自动刷新界面
        /// </summary>
        public bool AutoRedraw
        {
            get { return (bool)GetValue(AutoRedrawProperty); }
            set { SetValue(AutoRedrawProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoRedraw.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoRedrawProperty =
            DependencyProperty.Register("AutoRedraw", typeof(bool), typeof(CartesianChart), new PropertyMetadata(false, OnAutoRedrawChanged));

        private static void OnAutoRedrawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
        
        /// <summary>
        /// 波形刷新帧率，默认10FPS
        /// </summary>
        public int FramesPerSecond
        {
            get { return (int)GetValue(FramesPerSecondProperty); }
            set { SetValue(FramesPerSecondProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FramesPerSecond.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FramesPerSecondProperty =
            DependencyProperty.Register("FramesPerSecond", typeof(int), typeof(CartesianChart), new PropertyMetadata(10));




        private Timer _redrawTimer;

        public CartesianChart() : base()
        {
            // 初始化重绘事件
            NeedToRedrawPropertyChanged += DataPresenter_CollectionChanged;

            this.Loaded += CartesianChart_Loaded;
            
        }

        private void CartesianChart_Loaded(object sender, RoutedEventArgs e)
        {
            if (AutoRedraw)
            {
                int time = 1000 / FramesPerSecond;
                _redrawTimer = new Timer(new TimerCallback(OnRedrawTimer), null, 0, time);
            }
        }

        private async void OnRedrawTimer(object data)
        {
            // 当开启自动刷新时，每个100ms自动刷新一次界面
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (AutoRedraw)
                {
                    OnPropertyChanged(this, null);
                }
            });
        }

        public override void CreateAxes()
        {
            if (HorizontalAxis != null)
            {
                AxesCollection.Add(HorizontalAxis);
                HorizontalAxis.DataOrigin = DataOrigin.X;
            }

            if (VerticalAxis != null)
            {
                AxesCollection.Add(VerticalAxis);
                VerticalAxis.DataOrigin = DataOrigin.Y;
            }

        }

        public override void PrepareDataPresenter()
        {
            if (DataPresenter != null)
            {
                DataPresenter.Convert = Convert;
                DataPresenter.IsPointInRange = IsInRange;
                DataPresenter.CollectionChanged += DataPresenter_CollectionChanged;
            }
        }

        /// <summary>
        /// 重绘事件触发函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataPresenter_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // redraw the chart
            OnPropertyChanged(this, null);
        }

        private bool IsInRange(Point point) => VisibleRange.InRange(point);

        /// <summary>
        /// 将数据点值转换成像素点值
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private ChartPoint Convert(Point point)
        {
            return new ChartPoint(GetXCoordinate(point.X), GetYCoordinate(point.Y));
        }

        /// <summary>
        /// 将数据X坐标转换成像素X坐标
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float GetXCoordinate(double data)
        {
            return (float)(rootCanvas.ActualWidth * (data - VisibleRange.Minimum.X) / VisibleRange.Width);
        }
        /// <summary>
        /// 将数据y坐标转换成像素y坐标
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private float GetYCoordinate(double data)
        {
            return (float)(rootCanvas.ActualHeight * (VisibleRange.Maximum.Y - data) / VisibleRange.Height);
        }

        /// <summary>
        /// 更新坐标参数，绘制坐标时要用。
        /// </summary>
        /// <param name="newSize"></param>
        public override void UpdateAxes(ElementSize newSize)
        {
            if (HorizontalAxis != null && VisibleRange.InVerticalRange(DataOrigin.Y))
            {
                HorizontalAxis.StartPoint = new ChartPoint(0.0f, GetYCoordinate(DataOrigin.Y));
                HorizontalAxis.EndPoint = new ChartPoint(newSize.Width, GetYCoordinate(DataOrigin.Y));
                HorizontalAxis.DataXRatio = newSize.Width / VisibleRange.Width;
                HorizontalAxis.DataYRatio = newSize.Height / VisibleRange.Height;
                HorizontalAxis.OriginPoint = GetXCoordinate(DataOrigin.X);
                HorizontalAxis.MaxLine = newSize.Height;
                HorizontalAxis.VisibleRange = this.VisibleRange;
            }

            if (VerticalAxis != null && VisibleRange.InHorizontalRange(DataOrigin.X))
            {
                VerticalAxis.EndPoint = new ChartPoint(GetXCoordinate(DataOrigin.X), 0.0f);
                VerticalAxis.StartPoint = new ChartPoint(GetXCoordinate(DataOrigin.X), newSize.Height);
                VerticalAxis.DataYRatio = newSize.Height / VisibleRange.Height;
                VerticalAxis.DataXRatio = newSize.Width / VisibleRange.Width;
                VerticalAxis.OriginPoint = GetYCoordinate(DataOrigin.Y);
                VerticalAxis.MaxLine = newSize.Width;
                VerticalAxis.VisibleRange = this.VisibleRange;
            }
        }

    }
}
