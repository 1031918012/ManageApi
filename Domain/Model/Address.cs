using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Address : IAggregateRoot
    {
        /// <summary>
        /// 考勤地址id
        /// </summary>
        public int AddressId { get; set; }
        /// <summary>
        /// 考勤组id
        /// </summary>
        public int AttendanceGroupId { get; set; }
        /// <summary>
        /// 考勤地址名称
        /// </summary>
        public string AddressName { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string AddressDetailName { get; set; }
        /// <summary>
        /// 纬度（GCJ-02）
        /// </summary>
        public double Latitude { get; set; }
        /// <summary>
        /// 经度（GCJ-02）
        /// </summary>
        public double Longitude { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public int Distance { get; set; }
    }
}
