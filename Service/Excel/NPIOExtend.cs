using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    /// <summary>
    /// NPIO带数据的参数类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NPIOExtend<T>
    {
        public NPIOExtend(List<T> data, List<Diction> titles, string sheetName = "", string titleFont = "", AllColor titleFontColor = null)
        {
            SheetName = sheetName;
            TitleFont = titleFont;
            Titles = titles;
            Data = data;
            TitleFontColor = titleFontColor;
        }
        /// <summary>
        /// 表名(在excle中创建一个表)
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 标题,(标题内容长度与表头宽度保持一致)
        /// </summary>
        public string TitleFont { get; set; }
        /// <summary>
        /// 表头颜色
        /// </summary>
        public AllColor TitleFontColor { get; set; }
        /// <summary>
        /// 标题部分
        /// </summary>
        public List<Diction> Titles { get; set; }
        /// <summary>
        /// 数据部分
        /// </summary>
        public List<T> Data { get; set; }

    }
}
