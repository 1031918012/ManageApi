using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class OvertimeService : BaseService, IOvertimeService
    {
        private readonly IOvertimeRepository _overtimeRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        public OvertimeService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IOvertimeRepository overtimeRepository, IAttendanceGroupRepository attendanceGroupRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _overtimeRepository = overtimeRepository;
            _attendanceGroupRepository = attendanceGroupRepository;
        }

        public (bool, string) InsertOvertime(OvertimeDTO overtimeDTO, FyuUser user)
        {
            if (string.IsNullOrEmpty(overtimeDTO.OverTimeName))
            {
                return (false, "请输入名称");
            }
            if (_overtimeRepository.EntityQueryable<Overtime>(s => s.OvertimeName == overtimeDTO.OverTimeName&&!s.IsDelete).Any())
            {
                return (false, "加班规则名称重复");
            }
            Overtime overtime = new Overtime
            {
                OvertimeID = Guid.NewGuid(),
                CompanyID = user.customerId,
                CreateTime = DateTime.Now,
                ExcludingOvertime = overtimeDTO.ExcludingOvertime,
                HolidayCalculationMethod = (CalculationMethodEnum)overtimeDTO.HolidayCalculationMethod,
                HolidayCompensationMode = (CompensationModeEnum)overtimeDTO.HolidayCompensationMode,
                LongestOvertime = overtimeDTO.LongestOvertime,
                MinimumOvertime = overtimeDTO.MinimumOvertime,
                OvertimeName = overtimeDTO.OverTimeName,
                RestCalculationMethod = (CalculationMethodEnum)overtimeDTO.RestCalculationMethod,
                RestCompensationMode = (CompensationModeEnum)overtimeDTO.RestCompensationMode,
                WorkingCalculationMethod = (CalculationMethodEnum)overtimeDTO.WorkingCalculationMethod,
                WorkingCompensationMode = (CompensationModeEnum)overtimeDTO.WorkingCompensationMode,
            };
            _attendanceUnitOfWork.Add(overtime);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "添加失败");
            }
            return (true, "添加成功");
        }

        public PageResult<OvertimePageDTO> OvertimePage(int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<Overtime, bool>> exp = s => s.CompanyID == user.customerId&&!s.IsDelete;
            var list = _overtimeRepository.GetByPage(pageIndex, pageSize, exp, s => s.CreateTime, SortOrderEnum.Descending);
            return PageMap<OvertimePageDTO, Overtime>(list);
        }
        public (bool, string) UpdateOvertime(OvertimeDTO overtimeDTO, FyuUser user)
        {
            if (string.IsNullOrEmpty(overtimeDTO.OverTimeName))
            {
                return (false, "请输入名称");
            }
            var overtime = _overtimeRepository.EntityQueryable<Overtime>(s => s.OvertimeID == overtimeDTO.OverTimeID&&!s.IsDelete).FirstOrDefault();
            if (overtime == null)
            {
                return (false, "该条记录已被更改，刷新页面重试");
            }
            overtime.OvertimeID = overtimeDTO.OverTimeID;
            overtime.ExcludingOvertime = overtimeDTO.ExcludingOvertime;
            overtime.HolidayCalculationMethod = (CalculationMethodEnum)overtimeDTO.HolidayCalculationMethod;
            overtime.HolidayCompensationMode = (CompensationModeEnum)overtimeDTO.HolidayCompensationMode;
            overtime.LongestOvertime = overtimeDTO.LongestOvertime;
            overtime.MinimumOvertime = overtimeDTO.MinimumOvertime;
            overtime.OvertimeName = overtimeDTO.OverTimeName;
            overtime.RestCalculationMethod = (CalculationMethodEnum)overtimeDTO.RestCalculationMethod;
            overtime.RestCompensationMode = (CompensationModeEnum)overtimeDTO.RestCompensationMode;
            overtime.WorkingCalculationMethod = (CalculationMethodEnum)overtimeDTO.WorkingCalculationMethod;
            overtime.WorkingCompensationMode = (CompensationModeEnum)overtimeDTO.WorkingCompensationMode;
            _attendanceUnitOfWork.Update(overtime);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "修改失败");
            }
            return (true, "修改成功");
        }
        public (bool, string) DeleteOvertime(string overtimeID)
        {
            if (_attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.OvertimeID.ToString() == overtimeID, true).Any())
            {
                return (false, "该考勤规则已被考勤组被使用，无法删除");
            }
            var overtime = _overtimeRepository.EntityQueryable<Overtime>(s => s.OvertimeID.ToString() == overtimeID&&!s.IsDelete).FirstOrDefault();
            if (overtime == null)
            {
                return (false, "该对象已被删除,刷新重试");
            }
            if (!overtime.IsUsed)
            {
                _attendanceUnitOfWork.BatchDelete<Overtime>(s => s.OvertimeID.ToString() == overtimeID);
            }
            _attendanceUnitOfWork.BatchUpdate<Overtime>(s => new Overtime() { IsDelete = true }, s => s.OvertimeID.ToString() == overtimeID);
            var res = _attendanceUnitOfWork.Commit();
            if (!res)
            {
                return (false, "删除失败");
            }
            return (true, "删除成功");

        }

        public OvertimeDTO GetOvertime(string overtimeID)
        {
            var overtime = _overtimeRepository.EntityQueryable<Overtime>(s => s.OvertimeID.ToString() == overtimeID&&!s.IsDelete).FirstOrDefault();
            if (overtime == null)
            {
                return new OvertimeDTO();
            }
            return _mapper.Map<Overtime, OvertimeDTO>(overtime);
        }

        public List<OvertimeGroupDTO> OvertimeList(FyuUser user)
        {
            return _overtimeRepository.EntityQueryable<Overtime>(s => s.CompanyID == user.customerId&&!s.IsDelete).Select(s => new OvertimeGroupDTO { OverTimeID = s.OvertimeID, OverTimeName = s.OvertimeName }).ToList();
        }
    }
}
