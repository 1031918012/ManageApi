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
    public class AttendanceGroupService : BaseService, IAttendanceGroupService
    {
        private readonly IWeekDaysSettingRepository _weekDaysSettingRepository;
        private readonly IAttendanceRuleRepository _attendanceRuleRepository;
        private readonly IGroupPersonnelRepository _groupPersonnelRepository;
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        private readonly IGroupAddressRepository _groupAddressRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IClockInAddressRepository _clockInAddressRepository;
        ///
        public AttendanceGroupService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, IAttendanceGroupRepository attendanceGroupRepository, IGroupPersonnelRepository groupPersonnelRepository, IAttendanceRuleRepository attendanceRuleRepository, ISerializer<string> serializer, IWeekDaysSettingRepository weekDaysSettingRepository, IPersonRepository personRepository, IGroupAddressRepository groupAddressRepository, IClockInAddressRepository clockInAddressRepository) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _attendanceGroupRepository = attendanceGroupRepository;
            _groupPersonnelRepository = groupPersonnelRepository;
            _attendanceRuleRepository = attendanceRuleRepository;
            _weekDaysSettingRepository = weekDaysSettingRepository;
            _personRepository = personRepository;
            _groupAddressRepository = groupAddressRepository;
            _clockInAddressRepository = clockInAddressRepository;
        }
        /// <summary>
        /// 获取考勤组基本信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ruleName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<AttendanceGroupDTO> GetAttendanceGrouPage(int pageIndex, int pageSize, string groupName, FyuUser user)
        {
            var persons = _personRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId).ToList();
            Expression<Func<AttendanceGroup, bool>> exp = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(groupName))
            {
                exp = s => s.Name.Contains(groupName) && s.CompanyID == user.customerId;
            }
            var Group = _attendanceGroupRepository.GetByPage(pageIndex, pageSize, exp);
            foreach (var attendanceGroup in Group.Data)
            {
                var idCards = attendanceGroup.GroupPersonnels.Select(s => s.IDCard).Take(4);
                var personNames = persons.Where(s => idCards.Contains(s.IDCard)).Select(s => s.Name).ToList();
                if (personNames.Count > 0)
                {
                    attendanceGroup.Range = string.Join('、', personNames) + "等";
                }
                else
                {
                    attendanceGroup.Range = "";
                }

            }
            return PageMap<AttendanceGroupDTO, AttendanceGroup>(Group);
        }
        /// <summary>
        /// 删除考勤组
        /// </summary>
        /// <param name="attendanceRuleID"></param>
        /// <returns></returns>
        public JsonResponse DeleteAttendanceRule(string attendanceGroupID)
        {
            _attendanceUnitOfWork.BatchDelete<AttendanceGroup>(s => s.AttendanceGroupID == attendanceGroupID);
            _attendanceUnitOfWork.BatchDelete<GroupPersonnel>(s => s.AttendanceGroupID == attendanceGroupID);
            _attendanceUnitOfWork.BatchDelete<WeekDaysSetting>(s => s.AttendanceGroupID == attendanceGroupID);
            _attendanceUnitOfWork.BatchDelete<GroupAddress>(s => s.AttendanceGroupID == attendanceGroupID);
            if (!_attendanceUnitOfWork.Commit())
            {
                return new JsonResponse { Code = CodeEnum.InternalServerError, IsSuccess = false, Message = "服务器错误，删除失败" };
            }
            return new JsonResponse { Code = CodeEnum.ok, IsSuccess = true, Message = "删除成功" };
        }
        /// <summary>
        /// 构建考勤组对象
        /// </summary>
        /// <param name="attendanceGroupDTO"></param>
        /// <param name="fyuUser"></param>
        /// <returns></returns>
        private AttendanceGroup AddGourp(AttendanceGroupAddDTO attendanceGroupDTO, FyuUser fyuUser)
        {
            //新增考勤组
            var attendanceGroup = new AttendanceGroup
            {
                AttendanceGroupID = Guid.NewGuid().ToString(),
                AttendanceRuleID = attendanceGroupDTO.AttendanceRuleID,
                ClockInWay = ClockInWayEnum.Location,
                CompanyID = fyuUser.customerId,
                CompanyName = fyuUser.customerName,
                CreateTime = DateTime.Now,
                Creator = fyuUser.realName,
                CreatorID = fyuUser.userId,
                IsDynamicRowHugh = attendanceGroupDTO.IsDynamicRowHugh,
                Name = attendanceGroupDTO.Name,
                OvertimeID = attendanceGroupDTO.OvertimeID,
                Range = string.Empty,
                ShiftType = (ShiftTypeEnum)attendanceGroupDTO.ShiftType,
            };
            return attendanceGroup;
        }
        private List<WeekDaysSetting> AddWeekDaysSetting(List<ShiftInfoDTO> ShiftInfo, FyuUser fyuUser, AttendanceGroup attendanceGroup)
        {
            List<WeekDaysSetting> weekDaysSettings = new List<WeekDaysSetting>();

            for (int i = 0; i < ShiftInfo.Count; i++)
            {
                WeekDaysSetting weekDaysSetting = new WeekDaysSetting
                {
                    AttendanceGroupID = attendanceGroup.AttendanceGroupID,
                    ShiftID = ShiftInfo[i].ShiftId,
                    Week = i + 1,
                    WeekDaysSettingID = Guid.NewGuid().ToString(),
                    IsHolidayWork = ShiftInfo[i].IsHolidayWork
                };
                if (weekDaysSetting.Week == 7)
                {
                    weekDaysSetting.Week = 0;
                }
                weekDaysSettings.Add(weekDaysSetting);
            }
            return weekDaysSettings;
        }
        /// <summary>
        /// 添加考勤组地址
        /// </summary>
        /// <param name="attendanceGroupDTO"></param>
        /// <param name="fyuUser"></param>
        /// <param name="attendanceGroup"></param>
        /// <returns></returns>
        private List<GroupAddress> AddGroupAddress(List<string> site, FyuUser fyuUser, AttendanceGroup attendanceGroup)
        {
            List<GroupAddress> addressList = new List<GroupAddress>();
            site.ForEach(s =>
            {
                var address = new GroupAddress
                {
                    AttendanceGroupID = attendanceGroup.AttendanceGroupID,
                    ClockInAddressID = s,
                };
                addressList.Add(address);
            });
            return addressList;
        }
        /// <summary>
        /// 新增考勤组与人员关系数据
        /// </summary>
        /// <param name="attendanceGroupDTO"></param>
        /// <param name="fyuUser"></param>
        /// <param name="attendanceGroup"></param>
        /// <returns></returns>
        public (List<GroupPersonnel>, JsonResponse, List<string>) AddGroupPersonnel(List<string> IdCardList, FyuUser fyuUser, AttendanceGroup attendanceGroup)
        {
            List<GroupPersonnel> groupPersonnels = new List<GroupPersonnel>();
            var persons = _personRepository.EntityQueryable<Person>(t => IdCardList.Contains(t.IDCard) && t.CompanyID == fyuUser.customerId).ToList();
            if (IdCardList.Count <= 0)
            {
                var response = new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "添加失败，请选择考勤人员！" };
                return (null, null, null);
            }
            else
            {
                var groupPersonnelList = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(a => a.CompanyID == fyuUser.customerId && a.AttendanceGroupID != attendanceGroup.AttendanceGroupID, true).Select(a => a.IDCard).ToList();
                //空引用判断
                bool flag = true;
                if (groupPersonnelList == null)
                {
                    flag = false;
                }
                foreach (var person in persons)
                {

                    if (flag && groupPersonnelList.Contains(person.IDCard))
                    {
                        var response = new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "添加失败，" + person.Name + "存在其他考勤组里！" };
                        return (null, response, null);
                    }
                    else
                    {
                        GroupPersonnel groupPersonnel = new GroupPersonnel()
                        {
                            GroupPersonnelID = Guid.NewGuid().ToString(),
                            AttendanceGroupID = attendanceGroup.AttendanceGroupID,
                            CompanyID = fyuUser.customerId,
                            IDCard = person.IDCard
                        };
                        groupPersonnels.Add(groupPersonnel);
                    }
                }
            }
            return (groupPersonnels, null, persons.Select(t => t.Name).ToList());
        }

        public JsonResponse AddAttendanceGroup(AttendanceGroupAddDTO attendanceGroupDTO, FyuUser fyuUser)
        {
            if (attendanceGroupDTO.ShiftType == 1)
            {
                return AddAttendanceGroup1(attendanceGroupDTO, fyuUser);
            }
            if (attendanceGroupDTO.ShiftType == 2)
            {
                return AddAttendanceGroup2(attendanceGroupDTO, fyuUser);
            }
            return new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "班次类型无效" };
        }
        private JsonResponse AddAttendanceGroup2(AttendanceGroupAddDTO attendanceGroupDTO, FyuUser fyuUser)
        {
            if (!attendanceGroupDTO.IdCardList.Any())
            {
                return new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "请选择人员" };
            }
            var attendanceGroup = AddGourp(attendanceGroupDTO, fyuUser);
            var groupPersonnels = AddGroupPersonnel(attendanceGroupDTO.IdCardList, fyuUser, attendanceGroup);
            if (groupPersonnels.Item2 != null)
            {
                return groupPersonnels.Item2;
            }
            //列表展示
            attendanceGroup.Range = string.Join('、', groupPersonnels.Item3.Take(4)) + "等";
            //提交数据库
            if (groupPersonnels.Item1.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(groupPersonnels.Item1);
            }
            _attendanceUnitOfWork.Add(attendanceGroup);
            var isSuccess = _attendanceUnitOfWork.Commit();
            if (!isSuccess)
            {
                return new JsonResponse() { Code = CodeEnum.Created, IsSuccess = false, Message = "添加失败！" };
            }
            return new JsonResponse() { Code = CodeEnum.Created, IsSuccess = true, Message = "添加成功！" };
        }
        /// <summary>
        /// 新增考情组
        /// </summary>
        /// <param name="attendanceGroupDTO"></param>
        /// <returns></returns>
        private JsonResponse AddAttendanceGroup1(AttendanceGroupAddDTO attendanceGroupDTO, FyuUser fyuUser)
        {
            if (!attendanceGroupDTO.Site.Any())
            {
                return new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "添加失败，请选择考勤地点！" };
            }
            var attendanceGroup = AddGourp(attendanceGroupDTO, fyuUser);

            var weekDaysSettings = AddWeekDaysSetting(attendanceGroupDTO.ShiftInfo, fyuUser, attendanceGroup);
            if (weekDaysSettings.Count <= 0)
            {
                return new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "添加失败，考勤组异常请重新设置考勤组！" };
            }
            if (weekDaysSettings.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(weekDaysSettings);
            }

            var groupAddress = AddGroupAddress(attendanceGroupDTO.Site, fyuUser, attendanceGroup);
            if (groupAddress.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(groupAddress);
            }

            var groupPersonnels = AddGroupPersonnel(attendanceGroupDTO.IdCardList, fyuUser, attendanceGroup);
            if (groupPersonnels.Item2 != null)
            {
                return groupPersonnels.Item2;
            }
            //列表展示
            attendanceGroup.Range = string.Join('、', groupPersonnels.Item3.Take(4)) + "等";
            //提交数据库
            if (groupPersonnels.Item1.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(groupPersonnels.Item1);
            }

            _attendanceUnitOfWork.BatchUpdate<AttendanceRule>(s => new AttendanceRule() { IsUsed = true }, s => s.AttendanceRuleID == attendanceGroupDTO.AttendanceRuleID);
            _attendanceUnitOfWork.BatchUpdate<Overtime>(s => new Overtime() { IsUsed = true }, s => s.OvertimeID == attendanceGroupDTO.OvertimeID);
            _attendanceUnitOfWork.Add(attendanceGroup);
            var isSuccess = _attendanceUnitOfWork.Commit();
            if (!isSuccess)
            {
                return new JsonResponse() { Code = CodeEnum.Created, IsSuccess = false, Message = "添加失败！" };
            }
            return new JsonResponse() { Code = CodeEnum.Created, IsSuccess = true, Message = "添加成功！" };
        }

        public AttendanceGroupDTO GetSingleAttendanceGroupDTO(string attendanceGroupID)
        {
            var attendanceGroup = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(a => a.AttendanceGroupID == attendanceGroupID).FirstOrDefault();
            var attendanceGroupDTO = _mapper.Map<AttendanceGroupDTO>(attendanceGroup);
            var groupPersonnel = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(a => a.AttendanceGroupID == attendanceGroupID, true).Select(a => a.IDCard).ToList();
            attendanceGroupDTO.IdCardList = groupPersonnel;
            if (attendanceGroup.ShiftType != ShiftTypeEnum.DoNotClockIn)
            {
                var weekDaysSetting = _weekDaysSettingRepository.EntityQueryable<WeekDaysSetting>(a => a.AttendanceGroupID == attendanceGroupID, true).OrderBy(a => a.Week).Select(a => new ShiftInfoDTO { ShiftId = a.ShiftID, IsHolidayWork = a.IsHolidayWork }).ToList();
                attendanceGroupDTO.ShiftInfo = ResortList(weekDaysSetting);

                var address = _groupAddressRepository.EntityQueryable<GroupAddress>(s => s.AttendanceGroupID == attendanceGroupID).Select(s => s.ClockInAddressID).ToList();
                List<ClockInAddressDTO> addressdetail = _clockInAddressRepository.EntityQueryable<ClockInAddress>(s => address.Contains(s.ClockInAddressID)).Select(s => new ClockInAddressDTO { ClockInAddressID = s.ClockInAddressID, ClockName = s.ClockName, Distance = s.Distance, Latitude = s.Latitude, LatitudeBD = s.LatitudeBD, Longitude = s.Longitude, LongitudeBD = s.LongitudeBD, SiteName = s.SiteName }).ToList();
                attendanceGroupDTO.SiteAttendance = JsonConvert.SerializeObject(addressdetail);
            }
            return attendanceGroupDTO;
        }
        /// <summary>
        /// 为datetime中的周枚举转换，周日存0，页面上周日要放在最后一个
        /// </summary>
        /// <param name="weekDaysSetting"></param>
        /// <returns></returns>
        public List<T> ResortList<T>(List<T> weekDaysSetting)
        {
            var firstweekDay = weekDaysSetting.First();
            weekDaysSetting.RemoveAt(0);
            weekDaysSetting.Add(firstweekDay);
            return weekDaysSetting;
        }

        public JsonResponse UpdateAttendanceGroup(AttendanceGroupDTO attendanceGroupDTO, FyuUser fyuUser)
        {
            var attendanceGroup = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(a => a.AttendanceGroupID == attendanceGroupDTO.AttendanceGroupID).FirstOrDefault();
            if (attendanceGroup == null)
            {
                return new JsonResponse() { Code = CodeEnum.BadRequest, IsSuccess = false, Message = "更新失败，考勤组被删除！" };
            }
            attendanceGroup.Name = attendanceGroupDTO.Name;
            var groupPersonnels = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(a => a.AttendanceGroupID == attendanceGroupDTO.AttendanceGroupID).ToList();
            foreach (var item in groupPersonnels)
            {
                _attendanceUnitOfWork.Delete(item);
            }
            var groupPersonnel = AddGroupPersonnel(attendanceGroupDTO.IdCardList, fyuUser, attendanceGroup);
            if (groupPersonnel.Item2 != null)
            {
                return groupPersonnel.Item2;
            }
            attendanceGroup.Range = string.Join('、', groupPersonnel.Item3.Take(4)) + "等";
            if (groupPersonnel.Item1.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(groupPersonnel.Item1);
            }
            if (attendanceGroup.ShiftType != ShiftTypeEnum.DoNotClockIn)
            {
                attendanceGroup.OvertimeID = attendanceGroupDTO.OvertimeID;
                attendanceGroup.ShiftType = (ShiftTypeEnum)attendanceGroupDTO.ShiftType;
                attendanceGroup.AttendanceRuleID = attendanceGroupDTO.AttendanceRuleID;
                attendanceGroup.ClockInWay = (ClockInWayEnum)attendanceGroupDTO.ClockInWay;
                attendanceGroup.IsDynamicRowHugh = attendanceGroupDTO.IsDynamicRowHugh;

                var weekDaysSettings = _weekDaysSettingRepository.EntityQueryable<WeekDaysSetting>(a => a.AttendanceGroupID == attendanceGroupDTO.AttendanceGroupID).OrderBy(a => a.Week).ToList();
                weekDaysSettings = ResortList(weekDaysSettings);
                for (int i = 0; i < weekDaysSettings.Count; i++)
                {
                    weekDaysSettings[i].ShiftID = attendanceGroupDTO.ShiftInfo[i].ShiftId;
                    weekDaysSettings[i].IsHolidayWork = attendanceGroupDTO.ShiftInfo[i].IsHolidayWork;
                }
                _attendanceUnitOfWork.Update(weekDaysSettings);

                var address = _groupAddressRepository.EntityQueryable<GroupAddress>(a => a.AttendanceGroupID == attendanceGroupDTO.AttendanceGroupID).ToList();
                foreach (var item in address)
                {
                    _attendanceUnitOfWork.Delete(item);
                }
                var groupAddress = AddGroupAddress(attendanceGroupDTO.Site, fyuUser, attendanceGroup);
                if (groupAddress.Count > 0)
                {
                    _attendanceUnitOfWork.BatchInsert(groupAddress);
                }
                _attendanceUnitOfWork.BatchUpdate<AttendanceRule>(s => new AttendanceRule() { IsUsed = true }, s => s.AttendanceRuleID == attendanceGroupDTO.AttendanceRuleID);
            }
            _attendanceUnitOfWork.Update(attendanceGroup);
            var IsSuccess = _attendanceUnitOfWork.Commit();
            return new JsonResponse() { Code = CodeEnum.ok, IsSuccess = IsSuccess, Message = "更新成功！" };
        }

        public List<GroupDTO> GetAttendanceGroupList(FyuUser user)
        {
            var res = _attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.CompanyID == user.customerId).Select(s => new GroupDTO { AttendanceGroupID = s.AttendanceGroupID, Name = s.Name }).ToList();
            res.Insert(0, new GroupDTO { AttendanceGroupID = "", Name = "全部" });
            return res;
        }

        public TimeSpan aaa(FyuUser user)
        {
            var time = DateTime.UtcNow;
            var a2 = _attendanceGroupRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId).ToList();
            var b2 = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.CompanyID == user.customerId).ToList();

            var query2 = from p in a2
                         join gp in b2 on p.IDCard equals gp.IDCard
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
            var dic2 = query2.ToList();
            return DateTime.UtcNow - time;
        }
        public TimeSpan aaa2(FyuUser user)
        {
            var time = DateTime.UtcNow;
            var a = _attendanceGroupRepository.EntityQueryable<Person>(s => s.CompanyID == user.customerId).ToDictionary(ok => ok.IDCard);
            var b = _groupPersonnelRepository.EntityQueryable<GroupPersonnel>(s => s.CompanyID == user.customerId).ToDictionary(ok => ok.IDCard);
            var query = from p in a
                        join gp in b on p.Key equals gp.Key
                        select new AddComputeDayDTO
                        {
                            CustomerID = p.Value.CustomerID,
                            DepartmentID = p.Value.DepartmentID,
                            CompanyID = p.Value.CompanyID,
                            CompanyName = p.Value.CompanyName,
                            Department = p.Value.Department,
                            Name = p.Value.Name,
                            Position = p.Value.Position,
                            IDCard = p.Value.IDCard,
                            IDType = p.Value.IDType,
                            EmployeeNo = p.Value.JobNumber,
                            AttendanceGroupID = gp.Value.AttendanceGroupID,
                        };
            var dic = query.ToList();
            return DateTime.UtcNow - time;
        }
    }
}
