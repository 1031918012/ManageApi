using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class HolidayService : IHolidayService
    {
        private readonly IMapper _mapper;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IWorkPaidLeaveRepository _workPaidLeaveRepository;
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;

        public HolidayService(IMapper mapper, IHolidayRepository holidayRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IAttendanceUnitOfWork salaryUnitOfWork)
        {
            _mapper = mapper;
            _holidayRepository = holidayRepository;
            _workPaidLeaveRepository = workPaidLeaveRepository;
            _salaryUnitOfWork = salaryUnitOfWork;
        }

        /// <summary>
        /// 根据年份查询节假日列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="year">节假日所属年份</param>
        /// <returns></returns>
        public PageResult<HolidayDTO> SelectHolidayList(int pageIndex, int pageSize, int year)
        {
            Expression<Func<Holiday, bool>> expression = s => !s.IsDelete;
            if (year != 0)
            {
                expression = expression.And(s => s.HolidayYear == year);
            }
            //查询
            var holiday = _holidayRepository.EntityQueryable(expression).OrderBy(s => s.StartHolidayTime);
            var holidayList = _holidayRepository.GetByPage(pageIndex, pageSize, holiday);
            //排序、映射
            var listDTO = _mapper.Map<List<HolidayDTO>>(holidayList.Data);
            listDTO.ForEach(s =>
            {
                var workPaidLeave = _workPaidLeaveRepository.GetEntityList(ss => ss.HolidayID == s.HolidayID, ss => ss.PaidLeaveTime, SortOrderEnum.Ascending).ToList();
                s.WorkPaidLeaveList = _mapper.Map<List<WorkPaidLeaveDTO>>(workPaidLeave);
            });
            return new PageResult<HolidayDTO>(holidayList.TotalNumber, holidayList.TotalPage, pageIndex, pageSize, listDTO);
        }

        /// <summary>
        /// 根据HolidayID获取节假日信息
        /// </summary>
        /// <param name="ShiftManagementID"></param>
        /// <returns></returns>
        public HolidayDTO GetHolidayByID(string HolidayID)
        {
            var entity = _holidayRepository.GetEntity(s => s.HolidayID == HolidayID && !s.IsDelete);
            var holiday = _mapper.Map<HolidayDTO>(entity);
            var workPaidLeave = _workPaidLeaveRepository.GetEntityList(s => s.HolidayID == HolidayID, s => s.PaidLeaveTime, SortOrderEnum.Ascending).ToList();
            holiday.WorkPaidLeaveList = _mapper.Map<List<WorkPaidLeaveDTO>>(workPaidLeave);
            return holiday;
        }

        #region 节假日的增、删、改
        /// <summary>
        /// 检查节假日是否重名
        /// </summary>
        /// <param name="name">班次名称</param>
        /// <returns></returns>
        public bool CheckSameName(string name, string id, int year)
        {
            var List = _holidayRepository.GetEntityList(s => !s.IsDelete && s.HolidayID != id && s.HolidayYear == year, s => s.StartHolidayTime, SortOrderEnum.Ascending).ToList();
            return !List.Where(s => s.HolidayName == name).Any();
        }

        /// <summary>
        /// 添加节假日
        /// </summary>
        /// <param name="addHolidayDTO"></param>
        /// <returns></returns>
        public (string, bool) AddHoliday(HolidayAddDTO addHolidayDTO)
        {
            Holiday holiday = new Holiday
            {
                CreateTime = DateTime.Now,
                EndHolidayTime = addHolidayDTO.EndHolidayTime.AddDays(1).AddSeconds(-1),
                HolidayID = Guid.NewGuid().ToString(),
                HolidayName = addHolidayDTO.HolidayName,
                HolidayNumber = addHolidayDTO.HolidayNumber,
                HolidayYear = addHolidayDTO.HolidayYear,
                IsDelete = false,
                StartHolidayTime = addHolidayDTO.StartHolidayTime
            };
            List<WorkPaidLeave> workPaidLeaveList = new List<WorkPaidLeave>();
            for (int i = 0; i < addHolidayDTO.WorkPaidLeaveList.Count; i++)
            {
                WorkPaidLeave workPaidLeave = new WorkPaidLeave
                {
                    HolidayID = holiday.HolidayID,
                    HolidayName = holiday.HolidayName,
                    PaidLeaveTime = addHolidayDTO.WorkPaidLeaveList[i].PaidLeaveTime,
                    Type = (TypeEnum)addHolidayDTO.WorkPaidLeaveList[i].Type,
                    WorkPaidLeaveID = Guid.NewGuid().ToString()
                };
                workPaidLeaveList.Add(workPaidLeave);
            }
            _salaryUnitOfWork.Add(holiday);
            if (workPaidLeaveList.Count != 0)
            {
                _salaryUnitOfWork.BatchInsert(workPaidLeaveList);
            }
            var res = _salaryUnitOfWork.Commit();
            if (res)
            {
                return ("新增成功", true);
            }
            return ("服务器内部错误", false);

        }

        /// <summary>
        /// 删除节假日
        /// </summary>
        /// <param name="holidayID"></param>
        /// <returns></returns>
        public bool UpdateHolidayEntity(string holidayID)
        {
            var holiday = _holidayRepository.GetEntity(s => s.HolidayID == holidayID && !s.IsDelete);
            if (holiday == null) { return false; }
            holiday.IsDelete = true;
            _salaryUnitOfWork.Update(holiday);
            var workPaidLeaveList = _workPaidLeaveRepository.GetEntityList(s => s.HolidayID == holidayID).ToList();
            if (workPaidLeaveList.Count != 0)
            {
                workPaidLeaveList.ForEach(s =>
                {
                    _salaryUnitOfWork.Delete(s);
                });
            }
            return _salaryUnitOfWork.Commit();
        }

        /// <summary>
        /// 修改节假日
        /// </summary>
        /// <param name="HolidayDTO"></param>
        /// <returns></returns>
        public bool UpdateHoliday(HolidayDTO HolidayDTO)
        {
            var holiday = _holidayRepository.GetEntity(s => s.HolidayID == HolidayDTO.HolidayID && !s.IsDelete);
            var Holiday = _mapper.Map<Holiday>(HolidayDTO);
            holiday.HolidayName = Holiday.HolidayName;
            holiday.HolidayYear = Holiday.HolidayYear;
            holiday.HolidayNumber = Holiday.HolidayNumber;
            holiday.StartHolidayTime = Holiday.StartHolidayTime;
            if (Holiday.EndHolidayTime != holiday.EndHolidayTime)
            {
                holiday.EndHolidayTime = Holiday.EndHolidayTime.AddDays(1).AddSeconds(-1);
            }
            _salaryUnitOfWork.Update(holiday);
            _salaryUnitOfWork.BatchDelete<WorkPaidLeave>(s => s.HolidayID == Holiday.HolidayID);
            List<WorkPaidLeave> workPaidLeaveList = new List<WorkPaidLeave>();
            for (int i = 0; i < HolidayDTO.WorkPaidLeaveList.Count; i++)
            {
                var WorkPaidLeave = _mapper.Map<WorkPaidLeave>(HolidayDTO.WorkPaidLeaveList[i]);
                WorkPaidLeave.WorkPaidLeaveID = Guid.NewGuid().ToString();
                WorkPaidLeave.HolidayName = Holiday.HolidayName;
                WorkPaidLeave.Type = TypeEnum.One;
                WorkPaidLeave.HolidayID = Holiday.HolidayID;
                WorkPaidLeave.HolidayName = Holiday.HolidayName;
                workPaidLeaveList.Add(WorkPaidLeave);
            }
            if (workPaidLeaveList.Count != 0)
            {
                _salaryUnitOfWork.BatchInsert(workPaidLeaveList);
            }
            return _salaryUnitOfWork.Commit();
        }
        #endregion
    }
}
