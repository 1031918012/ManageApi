using System;

namespace Domain
{
    /// <summary>
    /// 调休上班
    /// </summary>
    public class WorkPaidLeave : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string WorkPaidLeaveID { get; set; }
        /// <summary>
        /// 调休上班日期yyyyMMdd
        /// </summary>
        public DateTime PaidLeaveTime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public TypeEnum Type { get; set; }
        /// <summary>
        /// 关联的节假日ID
        /// </summary>
        public string HolidayID { get; set; }
        /// <summary>
        /// 关联的节假日名称
        /// </summary>
        public string HolidayName { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum TypeEnum
    {
        /// <summary>
        /// 
        /// </summary>
        One = 1,
        /// <summary>
        /// 
        /// </summary>
        Two = 2
    }
}
