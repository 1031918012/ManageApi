using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class AttendanceRecordAddService : RecordBaseService, IAttendanceRecordAddService
    {
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IShiftManagementRepository _shiftManagementRepository;
        private readonly IWeekDaysSettingRepository _weekDaysSettingRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IWorkPaidLeaveRepository _workPaidLeaveRepository;
        private readonly IRedisProvider _redisProvider;
        private readonly IAttendanceItemCatagoryRepository _attendanceItemCatagoryRepository;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IGroupAddressRepository _groupAddressRepository;
        private readonly IClockInAddressRepository _clockInAddressRepository;
        private readonly ILogger<AttendanceRecordService> _logger;

        public AttendanceRecordAddService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IAttendanceRecordRepository attendanceRecordRepository, IAttendanceGroupRepository attendanceGroupRepository, IGroupPersonnelRepository groupPersonnelRepository, IShiftManagementRepository shiftManagementRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IPersonRepository personRepository, IHolidayRepository holidayRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository, IOvertimeRepository overtimeRepository,
            IRedisProvider redisProvider, ILogger<AttendanceRecordService> logger, IEnterpriseSetRepository enterpriseSetRepository, IGroupAddressRepository groupAddressRepository, IClockInAddressRepository clockInAddressRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, attendanceItemCatagoryRepository)
        {
            _attendanceRecordRepository = attendanceRecordRepository;
            _attendanceGroupRepository = attendanceGroupRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _shiftManagementRepository = shiftManagementRepository;
            _weekDaysSettingRepository = weekDaysSettingRepository;
            _personRepository = personRepository;
            _holidayRepository = holidayRepository;
            _workPaidLeaveRepository = workPaidLeaveRepository;
            _attendanceItemCatagoryRepository = attendanceItemCatagoryRepository;
            _enterpriseSetRepository = enterpriseSetRepository;
            _overtimeRepository = overtimeRepository;
            _redisProvider = redisProvider;
            _groupAddressRepository = groupAddressRepository;
            _clockInAddressRepository = clockInAddressRepository;
            _logger = logger;
        }


        /// <summary>
        /// 检验是否有节假日
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool IsHoliday(DateTime time)
        {
            return _holidayRepository.EntityQueryable<Holiday>(s => s.HolidayYear == time.Year && s.StartHolidayTime <= time && time <= s.EndHolidayTime && !s.IsDelete).Any();
        }

        /// <summary>
        /// 计算某时间是否调班
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private bool IsWorkPaidLeave(DateTime time)
        {
            return _workPaidLeaveRepository.EntityQueryable<WorkPaidLeave>(s => s.PaidLeaveTime.Date == time.Date).Any();
        }

        #region 添加日表方法
        /// <summary>
        /// 添加日表打卡记录
        /// </summary>
        public (bool, string) DoAllAdd(DateTime time)
        {
            List<string> record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate == time.Date).Select(s => s.AttendanceRecordID).ToList();
            if (record.Any())
            {
                return (false, "今天的记录已经存在，无需添加");
            }
            _attendanceUnitOfWork.BatchDelete<AttendanceRecord>(s => s.AttendanceDate == time.Date);
            bool isHoliday = false;
            bool isWorkPaidLeave = false;
            if (IsHoliday(time))
            {
                isHoliday = true;
            }
            if (IsWorkPaidLeave(time))
            {
                isWorkPaidLeave = true;
            }
            var groupPeople = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => true);
            var people = _personRepository.EntityQueryable<Person>(s => true);

            var query = from p in people
                        join gp in groupPeople on p.IDCard equals gp.IDCard
                        select new AddComputeDayDTO
                        {
                            CustomerID = p.CustomerID,
                            DepartmentID = p.DepartmentID,
                            CompanyID = p.CompanyID,
                            CompanyName = p.CompanyName,
                            Department = p.Department,
                            Name = p.Name,
                            Position = p.Position,
                            IDCard = p.IDCard,
                            IDType = p.IDType,
                            EmployeeNo = p.JobNumber,
                            AttendanceGroupID = gp.AttendanceGroupID,
                        };
            var allGroupPeople = query.GroupBy(s => s.AttendanceGroupID).ToList();
            List<ClockMessageDTO> clockMessage = new List<ClockMessageDTO>();
            foreach (var item in allGroupPeople)
            {
                AddattendanceRecords(item.ToList(), isHoliday, isWorkPaidLeave, time, clockMessage);
                var res = _attendanceUnitOfWork.Commit();
            }
            if (clockMessage.Any())
            {
                var messagetime = clockMessage.GroupBy(s => s.MessageTime).ToList();
                foreach (var item in messagetime)
                {
                    DateTime messtime = item.FirstOrDefault().MessageTime;
                    try
                    {
                        _redisProvider.SetAddAsync(time.AddDays(messtime.Day - 1).ToString("yyyyMMdd") + messtime.ToString("HHmm"), item.ToList(), time.AddDays(3));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("消息提醒晚上添加失败" + ex.ToString());
                        continue;
                    }
                }
                return (true, "执行提交完毕");
            }
            else
            {
                return (false, "执行提交无消息的成功");
            }
        }

        /// <summary>
        /// 按身份证添加日表
        /// </summary>
        /// <param name="groupPersonnels"></param>
        /// <param name="isHoliday"></param>
        /// <param name="isWorkPaidLeave"></param>
        /// <param name="time"></param>
        private void AddattendanceRecords(List<AddComputeDayDTO> groupPersonnels, bool isHoliday, bool isWorkPaidLeave, DateTime time, List<ClockMessageDTO> clockMessage)
        {
            AttendanceGroup attendanceGroup = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.AttendanceGroupID == groupPersonnels.FirstOrDefault().AttendanceGroupID).FirstOrDefault();
            if (attendanceGroup == null)
            {
                _logger.LogError("考勤组为空" + string.Join("|", groupPersonnels.Select(s => s.IDCard)));
                return;
            }
            WeekDaysSetting weekDaysSetting = attendanceGroup.WeekDaysSettings.Where(t => t.Week == (int)time.DayOfWeek).FirstOrDefault();
            if (weekDaysSetting == null)
            {
                _logger.LogError("固定班次关系为空" + string.Join("|", groupPersonnels.Select(s => s.IDCard)));
                return;
            }
            ShiftManagement shift = _shiftManagementRepository.EntityQueryable<ShiftManagement>(s => s.ShiftID == weekDaysSetting.ShiftID).FirstOrDefault();
            if (shift == null)
            {
                _logger.LogError("班次为空" + string.Join("|", groupPersonnels.Select(s => s.IDCard)));
                return;
            }
            Overtime overtime = _overtimeRepository.EntityQueryable<Overtime>(s => s.OvertimeID == attendanceGroup.OvertimeID).FirstOrDefault();
            if (shift == null)
            {
                _logger.LogError("加班规则为空" + string.Join("|", groupPersonnels.Select(s => s.IDCard)));
                return;
            }
            var groupAddress = _groupAddressRepository.EntityQueryable<GroupAddress>(s => s.AttendanceGroupID == attendanceGroup.AttendanceGroupID).Select(s => s.ClockInAddressID).ToList();
            var address = _clockInAddressRepository.EntityQueryable<ClockInAddress>(s => groupAddress.Contains(s.ClockInAddressID)).Select(s => new ClockInAddressDTO { ClockInAddressID = s.ClockInAddressID, ClockName = s.ClockName, Distance = s.Distance, Latitude = s.Latitude, LatitudeBD = s.LatitudeBD, Longitude = s.Longitude, LongitudeBD = s.LongitudeBD, SiteName = s.SiteName }).ToList();
            if (groupAddress.Count <= 0)
            {
                _logger.LogError("考勤地址为空" + string.Join("|", groupPersonnels.Select(s => s.IDCard)));
                return;
            }
            List<AttendanceItemForComputDTO> itemDto = GetItemComputDTO(attendanceGroup.CompanyID);
            List<DayTableCalculation> dayTableCalculations = new List<DayTableCalculation>();
            List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();
            for (int i = 0; i < groupPersonnels.Count(); i++)
            {
                AddDayRecordEveryday(groupPersonnels[i], attendanceGroup, weekDaysSetting, shift, isHoliday, isWorkPaidLeave, time, clockMessage, attendanceRecords, dayTableCalculations, itemDto, overtime, address);
            }
            if (attendanceRecords.Any())
            {
                _attendanceUnitOfWork.BatchInsert(attendanceRecords);
            }
            if (dayTableCalculations.Any())
            {
                _attendanceUnitOfWork.BatchInsert(dayTableCalculations);
            }
        }

        /// <summary>
        /// 判断添加时的类型
        /// </summary>
        /// <param name="s"></param>
        /// <param name="isHoliday"></param>
        /// <param name="isWorkPaidLeave"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private void AddDayRecordEveryday(AddComputeDayDTO addComputeDayDTO, AttendanceGroup attendanceGroup, WeekDaysSetting weekDaysSetting, ShiftManagement shift, bool isHoliday, bool isWorkPaidLeave, DateTime time, List<ClockMessageDTO> clockMessage, List<AttendanceRecord> attendanceRecords, List<DayTableCalculation> dayTableCalculations, List<AttendanceItemForComputDTO> itemDto, Overtime overtime, List<ClockInAddressDTO> address)
        {
            if (attendanceGroup.ShiftType == ShiftTypeEnum.Fixed)
            {
                bool type = attendanceGroup.IsDynamicRowHugh ? (isHoliday ? false : isWorkPaidLeave ? true : weekDaysSetting.IsHolidayWork) : weekDaysSetting.IsHolidayWork;
                var AttendanceRecord = new AttendanceRecord
                {
                    AttendanceRecordID = Guid.NewGuid().ToString(),
                    AttendanceDate = time.Date,
                    AttendanceGroupID = addComputeDayDTO.AttendanceGroupID,
                    AttendanceGroupName = attendanceGroup.Name,
                    AttendanceItemDTOJson = JsonConvert.SerializeObject(itemDto),
                    Shift = type ? shift.AttendanceTime : "未排班",
                    Position = addComputeDayDTO.Position,
                    Name = addComputeDayDTO.Name,
                    CompanyID = addComputeDayDTO.CompanyID,
                    CompanyName = addComputeDayDTO.CompanyName,
                    CustomerID = addComputeDayDTO.CustomerID,
                    DepartmentID = addComputeDayDTO.DepartmentID,
                    Department = addComputeDayDTO.Department,
                    EmployeeNo = addComputeDayDTO.EmployeeNo,
                    IDCard = addComputeDayDTO.IDCard,
                    ShiftID = shift.ShiftID,
                    ShiftName = shift.ShiftName,
                    IsLackOfCard = false,
                    Status = ResultEnum.Init,
                    BusinessTripDuration = 0,
                    EarlyLeaveMinutes = 0,
                    EarlyLeaveTimes = 0,
                    LateMinutes = 0,
                    LateTimes = 0,
                    LeaveDuration = 0,
                    NotClockInTimes = 0,
                    NotClockOutTimes = 0,
                    OutsideDuration = 0,
                    WorkingTime = 0,
                };
                attendanceRecords.Add(AttendanceRecord);
                DayTableCalculation dayTableCalculation = new DayTableCalculation
                {
                    AttendanceRecordID = AttendanceRecord.AttendanceRecordID,
                    AttendanceRuleID = attendanceGroup.AttendanceRuleID,
                    ClockRule = shift.ClockRule,
                    EarlyLeaveMinutes = shift.EarlyLeaveMinutes,
                    FlexibleMinutes = shift.FlexibleMinutes,
                    IsDynamicRowHugh = attendanceGroup.IsDynamicRowHugh,
                    IsExemption = shift.IsExemption,
                    IsFlexible = shift.IsFlexible,
                    IsHoliday = isHoliday,
                    IsHolidayWork = weekDaysSetting.IsHolidayWork,
                    IsWorkPaidLeave = isWorkPaidLeave,
                    LateMinutes = shift.LateMinutes,
                    ShiftTimeManagementList = JsonConvert.SerializeObject(shift.ShiftTimeManagementList),
                    ShiftType = attendanceGroup.ShiftType,
                    SiteAttendance = JsonConvert.SerializeObject(address),
                    Week = weekDaysSetting.Week,
                    WorkHours = shift.WorkHours,
                    ExcludingOvertime = overtime.ExcludingOvertime,
                    HolidayCalculationMethod = overtime.HolidayCalculationMethod,
                    HolidayCompensationMode = overtime.HolidayCompensationMode,
                    LongestOvertime = overtime.LongestOvertime,
                    MinimumOvertime = overtime.MinimumOvertime,
                    RestCalculationMethod = overtime.RestCalculationMethod,
                    RestCompensationMode = overtime.RestCompensationMode,
                    WorkingCalculationMethod = overtime.WorkingCalculationMethod,
                    WorkingCompensationMode = overtime.WorkingCompensationMode
                };
                dayTableCalculations.Add(dayTableCalculation);
                if (type)
                {
                    foreach (var item in shift.ShiftTimeManagementList)
                    {
                        var stmessagetime = item.StartWorkTime.AddMinutes(-10).ConvertTime4();
                        var edmessagetime = item.EndWorkTime.AddMinutes(10).ConvertTime4();
                        if (shift.IsFlexible)
                        {
                            stmessagetime = stmessagetime.AddMinutes(shift.FlexibleMinutes);
                            edmessagetime = edmessagetime.AddMinutes(shift.FlexibleMinutes);
                        }
                        ClockMessageDTO Working = new ClockMessageDTO
                        {
                            ClockType = ClockTypeEnum.Working,
                            IDCard = addComputeDayDTO.IDCard,
                            Name = addComputeDayDTO.Name,
                            ShiftTimeID = item.ShiftTimeID,
                            Worktime = item.StartWorkTime,
                            MessageTime = stmessagetime
                        };

                        clockMessage.Add(Working);
                        ClockMessageDTO GetOffWork = new ClockMessageDTO
                        {
                            ClockType = ClockTypeEnum.GetOffWork,
                            IDCard = addComputeDayDTO.IDCard,
                            Name = addComputeDayDTO.Name,
                            ShiftTimeID = item.ShiftTimeID,
                            Worktime = item.EndWorkTime,
                            MessageTime = edmessagetime
                        };
                        clockMessage.Add(GetOffWork);
                    }
                }
            }
        }

        #endregion

    }
}

