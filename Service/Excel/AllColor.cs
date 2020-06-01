using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    /// <summary>
    /// 单元格，字体，边框，rgb颜色
    /// </summary>
    public class AllColor
    {
        /// <summary>
        /// 单元格颜色，文本颜色，边框颜色()
        /// </summary>
        /// <param name="CellColor"></param>
        /// <param name="FontColor"></param>
        /// <param name="BoderColor"></param>
        public AllColor(RGBColor CellColor, RGBColor FontColor, RGBColor BoderColor)
        {
            this.CellColor = CellColor;
            this.FontColor = FontColor;
            this.BoderColor = BoderColor;

        }
        /// <summary>
        /// 单元格
        /// </summary>
        public RGBColor CellColor { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        public RGBColor FontColor { get; set; }
        /// <summary>
        /// 边框
        /// </summary>
        public RGBColor BoderColor { get; set; }
    }
}
