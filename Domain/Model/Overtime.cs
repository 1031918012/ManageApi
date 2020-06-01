using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 请假表
    /// </summary>
    public class Overtime : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public Guid OvertimeID { get; set; }
        /// <summary>
        /// 加班规则名称
        /// </summary>
        public string OvertimeName { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 工作日计算方式
        /// </summary>
        public CalculationMethodEnum WorkingCalculationMethod { get; set; }
        /// <summary>
        /// 休息日计算方式
        /// </summary>
        public CalculationMethodEnum RestCalculationMethod { get; set; }
        /// <summary>
        /// 节假日计算方式
        /// </summary>
        public CalculationMethodEnum HolidayCalculationMethod { get; set; }
        /// <summary>
        /// 工作日补偿模式
        /// </summary>
        public CompensationModeEnum WorkingCompensationMode { get; set; }
        /// <summary>
        /// 休息日补偿模式
        /// </summary>
        public CompensationModeEnum RestCompensationMode { get; set; }
        /// <summary>
        /// 节假日补偿模式
        /// </summary>
        public CompensationModeEnum HolidayCompensationMode { get; set; }
        /// <summary>
        /// 不计加班时长限制
        /// </summary>
        public int ExcludingOvertime { get; set; }
        /// <summary>
        /// 最长加班时长限制
        /// </summary>
        public int LongestOvertime { get; set; }
        /// <summary>
        /// 最短加班时长限制
        /// </summary>
        public int MinimumOvertime { get; set; }
        /// <summary>
        /// 是否被使用过
        /// </summary>
        public bool IsUsed { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
    }
}
