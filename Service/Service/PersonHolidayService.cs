using AutoMapper;
using Domain;
using Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class PersonHolidayService : BaseService, IPersonHolidayService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IHolidayManagementRepository _holidayManagementRepository;
        private readonly IPersonHolidayRepository _personHolidayRepository;
        private readonly IPersonHolidayDetailRepository _personHolidayDetailRepository;
        public PersonHolidayService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IHolidayManagementRepository holidayManagementRepository, IPersonHolidayRepository personHolidayRepository, IPersonHolidayDetailRepository personHolidayDetailRepository, IPersonRepository personRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _holidayManagementRepository = holidayManagementRepository;
            _personHolidayRepository = personHolidayRepository;
            _personHolidayDetailRepository = personHolidayDetailRepository;
            _personRepository = personRepository;
        }
        /// <summary>
        /// 查询余额人员的分页列表页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<BalancePageDTO> GetBalancePage(int pageIndex, int pageSize, FyuUser user, string Name = "")
        {
            Expression<Func<Person, bool>> exp = s => s.IsSynchroHoliday && s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(Name))
            {
                exp = s => s.IsSynchroHoliday && s.CompanyID == user.customerId && s.Name.Contains(Name);
            }
            var person = _personRepository.EntityQueryable(exp);
            var personPage = _holidayManagementRepository.GetByPage(pageIndex, pageSize, person);
            return PageMap<BalancePageDTO, Person>(personPage);
        }

        /// <summary>
        /// 查询余额人员的分页列表页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<HolidayManagementDTO> GetHolidayList(FyuUser user)
        {
            return _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.EnableBalance && s.IsProhibit && s.CompanyID == user.customerId).Select(s => new HolidayManagementDTO { HolidayManagementID = s.HolidayManagementID, HolidayName = s.HolidayName }).ToList();
        }

        /// <summary>
        /// 同步人员
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string) SynchroHoliday(FyuUser user)
        {
            var personList = _personRepository.EntityQueryable<Person>(s => !s.IsSynchroHoliday && s.CompanyID == user.customerId).ToList();
            if (!personList.Any())
            {
                return (true, "同步成功");
            }
            _attendanceUnitOfWork.BatchUpdate<Person>(s => new Person() { IsSynchroHoliday = true }, s => !s.IsSynchroHoliday && s.CompanyID == user.customerId);
            var holiday = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => true).ToList();
            if (!holiday.Any())
            {
                return (true, "同步成功");
            }
            List<PersonHoliday> personHolidays = new List<PersonHoliday>();
            foreach (var personItem in personList)
            {
                foreach (var holidayItem in holiday)
                {
                    PersonHoliday personHoliday = new PersonHoliday
                    {
                        CompanyID = user.customerId,
                        HolidayManagementID = holidayItem.HolidayManagementID,
                        IDCard = personItem.IDCard,
                        SurplusAmount = 0,
                        TotalSettlement = 0,
                    };
                    personHolidays.Add(personHoliday);
                }
            }
            _attendanceUnitOfWork.BatchInsert(personHolidays);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "同步失败");
            }
            return (true, "同步成功");
        }
        public (bool, string) UpdateList(HolidayUpdateParamDTO holidayUpdate, FyuUser user)
        {
            if (!holidayUpdate.PersonAny())
            {
                return (false, "请选择人员");
            }
            if (holidayUpdate.HolidayManagementEmpty())
            {
                return (false, "请选择需要修改的假期");
            }
            if (holidayUpdate.DaysLessZero())
            {
                return (false, "请输入大于0的数");
            }
            if (holidayUpdate.DaysMoreMax())
            {
                return (false, "请输入小于180的数");
            }
            var personHoliday = _personHolidayRepository.EntityQueryable<PersonHoliday>(s => s.CompanyID == user.customerId && holidayUpdate.IDCards.Contains(s.IDCard) && s.HolidayManagementID.ToString() == holidayUpdate.HolidayManagementID).ToList();
            if (!personHoliday.Any())
            {
                return (false, "无可修改人员,请刷新重试");
            }
            ChangeTypeEnum number = holidayUpdate.Types > 0 ? ChangeTypeEnum.Add : ChangeTypeEnum.Sub;
            List<PersonHolidayDetail> holidayDetails = new List<PersonHolidayDetail>();
            foreach (var item in personHoliday)
            {
                item.SurplusAmount = item.SurplusAmount + (int)number * holidayUpdate.Days;
                if (item.SurplusAmount < 0)
                {
                    return (false, "减少余额过多,修改失败");
                }
                if (item.SurplusAmount > 180)
                {
                    return (false, "超出余额容量，修改失败");
                }
                PersonHolidayDetail holidayDetail = new PersonHolidayDetail
                {
                    ChangeTime = holidayUpdate.Days,
                    ChangeType = number,
                    CompanyID = user.customerId,
                    IDCard = item.IDCard,
                    CurrentQuota = item.SurplusAmount,
                    Remark = "手动" + number.GetDescription(),
                    AttendanceRecordID = string.Empty,
                    CreateTime = DateTime.Now,
                    HolidayManagementID = item.HolidayManagementID
                };
                holidayDetails.Add(holidayDetail);
                _attendanceUnitOfWork.Update(item);
            }
            if (holidayDetails.Any())
            {
                _attendanceUnitOfWork.BatchInsert(holidayDetails);
            }
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "修改失败");
            }
            return (true, "修改成功");
        }

        /// <summary>
        /// 查询余额的分页列表页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<PersonHolidayPageDTO> GetPersonHolidayPage(int pageIndex, int pageSize, FyuUser user, string IDCard = "", string holidayManagementID = "")
        {
            Expression<Func<PersonHoliday, bool>> exp = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(IDCard))
            {
                exp = s => s.CompanyID == user.customerId && s.IDCard == IDCard;
            }
            if (!string.IsNullOrEmpty(holidayManagementID))
            {
                exp = s => s.CompanyID == user.customerId && s.IDCard == IDCard && s.HolidayManagementID.ToString() == holidayManagementID;
            }
            var personHoliday = _personHolidayRepository.EntityQueryable(exp);
            var holidayMagagement = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.CompanyID == user.customerId && s.EnableBalance && s.IsProhibit);
            var query = from holiday in holidayMagagement
                        join person in personHoliday on holiday.HolidayManagementID equals person.HolidayManagementID
                        select new PersonHolidayPageDTO
                        {
                            HolidayManagementName = holiday.HolidayName,
                            SurplusAmount = person.SurplusAmount,
                            TotalSettlement = person.TotalSettlement,
                            CreateTime = holiday.CreateTime,
                        };
            int totalNumber = query.Count();
            List<PersonHolidayPageDTO> list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderByDescending(s => s.CreateTime).ToList();
            return new PageResult<PersonHolidayPageDTO>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, list);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IDCard"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<PersonHolidayDetailPageDTO> GetPersonHolidayDetailPage(string IDCard, int pageIndex, int pageSize, FyuUser user, string holidayManagementID = "")
        {
            Expression<Func<PersonHolidayDetail, bool>> exp = s => s.IDCard == IDCard && s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(holidayManagementID))
            {
                exp = s => s.CompanyID == user.customerId && s.IDCard == IDCard && s.HolidayManagementID.ToString() == holidayManagementID;
            }
            var personHolidayDetailQuery = _personHolidayDetailRepository.EntityQueryable(exp);
            var holidayMagagement = _holidayManagementRepository.EntityQueryable<HolidayManagement>(s => s.CompanyID == user.customerId && s.EnableBalance && s.IsProhibit);
            var query = from holiday in holidayMagagement
                        join detail in personHolidayDetailQuery on holiday.HolidayManagementID equals detail.HolidayManagementID
                        select new PersonHolidayDetailPageDTO
                        {
                            HolidayManagementName = holiday.HolidayName,
                            CreateTime = detail.CreateTime,
                            ChangeTime = detail.ChangeTime,
                            ChangeType = detail.ChangeType.GetDescription(),
                            CurrentQuota = detail.CurrentQuota,
                            Remark = detail.Remark,
                        };
            int totalNumber = query.Count();
            List<PersonHolidayDetailPageDTO> list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderByDescending(s => s.CreateTime).ToList();
            return new PageResult<PersonHolidayDetailPageDTO>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, list);
        }
    }
}
