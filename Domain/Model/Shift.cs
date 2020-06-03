using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class Shift : IAggregateRoot
    {
        /// <summary>
        /// 人员考勤组关系ID
        /// </summary>
        public int ShiftId { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 打卡时段
        /// </summary>
        public virtual ICollection<ShiftDetail> ShiftDetails { get; set; }
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
        /// 是否存在弹性时间
        /// </summary>
        public bool IsFlexible { get; set; }
        /// <summary>
        /// 早到早走
        /// </summary>
        public int EarlyFlexible { get; set; }
        /// <summary>
        /// 晚到晚走
        /// </summary>
        public int LateFlexible { get; set; }
    }
}
