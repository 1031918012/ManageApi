﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class ShiftDetail : IAggregateRoot
    {
        /// <summary>
        /// 班次详细Id
        /// </summary>
        public int ShiftDetailId { get; set; }
        /// <summary>
        /// 班次ID
        /// </summary>
        public int ShiftId { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public TimeSpan StartWorkTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public TimeSpan EndWorkTime { get; set; }
        /// <summary>
        /// 是否开启休息时间
        /// </summary>
        public bool IsEnableRest {get;set;}
        /// <summary>
        /// 开始休息时间
        /// </summary>
        public TimeSpan StartRestTime { get; set; }
        /// <summary>
        /// 结束休息时间
        /// </summary>
        public TimeSpan EndRestTime { get; set; }
        /// <summary>
        /// 是否开启打卡时段
        /// </summary>
        public bool IsEnableTime{get;set;}
        /// <summary>
        /// 上班打卡时段始
        /// </summary>
        public TimeSpan UpStartClockTime { get; set; }
        /// <summary>
        /// 上班打卡时段末
        /// </summary>
        public TimeSpan UpEndClockTime { get; set; }
        /// <summary>
        /// 下班打卡时段始
        /// </summary>
        public TimeSpan DownStartClockTime { get; set; }
        /// <summary>
        /// 下班打卡时段末
        /// </summary>
        public TimeSpan DownEndClockTime { get; set; }
    }
}
