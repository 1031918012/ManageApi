using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class ClockRecord : IAggregateRoot
    {
        /// <summary>
        /// 打卡记录id
        /// </summary>
        public int ClockRecordId { get; set; }
        /// <summary>
        /// 人员唯一编号
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime AttendanceDate { get; set; }
        /// <summary>
        /// 班次时间
        /// </summary>
        public DateTime ShiftTime { get; set; }
        /// <summary>
        /// 打卡时间
        /// </summary>
        public DateTime ClockTime { get; set; }
        /// <summary>
        /// 打卡地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 打卡精度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 打卡纬度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 打卡类型 内勤 外勤 补卡 
        /// </summary>
        public ClockTypeEnum ClockType { get; set; }
        /// <summary>
        /// 打卡结果 正常 迟到 早退
        /// </summary>
        public ClockResultEnum Result { get; set; }
        /// <summary>
        /// 打卡图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 打卡备注
        /// </summary>
        public string Remark { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ClockTypeEnum
    {
        /// <summary>
        /// 内勤
        /// </summary>
        [Description("正常内勤")]
        Inside = 1,
        /// <summary>
        /// 外勤
        /// </summary>
        [Description("正常外勤")]
        Outside = 2,
        /// <summary>
        /// 补卡
        /// </summary>
        [Description("补卡")]
        Supplement = 3,
        /// <summary>
        /// 拍照内勤
        /// </summary>
        [Description("拍照内勤")]
        SpecialInside = 4,
        /// <summary>
        /// 拍照外勤
        /// </summary>
        [Description("拍照外勤")]
        SpecialOutside = 5,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ClockResultEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 迟到
        /// </summary>
        [Description("迟到")]
        Late = 2,
        /// <summary>
        /// 早退
        /// </summary>
        [Description("早退")]
        EarlyLeave = 3,
    }
}
