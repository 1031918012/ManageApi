using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Scheduling : IAggregateRoot
    {
        /// <summary>
        /// 排班id
        /// </summary>
        public int SchedulingId { get; set; }
        /// <summary>
        /// 排班id
        /// </summary>
        public string IdCard { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 班次id
        /// </summary>
        public string ShiftId { get; set; }
    }
}
