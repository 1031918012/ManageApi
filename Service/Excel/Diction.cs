using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    /// <summary>
    /// 表头和数据的颜色，以及键值对应表头与数据()
    /// </summary>
    public class Diction
    {
        public Diction(string Key = null, string Value = null, AllColor Titlecolor = null, string Postil = null, string[] Select = null, AllColor Datacolor = null)
        {
            this.Key = Key;
            this.Value = Value;
            TitleColor = Titlecolor;
            this.Postil = Postil;
            DataColor = Datacolor;
            this.Select = Select;
        }
        /// <summary>
        /// 键,用于将字段和值匹配
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 值，用于填充表头的文字
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 标题的颜色(本格)
        /// </summary>
        public AllColor TitleColor { get; set; }
        /// <summary>
        /// 标题的批注(本格)
        /// </summary>
        public string Postil { get; set; }
        /// <summary>
        /// 数据的颜色(本格)
        /// </summary>
        public AllColor DataColor { get; set; }
        /// <summary>
        /// 数据的下拉选项以及数据验证(本列)
        /// </summary>
        public string[] Select { get; set; }
    }
}
