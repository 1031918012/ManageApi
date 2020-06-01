using Infrastructure;
using System;
using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// 考勤组基本信息
    /// </summary>
    public class AttendanceGroup : IAggregateRoot
    {
        /// <summary>
        /// 考勤组id
        /// </summary>
        public string AttendanceGroupID { get; set; }
        /// <summary>
        /// 考勤规则ID
        /// </summary>
        public string AttendanceRuleID { get; set; }
        /// <summary>
        /// 考勤组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 班次类型
        /// </summary>
        public ShiftTypeEnum ShiftType { get; set; }
        /// <summary>
        /// 打卡类型
        /// </summary>
        public ClockInWayEnum ClockInWay { get; set; }
        /// <summary>
        /// 法定节假日是否自动排休
        /// </summary>
        public bool IsDynamicRowHugh { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 考勤人员范围
        /// </summary>
        public string Range { get; set; }
        /// <summary>
        /// 加班规则id
        /// </summary>
        public Guid OvertimeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual ICollection<WeekDaysSetting> WeekDaysSettings { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<GroupPersonnel> GroupPersonnels { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<GroupAddress> GroupAddress { get; set; }
    }
}
