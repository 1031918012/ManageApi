using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 打卡详情
    /// </summary>
    public class ClockInfo : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ClockInfoID { get; set; }
        /// <summary>
        /// 日表ID
        /// </summary>
        public string AttendanceRecordID { get; set; }
        /// <summary>
        /// 班次时间段id
        /// </summary>
        public string ShiftTimeID { get; set; }
        /// <summary>
        /// 上班打卡时间
        /// </summary>
        public DateTime ClockInTime { get; set; }
        /// <summary>
        /// 上班打卡结果
        /// </summary>
        public ClockResultEnum ClockInResult { get; set; }
        /// <summary>
        /// 上班打卡地址
        /// </summary>
        public string StartLocation { get; set; }
        /// <summary>
        /// 下班打卡时间
        /// </summary>
        public DateTime ClockOutTime { get; set; }
        /// <summary>
        /// 下班打卡结果
        /// </summary>
        public ClockResultEnum ClockOutResult { get; set; }
        /// <summary>
        /// 下班打卡地址
        /// </summary>
        public string EndLocation { get; set; }
        /// <summary>
        /// 特殊情况统计
        /// </summary>
        public string AttendanceItemDTOJson { get; set; }
    }
}
