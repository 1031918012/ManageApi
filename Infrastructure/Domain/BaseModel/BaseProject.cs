using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    /// <summary>
    /// 分组项和薪资项基类
    /// </summary>
    public class BaseProject
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortNumber { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        [NotMapped]
        public string Formula { get; set; }
    }
}
