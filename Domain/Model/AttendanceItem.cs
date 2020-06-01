using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 考勤项
    /// </summary>
    public class AttendanceItem : IEntity
    {
        /// <summary>
        /// ID 标识 考勤项
        /// </summary>
        public string AttendanceItemID { get; set; }
        /// <summary>
        /// 考勤项名称
        /// </summary>
        public string AttendanceItemName { get; set; }
        /// <summary>
        /// 考勤项类别ID
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
        public virtual ICollection<AttendanceItemUnit> AttendanceItemUnitList { get; set; }
    }
}
