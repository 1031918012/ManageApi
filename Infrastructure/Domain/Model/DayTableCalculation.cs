using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 日表计算规则
    /// </summary>
    public class DayTableCalculation : IAggregateRoot
    {
        /// <summary>
        /// 考勤组id
        /// </summary>
        public int DayTableCalculationID { get; set; }
        /// <summary>
        /// 日表id
        /// </summary>
        public string AttendanceRecordID { get; set; }
        /// <summary>
        /// 考勤规则ID
        /// </summary>
        public string AttendanceRuleID { get; set; }
        /// <summary>
        /// 班次类型
        /// </summary>
        public ShiftTypeEnum ShiftType { get; set; }
        /// <summary>
        /// 考勤地点（SiteDTO）
        /// </summary>
        public string SiteAttendance { get; set; }
        /// <summary>
        /// 法定节假日是否自动排休
        /// </summary>
        public bool IsDynamicRowHugh { get; set; }
        /// <summary>
        /// 是否节假日
        /// </summary>
        public bool IsHoliday { get; set; }
        /// <summary>
        /// 是否调班
        /// </summary>
        public bool IsWorkPaidLeave { get; set; }
        /// <summary>
        /// 周几
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// 是否上班
        /// </summary>
        public bool IsHolidayWork { get; set; }
        /// <summary>
        /// 工作时长
        /// </summary>
        public decimal WorkHours { get; set; }
        /// <summary>
        /// 打卡规则
        /// </summary>
        public ClockRuleEnum ClockRule { get; set; }
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
        /// 是否存在弹性时间(晚打卡,则晚下班)
        /// </summary>
        public bool IsFlexible { get; set; }
        /// <summary>
        /// 弹性分钟数
        /// </summary>
        public int FlexibleMinutes { get; set; }

        /// <summary>
        /// 班次上下班时间管理
        /// </summary>
        public string ShiftTimeManagementList { get; set; }
        /// <summary>
        /// 工作日计算方式
        /// </summary>
        public CalculationMethodEnum WorkingCalculationMethod { get; set; }
        /// <summary>
        /// 休息日计算方式
        /// </summary>
        public CalculationMethodEnum RestCalculationMethod { get; set; }
        /// <summary>
        /// 节假日计算方式
        /// </summary>
        public CalculationMethodEnum HolidayCalculationMethod { get; set; }
        /// <summary>
        /// 工作日补偿模式
        /// </summary>
        public CompensationModeEnum WorkingCompensationMode { get; set; }
        /// <summary>
        /// 休息日补偿模式
        /// </summary>
        public CompensationModeEnum RestCompensationMode { get; set; }
        /// <summary>
        /// 节假日补偿模式
        /// </summary>
        public CompensationModeEnum HolidayCompensationMode { get; set; }
        /// <summary>
        /// 不计加班时长限制
        /// </summary>
        public int ExcludingOvertime { get; set; }
        /// <summary>
        /// 最长加班时长限制
        /// </summary>
        public int LongestOvertime { get; set; }
        /// <summary>
        /// 最短加班时长限制
        /// </summary>
        public int MinimumOvertime { get; set; }
    }
}
