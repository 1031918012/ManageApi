using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AttendanceMonthlyRecordService : RecordBaseService, IAttendanceMonthlyRecordService
    {
        private readonly IWeekDaysSettingRepository _weekDaysSettingRepository;
        private readonly IAttendanceRuleRepository _attendanceRuleRepository;
        private readonly IAttendanceRuleDetailRepository _attendanceRuleDetailRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        private readonly IAttendanceMonthlyRecordRepository _attendanceMonthlyRecordRepository;
        private readonly IEnterpriseSetRepository _enterpriseSetRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IShiftManagementRepository _shiftManagementRepository;
        private readonly IWorkPaidLeaveRepository _workPaidLeaveRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IDayTableCalculationRepository _dayTableCalculationRepository;
        private readonly IAttendanceItemCatagoryRepository _attendanceItemCatagoryRepository;

        protected IEnumerable<WorkPaidLeave> _workPaidLeaves;
        protected decimal absent = 0;
        protected decimal personalReason = 0;
        protected decimal durationOfAttendance = 0;
        public AttendanceMonthlyRecordService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, IAttendanceGroupRepository attendanceGroupRepository, IGroupPersonnelRepository groupPersonnelRepository, IAttendanceRuleRepository attendanceRuleRepository, ISerializer<string> serializer, IWeekDaysSettingRepository weekDaysSettingRepository, IAttendanceMonthlyRecordRepository attendanceMonthlyRecordRepository, IAttendanceRuleDetailRepository attendanceRuleDetailRepository, IEnterpriseSetRepository enterpriseSetRepository, IHolidayRepository holidayRepository, IAttendanceRecordRepository attendanceRecordRepository, IShiftManagementRepository shiftManagementRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IPersonRepository personRepository, ILeaveRepository leaveRepository, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository, IDayTableCalculationRepository dayTableCalculationRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, attendanceItemCatagoryRepository)
        {
            _attendanceGroupRepository = attendanceGroupRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _attendanceRuleRepository = attendanceRuleRepository;
            _weekDaysSettingRepository = weekDaysSettingRepository;
            _attendanceMonthlyRecordRepository = attendanceMonthlyRecordRepository;
            _attendanceRuleDetailRepository = attendanceRuleDetailRepository;
            _enterpriseSetRepository = enterpriseSetRepository;
            _holidayRepository = holidayRepository;
            _attendanceRecordRepository = attendanceRecordRepository;
            _shiftManagementRepository = shiftManagementRepository;
            _workPaidLeaveRepository = workPaidLeaveRepository;
            _personRepository = personRepository;
            _leaveRepository = leaveRepository;
            _attendanceItemCatagoryRepository = attendanceItemCatagoryRepository;
            _dayTableCalculationRepository = dayTableCalculationRepository;
        }
        /// <summary>
        /// 月表分页数据
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="idCards"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<AttendanceMonthlyRecordDTO> GetMonthlyRecordList(int year, int month, List<string> idCards, int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<AttendanceMonthlyRecord, bool>> expression = s => s.CompanyID == user.customerId;

            if (year != 0)
            {
                expression = expression.And(s => s.AttendanceDate.Year == year);
            }

            if (month != 0)
            {
                expression = expression.And(s => s.AttendanceDate.Month == month);
            }

            if (idCards.Any())
            {
                expression = expression.And(s => idCards.Contains(s.IDCard));
            }

            var list = _attendanceMonthlyRecordRepository.GetByPage(pageIndex, pageSize, expression, a => a.EmployeeNo, SortOrderEnum.Ascending);
            foreach (var item in list.Data)
            {
                item.AttendanceProjects = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(item.AttendanceProjectsJson);
            }
            var pageResult = PageMap<AttendanceMonthlyRecordDTO, AttendanceMonthlyRecord>(list);


            return pageResult;
        }
        /// <summary>
        /// 计算月表
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="attendanceRuleDetails"></param>
        /// <param name="shiftManagement"></param>
        /// <returns></returns>
        public bool ComputeMonthlyRecordByIdCard(FyuUser fyuUser, string idCard, int year, int month)
        {
            var recordAll = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Year == year && s.AttendanceDate.Month == month && s.IDCard == idCard).ToList();
            var dayTable = _attendanceRecordRepository.EntityQueryable<DayTableCalculation>(s => recordAll.Select(t => t.AttendanceRecordID).Contains(s.AttendanceRecordID)).ToList();

            //获取公司所有考勤组
            //var attendanceGroups = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(a => a.CompanyID == fyuUser.customerId).ToList();

            //var ruleIds = attendanceGroups.Select(a => a.AttendanceRuleID);
            ////获取公司所有班次
            //var shiftManagements = _shiftManagementRepository.EntityQueryable<ShiftManagement>(a => a.CompanyID == fyuUser.customerId).ToList();

            ////获取考勤所有考勤组对应的考勤规则
            //var attendanceRuleDetails = _attendanceRuleDetailRepository.EntityQueryable<AttendanceRuleDetail>(a => ruleIds.Contains(a.AttendanceRuleID)).ToList();

            ////获取当年所有节假日
            //var holidays = GetHolidays(DateTime.Now);

            ////获取公司所有人当月的日表记录(不包括统计当天)
            //var attendanceRecordList = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(a => a.CompanyID == fyuUser.customerId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month && a.IDCard == idCard && a.ClockInResult1 != ClockResultEnum.NotWorkCard).ToList();
            //_workPaidLeaves = GetWorkPaidLeaves();
            ////获取考勤项
            //var attendanceItemComputDTOs = GetItemComputDTO(fyuUser);

            //List<AttendanceMonthlyRecord> attendanceMonthlyRecords = new List<AttendanceMonthlyRecord>();

            ////通过idcard分组把当月每天的打卡记录累加
            ////foreach (var attendanceRecords in attendanceRecordList)
            ////{
            //var firstRecord = attendanceRecordList.FirstOrDefault();
            //AttendanceMonthlyRecord attendanceMonthlyRecord = new AttendanceMonthlyRecord
            //{
            //    AttendanceDate = new DateTime(year, month, 1),
            //    AttendanceMonthlyRecordID = Guid.NewGuid().ToString(),
            //    CompanyID = fyuUser.customerId,
            //    CompanyName = fyuUser.customerName,
            //    Department = firstRecord.Department,
            //    EmployeeNo = firstRecord.EmployeeNo,
            //    IDCard = firstRecord.IDCard,
            //    Name = firstRecord.Name,
            //    Position = firstRecord.Position
            //};
            //var attendanceItemComputDTOList = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(_serializer.Serialize(attendanceItemComputDTOs));


            ////对每个人进行累加
            //foreach (var attendanceRecord in attendanceRecordList)
            //{
            //    //获取所属考勤组
            //    var attendanceGroup = attendanceGroups.Where(a => a.AttendanceGroupID == firstRecord.AttendanceGroupID).FirstOrDefault();
            //    if (attendanceGroup == null)
            //    {
            //        continue;
            //    }
            //    //获取考勤组选择的考勤规则
            //    var ruleDetails = attendanceRuleDetails.Where(a => a.AttendanceRuleID == attendanceGroup.AttendanceRuleID);
            //    //获取当天班次
            //    var weekDay = attendanceGroup.WeekDaysSettings.Where(a => a.Week == (int)attendanceRecord.AttendanceDate.DayOfWeek).FirstOrDefault();
            //    var shiftManagement = shiftManagements.Where(a => a.ShiftID == weekDay.ShiftID).FirstOrDefault();
            //    //计算应出勤天数、小时
            //    var computeDaysOfAttendance = ComputeDaysOfAttendance(attendanceRecord.AttendanceDate, attendanceGroup.WeekDaysSettings.ToList(), holidays, shiftManagements, attendanceGroup.IsDynamicRowHugh);
            //    var daysOfAttendance = computeDaysOfAttendance.Item1;
            //    var hoursOfAttendance = computeDaysOfAttendance.Item2;
            //    //计算每天考勤
            //    Compute(attendanceRecord, attendanceItemComputDTOList, ruleDetails, shiftManagement, daysOfAttendance, hoursOfAttendance);
            //}
            //attendanceMonthlyRecord.AttendanceProjectsJson = _serializer.Serialize(attendanceItemComputDTOList);
            //attendanceMonthlyRecords.Add(attendanceMonthlyRecord);
            ////}
            //_attendanceUnitOfWork.BatchDelete<AttendanceMonthlyRecord>(a => a.CompanyID == fyuUser.customerId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month && a.IDCard == idCard);
            //if (attendanceMonthlyRecords.Count > 0)
            //{
            //    _attendanceUnitOfWork.BatchInsert(attendanceMonthlyRecords);
            //}
            return _attendanceUnitOfWork.Commit();
        }


        public (bool, int) ComputeMonthlyRecord(FyuUser fyuUser, int year, int month, string idCard)
        {
            bool flag = false;
            ////获取公司所有考勤组
            ////var attendanceGroups = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(a => a.CompanyID == fyuUser.customerId).ToList();

            ////var ruleIds = attendanceGroups.Select(a => a.AttendanceRuleID);
            ////获取公司所有班次
            //var shiftManagements = _shiftManagementRepository.EntityQueryable<ShiftManagement>(a => a.CompanyID == fyuUser.customerId).ToList();

            ////获取考勤所有考勤组对应的考勤规则
            //// var attendanceRuleDetails = _attendanceRuleDetailRepository.EntityQueryable<AttendanceRuleDetail>(a => ruleIds.Contains(a.AttendanceRuleID)).ToList();
            //var attendanceRules = _attendanceRuleRepository.EntityQueryable<AttendanceRule>(a => a.CompanyID == fyuUser.customerId).Select(s => s.AttendanceRuleID).ToList();
            //var attendanceRuleDetails = _attendanceRuleDetailRepository.EntityQueryable<AttendanceRuleDetail>(a => attendanceRules.Contains(a.AttendanceRuleID)).ToList();
            ////获取当年所有节假日
            //var holidays = GetHolidays(year);

            ////获取公司所有人当月的日表记录(不包括统计当天)
            //var attendanceRecordList = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(a => a.CompanyID == fyuUser.customerId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month && a.AttendanceDate.Date < DateTime.Now.Date && a.ClockInResult1 != ClockResultEnum.NotWorkCard && a.IDCard == idCard, true).OrderBy(a => a.AttendanceDate).GroupBy(a => a.IDCard).ToList();
            ////获取所有人身份证
            //var idCards = attendanceRecordList.Select(a => a.Key);
            //var persons = _personRepository.EntityQueryable<Person>(s => idCards.Contains(s.IDCard)).ToList();
            //_workPaidLeaves = GetWorkPaidLeaves();
            ////获取考勤项
            //var attendanceItemComputDTOs = GetItemComputDTO(fyuUser.customerId);

            //var leaves = _leaveRepository.EntityQueryable<Leave>(s => s.CompanyID == fyuUser.customerId, true).ToList();

            List<AttendanceMonthlyRecord> attendanceMonthlyRecords = new List<AttendanceMonthlyRecord>();

            ////通过idcard分组把当月每天的打卡记录累加
            //foreach (var attendanceRecords in attendanceRecordList)
            //{
            //    var firstRecord = attendanceRecords.FirstOrDefault();
            //    AttendanceMonthlyRecord attendanceMonthlyRecord = new AttendanceMonthlyRecord
            //    {
            //        AttendanceDate = new DateTime(year, month, 1),
            //        AttendanceMonthlyRecordID = Guid.NewGuid().ToString(),
            //        CompanyID = fyuUser.customerId,
            //        CompanyName = fyuUser.customerName,
            //        Department = firstRecord.Department,
            //        EmployeeNo = firstRecord.EmployeeNo,
            //        IDCard = firstRecord.IDCard,
            //        Name = firstRecord.Name,
            //        Position = firstRecord.Position
            //    };
            //    var person = persons.Where(a => a.IDCard == firstRecord.IDCard).FirstOrDefault();
            //    //没找到人员则计算下一个人
            //    if (person == null)
            //    {
            //        continue;
            //    }
            //    var attendanceItemComputDTOList = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(_serializer.Serialize(attendanceItemComputDTOs));

            //    List<AttendanceRuleDetail> ruleDetails = null;
            //    absent = 0;
            //    int daysOfAttendance = 0;
            //    decimal hoursOfAttendance = 0;
            //    var attendanceGroupFrist = _serializer.Desrialize<string, AttendanceGroup>(firstRecord.AttendanceGroupJson);
            //    if (attendanceGroupFrist == null)
            //    {
            //        continue;
            //    }
            //    //计算应出勤天数、小时
            //    var computeDaysOfAttendance = ComputeDaysOfAttendance(firstRecord.AttendanceDate, attendanceGroupFrist.WeekDaysSettings.ToList(), holidays, shiftManagements, attendanceGroupFrist.IsDynamicRowHugh, person.Hiredate);
            //    daysOfAttendance = computeDaysOfAttendance.Item1;
            //    hoursOfAttendance = computeDaysOfAttendance.Item2;
            //    //personalReason = 0;
            //    //对每个人进行累加
            //    foreach (var attendanceRecord in attendanceRecords)
            //    {
            //        //获取所属考勤组
            //        //var attendanceGroup = attendanceGroups.Where(a => a.AttendanceGroupID == firstRecord.AttendanceGroupID).FirstOrDefault();
            //        var attendanceGroup = _serializer.Desrialize<string, AttendanceGroup>(attendanceRecord.AttendanceGroupJson);
            //        if (attendanceGroup == null)
            //        {
            //            continue;
            //        }
            //        //获取考勤组选择的考勤规则
            //        ruleDetails = attendanceRuleDetails.Where(a => a.AttendanceRuleID == attendanceGroup.AttendanceRuleID).ToList();
            //        //获取当天班次
            //        //var weekDay = attendanceGroup.WeekDaysSettings.Where(a => a.Week == (int)attendanceRecord.AttendanceDate.DayOfWeek).FirstOrDefault();
            //        //var shiftManagement = shiftManagements.Where(a => a.ShiftID == weekDay.ShiftID).FirstOrDefault();
            //        var shiftManagement = _serializer.Desrialize<string, ShiftManagement>(attendanceRecord.ShiftJson);
            //        //计算每天考勤
            //        Compute(attendanceRecord, attendanceItemComputDTOList, ruleDetails, shiftManagement, daysOfAttendance, hoursOfAttendance, leaves);
            //    }
            //    if (hoursOfAttendance != 0 && daysOfAttendance != 0)
            //    {
            //        ComputeNotClockMonthly(attendanceItemComputDTOList, ruleDetails, hoursOfAttendance / daysOfAttendance);
            //        ComputeDurationOfAttendanceMonthly(attendanceItemComputDTOList);
            //    }

            //    attendanceMonthlyRecord.AttendanceProjectsJson = _serializer.Serialize(attendanceItemComputDTOList);
            //    attendanceMonthlyRecords.Add(attendanceMonthlyRecord);
            //}
            //_attendanceUnitOfWork.BatchDelete<AttendanceMonthlyRecord>(a => a.CompanyID == fyuUser.customerId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month && a.IDCard == idCard);
            //if (attendanceMonthlyRecords.Count > 0)
            //{
            //    _attendanceUnitOfWork.BatchInsert(attendanceMonthlyRecords);
            //}
            flag = _attendanceUnitOfWork.Commit();
            return (flag, attendanceMonthlyRecords.Count);
        }


        private void ComPuteMonthRule(List<AttendanceItemForComputDTO> AttendanceItemDTOJson, AttendanceRule rule, List<AttendanceRuleDetail> ruleDeatil, decimal WorkHours)
        {
            bool isMonthNotClock = rule.NotClockRule == NotClockEnum.AbsenceGradient;
            var notClockRule = ruleDeatil.Where(s => s.RuleType == RuleTypeEnum.NotClock).ToList();
            decimal outClock = 0;
            if (isMonthNotClock)
            {
                foreach (var item in AttendanceItemDTOJson.Where(s => s.AttendanceItemName.Contains("缺卡") && s.Unit == "次数").ToList())
                {
                    outClock += item.AttendanceItemValue;
                    item.AttendanceItemValue = 0;
                };
                AttendanceRuleDetail attendanceRuleDetail = ConputeRule(notClockRule, outClock);
                if (attendanceRuleDetail != null)
                {
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == attendanceRuleDetail.CallRuleType.GetDescription() && s.Unit == "小时").FirstOrDefault().AttendanceItemValue += attendanceRuleDetail.Time;
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue = AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue - attendanceRuleDetail.Time;
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue =
                        AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue + attendanceRuleDetail.Time / WorkHours;
                }
            }
        }

        private void ComPuteDayRule(List<AttendanceItemForComputDTO> AttendanceItemDTOJson, List<AttendanceItemForComputDTO> Item, AttendanceRule rule, List<AttendanceRuleDetail> ruleDeatil, DayTableCalculation dayTableCalculation)
        {
            bool isDayLate = rule.LateRule == LateEnum.LateGradient;
            bool isDayEarlyLeave = rule.EarlyLeaveRule == EarlyLeaveEnum.EarlyLeaveGradient;
            foreach (var a in Item)
            {
                if (isDayLate && a.AttendanceItemName == "迟到" && a.Unit == "分钟数" && a.AttendanceItemValue > 0)
                {
                    var lateRule = ruleDeatil.Where(s => s.RuleType == RuleTypeEnum.Late).ToList();
                    AttendanceRuleDetail attendanceRuleDetail = ConputeRule(lateRule, a.AttendanceItemValue);
                    if (attendanceRuleDetail != null)
                    {
                        SetRuleValue(attendanceRuleDetail, AttendanceItemDTOJson, dayTableCalculation, a, RuleTypeEnum.Late);
                        continue;
                    }
                }
                if (isDayEarlyLeave && a.AttendanceItemName == "早退" && a.Unit == "分钟数" && a.AttendanceItemValue > 0)
                {
                    var earlyLeaveRule = ruleDeatil.Where(s => s.RuleType == RuleTypeEnum.EarlyLeave).ToList();
                    AttendanceRuleDetail attendanceRuleDetail = ConputeRule(earlyLeaveRule, a.AttendanceItemValue);
                    if (attendanceRuleDetail != null)
                    {
                        SetRuleValue(attendanceRuleDetail, AttendanceItemDTOJson, dayTableCalculation, a, RuleTypeEnum.EarlyLeave);
                        continue;
                    }
                }
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == a.AttendanceItemName && s.Unit == a.Unit).FirstOrDefault().AttendanceItemValue += a.AttendanceItemValue;
            }
        }

        private void SetRuleValue(AttendanceRuleDetail attendanceRuleDetail, List<AttendanceItemForComputDTO> AttendanceItemDTOJson, DayTableCalculation dayTableCalculation, AttendanceItemForComputDTO a, RuleTypeEnum ruleTypeEnum)
        {
            if (attendanceRuleDetail.CallRuleType == ruleTypeEnum)
            {
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == attendanceRuleDetail.CallRuleType.GetDescription() && s.Unit == "分钟数").FirstOrDefault().AttendanceItemValue += attendanceRuleDetail.Time;
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue = AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue + a.AttendanceItemValue / 60;
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue =
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue + a.AttendanceItemValue / 60 / dayTableCalculation.WorkHours;
            }
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.PersonalReason)
            {
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == attendanceRuleDetail.CallRuleType.GetDescription() && s.Unit == "小时").FirstOrDefault().AttendanceItemValue += attendanceRuleDetail.Time;
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == attendanceRuleDetail.CallRuleType.GetDescription() && s.Unit == "天").FirstOrDefault().AttendanceItemValue += (attendanceRuleDetail.Time / dayTableCalculation.WorkHours);
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue = AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue - attendanceRuleDetail.Time + a.AttendanceItemValue / 60;
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue =
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue - attendanceRuleDetail.Time / dayTableCalculation.WorkHours + a.AttendanceItemValue / 60 / dayTableCalculation.WorkHours;
            }
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.Absent)
            {
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == attendanceRuleDetail.CallRuleType.GetDescription() && s.Unit == "天").FirstOrDefault().AttendanceItemValue += (attendanceRuleDetail.Time / dayTableCalculation.WorkHours);
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue = AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue - attendanceRuleDetail.Time + a.AttendanceItemValue / 60;
                AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue =
                    AttendanceItemDTOJson.Where(s => s.AttendanceItemName == "实出勤时长" && s.Unit == "天").FirstOrDefault().AttendanceItemValue - attendanceRuleDetail.Time / dayTableCalculation.WorkHours + a.AttendanceItemValue / 60 / dayTableCalculation.WorkHours;
            }
        }

        public AttendanceRuleDetail ConputeRule(List<AttendanceRuleDetail> ruleDeatil, decimal value)
        {
            foreach (var item in ruleDeatil)
            {
                if (item.MinJudge == SymbolEnum.LessThan && item.MaxJudge == SymbolEnum.LessThan && item.MinTime < value && value < item.MaxTime)
                {
                    return item;
                }
                if (item.MinJudge == SymbolEnum.LessThan && item.MaxJudge == SymbolEnum.LessThanOrEqualTo && item.MinTime < value && value <= item.MaxTime)
                {
                    return item;
                }
                if (item.MinJudge == SymbolEnum.LessThanOrEqualTo && item.MaxJudge == SymbolEnum.LessThan && item.MinTime <= value && value < item.MaxTime)
                {
                    return item;
                }
                if (item.MinJudge == SymbolEnum.LessThanOrEqualTo && item.MaxJudge == SymbolEnum.LessThanOrEqualTo && item.MinTime <= value && value <= item.MaxTime)
                {
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 计算月表
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="attendanceRuleDetails"></param>
        /// <param name="shiftManagement"></param>
        /// <returns></returns>
        public (bool, int) ComputeMonthlyRecord(FyuUser fyuUser, int year, int month)
        {
            bool flag = false;
            var recordAll = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(a => a.CompanyID == fyuUser.customerId && a.Shift != "未排班" && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month && a.AttendanceDate.Date < DateTime.Now.Date);
            var dayTable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => true);
            var query = from record in recordAll
                        join dayData in dayTable
                        on record.AttendanceRecordID equals dayData.AttendanceRecordID
                        select new { record, dayData };
            var records = query.ToList().OrderBy(s => s.record.AttendanceDate).GroupBy(s => s.record.IDCard);
            if (!recordAll.Any())
            {
                return (false, 0);
            }
            // .OrderBy(s => s.AttendanceDate).GroupBy(s => s.IDCard)
            var ruleAll = _attendanceRuleRepository.EntityQueryable<AttendanceRule>(s => s.CompanyID == fyuUser.customerId).ToList();

            var ruleIDs = ruleAll.Select(s => s.AttendanceRuleID);
            //最后一天的规则

            List<AttendanceRuleDetail> ruleDeatils = _attendanceRuleDetailRepository.EntityQueryable<AttendanceRuleDetail>(s => ruleIDs.Contains(s.AttendanceRuleID)).ToList();
            ConcurrentBag<AttendanceMonthlyRecord> attendanceMonthlyRecords = new ConcurrentBag<AttendanceMonthlyRecord>();

            List<AttendanceItemForComputDTO> AttendanceItemDTOJsonAll = GetItemComputDTO(fyuUser.customerId);

            Parallel.ForEach(records, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, item =>
            {
                //计算每个人工作日的数据
                var record = item;
                var AttendanceItemDTOJson = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(_serializer.Serialize(AttendanceItemDTOJsonAll));
                AttendanceRule rule = ruleAll.Where(s => s.AttendanceRuleID == record.LastOrDefault().dayData.AttendanceRuleID).FirstOrDefault();

                var ruleDeatil = ruleDeatils.Where(s => s.AttendanceRuleID == rule.AttendanceRuleID).ToList();

                foreach (var oneRecord in record)
                {
                    var Item = JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(oneRecord.record.AttendanceItemDTOJson);
                    ComPuteDayRule(AttendanceItemDTOJson, Item, rule, ruleDeatil, oneRecord.dayData);
                }
                decimal workHours = record.Select(s => s.dayData.WorkHours).Sum() / record.Count();
                ComPuteMonthRule(AttendanceItemDTOJson, rule, ruleDeatil, workHours);
                var person = record.LastOrDefault().record;
                AttendanceMonthlyRecord attendanceMonthlyRecord = new AttendanceMonthlyRecord
                {
                    AttendanceDate = new DateTime(year, month, 1),
                    AttendanceMonthlyRecordID = Guid.NewGuid().ToString(),
                    AttendanceProjectsJson = JsonConvert.SerializeObject(AttendanceItemDTOJson),
                    CompanyID = person.CompanyID,
                    CompanyName = person.CompanyName,
                    Department = person.Department,
                    EmployeeNo = person.EmployeeNo,
                    IDCard = person.IDCard,
                    Name = person.Name,
                    Position = person.Name
                };
                attendanceMonthlyRecords.Add(attendanceMonthlyRecord);
            });
            _attendanceUnitOfWork.BatchDelete<AttendanceMonthlyRecord>(a => a.CompanyID == fyuUser.customerId && a.AttendanceDate.Year == year && a.AttendanceDate.Month == month);
            _attendanceUnitOfWork.BatchInsert(attendanceMonthlyRecords.ToList());
            flag = _attendanceUnitOfWork.Commit();
            return (flag, attendanceMonthlyRecords.Count);
        }


        public void ComputeNotClockMonthly(List<AttendanceItemForComputDTO> attendanceItemComputDTOList, List<AttendanceRuleDetail> ruleDetails, decimal hoursOfAttendanceAverage)
        {
            var notClockInTimes = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "上班缺卡" && a.Unit == "次数").FirstOrDefault();
            var notClockOutTimes = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "下班缺卡" && a.Unit == "次数").FirstOrDefault();
            var notClockTimes = notClockInTimes.AttendanceItemValue + notClockOutTimes.AttendanceItemValue;
            var notClockAttendanceRules = ruleDetails.Where(a => a.RuleType == RuleTypeEnum.NotClock).OrderBy(a => a.MinTime);
            if (!notClockAttendanceRules.Any())
            {
                return;
            }
            var maxTime = notClockAttendanceRules.LastOrDefault();
            var notClockAttendanceRule = GetRuleDetail(notClockAttendanceRules, (int)notClockTimes);
            if (notClockAttendanceRule != null)
            {
                ComputeAbsentAndPersonalReason(attendanceItemComputDTOList, hoursOfAttendanceAverage, notClockAttendanceRule);
            }
            else if ((maxTime.MaxTime < notClockTimes && maxTime.MaxJudge == SymbolEnum.LessThanOrEqualTo) || (maxTime.MaxTime <= notClockTimes && maxTime.MaxJudge == SymbolEnum.LessThan))
            {
                var notClockAttendanceRuleMax = notClockAttendanceRules.LastOrDefault();
                ComputeAbsentAndPersonalReason(attendanceItemComputDTOList, hoursOfAttendanceAverage, notClockAttendanceRuleMax);
            }
        }
        /// <summary>
        /// 月实际出勤
        /// </summary>
        public void ComputeDurationOfAttendanceMonthly(List<AttendanceItemForComputDTO> attendanceItemComputDTOList)
        {

            var durationOfAttendanceMonthlyHours = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "实出勤时长" && a.Unit == "小时").FirstOrDefault();
            var durationOfAttendanceMonthlyDays = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "实出勤时长" && a.Unit == "天").FirstOrDefault();
            //var personalReasonHours = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "事假" && a.Unit == "小时").FirstOrDefault();
            //var personalReasonDays = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "事假" && a.Unit == "天").FirstOrDefault();
            var absentDays = attendanceItemComputDTOList.Where(a => a.AttendanceItemName == "旷工" && a.Unit == "天").FirstOrDefault();
            durationOfAttendanceMonthlyHours.AttendanceItemValue = durationOfAttendanceMonthlyHours.AttendanceItemValue - absent;
            durationOfAttendanceMonthlyDays.AttendanceItemValue = durationOfAttendanceMonthlyDays.AttendanceItemValue - absentDays.AttendanceItemValue;
        }
        /// <summary>
        /// 计算考勤项
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="attendanceRuleDetails"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="daysOfAttendance"></param>
        /// <param name="hoursOfAttendance"></param>
        public void Compute(AttendanceRecord attendanceRecord, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, IEnumerable<AttendanceRuleDetail> attendanceRuleDetails, ShiftManagement shiftManagement, int daysOfAttendance, decimal hoursOfAttendance, List<Leave> leaves)
        {
            var lateAttendanceRules = attendanceRuleDetails.Where(a => a.RuleType == RuleTypeEnum.Late).OrderBy(a => a.MinTime).ToList();
            var earlyLeaveAttendanceRules = attendanceRuleDetails.Where(a => a.RuleType == RuleTypeEnum.EarlyLeave).OrderBy(a => a.MinTime).ToList();
            var notClockAttendanceRules = attendanceRuleDetails.Where(a => a.RuleType == RuleTypeEnum.NotClock).OrderBy(a => a.MinTime).ToList();



            decimal earlyLeaveMinutes = 0;
            decimal LateMinutes = 0;



            durationOfAttendance = 0;
            foreach (var attendanceItem in attendanceItemForComputDTOs)
            {
                if (attendanceItem.AttendanceItemName == "迟到" && attendanceItem.Unit == "分钟数")
                {
                    if (!lateAttendanceRules.Any())
                    {
                        continue;
                    }
                    LateMinutes = ComputeLateMinute(attendanceRecord, attendanceItemForComputDTOs, lateAttendanceRules, shiftManagement, attendanceItem);
                }
                if (attendanceItem.AttendanceItemName == "迟到" && attendanceItem.Unit == "次数")
                {
                    if (!lateAttendanceRules.Any())
                    {
                        continue;
                    }
                    ComputeLateTimes(attendanceRecord, attendanceItem);
                }
                if (attendanceItem.AttendanceItemName == "早退" && attendanceItem.Unit == "分钟数")
                {
                    if (!earlyLeaveAttendanceRules.Any())
                    {
                        continue;
                    }
                    earlyLeaveMinutes = ComputeEarlyLeaveMinute(attendanceRecord, attendanceItemForComputDTOs, earlyLeaveAttendanceRules, shiftManagement, attendanceItem);
                }
                if (attendanceItem.AttendanceItemName == "早退" && attendanceItem.Unit == "次数")
                {
                    if (!earlyLeaveAttendanceRules.Any())
                    {
                        continue;
                    }
                    ComputeEarlyLeaveTimes(attendanceRecord, attendanceItem);
                }
                if (attendanceItem.AttendanceItemName.Contains("缺卡"))
                {
                    //if (!notClockAttendanceRules.Any())
                    //{
                    //    continue;
                    //}
                    ComputeNotClock(attendanceRecord, attendanceItemForComputDTOs, notClockAttendanceRules, shiftManagement, attendanceItem);
                }
                if (attendanceItem.AttendanceItemCatagoryName == AttendanceTypeEnum.Leave.GetDescription() || attendanceItem.AttendanceItemCatagoryName == AttendanceTypeEnum.Outside.GetDescription() || attendanceItem.AttendanceItemCatagoryName == AttendanceTypeEnum.Overtime.GetDescription())
                {
                    ComputeLeave(attendanceRecord, attendanceItem, attendanceItemForComputDTOs, shiftManagement);
                }
                if (attendanceItem.AttendanceItemName == "应出勤时长" && attendanceItem.Unit == "小时")
                {
                    attendanceItem.AttendanceItemValue = hoursOfAttendance;
                }
                if (attendanceItem.AttendanceItemName == "应出勤时长" && attendanceItem.Unit == "天")
                {
                    attendanceItem.AttendanceItemValue = daysOfAttendance;
                }
                if (attendanceItem.AttendanceItemName == "实出勤时长")
                {
                    ComputeDurationOfAttendance(attendanceRecord, attendanceItemForComputDTOs, attendanceItem, shiftManagement, earlyLeaveMinutes, LateMinutes);
                }
            }
        }
        /// <summary>
        /// 计算假期、出差外出
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItem"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="shiftManagement"></param>
        public void ComputeLeave(AttendanceRecord attendanceRecord, AttendanceItemForComputDTO attendanceItem, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, ShiftManagement shiftManagement)
        {
            if (string.IsNullOrEmpty(attendanceRecord.AttendanceItemDTOJson) || attendanceRecord.AttendanceItemDTOJson == "null")
            {
                attendanceItem.AttendanceItemValue += 0;
            }
            else
            {
                List<AttendanceItemForComputDTO> daliyComputDTO = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(attendanceRecord.AttendanceItemDTOJson);
                var personalLeave = daliyComputDTO.Where(s => s.AttendanceItemName == attendanceItem.AttendanceItemName).FirstOrDefault();
                if (personalLeave != null)
                {
                    if (attendanceItem.Unit == UnitEnum.Hour.GetDescription())
                    {
                        attendanceItem.AttendanceItemValue = personalLeave.AttendanceItemValue + attendanceItem.AttendanceItemValue;
                    }
                    if (attendanceItem.Unit == UnitEnum.day.GetDescription())
                    {
                        attendanceItem.AttendanceItemValue = personalLeave.AttendanceItemValue / shiftManagement.WorkHours + attendanceItem.AttendanceItemValue;
                    }

                }
                else
                {
                    attendanceItem.AttendanceItemValue += 0;
                }
            }

        }
        /// <summary>
        /// 计算实际出勤时间
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="attendanceItem"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="earlyLeaveMinutes"></param>
        /// <param name="LateMinutes"></param>
        public void ComputeDurationOfAttendance(AttendanceRecord attendanceRecord, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, AttendanceItemForComputDTO attendanceItem, ShiftManagement shiftManagement, decimal earlyLeaveMinutes, decimal LateMinutes)
        {
            if (attendanceItem.Unit == "小时")
            {
                List<AttendanceItemForComputDTO> daliyComputDto = null;
                decimal daliycomput = 0;
                // durationOfAttendance = shiftManagement.WorkHours - earlyLeaveMinutes / 60 - LateMinutes / 60;
                if (!string.IsNullOrEmpty(attendanceRecord.AttendanceItemDTOJson) && attendanceRecord.AttendanceItemDTOJson != "null")
                {
                    daliyComputDto = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(attendanceRecord.AttendanceItemDTOJson);
                    daliycomput = daliyComputDto.Where(s => s.AttendanceItemCatagoryName == AttendanceTypeEnum.Leave.GetDescription()).Sum(s => s.AttendanceItemValue);
                }
                durationOfAttendance = shiftManagement.WorkHours - daliycomput;
                attendanceItem.AttendanceItemValue = durationOfAttendance + attendanceItem.AttendanceItemValue;
            }
            if (attendanceItem.Unit == "天")
            {
                attendanceItem.AttendanceItemValue = durationOfAttendance / shiftManagement.WorkHours + attendanceItem.AttendanceItemValue;
            }
        }
        /// <summary>
        /// 计算早退
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="earlyLeaveAttendanceRules"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="attendanceItem"></param>
        public decimal ComputeEarlyLeaveMinute(AttendanceRecord attendanceRecord, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, List<AttendanceRuleDetail> earlyLeaveAttendanceRules, ShiftManagement shiftManagement, AttendanceItemForComputDTO attendanceItem)
        {
            decimal earlyLeaveMinutes = 0;
            //var earlyLeaveAttendanceRuleDetail = GetRuleDetail(earlyLeaveAttendanceRules, attendanceRecord.EarlyLeaveMinutes);
            //var maxTime = earlyLeaveAttendanceRules.LastOrDefault();
            ////早退在规则范围内
            //if (earlyLeaveAttendanceRuleDetail != null)
            //{
            //    if (earlyLeaveAttendanceRuleDetail.CallRuleType == RuleTypeEnum.EarlyLeave)
            //    {
            //        attendanceItem.AttendanceItemValue = earlyLeaveAttendanceRuleDetail.Time + attendanceItem.AttendanceItemValue;
            //        earlyLeaveMinutes = earlyLeaveAttendanceRuleDetail.Time;
            //    }

            //    if (earlyLeaveAttendanceRuleDetail.CallRuleType != RuleTypeEnum.EarlyLeave)
            //    {
            //        attendanceItem.AttendanceItemValue = attendanceRecord.EarlyLeaveMinutes + attendanceItem.AttendanceItemValue;
            //        //earlyLeaveMinutes = attendanceRecord.EarlyLeaveMinutes;
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, earlyLeaveAttendanceRuleDetail);
            //    }
            //}
            ////早退时间大于最大范围上限
            //else if ((maxTime.MaxTime < attendanceRecord.LateMinutes && maxTime.MaxJudge == SymbolEnum.LessThanOrEqualTo) || (maxTime.MaxTime <= attendanceRecord.LateMinutes && maxTime.MaxJudge == SymbolEnum.LessThan))
            //{
            //    if (maxTime.CallRuleType == RuleTypeEnum.EarlyLeave)
            //    {
            //        attendanceItem.AttendanceItemValue = maxTime.Time + attendanceItem.AttendanceItemValue;
            //        earlyLeaveMinutes = maxTime.Time;
            //    }

            //    if (maxTime.CallRuleType != RuleTypeEnum.EarlyLeave)
            //    {
            //        attendanceItem.AttendanceItemValue = attendanceRecord.EarlyLeaveMinutes + attendanceItem.AttendanceItemValue;
            //        //earlyLeaveMinutes = attendanceRecord.EarlyLeaveMinutes;
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, maxTime);
            //    }
            //}
            return earlyLeaveMinutes;

        }
        /// <summary>
        /// 计算早退
        /// </summary>

        public void ComputeEarlyLeaveTimes(AttendanceRecord attendanceRecord, AttendanceItemForComputDTO attendanceItem)
        {
            //早退计早退
            //attendanceItem.AttendanceItemValue = attendanceRecord.EarlyLeaveTimes + attendanceItem.AttendanceItemValue;
        }
        /// <summary>
        /// 计算迟到
        /// </summary>
        /// <returns></returns>
        public void ComputeLateTimes(AttendanceRecord attendanceRecord, AttendanceItemForComputDTO attendanceItem)
        {
            //attendanceItem.AttendanceItemValue = attendanceRecord.LateTimes + attendanceItem.AttendanceItemValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="lateAttendanceRules"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="attendanceItem"></param>
        /// <returns></returns>
        public decimal ComputeLateMinute(AttendanceRecord attendanceRecord, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, List<AttendanceRuleDetail> lateAttendanceRules, ShiftManagement shiftManagement, AttendanceItemForComputDTO attendanceItem)
        {
            decimal LateMinutes = 0;
            //var lateAttendanceRuleDetail = GetRuleDetail(lateAttendanceRules, attendanceRecord.LateMinutes);
            //var maxTime = lateAttendanceRules.LastOrDefault();
            //if (lateAttendanceRuleDetail != null)
            //{
            //    if (lateAttendanceRuleDetail.CallRuleType == RuleTypeEnum.Late)
            //    {
            //        attendanceItem.AttendanceItemValue = lateAttendanceRuleDetail.Time + attendanceItem.AttendanceItemValue;
            //        LateMinutes = lateAttendanceRuleDetail.Time;
            //    }
            //    if (lateAttendanceRuleDetail.CallRuleType != RuleTypeEnum.Late)
            //    {
            //        attendanceItem.AttendanceItemValue = attendanceRecord.LateMinutes + attendanceItem.AttendanceItemValue;
            //        //LateMinutes = attendanceRecord.LateMinutes;
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, lateAttendanceRuleDetail);
            //    }
            //}
            //if ((maxTime.MaxTime < attendanceRecord.LateMinutes && maxTime.MaxJudge == SymbolEnum.LessThanOrEqualTo) || (maxTime.MaxTime <= attendanceRecord.LateMinutes && maxTime.MaxJudge == SymbolEnum.LessThan))
            //{
            //    if (maxTime.CallRuleType == RuleTypeEnum.Late)
            //    {
            //        attendanceItem.AttendanceItemValue = maxTime.Time + attendanceItem.AttendanceItemValue;
            //        LateMinutes = maxTime.Time;
            //    }
            //    if (maxTime.CallRuleType != RuleTypeEnum.Late)
            //    {
            //        attendanceItem.AttendanceItemValue = attendanceRecord.LateMinutes + attendanceItem.AttendanceItemValue;
            //        //LateMinutes = attendanceRecord.LateMinutes;
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, maxTime);
            //    }
            //}
            return LateMinutes;
        }
        /// <summary>
        /// 计算未打卡
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="notClockAttendanceRules"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="attendanceItem"></param>
        /// <param name="absent"></param>
        /// <param name="personalReason"></param>
        public void ComputeNotClock(AttendanceRecord attendanceRecord, List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, List<AttendanceRuleDetail> notClockAttendanceRules, ShiftManagement shiftManagement, AttendanceItemForComputDTO attendanceItem)
        {
            //上班缺卡
            //var notClockAttendanceRuleDetail = GetRuleDetail(notClockAttendanceRules, attendanceRecord.NotClockInTimes);
            //if (notClockAttendanceRuleDetail != null)
            //{
            //缺卡计缺卡
            if (attendanceItem.AttendanceItemName == "上班缺卡" && attendanceItem.Unit == "次数")
            {
                //attendanceItem.AttendanceItemValue = attendanceRecord.NotClockInTimes + attendanceItem.AttendanceItemValue;
            }

            //    if (notClockAttendanceRuleDetail.CallRuleType != RuleTypeEnum.Late)
            //    {
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, notClockAttendanceRuleDetail);
            //    }
            //}
            //下班缺卡
            //notClockAttendanceRuleDetail = GetRuleDetail(notClockAttendanceRules, attendanceRecord.NotClockOutTimes);
            //if (notClockAttendanceRuleDetail != null)
            //{
            if (attendanceItem.AttendanceItemName == "下班缺卡" && attendanceItem.Unit == "次数")
            {
                //attendanceItem.AttendanceItemValue = attendanceRecord.NotClockOutTimes + attendanceItem.AttendanceItemValue;
            }
            //    if (notClockAttendanceRuleDetail.CallRuleType != RuleTypeEnum.Late)
            //    {
            //        ComputeAbsentAndPersonalReason(attendanceItemForComputDTOs, shiftManagement, notClockAttendanceRuleDetail);
            //    }
            //}

        }
        /// <summary>
        /// 计算旷工事假(缺卡)
        /// </summary>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="attendanceRuleDetail"></param>
        /// <param name="absent"></param>
        /// <param name="personalReason"></param>
        public void ComputeAbsentAndPersonalReason(List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, decimal hoursOfAttendanceAverage, AttendanceRuleDetail attendanceRuleDetail)
        {
            //旷工
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.Absent)
            {
                absent = attendanceRuleDetail.Time + absent;
                var absents = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "旷工").FirstOrDefault();
                absents.AttendanceItemValue = attendanceRuleDetail.Time / hoursOfAttendanceAverage + absents.AttendanceItemValue;

            }
            //事假
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.PersonalReason)
            {
                var personalReasonHour = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "小时").FirstOrDefault();

                personalReasonHour.AttendanceItemValue = attendanceRuleDetail.Time + personalReasonHour.AttendanceItemValue;
                //确保精度，保留一位小数在前端页面处理
                var personalReasonDay = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "天").FirstOrDefault();
                personalReasonDay.AttendanceItemValue = attendanceRuleDetail.Time / hoursOfAttendanceAverage + personalReasonDay.AttendanceItemValue;

            }
        }
        /// <summary>
        /// 计算旷工事假
        /// </summary>
        /// <param name="attendanceItemForComputDTOs"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="attendanceRuleDetail"></param>
        /// <param name="absent"></param>
        /// <param name="personalReason"></param>
        public void ComputeAbsentAndPersonalReason(List<AttendanceItemForComputDTO> attendanceItemForComputDTOs, ShiftManagement shiftManagement, AttendanceRuleDetail attendanceRuleDetail)
        {
            //旷工
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.Absent)
            {
                absent = attendanceRuleDetail.Time + absent;
                //if (absent <= shiftManagement.WorkHours)
                //{
                var absents = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "旷工").FirstOrDefault();
                absents.AttendanceItemValue = attendanceRuleDetail.Time / shiftManagement.WorkHours + absents.AttendanceItemValue;
                //}
                //else
                //{
                //    var absents = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "旷工").FirstOrDefault();

                //    absents.AttendanceItemValue = 1 + absents.AttendanceItemValue;
                //}

            }
            //事假
            if (attendanceRuleDetail.CallRuleType == RuleTypeEnum.PersonalReason)
            {
                //personalReason = attendanceRuleDetail.Time + personalReason;
                //if (personalReason <= shiftManagement.WorkHours)
                //{
                var personalReasonHour = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "小时").FirstOrDefault();

                personalReasonHour.AttendanceItemValue = attendanceRuleDetail.Time + personalReasonHour.AttendanceItemValue;
                //确保精度，保留一位小数在前端页面处理
                var personalReasonDay = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "天").FirstOrDefault();
                personalReasonDay.AttendanceItemValue = attendanceRuleDetail.Time / shiftManagement.WorkHours + personalReasonDay.AttendanceItemValue;
                //}
                //else
                //{
                //    var personalReasonHour = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "小时").FirstOrDefault();

                //    personalReasonHour.AttendanceItemValue = shiftManagement.WorkHours + personalReasonHour.AttendanceItemValue;
                //    //确保精度，保留一位小数在前端页面处理
                //    var personalReasonDay = attendanceItemForComputDTOs.Where(a => a.AttendanceItemName == "事假" && a.Unit == "天").FirstOrDefault();
                //    personalReasonDay.AttendanceItemValue = 1 + personalReasonDay.AttendanceItemValue;
                //}
            }
        }
        /// <summary>
        /// 计算应出勤时间
        /// </summary>
        /// <param name="attendanceTime"></param>
        /// <param name="weekDaysSettings"></param>
        /// <param name="holidays"></param>
        /// <param name="shiftManagement"></param>
        /// <returns></returns>
        public (int, decimal) ComputeDaysOfAttendance(DateTime attendanceTime, List<WeekDaysSetting> weekDaysSettings, List<Holiday> holidays, List<ShiftManagement> shiftManagements, bool isDynamicRowHugh, DateTime hiredate)
        {
            DateTime dateTime = Convert.ToDateTime(attendanceTime.ToString("yyyy-MM-01"));
            if (attendanceTime.Year == hiredate.Year && attendanceTime.Month == hiredate.Month)
            {
                dateTime = hiredate;
            }
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day - 1;
            int days = DateTime.DaysInMonth(year, month);
            int weekDays = 0;
            decimal weekHours = 0;
            for (int i = day; i < days; i++)
            {
                if (!IsHoliday(holidays, dateTime) || !isDynamicRowHugh)
                {
                    switch (dateTime.DayOfWeek)
                    {
                        case DayOfWeek.Friday:
                            (int, decimal) shouldWeek = IsWeekDay(DayOfWeek.Friday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Monday:
                            shouldWeek = IsWeekDay(DayOfWeek.Monday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Saturday:
                            shouldWeek = IsWeekDay(DayOfWeek.Saturday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Sunday:
                            shouldWeek = IsWeekDay(DayOfWeek.Sunday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Thursday:
                            shouldWeek = IsWeekDay(DayOfWeek.Thursday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Tuesday:
                            shouldWeek = IsWeekDay(DayOfWeek.Tuesday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                        case DayOfWeek.Wednesday:
                            shouldWeek = IsWeekDay(DayOfWeek.Wednesday, weekDays, weekHours, weekDaysSettings, dateTime, shiftManagements);
                            weekDays = shouldWeek.Item1;
                            weekHours = shouldWeek.Item2;
                            break;
                    }
                }
                dateTime = dateTime.AddDays(1);
            }
            return (weekDays, weekHours);
        }
        /// <summary>
        /// 应出勤天数累加
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <param name="weekDays"></param>
        /// <param name="weekDaysSettings"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public (int, decimal) IsWeekDay(DayOfWeek dayOfWeek, int weekDays, decimal weekHours, List<WeekDaysSetting> weekDaysSettings, DateTime dateTime, List<ShiftManagement> shiftManagements)
        {
            var weekdaySetting = weekDaysSettings.Where(t => t.Week == (int)dayOfWeek).FirstOrDefault();
            var shiftManagement = shiftManagements.Where(t => t.ShiftID == weekdaySetting.ShiftID).FirstOrDefault();
            if (shiftManagement == null)
            {
                return (weekDays, weekHours);
            }
            var isWorkPaidLeave = IsWorkPaidLeave(dateTime, _workPaidLeaves);//是否节假日调班
            if (isWorkPaidLeave || weekdaySetting.IsHolidayWork)
            {
                return (weekDays + 1, weekHours + shiftManagement.WorkHours);
            }
            else
            {
                return (weekDays, weekHours);
            }
        }
        /// <summary>
        /// 判断是否节假日调班
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="workPaidLeaves"></param>
        /// <returns></returns>
        public bool IsWorkPaidLeave(DateTime dateTime, IEnumerable<WorkPaidLeave> workPaidLeaves)
        {
            return workPaidLeaves.Where(a => a.PaidLeaveTime.ToString("yyyy-MM-dd") == dateTime.ToString("yyyy-MM-dd")).Any();
        }
        /// <summary>
        /// 获取节假日调班
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkPaidLeave> GetWorkPaidLeaves()
        {
            return _workPaidLeaveRepository.EntityQueryable<WorkPaidLeave>(a => a.PaidLeaveTime.Year == DateTime.Now.Year).ToList();
        }
        /// <summary>
        /// 获取节假日
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public List<Holiday> GetHolidays(int year)
        {
            return _holidayRepository.EntityQueryable<Holiday>(t => t.HolidayYear == year && !t.IsDelete).ToList();
        }
        /// <summary>
        /// 判断是否节假日
        /// </summary>
        /// <param name="holidays"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool IsHoliday(List<Holiday> holidays, DateTime dateTime)
        {
            // var holiday = holidays.Where(s => s.HolidayYear == dateTime.Year && s.StartHolidayTime <= dateTime && dateTime <= s.EndHolidayTime);
            return holidays.Where(s => s.HolidayYear == dateTime.Year && s.StartHolidayTime <= dateTime && dateTime <= s.EndHolidayTime && !s.IsDelete).Any();
        }
        /// <summary>
        /// 获取规则
        /// </summary>
        /// <param name="attendanceRuleDetails"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        public AttendanceRuleDetail GetRuleDetail(IEnumerable<AttendanceRuleDetail> attendanceRuleDetails, int itemValue)
        {

            return attendanceRuleDetails.Where(a => ((a.MinTime < itemValue && a.MinJudge == SymbolEnum.LessThan) ||
                                            (a.MinTime <= itemValue && a.MinJudge == SymbolEnum.LessThanOrEqualTo)) &&
                                            ((a.MaxTime > itemValue && a.MaxJudge == SymbolEnum.LessThan) ||
                                            (a.MaxTime >= itemValue && a.MaxJudge == SymbolEnum.LessThanOrEqualTo))).FirstOrDefault();
        }
        /// <summary>
        /// 获取考勤规则
        /// </summary>
        /// <param name="attendanceGroupID"></param>
        /// <returns></returns>
        public AttendanceRule GetAttendanceRule(string attendanceGroupID)
        {
            var attendanceRuleId = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(a => a.AttendanceGroupID == attendanceGroupID).Select(a => a.AttendanceRuleID).FirstOrDefault();
            if (string.IsNullOrEmpty(attendanceRuleId))
            {
                return null;
            }
            return _attendanceRuleRepository.EntityQueryable<AttendanceRule>(a => a.AttendanceRuleID == attendanceRuleId).FirstOrDefault();
        }



        /// <summary>
        /// 汇总表导出
        /// </summary>
        /// <returns></returns>
        public byte[] GetMonthlyRecord(int year, int month, List<string> idCards, FyuUser user)
        {

            //表格内容

            Expression<Func<AttendanceMonthlyRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (year != 0)
            {
                expression = expression.And(s => s.AttendanceDate.Year == year);
            }

            if (month != 0)
            {
                expression = expression.And(s => s.AttendanceDate.Month == month);
            }

            if (idCards.Any())
            {
                expression = expression.And(s => idCards.Contains(s.IDCard));
            }

            var record = _attendanceMonthlyRecordRepository.EntityQueryable(expression).OrderByDescending(s => s.EmployeeNo).ToList();
            if (record.Count <= 0)
            {
                return Encoding.Default.GetBytes("");
            }
            foreach (var item in record)
            {
                var attendanceProjecs = _serializer.Desrialize<string, List<AttendanceItemForComputDTO>>(item.AttendanceProjectsJson);
                if (attendanceProjecs.Count > 0)
                {
                    attendanceProjecs = attendanceProjecs.Where(a => a.UnitIsUsed).ToList();
                }
                item.AttendanceProjects = attendanceProjecs;
            }
            var title = new List<Diction>()
            {
                new Diction("Name", "姓名"),
                new Diction("EmployeeNo", "工号"),
                new Diction("Department", "部门"),
                new Diction("Position", "职位"),
            };
            var attendanceItemComputs = record.FirstOrDefault().AttendanceProjects;
            if (attendanceItemComputs == null)
            {
                return Encoding.Default.GetBytes("企业设置错误！");
            }
            foreach (var attendanceItemComput in attendanceItemComputs)
            {
                title.Add(new Diction(string.Empty, attendanceItemComput.AttendanceItemName + "(" + attendanceItemComput.Unit + ")"));
            }
            var record2 = _mapper.Map<List<AttendanceMonthlyRecord>>(record);
            return NPOIHelp.OutputMonthlyRecordExcel(new NPIOExtend<AttendanceMonthlyRecord>(record2, title, "考勤汇总"));
        }

        public List<AttendanceItemForComputDTO> GetItemComputDTO(FyuUser user)
        {
            return GetItemComputDTO(user.customerId);
        }

    }
}
