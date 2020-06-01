using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 假期人员关系表
    /// </summary>
    public class PersonHoliday :IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int PersonHolidayID { get; set; }
        /// <summary>
        /// 证照号吗
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 假期名称
        /// </summary>
        public Guid HolidayManagementID { get; set; }
        /// <summary>
        /// 已使用额度
        /// </summary>
        public decimal TotalSettlement { get; set; }
        /// <summary>
        /// 剩余额度
        /// </summary>
        public decimal SurplusAmount { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
    }
}
