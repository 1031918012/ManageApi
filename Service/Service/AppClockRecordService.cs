using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.AspNetCore.Http;
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
    public class AppClockRecordService : RecordBaseService, IAppClockRecordService
    {
        private readonly IRedisProvider _RedisProvider;
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

        public AppClockRecordService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IRedisProvider redisProvider, IClockRecordRepository clockRecordRepository, IPersonRepository personRepository, IGroupPersonnelRepository groupPersonnelRepository, IAttendanceGroupRepository attendanceGroupRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IShiftManagementRepository shiftManagementRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IHolidayRepository holidayRepository, IAttendanceRecordRepository attendanceRecordRepository, IAttendanceItemCatagoryRepository itemCatagoryRepository, ILogger<ClockRecordService> logger, ILeaveRepository leaveRepository, IEnterpriseSetRepository enterpriseSetRepository, IDayTableCalculationRepository dayTableCalculationRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, itemCatagoryRepository)
        {
            _RedisProvider = redisProvider;
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
        /// 打卡前的校验
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) ValidateBeforeClock(string IDCard)
        {
            var person = _personRepository.EntityQueryable<Person>(s => s.IDCard == IDCard, true).FirstOrDefault();
            if (person == null)
            {
                return (false, WechatCodeEnum.NoThisEmployee, "该人员未加入考勤系统");
            }
            //检查员工是否已加入考勤组
            var joinedGroup = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.IDCard == IDCard, true).FirstOrDefault();
            if (joinedGroup == null)
            {
                return (false, WechatCodeEnum.NoAttendanceGroup, "当前您未加入任何考勤组，如有疑问，请与管理员联系");
            }
            //检查员工是否需要打卡
            var group = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.AttendanceGroupID == joinedGroup.AttendanceGroupID, true).FirstOrDefault();
            if (group.ShiftType == ShiftTypeEnum.DoNotClockIn)
            {
                return (false, WechatCodeEnum.NoNeedClock, "您是考勤豁免人员，无需打卡哦");
            }
            if (!_attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == DateTime.Now.Date).Any())
            {
                return (false, WechatCodeEnum.NoRecord, "本日无法打卡，考勤设置次日生效");
            }
            return (true, WechatCodeEnum.OtherClock, "验证通过");
        }
        /// <summary>
        /// 获取打卡信息
        /// </summary>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public AppClockInfoDTO GetClockInfo(string IDCard)
        {
            var time = DateTime.Now;
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.Shift, s.AttendanceRecordID }).FirstOrDefault();
            if (record == null)
            {
                return new AppClockInfoDTO { Shift = "管理员未给您排班", Address = new List<ClockInAddressDTO>(), TodayDate = time.ToString("yyyy-MM-dd"), Week = time.DayOfWeek.ConvertWeek(), WorkedContent = "", WorkingContent = "" };
            }
            var daytable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record.AttendanceRecordID).Select(s => new { s.ShiftTimeManagementList, s.SiteAttendance, s.IsDynamicRowHugh, s.IsHoliday, s.IsHolidayWork, s.IsWorkPaidLeave }).FirstOrDefault();
            var shift = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(daytable.ShiftTimeManagementList).FirstOrDefault();
            if (shift.UpStartClockTime > time.ConvertTime2())
            {
                time = time.AddDays(-1);
                record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.Shift, s.AttendanceRecordID }).FirstOrDefault();
                if (record == null)
                {
                    return new AppClockInfoDTO { Shift = "管理员未给您排班", Address = new List<ClockInAddressDTO>(), TodayDate = time.ToString("yyyy-MM-dd"), Week = time.DayOfWeek.ConvertWeek(), WorkedContent = "", WorkingContent = "" };
                }
                daytable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record.AttendanceRecordID).Select(s => new { s.ShiftTimeManagementList, s.SiteAttendance, s.IsDynamicRowHugh, s.IsHoliday, s.IsHolidayWork, s.IsWorkPaidLeave }).FirstOrDefault();
                shift = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(daytable.ShiftTimeManagementList).FirstOrDefault();
            }
            var address = JsonConvert.DeserializeObject<List<ClockInAddress>>(daytable.SiteAttendance);
            var AddressDTO = _mapper.Map<List<ClockInAddressDTO>>(address);
            bool isworktime = shift.StartWorkTime >= time.ConvertTime() && shift.UpStartClockTime <= time.ConvertTime();
            int IsMissingCards = isworktime ? 0 : 1;
            if (!(daytable.IsDynamicRowHugh ? (daytable.IsHoliday ? false : daytable.IsWorkPaidLeave ? true : daytable.IsHolidayWork) : daytable.IsHolidayWork))
            {
                IsMissingCards = 0;
            }
            return new AppClockInfoDTO
            {
                IsMissingCards = IsMissingCards,
                Address = AddressDTO,
                Shift = "班次:" + shift.ShiftName,
                TodayDate = time.ToString("yyyy-MM-dd"),
                Week = time.DayOfWeek.ConvertWeek(),
                WorkingContent = shift.UpStartClockTime.ToString("HH:mm") + "~" + shift.StartWorkTime.ToString("HH:mm") + "打卡为正常",
                WorkedContent = shift.EndWorkTime.ToString("HH:mm") + "~次日" + shift.UpStartClockTime.ToString("HH:mm") + "打卡为正常"
            };
        }

        /// <summary>
        /// 获取今天的打卡记录
        /// </summary>
        /// <param name="locationDTO"></param>
        /// <returns></returns>
        public List<ClockRecordLiteDTO> GetTodayClock(string IDCard)
        {
            DateTime time = DateTime.Now;
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.Shift, s.AttendanceRecordID }).FirstOrDefault();
            var daytable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record.AttendanceRecordID).Select(s => s.ShiftTimeManagementList).FirstOrDefault();
            var shift = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(daytable).FirstOrDefault();
            if (shift.UpStartClockTime > time.ConvertTime2())
            {
                time = time.AddDays(-1);
            }
            var clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == time.Date && s.IDCard == IDCard, true).OrderBy(s => s.ClockTime).ToList();
            var ClockRecordList = (from clock in clockRecord
                                   select new ClockRecordLiteDTO()
                                   {
                                       ClockType = (int)clock.ClockType,
                                       ClockTime = clock.ClockTime.ToString("HH:mm"),
                                       ClockResult = clock.ClockResult.GetDescription(),
                                       Location = clock.Location,
                                       IsInRange = clock.IsInRange,
                                       IsFieldAudit = clock.IsFieldAudit == FieldAuditEnum.Checked,
                                       ClockWay = (int)clock.ClockWay
                                   }).ToList();
            return ClockRecordList;
        }

        /// <summary>
        /// 提交打卡地点（处理打卡时的经纬度）
        /// 根据传过来的位置信息判断是打内勤卡还是外勤卡
        /// </summary>
        /// <param name="locationDTO"></param>
        /// <returns></returns>
        public AppPostLocation PostLocation(string IDCard, double latitude, double longitude)
        {

            DateTime time = DateTime.Now;
            if (latitude < 0 || longitude < 0)
            {
                return new AppPostLocation { NowTime = time.ToString("HH:mm") };
            }
            var id = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => s.AttendanceRecordID).FirstOrDefault();
            var siteAttendance = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == id).Select(s => new { s.SiteAttendance, s.ShiftTimeManagementList }).FirstOrDefault();
            var shift = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(siteAttendance.ShiftTimeManagementList).FirstOrDefault();
            if (shift.UpStartClockTime > time.ConvertTime2())
            {
                time = time.AddDays(-1);
                id = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => s.AttendanceRecordID).FirstOrDefault();
                siteAttendance = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == id).Select(s => new { s.SiteAttendance, s.ShiftTimeManagementList }).FirstOrDefault();
            }
            List<ClockInAddress> locations = JsonConvert.DeserializeObject<List<ClockInAddress>>(siteAttendance.SiteAttendance);
            ClockInAddress MatchedLocation = null;
            //根据经纬度查匹配的考勤地点
            foreach (var i in locations)
            {
                var currentDistance = LocationUtil.GetDistance(i.Longitude, i.Latitude, longitude, latitude);
                if (currentDistance <= i.Distance)
                {
                    MatchedLocation = i;
                    break;
                }
            }
            if (MatchedLocation == null)
            {
                //外勤打卡
                return new AppPostLocation { IsRange = false, NowLocation = "", NowTime = time.ToString("HH:mm") };
            }
            else
            {
                //内勤打卡
                return new AppPostLocation { IsRange = true, NowLocation = MatchedLocation.ClockName, NowTime = time.ToString("HH:mm") };
            }
        }

        /// <summary>
        /// 外勤打卡
        /// </summary>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) AddOutsideClock(string IDCard, string location, double latitude, double longitude, string remark, IFormFile ImageBase64, bool IsAnomaly)
        {
            DateTime time = DateTime.Now;
            var isCross = false;
            var record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.AttendanceRecordID, s.IDCard, s.Name, s.EmployeeNo, s.Department, s.Position, s.CompanyID, s.CompanyName, s.AttendanceDate, s.DepartmentID, s.CustomerID }).FirstOrDefault();
            if (record1 == null)
            {
                return (false, WechatCodeEnum.ClockFailed, "未获取到当日记录，请联系相关人员");
            }
            var calculation = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID).FirstOrDefault();
            var shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList);
            if (shiftTime.FirstOrDefault().UpStartClockTime > time.ConvertTime2())
            {
                isCross = true;
                time = time.AddDays(-1);
                record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.AttendanceRecordID, s.IDCard, s.Name, s.EmployeeNo, s.Department, s.Position, s.CompanyID, s.CompanyName, s.AttendanceDate, s.DepartmentID, s.CustomerID }).FirstOrDefault();
                if (record1 == null)
                {
                    return (false, WechatCodeEnum.ClockFailed, "打卡失败：无班次");
                }
                calculation = calculation = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID).FirstOrDefault();
                shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList);
            }
            var clockRecord = new ClockRecord
            {
                IDCard = record1.IDCard,
                Name = record1.Name,
                EmployeeNo = record1.EmployeeNo,
                Department = record1.Department,
                Position = record1.Position,
                CompanyID = record1.CompanyID,
                CustomerID = record1.CustomerID,
                DepartmentID = record1.DepartmentID,
                CompanyName = record1.CompanyName,
                AttendanceDate = record1.AttendanceDate,
                ClockTime = time,
                ClockWay = ClockWayEnum.GPS,
                IsInRange = false,
                Location = location,
                Remark = remark,
                Longitude = longitude.ToString(),
                Latitude = latitude.ToString(),
                IsFieldAudit = FieldAuditEnum.PendingReview,
                IsAcross = isCross,
                IsAnomaly = IsAnomaly
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
                return (isSuccess, WechatCodeEnum.ClockFailed, "打卡失败：无班次");
            }
            //处理图片
            if (ImageBase64 != null)
            {
                var image = ProcessImage(ImageBase64, record1.Name, location);
                if (image.Item1 == false)
                {
                    _logger.LogError($"{record1.Name}（{record1.IDCard}）外勤打卡失败，原因：处理打卡图片失败");
                    return (isSuccess, WechatCodeEnum.ClockFailed, "打卡失败：图片处理错误");
                }
                clockRecord.ClockImage1 = image.Item2;
            }
            _attendanceUnitOfWork.Add(clockRecord);
            isSuccess = _attendanceUnitOfWork.Commit();
            if (isSuccess)
            {
                return (isSuccess, WechatCodeEnum.ClockSuccessNormal, "打卡成功");
            }
            else
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）外勤打卡失败，原因：写数据库失败");
                return (isSuccess, WechatCodeEnum.ClockFailed, "打卡失败：系统错误");
            }
        }
        /// <summary>
        /// 内勤打卡
        /// </summary>
        /// <param name="clockRecordParamDTO"></param>
        /// <returns></returns>
        public (bool, WechatCodeEnum, string) AddInsideClock(string IDCard, string location, double latitude, double longitude, bool IsAnomaly)
        {
            DateTime time = DateTime.Now;
            var isCross = false;
            var record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.AttendanceRecordID, s.IDCard, s.Name, s.EmployeeNo, s.Department, s.Position, s.CompanyID, s.CompanyName, s.AttendanceDate, s.DepartmentID, s.CustomerID }).FirstOrDefault();
            var calculation = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID).FirstOrDefault();
            var shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList);
            if (shiftTime.FirstOrDefault().UpStartClockTime > time.ConvertTime2())
            {
                isCross = true;
                time = time.AddDays(-1);
                record1 = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == time.Date).Select(s => new { s.AttendanceRecordID, s.IDCard, s.Name, s.EmployeeNo, s.Department, s.Position, s.CompanyID, s.CompanyName, s.AttendanceDate, s.DepartmentID, s.CustomerID }).FirstOrDefault();
                if (record1 == null)
                {
                    return (false, WechatCodeEnum.ClockFailed, "打卡失败：无班次");
                }
                calculation = calculation = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record1.AttendanceRecordID).FirstOrDefault();
                shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(calculation.ShiftTimeManagementList);
            }
            var clockRecord = new ClockRecord
            {
                IDCard = record1.IDCard,
                Name = record1.Name,
                EmployeeNo = record1.EmployeeNo,
                Department = record1.Department,
                Position = record1.Position,
                CompanyID = record1.CompanyID,
                CustomerID = record1.CustomerID,
                DepartmentID = record1.DepartmentID,
                CompanyName = record1.CompanyName,
                AttendanceDate = record1.AttendanceDate,
                ClockTime = time,
                ClockWay = ClockWayEnum.GPS,
                IsInRange = true,
                Location = location,
                Remark = "",
                Latitude = latitude.ToString(),
                Longitude = longitude.ToString(),
                IsFieldAudit = FieldAuditEnum.Checked,
                IsAcross = isCross,
                IsAnomaly = IsAnomaly
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
                return (isSuccess, WechatCodeEnum.ClockFailed, "打卡失败：无班次");
            }
            _attendanceUnitOfWork.Add(clockRecord);
            isSuccess = _attendanceUnitOfWork.Commit();
            if (isSuccess)
            {
                _RedisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), new List<RealTimeDTO> { new RealTimeDTO { clockType = clockRecord.ClockType == ClockTypeEnum.GetOffWork, IDcard = clockRecord.IDCard, time = clockRecord.AttendanceDate } });
                return (isSuccess, WechatCodeEnum.ClockSuccessNormal, "打卡成功");
            }
            else
            {
                _logger.LogError($"{record1.Name}（{record1.IDCard}）内勤打卡失败，原因：写数据库失败 " + JsonConvert.SerializeObject(clockRecord));
                return (isSuccess, WechatCodeEnum.ClockFailed, "打卡失败：系统错误");
            }
        }

        /// <summary>
        /// 获取日历
        /// </summary>
        /// <param name="Times"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public List<AppMonthStatics> GetAppMonthStatics(DateTime Times, string IDCard)
        {
            List<AppMonthStatics> appMonthStatics = new List<AppMonthStatics>();
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Year == Times.Year && s.AttendanceDate.Month == Times.Month && s.IDCard == IDCard).Select(s => new { s.Shift, s.IsLackOfCard, s.LateTimes, s.EarlyLeaveTimes, s.Status, s.AttendanceDate }).ToList().OrderBy(s => s.AttendanceDate).ToList();
            foreach (var s in record)
            {
                int statue = 0;
                if (s.Shift != "未排班")
                {
                    if (s.Status == ResultEnum.Normal)
                    {
                        statue = 1;
                    }
                    if (s.LateTimes > 0 || s.EarlyLeaveTimes > 0)
                    {
                        statue = 2;
                    }
                    if (s.IsLackOfCard)
                    {
                        statue = 3;
                    }
                }
                AppMonthStatics appMonth = new AppMonthStatics
                {
                    DateTime = s.AttendanceDate,
                    Statue = statue
                };
                appMonthStatics.Add(appMonth);
            }
            return appMonthStatics;
        }

        /// <summary>
        /// 获取月统计
        /// </summary>
        /// <param name="Times"></param>
        /// <param name="IDCard"></param>
        /// <returns></returns>
        public AppStatics GetAppStatics(DateTime Times, string IDCard)
        {
            AppStatics appStatics = new AppStatics();

            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Year == Times.Year && s.AttendanceDate.Month == Times.Month && s.IDCard == IDCard && s.Status != 0 && s.Shift != "未排班").Select(s => new { s.AttendanceDate, s.Shift, s.LateTimes, s.EarlyLeaveTimes, s.NotClockInTimes, s.NotClockOutTimes, s.WorkingTime, s.AttendanceItemDTOJson }).ToList();
            var clockrecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate.Year == Times.Year && s.AttendanceDate.Month == Times.Month && s.IDCard == IDCard && s.IsFieldAudit == FieldAuditEnum.Checked).ToList();
            var workTimelist = record.Select(s => s.WorkingTime).ToList();
            appStatics.AvgWorkHours = 0;
            if (workTimelist.Any())
            {
                appStatics.AvgWorkHours = workTimelist.Sum() / workTimelist.Count;
            }
            appStatics.LateTimes = (int)record.Select(s => s.LateTimes).Sum();
            appStatics.EarlyLeaveTimes = (int)record.Select(s => s.EarlyLeaveTimes).Sum();
            appStatics.NotClockTimes = (int)record.Select(s => s.NotClockInTimes).Sum() + (int)record.Select(s => s.NotClockOutTimes).Sum();
            foreach (var item in record)
            {
                var attendanceItemDTOJson = JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(item.AttendanceItemDTOJson);
                appStatics.OvertimeHours += attendanceItemDTOJson.Where(s => (s.AttendanceItemName == "工作日加班" || s.AttendanceItemName == "节假日加班" || s.AttendanceItemName == "休息日加班") && s.Unit == "小时").FirstOrDefault().AttendanceItemValue;
                appStatics.AttendanceDays += clockrecord.Where(s => s.AttendanceDate == item.AttendanceDate).Any() ? 1 : 0;
            }

            appStatics.AttendanceDays = Math.Round(appStatics.AttendanceDays, 1);
            appStatics.AvgWorkHours = Math.Round(appStatics.AvgWorkHours, 2);
            appStatics.OvertimeHours = Math.Round(appStatics.OvertimeHours, 2);
            return appStatics;
        }

        /// <summary>
        /// 获取所有打卡
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public DayClockSelect GetAnyClock(DateTime dateTime, string IDCard)
        {
            DayClockSelect dayClock = new DayClockSelect();
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.IDCard == IDCard && s.AttendanceDate == dateTime.Date).Select(s => new { s.AttendanceDate, s.ShiftName, s.Shift, s.AttendanceRecordID }).FirstOrDefault();
            if (record == null)
            {
                dayClock.Shift = "未排班";
                return dayClock;
            }
            var dattable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record.AttendanceRecordID).FirstOrDefault();
            var shift = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(dattable.ShiftTimeManagementList);
            dayClock.NowDay = record.AttendanceDate.ToString("yyyy-MM-dd") + record.AttendanceDate.DayOfWeek.ConvertWeek();
            dayClock.Shift = record.Shift;
            var clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == dateTime.Date && s.IDCard == IDCard && s.ShiftTimeID == shift.FirstOrDefault().ShiftTimeID, true).OrderBy(s => s.ClockTime).ToList();
            dayClock.ClockList = (from clock in clockRecord
                                  select new ClockRecordLiteDTO()
                                  {
                                      ClockTime = clock.ClockTime.ToString("HH:mm"),
                                      ClockResult = clock.ClockResult.GetDescription(),
                                      Location = clock.Location,
                                      IsInRange = clock.IsInRange,
                                      IsFieldAudit = clock.IsFieldAudit == FieldAuditEnum.Checked,
                                      ClockWay = (int)clock.ClockWay
                                  }).ToList();
            TimeSpan workingHours = new TimeSpan();
            if (clockRecord.Any())
            {
                var startWorkTime = clockRecord.Where(s => s.ClockType == ClockTypeEnum.Working).FirstOrDefault();
                var endWorkTime = clockRecord.Where(s => s.ClockType == ClockTypeEnum.GetOffWork).LastOrDefault();
                if (startWorkTime != null && endWorkTime != null)
                {
                    workingHours = endWorkTime.ClockTime.ConvertTime5().Subtract(startWorkTime.ClockTime.ConvertTime5());
                    foreach (var item in shift)
                    {
                        if (item.EndRestTime != null && item.StartRestTime != null)
                        {
                            workingHours = workingHours.Subtract(((DateTime)item.EndRestTime).ConvertTime5() - ((DateTime)item.StartRestTime).ConvertTime5());
                        }
                    }
                }
            }
            dayClock.WorkHours = workingHours.ToString("%h") + "小时" + workingHours.ToString("%m") + "分";
            return dayClock;
        }


        public List<AppShiftDTO> GetMonthShift(DateTime dateTime, string IDCard)
        {
            var groupperson = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.IDCard == IDCard, true).FirstOrDefault();
            if (groupperson == null)
            {
                return null;
            }
            var group = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.AttendanceGroupID == groupperson.AttendanceGroupID).FirstOrDefault();
            var weekDays = _weekDaysSettingRepository.EntityQueryable<WeekDaysSetting>(s => s.AttendanceGroupID == groupperson.AttendanceGroupID, true).ToList();
            var shifts = _shiftManagementRepository.EntityQueryable<ShiftManagement>(s => weekDays.Select(t => t.ShiftID).Contains(s.ShiftID)).ToList();
            if (!shifts.Any())
            {
                return null;
            }
            var holidays = _holidayRepository.EntityQueryable<Holiday>(s => true, true).ToList();
            var workPay = _workPaidLeaveRepository.EntityQueryable<WorkPaidLeave>(s => true, true).ToList();
            var timeall = dateTime.AddMonths(1);
            List<AppShiftDTO> res = new List<AppShiftDTO>();
            for (DateTime i = dateTime.Date; i < timeall; i = i.AddDays(1).Date)
            {
                WeekDaysSetting week = weekDays.Where(s => s.Week == (int)i.DayOfWeek).FirstOrDefault();
                ShiftManagement shiftOne = shifts.Where(s => s.ShiftID == week.ShiftID && !s.IsDelete).FirstOrDefault();
                var shiftany = shiftOne != null;
                var holiday = holidays.Where(s => s.StartHolidayTime <= i && s.EndHolidayTime >= i && !s.IsDelete).Any();
                var workPayany = workPay.Where(s => s.PaidLeaveTime.Date == i).Any();
                AppShiftDTO appShiftDTO = new AppShiftDTO
                {
                    Time = i.ToString("yyyy-MM-dd"),
                    Shift = shiftany ? ShiftCheck(shiftOne, i) : null,
                };
                if (week.IsHolidayWork)
                {
                    appShiftDTO.ShowContent = "班";
                }
                else
                {
                    appShiftDTO.ShowContent = "休息日";
                }
                if (group.IsDynamicRowHugh && holiday)
                {
                    appShiftDTO.ShowContent = "节假日";
                }
                if (group.IsDynamicRowHugh && workPayany)
                {
                    appShiftDTO.ShowContent = "班";
                }
                res.Add(appShiftDTO);
            }
            return res;
        }
        private AppShiftDetailDTO ShiftCheck(ShiftManagement shift, DateTime time)
        {
            AppShiftDetailDTO appShiftDetailDTO = new AppShiftDetailDTO
            {
                AttendanceTime = time.ToString("yyyy年MM月dd日") + time.DayOfWeek.ConvertWeekStatic(),
                EndWorkTime = shift.ShiftTimeManagementList.FirstOrDefault().EndWorkTime.ToString("HH:mm"),
                StartWorkTime = shift.ShiftTimeManagementList.FirstOrDefault().StartWorkTime.ToString("HH:mm"),
                ShiftName = shift.ShiftName,
                WorkHours = shift.WorkHours,
                HumanizedSetting = "无"
            };
            if (shift.IsFlexible)
            {
                appShiftDetailDTO.HumanizedSetting = "允许晚到" + shift.FlexibleMinutes + "分钟，晚到晚走，累计工作时长达到标准工作时长即可";
            }
            if (shift.IsExemption)
            {
                appShiftDetailDTO.HumanizedSetting = "允许晚到" + shift.LateMinutes + "分钟内不记迟到， 早退" + shift.EarlyLeaveMinutes + "分钟内不记早退";

            }
            return appShiftDetailDTO;
        }
        #region 私有方法
        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="imageBase64"></param>
        /// <returns></returns>
        private (bool, string) ProcessImage(IFormFile imageBase64, string name, string addresse)
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
                addresse = string.IsNullOrWhiteSpace(addresse.Trim()) ? "暂无地址" : addresse;
                using (FileStream fs = System.IO.File.Create(path))
                {
                    imageBase64.CopyTo(fs);
                    fs.Flush();
                }
                //ImageHelper.AddWaterMark(imageBase64, name, addresse, DateTime.Now, path);
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
        #endregion

    }
}