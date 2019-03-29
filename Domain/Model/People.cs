using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 人员管理模型
    /// </summary>
    public class People : IManage
    {
        /// <summary>
        /// 主键，人员id
        /// </summary>
        public Guid PeopleID { get; set; }
        /// <summary>
        /// 创建者
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankCard { get; set; }



    }
}
