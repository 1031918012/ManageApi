using Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Service
{
    public interface IClockRecordService : IService
    {
        PageResult<ClockRecordDTO> GetClockRecordList(string startTime, string endTime, int orgType, int departmentID, string name, int clockStatus,
         int rangeStatus, int pageIndex, int pageSize, FyuUser user);
        WechatPageInfoDTO GetInfo(string IDCard);
        WechatPageInfoDTO GetInfo(string IDCard, DateTime selectTime);
        (bool, WechatCodeEnum) ValidateBeforeClock(string IDCard);
        (bool, WechatCodeEnum) PostLocation(LocationDTO locationDTO);
        (bool, WechatCodeEnum, string) AddInsideClock(ClockRecordParamDTO clockRecordParamDTO);
        (bool, WechatCodeEnum, string) AddOutsideClock(ClockRecordParamDTO clockRecordParamDTO);
        (bool, WechatCodeEnum, string) AddInsideClockTest(List<string> IDCards, string ShiftID, DateTime ClockTime);
        (bool, string, List<ImportClockRecord>) ImportRepairExcel(Stream stream, FyuUser user);
        PageResult<ClockRecordDTO> GetPageRepair(int pageIndex, int pageSize, FyuUser user);
        (bool, string, List<ImportLeaveRecord>) ImportLeaveExcel(int type, Stream stream, FyuUser user);
        LeaveDTO GetLeaveDTO(string IDCard, DateTime leaveTimes);
        List<WeChatShiftDTO> GetWeChatShiftDTO(string iDCard, DateTime time);
        (bool, string) AddSupplementCard(string IDCard, DateTime time, string address, FyuUser user);
        List<ClockFieldAuditDTO> GetClockFieldAuditList(FyuUser user);
        (bool,string) UpdateClockFieldAuditList(bool isFieldAudit, List<int> list, FyuUser user);
    }
}
