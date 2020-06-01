using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 版本更新
    /// </summary>
    public class VersionUpdate : IAggregateRoot
    {
        /// <summary>
        /// 版本更新主键
        /// </summary>
        public int VersionUpdateID { get; set; }
        /// <summary>
        /// 版本更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber { get; set; }
        /// <summary>
        /// 版本更新标题
        /// </summary>
        public string VersionTitle { get; set; }
        /// <summary>
        /// 版本更新内容
        /// </summary>
        public string VersionContent { get; set; }
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
    }
}
