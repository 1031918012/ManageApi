using Infrastructure;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 考勤规则详情
    /// </summary>
    public class AttendanceRuleDetail : IAggregateRoot
    {
        /// <summary>
        /// 考勤规则详情id
        /// </summary>
        public string AttendanceRuleDetailID { get; set; }
        /// <summary>
        /// 考勤规则id
        /// </summary>
        public string AttendanceRuleID { get; set; }
        /// <summary>
        /// 规则类型
        /// </summary>
        public RuleTypeEnum RuleType { get; set; }
        /// <summary>
        /// 最小时间
        /// </summary>
        public int MinTime { get; set; }
        /// <summary>
        /// 判断符号
        /// </summary>
        public SymbolEnum MinJudge { get; set; }
        /// <summary>
        /// 判断符号
        /// </summary>
        public SymbolEnum MaxJudge { get; set; }
        /// <summary>
        /// 最大时间
        /// </summary>
        public int MaxTime { get; set; }
        /// <summary>
        /// 记规则类型
        /// </summary>
        public RuleTypeEnum CallRuleType { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public UnitEnum Unit { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
