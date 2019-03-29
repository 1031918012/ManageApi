using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 用户管理
    /// </summary>
    public class User : IManage
    {

        /// <summary>
        /// ID
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Uname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        public string UNickname { get; set; }
        /// <summary>
        /// 身份
        /// </summary>
        public string Sub { get; set; }
    }
}
