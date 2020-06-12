using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Wifi : IAggregateRoot
    {
        /// <summary>
        /// 考勤地址id
        /// </summary>
        public int WifiId { get; set; }
        /// <summary>
        /// 考勤组id
        /// </summary>
        public int AttendanceGroupId { get; set; }
        /// <summary>
        /// Wifi名称
        /// </summary>
        public string WifiName { get; set; }
        /// <summary>
        /// Mac地址
        /// </summary>
        public string Mac { get; set; }

    }
}
