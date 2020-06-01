using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    /// <summary>
    /// rgb颜色类
    /// </summary>
    public class RGBColor
    {
        /// <summary>
        /// 新建一个RGB颜色类
        /// </summary>
        /// <param name="index"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        public RGBColor(short index, byte red, byte green, byte blue)
        {
            Index = index;
            Red = red;
            Green = green;
            Blue = blue;

        }
        public RGBColor()
        {

        }
        /// <summary>
        /// 编号(用于兼容HSSF中的调色板编号)
        /// </summary>
        public short Index { get; set; }
        /// <summary>
        /// 红
        /// </summary>
        public byte Red { get; set; }
        /// <summary>
        /// 绿
        /// </summary>
        public byte Green { get; set; }
        /// <summary>
        /// 蓝
        /// </summary>
        public byte Blue { get; set; }
    }
}
