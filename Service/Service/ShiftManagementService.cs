using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class ShiftManagementService : IShiftManagementService
    {
        private readonly IMapper _mapper;
        private readonly IShiftManagementRepository _shiftManagementRepository;
        private readonly IWeekDaysSettingRepository _weekDaysSettingRepository;
        private readonly IAttendanceUnitOfWork _salaryUnitOfWork;
        private readonly ISerializer<string> _json;
        public ShiftManagementService(IMapper mapper, IShiftManagementRepository shiftManagementRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IAttendanceUnitOfWork salaryUnitOfWork, ISerializer<string> json)
        {
            _mapper = mapper;
            _shiftManagementRepository = shiftManagementRepository;
            _weekDaysSettingRepository = weekDaysSettingRepository;
            _salaryUnitOfWork = salaryUnitOfWork;
            _json = json;
        }
        /// <summary>
        /// 获取公司下所有班次
        /// </summary>
        /// <param name="fyuUser"></param>
        /// <returns></returns>
        public List<ShiftManagementDTO> GetCompanyShiftList(FyuUser fyuUser)
        {
            var shiftList = _shiftManagementRepository.EntityQueryable<ShiftManagement>(a => a.CompanyID == fyuUser.customerId && !a.IsDelete).ToList();
            var shiftDTOList = _mapper.Map<List<ShiftManagementDTO>>(shiftList);
            return shiftDTOList;
        }

        /// <summary>
        /// 查询班次列表
        /// </summary>
        /// <returns></returns>
        public PageResult<ShiftManagementDTO> SelectShiftList(int pageIndex, int pageSize, string companyID, string shiftname)
        {
            Expression<Func<ShiftManagement, bool>> expression = s => !s.IsDelete;
            if (!string.IsNullOrEmpty(companyID))
            {
                expression = expression.And(s => s.CompanyID == companyID);
            }
            if (!string.IsNullOrEmpty(shiftname))
            {
                expression = expression.And(s => s.ShiftName.Contains(shiftname));
            }
            //查询
            var shift = _shiftManagementRepository.EntityQueryable(expression).OrderByDescending(s => s.CreateTime);
            var shiftList = _shiftManagementRepository.GetByPage(pageIndex, pageSize, shift);
            //排序、映射
            var listDTO = _mapper.Map<List<ShiftManagementDTO>>(shiftList.Data);
            return new PageResult<ShiftManagementDTO>(shiftList.TotalNumber, shiftList.TotalPage, pageIndex, pageSize, listDTO);
        }

        /// <summary>
        /// 根据ShiftManagementID获取班次信息
        /// </summary>
        /// <param name="ShiftManagementID"></param>
        /// <returns></returns>
        public ShiftManagementDTO GetShiftManagementByID(string ShiftManagementID)
        {
            var shift = _shiftManagementRepository.GetEntity(s => s.ShiftID == ShiftManagementID && !s.IsDelete);
            return _mapper.Map<ShiftManagementDTO>(shift);
        }

        #region 班次的增、删、改
        /// <summary>
        /// 检查班次是否重名
        /// </summary>
        /// <param name="name">班次名称</param>
        /// <returns></returns>
        public bool CheckSameName(string name, string companyid, string id = "")
        {
            Expression<Func<ShiftManagement, bool>> expression = s => s.CompanyID == companyid && s.ShiftName == name && !s.IsDelete;
            if (!string.IsNullOrEmpty(id))
            {
                expression = s => s.CompanyID == companyid && s.ShiftName == name && !s.IsDelete && s.ShiftID != id;
            }
            return _shiftManagementRepository.EntityQueryable(expression).Select(s => s.ShiftName).Any();
        }

        /// <summary>
        /// 添加班次
        /// </summary>
        /// <param name="addShiftManagementDTO"></param>
        /// <returns></returns>
        public bool AddShift(ShiftManagementAddDTO addShiftManagementDTO, FyuUser user)
        {
            ShiftManagement addShiftManagement = new ShiftManagement
            {
                ShiftName = addShiftManagementDTO.ShiftName,
                AttendanceTime = addShiftManagementDTO.AttendanceTime,
                WorkHours = addShiftManagementDTO.WorkHours,
                ShiftRemark = addShiftManagementDTO.ShiftRemark,
                ClockRule = (ClockRuleEnum)addShiftManagementDTO.ClockRuleType,
                IsExemption = addShiftManagementDTO.IsExemption,
                LateMinutes = addShiftManagementDTO.LateMinutes,
                EarlyLeaveMinutes = addShiftManagementDTO.EarlyLeaveMinutes,
                IsFlexible = addShiftManagementDTO.IsFlexible,
                FlexibleMinutes = addShiftManagementDTO.FlexibleMinutes
            };
            addShiftManagement.ShiftID = Guid.NewGuid().ToString();
            addShiftManagement.CompanyID = user.customerId;
            addShiftManagement.CompanyName = user.customerName;
            addShiftManagement.CreateTime = DateTime.Now;
            addShiftManagement.Creator = user.realName;
            addShiftManagement.CreatorID = user.userId;
            addShiftManagement.IsDelete = false;

            List<ShiftTimeManagement> shiftTimeManagementList = new List<ShiftTimeManagement>();
            var shift = addShiftManagementDTO.ShiftTimeManagementList[0];
            var ShiftTime = new ShiftTimeManagement
            {
                DownEndClockTime = shift.DownEndClockTime,
                DownStartClockTime = shift.DownStartClockTime,
                EndRestTime = shift.EndRestTime,
                EndWorkTime = shift.EndWorkTime,
                ShiftTimeNumber = shift.ShiftTimeNumber,
                StartRestTime = shift.StartRestTime,
                StartWorkTime = shift.StartWorkTime,
                UpEndClockTime = shift.UpEndClockTime,
                UpStartClockTime = shift.UpStartClockTime,
            };
            //考勤时间的处理，分早晚打卡，分段打卡两种情况
            if (addShiftManagementDTO.ClockRuleType == 1)
            {
                addShiftManagement.ClockRule = ClockRuleEnum.SoonerOrLaterClock;
                if (ShiftTime.StartRestTime == null || ShiftTime.EndRestTime == null)
                {
                    //判断下班时间是否为次日
                    if (ShiftTime.EndWorkTime.CompareTo(ShiftTime.StartWorkTime) > 0)
                    {
                        addShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：---";
                    }
                    else
                    {
                        addShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-次日{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：---";
                    }
                }
                else
                {
                    DateTime datetime1 = (DateTime)ShiftTime.StartRestTime;
                    DateTime datetime2 = (DateTime)ShiftTime.EndRestTime;
                    //判断下班时间是否为次日
                    if (ShiftTime.EndWorkTime.CompareTo(ShiftTime.StartWorkTime) > 0)
                    {
                        addShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：{datetime1.ToString("HH:mm")}-{ datetime2.ToString("HH:mm") }";
                    }
                    else
                    {
                        addShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-次日{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：{datetime1.ToString("HH:mm")}-{ datetime2.ToString("HH:mm") }";
                    }
                }
            }
            if (addShiftManagementDTO.ClockRuleType == 2)
            {
                addShiftManagement.ClockRule = ClockRuleEnum.SegmentClock;
                addShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ShiftTime.EndWorkTime.ToString("HH:mm")},";
            }
            addShiftManagement.AttendanceTime = addShiftManagement.AttendanceTime.TrimEnd(',');
            ShiftTime.ShiftTimeID = Guid.NewGuid().ToString();
            ShiftTime.ShiftID = addShiftManagement.ShiftID;
            ShiftTime.ShiftName = addShiftManagement.ShiftName;
            ShiftTime.ShiftTimeNumber = 1;
            #region 将时间类型转化为0001-01-01 hh:mm:ss
            ShiftTime.StartWorkTime = new DateTime(0001, 01, 01, ShiftTime.StartWorkTime.Hour, ShiftTime.StartWorkTime.Minute, 59);
            ShiftTime.EndWorkTime = new DateTime(0001, 01, 01, ShiftTime.EndWorkTime.Hour, ShiftTime.EndWorkTime.Minute, 0);
            //判断下班时间是否大于上班时间，若大于，则下班时间为0001-01-01 hh:mm:ss；若小于，则下班时间为0001-01-02 hh:mm:ss
            if (DateTime.Compare(ShiftTime.StartWorkTime, ShiftTime.EndWorkTime) > 0)
            {
                ShiftTime.EndWorkTime = ShiftTime.EndWorkTime.AddDays(1);
            }
            if (ShiftTime.StartRestTime != null)
            {
                DateTime datetime = (DateTime)ShiftTime.StartRestTime;
                ShiftTime.StartRestTime = new DateTime(0001, 01, 01, datetime.Hour, datetime.Minute, datetime.Second);
            }
            if (ShiftTime.EndRestTime != null)
            {
                DateTime datetime = (DateTime)ShiftTime.EndRestTime;
                ShiftTime.EndRestTime = new DateTime(0001, 01, 01, datetime.Hour, datetime.Minute, datetime.Second);
            }
            ShiftTime.UpStartClockTime = new DateTime(0001, 01, 01, ShiftTime.UpStartClockTime.Hour, ShiftTime.UpStartClockTime.Minute, 0);
            ShiftTime.UpEndClockTime = new DateTime(0001, 01, 01, ShiftTime.UpEndClockTime.Hour, ShiftTime.UpEndClockTime.Minute, 59);
            //下班打卡开始时间为0001-01-01 hh:mm:ss
            ShiftTime.DownStartClockTime = new DateTime(0001, 01, 01, ShiftTime.DownStartClockTime.Hour, ShiftTime.DownStartClockTime.Minute, 0);
            //下班打卡结束时间为0001-01-01 hh:mm:ss
            ShiftTime.DownEndClockTime = new DateTime(0001, 01, 01, ShiftTime.DownEndClockTime.Hour, ShiftTime.DownEndClockTime.Minute, 59);
            //若下班打卡开始时间大于上班时间，下班打卡结束时间小于上班时间，则下班打卡结束时间为0001-01-02 hh:mm:ss
            if (DateTime.Compare(ShiftTime.DownStartClockTime, ShiftTime.StartWorkTime) > 0 && DateTime.Compare(ShiftTime.DownEndClockTime, ShiftTime.StartWorkTime) < 0)
            {
                ShiftTime.DownEndClockTime = ShiftTime.DownEndClockTime.AddDays(1);
            }
            //若下班打卡开始时间、结束时间都小于上班时间，则下班打卡开始、结束时间都为0001-01-02 hh:mm:ss
            if (DateTime.Compare(ShiftTime.DownStartClockTime, ShiftTime.StartWorkTime) < 0 && DateTime.Compare(ShiftTime.DownEndClockTime, ShiftTime.StartWorkTime) < 0)
            {
                ShiftTime.DownStartClockTime = ShiftTime.DownStartClockTime.AddDays(1);
                ShiftTime.DownEndClockTime = ShiftTime.DownEndClockTime.AddDays(1);
            }
            #endregion
            shiftTimeManagementList.Add(ShiftTime);
            addShiftManagement.ShiftTimeManagementList = shiftTimeManagementList;
            _salaryUnitOfWork.Add(addShiftManagement);
            return _salaryUnitOfWork.Commit();
        }

        /// <summary>
        /// 删除班次
        /// </summary>
        /// <param name="shiftManagementID"></param>
        /// <returns></returns>
        public JsonResponse UpdateShiftEntity(string shiftManagementID)
        {
            var shift = _shiftManagementRepository.GetEntity(s => s.ShiftID == shiftManagementID && !s.IsDelete);
            if (shift == null)
            {
                return new JsonResponse { IsSuccess = false, Message = "这个班次已经被删除，请刷新页面重试！", Code = CodeEnum.NotFound };
            }
            if (_weekDaysSettingRepository.EntityQueryable<WeekDaysSetting>(s => s.ShiftID == shiftManagementID).Any())
            {
                return new JsonResponse { IsSuccess = false, Message = "该班次已被考勤组使用，无法删除", Code = CodeEnum.NotFound };
            }
            shift.IsDelete = true;
            _salaryUnitOfWork.Update(shift);
            bool isSuccess = _salaryUnitOfWork.Commit();
            return new JsonResponse { IsSuccess = isSuccess, Message = isSuccess ? string.Format("删除成功") : string.Format("删除失败") };
        }

        /// <summary>
        /// 修改班次
        /// </summary>
        /// <param name="ShiftManagementDTO"></param>
        /// <returns></returns>
        public JsonResponse UpdateShift(ShiftManagementDTO ShiftManagementDTO)
        {
            var shift = _shiftManagementRepository.GetEntity(s => s.ShiftID == ShiftManagementDTO.ShiftID && !s.IsDelete);
            if (shift == null)
            {
                return new JsonResponse { IsSuccess = false, Message = "该班次已不存在,刷新页面重试", Code = CodeEnum.ok };
            }
            var ShiftManagement = _mapper.Map<ShiftManagement>(ShiftManagementDTO);
            shift.ShiftName = ShiftManagementDTO.ShiftName;
            shift.WorkHours = ShiftManagementDTO.WorkHours;
            shift.ShiftRemark = ShiftManagementDTO.ShiftRemark;
            shift.IsExemption = ShiftManagementDTO.IsExemption;
            shift.LateMinutes = ShiftManagementDTO.LateMinutes;
            shift.EarlyLeaveMinutes = ShiftManagementDTO.EarlyLeaveMinutes;
            shift.IsFlexible = ShiftManagementDTO.IsFlexible;
            shift.FlexibleMinutes = ShiftManagementDTO.FlexibleMinutes;
            shift.EarlyLeaveMinutes = ShiftManagementDTO.EarlyLeaveMinutes;
            _salaryUnitOfWork.BatchDelete<ShiftTimeManagement>(s => s.ShiftID == ShiftManagementDTO.ShiftID);
            List<ShiftTimeManagement> shiftTimeManagementList = new List<ShiftTimeManagement>();
            for (int i = 0; i < ShiftManagement.ShiftTimeManagementList.Count; i++)
            {
                var ShiftTime = ShiftManagement.ShiftTimeManagementList[i];
                //考勤时间的处理，分早晚打卡，分段打卡两种情况
                if (ShiftManagementDTO.ClockRuleType == 1)
                {
                    ShiftManagement.ClockRule = ClockRuleEnum.SoonerOrLaterClock;
                    if (ShiftTime.StartRestTime == null || ShiftTime.EndRestTime == null)
                    {
                        //判断下班时间是否为次日
                        if (ShiftTime.EndWorkTime.CompareTo(ShiftTime.StartWorkTime) > 0)
                        {
                            ShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：---";
                        }
                        else
                        {
                            ShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-次日{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：---";
                        }
                    }
                    else
                    {
                        DateTime datetime1 = (DateTime)ShiftTime.StartRestTime;
                        DateTime datetime2 = (DateTime)ShiftTime.EndRestTime;
                        //判断下班时间是否为次日
                        if (ShiftTime.EndWorkTime.CompareTo(ShiftTime.StartWorkTime) > 0)
                        {
                            ShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：{datetime1.ToString("HH:mm")}-{ datetime2.ToString("HH:mm") }";
                        }
                        else
                        {
                            ShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-次日{ ShiftTime.EndWorkTime.ToString("HH:mm") }，休息：{datetime1.ToString("HH:mm")}-{ datetime2.ToString("HH:mm") }";
                        }
                    }
                }
                if (ShiftManagementDTO.ClockRuleType == 2)
                {
                    ShiftManagement.ClockRule = ClockRuleEnum.SegmentClock;
                    ShiftManagement.AttendanceTime = $"上下班：{ShiftTime.StartWorkTime.ToString("HH:mm")}-{ShiftTime.EndWorkTime.ToString("HH:mm")},";
                }
                ShiftManagement.AttendanceTime = ShiftManagement.AttendanceTime.TrimEnd(',');
                ShiftTime.ShiftName = ShiftManagement.ShiftName;
                ShiftTime.ShiftTimeNumber = i + 1;
                #region 将时间类型转化为0001-01-01 hh:mm:ss
                ShiftTime.StartWorkTime = new DateTime(0001, 01, 01, ShiftTime.StartWorkTime.Hour, ShiftTime.StartWorkTime.Minute, 59);
                ShiftTime.EndWorkTime = new DateTime(0001, 01, 01, ShiftTime.EndWorkTime.Hour, ShiftTime.EndWorkTime.Minute, 0);
                //判断下班时间是否大于上班时间，若大于，则下班时间为0001-01-01 hh:mm:ss；若小于，则下班时间为0001-01-02 hh:mm:ss
                if (DateTime.Compare(ShiftTime.StartWorkTime, ShiftTime.EndWorkTime) > 0)
                {
                    ShiftTime.EndWorkTime = ShiftTime.EndWorkTime.AddDays(1);
                }
                if (ShiftTime.StartRestTime != null)
                {
                    DateTime datetime = (DateTime)ShiftTime.StartRestTime;
                    ShiftTime.StartRestTime = new DateTime(0001, 01, 01, datetime.Hour, datetime.Minute, datetime.Second);
                }
                if (ShiftTime.EndRestTime != null)
                {
                    DateTime datetime = (DateTime)ShiftTime.EndRestTime;
                    ShiftTime.EndRestTime = new DateTime(0001, 01, 01, datetime.Hour, datetime.Minute, datetime.Second);
                }
                ShiftTime.UpStartClockTime = new DateTime(0001, 01, 01, ShiftTime.UpStartClockTime.Hour, ShiftTime.UpStartClockTime.Minute, 0);
                ShiftTime.UpEndClockTime = new DateTime(0001, 01, 01, ShiftTime.UpEndClockTime.Hour, ShiftTime.UpEndClockTime.Minute, 59);
                //下班打卡开始时间为0001-01-01 hh:mm:ss
                ShiftTime.DownStartClockTime = new DateTime(0001, 01, 01, ShiftTime.DownStartClockTime.Hour, ShiftTime.DownStartClockTime.Minute, 0);
                //下班打卡结束时间为0001-01-01 hh:mm:ss
                ShiftTime.DownEndClockTime = new DateTime(0001, 01, 01, ShiftTime.DownEndClockTime.Hour, ShiftTime.DownEndClockTime.Minute, 59);
                //若下班打卡开始时间大于上班时间，下班打卡结束时间小于上班时间，则下班打卡结束时间为0001-01-02 hh:mm:ss
                if (DateTime.Compare(ShiftTime.DownStartClockTime, ShiftTime.StartWorkTime) > 0 && DateTime.Compare(ShiftTime.DownEndClockTime, ShiftTime.StartWorkTime) < 0)
                {
                    ShiftTime.DownEndClockTime = ShiftTime.DownEndClockTime.AddDays(1);
                }
                //若下班打卡开始时间、结束时间都小于上班时间，则下班打卡开始、结束时间都为0001-01-02 hh:mm:ss
                if (DateTime.Compare(ShiftTime.DownStartClockTime, ShiftTime.StartWorkTime) < 0 && DateTime.Compare(ShiftTime.DownEndClockTime, ShiftTime.StartWorkTime) < 0)
                {
                    ShiftTime.DownStartClockTime = ShiftTime.DownStartClockTime.AddDays(1);
                    ShiftTime.DownEndClockTime = ShiftTime.DownEndClockTime.AddDays(1);
                }
                #endregion
                shiftTimeManagementList.Add(ShiftTime);
            }
            shift.AttendanceTime = ShiftManagement.AttendanceTime;
            _salaryUnitOfWork.Update(shift);
            _salaryUnitOfWork.BatchInsert(shiftTimeManagementList);
            var success = _salaryUnitOfWork.Commit();
            if (!success)
            {
                return new JsonResponse { IsSuccess = false, Message = "班次修改失败", Code = CodeEnum.InternalServerError };
            }
            return new JsonResponse { IsSuccess = true, Message = "班次修改成功", Code = CodeEnum.ok };
        }
        #endregion


        /// <summary>
        /// 是否在打卡有效时间范围
        /// </summary>
        /// <param name="shiftID"></param>
        /// <returns></returns>
        public bool IsCanClock(string shiftID)
        {
            DateTime time = DateTime.Now.ConvertTime();
            ShiftManagement shift = _shiftManagementRepository.EntityQueryable<ShiftManagement>(s => !s.IsDelete && s.ShiftID == shiftID).FirstOrDefault();
            if (shift == null)
            {
                return false;
            }
            if (shift.ShiftTimeManagementList.Where(a => a.UpStartClockTime <= time && time <= a.DownEndClockTime).Any())
            {
                return true;
            }
            time = time.AddDays(1);
            if (shift.ShiftTimeManagementList.Where(a => a.UpStartClockTime <= time && time <= a.DownEndClockTime).Any())
            {
                return true;
            }
            return false;
        }
    }
}