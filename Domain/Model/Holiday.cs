using System;

namespace Domain
{
    /// <summary>
    /// 节假日
    /// </summary>
    public class Holiday : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string HolidayID { get; set; }
        /// <summary>
        /// 节假日名称
        /// </summary>
        public string HolidayName { get; set; }
        /// <summary>
        /// 节假日所属年份
        /// </summary>
        public int HolidayYear { get; set; }
        /// <summary>
        /// 放假天数
        /// </summary>
        public int HolidayNumber { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 开始放假时间yyyyMMdd
        /// </summary>
        public DateTime StartHolidayTime { get; set; }
        /// <summary>
        /// 结束放假时间yyyyMMdd
        /// </summary>
        public DateTime EndHolidayTime { get; set; }
    }
}
