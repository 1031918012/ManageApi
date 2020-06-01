using Infrastructure;
using System;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 考勤项单位
    /// </summary>
    public class AttendanceItemUnit : IEntity
    {
        /// <summary>
        /// ID 标识 考勤项单位
        /// </summary>
        public string AttendanceItemUnitID { get; set; }
        /// <summary>
        /// 考勤项单位名称
        /// </summary>
        public string AttendanceItemUnitName { get; set; }
        /// <summary>
        /// 考勤项ID
        /// </summary>
        public string AttendanceItemID { get; set; }
        /// <summary>
        /// 考勤项名称
        /// </summary>
        public string AttendanceItemName { get; set; }
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
    }
}

