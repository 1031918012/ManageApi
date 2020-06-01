using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class BreakoffService : BaseService, IBreakoffService
    {
        private readonly IPersonRepository _personRepository;
        private readonly IBreakoffRepository _breakoffRepository;
        private readonly IBreakoffDetailRepository _breakoffDetailRepository;
        public BreakoffService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IBreakoffRepository breakoffRepository, IBreakoffDetailRepository breakoffDetailRepository, IPersonRepository personRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _personRepository = personRepository;
            _breakoffRepository = breakoffRepository;
            _breakoffDetailRepository = breakoffDetailRepository;
        }

        public PageResult<BreakoffDetailPageDTO> GetBreakoffDetailPage(string IDCard, int pageIndex, int pageSize, FyuUser user)
        {
            var BreakoffDetailQuery = _breakoffDetailRepository.EntityQueryable<BreakoffDetail>(s => s.IDCard == IDCard && s.CompanyID == user.customerId).OrderByDescending(s => s.CreateTime);
            var breakoffDetailList = _breakoffDetailRepository.GetByPage(pageIndex, pageSize, BreakoffDetailQuery);
            return PageMap<BreakoffDetailPageDTO, BreakoffDetail>(breakoffDetailList);
        }

        public PageResult<BreakoffPageDTO> GetBreakoffPage(int pageIndex, int pageSize, string Name, FyuUser user)
        {
            Expression<Func<Person, bool>> exp = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(Name))
            {
                exp = s => s.Name.Contains(Name) && s.CompanyID == user.customerId;
            }
            var person = _personRepository.EntityQueryable(exp);
            var breakoff = _breakoffRepository.EntityQueryable<Breakoff>(s => s.CompanyID == user.customerId);
            var query = from p in person
                        join b in breakoff on p.IDCard equals b.IDCard into gc
                        from gci in gc.DefaultIfEmpty()
                        select new BreakoffPageDTO
                        {
                            IDCard = p.IDCard,
                            Department = p.Department,
                            Name = p.Name,
                            Position = p.Position,
                            SurplusAmount = gci.SurplusAmount,
                            TotalSettlement = gci.TotalSettlement,
                        };
            int totalNumber = query.Count();
            List<BreakoffPageDTO> list = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(s => s.IDCard).ToList();
            return new PageResult<BreakoffPageDTO>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, list);
        }
        public (bool, string) UpdateBreakoff(string IDCard, int type, decimal ChangeTime, FyuUser user)
        {
            ChangeTypeEnum number = type > 0 ? ChangeTypeEnum.Add : ChangeTypeEnum.Sub;

            var breakoff = _breakoffRepository.EntityQueryable<Breakoff>(s => s.IDCard == IDCard && s.CompanyID == user.customerId).FirstOrDefault();
            if (breakoff == null)
            {
                breakoff = new Breakoff
                {
                    CompanyID = user.customerId,
                    IDCard = IDCard,
                    SurplusAmount = 0,
                    TotalSettlement = 0
                };
            }
            breakoff.SurplusAmount = breakoff.SurplusAmount + (int)number * ChangeTime;
            if (breakoff.IsSurplusAmountZero())
            {
                return (false, "剩余额度不小于0天");
            }
            if (breakoff.IsSurplusAmountMax())
            {
                return (false, "剩余额度不超过180天");
            }
            BreakoffDetail breakoffDetail = new BreakoffDetail
            {
                ChangeTime = ChangeTime,
                ChangeType = number,
                CompanyID = user.customerId,
                IDCard = breakoff.IDCard,
                CurrentQuota = breakoff.SurplusAmount,
                Remark = "手动" + number.GetDescription(),
                AttendanceRecordID = string.Empty
            };
            _attendanceUnitOfWork.Add(breakoffDetail);
            if (breakoff.BreakoffID == 0)
            {
                _attendanceUnitOfWork.Add(breakoff);
            }
            else
            {
                _attendanceUnitOfWork.Update(breakoff);
            }
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "提交失败，服务器错误");
            }
            return (true, "修改成功");
        }
    }
}
