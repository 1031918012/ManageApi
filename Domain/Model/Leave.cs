using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 请假表
    /// </summary>
    public class Leave : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string LeaveID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 证照号吗
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNumber { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 外出时长
        /// </summary>
        public double AllTime { get; set; }
        /// <summary>
        /// 外出天数
        /// </summary>
        public double AllDay { get; set; }
        /// <summary>
        /// 假期类型
        /// </summary>
        public string LeaveName { get; set; }
    }
}
