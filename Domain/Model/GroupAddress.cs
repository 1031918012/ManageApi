using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 考勤组地址关系表
    /// </summary>
    public class GroupAddress : IAggregateRoot
    {
        /// <summary>
        /// 关系表ID
        /// </summary>
        public int GroupAddressID { get; set; }
        /// <summary>
        /// 考勤组id
        /// </summary>
        public string AttendanceGroupID { get; set; }
        /// <summary>
        /// 考勤地址id
        /// </summary>
        public string ClockInAddressID { get; set; }
    }
}
