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
    public class AttendanceRecordSelectService : BaseService, IAttendanceRecordSelectService
    {
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IClockRecordRepository _clockRecordRepository;
        private readonly IRedisProvider _redisProvider;
        private readonly IClockInfoRepository _clockInfoRepository;
        private readonly ILogger<AttendanceRecordService> _logger;

        public AttendanceRecordSelectService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IAttendanceRecordRepository attendanceRecordRepository, IClockRecordRepository clockRecordRepository, IClockInfoRepository clockInfoRepository,
            IRedisProvider redisProvider, ILogger<AttendanceRecordService> logger) : base(attendanceUnitOfWork, mapper, serializer)
        {
            _attendanceRecordRepository = attendanceRecordRepository;
            _clockRecordRepository = clockRecordRepository;
            _clockInfoRepository = clockInfoRepository;
            _redisProvider = redisProvider;
            _logger = logger;
        }

        public PageResult<AttendanceRecordDTO> GetAttendanceRecordList(string startTime, string endTime, int orgType, int departmentID, string name, int status, string groupID, FyuUser user, int pageIndex = 1, int pageSize = 10)
        {
            Expression<Func<AttendanceRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (orgType == 1)
            {
                expression = expression.And(s => s.CustomerID == departmentID);
            }
            if (orgType > 1)
            {
                expression = expression.And(s => s.DepartmentID == departmentID);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(groupID))
            {
                expression = expression.And(s => s.AttendanceGroupID == groupID);
            }
            if (status != 0)
            {
                expression = expression.And(s => s.Status == (ResultEnum)status);
            }
            PageResult<AttendanceRecord> list = _attendanceRecordRepository.GetByPage(pageIndex, pageSize, expression, s => s.AttendanceDate, SortOrderEnum.Descending);
            return PageMap<AttendanceRecordDTO, AttendanceRecord>(list);
        }

        public PageResult<ImportRecordDTO> GetAttendanceRecordAll(string startTime, string endTime, int status, string name, List<string> filter, int pageIndex, int pageSize, FyuUser user)
        {
            Expression<Func<AttendanceRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.AttendanceGroupID == name);
            }
            if (status != 0)
            {
                expression = expression.And(s => s.Status == (ResultEnum)status);
            }
            if (filter.Any() && filter != null)
            {
                expression = expression.And(s => filter.Contains(s.IDCard));
            }
            var record = _attendanceRecordRepository.EntityQueryable(expression);
            var clockInfos = _attendanceRecordRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from r in record
                        join c in clockInfos on r.AttendanceRecordID equals c.AttendanceRecordID into gc
                        from gci in gc.DefaultIfEmpty()
                        select new ImportRecordDTO
                        {
                            Name = r.Name,
                            AttendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd") + r.AttendanceDate.DayOfWeek.ConvertWeek(),
                            AttendanceRecordID = r.AttendanceRecordID,
                            ClockIn1 = gci.ClockInTime,
                            ClockInAddress = gci.StartLocation,
                            ClockInResult1 = gci.ClockInResult,
                            ClockOut1 = gci.ClockOutTime,
                            ClockOutAddress = gci.EndLocation,
                            ClockOutResult1 = gci.ClockOutResult,
                            Department = r.Department,
                            EmployeeNo = r.EmployeeNo,
                            IDCard = r.IDCard,
                            Status = r.Status.GetDescription(),
                            Shift = r.Shift,
                            Position = r.Position,
                            OutsideDuration = r.OutsideDuration,
                            BusinessTripDuration = r.BusinessTripDuration,
                            EarlyLeaveMinutes = r.EarlyLeaveMinutes,
                            EarlyLeaveTimes = r.EarlyLeaveTimes,
                            LateMinutes = r.LateMinutes,
                            LateTimes = r.LateTimes,
                            LeaveDuration = r.LeaveDuration,
                            NotClockInTimes = r.NotClockInTimes,
                            NotClockOutTimes = r.NotClockOutTimes
                        };
            int totalNumber = query.Count();
            List<ImportRecordDTO> record2 = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            DataSolve(record2);
            return new PageResult<ImportRecordDTO>(totalNumber, (totalNumber + pageSize - 1) / pageSize, pageIndex, pageSize, record2);
        }
        /// <summary>
        /// 获取微信详情页月表数组  //未过滤休息日，
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public List<RecordDTO> GetRecordInfo(string idcard, DateTime time)
        {
            var res = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate >= time.Date && s.AttendanceDate < time.AddMonths(1).Date && s.IDCard == idcard).OrderBy(s => s.AttendanceDate).Select(s => new RecordDTO { Status = (int)s.Status, date = s.AttendanceDate.ToString("yyyy-MM-dd"), AttendanceDate = s.AttendanceDate, Shift = s.Shift }).ToList();
            var clock = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate >= time.Date && s.AttendanceDate < time.AddMonths(1).Date && s.IDCard == idcard).GroupBy(s => s.AttendanceDate).Select(s => new RecordDTO { AttendanceDate = s.FirstOrDefault().AttendanceDate, Count = s.Count() }).ToList();
            foreach (var item in res)
            {
                foreach (var item2 in clock)
                {
                    if (item.AttendanceDate == item2.AttendanceDate)
                    {
                        item.Count = item2.Count;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取打卡信息列表
        /// </summary>
        /// <param name="attendanceRecordID"></param>
        /// <returns></returns>
        public List<ClockInfoDTO> GetClockInfoList(string attendanceRecordID)
        {
            var res = _clockInfoRepository.EntityQueryable<ClockInfo>(s => s.AttendanceRecordID == attendanceRecordID);
            if (!res.Any())
            {
                return new List<ClockInfoDTO>();
            }
            var a = res.ToList();
            return _mapper.Map<List<ClockInfoDTO>>(a);
        }

        /// <summary>
        /// 日表特定时间导出
        /// </summary>
        /// <returns></returns>
        public byte[] GetDayRecord(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user)
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "Name", "姓名"),
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Position", "职位"),
                new Diction( "IDCard", "证照号码"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次信息"),
                new Diction( "Status", "出勤状态"),
                new Diction( "ClockIn", "上班打卡时间"),
                new Diction( "ClockInResult", "上班打卡结果"),
                new Diction( "ClockInAddress", "上班打卡地址"),
                new Diction( "ClockOut", "下班打卡时间"),
                new Diction( "ClockOutResult", "下班打卡结果"),
                new Diction( "ClockOutAddress", "下班打卡地址"),
                new Diction( "LateTimes", "迟到次数"),
                new Diction( "LateMinutes", "迟到时长（分钟）"),
                new Diction( "EarlyLeaveTimes", "早退次数"),
                new Diction( "EarlyLeaveMinutes", "早退时长（分钟）"),
                new Diction( "NotClockInTimes", "上班缺卡次数"),
                new Diction( "NotClockOutTimes", "下班缺卡次数"),
                new Diction( "BusinessTripDuration", "出差时长(天)"),
                new Diction( "OutsideDuration", "外出时长(小时)"),
                new Diction( "LeaveDuration", "请假时长(小时)"),
            };
            Expression<Func<AttendanceRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (status != 0)
            {
                expression = expression.And(s => s.Status == (ResultEnum)status);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.AttendanceGroupID == name);
            }
            if (filter.Any() && filter != null)
            {
                expression = expression.And(s => filter.Contains(s.IDCard));
            }
            var record = _attendanceRecordRepository.EntityQueryable(expression);
            var clockInfos = _attendanceRecordRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from r in record
                        join c in clockInfos on r.AttendanceRecordID equals c.AttendanceRecordID into gc
                        from gci in gc.DefaultIfEmpty()
                        select new ImportRecordDTO
                        {
                            Name = r.Name,
                            AttendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd") + r.AttendanceDate.DayOfWeek.ConvertWeek(),
                            AttendanceRecordID = r.AttendanceRecordID,
                            ClockIn1 = gci.ClockInTime,
                            ClockInAddress = gci.StartLocation,
                            ClockInResult1 = gci.ClockInResult,
                            ClockOut1 = gci.ClockOutTime,
                            ClockOutAddress = gci.EndLocation,
                            ClockOutResult1 = gci.ClockOutResult,
                            Department = r.Department,
                            EmployeeNo = r.EmployeeNo,
                            IDCard = r.IDCard,
                            Status = r.Status.GetDescription(),
                            Shift = r.Shift,
                            Position = r.Position,
                            OutsideDuration = r.OutsideDuration,
                            BusinessTripDuration = r.BusinessTripDuration,
                            EarlyLeaveMinutes = r.EarlyLeaveMinutes,
                            EarlyLeaveTimes = r.EarlyLeaveTimes,
                            LateMinutes = r.LateMinutes,
                            LateTimes = r.LateTimes,
                            LeaveDuration = r.LeaveDuration,
                            NotClockInTimes = r.NotClockInTimes,
                            NotClockOutTimes = r.NotClockOutTimes
                        };
            var record2 = query.OrderByDescending(s => s.AttendanceDate).ToList();
            DataSolve(record2);
            return NPOIHelp.OutputExcelXSSF(new NPIOExtend<ImportRecordDTO>(record2, title, "签到统计"));
        }

        /// <summary>
        /// 确认审核外勤打卡转为已审核
        /// </summary>
        /// <param name="clockRecordID"></param>
        /// <returns></returns>
        public bool Itinerancy(int clockRecordID)
        {
            var clockRecordWq = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.ClockRecordID == clockRecordID).FirstOrDefault();
            if (clockRecordWq == null)
            {
                return false;
            }
            clockRecordWq.IsFieldAudit = FieldAuditEnum.Checked;
            _attendanceUnitOfWork.Update(clockRecordWq);
            var res = _attendanceUnitOfWork.Commit();
            if (res)
            {
                _redisProvider.ListLeftPushAsync(MessageTypeEnum.Compute.GetDescription(), new List<RealTimeDTO> { new RealTimeDTO { clockType = clockRecordWq.ClockType == ClockTypeEnum.GetOffWork, IDcard = clockRecordWq.IDCard, time = clockRecordWq.AttendanceDate } });
                return res;
            }
            return res;
        }

        /// <summary>
        /// 获取错误消息，写入Redis
        /// </summary>
        /// <param name="time"></param>
        public void GetErrorRecordWechatMessage(DateTime time)
        {
            DateTime today = time.AddDays(-1).Date;
            List<ErrorMessageDTO> Records = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate == today && s.Status == ResultEnum.Abnormal && s.IsLackOfCard).Select(s => new ErrorMessageDTO { IDCard = s.IDCard, Name = s.Name }).ToList();
            var clockrecords = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == today).Select(s => new { s.IDCard, s.ClockType }).ToList();
            Records = Records.Where(s =>
            {
                return !(clockrecords.Where(t => t.IDCard == s.IDCard && t.ClockType == ClockTypeEnum.GetOffWork).Any() && clockrecords.Where(t => t.IDCard == s.IDCard && t.ClockType == ClockTypeEnum.Working).Any());
            }).ToList();
            if (Records.Any())
            {
                _redisProvider.SetAddAsync(MessageTypeEnum.ErrorMessage.GetDescription(), Records);
            }
        }

        /// <summary>
        /// 日表特定时间导出
        /// </summary>
        /// <returns></returns>
        public (string, string) GetDayRecordTest(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user)
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "Name", "姓名"),
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Position", "职位"),
                new Diction( "IDCard", "证照号码"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次信息"),
                new Diction( "Status", "出勤状态"),
                new Diction( "ClockIn", "上班打卡时间"),
                new Diction( "ClockInResult", "上班打卡结果"),
                new Diction( "ClockInAddress", "上班打卡地址"),
                new Diction( "ClockOut", "下班打卡时间"),
                new Diction( "ClockOutResult", "下班打卡结果"),
                new Diction( "ClockOutAddress", "下班打卡地址"),
                new Diction( "LateTimes", "迟到次数"),
                new Diction( "LateMinutes", "迟到时长（分钟）"),
                new Diction( "EarlyLeaveTimes", "早退次数"),
                new Diction( "EarlyLeaveMinutes", "早退时长（分钟）"),
                new Diction( "NotClockInTimes", "上班缺卡次数"),
                new Diction( "NotClockOutTimes", "下班缺卡次数"),
                new Diction( "BusinessTripDuration", "出差时长(天)"),
                new Diction( "OutsideDuration", "外出时长(小时)"),
                new Diction( "LeaveDuration", "请假时长(小时)"),
            };
            Expression<Func<AttendanceRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (status != 0)
            {
                expression = expression.And(s => s.Status == (ResultEnum)status);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.AttendanceGroupID == name);
            }
            if (filter.Any() && filter != null)
            {
                expression = expression.And(s => filter.Contains(s.IDCard));
            }
            var record = _attendanceRecordRepository.EntityQueryable(expression);
            var clockInfos = _attendanceRecordRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from r in record
                        join c in clockInfos on r.AttendanceRecordID equals c.AttendanceRecordID into gc
                        from gci in gc.DefaultIfEmpty()
                        select new ImportRecordDTO
                        {
                            Name = r.Name,
                            AttendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd") + r.AttendanceDate.DayOfWeek.ConvertWeek(),
                            AttendanceRecordID = r.AttendanceRecordID,
                            ClockIn1 = gci.ClockInTime,
                            ClockInAddress = gci.StartLocation,
                            ClockInResult1 = gci.ClockInResult,
                            ClockOut1 = gci.ClockOutTime,
                            ClockOutAddress = gci.EndLocation,
                            ClockOutResult1 = gci.ClockOutResult,
                            Department = r.Department,
                            EmployeeNo = r.EmployeeNo,
                            IDCard = r.IDCard,
                            Status = r.Status.GetDescription(),
                            Shift = r.Shift,
                            Position = r.Position,
                            OutsideDuration = Math.Round(r.OutsideDuration, 3),
                            BusinessTripDuration = Math.Round(r.BusinessTripDuration, 3),
                            EarlyLeaveMinutes = Math.Round(r.EarlyLeaveMinutes, 3),
                            EarlyLeaveTimes = r.EarlyLeaveTimes,
                            LateMinutes = Math.Round(r.LateMinutes, 3),
                            LateTimes = r.LateTimes,
                            LeaveDuration = Math.Round(r.LeaveDuration, 3),
                            NotClockInTimes = r.NotClockInTimes,
                            NotClockOutTimes = r.NotClockOutTimes
                        };
            int totalNumber = query.Count();
            var time = DateTime.Now.Ticks.ToString();
            string filename = "签到统计" + time + ".xlsx";
            //string zipname = "签到统计" + time + ".zip";
            string path = $"{Directory.GetCurrentDirectory()}/wwwroot/Excle/{filename}";
            //string zippath = $"{Directory.GetCurrentDirectory()}/wwwroot/Excle/{zipname}";
            string filePath = Path.GetDirectoryName(path);
            if (!Directory.Exists(filePath))
            {
                if (filePath != null)
                {
                    Directory.CreateDirectory(filePath);
                }
            }
            NPOIHelp.BeginGenerate(path, query, totalNumber, title, "签到统计", DataSolve);
            //ZipHelp.ZipFile(path,zippath,"123456");
            return (filename, path);
        }

        private void DataSolve(List<ImportRecordDTO> record2)
        {
            record2.ForEach(s =>
            {
                s.ClockIn = s.ClockIn1 == null ? "" : s.ClockIn1 == new DateTime() ? "" : ((DateTime)s.ClockIn1).ToString("yyyy/MM/dd HH:mm");
                s.ClockOut = s.ClockOut1 == null ? "" : s.ClockOut1 == new DateTime() ? "" : ((DateTime)s.ClockOut1).ToString("yyyy/MM/dd HH:mm");
                s.ClockInResult = s.ClockInResult1 == null ? "" : s.ClockInResult1.GetDescription();
                s.ClockOutResult = s.ClockOutResult1 == null ? "" : s.ClockOutResult1.GetDescription();
                s.OutsideDuration = Math.Round(s.OutsideDuration, 3);
                s.BusinessTripDuration = Math.Round(s.BusinessTripDuration, 3);
                s.EarlyLeaveMinutes = Math.Round(s.EarlyLeaveMinutes, 3);
                s.LateMinutes = Math.Round(s.LateMinutes, 3);
                s.LeaveDuration = Math.Round(s.LeaveDuration, 3);
            });
        }

        /// <summary>
        /// 日表特定时间导出
        /// </summary>
        /// <returns></returns>
        public byte[] GetDayRecord3(string startTime, string endTime, int status, List<string> filter, string name, FyuUser user)
        {
            //表格内容
            var title = new List<Diction>()
            {
                new Diction( "Name", "姓名"),
                new Diction( "EmployeeNo", "工号"),
                new Diction( "Department", "部门"),
                new Diction( "Position", "职位"),
                new Diction( "IDCard", "证照号码"),
                new Diction( "AttendanceDate", "考勤日期"),
                new Diction( "Shift", "班次信息"),
                new Diction( "Status", "出勤状态"),
                new Diction( "ClockIn", "上班打卡时间"),
                new Diction( "ClockInResult", "上班打卡结果"),
                new Diction( "ClockInAddress", "上班打卡地址"),
                new Diction( "ClockOut", "下班打卡时间"),
                new Diction( "ClockOutResult", "下班打卡结果"),
                new Diction( "ClockOutAddress", "下班打卡地址"),
                new Diction( "LateTimes", "迟到次数"),
                new Diction( "LateMinutes", "迟到时长（分钟）"),
                new Diction( "EarlyLeaveTimes", "早退次数"),
                new Diction( "EarlyLeaveMinutes", "早退时长（分钟）"),
                new Diction( "NotClockInTimes", "上班缺卡次数"),
                new Diction( "NotClockOutTimes", "下班缺卡次数"),
                new Diction( "BusinessTripDuration", "出差时长(天)"),
                new Diction( "OutsideDuration", "外出时长(小时)"),
                new Diction( "LeaveDuration", "请假时长(小时)"),
            };
            Expression<Func<AttendanceRecord, bool>> expression = s => s.CompanyID == user.customerId;
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.Parse(startTime);
                expression = expression.And(s => s.AttendanceDate >= start.Date);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.Parse(endTime).AddDays(1).AddSeconds(-1);
                expression = expression.And(s => s.AttendanceDate <= end.Date);
            }
            if (status != 0)
            {
                expression = expression.And(s => s.Status == (ResultEnum)status);
            }
            if (!string.IsNullOrEmpty(name))
            {
                expression = expression.And(s => s.AttendanceGroupID == name);
            }
            if (filter.Any() && filter != null)
            {
                expression = expression.And(s => filter.Contains(s.IDCard));
            }
            var record = _attendanceRecordRepository.EntityQueryable(expression);
            var clockInfos = _attendanceRecordRepository.EntityQueryable<ClockInfo>(s => true);
            var query = from r in record
                        join c in clockInfos on r.AttendanceRecordID equals c.AttendanceRecordID into gc
                        from gci in gc.DefaultIfEmpty()
                        select new ImportRecordDTO
                        {
                            Name = r.Name,
                            AttendanceDate = r.AttendanceDate.ToString("yyyy-MM-dd") + r.AttendanceDate.DayOfWeek.ConvertWeek(),
                            AttendanceRecordID = r.AttendanceRecordID,
                            ClockIn1 = gci.ClockInTime,
                            ClockInAddress = gci.StartLocation,
                            ClockInResult1 = gci.ClockInResult,
                            ClockOut1 = gci.ClockOutTime,
                            ClockOutAddress = gci.EndLocation,
                            ClockOutResult1 = gci.ClockOutResult,
                            Department = r.Department,
                            EmployeeNo = r.EmployeeNo,
                            IDCard = r.IDCard,
                            Status = r.Status.GetDescription(),
                            Shift = r.Shift,
                            Position = r.Position,
                            OutsideDuration = r.OutsideDuration,
                            BusinessTripDuration = r.BusinessTripDuration,
                            EarlyLeaveMinutes = r.EarlyLeaveMinutes,
                            EarlyLeaveTimes = r.EarlyLeaveTimes,
                            LateMinutes = r.LateMinutes,
                            LateTimes = r.LateTimes,
                            LeaveDuration = r.LeaveDuration,
                            NotClockInTimes = r.NotClockInTimes,
                            NotClockOutTimes = r.NotClockOutTimes
                        };
            var totalNumber = query.Count();
            return NPOIHelp.OutputExcelNew2(query, totalNumber, title, "签到统计", DataSolve);
        }
    }
}

