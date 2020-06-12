using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class HolidayManagement : IAggregateRoot
    {
        /// <summary>
        /// 节假日管理
        /// </summary>
        public int HolidayManagementId { get; set; }
        /// <summary>
        /// 节假日Name
        /// </summary>
        public string HolidayManagementName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 调休上班日期
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// 所属客户编号
        /// </summary>
        public string CustomerId { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// 调休上班日期
        /// </summary>
        public string BalanceTime { get; set; }
        /// <summary>
        /// 调休上班班次
        /// </summary>
        public string BalanceShift { get; set; }
    }
}
