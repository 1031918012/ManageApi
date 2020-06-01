using Infrastructure;
using System;
using System.ComponentModel;

namespace Domain
{
    /// <summary>
    /// 人员管理
    /// </summary>
    public class Person : IAggregateRoot
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string PersonID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string Department { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public int DepartmentID { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        public string PositionID { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDType { get; set; }
        /// <summary>
        /// 证照号吗
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneCode { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string JobNumber { get; set; }
        /// <summary>
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 子公司id
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 子公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        public DateTime Hiredate { get; set; } = DateTime.Now;
        /// <summary>
        /// 参加工作时间
        /// </summary>
        public DateTime StartJobTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 性别 1男，2女
        /// </summary>
        public int Sex { get; set; }
        /// <summary>
        /// 是否绑定微信
        /// </summary>
        public WechatBindEnum IsBindWechat { get; set; }
        /// <summary>
        /// 是否同步假期
        /// </summary>
        public bool IsSynchroHoliday { get; set; }
    }
}
