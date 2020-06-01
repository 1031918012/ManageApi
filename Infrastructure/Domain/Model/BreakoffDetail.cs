using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 请假表
    /// </summary>
    public class BreakoffDetail : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int BreakoffDetailID { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 变动类型（+或者减）  减是-1，加是1
        /// </summary>
        public ChangeTypeEnum ChangeType { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 变动时长（天）
        /// </summary>
        public decimal ChangeTime { get; set; }
        /// <summary>
        /// 当前额度（天）
        /// </summary>
        public decimal CurrentQuota { get; set; }
        /// <summary>
        /// 调整原因
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 日表ID
        /// </summary>
        public string AttendanceRecordID { get; set; }
    }
}
