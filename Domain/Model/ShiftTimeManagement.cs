using System;

namespace Domain
{
    /// <summary>
    /// 班次上下班时间管理
    /// </summary>
    public class ShiftTimeManagement : IAggregateRoot
    {
        /// <summary>
        /// 班次时间ID
        /// </summary>
        public string ShiftTimeID { get; set; }
        /// <summary>
        /// 班次ID
        /// </summary>
        public string ShiftID { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 班次打卡时段数
        /// </summary>
        public int ShiftTimeNumber { get; set; } 

        #region 上下班时间
        /// <summary>
        /// 上班时间
        /// </summary>
        public DateTime StartWorkTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public DateTime EndWorkTime { get; set; }
        /// <summary>
        /// 开始休息时间
        /// </summary>
        public DateTime? StartRestTime { get; set; }
        /// <summary>
        /// 结束休息时间
        /// </summary>
        public DateTime? EndRestTime { get; set; }
        #endregion

        #region 有效打卡时段
        /// <summary>
        /// 上班打卡时段始
        /// </summary>
        public DateTime UpStartClockTime { get; set; }
        /// <summary>
        /// 上班打卡时段末
        /// </summary>
        public DateTime UpEndClockTime { get; set; }
        /// <summary>
        /// 下班打卡时段始
        /// </summary>
        public DateTime DownStartClockTime { get; set; }
        /// <summary>
        /// 下班打卡时段末
        /// </summary>
        public DateTime DownEndClockTime { get; set; }
        #endregion
    }
}
