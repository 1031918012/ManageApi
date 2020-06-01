using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IAttendanceRecordSelectService : IService
    {
        /// <summary>
        /// 微信签到统计展示
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        List<RecordDTO> GetRecordInfo(string idcard, DateTime time);
        /// <summary>
        /// 日表HR端分页
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        PageResult<AttendanceRecordDTO> GetAttendanceRecordList(string startTime, string endTime, int orgType, int departmentID, string name, int status, string groupID, FyuUser user, int pageIndex = 1, int pageSize = 10);
        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="name"></param>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        PageResult<ImportRecordDTO> GetAttendanceRecordAll(string startTime, string endTime, int status, string name, List<string> filter, int pageIndex, int pageSize, FyuUser user);
        /// <summary>
        /// 导出签到统计表格
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <param name="filter"></param>
        /// <param name="name"></param>
        byte[] GetDayRecord(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user);
        /// <summary>
        /// 获取有效打卡列表
        /// </summary>
        /// <param name="attendanceRecordID"></param>
        /// <returns></returns>
        List<ClockInfoDTO> GetClockInfoList(string attendanceRecordID);
        /// <summary>
        /// 获取需要发微信错误消息的记录
        /// </summary>
        /// <param name="time"></param>
        void GetErrorRecordWechatMessage(DateTime time);
        /// <summary>
        /// 审核是否通过外勤审核
        /// </summary>
        /// <param name="clockRecordID"></param>
        /// <returns></returns>
        bool Itinerancy(int clockRecordID);
        (string, string) GetDayRecordTest(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user);
        byte[] GetDayRecord3(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user);
    }
}
