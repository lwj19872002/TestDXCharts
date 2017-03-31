using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using System.Threading;

using GalaSoft.MvvmLight;
using DXCharts.Controls.Classes;
using DXCharts.Controls.ChartElements.Interfaces;
using DXCharts.Controls.ChartElements.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TestDXCharts
{
    public class MainPageVM : ViewModelBase
    {
        private DataRange _C1DataRange;
        /// <summary>
        /// 
        /// </summary>
        public DataRange C1DataRange
        {
            get { return _C1DataRange; }
            set
            {
                _C1DataRange = value;
                this.RaisePropertyChanged("C1DataRange");
            }
        }

        private IChartDataPresenter _C1DataPre;
        /// <summary>
        /// 
        /// </summary>
        public IChartDataPresenter C1DataPre
        {
            get { return _C1DataPre; }
            set
            {
                _C1DataPre = value;
                this.RaisePropertyChanged("C1DataPre");
            }
        }

        private List<Point> _C1Points;
        /// <summary>
        /// 
        /// </summary>
        public List<Point> C1Points
        {
            get { return _C1Points; }
            set
            {
                _C1Points = value;
                this.RaisePropertyChanged("C1Points");
            }
        }



        private DataRange _C2DataRange;
        /// <summary>
        /// 
        /// </summary>
        public DataRange C2DataRange
        {
            get { return _C2DataRange; }
            set
            {
                _C2DataRange = value;
                this.RaisePropertyChanged("C2DataRange");
            }
        }

        private bool _bNeedToRedraw;
        /// <summary>
        /// 
        /// </summary>
        public bool BNeedToRedraw
        {
            get { return _bNeedToRedraw; }
            set
            {
                _bNeedToRedraw = value;
                this.RaisePropertyChanged("BNeedToRedraw");
            }
        }


        private UInt32 _dataFlag;

        private Timer _DataTimer;
        private Timer _CurveTimer;

        private LinePresenter _linePre;

        private MainPage _wnd;

        public List<Point> _dataSrc;
        //public List<Point> C1Points => _dataSrc.ToList();

        public MainPageVM(MainPage wnd)
        {
            _dataFlag = 0;
            _wnd = wnd;

            _dataSrc = new List<Point>();
            C1Points = _dataSrc;

            for (int i = 0; i < 200; i++)
            {
                double y = Math.Sin((i / 200.0) * 2.0 * Math.PI) * 20.0;
                _dataSrc.Add(new Point(i, y));
            }

            C1DataRange = new DataRange(0, -20, 200, 20);

            StandardLine line = new StandardLine();
            line.Color = Colors.Blue;
            line.Thickness = 2.0;

            _linePre = new LinePresenter();
            _linePre.LineElement = line;

            //Binding dataBind = new Binding();
            //dataBind.Source = C1Points;
            //dataBind.Mode = BindingMode.TwoWay;

            //_wnd.SetBinding(LinePresenter.DataProperty, dataBind);

            C1DataPre = _wnd.Resources["LinePresenter"] as IChartDataPresenter;

            C2DataRange = new DataRange(-30, -10, 30, 10);

            _DataTimer = new Timer(new TimerCallback(DataUpdateHandler), null, 0, 10);
            _CurveTimer = new Timer(new TimerCallback(CurveUpdateHandler), this, 0, 100);
        }

        private async void DataUpdateHandler(object data)
        {
            await _wnd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                for (int i = 0; i < (200 - 1); i++)
                {
                    Point p = _dataSrc[i];
                    p.Y = _dataSrc[i + 1].Y;
                    _dataSrc[i] = p;
                }

                double y = Math.Sin((_dataFlag / 50.0) * 2.0 * Math.PI) * 20.0;
                Point pp = _dataSrc[199];
                pp.Y = y;
                _dataSrc[199] = pp;

                _dataFlag++;
                if (_dataFlag >= 200)
                {
                    _dataFlag = 0;
                }
            });
        }

        private async void CurveUpdateHandler(object data)
        {
            await _wnd.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                //BNeedToRedraw = true;
                //_linePre.Data = C1Points;
                //C1Points = _dataSrc.ToList();
                //(data as MainPageVM).RaisePropertyChanged("C1Points");
                //this.RaisePropertyChanged("C1DataPre");
                //C1DataPre = _linePre;
            });
        }
    }
}
