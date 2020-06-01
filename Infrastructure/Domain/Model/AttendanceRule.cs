using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 考勤规则基本信息
    /// </summary>
    public class AttendanceRule : IAggregateRoot
    {
        /// <summary>
        /// 考勤规则id
        /// </summary>
        public string AttendanceRuleID { get; set; }
        /// <summary>
        /// 考勤规则名称
        /// </summary>
        public string RuleName { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 创建者id
        /// </summary>
        public string CreatorID { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 迟到处理规则
        /// </summary>
        public LateEnum LateRule { get; set; }
        /// <summary>
        /// 早退处理规则
        /// </summary>
        public EarlyLeaveEnum EarlyLeaveRule { get; set; }
        /// <summary>
        /// 缺卡处理规则
        /// </summary>
        public NotClockEnum NotClockRule { get; set; }
        /// <summary>
        /// 是否被用过
        /// </summary>
        public bool IsUsed { get; set; }
        /// <summary>
        /// 是否被用过
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
