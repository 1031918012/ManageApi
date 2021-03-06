﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class ClockStatistics : IAggregateRoot
    {
        /// <summary>
        /// 打卡统计主键
        /// </summary>
        public int ClockStatisticsId { get; set; }
        /// <summary>
        /// 人员唯一编号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 设置的时间
        /// </summary>
        public DateTime SettingTime { get; set; }
        /// <summary>
        /// 打卡主键
        /// </summary>
        public string ClockId { get; set; }
        /// <summary>
        /// 迟到次数
        /// </summary>
        public int LateTime { get; set; }
        /// <summary>
        /// 迟到时间
        /// </summary>
        public TimeSpan LateMinutes { get; set; }
        /// <summary>
        /// 早退次数
        /// </summary>
        public int EarlyLeaveTimes { get; set; }
        /// <summary>
        /// 早退时间
        /// </summary>
        public TimeSpan EarlyLeaveMinutes { get; set; }
        /// <summary>
        /// 工作时长
        /// </summary>
        public TimeSpan WorkTime { get; set; }
        /// <summary>
        /// 上班缺卡次数
        /// </summary>
        public int NotClockInTimes { get; set; }
        /// <summary>
        /// 下班缺卡次数
        /// </summary>
        public int NotClockOutTimes { get; set; }
        /// <summary>
        /// 工作日加班时长
        /// </summary>
        public TimeSpan WorkingOvertime { get; set; }
        /// <summary>
        /// 休息日加班时长
        /// </summary>
        public TimeSpan RestOvertime { get; set; }
        /// <summary>
        /// 节假日加班时长
        /// </summary>
        public TimeSpan HolidayOvertime { get; set; }
        /// <summary>
        /// 加班总时长
        /// </summary>
        public TimeSpan AllOvertime { get; set; }
        /// <summary>
        /// 出差时长
        /// </summary>
        public TimeSpan Travel { get; set; }
        /// <summary>
        /// 外出时长
        /// </summary>
        public TimeSpan GoOut { get; set; }
        /// <summary>
        /// 请假时长
        /// </summary>
        public TimeSpan Leave { get; set; }
        /// <summary>
        /// 应出勤天数
        /// </summary>
        public int AttendanceDay { get; set; }
        /// <summary>
        /// 应出勤时长
        /// </summary>
        public TimeSpan AttendanceTime { get; set; }
        /// <summary>
        /// 实出勤天数
        /// </summary>
        public int ActualAttendanceDay { get; set; }
        /// <summary>
        /// 实出勤时长
        /// </summary>
        public TimeSpan ActualAttendanceTime { get; set; }
    }
}
