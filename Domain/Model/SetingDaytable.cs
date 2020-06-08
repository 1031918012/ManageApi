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
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 打卡时段
        /// </summary>
        public string ShiftDetails { get; set; }
        /// <summary>
        /// 是否存在豁免时间(定义：迟到多少分钟不算，早退多少分钟不算)
        /// </summary>
        public bool IsExemption { get; set; }
        /// <summary>
        /// 豁免迟到分钟数
        /// </summary>
        public int LateMinutes { get; set; }
        /// <summary>
        /// 豁免早退分钟数
        /// </summary>
        public int EarlyLeaveMinutes { get; set; }
        /// <summary>
        /// 是否存在弹性时间
        /// </summary>
        public bool IsFlexible { get; set; }
        /// <summary>
        /// 早到早走
        /// </summary>
        public int EarlyFlexible { get; set; }
        /// <summary>
        /// 晚到晚走
        /// </summary>
        public int LateFlexible { get; set; }
    }
}
