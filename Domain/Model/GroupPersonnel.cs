using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 考勤分组人员关系表
    /// </summary>
    public class GroupPersonnel : IAggregateRoot
    {
        /// <summary>
        /// 分组人员ID
        /// </summary>
        public string GroupPersonnelID { get; set; }
        /// <summary>
        /// 考勤组ID
        /// </summary>
        public string AttendanceGroupID { get; set; }
        /// <summary>
        /// 分组人员身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
    }
}
