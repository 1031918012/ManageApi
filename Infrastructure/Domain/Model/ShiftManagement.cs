using Infrastructure;
using System;
using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// 班次管理
    /// </summary>
    public class ShiftManagement : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ShiftID { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 考勤时间
        /// </summary>
        public string AttendanceTime { get; set; }
        /// <summary>
        /// 工作时长
        /// </summary>
        public decimal WorkHours { get; set; }
        /// <summary>
        /// 班次备注
        /// </summary>
        public string ShiftRemark { get; set; }
        /// <summary>
        /// 打卡规则
        /// </summary>
        public ClockRuleEnum ClockRule { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
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
        /// 是否删除
        /// </summary>
        public bool IsDelete { get; set; }
        /// <summary>
        /// 是否存在豁免时间(定义：迟到多少分钟不算，早退多少分钟不算)
        /// </summary>
        public bool IsExemption { get; set; }
        /// <summary>
        /// 豁免迟到分钟数
        /// </summary>
        public int LateMinutes { get; set; }
        /// <summary>
        /// 豁免早退分钟数
        /// </summary>
        public int EarlyLeaveMinutes { get; set; }
        /// <summary>
        /// 是否存在弹性时间(晚打卡,则晚下班)
        /// </summary>
        public bool IsFlexible { get; set; }
        /// <summary>
        /// 弹性分钟数
        /// </summary>
        public int FlexibleMinutes { get; set; }

        /// <summary>
        /// 班次上下班时间管理
        /// </summary>
        public virtual List<ShiftTimeManagement> ShiftTimeManagementList { get; set; }
    }
}
