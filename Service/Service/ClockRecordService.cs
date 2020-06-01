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
    public class ClockRecordService : RecordBaseService, IClockRecordService
    {
        private readonly IRedisProvider _redisProvider;
        private readonly IPersonRepository _personRepository;
        private readonly IClockRecordRepository _clockRecordRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        private readonly IWeekDaysSettingRepository _weekDaysSettingRepository;
        private readonly IShiftManagementRepository _shiftManagementRepository;
        private readonly IWorkPaidLeaveRepository _workPaidLeaveRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IAttendanceItemCatagoryRepository _itemCatagoryRepository;
        private readonly ILogger<ClockRecordService> _logger;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IDayTableCalculationRepository _dayTableCalculationRepository;

        public ClockRecordService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IRedisProvider redisProvider, IClockRecordRepository clockRecordRepository, IPersonRepository personRepository, IGroupPersonnelRepository groupPersonnelRepository, IAttendanceGroupRepository attendanceGroupRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IShiftManagementRepository shiftManagementRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IHolidayRepository holidayRepository, IAttendanceRecordRepository attendanceRecordRepository, IAttendanceItemCatagoryRepository itemCatagoryRepository, ILogger<ClockRecordService> logger, ILeaveRepository leaveRepository, IEnterpriseSetRepository enterpriseSetRepository, IDayTableCalculationRepository dayTableCalculationRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, itemCatagoryRepository)
        {
            _redisProvider = redisProvider;
            _personRepository = personRepository;
            _clockRecordRepository = clockRecordRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _attendanceGroupRepository = attendanceGroupRepository;
            _weekDaysSettingRepository = weekDaysSettingRepository;
            _shiftManagementRepository = shiftManagementRepository;
            _workPaidLeaveRepository = workPaidLeaveRepository;
            _holidayRepository = holidayRepository;
            _attendanceRecordRepository = attendanceRecordRepository;
            _itemCatagoryRepository = itemCatagoryRepository;
            _leaveRepository = leaveRepository;
            _logger = logger;
            _enterpriseSetRepository = enterpriseSetRepository;
            _dayTableCalculationRepository = dayTableCalculationRepository;
        }

        /// <summary>
        /// 查询原始记录表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="filter"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<ClockRecordDTO> GetClockRecordList(string startTime, string endTime, int orgType, int departmentID, string name, int clockStatus,
     int rangeStatus, int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<ClockRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.Name.Contains(name) || s.EmployeeNo.Contains(name));
            }
            if (clockStatus != 0)
            {
                expression = expression.And(s => s.ClockType == (ClockTypeEnum)clockStatus);
            }
            if (rangeStatus != 2)
            {
                expression = expression.And(s => s.IsInRange == (rangeStatus == 1));
            }
            if (orgType == 1)
            {
                expression = expression.And(s => s.CustomerID == departmentID);
            }
            if (orgType > 1)
            {
                expression = expression.And(s => s.DepartmentID == departmentID);
            }
            var list = _clockRecordRepository.GetByPage(pageIndex, pageSize, expression, s => s.ClockTime, SortOrderEnum.Descending);
            return PageMap<ClockRecordDTO, ClockRecord>(list);
        }

        /// <summary>
        /// 获取微信端页面需要的信息
        /// 如果微信端页面展示信息有误，就找这里的问题
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public WechatPageInfoDTO GetInfo(string IDCard)
        {
            //如果有班次时间，考勤日期减一天
            (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(IDCard, DateTime.Now);
            List<ShiftTimeManagement> shiftTime = resRecord.Item2;
            if (!shiftTime.Any())
            {
                return null;
            }
            AttendanceRecord record1 = resRecord.Item3;
            DayTableCalculation calculation = resRecord.Item4;
            var clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.IDCard == record1.IDCard && s.AttendanceDate == record1.AttendanceDate, true).OrderBy(s => s.ClockTime).ToList();
            ClockSlotDTO startClockSlot = new ClockSlotDTO
            {
                ClockSlot = shiftTime.FirstOrDefault().UpStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().UpEndClockTime.ToString("HH:mm"),
                ClockRecordList = (from record in clockRecord
                                   where record.ClockType == ClockTypeEnum.Working
                                   select new ClockRecordLiteDTO()
                                   {
                                       ClockTime = record.ClockTime.ToString("HH:mm"),
                                       ClockResult = record.ClockResult.GetDescription(),
                                       Location = record.Location,
                                       IsInRange = record.IsInRange,
                                       IsFieldAudit = record.IsFieldAudit == FieldAuditEnum.Checked,
                                       ClockWay = (int)record.ClockWay
                                   }).ToList()
            };
            ClockSlotDTO endClockSlot = new ClockSlotDTO
            {
                ClockSlot = shiftTime.FirstOrDefault().DownStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().DownEndClockTime.ToString("HH:mm"),
                ClockRecordList = (from record in clockRecord
                                   where record.ClockType == ClockTypeEnum.GetOffWork
                                   select new ClockRecordLiteDTO()
                                   {
                                       ClockTime = record.ClockTime.ToString("HH:mm"),
                                       ClockResult = record.ClockResult.GetDescription(),
                                       Location = record.Location,
                                       IsInRange = record.IsInRange,
                                       IsFieldAudit = record.IsFieldAudit == FieldAuditEnum.Checked,
                                       ClockWay = (int)record.ClockWay
                                   }).ToList()
            };
            WechatPageInfoDTO wechatPageInfo = new WechatPageInfoDTO
            {
                PersonName = record1.Name,
                JoinedGroupName = record1.AttendanceGroupName,
                DateOfNow = DateTime.Now.Date.ToString("yyyy-MM-dd"),
                IsWorkingDay = GetIsWorkingDay(calculation),
                ShiftID = record1.ShiftID,
                ShiftName = record1.ShiftName,
                ShiftTime = shiftTime.FirstOrDefault().StartWorkTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().EndWorkTime.ToString("HH:mm"),
                StartClockSlot = startClockSlot,
                EndClockSlot = endClockSlot
            };
            return wechatPageInfo;
        }
        public WechatPageInfoDTO GetInfo(string IDCard, DateTime selectTime)
        {
            var record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == selectTime.Date).FirstOrDefault();
            if (record1 == null)
            {
                return null;
            }
            var calculation = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID).FirstOrDefault();
            List<ShiftTimeManagement> shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList).ToList();

            var clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == selectTime.Date && s.IDCard == IDCard, true).OrderBy(s => s.ClockTime).ToList();
            //计算工时
            TimeSpan workingHours = new TimeSpan();
            if (clockRecord.Any())
            {
                var startWorkTime = clockRecord.Where(s => s.ClockType == ClockTypeEnum.Working).FirstOrDefault();
                var endWorkTime = clockRecord.Where(s => s.ClockType == ClockTypeEnum.GetOffWork).LastOrDefault();
                if (startWorkTime != null && endWorkTime != null)
                {
                    workingHours = endWorkTime.ClockTime.ConvertTime5().Subtract(startWorkTime.ClockTime.ConvertTime5());
                    foreach (var item in shiftTime)
                    {
                        if (item.EndRestTime != null && item.StartRestTime != null)
                        {
                            workingHours = workingHours.Subtract(((DateTime)item.EndRestTime).ConvertTime5() - ((DateTime)item.StartRestTime).ConvertTime5());
                        }
                    }
                }
            }

            ClockSlotDTO startClockSlot = new ClockSlotDTO
            {
                ClockSlot = shiftTime.FirstOrDefault().UpStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().UpEndClockTime.ToString("HH:mm"),
                ClockRecordList = (from record in clockRecord
                                   where record.ClockType == ClockTypeEnum.Working
                                   select new ClockRecordLiteDTO()
                                   {
                                       ClockTime = record.ClockTime.ToString("HH:mm"),
                                       ClockResult = record.ClockResult.GetDescription(),
                                       Location = record.Location,
                                       IsInRange = record.IsInRange,
                                       IsFieldAudit = record.IsFieldAudit == FieldAuditEnum.Checked,
                                       ClockWay = (int)record.ClockWay
                                   }).ToList()
            };
            ClockSlotDTO endClockSlot = new ClockSlotDTO
            {
                ClockSlot = shiftTime.FirstOrDefault().DownStartClockTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().DownEndClockTime.ToString("HH:mm"),
                ClockRecordList = (from record in clockRecord
                                   where record.ClockType == ClockTypeEnum.GetOffWork
                                   select new ClockRecordLiteDTO()
                                   {
                                       ClockTime = record.ClockTime.ToString("HH:mm"),
                                       ClockResult = record.ClockResult.GetDescription(),
                                       Location = record.Location,
                                       IsInRange = record.IsInRange,
                                       IsFieldAudit = record.IsFieldAudit == FieldAuditEnum.Checked,
                                       ClockWay = (int)record.ClockWay
                                   }).ToList()
            };
            WechatPageInfoDTO wechatPageInfo = new WechatPageInfoDTO
            {
                PersonName = record1.Name,
                JoinedGroupName = record1.AttendanceGroupName,
                DateOfNow = selectTime.Date.ToString("yyyy-MM-dd"),
                IsWorkingDay = GetIsWorkingDay(calculation),
                ShiftID = record1.ShiftID,
                ShiftName = record1.ShiftName,
                ShiftTime = shiftTime.FirstOrDefault().StartWorkTime.ToString("HH:mm") + "-" + shiftTime.FirstOrDefault().EndWorkTime.ToString("HH:mm"),
                WorkingHours = workingHours.ToString("%h") + "小时" + workingHours.ToString("%m") + "分钟",
                StartClockSlot = startClockSlot,
                EndClockSlot = endClockSlot
            };
            return wechatPageInfo;
        }
        private bool GetIsWorkingDay(DayTableCalculation calculation)
        {
            if (calculation.IsDynamicRowHugh)
            {
                if (calculation.IsHoliday)
                {
                    return false;
                }
                if (calculation.IsWorkPaidLeave)
                {
                    return true;
                }
                if (calculation.IsHolidayWork)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (calculation.IsHolidayWork)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) JudgeEffectiveDay(string IDCard, DateTime now)
        {
            bool IsAcross = false;
            AttendanceRecord record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == now.Date, true).FirstOrDefault();
            if (record1 == null)
            {
                return (IsAcross, new List<ShiftTimeManagement>(), null, null);
            }
            DayTableCalculation calculation = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID, true).FirstOrDefault();
            var shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList).OrderBy(s => s.UpStartClockTime).ToList();
            if (shiftTime.FirstOrDefault().UpStartClockTime > now.ConvertTime2())//上班时间只可能是01
            {
                var record2 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == now.AddDays(-1).Date, true).FirstOrDefault();
                if (record2 != null)
                {
                    record1 = record2;
                    calculation = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record2.AttendanceRecordID, true).FirstOrDefault();
                    IsAcross = true;
                    shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList).OrderBy(s => s.UpStartClockTime).ToList();
                }
            }
            return (IsAcross, shiftTime, record1, calculation);
        }

        /// <summary>
        /// 打卡前的校验
        /// 判断当前是打上班卡还是下班卡
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum) ValidateBeforeClock(string IDCard)
        {
            try
            {
                DateTime time = DateTime.Now;
                //如果有班次时间，考勤日期减一天
                (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(IDCard, time);
                List<ShiftTimeManagement> shiftTime = resRecord.Item2;
                if (!shiftTime.Any())
                {
                    return (false, WechatCodeEnum.NoRecord);
                }
                AttendanceRecord record1 = resRecord.Item3;
                DayTableCalculation calculation = resRecord.Item4;
                if (!shiftTime.Any())
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
                    return (false, WechatCodeEnum.NoRecord);
                }
                //检查员工当前是上班打卡还是下班打卡
                var currentTime = DateTime.Now.ConvertTime();
                if (shiftTime.Where(a => currentTime >= a.UpStartClockTime && currentTime <= a.UpEndClockTime).Any())
                {
                    return (true, WechatCodeEnum.StartWorkClock);
                }
                if (shiftTime.Where(a => currentTime >= a.DownStartClockTime && currentTime <= a.DownEndClockTime).Any())
                {
                    return (true, WechatCodeEnum.EndWorkClock);
                }
                //跨天处理后检查员工当前是否要打下班卡
                currentTime = currentTime.AddDays(1);
                if (shiftTime.Where(a => currentTime >= a.DownStartClockTime && currentTime <= a.DownEndClockTime).Any())
                {
                    return (true, WechatCodeEnum.EndWorkClock);
                }
                return (true, WechatCodeEnum.OtherClock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "打卡前的校验出错");
                return (false, WechatCodeEnum.ValidationFailed);
            }
        }

        /// <summary>
        /// 提交打卡地点（处理打卡时的经纬度）
        /// 根据传过来的位置信息判断是打内勤卡还是外勤卡
        /// </summary>
        /// <param name="locationDTO"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum) PostLocation(LocationDTO locationDTO)
        {
            DateTime time = DateTime.Now;
            //如果有班次时间，考勤日期减一天
            (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(locationDTO.IDCard, time);
            List<ShiftTimeManagement> shiftTime = resRecord.Item2;
            if (!shiftTime.Any())
            {
                return (false, WechatCodeEnum.NoRecord);
            }
            AttendanceRecord record1 = resRecord.Item3;
            DayTableCalculation calculation = resRecord.Item4;
            List<ClockInAddress> locations = JsonConvert.DeserializeObject<List<ClockInAddress>>(calculation.SiteAttendance);
            if (!locations.Any())
            {
                return (false, WechatCodeEnum.LocationRetrievalFailed);
            }
            ClockInAddress MatchedLocation = null;
            //根据经纬度查匹配的考勤地点
            foreach (var i in locations)
            {
                var currentDistance = LocationUtil.GetDistance(i.Longitude, i.Latitude, locationDTO.Longitude, locationDTO.Latitude);
                if (currentDistance <= i.Distance)
                {
                    MatchedLocation = i;
                    break;
                }
            }
            if (MatchedLocation == null)
            {
                string GeoInfo = JsonConvert.SerializeObject(new { locationDTO.Latitude, locationDTO.Longitude });
                _redisProvider.Add(locationDTO.IDCard, GeoInfo, TimeSpan.FromSeconds(300));
                //外勤打卡
                return (true, WechatCodeEnum.OutsideClock);
            }
            else
            {
                string GeoInfo = JsonConvert.SerializeObject(new { locationDTO.Latitude, locationDTO.Longitude, MatchedLocation.ClockName });
                _redisProvider.Add(locationDTO.IDCard, GeoInfo, TimeSpan.FromSeconds(30));
                //内勤打卡
                return (true, WechatCodeEnum.InsideClock);
            }
        }

        /// <summary>
        /// 内勤打卡
        /// </summary>
        /// <param name="clockRecordParamDTO"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) AddInsideClock(ClockRecordParamDTO clockRecordParamDTO)
        {
            DateTime time = DateTime.Now;
            //如果有班次时间，考勤日期减一天
            (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(clockRecordParamDTO.IDCard, time);
            List<ShiftTimeManagement> shiftTime = resRecord.Item2;
            if (!shiftTime.Any())
            {
                return (false, WechatCodeEnum.NoRecord, null);
            }
            AttendanceRecord record1 = resRecord.Item3;
            DayTableCalculation calculation = resRecord.Item4;
            JObject location = null;
            try
            {
                location = JObject.Parse(_redisProvider.Get<string>(clockRecordParamDTO.IDCard));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{record1.Name}（{record1.IDCard}）内勤打卡失败，原因：从Redis获取位置信息失败或位置信息已过期！");
                throw;
            }

            var clockRecord = new ClockRecord
            {
                IDCard = record1.IDCard,
                Name = record1.Name,
                EmployeeNo = record1.EmployeeNo,
                Department = record1.Department,
                CustomerID = record1.CustomerID,
                DepartmentID = record1.DepartmentID,
                Position = record1.Position,
                CompanyID = record1.CompanyID,
                CompanyName = record1.CompanyName,
                AttendanceDate = record1.AttendanceDate,
                ClockTime = time,
                ClockWay = ClockWayEnum.GPS,
                IsInRange = location != null ? true : false,
                Location = location != null ? location["ClockName"].ToString() : string.Empty,
                Remark = clockRecordParamDTO.Remark,
                Latitude = location["Latitude"].ToString(),
                Longitude = location["Longitude"].ToString(),
                IsFieldAudit = FieldAuditEnum.Checked,
                IsAcross = resRecord.Item1,
                IsAnomaly = false
            };

            bool isSuccess = false;

            //计算打卡状态
            clockRecord = ComputeClockRecord(calculation, shiftTime, clockRecord);
            if (clockRecord == null)
            {
                return (isSuccess, WechatCodeEnum.NotInClockSlot, null);
            }
            if (string.IsNullOrEmpty(clockRecord.ShiftTimeID) && clockRecord.ClockType == 0 && clockRecord.ClockResult == 0)
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）内勤打卡失败，原因：找不到班次或班次时间！");
                return (isSuccess, WechatCodeEnum.ClockFailed, null);
            }

            _attendanceUnitOfWork.Add(clockRecord);
            isSuccess = _attendanceUnitOfWork.Commit();
            if (isSuccess)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), new List<RealTimeDTO> { new RealTimeDTO { clockType = clockRecord.ClockType == ClockTypeEnum.GetOffWork, IDcard = clockRecord.IDCard, time = clockRecord.AttendanceDate } });
                if (clockRecord.ClockResult == ClockResultEnum.Normal)
                    return (isSuccess, WechatCodeEnum.ClockSuccessNormal, time.ToString("HH:mm"));
                else if (clockRecord.ClockResult == ClockResultEnum.Late)
                    return (isSuccess, WechatCodeEnum.ClockSuccessLate, time.ToString("HH:mm"));
                else if (clockRecord.ClockResult == ClockResultEnum.EarlyLeave)
                    return (isSuccess, WechatCodeEnum.ClockSuccessEarlyLeave, time.ToString("HH:mm"));
                else
                    return (isSuccess, WechatCodeEnum.ClockSuccessUnknown, time.ToString("HH:mm"));//不会返回该结果
            }
            else
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）内勤打卡失败，原因：写数据库失败 " + JsonConvert.SerializeObject(clockRecord));
                return (isSuccess, WechatCodeEnum.ClockFailed, null);
            }
        }

        /// <summary>
        /// 根据班次信息计算打卡状态
        /// </summary>
        /// <param name="shiftID"></param>
        /// <param name="clockRecord"></param>
        /// <returns></returns>
        private ClockRecord ComputeClockRecord(DayTableCalculation DayTable, List<ShiftTimeManagement> ShiftTimeManagementList, ClockRecord clockRecord)
        {
            //无班次信息，不做进一步的处理，直接返回对象
            if (DayTable == null)
            {
                return clockRecord;
            }
            //转化打卡时间为可判断类型
            var clockTime = clockRecord.ClockTime.ConvertTime();
            //找出合适的打卡时段范围
            var shiftTime = ShiftTimeManagementList.FirstOrDefault();
            //如果无班次时间，尝试进行跨天处理
            if (shiftTime == null)
            {
                return null;
            }
            var startWorkTime = shiftTime.StartWorkTime;
            var endWorkTime = shiftTime.EndWorkTime;
            //处理弹性时间
            if (DayTable.IsFlexible)
            {
                //上班有效时间段
                if (shiftTime.UpStartClockTime <= clockTime && clockTime <= shiftTime.UpEndClockTime)//59
                {
                    if (clockTime > startWorkTime)
                    {
                        startWorkTime = startWorkTime.AddMinutes(DayTable.FlexibleMinutes);
                        if (clockTime > startWorkTime)
                        {
                            clockRecord.ClockResult = ClockResultEnum.Late;
                        }
                        else
                        {
                            clockRecord.ClockResult = ClockResultEnum.Normal;
                        }
                    }
                    else
                    {
                        clockRecord.ClockResult = ClockResultEnum.Normal;
                    }
                    clockRecord.ClockType = ClockTypeEnum.Working;
                    clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                    return clockRecord;
                }
                //下班有效时段
                else
                {
                    int flexible = 0;
                    var clockall = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.IDCard == clockRecord.IDCard && s.AttendanceDate == clockRecord.AttendanceDate).OrderBy(s => s.ClockTime).ToList();
                    if (clockall.Any())
                    {
                        var clockmorning = clockall.Where(s => s.ClockType == ClockTypeEnum.Working).FirstOrDefault();
                        if (clockmorning != null)
                        {
                            if (clockmorning.ClockTime.ConvertTime2() > startWorkTime)
                            {
                                var minutes = Math.Abs(clockmorning.ClockTime.ConvertTime2().Subtract(startWorkTime).TotalMinutes);
                                flexible = minutes > DayTable.FlexibleMinutes ? DayTable.FlexibleMinutes : (int)minutes;
                            }
                        }
                    }
                    endWorkTime = endWorkTime.AddMinutes(flexible);//上午的弹性时长
                    clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                    clockRecord = ComputeTime(clockRecord, clockTime, endWorkTime, 2);
                }
                return clockRecord;
            }
            //处理豁免时间
            if (DayTable.IsExemption)
            {
                //上班有效时间段
                if (shiftTime.UpStartClockTime <= clockTime && clockTime <= shiftTime.UpEndClockTime)
                {
                    startWorkTime = startWorkTime.AddMinutes(DayTable.LateMinutes);
                    clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                    clockRecord = ComputeTime(clockRecord, clockTime, startWorkTime, 1);
                }
                //下班有效时段
                else
                {
                    endWorkTime = endWorkTime.AddMinutes(-DayTable.EarlyLeaveMinutes);
                    clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                    clockRecord = ComputeTime(clockRecord, clockTime, endWorkTime, 2);
                }
                return clockRecord;
            }
            //上班有效时间段
            if (shiftTime.UpStartClockTime <= clockTime && clockTime <= shiftTime.UpEndClockTime)
            {
                clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                clockRecord = ComputeTime(clockRecord, clockTime, startWorkTime, 1);
            }
            //下班有效时段
            else
            {
                clockRecord.ShiftTimeID = shiftTime.ShiftTimeID;
                clockRecord = ComputeTime(clockRecord, clockTime, endWorkTime, 2);
            }
            return clockRecord;
        }

        /// <summary>
        /// 判断迟到早退
        /// </summary>
        /// <param name="clockRecordTime"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private ClockRecord ComputeTime(ClockRecord clockRecord, DateTime clockRecordTime, DateTime time, int type)
        {

            if (type == 1)
            {
                //正常
                if (clockRecordTime <= time)
                {
                    clockRecord.ClockType = ClockTypeEnum.Working;
                    clockRecord.ClockResult = ClockResultEnum.Normal;
                }
                //迟到
                if (clockRecordTime > time)
                {
                    clockRecord.ClockType = ClockTypeEnum.Working;
                    clockRecord.ClockResult = ClockResultEnum.Late;
                }

            }
            if (type == 2)
            {
                if (clockRecord.IsAcross)
                {
                    clockRecordTime = clockRecordTime.AddDays(1);
                }
                if (clockRecordTime < time)
                {
                    //早退
                    clockRecord.ClockType = ClockTypeEnum.GetOffWork;
                    clockRecord.ClockResult = ClockResultEnum.EarlyLeave;
                }
                if (clockRecordTime >= time)
                {
                    //正常
                    clockRecord.ClockType = ClockTypeEnum.GetOffWork;
                    clockRecord.ClockResult = ClockResultEnum.Normal;
                }
            }
            return clockRecord;
        }

        /// <summary>
        /// 外勤打卡
        /// </summary>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) AddOutsideClock(ClockRecordParamDTO clockRecordParamDTO)
        {
            DateTime time = DateTime.Now;
            //如果有班次时间，考勤日期减一天
            (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(clockRecordParamDTO.IDCard, time);
            List<ShiftTimeManagement> shiftTime = resRecord.Item2;
            if (!shiftTime.Any())
            {
                return (false, WechatCodeEnum.NoRecord, null);
            }
            AttendanceRecord record1 = resRecord.Item3;
            DayTableCalculation calculation = resRecord.Item4;

            JObject location = null;
            string loc = _redisProvider.Get<string>(clockRecordParamDTO.IDCard);
            var jge = string.IsNullOrEmpty(loc);
            if (!jge)
            {
                location = JObject.Parse(loc);
            }
            var clockRecord = new ClockRecord
            {
                IDCard = record1.IDCard,
                Name = record1.Name,
                EmployeeNo = record1.EmployeeNo,
                Department = record1.Department,
                Position = record1.Position,
                CompanyID = record1.CompanyID,
                DepartmentID = record1.DepartmentID,
                CustomerID = record1.CustomerID,
                CompanyName = record1.CompanyName,
                AttendanceDate = record1.AttendanceDate,
                ClockTime = time,
                ClockWay = ClockWayEnum.GPS,
                IsInRange = false,
                Location = clockRecordParamDTO.Address,
                Remark = clockRecordParamDTO.Remark,
                Longitude = jge ? "" : location["Longitude"].ToString(),
                Latitude = jge ? "" : location["Latitude"].ToString(),
                IsFieldAudit = FieldAuditEnum.PendingReview,
                IsAcross = resRecord.Item1,
                IsAnomaly = false
            };

            bool isSuccess = false;

            //计算打卡状态
            clockRecord = ComputeClockRecord(calculation, shiftTime, clockRecord);
            if (clockRecord == null)
            {
                return (isSuccess, WechatCodeEnum.NotInClockSlot, null);
            }
            if (string.IsNullOrEmpty(clockRecord.ShiftTimeID) && clockRecord.ClockType == 0 && clockRecord.ClockResult == 0)
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）外勤打卡失败，原因：找不到班次或班次时间！");
                return (isSuccess, WechatCodeEnum.ClockFailed, null);
            }
            //处理图片
            if (!string.IsNullOrEmpty(clockRecordParamDTO.ImageBase64))
            {
                var image = ProcessImage(clockRecordParamDTO.ImageBase64, record1.Name, clockRecordParamDTO.Address);
                if (image.Item1 == false)
                {
                    _logger.LogError($"{record1.Name}（{record1.IDCard}）外勤打卡失败，原因：处理打卡图片失败");
                    return (isSuccess, WechatCodeEnum.ClockFailed, null);
                }
                clockRecord.ClockImage1 = image.Item2;
            }

            _attendanceUnitOfWork.Add(clockRecord);
            isSuccess = _attendanceUnitOfWork.Commit();
            if (isSuccess)
            {
                if (clockRecord.ClockResult == ClockResultEnum.Normal)
                    return (isSuccess, WechatCodeEnum.ClockSuccessNormal, time.ToString("HH:mm"));
                else if (clockRecord.ClockResult == ClockResultEnum.Late)
                    return (isSuccess, WechatCodeEnum.ClockSuccessLate, time.ToString("HH:mm"));
                else if (clockRecord.ClockResult == ClockResultEnum.EarlyLeave)
                    return (isSuccess, WechatCodeEnum.ClockSuccessEarlyLeave, time.ToString("HH:mm"));
                else
                    return (isSuccess, WechatCodeEnum.ClockSuccessUnknown, time.ToString("HH:mm"));//不会返回该结果
            }
            else
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）外勤打卡失败，原因：写数据库失败");
                return (isSuccess, WechatCodeEnum.ClockFailed, null);
            }
        }

        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        private (bool, string) ProcessImage(string imageBase64, string name, string addresse)
        {
            bool flag = false;
            try
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string filename = Guid.NewGuid().ToString();
                string path = $"{Directory.GetCurrentDirectory()}/wwwroot/images/{date}/{filename}.png";
                string filePath = Path.GetDirectoryName(path);
                if (!Directory.Exists(filePath))
                {
                    if (filePath != null)
                    {
                        Directory.CreateDirectory(filePath);
                    }
                }
                var match = Regex.Match(imageBase64, "data:image/jpeg;base64,([\\w\\W]*)$");
                if (!match.Success)
                {
                    return (flag, null);
                }
                imageBase64 = match.Groups[1].Value;
                byte[] imageBytes = Convert.FromBase64String(imageBase64);
                addresse = string.IsNullOrWhiteSpace(addresse.Trim()) ? "暂无地址" : addresse;
                Infrastructure.ImageHelper.AddWaterMark(imageBytes, name, addresse, DateTime.Now, path);
                //File.WriteAllBytes(path, imageBytes);
                var link = AppConfig.WeChat.EnvironmentHost + $"/images/{date}/{filename}.png";
                flag = true;
                return (flag, link);
            }
            catch (Exception)
            {
                return (flag, null);
            }
        }

        /// <summary>
        /// 内勤打卡（测试用）
        /// </summary>
        /// <param name="clockRecordParamDTO"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) AddInsideClockTest(List<string> IDCards, string ShiftID, DateTime ClockTime)
        {
            List<ClockRecord> clockRecords = new List<ClockRecord>();
            List<RealTimeDTO> realTimes = new List<RealTimeDTO>();
            foreach (var id in IDCards)
            {
                //如果有班次时间，考勤日期减一天
                (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(id, ClockTime);
                List<ShiftTimeManagement> shiftTime = resRecord.Item2;
                if (!shiftTime.Any())
                {
                    return (false, WechatCodeEnum.NoRecord, null);
                }
                AttendanceRecord record1 = resRecord.Item3;
                DayTableCalculation calculation = resRecord.Item4;
                var clockRecord = new ClockRecord
                {
                    IDCard = record1.IDCard,
                    Name = record1.Name,
                    EmployeeNo = record1.EmployeeNo,
                    Department = record1.Department,
                    CustomerID = record1.CustomerID,
                    DepartmentID = record1.DepartmentID,
                    Position = record1.Position,
                    CompanyID = record1.CompanyID,
                    CompanyName = record1.CompanyName,
                    AttendanceDate = record1.AttendanceDate,
                    ClockTime = ClockTime,
                    ClockWay = ClockWayEnum.GPS,
                    IsInRange = true,
                    Location = "在哪打的不重要",
                    IsFieldAudit = FieldAuditEnum.Checked,
                    IsAcross = resRecord.Item1,
                    IsAnomaly = false
                };
                //计算打卡状态
                clockRecord = ComputeClockRecord(calculation, shiftTime, clockRecord);
                if (clockRecord == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(clockRecord.ShiftTimeID) && clockRecord.ClockType == 0 && clockRecord.ClockResult == 0)
                {
                    continue;
                }
                _attendanceUnitOfWork.Add(clockRecord);
                realTimes.Add(new RealTimeDTO { clockType = clockRecord.ClockType == ClockTypeEnum.GetOffWork, IDcard = clockRecord.IDCard, time = clockRecord.AttendanceDate });
            }
            bool isSuccess = _attendanceUnitOfWork.Commit();
            if (isSuccess)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), realTimes);
            }
            return (true, WechatCodeEnum.ClockSuccessNormal, null);
        }

        /// <summary>
        /// 导入补卡记录
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public (bool, string, List<ImportClockRecord>) ImportRepairExcel(Stream stream, FyuUser user)
        {
            (List<ImportClockRecord>, string, List<ImportClockRecord>) data = NPOIHelp.ResolveRepairData(stream);
            if (!data.Item1.Any())
            {
                return (false, data.Item2, data.Item3);
            }
            int number = data.Item1.Count + data.Item3.Count;
            List<ClockRecord> clocks = new List<ClockRecord>();
            List<RealTimeDTO> realTimes = new List<RealTimeDTO>();
            var clockgroup = data.Item1; ;
            var recordAll = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => clockgroup.Select(t => t.Clocktime.Date).Contains(s.AttendanceDate) && clockgroup.Select(t => t.IdCard).Contains(s.IDCard) && s.CompanyID == user.customerId, true).ToList();
            var calculationAll = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => recordAll.Select(t => t.AttendanceRecordID).Contains(s.AttendanceRecordID), true).ToList();
            foreach (var clock in clockgroup.GroupBy(s => s.IdCard))
            {
                var record = recordAll.Where(s => s.IDCard == clock.FirstOrDefault().IdCard).FirstOrDefault();
                if (record == null)
                {
                    data.Item3.AddRange(clock);
                    continue;
                }
                var calculation = calculationAll.Where(s => s.AttendanceRecordID == record.AttendanceRecordID).FirstOrDefault();
                var shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList);
                clock.ToList().ForEach(s =>
                {
                    var clockRecord = new ClockRecord
                    {
                        IDCard = record.IDCard,
                        Name = record.Name,
                        EmployeeNo = record.EmployeeNo,
                        Department = record.Department,
                        Position = record.Position,
                        CompanyID = record.CompanyID,
                        CompanyName = record.CompanyName,
                        AttendanceDate = s.Clocktime.Date,
                        ClockTime = s.Clocktime,
                        ClockWay = ClockWayEnum.Repair,
                        IsInRange = true,
                        Location = s.Site,
                        Remark = "",
                        Latitude = "",
                        Longitude = "",
                        IsFieldAudit = FieldAuditEnum.Checked,
                        IsAnomaly = false,
                        DepartmentID = record.DepartmentID,
                        CustomerID = record.CustomerID,
                    };
                    clockRecord = ComputeClockRecord(calculation, shiftTime, clockRecord);
                    clocks.Add(clockRecord);
                    realTimes.Add(new RealTimeDTO { clockType = true, IDcard = clockRecord.IDCard, time = clockRecord.AttendanceDate });
                });
            }
            if (clocks.Any())
            {
                _attendanceUnitOfWork.BatchInsert(clocks);
            }
            var success = _attendanceUnitOfWork.Commit();
            if (success)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), realTimes);
                return (true, "本次成功导入" + (number - data.Item3.Count).ToString() + "条数据，导入失败" + data.Item3.Count + "条数据。", data.Item3);
            }
            return (false, "导入失败", null);
        }

        public PageResult<ClockRecordDTO> GetPageRepair(int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<ClockRecord, bool>> expression = s => s.CompanyID == user.customerId && s.ClockWay == ClockWayEnum.Repair;
            var list = _clockRecordRepository.GetByPage(pageIndex, pageSize, expression, s => s.ClockTime, SortOrderEnum.Descending);
            return PageMap<ClockRecordDTO, ClockRecord>(list);
        }

        public (bool, string, List<ImportLeaveRecord>) ImportLeaveExcel(int type, Stream stream, FyuUser user)
        {
            (List<ImportLeaveRecord>, string, List<ImportLeaveRecord>) data = NPOIHelp.ResolveLeaveData(type, stream);
            if (!data.Item1.Any())
            {
                return (false, data.Item2, data.Item3);
            }
            int number = data.Item1.Count + data.Item3.Count;
            List<Leave> Leaves = new List<Leave>();
            List<RealTimeDTO> redis = new List<RealTimeDTO>();
            foreach (var item in data.Item1)
            {
                var itemCatagory = _itemCatagoryRepository.EntityQueryable<AttendanceItem>(s => s.AttendanceItemCatagoryName == "请假考勤项" || s.AttendanceItemCatagoryName == "外出考勤项").Select(s => s.AttendanceItemName);
                if (!itemCatagory.Contains(item.LeaveType) || item.StartTime > item.EndTime)
                {
                    data.Item3.Add(item);
                    continue;
                }
                var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == item.IdCard).FirstOrDefault();
                if (record == null)
                {
                    data.Item3.Add(item);
                    continue;
                }
                Leave leave = new Leave
                {
                    IDCard = item.IdCard,
                    CompanyID = record.CompanyID,
                    CompanyName = record.CompanyName,
                    Department = record.Department,
                    EndTime = item.EndTime,
                    StartTime = item.StartTime,
                    JobNumber = record.EmployeeNo,
                    LeaveID = Guid.NewGuid().ToString(),
                    LeaveName = item.LeaveType,
                    Name = record.Name,
                };
                Leaves.Add(leave);
                for (DateTime i = item.StartTime.Date; i <= item.EndTime.Date; i = i.AddDays(1))
                {
                    RealTimeDTO realTimeDTO = new RealTimeDTO
                    {
                        clockType = true,
                        IDcard = item.IdCard,
                        time = i
                    };
                    redis.Add(realTimeDTO);
                }
            }
            if (Leaves.Any())
            {
                _attendanceUnitOfWork.BatchInsert(Leaves);
            }
            var success = _attendanceUnitOfWork.Commit();
            if (success)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), redis);
                return (true, "本次成功导入" + (number - data.Item3.Count).ToString() + "条数据，导入失败" + data.Item3.Count + "条数据。", data.Item3);
            }
            return (false, "导入失败", null);
        }

        public LeaveDTO GetLeaveDTO(string IDCard, DateTime leaveTimes)
        {
            var Leave = _leaveRepository.EntityQueryable<Leave>(s => s.StartTime.Date <= leaveTimes.Date && s.EndTime.Date >= leaveTimes.Date && s.IDCard == IDCard);
            if (Leave.Any())
            {
                return _mapper.Map<LeaveDTO>(Leave);
            }
            return null;
        }

        public List<WeChatShiftDTO> GetWeChatShiftDTO(string iDCard, DateTime time)
        {
            var groupperson = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.IDCard == iDCard, true).FirstOrDefault();
            if (groupperson == null)
            {
                return null;
            }
            var weekDays = _weekDaysSettingRepository.EntityQueryable<WeekDaysSetting>(s => s.AttendanceGroupID == groupperson.AttendanceGroupID, true).ToList();
            var shifts = _shiftManagementRepository.EntityQueryable<ShiftManagement>(s => weekDays.Select(t => t.ShiftID).Contains(s.ShiftID)).ToList();
            if (!shifts.Any())
            {
                return null;
            }
            var holidays = _holidayRepository.EntityQueryable<Holiday>(s => true, true).ToList();
            var workPay = _workPaidLeaveRepository.EntityQueryable<WorkPaidLeave>(s => true, true).ToList();
            var timeall = time.AddMonths(1);
            List<WeChatShiftDTO> res = new List<WeChatShiftDTO>();
            for (DateTime i = time.Date; i < timeall; i = i.AddDays(1).Date)
            {
                WeekDaysSetting week = weekDays.Where(s => s.Week == (int)i.DayOfWeek).FirstOrDefault();
                var weekany = week != null;
                ShiftManagement shiftOne = shifts.Where(s => s.ShiftID == week.ShiftID && !s.IsDelete).FirstOrDefault();
                var shiftany = shiftOne != null;
                var holiday = holidays.Where(s => s.StartHolidayTime <= i && s.EndHolidayTime >= i && !s.IsDelete).Any();
                var workPayany = workPay.Where(s => s.PaidLeaveTime.Date == i).Any();
                WeChatShiftDTO weChatShiftDTO = new WeChatShiftDTO
                {
                    Time = i.ToString("yyyy-MM-dd"),
                    IsShift = workPayany,
                    IsWork = weekany ? week.IsHolidayWork : false,
                    Shift = shiftany ? _mapper.Map<ShiftManagementDTO>(shiftOne) : null,
                    IsHolidy = holiday
                };
                res.Add(weChatShiftDTO);
            }
            return res;
        }

        public (bool, string) AddSupplementCard(string IDCard, DateTime time, string address, FyuUser user)
        {
            if (string.IsNullOrEmpty(IDCard))
            {
                return (false, "请选择人员");
            }
            if (string.IsNullOrEmpty(address))
            {
                return (false, "请选择考勤地址");
            }
            (bool, List<ShiftTimeManagement>, AttendanceRecord, DayTableCalculation) resRecord = JudgeEffectiveDay(IDCard, time);
            List<ShiftTimeManagement> shiftTime = resRecord.Item2;
            if (!shiftTime.Any())
            {
                return (false, "该日未发现排班");
            }
            AttendanceRecord record = resRecord.Item3;
            DayTableCalculation calculation = resRecord.Item4;
            var clockRecord = new ClockRecord
            {
                IDCard = record.IDCard,
                Name = record.Name,
                EmployeeNo = record.EmployeeNo,
                Department = record.Department,
                Position = record.Position,
                CompanyID = record.CompanyID,
                CompanyName = record.CompanyName,
                AttendanceDate = time.Date,
                ClockTime = time,
                ClockWay = ClockWayEnum.Repair,
                IsInRange = true,
                Location = address,
                Remark = "",
                Latitude = "",
                Longitude = "",
                IsFieldAudit = FieldAuditEnum.Checked,
                IsAnomaly = false,
                DepartmentID = record.DepartmentID,
                CustomerID = record.CustomerID,
            };
            clockRecord = ComputeClockRecord(calculation, shiftTime, clockRecord);
            _attendanceUnitOfWork.Add(clockRecord);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "补卡执行失败");
            }
            _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), new List<RealTimeDTO> { new RealTimeDTO { clockType = true, IDcard = clockRecord.IDCard, time = clockRecord.AttendanceDate } });
            return (true, "补卡成功");
        }

        public List<ClockFieldAuditDTO> GetClockFieldAuditList(FyuUser user)
        {
            return _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.CompanyID == user.customerId && s.IsFieldAudit == FieldAuditEnum.PendingReview).Select(s => new ClockFieldAuditDTO
            {
                Name = s.Name,
                AttendanceDate = s.AttendanceDate.ToString("yyyy-MM-dd"),
                ClockImage1 = s.ClockImage1,
                ClockRecordID = s.ClockRecordID,
                ClockTime = s.ClockTime.ToString("yyyy-MM-dd HH:mm"),
                ClockType = s.ClockType.GetDescription(),
                Department = s.Department,
                EmployeeNo = s.EmployeeNo,
                IsInRange = s.IsInRange,
                Location = s.Location
            }).ToList();
        }
        /// <summary>
        /// 确认审核外勤打卡转为已审核
        /// </summary>
        /// <param name="clockRecordID"></param>
        /// <returns></returns>
        public (bool, string) UpdateClockFieldAuditList(bool isFieldAudit, List<int> list, FyuUser user)
        {
            List<RealTimeDTO> clockRecordWq = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.CompanyID == user.customerId && list.Contains(s.ClockRecordID)).Select(s => new RealTimeDTO { IDcard = s.IDCard, time = s.AttendanceDate, clockType = true }).Distinct().ToList();
            if (isFieldAudit)
            {
                _attendanceUnitOfWork.BatchUpdate<ClockRecord>(s => new ClockRecord { IsFieldAudit = FieldAuditEnum.Checked }, s => s.CompanyID == user.customerId && list.Contains(s.ClockRecordID));
            }
            else
            {
                _attendanceUnitOfWork.BatchUpdate<ClockRecord>(s => new ClockRecord { IsFieldAudit = FieldAuditEnum.NoPass }, s => s.CompanyID == user.customerId && list.Contains(s.ClockRecordID));
            }
            var res = _attendanceUnitOfWork.Commit();
            if (res)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), clockRecordWq);
                return (res, "修改成功");
            }
            return (res, "修改失败");
        }
    }
}
