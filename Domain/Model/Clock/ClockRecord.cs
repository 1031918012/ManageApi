using System;
using System.Collections.Generic;
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
        public DateTime  AttendanceDate{ get; set; }
        /// <summary>
        /// 打卡时间
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
        public string ClockType { get; set; }
        /// <summary>
        /// 打卡结果 正常 迟到 早退
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 打卡图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 打卡备注
        /// </summary>
        public string Remark { get; set; }
    }
}
