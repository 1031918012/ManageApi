using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 工作日设置表
    /// </summary>
    public class WeekDaysSetting:IAggregateRoot
    {
        /// <summary>
        /// 工作表设定id
        /// </summary>
        public string WeekDaysSettingID { get; set; }
        /// <summary>
        /// 考勤组id
        /// </summary>
        public string AttendanceGroupID { get; set; }
        /// <summary>
        /// 周
        /// </summary>
        public int Week { get; set; }
        /// <summary>
        /// 班次id
        /// </summary>
        public string ShiftID { get; set; }
        /// <summary>
        /// 是否上班
        /// </summary>
        public bool IsHolidayWork { get; set; }
    }
}
