using Infrastructure;
using System;

namespace Domain
{
    /// <summary>
    /// 打卡记录（原始记录）
    /// </summary>
    public class ClockRecord : IAggregateRoot
    {
        /// <summary>
        /// 打卡记录ID
        /// </summary>
        public int ClockRecordID { get; set; }
        /// <summary>
        /// 打卡人身份证号码
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 打卡人姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string EmployeeNo { get; set; }
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
        /// 公司id
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 子公司id
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime AttendanceDate { get; set; }
        /// <summary>
        /// 打卡时间
        /// </summary>
        public DateTime ClockTime { get; set; }
        /// <summary>
        /// 上班卡还是下班卡
        /// </summary>
        public ClockTypeEnum ClockType { get; set; }
        /// <summary>
        /// 打卡结果
        /// </summary>
        public ClockResultEnum ClockResult { get; set; }
        /// <summary>
        /// 打卡方式
        /// </summary>
        public ClockWayEnum ClockWay { get; set; }
        /// <summary>
        /// 是否在考勤范围
        /// </summary>
        public bool IsInRange { get; set; }
        /// <summary>
        /// 打卡地点
        /// </summary>
        public string Location { get; set; }
        /// <summary>
        /// 打卡备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 打卡异常原因
        /// </summary>
        public string AbnormalReason { get; set; }
        /// <summary>
        /// 打卡图片1
        /// </summary>
        public string ClockImage1 { get; set; }
        /// <summary>
        /// 打卡图片2
        /// </summary>
        public string ClockImage2 { get; set; }
        /// <summary>
        /// 打卡设备
        /// </summary>
        public string ClockDevice { get; set; }
        /// <summary>
        /// 班次类型范围id
        /// </summary>
        public string ShiftTimeID { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 是否为跨天打卡
        /// </summary>
        public bool IsAcross { get; set; }
        /// <summary>
        /// 是否异常环境打卡
        /// </summary>
        public bool IsAnomaly { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public FieldAuditEnum IsFieldAudit { get; set; }
    }
}
