using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXCharts.Controls.ChartElements.Primitives
{
    using Interfaces;
    using Classes;
    using Microsoft.Graphics.Canvas;
    using Microsoft.Graphics.Canvas.Geometry;
    using Windows.UI;

    public class LayeringAxis : IChartAxis
    {
        /// <summary>
        /// Color of the axis
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// End point of the axis
        /// </summary>
        public ChartPoint EndPoint { get; set; }

        /// <summary>
        /// If axis is working in inverse mode
        /// </summary>
        public bool IsInverse { get; set; }

        /// <summary>
        /// Axis start point
        /// </summary>
        public ChartPoint StartPoint { get; set; }

        /// <summary>
        /// Axis stroke style
        /// </summary>
        public CanvasStrokeStyle StrokeStyle { get; set; }

        /// <summary>
        /// Axis thickness
        /// </summary>
        public double Thickness { get; set; }

        /// <summary>
        /// Element used as arrowhead
        /// </summary>
        public IChartPointElement ArrowHead { get; set; }

        /// <summary>
        /// Element used as tick
        /// </summary>
        public TickBase Tick { get; set; }

        /// <summary>
        /// Data incerement for tick placememnt
        /// </summary>
        public double TickIncrement { get; set; }

        /// <summary>
        /// Pixels per data X
        /// </summary>
        public double DataXRatio { get; set; }

        /// <summary>
        /// Pixels per data Y
        /// </summary>
        public double DataYRatio { get; set; }

        /// <summary>
        /// Orgin of data
        /// </summary>
        public double DataOrigin { get; set; }

        /// <summary>
        /// Orgin of axis
        /// </summary>
        public float OriginPoint { get; set; }

        /// <summary>
        /// 网格坐标或者分层坐标最大位置，起始默认从0开始
        /// </summary>
        public float MaxLine { get; set; }

        /// <summary>
        /// 知道控件的显示范围，用于绘制坐标
        /// </summary>
        public DataRange VisibleRange { get; set; }

        public LayeringAxis()
        {
            this.Color = Colors.Gray;
            this.IsInverse = false;
            this.StartPoint = default(ChartPoint);
            this.EndPoint = default(ChartPoint);
            this.ArrowHead = null;
            this.Thickness = 1.0f;
            this.Tick = null;
            this.TickIncrement = 0.0f;
            this.DataOrigin = 0.0d;
            this.StrokeStyle = default(CanvasStrokeStyle);
            this.MaxLine = 0.0f;
            this.VisibleRange = default(DataRange);
        }

        public void DrawOnCanvas(CanvasDrawingSession drawingSession)
        {
            //drawingSession.DrawLine(this.StartPoint.X, this.StartPoint.Y, this.EndPoint.X, this.EndPoint.Y, this.Color, (float)this.Thickness, this.StrokeStyle);

            bool isHorizontal = Math.Abs(this.EndPoint.X - this.StartPoint.X) > Math.Abs(this.EndPoint.Y - this.StartPoint.Y);

            if (VisibleRange == null)
            {
                return;
            }

            if ((this.TickIncrement > 0.0f) && (this.MaxLine > 0.0f))
            {
                float curLine = 0.0f;
                float spaceRadio = 0.02f;

                if (isHorizontal)
                {
                    if ((this.EndPoint.X - this.StartPoint.X) < 200)
                    {

                    }
                    curLine += (float)(this.TickIncrement * this.DataYRatio);
                    double lableValue = VisibleRange.Maximum.Y - this.TickIncrement;
        
                    float distence = (float)(VisibleRange.Width * spaceRadio * DataXRatio);
                    while (curLine <= MaxLine)
                    {
                        if ((curLine > (MaxLine * spaceRadio)) && ((curLine < (MaxLine * (1 - 2 * spaceRadio)))))
                        {
                            drawingSession.DrawText($"{lableValue:0.0}", this.EndPoint.X - distence * 2 + 5, curLine - 10, this.Color, new Microsoft.Graphics.Canvas.Text.CanvasTextFormat() { FontSize = 12 });
                            drawingSession.DrawLine(this.StartPoint.X + distence, curLine, this.EndPoint.X - distence * 2, curLine, this.Color, (float)this.Thickness, this.StrokeStyle);
                        }
                        lableValue -= this.TickIncrement;
                        curLine += (float)(this.TickIncrement * this.DataYRatio);
                    }
                }
                else
                {
                    curLine += (float)(this.TickIncrement * this.DataXRatio);
                    float distence = (float)(VisibleRange.Height * spaceRadio * DataYRatio);
                    double lableValue = VisibleRange.Minimum.X + this.TickIncrement;
                    while (curLine <= MaxLine)
                    {
                        if ((curLine > (MaxLine * spaceRadio)) && ((curLine < (MaxLine * (1 - 2 * spaceRadio)))))
                        {
                            drawingSession.DrawLine(curLine, this.StartPoint.Y - distence * 2, curLine, this.EndPoint.Y + distence, this.Color, (float)this.Thickness, this.StrokeStyle);
                            drawingSession.DrawText($"{lableValue:0.0}", curLine - 10, this.StartPoint.Y - 15, this.Color, new Microsoft.Graphics.Canvas.Text.CanvasTextFormat() { FontSize = 12 });
                        }
                        lableValue += this.TickIncrement;
                        curLine += (float)(this.TickIncrement * this.DataXRatio);
                    }
                }
            }
        }
    }
}
