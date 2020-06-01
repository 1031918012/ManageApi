using Infrastructure;
using System;
using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// 考勤项类别
    /// </summary>
    public class AttendanceItemCatagory : IAggregateRoot
    {
        /// <summary>
        /// ID 标识 考勤项类别
        /// </summary>
        public string AttendanceItemCatagoryID { get; set; }
        /// <summary>
        /// 考勤项类别名称
        /// </summary>
        public string AttendanceItemCatagoryName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 考勤项
        /// </summary>
        public virtual ICollection<AttendanceItem> AttendanceItemList { get; set; }
    }
}