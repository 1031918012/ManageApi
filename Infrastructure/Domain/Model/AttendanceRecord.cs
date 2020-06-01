using Infrastructure;
using System;
using System.Collections.Generic;

namespace Domain
{
    /// <summary>
    /// 考勤日表
    /// </summary>
    public class AttendanceRecord : IAggregateRoot
    {
        /// <summary>
        /// 考勤记录ID
        /// </summary>
        public string AttendanceRecordID { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 姓名
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
        /// 公司id
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 所属考勤组ID
        /// </summary>
        public string AttendanceGroupID { get; set; }
        /// <summary>
        /// 所属考勤组名称
        /// </summary>
        public string AttendanceGroupName { get; set; }
        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 班次id
        /// </summary>
        public string ShiftID { get; set; }
        /// <summary>
        /// 班次名称
        /// </summary>
        public string ShiftName { get; set; }
        /// <summary>
        /// 班次信息
        /// </summary>
        public string Shift { get; set; }
        /// <summary>
        /// 出勤状态 0,初始状态,  1正常，2异常 ,
        /// </summary>
        public ResultEnum Status { get; set; }
        /// <summary>
        /// 是否缺卡
        /// </summary>
        public bool IsLackOfCard { get; set; }
        /// <summary>
        /// 请假时长
        /// </summary>
        public string AttendanceItemDTOJson { get; set; }
        /// <summary>
        /// 工时（上班-下班-（休息））
        /// </summary>
        public double WorkingTime { get; set; }
        /// <summary>
        /// 迟到次数
        /// </summary>
        public double LateTimes { get; set; }
        /// <summary>
        /// 迟到时间
        /// </summary>
        public double LateMinutes { get; set; }
        /// <summary>
        /// 早退次数
        /// </summary>
        public double EarlyLeaveTimes { get; set; }
        /// <summary>
        /// 早退时间
        /// </summary>
        public double EarlyLeaveMinutes { get; set; }
        /// <summary>
        /// 上班缺卡次数
        /// </summary>
        public double NotClockInTimes { get; set; }
        /// <summary>
        /// 下班缺卡次数
        /// </summary>
        public double NotClockOutTimes { get; set; }
        /// <summary>
        /// 出差时长（天）
        /// </summary>
        public double BusinessTripDuration { get; set; }
        /// <summary>
        /// 外出时长（小时）
        /// </summary>
        public double OutsideDuration { get; set; }
        /// <summary>
        /// 请假时长（小时）
        /// </summary>
        public double LeaveDuration { get; set; }
    }
}
