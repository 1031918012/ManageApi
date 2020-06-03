using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class SetingDaytable : IAggregateRoot
    {
        /// <summary>
        /// 人员考勤组关系ID
        /// </summary>
        public int SetingDaytableId { get; set; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 人员唯一编号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 设置的时间
        /// </summary>
        public DateTime SettingTime { get; set; }
        /// <summary>
        /// 关联的考勤组
        /// </summary>
        public int AttendanceGroupId { get; set; }
        /// <summary>
        /// 所属客户编号
        /// </summary>
        public string CompanyId { get; set; }
    }
}
