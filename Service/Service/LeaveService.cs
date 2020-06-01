using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class LeaveService : BaseService, ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        public LeaveService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer,ILeaveRepository leaveRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _leaveRepository = leaveRepository;
        }
        public PageResult<LeaveDTO> GetPageOut(int type, int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<Leave, bool>> exp = s => s.CompanyID == user.customerId;
            if (type==1)
            {
                exp = s => s.CompanyID == user.customerId && s.LeaveName == "外出";
            }
            if (type == 2)
            {
                exp = s => s.CompanyID == user.customerId && s.LeaveName == "出差";
            }
            if (type == 3)
            {
                exp = s => s.CompanyID == user.customerId && s.LeaveName != "外出"&&s.LeaveName!="出差";
            }
            var list = _leaveRepository.GetByPage(pageIndex, pageSize, exp, s => s.EndTime, SortOrderEnum.Descending);
            return PageMap<LeaveDTO, Leave>(list);
        }
    }
}
