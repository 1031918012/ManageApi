using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    /// <summary>
    /// 意见反馈
    /// </summary>
    public class Feedback : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int FeedbackID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 反馈建议类型
        /// </summary>
        public FeedbackEnum FeedbackType { get; set; }
        /// <summary>
        /// 反馈建议内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string Path { get; set; }
    }
}
