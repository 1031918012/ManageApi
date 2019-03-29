using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Domain
{
    /// <summary>
    /// 书籍管理
    /// </summary>
    public class ManageItem : IManage
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid BookID { get; set; }
        /// <summary>
        /// 书籍名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool Isdelete { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }
    }
}
