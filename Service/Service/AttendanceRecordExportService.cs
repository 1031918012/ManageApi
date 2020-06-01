using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Cache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    public class AttendanceRecordExportService : IAttendanceRecordExportService
    {
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IDayTableCalculationRepository _dayTableCalculationRepository;
        private readonly IClockInfoRepository _clockInfoRepository;
        private readonly ILogger<AttendanceRecordService> _logger;

        public AttendanceRecordExportService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IAttendanceRecordRepository attendanceRecordRepository, IClockRecordRepository clockRecordRepository, IAttendanceMonthlyRecordRepository attendanceMonthlyRecordRepository, IAttendanceGroupRepository attendanceGroupRepository, IGroupPersonnelRepository groupPersonnelRepository, IShiftManagementRepository shiftManagementRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IPersonRepository personRepository, IHolidayRepository holidayRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IHttpRequestHelp httpRequestHelp, IFYUServerClient fYUServerClient, IWeChatMiddleware weChatMiddleware, IShiftTimeRepository shiftTimeRepository, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository, IDayTableCalculationRepository dayTableCalculationRepository, IClockInfoRepository clockInfoRepository, IOvertimeRepository overtimeRepository,
            IRedisProvider redisProvider, ILogger<AttendanceRecordService> logger, IEnterpriseSetRepository enterpriseSetRepository)
        {
            _clockInfoRepository = clockInfoRepository;
            _attendanceRecordRepository = attendanceRecordRepository;
            _dayTableCalculationRepository = dayTableCalculationRepository;
            _logger = logger;
        }

        /// <summary>
        /// 导出补卡模板
        /// </summary>
        /// <returns></returns>
        public byte[] ExportRepairExcel()
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "IdCard", "姓名",null,"特殊字符、数字均无效，输入中文名或英文名,例：张三，李四，windy wang等"),
                new Diction( "IdCard", "证照号码",null,"暂时只支持身份证，例：420114199602191215"),
                new Diction( "Clocktime", "补卡时间",null,"补卡时间需要精确到时分，注意不支持2019-01-01 18：01这样的格式，例：2019/1/1 18:01或2019/1/2 8:01"),
                new Diction( "Site", "补卡地址",null,"仅各个地区的地址名称，其他字符、字母、数字均无效,例：深圳市泛亚人力资源股份有限公司 等"),
            };
            return NPOIHelp.OutputExcelXSSF(new NPIOExtend<ImportClockRecord>(null, title, "补卡记录"));
        }

        /// <summary>
        /// 导出假期模板
        /// </summary>
        /// <returns></returns>
        public byte[] ExportLeaveExcel()
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "IdCard", "姓名",null,"特殊字符、数字均无效，输入中文名或英文名,例：张三，李四，windy wang等"),
                new Diction( "IdCard", "证照号码",null,"暂时只支持身份证，例：420114199602191215"),
                new Diction( "StartTime", "开始时间",null,"开始时间必须小于结束时间，否则无法导入，注意不支持2019-01-01 18：01这样的格式，例：2019/1/1 8:01"),
                new Diction( "EndTime", "结束时间",null,"结束时间必须大于开始时间，否则无法导入，例：2019/1/1 18:01"),
                new Diction( "LeaveType", "请假类型",null,"仅支持企业设置中请假的各种类型，其他字符、字母、数字均无效,例：事假、哺乳假等"),
            };
            return NPOIHelp.OutputExcelXSSF(new NPIOExtend<ImportLeaveRecord>(null, title, "请假记录"));
        }

        /// <summary>
        /// 导出出差外出模板
        /// </summary>
        /// <returns></returns>
        public byte[] ExportOutExcel()
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "IdCard", "姓名",null,"特殊字符、数字均无效，输入中文名或英文名,例：张三，李四，windy wang等"),
                new Diction( "IdCard", "证照号码",null,"暂时只支持身份证，例：420114199602191215"),
                new Diction( "StartTime", "开始时间",null,"开始时间必须小于结束时间，否则无法导入，注意不支持2019-01-01 18：01这样的格式，例：2019/1/1 8:01"),
                new Diction( "EndTime", "结束时间",null,"结束时间必须大于开始时间，否则无法导入，例：2019/1/1 18:01"),
            };
            return NPOIHelp.OutputExcelXSSF(new NPIOExtend<ImportLeaveRecord>(null, title, "外出记录"));
        }

        /// <summary>
        /// 导出工作日加班表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[] ExportOvertimeSt(FyuUser user, DateTime time)
        {
            var title = new List<Diction>()
            {
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Name", "姓名"),
                new Diction( "AttendanceGroupName", "考勤组"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次时间"),
                new Diction( "WorkHours", "标准工作时间(小时)"),
                new Diction( "ClockInTime", "上班时间"),
                new Diction( "ClockOutTime", "下班时间"),
                new Diction( "Overtime", "加班时长(小时)"),
            };
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Month == time.Month && s.AttendanceDate.Year == time.Year && s.CompanyID == user.customerId && s.Shift.Length > 3);
            var dayTable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => true);
            var clockinfo = _clockInfoRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from p in record
                        join gp in dayTable on p.AttendanceRecordID equals gp.AttendanceRecordID
                        join ci in clockinfo on gp.AttendanceRecordID equals ci.AttendanceRecordID
                        select new ExportOvertimeDTO
                        {
                            AttendanceDate = p.AttendanceDate.ToString("yyyy/MM/dd"),
                            AttendanceGroupName = p.AttendanceGroupName,
                            ClockInTime = ci.ClockInTime == new DateTime() ? "" : ci.ClockInTime.ToString("HH:mm"),
                            ClockOutTime = ci.ClockOutTime == new DateTime() ? "" : ci.ClockOutTime.ToString("HH:mm"),
                            Department = p.Department,
                            EmployeeNo = p.EmployeeNo,
                            Name = p.Name,
                            Shift = p.Shift,
                            WorkHours = gp.WorkHours,
                            ItemDTO = p.AttendanceItemDTOJson,
                            Overtime = 0,
                        };
            var allGroupPeople = query.ToList().OrderBy(s => s.EmployeeNo).OrderBy(s => s.Department).ToList();
            allGroupPeople.ForEach(s =>
            {
                s.Overtime = decimal.Round(JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(s.ItemDTO).Where(t => t.AttendanceItemName == "工作日加班").FirstOrDefault().AttendanceItemValue, 2);
            });
            return NPOIHelp.StOutputExcelXSSF(new NPIOExtend<ExportOvertimeDTO>(allGroupPeople, title, "加班记录", time.ToString("yyyy年MM月") + user.customerName + "工作日加班统计表"), user);
        }

        /// <summary>
        /// 导出缺卡统计表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[] ExportMissingClockSt(FyuUser user, DateTime time)
        {
            var title = new List<Diction>()
            {
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Name", "姓名"),
                new Diction( "AttendanceGroupName", "考勤组"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次时间"),
                new Diction( "ClockInTime", "上班时间"),
                new Diction( "ClockOutTime", "下班时间"),
                new Diction( "ClockResult", "考勤结果"),
            };
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Month == time.Month && s.AttendanceDate.Year == time.Year && s.CompanyID == user.customerId && s.IsLackOfCard && s.Status == ResultEnum.Abnormal);
            var clockinfo = _clockInfoRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from p in record
                        join ci in clockinfo on p.AttendanceRecordID equals ci.AttendanceRecordID
                        select new ExportMissingClockDTO
                        {
                            AttendanceDate = p.AttendanceDate.ToString("yyyy/MM/dd"),
                            AttendanceGroupName = p.AttendanceGroupName,
                            ClockInTime = ci.ClockInTime == new DateTime() ? "" : ci.ClockInTime.ToString("HH:mm"),
                            ClockOutTime = ci.ClockOutTime == new DateTime() ? "" : ci.ClockOutTime.ToString("HH:mm"),
                            Department = p.Department,
                            EmployeeNo = p.EmployeeNo,
                            Name = p.Name,
                            Shift = p.Shift,
                            ClockInResult = ci.ClockInResult,
                            ClockOutResult = ci.ClockOutResult,
                            ClockResult = ""
                        };
            var allGroupPeople = query.ToList().OrderBy(s => s.EmployeeNo).OrderBy(s => s.Department).ToList();
            foreach (var s in allGroupPeople)
            {
                if (s.ClockOutResult == ClockResultEnum.LackOfWorkCard && s.ClockInResult == ClockResultEnum.WorkCardMissing)
                {
                    s.ClockResult = "上班缺卡/下班缺卡";
                    continue;
                }
                if (s.ClockInResult == ClockResultEnum.WorkCardMissing)
                {
                    s.ClockResult = "上班缺卡";
                    continue;
                }
                if (s.ClockOutResult == ClockResultEnum.LackOfWorkCard)
                {
                    s.ClockResult = "下班缺卡";
                    continue;

                }
            }
            return NPOIHelp.StOutputExcelXSSF(new NPIOExtend<ExportMissingClockDTO>(allGroupPeople, title, "加班记录", time.ToString("yyyy年MM月") + user.customerName + "缺卡统计表"), user);
        }

        /// <summary>
        /// 导出迟到早退统计表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[] ExportLateSt(FyuUser user, DateTime time)
        {
            var title = new List<Diction>()
            {
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Name", "姓名"),
                new Diction( "AttendanceGroupName", "考勤组"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次时间"),
                new Diction( "ClockInTime", "上班时间"),
                new Diction( "ClockOutTime", "下班时间"),
                new Diction( "LateTime", "迟到时间（分钟）"),
                new Diction( "EarlyLeaveTime", "早退时间（分钟）"),
                new Diction( "ClockResult", "考勤结果"),
            };
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.CompanyID == user.customerId && s.AttendanceDate.Month == time.Month && s.AttendanceDate.Year == time.Year);
            var clockinfo = _clockInfoRepository.EntityQueryable<ClockInfo>(s => s.ClockInResult == ClockResultEnum.Late || s.ClockOutResult == ClockResultEnum.EarlyLeave);
            var query = from p in record
                        join ci in clockinfo on p.AttendanceRecordID equals ci.AttendanceRecordID
                        select new ExportLateOrEarlyLeaveDTO
                        {
                            AttendanceDate = p.AttendanceDate.ToString("yyyy/MM/dd"),
                            AttendanceGroupName = p.AttendanceGroupName,
                            ClockInTime = ci.ClockInTime == new DateTime() ? "" : ci.ClockInTime.ToString("HH:mm"),
                            ClockOutTime = ci.ClockOutTime == new DateTime() ? "" : ci.ClockOutTime.ToString("HH:mm"),
                            Department = p.Department,
                            EmployeeNo = p.EmployeeNo,
                            Name = p.Name,
                            Shift = p.Shift,
                            ItemDTO = p.AttendanceItemDTOJson,
                            ClockInResult = ci.ClockInResult,
                            ClockOutResult = ci.ClockOutResult,
                            LateTime = 0,
                            EarlyLeaveTime = 0,
                            ClockResult = ""
                        };
            var allGroupPeople = query.ToList().OrderBy(s => s.EmployeeNo).OrderBy(s => s.Department).ToList();
            foreach (var s in allGroupPeople)
            {
                s.LateTime = (int)JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(s.ItemDTO).Where(t => t.AttendanceItemName == "迟到" && t.Unit == "分钟数").FirstOrDefault().AttendanceItemValue;
                s.EarlyLeaveTime = (int)JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(s.ItemDTO).Where(t => t.AttendanceItemName == "早退" && t.Unit == "分钟数").FirstOrDefault().AttendanceItemValue;
                if (s.ClockOutResult == ClockResultEnum.EarlyLeave && s.ClockInResult == ClockResultEnum.Late)
                {
                    s.ClockResult = "迟到/早退";
                    continue;
                }
                if (s.ClockInResult == ClockResultEnum.Late)
                {
                    s.ClockResult = "迟到";
                    continue;
                }
                if (s.ClockOutResult == ClockResultEnum.EarlyLeave)
                {
                    s.ClockResult = "早退";
                    continue;
                }
            }
            return NPOIHelp.StOutputExcelXSSF(new NPIOExtend<ExportLateOrEarlyLeaveDTO>(allGroupPeople, title, "迟到、早退记录", time.ToString("yyyy年MM月") + user.customerName + "迟到、早退统计表"), user);
        }

        /// <summary>
        /// 导出工时统计表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public byte[] ExportWorkSt(FyuUser user, DateTime time)
        {
            var title = new List<Diction>()
            {
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Name", "姓名"),
                new Diction( "AttendanceGroupName", "考勤组"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次时间"),
                new Diction( "WorkHours", "标准工作时间(小时)"),
                new Diction( "ClockInTime", "上班时间"),
                new Diction( "ClockOutTime", "下班时间"),
                new Diction( "WorkingTime", "工作时长(小时)"),
            };
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate.Month == time.Month && s.AttendanceDate.Year == time.Year && s.CompanyID == user.customerId);
            var dayTable = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => true);
            var clockinfo = _clockInfoRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from p in record
                        join gp in dayTable on p.AttendanceRecordID equals gp.AttendanceRecordID
                        join ci in clockinfo on gp.AttendanceRecordID equals ci.AttendanceRecordID
                        select new ExportWorkDTO
                        {
                            AttendanceDate = p.AttendanceDate.ToString("yyyy/MM/dd"),
                            AttendanceGroupName = p.AttendanceGroupName,
                            ClockInTime = ci.ClockInTime == new DateTime() ? "" : ci.ClockInTime.ToString("HH:mm"),
                            ClockOutTime = ci.ClockOutTime == new DateTime() ? "" : ci.ClockOutTime.ToString("HH:mm"),
                            Department = p.Department,
                            EmployeeNo = p.EmployeeNo,
                            Name = p.Name,
                            Shift = p.Shift,
                            WorkHours = gp.WorkHours,
                            WorkingTime = p.WorkingTime,
                        };
            var allGroupPeople = query.ToList().OrderBy(s => s.EmployeeNo).OrderBy(s => s.Department).ToList();
            return NPOIHelp.StOutputExcelXSSF(new NPIOExtend<ExportWorkDTO>(allGroupPeople, title, "员工工时记录", time.ToString("yyyy年MM月") + user.customerName + "员工工时统计表"), user);
        }
    }
}

