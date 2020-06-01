using AutoMapper;
using Domain;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class HolidayManagementService : BaseService, IHolidayManagementService
    {
        private readonly IHolidayManagementRepository _holidayManagementRepository;
        private readonly IPersonHolidayRepository _personHolidayRepository;
        private readonly IPersonHolidayDetailRepository _personHolidayDetailRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<HolidayManagementService> _logger;
        public HolidayManagementService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IHolidayManagementRepository holidayManagementRepository, IPersonHolidayRepository personHolidayRepository, IPersonHolidayDetailRepository personHolidayDetailRepository, IPersonRepository personRepository, ILogger<HolidayManagementService> logger) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _holidayManagementRepository = holidayManagementRepository;
            _personHolidayRepository = personHolidayRepository;
            _personHolidayDetailRepository = personHolidayDetailRepository;
            _personRepository = personRepository;
            _logger = logger;
        }
        /// <summary>
        /// 删除该条规则
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public (bool, string) Delete(Guid ID)
        {
            _attendanceUnitOfWork.BatchDelete<HolidayManagement>(s => s.HolidayManagementID == ID);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "删除失败");
            }
            return (true, "删除成功");
        }
        /// <summary>
        /// 启用或禁用
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public (bool, string) EnableOrProhibit(Guid ID, bool enable)
        {
            var item = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.HolidayManagementID == ID).FirstOrDefault();
            if (item == null)
            {
                return (false, "该对象已被删除，刷新页面重试");
            }
            item.IsProhibit = enable;
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, enable ? "启用失败" : "禁用失败");
            }
            return (true, enable ? "启用成功" : "禁用成功");
        }
        /// <summary>
        /// 机器规则分页
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public PageResult<HolidayManagementPageDTO> GetHolidayManagementPage(int pageIndex, int pageSize, FyuUser user)
        {
            var holidayManagementQuery = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.CompanyID == user.customerId).OrderByDescending(s => s.CreateTime);
            var holidayManagementList = _holidayManagementRepository.GetByPage(pageIndex, pageSize, holidayManagementQuery);
            return PageMap<HolidayManagementPageDTO, HolidayManagement>(holidayManagementList);
        }
        /// <summary>
        /// 新增假期规则
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public HolidayParamDTO GetHolidayManagement(Guid ID)
        {
            var holidayM = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.HolidayManagementID == ID).FirstOrDefault();
            if (holidayM == null)
            {
                return null;
            }
            return _mapper.Map<HolidayParamDTO>(holidayM);
        }

        private (bool, string) CheckData(HolidayParamDTO holiday, FyuUser user)
        {
            if (holiday.HolidayNameEmpty())
            {
                return (false, "请输入假期名称");
            }
            if (holiday.HolidayNameLength())
            {
                return (false, "假期名称长度过长");
            }
            foreach (HolidayManagementItemDTO item in holiday.WorkingYears)
            {
                if (item.MinEmpty())
                {
                    return (false, "工龄最小值无法小于零");
                }
                if (item.MaxEmpty())
                {
                    return (false, "工龄最大值无法小于零");
                }
                if (item.NumberEmpty())
                {
                    return (false, "工龄享有年限无法小于零");
                }
                if (item.MinMax())
                {
                    return (false, "工龄最小值输入过大");
                }
                if (item.MaxMax())
                {
                    return (false, "工龄最大值输入过大");
                }
                if (item.NumberMax())
                {
                    return (false, "工龄享有年限输入过大");
                }
            }
            foreach (HolidayManagementItemDTO item in holiday.Seniority)
            {
                if (item.MinEmpty())
                {
                    return (false, "司龄最小值无法小于零");
                }
                if (item.MaxEmpty())
                {
                    return (false, "司龄最大值无法小于零");
                }
                if (item.NumberEmpty())
                {
                    return (false, "司龄享有年限无法小于零");
                }
                if (item.MinMax())
                {
                    return (false, "司龄最小值输入过大");
                }
                if (item.MaxMax())
                {
                    return (false, "司龄最大值输入过大");
                }
                if (item.NumberMax())
                {
                    return (false, "司龄享有年限输入过大");
                }
            }
            return (true, "ok");
        }
        /// <summary>
        /// 新增假期规则
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public (bool, string) AddHolidayManagement(HolidayParamDTO holiday, FyuUser user)
        {
            var res = CheckData(holiday, user);
            if (!res.Item1)
            {
                return res;
            }
            if (_holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.CompanyID == user.customerId && s.HolidayName == holiday.HolidayName).Any())
            {
                return (false, "假期名称重复");
            }
            HolidayManagement newHoliday = new HolidayManagement
            {
                HolidayName = holiday.HolidayName,
                CompanyID = user.customerId,
                CreateTime = DateTime.Now,
                EnableBalance = holiday.EnableBalance,
                HolidayManagementID = Guid.NewGuid(),
                IsProhibit = false,
                DateOfIssue = DateOfIssueEnum.No,
                DistributionMethod = DistributionMethodEnum.No,
                FixedData = 0,
                IssuingCycle = IssuingCycleEnum.No,
                QuotaRule = QuotaRuleEnum.No,
                Seniority = JsonConvert.SerializeObject(string.Empty),
                ValidityOfLimit = 0,
                WorkingYears = JsonConvert.SerializeObject(string.Empty),
            };
            if (newHoliday.EnableBalance)
            {
                newHoliday.DistributionMethod = (DistributionMethodEnum)holiday.DistributionMethod;
            }
            if (newHoliday.DistributionMethod == DistributionMethodEnum.Auto)
            {
                newHoliday.DateOfIssue = (DateOfIssueEnum)holiday.DateOfIssue;
                newHoliday.IssuingCycle = (IssuingCycleEnum)holiday.IssuingCycle;
                newHoliday.QuotaRule = (QuotaRuleEnum)holiday.QuotaRule;
                newHoliday.ValidityOfLimit = holiday.ValidityOfLimit;
                if (newHoliday.ValidityOfLimit < 0)
                {
                    return (false, "额度有效期过小");
                }
            }
            if (newHoliday.QuotaRule == QuotaRuleEnum.Fixed)
            {
                newHoliday.FixedData = holiday.FixedData;
                if (newHoliday.FixedData < 0)
                {
                    return (false, "额度有效期过小");
                }
            }
            if (newHoliday.QuotaRule != QuotaRuleEnum.Fixed && newHoliday.QuotaRule != QuotaRuleEnum.No)
            {
                newHoliday.WorkingYears = JsonConvert.SerializeObject(holiday.WorkingYears);
                newHoliday.Seniority = JsonConvert.SerializeObject(holiday.Seniority);
            }
            var peson = _personRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId && s.IsSynchroHoliday).Select(s => s.IDCard).ToList();
            List<PersonHoliday> personHolidays = new List<PersonHoliday>();
            if (peson.Any())
            {
                foreach (var item in peson)
                {
                    PersonHoliday personHoliday = new PersonHoliday
                    {
                        CompanyID = newHoliday.CompanyID,
                        HolidayManagementID = newHoliday.HolidayManagementID,
                        IDCard = item,
                        SurplusAmount = 0,
                        TotalSettlement = 0,
                    };
                    personHolidays.Add(personHoliday);
                };
            }
            if (personHolidays.Any())
            {
                _attendanceUnitOfWork.BatchInsert(personHolidays);
            }
            return AddHolidayManagement(newHoliday, user);
        }
        private (bool, string) AddHolidayManagement(HolidayManagement holiday, FyuUser user)
        {
            _attendanceUnitOfWork.Add(holiday);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "添加失败");
            }
            return (true, "添加成功");
        }
        /// <summary>
        /// 新增假期规则
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public (bool, string) UpdateHolidayManagement(HolidayParamDTO holiday, FyuUser user)
        {
            var res = CheckData(holiday, user);
            if (!res.Item1)
            {
                return res;
            }
            HolidayManagement newHoliday = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.HolidayManagementID == holiday.HolidayManagementID).FirstOrDefault();
            if (newHoliday == null)
            {
                return (false, "该假期规则已被删除，刷新页面重试");
            }
            if (_holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.CompanyID == user.customerId && s.HolidayName == holiday.HolidayName && s.HolidayManagementID != holiday.HolidayManagementID).Any())
            {
                return (false, "假期名称重复");
            }
            newHoliday.HolidayName = holiday.HolidayName;
            newHoliday.EnableBalance = holiday.EnableBalance;
            newHoliday.DateOfIssue = DateOfIssueEnum.No;
            newHoliday.DistributionMethod = DistributionMethodEnum.No;
            newHoliday.FixedData = 0;
            newHoliday.IssuingCycle = IssuingCycleEnum.No;
            newHoliday.QuotaRule = QuotaRuleEnum.No;
            newHoliday.Seniority = JsonConvert.SerializeObject(string.Empty);
            newHoliday.ValidityOfLimit = 0;
            newHoliday.WorkingYears = JsonConvert.SerializeObject(string.Empty);

            if (holiday.EnableBalance)
            {
                newHoliday.DateOfIssue = (DateOfIssueEnum)holiday.DateOfIssue;
                newHoliday.DistributionMethod = (DistributionMethodEnum)holiday.DistributionMethod;
                newHoliday.IssuingCycle = (IssuingCycleEnum)holiday.IssuingCycle;
                newHoliday.QuotaRule = (QuotaRuleEnum)holiday.QuotaRule;
                newHoliday.ValidityOfLimit = holiday.ValidityOfLimit;
                if (newHoliday.ValidityOfLimit < 0)
                {
                    return (false, "额度有效期过小");
                }
            }
            if (holiday.QuotaRule == 1)
            {
                newHoliday.FixedData = holiday.FixedData;
                if (newHoliday.FixedData < 0)
                {
                    return (false, "额度有效期过小");
                }
            }
            if (holiday.QuotaRule != 1 && holiday.QuotaRule != 0)
            {
                newHoliday.WorkingYears = JsonConvert.SerializeObject(holiday.WorkingYears);
                newHoliday.Seniority = JsonConvert.SerializeObject(holiday.Seniority);
            }
            return UpdateHolidayManagement(newHoliday, user);
        }
        private (bool, string) UpdateHolidayManagement(HolidayManagement holiday, FyuUser user)
        {
            _attendanceUnitOfWork.Update(holiday);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "修改失败");
            }
            return (true, "修改成功");
        }

        public (bool, string) AutoHoliday(DateTime time)
        {
            _logger.LogError("自动发假开始");
            var holidayManagement = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.EnableBalance && s.IsProhibit && s.DistributionMethod == DistributionMethodEnum.Auto, true).GroupBy(s => s.CompanyID);
            foreach (var item in holidayManagement)
            {
                List<HolidayManagement> holidatOneCompany = item.ToList();
                try
                {
                    var res = AutoHoliday(time, holidatOneCompany);
                    _logger.LogError(holidatOneCompany.FirstOrDefault()?.CompanyID + res.Item1 + res.Item2);
                }
                catch (Exception ex)
                {
                    _logger.LogError(holidatOneCompany.FirstOrDefault()?.CompanyID + ex.ToString());
                    continue;
                }
            }
            return (true, "获取完成");
        }
        public (bool, string) AutoHoliday(DateTime time, List<HolidayManagement> holidatOneCompany)
        {
            var Person = _personRepository.EntityQueryable<Person>(s => s.IsSynchroHoliday && s.CompanyID == holidatOneCompany.FirstOrDefault().CompanyID).ToList();
            if (!Person.Any())
            {
                return (false, "没查到同步过的人");
            }
            var personHoliDayAll = _personHolidayRepository.EntityQueryable<PersonHoliday>(s => s.CompanyID == holidatOneCompany.FirstOrDefault().CompanyID).ToList();
            if (!personHoliDayAll.Any())
            {
                return (false, "没查到同步后添加的假期余额");
            }
            var holidayManagementMonth = holidatOneCompany.Where(s => s.IssuingCycle == IssuingCycleEnum.Month).ToList();
            if (holidayManagementMonth.Any())
            {
                foreach (HolidayManagement holiday in holidayManagementMonth)
                {
                    //第一步，清
                    List<PersonHoliday> personHoliDay = personHoliDayAll.Where(s => s.HolidayManagementID == holiday.HolidayManagementID).ToList();
                    ClearHolidayData(personHoliDay);
                    //第二步，发
                    AutoGrantData(personHoliDay, holiday, Person, time);
                }
            }
            var holidayManagementYear = holidatOneCompany.Where(s => s.IssuingCycle == IssuingCycleEnum.Year).ToList();
            if (holidayManagementYear.Any())
            {
                foreach (HolidayManagement holiday in holidayManagementYear)
                {
                    //第一步，清
                    List<PersonHoliday> personHoliDay = personHoliDayAll.Where(s => s.HolidayManagementID == holiday.HolidayManagementID).ToList();
                    if (holiday.ValidityOfLimit == time.Month - 1)
                    {
                        ClearHolidayData(personHoliDay);
                    }
                    //第二步，发
                    if (time.Month == 1)
                    {
                        if (holiday.ValidityOfLimit == 12)
                        {
                            ClearHolidayData(personHoliDay);
                        }
                        AutoGrantData(personHoliDay, holiday, Person, time);
                    }
                }
            }
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (res, "提交失败");
            }
            return (res, "提交成功");
        }

        public void ClearHolidayData(List<PersonHoliday> personHolidays)
        {
            List<PersonHolidayDetail> personHolidayDetails = new List<PersonHolidayDetail>();
            personHolidays.ForEach(s =>
            {
                PersonHolidayDetail personHolidayDetail = new PersonHolidayDetail
                {
                    AttendanceRecordID = string.Empty,
                    ChangeTime = s.SurplusAmount,
                    ChangeType = ChangeTypeEnum.Sub,
                    CompanyID = s.CompanyID,
                    CreateTime = DateTime.Now,
                    CurrentQuota = 0,
                    HolidayManagementID = s.HolidayManagementID,
                    IDCard = s.IDCard,
                    Remark = "系统自动减少"
                };
                personHolidayDetails.Add(personHolidayDetail);
                s.SurplusAmount = 0;
            });
            _attendanceUnitOfWork.BatchInsert(personHolidayDetails);
        }
        public void AutoGrantData(List<PersonHoliday> personHolidays, HolidayManagement holiday, List<Person> people, DateTime time)
        {
            List<PersonHolidayDetail> personHolidayDetails = new List<PersonHolidayDetail>();
            foreach (var s in personHolidays)
            {
                Person person = people.Where(t => t.IDCard == s.IDCard).DefaultIfEmpty().FirstOrDefault();
                if (person == null)
                {
                    continue;
                }
                decimal? dataReturn = FindGrantData(holiday, person, time);
                if (dataReturn == null)
                {
                    continue;
                }
                decimal data = (decimal)dataReturn;
                PersonHolidayDetail personHolidayDetail = new PersonHolidayDetail
                {
                    AttendanceRecordID = string.Empty,
                    ChangeTime = data,
                    ChangeType = ChangeTypeEnum.Add,
                    CompanyID = s.CompanyID,
                    CreateTime = DateTime.Now,
                    CurrentQuota = data,
                    HolidayManagementID = s.HolidayManagementID,
                    IDCard = s.IDCard,
                    Remark = "系统自动增加"
                };
                personHolidayDetails.Add(personHolidayDetail);
                s.SurplusAmount = data;
            }
            if (personHolidayDetails.Any())
            {
                _attendanceUnitOfWork.BatchInsert(personHolidayDetails);
            }
            _attendanceUnitOfWork.Update(personHolidays);
            return;
        }

        public decimal? FindGrantData(HolidayManagement holiday, Person person, DateTime time)
        {
            if (holiday.QuotaRule == QuotaRuleEnum.Fixed)
            {
                return holiday.FixedData;
            }
            decimal a = time.Month - person.StartJobTime.Month;
            decimal a1 = ((int)(a * 100))/12/(decimal)100;
            decimal workYears = time.Year - person.StartJobTime.Year + a1;
            List<HolidayManagementItemDTO> workYearsItems = JsonConvert.DeserializeObject<List<HolidayManagementItemDTO>>(holiday.WorkingYears);
            decimal? dataW = workYearsItems.Where(t => t.Min < workYears && workYears <= t.Max).FirstOrDefault()?.Number;
            if (holiday.QuotaRule == QuotaRuleEnum.WorkingYears)
            {
                return dataW;
            }
            decimal b = time.Month - person.Hiredate.Month;
            decimal b1 = ((int)(b * 100))/12 / (decimal)100;
            var seniority = time.Year - person.Hiredate.Year + b1;
            List<HolidayManagementItemDTO> seniorityItems = JsonConvert.DeserializeObject<List<HolidayManagementItemDTO>>(holiday.Seniority);
            var dataS = seniorityItems.Where(t => t.Min < seniority && seniority <= t.Max).FirstOrDefault()?.Number;
            if (holiday.QuotaRule == QuotaRuleEnum.Seniority)
            {
                return dataS;
            }
            if (holiday.QuotaRule == QuotaRuleEnum.WorkingYearsAndSeniority)
            {
                return dataW + dataS;
            }
            if (holiday.QuotaRule == QuotaRuleEnum.SelectMax)
            {
                return dataW > dataS ? dataW : dataS;
            }
            _logger.LogError("自动发假不可能出现的问题--------" + JsonConvert.SerializeObject(holiday) + JsonConvert.SerializeObject(person));
            return null;
        }
    }
}
