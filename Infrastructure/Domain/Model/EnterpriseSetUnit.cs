using Infrastructure;
using System;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 企业设置单位
    /// </summary>
    public class EnterpriseSetUnit : IEntity
    {
        /// <summary>
        /// ID 标识 企业设置单位
        /// </summary>
        public string EnterpriseSetUnitID { get; set; }
        /// <summary>
        /// 排序编号
        /// </summary>
        public int SortNumber { get; set; }
        /// <summary>
        /// 企业设置ID
        /// </summary>
        public string EnterpriseSetID { get; set; }
        /// <summary>
        /// 考勤项单位ID
        /// </summary>
        public string AttendanceItemUnitID { get; set; }
        /// <summary>
        /// 考勤项单位名称
        /// </summary>
        public string AttendanceItemUnitName { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public bool IsSelect { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
    }
}


