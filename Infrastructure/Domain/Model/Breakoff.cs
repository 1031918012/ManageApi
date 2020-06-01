using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 请假表
    /// </summary>
    public class Breakoff : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int BreakoffID { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
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
       /// <summary>
       /// 判断剩余额度是否大于零
       /// </summary>
       /// <returns></returns>
        public bool IsSurplusAmountZero()
        {
            return SurplusAmount < 0;
        }
        /// <summary>
        /// 判断剩余额度是否达到最大
        /// </summary>
        /// <returns></returns>
        public bool IsSurplusAmountMax()
        {
            return SurplusAmount > 180;
        }
    }
}
