using System;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 调休上班
    /// </summary>
    public class AttendanceGroup : IAggregateRoot
    {
        /// <summary>
        /// 考勤组id
        /// </summary>
        public int AttendanceGroupId { get; set; }
        /// <summary>
        /// 考勤组名称
        /// </summary>
        public string AttendanceGroupName { get; set; }
        /// <summary>
        /// 考勤类型
        /// </summary>
        public AttendanceTypeEnum AttendanceTypeEnum { get; set; }
        #region 固定考勤类型
        /// <summary>
        /// 固定考勤班次json
        /// </summary>
        public string FixedShift { get; set; }
        /// <summary>
        /// 法定节假日自动排休
        /// </summary>
        public bool AutomaticSchedule { get; set; }
        #endregion
        #region 不固定考勤类型
        /// <summary>
        /// 跨天时间
        /// </summary>
        public TimeSpan TimeAcrossDays { get; set; }
        #endregion
        #region 按排班考勤类型
        /// <summary>
        /// 按排班时间考勤班次json
        /// </summary>
        public string ScheduleShift { get; set; }
        #endregion
        /// <summary>
        /// 加班规则id = -1时默认不开启
        /// </summary>
        public int OvertimeRulesId { get; set; }
        /// <summary>
        /// 是否开启打卡地点
        /// </summary>
        public bool IsClockAddress { get; set; }
        /// <summary>
        /// 是否开启Wifi
        /// </summary>
        public bool IsWifi { get; set; }
        /// <summary>
        /// 所属客户编号
        /// </summary>
        public string CustomerId { get; set; }
    }


    /// <summary>
    /// 考勤类型
    /// </summary>
    public enum AttendanceTypeEnum
    {
        /// <summary>
        /// 不固定时间上下班
        /// </summary>
        [Description("不固定时间上下班")]
        Free = 0,
        /// <summary>
        /// 固定时间上下班
        /// </summary>
        [Description("固定时间上下班")]
        Fixed = 1,
        /// <summary>
        /// 按排班时间上下班
        /// </summary>
        [Description("按排班时间上下班")]
        Schedule = 2,
    }
}
