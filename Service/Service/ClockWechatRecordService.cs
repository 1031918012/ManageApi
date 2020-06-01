using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace Service
{
    public class ClockWechatRecordService : RecordBaseService, IClockWechatRecordService
    {
        private readonly IAttendanceItemCatagoryRepository _itemCatagoryRepository;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;


        public ClockWechatRecordService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IAttendanceItemCatagoryRepository itemCatagoryRepository, IEnterpriseSetRepository enterpriseSetRepository, IPersonRepository personRepository, IGroupPersonnelRepository groupPersonnelRepository, IAttendanceGroupRepository attendanceGroupRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, itemCatagoryRepository)
        {
            _itemCatagoryRepository = itemCatagoryRepository;
            _enterpriseSetRepository = enterpriseSetRepository;
            _personRepository = personRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _attendanceGroupRepository = attendanceGroupRepository;
        }

        /// <summary>
        /// 打卡前的校验
        /// 判断当前是打上班卡还是下班卡
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum) ValidateBeforeClock(string IDCard)
        {
            var person = _personRepository.EntityQueryable<Person>(s => s.IDCard == IDCard, true).FirstOrDefault();
            if (person == null)
            {
                return (false, WechatCodeEnum.NoThisEmployee);
            }
            //检查员工是否已加入考勤组
            var joinedGroup = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.IDCard == IDCard, true).FirstOrDefault();
            if (joinedGroup == null)
            {
                return (false, WechatCodeEnum.NoAttendanceGroup);
            }
            //检查员工是否需要打卡
            var group = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.AttendanceGroupID == joinedGroup.AttendanceGroupID, true).FirstOrDefault();
            if (group.ShiftType == ShiftTypeEnum.DoNotClockIn)
            {
                return (false, WechatCodeEnum.NoNeedClock);
            }
            return (true, WechatCodeEnum.OtherClock);
        }

        ///// <summary>
        ///// 获取微信端页面需要的信息
        ///// 如果微信端页面展示信息有误，就找这里的问题
        ///// </summary>
        ///// <param name="IDCard"></param>
        ///// <returns></returns>
        //public WechatPageInfoDTO GetInfo(string IDCard)
        //{
        //    //如果有班次时间，考勤日期减一天
        //    (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(IDCard, DateTime.Now);
        //    List<ShiftTimeManagement> shiftTime = resRecord.Item2;
        //    if (!shiftTime.Any())
        //    {
        //        return null;
        //    }
        //    AttendanceRecord record1 = resRecord.Item3;
        //    DayTableCalculation calculation = resRecord.Item4;
        //    var clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.IDCard == record1.IDCard && s.AttendanceDate == record1.AttendanceDate, true).OrderBy(s => s.ClockTime).ToList();
        //    ClockSlotDTO startClockSlot = new ClockSlotDTO
        //    {
        //        ClockSlot = shiftTime.FirstOrDefault().UpStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().UpEndClockTime.ToString("HH:mm"),
        //        ClockRecordList = (from record in clockRecord
        //                           where record.ClockType == ClockTypeEnum.Working
        //                           select new ClockRecordLiteDTO()
        //                           {
        //                               ClockTime = record.ClockTime.ToString("HH:mm"),
        //                               ClockResult = record.ClockResult.GetDescription(),
        //                               Location = record.Location,
        //                               IsInRange = record.IsInRange,
        //                               IsFieldAudit = record.IsFieldAudit,
        //                               ClockWay = (int)record.ClockWay
        //                           }).ToList()
        //    };
        //    ClockSlotDTO endClockSlot = new ClockSlotDTO
        //    {
        //        ClockSlot = shiftTime.FirstOrDefault().DownStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().DownEndClockTime.ToString("HH:mm"),
        //        ClockRecordList = (from record in clockRecord
        //                           where record.ClockType == ClockTypeEnum.GetOffWork
        //                           select new ClockRecordLiteDTO()
        //                           {
        //                               ClockTime = record.ClockTime.ToString("HH:mm"),
        //                               ClockResult = record.ClockResult.GetDescription(),
        //                               Location = record.Location,
        //                               IsInRange = record.IsInRange,
        //                               IsFieldAudit = record.IsFieldAudit,
        //                               ClockWay = (int)record.ClockWay
        //                           }).ToList()
        //    };
        //    WechatPageInfoDTO wechatPageInfo = new WechatPageInfoDTO
        //    {
        //        PersonName = record1.Name,
        //        JoinedGroupName = record1.AttendanceGroupName,
        //        DateOfNow = DateTime.Now.Date.ToString("yyyy-MM-dd"),
        //        IsWorkingDay = GetIsWorkingDay(calculation),
        //        ShiftID = record1.ShiftID,
        //        ShiftName = record1.ShiftName,
        //        ShiftTime = shiftTime.FirstOrDefault().StartWorkTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().EndWorkTime.ToString("HH:mm"),
        //        StartClockSlot = startClockSlot,
        //        EndClockSlot = endClockSlot
        //    };
        //    return wechatPageInfo;
        //}
    }
}
