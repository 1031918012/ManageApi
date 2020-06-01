using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 考勤月表
    /// </summary>
    public class AttendanceMonthlyRecord : IAggregateRoot
    {
        /// <summary>
        /// id
        /// </summary>
        public string AttendanceMonthlyRecordID { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 统计时间
        /// </summary>
        public DateTime AttendanceDate { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string EmployeeNo { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 最终考勤项统计数据
        /// </summary>
        public string AttendanceProjectsJson { get; set; }
        /// <summary>
        /// 最终考勤项统计数据
        /// </summary>
        [NotMapped]
        public List<AttendanceItemForComputDTO> AttendanceProjects { get; set; }
    }
}
