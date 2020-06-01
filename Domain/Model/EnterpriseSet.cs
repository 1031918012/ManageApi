using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 企业设置 
    /// </summary>
    public class EnterpriseSet : IAggregateRoot
    {
        /// <summary>
        /// 企业设置ID
        /// </summary>
        public string EnterpriseSetID { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        public int SortNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 关联的考勤项ID
        /// </summary>
        public string AttendanceItemID { get; set; }
        /// <summary>
        /// 关联的考勤项名称
        /// </summary>
        public string AttendanceItemName { get; set; }
        /// <summary>
        /// 是否是启用项
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 企业设置单位
        /// </summary>
        public virtual ICollection<EnterpriseSetUnit> EnterpriseSetUnitList { get; set; }
    }
}
