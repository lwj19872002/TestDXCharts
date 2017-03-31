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

using DXCharts.Controls.Classes;

namespace DXCharts.Controls.ChartElements.Interfaces
{

    /// <summary>
    /// The interface of axis element
    /// </summary>
    public interface IChartAxis : IChartLineElement
    {
        /// <summary>
        /// Pixels per data X
        /// </summary>
        double DataXRatio { get; set; }

        /// <summary>
        /// Pixels per data Y
        /// </summary>
        double DataYRatio { get; set; }

        /// <summary>
        /// Orgin of data
        /// </summary>
        double DataOrigin { get; set; }

        /// <summary>
        /// Orgin of axis
        /// </summary>
        float OriginPoint { get; set; }

        /// <summary>
        /// 网格坐标或者分层坐标最大位置，起始默认从0开始
        /// </summary>
        float MaxLine { get; set; }

        /// <summary>
        /// 知道控件的显示范围，用于绘制坐标
        /// </summary>
        DataRange VisibleRange { get; set; }
    }
}
