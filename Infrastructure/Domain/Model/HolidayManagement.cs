using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 假期管理
    /// </summary>
    public class HolidayManagement: IAggregateRoot
    {
        /// <summary>
        /// 假期名称
        /// </summary>
        public Guid HolidayManagementID { get; set; }
        /// <summary>
        /// 假期名称
        /// </summary>
        public string HolidayName { get; set; }
        /// <summary>
        /// 是否启用余额
        /// </summary>
        public bool EnableBalance { get; set; }
        /// <summary>
        /// 发放方式
        /// </summary>
        public DistributionMethodEnum DistributionMethod { get; set; }
        /// <summary>
        /// 发放周期
        /// </summary>
        public IssuingCycleEnum IssuingCycle { get; set; }
        /// <summary>
        /// 发放日期
        /// </summary>
        public DateOfIssueEnum DateOfIssue { get; set; }
        /// <summary>
        /// 额度规则
        /// </summary>
        public QuotaRuleEnum QuotaRule { get; set; }
        /// <summary>
        /// 固定额度
        /// </summary>
        public decimal FixedData { get; set; }
        /// <summary>
        /// 自发放起多少月
        /// </summary>
        public int ValidityOfLimit { get; set; }
        /// <summary>
        /// 工龄HolidayManagementDTO
        /// </summary>
        public string WorkingYears { get; set; }
        /// <summary>
        /// 司龄HolidayManagementDTO
        /// </summary>
        public string Seniority { get; set; }
        /// <summary>
        /// 属于哪个公司
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsProhibit { get; set; }
    }
}
