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
    public class AttendanceRecordService : RecordBaseService, IAttendanceRecordService
    {
        private readonly IAttendanceRecordRepository _attendanceRecordRepository;
        private readonly IClockRecordRepository _clockRecordRepository;
        private readonly IShiftManagementRepository _shiftManagementRepository;
        private readonly IHolidayRepository _holidayRepository;
        private readonly IWorkPaidLeaveRepository _workPaidLeaveRepository;
        private readonly IHttpRequestHelp _httpRequestHelp;
        private readonly IDayTableCalculationRepository _dayTableCalculationRepository;
        private readonly IBreakoffRepository _breakoffRepository;
        private readonly IBreakoffDetailRepository _breakoffDetailRepository;
        private readonly IPersonRepository _personRepository;
        private readonly ILogger<AttendanceRecordService> _logger;

        public AttendanceRecordService(IAttendanceUnitOfWork attendanceUnitOfWork, IMapper mapper, ISerializer<string> serializer, IAttendanceRecordRepository attendanceRecordRepository, IClockRecordRepository clockRecordRepository, IShiftManagementRepository shiftManagementRepository, IWeekDaysSettingRepository weekDaysSettingRepository, IHolidayRepository holidayRepository, IWorkPaidLeaveRepository workPaidLeaveRepository, IHttpRequestHelp httpRequestHelp, IAttendanceItemCatagoryRepository attendanceItemCatagoryRepository, IDayTableCalculationRepository dayTableCalculationRepository, ILogger<AttendanceRecordService> logger, IEnterpriseSetRepository enterpriseSetRepository, IBreakoffRepository breakoffRepository, IBreakoffDetailRepository breakoffDetailRepository, IPersonRepository personRepository) : base(attendanceUnitOfWork, mapper, serializer, enterpriseSetRepository, attendanceItemCatagoryRepository)
        {
            _personRepository = personRepository;
            _attendanceRecordRepository = attendanceRecordRepository;
            _clockRecordRepository = clockRecordRepository;
            _shiftManagementRepository = shiftManagementRepository;
            _holidayRepository = holidayRepository;
            _workPaidLeaveRepository = workPaidLeaveRepository;
            _dayTableCalculationRepository = dayTableCalculationRepository;
            _httpRequestHelp = httpRequestHelp;
            _logger = logger;
            _breakoffRepository = breakoffRepository;
            _breakoffDetailRepository = breakoffDetailRepository;
        }


        /// <summary>
        /// 计算打卡之后的个人计算
        /// </summary>
        /// <param name="IDcard"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool ComputeOneIDCard(string IDcard, DateTime time, bool clockType = true)
        {
            List<ClockRecord> clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(t => t.IDCard == IDcard && t.AttendanceDate == time.Date && t.IsFieldAudit == FieldAuditEnum.Checked).OrderBy(t => t.ClockTime).ToList();
            return ComputOnePeople(clockRecord, time, IDcard, clockType);
        }

        /// <summary>
        /// 统计前两日的日表统计
        /// </summary>
        public void DoAllComputeTwo(DateTime time)
        {
            DoAllComputeNew(time);
            DoAllComputeNew(time.AddDays(-1));
        }

        /// <summary>
        /// 计算所有人当天时间的日表记录
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public (bool, string) DoAllComputeNew(DateTime time)
        {
            var recordAll = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate == time.Date).GroupBy(s => s.CompanyID);
            if (!recordAll.Any())
            {
                return (false, "今日无打卡人员,无需计算!");
            }

            foreach (var item in recordAll)
            {
                var record = item.ToList();
                var dayTableCalculationCompany = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => record.Select(t => t.AttendanceRecordID).Contains(s.AttendanceRecordID)).ToList();
                var breakoff = _breakoffRepository.EntityQueryable<Breakoff>(s => s.CompanyID == record[0].CompanyID).ToList();
                for (int i = 0; i < record.Count(); i++)
                {
                    try
                    {
                        List<ClockRecord> clockRecord = _clockRecordRepository.EntityQueryable<ClockRecord>(t => t.IDCard == record[i].IDCard && t.AttendanceDate == record[i].AttendanceDate && t.IsFieldAudit == FieldAuditEnum.Checked).OrderBy(t => t.ClockTime).ToList();
                        var dayTableCalculation = dayTableCalculationCompany.Where(s => record[i].AttendanceRecordID == s.AttendanceRecordID).FirstOrDefault();
                        var breakoffitem = breakoff.Where(s => s.IDCard == record[i].IDCard).FirstOrDefault();
                        GetNowRecordNew(record[i], dayTableCalculation, time, clockRecord, breakoffitem);
                    }
                    catch (Exception ex)
                    {
                        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Release")
                        {
                            _httpRequestHelp.RequestGet("https://sc.ftqq.com/SCU50608T4d004dff917249bfb7d7fd5f3000c9745ccd6ed0741a2.send?text=添加纪录失败&&desp=" + "错误为" + JsonConvert.SerializeObject(ex) + "数据为" + JsonConvert.SerializeObject(record[i]) + time);
                        }
                        continue;
                    }
                }
            }
            return (true, "计算成功!");

        }

        #region 计算日表所有方法

        /// <summary>
        /// 处理日表
        /// </summary>
        /// <param name="clockRecord"></param>
        /// <param name="doTime"></param>
        /// <returns></returns>
        private bool ComputOnePeople(List<ClockRecord> clockRecord, DateTime doTime, string IDcard, bool clockType = true)
        {
            AttendanceRecord record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(t => t.IDCard == IDcard && t.AttendanceDate == doTime).FirstOrDefault();
            if (record == null)
            {
                return false;
            }
            DayTableCalculation dayTableCalculation = _dayTableCalculationRepository.EntityQueryable<DayTableCalculation>(s => s.AttendanceRecordID == record.AttendanceRecordID).FirstOrDefault();
            Breakoff breakoff = _breakoffRepository.EntityQueryable<Breakoff>(s => s.IDCard == record.IDCard && s.CompanyID == record.CompanyID).FirstOrDefault();
            return GetNowRecordNew(record, dayTableCalculation, doTime, clockRecord, breakoff, clockType);
        }

        /// <summary>
        /// 获取考勤组班次计算
        /// </summary>
        /// <returns></returns>
        private bool GetNowRecordNew(AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, DateTime time, List<ClockRecord> clockRecord, Breakoff breakoff, bool clockType = true)
        {
            List<ClockInfo> clockInfo = new List<ClockInfo>();
            //有了原始记录先获取考勤组
            if (dayTableCalculation.ShiftType == ShiftTypeEnum.Fixed)
            {
                //是否考虑节假日
                ComputeFixedShiftNew(clockRecord, attendanceRecord, dayTableCalculation, clockInfo, breakoff, clockType);
            }
            _attendanceUnitOfWork.Update(attendanceRecord);

            if (clockInfo.Any())
            {
                _attendanceUnitOfWork.BatchDelete<ClockInfo>(s => s.AttendanceRecordID == attendanceRecord.AttendanceRecordID);
                _attendanceUnitOfWork.BatchInsert(clockInfo);
            }
            //考虑请假
            return _attendanceUnitOfWork.Commit();
        }

        /// <summary>
        /// 切割特殊情况重复部分 //后申请的为主
        /// </summary>
        /// <param name="seleaveALL"></param>
        /// <returns></returns>
        private List<Leave> GetvalidLeavesAll(List<Leave> seleaveALL)
        {
            seleaveALL = seleaveALL.OrderByDescending(s => s.StartTime).ToList();
            List<Leave> leaves = new List<Leave>
            {
                seleaveALL[0]
            };

            for (int i = 1; i < seleaveALL.Count; i++)
            {
                var leaves2 = leaves.OrderByDescending(s => s.StartTime).ToList();
                for (int j = leaves2.Count; j > 0; j--)
                {
                    if (seleaveALL[i].EndTime < leaves2[j - 1].StartTime)
                    {
                        var leave = seleaveALL[i];
                        leaves.Add(leave);
                        break;
                    }
                    if (seleaveALL[i].EndTime >= leaves2[j - 1].StartTime && seleaveALL[i].EndTime <= leaves2[j - 1].EndTime)
                    {
                        var leave = seleaveALL[i];
                        leave.EndTime = leaves[j - 1].StartTime;
                        leaves.Add(leave);
                        break;
                    }
                    if (seleaveALL[i].EndTime > leaves2[j - 1].EndTime)
                    {
                        var leave = seleaveALL[i];
                        leave.EndTime = leaves[j - 1].StartTime;
                        leaves.Add(leave);
                        seleaveALL[i].StartTime = leaves[j - 1].EndTime;
                        if (j == 1)
                        {
                            var leave2 = seleaveALL[i];
                            leave2.StartTime = leaves[j - 1].EndTime;
                            leaves.Add(leave2);
                        }
                        continue;
                    }
                }
            }
            return leaves;
        }

        /// <summary>
        /// 计算特殊情况
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="attendanceRecord"></param>
        /// <param name="shiftTime"></param>
        /// <returns></returns>
        private void ComPuteOut(Leave leave, AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, ShiftTimeManagement shiftTime, List<AttendanceItemForComputDTO> AttendanceItemDTO)
        {
            //全包含不用打卡
            if (shiftTime.StartWorkTime >= leave.StartTime && leave.EndTime >= shiftTime.EndWorkTime)
            {
                var time0 = shiftTime.EndWorkTime.ConvertTime5() - shiftTime.StartWorkTime.ConvertTime5();
                double time1 = 0;
                if (shiftTime.StartRestTime != null && shiftTime.EndRestTime != null)
                {
                    time1 = (((DateTime)shiftTime.EndRestTime).ConvertTime5() - ((DateTime)shiftTime.StartRestTime).ConvertTime5()).TotalMinutes;
                }
                var time = ((decimal)(time0.TotalMinutes - time1) / 60);
                AttendanceItemDTO.ForEach(s =>
                {
                    if (s.AttendanceItemName == "上班缺卡")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == "下班缺卡")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == "迟到")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == "早退")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "小时")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "天")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time / dayTableCalculation.WorkHours;
                    }
                });
            }
            //包含上午不包含下午
            if (shiftTime.StartWorkTime >= leave.StartTime && leave.EndTime < shiftTime.EndWorkTime)
            {
                decimal time = ((decimal)(leave.EndTime - shiftTime.StartWorkTime.ConvertTime5()).TotalMinutes) / 60;
                if (shiftTime.StartRestTime != null || shiftTime.EndRestTime != null)
                {
                    if (leave.EndTime > shiftTime.EndRestTime)
                    {
                        time = ((decimal)(leave.EndTime - shiftTime.StartWorkTime.ConvertTime5()).Subtract(((DateTime)shiftTime.EndRestTime).ConvertTime5() - ((DateTime)shiftTime.StartRestTime).ConvertTime5()).TotalMinutes) / 60;
                    }
                    if (leave.EndTime <= shiftTime.EndRestTime && leave.EndTime >= shiftTime.StartRestTime)
                    {
                        time = ((decimal)(((DateTime)shiftTime.StartRestTime).ConvertTime5() - shiftTime.StartWorkTime.ConvertTime5()).TotalMinutes) / 60;
                    }
                    if (leave.EndTime <= shiftTime.StartRestTime)
                    {
                        time = ((decimal)(leave.EndTime - shiftTime.StartWorkTime.ConvertTime5()).TotalMinutes) / 60;
                    }
                }
                AttendanceItemDTO.ForEach(s =>
                {
                    if (s.AttendanceItemName == "上班缺卡")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == "迟到")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "小时")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "天")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time / dayTableCalculation.WorkHours;
                    }
                });
            }
            //包含下午不包含上午
            if (shiftTime.StartWorkTime < leave.StartTime && leave.EndTime >= shiftTime.EndWorkTime)
            {
                decimal time = ((decimal)(shiftTime.EndWorkTime.ConvertTime5() - leave.StartTime).TotalMinutes) / 60;
                if (shiftTime.StartRestTime != null || shiftTime.EndRestTime != null)
                {
                    if (leave.StartTime < shiftTime.StartRestTime)
                    {
                        time = ((decimal)(shiftTime.EndWorkTime.ConvertTime5() - leave.StartTime).Subtract(((DateTime)shiftTime.EndRestTime).ConvertTime5() - ((DateTime)shiftTime.StartRestTime).ConvertTime5()).TotalMinutes) / 60;
                    }
                    if (leave.StartTime <= shiftTime.EndRestTime && leave.StartTime >= shiftTime.StartRestTime)
                    {
                        time = ((decimal)(shiftTime.EndWorkTime.ConvertTime5() - ((DateTime)shiftTime.EndRestTime).ConvertTime5()).TotalMinutes) / 60;
                    }
                    if (leave.StartTime > shiftTime.EndRestTime)
                    {
                        time = ((decimal)(shiftTime.EndWorkTime.ConvertTime5() - leave.StartTime).TotalMinutes) / 60;
                    }
                }

                AttendanceItemDTO.ForEach(s =>
                {
                    if (s.AttendanceItemName == "下班缺卡")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == "早退")
                    {
                        s.AttendanceItemValue = 0;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "小时")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "天")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time / dayTableCalculation.WorkHours;
                    }
                });
            }
            //都不包含
            if (shiftTime.StartWorkTime < leave.StartTime && leave.EndTime < shiftTime.EndWorkTime)
            {
                decimal time = ((decimal)(leave.EndTime - leave.StartTime).TotalMinutes) / 60;
                if (shiftTime.StartRestTime != null || shiftTime.EndRestTime != null)
                {
                    if ((leave.StartTime < shiftTime.StartRestTime && leave.EndTime < shiftTime.StartRestTime) || (leave.StartTime > shiftTime.EndRestTime && leave.EndTime > shiftTime.EndRestTime))
                    {
                        time = ((decimal)(leave.EndTime - leave.StartTime).TotalMinutes) / 60;
                    }
                    if (leave.StartTime < shiftTime.StartRestTime && leave.EndTime >= shiftTime.StartRestTime && leave.EndTime <= shiftTime.EndRestTime)
                    {
                        time = ((decimal)(((DateTime)shiftTime.StartRestTime).ConvertTime5() - leave.StartTime).TotalMinutes) / 60;
                    }
                    if (leave.StartTime < shiftTime.StartRestTime && leave.EndTime > shiftTime.EndRestTime)
                    {
                        time = ((decimal)(leave.EndTime - leave.StartTime).Subtract(((DateTime)shiftTime.EndRestTime).ConvertTime5() - ((DateTime)shiftTime.StartRestTime).ConvertTime5()).TotalMinutes) / 60;
                    }
                    if (leave.StartTime >= shiftTime.StartRestTime && leave.StartTime <= shiftTime.EndRestTime && leave.EndTime > shiftTime.EndRestTime)
                    {
                        time = ((decimal)(leave.EndTime.ConvertTime5() - ((DateTime)shiftTime.EndRestTime).ConvertTime5()).TotalMinutes) / 60;
                    }
                }
                AttendanceItemDTO.ForEach(s =>
                {
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "小时")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time;
                    }
                    if (s.AttendanceItemName == leave.LeaveName && s.Unit == "天")
                    {
                        s.AttendanceItemValue = s.AttendanceItemValue + time / dayTableCalculation.WorkHours;
                    }
                });
            }
        }
        private void ComputeAll(List<ClockRecord> clockRecord, AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, List<ClockInfo> clockInfos, Breakoff breakoff, List<AttendanceItemForComputDTO> DailyComput, List<ShiftTimeManagement> shiftTime, List<Leave> seleaveALL, string isWhat, bool clockType = true)
        {
            bool isWork = true;
            shiftTime.ForEach(s =>
            {
                ClockInfo clockInfo = NewClockInfo(s, attendanceRecord);
                var clocks = clockRecord.Where(t => t.ShiftTimeID == s.ShiftTimeID).ToList();
                List<AttendanceItemForComputDTO> attendanceItemDTO = GetItemComputDTO(attendanceRecord.CompanyID);
                ComputeUpShiftNew(clocks, clockInfo, attendanceRecord, dayTableCalculation, s, attendanceItemDTO, clockType);
                if (isWhat == "节假日加班")
                {
                    ComputeHolidayOvertime(dayTableCalculation, attendanceItemDTO);
                    isWork = false;
                }
                if (isWhat == "调休上班" || isWhat == "工作日上班")
                {
                    ComputeWorkingOvertime(attendanceRecord, clocks, s, dayTableCalculation, attendanceItemDTO, breakoff);
                }
                if (isWhat == "休息日加班")
                {
                    ComputeRestOvertime(dayTableCalculation, attendanceItemDTO);
                    isWork = false;
                }
                if (seleaveALL.Any())
                {
                    ComputeLeaveShiftNew(seleaveALL, attendanceRecord, dayTableCalculation, s, attendanceItemDTO);
                }
                CalculateAttendanceTime(attendanceItemDTO, dayTableCalculation, isWork);
                clockInfo.AttendanceItemDTOJson = JsonConvert.SerializeObject(attendanceItemDTO);
                foreach (var item in DailyComput)
                {
                    item.AttendanceItemValue = item.AttendanceItemValue + attendanceItemDTO.Where(a => a.AttendanceItemName == item.AttendanceItemName && a.Unit == item.Unit).FirstOrDefault().AttendanceItemValue;
                }
                clockInfos.Add(clockInfo);
            });
            attendanceRecord.AttendanceItemDTOJson = JsonConvert.SerializeObject(DailyComput);
            foreach (var t in DailyComput)
            {
                if ((t.AttendanceItemName == "迟到" && t.AttendanceItemValue > 0) || (t.AttendanceItemName == "早退" && t.AttendanceItemValue > 0))
                {
                    attendanceRecord.Status = ResultEnum.Abnormal;
                }
                if ((clockType && t.AttendanceItemName == "上班缺卡" && t.AttendanceItemValue > 0) || (clockType && t.AttendanceItemName == "下班缺卡" && t.AttendanceItemValue > 0))
                {
                    attendanceRecord.IsLackOfCard = true;
                    attendanceRecord.Status = ResultEnum.Abnormal;
                }
                if (t.AttendanceItemName == "迟到" && t.Unit == "分钟数")
                {
                    attendanceRecord.LateMinutes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "迟到" && t.Unit == "次数")
                {
                    attendanceRecord.LateTimes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "早退" && t.Unit == "分钟数")
                {
                    attendanceRecord.EarlyLeaveMinutes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "早退" && t.Unit == "次数")
                {
                    attendanceRecord.EarlyLeaveTimes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "上班缺卡" && t.Unit == "次数")
                {
                    attendanceRecord.NotClockInTimes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "下班缺卡" && t.Unit == "次数")
                {
                    attendanceRecord.NotClockOutTimes = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "外出" && t.Unit == "小时")
                {
                    attendanceRecord.OutsideDuration = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemName == "出差" && t.Unit == "天")
                {
                    attendanceRecord.BusinessTripDuration = (double)t.AttendanceItemValue;
                }
                if (t.AttendanceItemCatagoryName == "请假考勤项" && t.Unit == "小时")
                {
                    attendanceRecord.LeaveDuration += (double)t.AttendanceItemValue;
                }
            }
        }

        /// <summary>
        /// 计算固定班次
        /// </summary>
        /// <returns></returns>
        private void ComputeFixedShiftNew(List<ClockRecord> clockRecord, AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, List<ClockInfo> clockInfos, Breakoff breakoff, bool clockType = true)
        {
            bool clockRecordAny = clockRecord.Any();
            List<Leave> seleaveALL = _workPaidLeaveRepository.EntityQueryable<Leave>(s => s.IDCard == attendanceRecord.IDCard && s.StartTime.Date <= attendanceRecord.AttendanceDate && s.EndTime >= attendanceRecord.AttendanceDate).ToList().Distinct(new ListComparer<Leave>((p1, p2) => p1.IDCard == p2.IDCard && p1.StartTime == p2.StartTime && p1.EndTime == p2.EndTime && p1.CompanyID == p2.CompanyID)).ToList();
            if (seleaveALL.Any())
            {
                seleaveALL = GetvalidLeavesAll(seleaveALL);
            }
            List<AttendanceItemForComputDTO> DailyComput = GetItemComputDTO(attendanceRecord.CompanyID);
            List<ShiftTimeManagement> shiftTime = JsonConvert.DeserializeObject<List<ShiftTimeManagement>>(dayTableCalculation.ShiftTimeManagementList);
            attendanceRecord.Status = ResultEnum.Normal;
            attendanceRecord.IsLackOfCard = false;
            //节假日
            if (dayTableCalculation.IsDynamicRowHugh && dayTableCalculation.IsHoliday)
            {
                //判断是否有审批  有审批说明加班，没有说明是乱打的不计
                if (clockRecordAny)
                {
                    //加班
                    attendanceRecord.Shift = "加班";
                    ComputeAll(clockRecord, attendanceRecord, dayTableCalculation, clockInfos, breakoff, DailyComput, shiftTime, seleaveALL, "节假日加班", clockType);
                }
                return;
            }
            //调休上班
            if (dayTableCalculation.IsDynamicRowHugh && dayTableCalculation.IsWorkPaidLeave)
            {
                ComputeAll(clockRecord, attendanceRecord, dayTableCalculation, clockInfos, breakoff, DailyComput, shiftTime, seleaveALL, "调休上班", clockType);
                return;
            }
            if (!dayTableCalculation.IsHolidayWork)
            {
                if (clockRecordAny)
                {
                    attendanceRecord.Shift = "加班";
                    ComputeAll(clockRecord, attendanceRecord, dayTableCalculation, clockInfos, breakoff, DailyComput, shiftTime, seleaveALL, "休息日加班", clockType);
                }
                return;
            }
            if (dayTableCalculation.IsHolidayWork)
            {
                ComputeAll(clockRecord, attendanceRecord, dayTableCalculation, clockInfos, breakoff, DailyComput, shiftTime, seleaveALL, "工作日上班", clockType);
                return;
            }
        }
        /// <summary>
        /// 计算节假日加班
        /// </summary>
        /// <param name="dayTableCalculation"></param>
        /// <param name="attendanceItemDTO"></param>
        private void ComputeHolidayOvertime(DayTableCalculation dayTableCalculation, List<AttendanceItemForComputDTO> attendanceItemDTO)
        {
            if (dayTableCalculation.HolidayCalculationMethod == CalculationMethodEnum.ApprovalTime)
            {
                return;
            }
            if (dayTableCalculation.HolidayCalculationMethod == CalculationMethodEnum.NotAllow)
            {
                return;
            }
        }
        /// <summary>
        /// 计算工作日加班
        /// </summary>
        /// <param name="clocks"></param>
        /// <param name="shiftTime"></param>
        /// <param name="dayTableCalculation"></param>
        /// <param name="attendanceItemDTO"></param>
        private void ComputeWorkingOvertime(AttendanceRecord attendanceRecord, List<ClockRecord> clocks, ShiftTimeManagement shiftTime, DayTableCalculation dayTableCalculation, List<AttendanceItemForComputDTO> attendanceItemDTO, Breakoff breakoff)
        {
            if (dayTableCalculation.WorkingCalculationMethod == CalculationMethodEnum.ApprovalTime)
            {
                return;
            }
            if (dayTableCalculation.WorkingCalculationMethod == CalculationMethodEnum.ClockInTimeOfApprovalPeriod)
            {
                return;
            }
            if (dayTableCalculation.WorkingCalculationMethod == CalculationMethodEnum.PunchTime)
            {
                if (dayTableCalculation.IsExemption)
                {
                    shiftTime.EndWorkTime = shiftTime.EndWorkTime.AddMinutes(dayTableCalculation.EarlyLeaveMinutes);
                }
                var lastClock = clocks.LastOrDefault();
                if (lastClock == null)
                {
                    return;
                }
                var overtimeStartTime = shiftTime.EndWorkTime.AddMinutes(dayTableCalculation.ExcludingOvertime);
                var overtime = lastClock.ClockTime.ConvertTime6(lastClock.AttendanceDate).Subtract(overtimeStartTime).TotalMinutes;
                if (overtime < dayTableCalculation.MinimumOvertime)
                {
                    overtime = 0;
                }
                if (overtime > dayTableCalculation.LongestOvertime)
                {
                    overtime = dayTableCalculation.LongestOvertime;
                }
                attendanceItemDTO.Where(s => s.AttendanceItemName == "工作日加班" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue = (decimal)overtime / 60;
                if (dayTableCalculation.WorkingCompensationMode == CompensationModeEnum.PlanForRest)
                {
                    if (breakoff == null)//第一次添加
                    {
                        breakoff = new Breakoff
                        {
                            CompanyID = attendanceRecord.CompanyID,
                            IDCard = attendanceRecord.IDCard,
                            SurplusAmount = 0,
                            TotalSettlement = 0
                        };
                        breakoff.SurplusAmount = breakoff.SurplusAmount + (decimal)overtime / 60 / dayTableCalculation.WorkHours;
                        BreakoffDetail breakoffDetail = new BreakoffDetail
                        {
                            ChangeTime = (decimal)overtime / 60 / dayTableCalculation.WorkHours,
                            ChangeType = ChangeTypeEnum.Add,
                            CompanyID = attendanceRecord.CompanyID,
                            IDCard = breakoff.IDCard,
                            CurrentQuota = breakoff.SurplusAmount,
                            Remark = "系统自动增加",
                            AttendanceRecordID = attendanceRecord.AttendanceRecordID
                        };
                        _attendanceUnitOfWork.Add(breakoffDetail);
                        _attendanceUnitOfWork.Add(breakoff);
                    }
                    else//不是第一次添加（但是有可能是第一次计算）
                    {
                        var breakoffdetail = _breakoffDetailRepository.EntityQueryable<BreakoffDetail>(s => s.AttendanceRecordID == attendanceRecord.AttendanceRecordID);
                        if (breakoffdetail.Any())//这是执行过的结果需要修改
                        {
                            var breakoffdetailitem = breakoffdetail.FirstOrDefault();
                            breakoff.SurplusAmount = breakoff.SurplusAmount - breakoffdetailitem.ChangeTime + (decimal)overtime / 60 / dayTableCalculation.WorkHours;
                            breakoffdetailitem.ChangeTime = (decimal)overtime / 60 / dayTableCalculation.WorkHours;
                            _attendanceUnitOfWork.Update(breakoffdetailitem);
                            _attendanceUnitOfWork.Update(breakoff);
                        }
                        else//这是没执行的，直接编辑+新增
                        {
                            breakoff.SurplusAmount = breakoff.SurplusAmount + (decimal)overtime / 60 / dayTableCalculation.WorkHours;
                            BreakoffDetail breakoffDetail = new BreakoffDetail
                            {
                                ChangeTime = (decimal)overtime / 60 / dayTableCalculation.WorkHours,
                                ChangeType = ChangeTypeEnum.Add,
                                CompanyID = attendanceRecord.CompanyID,
                                IDCard = breakoff.IDCard,
                                CurrentQuota = breakoff.SurplusAmount,
                                Remark = "系统自动增加",
                                AttendanceRecordID = attendanceRecord.AttendanceRecordID
                            };
                            _attendanceUnitOfWork.Add(breakoffDetail);
                            _attendanceUnitOfWork.Update(breakoff);
                        }
                    }
                }
            }
            if (dayTableCalculation.WorkingCalculationMethod == CalculationMethodEnum.NotAllow)
            {
                return;
            }
        }
        /// <summary>
        /// 计算休息日加班
        /// </summary>
        /// <param name="dayTableCalculation"></param>
        /// <param name="attendanceItemDTO"></param>
        private void ComputeRestOvertime(DayTableCalculation dayTableCalculation, List<AttendanceItemForComputDTO> attendanceItemDTO)
        {
            if (dayTableCalculation.RestCalculationMethod == CalculationMethodEnum.ApprovalTime)
            {
                return;
            }
            if (dayTableCalculation.RestCalculationMethod == CalculationMethodEnum.NotAllow)
            {
                return;
            }
        }

        /// <summary>
        /// 创建一个新的打卡信息对象
        /// </summary>
        /// <param name="s"></param>
        /// <param name="attendanceRecord"></param>
        /// <returns></returns>
        private ClockInfo NewClockInfo(ShiftTimeManagement s, AttendanceRecord attendanceRecord)
        {
            return new ClockInfo
            {
                AttendanceRecordID = attendanceRecord.AttendanceRecordID,
                ShiftTimeID = s.ShiftTimeID,
                ClockInTime = new DateTime(),
                ClockInResult = ClockResultEnum.WorkCardMissing,
                StartLocation = "--",
                ClockOutTime = new DateTime(),
                ClockOutResult = ClockResultEnum.LackOfWorkCard,
                EndLocation = "--"
            };
        }

        /// <summary>
        /// 统计迟到早退到ItemDto
        /// </summary>
        /// <param name="attendanceItemDTO"></param>
        /// <param name="dayTableCalculation"></param>
        /// <param name="IsWork"></param>
        private void CalculateAttendanceTime(List<AttendanceItemForComputDTO> attendanceItemDTO, DayTableCalculation dayTableCalculation, bool IsWork)
        {
            decimal noAttendanceTime = 0;
            attendanceItemDTO.ForEach(t =>
            {
                if ((t.AttendanceItemName == "迟到" && t.Unit == "分钟数") || (t.AttendanceItemName == "早退" && t.Unit == "分钟数"))
                {
                    noAttendanceTime = noAttendanceTime + (t.AttendanceItemValue / 60);
                }
                if (t.AttendanceItemCatagoryName == "请假考勤项" && t.Unit == "小时")
                {
                    noAttendanceTime = noAttendanceTime + t.AttendanceItemValue;
                }
            });

            if (IsWork)
            {
                attendanceItemDTO.ForEach(t =>
                {
                    if (t.AttendanceItemName == "应出勤时长" && t.Unit == "小时")
                    {
                        t.AttendanceItemValue = dayTableCalculation.WorkHours;
                    }
                    if (t.AttendanceItemName == "应出勤时长" && t.Unit == "天")
                    {
                        t.AttendanceItemValue = 1;
                    }
                    if (t.AttendanceItemName == "实出勤时长" && t.Unit == "小时")
                    {
                        t.AttendanceItemValue = dayTableCalculation.WorkHours - noAttendanceTime;
                    }
                    if (t.AttendanceItemName == "实出勤时长" && t.Unit == "天")
                    {
                        t.AttendanceItemValue = (dayTableCalculation.WorkHours - noAttendanceTime) / dayTableCalculation.WorkHours;
                    }
                });
            }
        }

        /// <summary>
        /// 具体计算班次
        /// </summary>
        /// <returns></returns>
        private void ComputeUpShiftNew(List<ClockRecord> clockRecord, ClockInfo clockInfo, AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, ShiftTimeManagement shiftTime, List<AttendanceItemForComputDTO> AttendanceItemDTO, bool clockType = true)
        {
            if (dayTableCalculation.IsExemption)
            {
                shiftTime.StartWorkTime = shiftTime.StartWorkTime.AddMinutes(dayTableCalculation.LateMinutes);
                shiftTime.EndWorkTime = shiftTime.EndWorkTime.AddMinutes(-dayTableCalculation.EarlyLeaveMinutes);
            }
            var upwork = clockRecord.Where(t => t.ClockType == ClockTypeEnum.Working);
            //如果上班有记录
            if (upwork.Any())
            {
                //取记录第一条
                var clock = upwork.FirstOrDefault();
                var ClockTime = clock.ClockTime.ConvertTime();
                if (clock.IsAcross)
                {
                    ClockTime = ClockTime.AddDays(1);
                }
                if (ClockTime > shiftTime.StartWorkTime)//判断迟到
                {
                    ComputeFlexibleNew(attendanceRecord, clockInfo, dayTableCalculation, shiftTime, clock, AttendanceItemDTO);
                }
                else
                {
                    clockInfo.ClockInTime = clock.ClockTime;
                    clockInfo.StartLocation = clock.Location;
                    clockInfo.ClockInResult = ClockResultEnum.Normal;
                }
            }
            else
            {
                AttendanceItemDTO.Where(s => s.AttendanceItemName == "上班缺卡" && s.Unit == "次数").FirstOrDefault().AttendanceItemValue++;
            }
            var downwork = clockRecord.Where(t => t.ClockType == ClockTypeEnum.GetOffWork);
            if (downwork.Any() && clockType)
            {
                //取记录第一条
                var clock = downwork.LastOrDefault();
                clockInfo.ClockOutTime = clock.ClockTime;
                clockInfo.EndLocation = clock.Location;

                var clockTime = clock.ClockTime.ConvertTime();
                if (clock.IsAcross)
                {
                    clockTime = clockTime.AddDays(1);
                }
                if (clockTime >= shiftTime.EndWorkTime.ConvertTime3())
                {
                    //正常
                    clockInfo.ClockOutResult = ClockResultEnum.Normal;
                }
                else
                {
                    //早退
                    clockInfo.ClockOutResult = ClockResultEnum.EarlyLeave;
                    AttendanceItemDTO.Where(s => s.AttendanceItemCatagoryName == "基本考勤项").ToList().ForEach(s =>
                    {
                        if (s.AttendanceItemName == "早退" && s.Unit == "分钟数")
                        {
                            s.AttendanceItemValue = (decimal)shiftTime.EndWorkTime.ConvertTime3().Subtract(clockTime).TotalMinutes;
                        }
                        if (s.AttendanceItemName == "早退" && s.Unit == "次数")
                        {
                            s.AttendanceItemValue++;
                        }
                    });
                }
            }
            else
            {
                AttendanceItemDTO.Where(s => s.AttendanceItemName == "下班缺卡" && s.Unit == "次数").FirstOrDefault().AttendanceItemValue++;
            }
            if (upwork.Any() && downwork.Any())
            {
                var clockup = upwork.FirstOrDefault();
                var clockdown = downwork.LastOrDefault();
                var WorkingTime = clockdown.ClockTime.ConvertTime5().Subtract(clockup.ClockTime.ConvertTime5());
                if (shiftTime.EndRestTime != null && shiftTime.StartRestTime != null)
                {
                    WorkingTime = WorkingTime.Subtract(((DateTime)shiftTime.EndRestTime).ConvertTime5() - ((DateTime)shiftTime.StartRestTime).ConvertTime5());
                }
                attendanceRecord.WorkingTime = WorkingTime.TotalHours;
            }
            return;
        }

        /// <summary>
        /// 处理时间,计算特殊情况
        /// </summary>
        /// <param name="leave"></param>
        /// <param name="attendanceRecord"></param>
        /// <param name="shiftManagement"></param>
        /// <param name="type">是否是加班</param>
        /// <returns></returns>
        private void ComputeLeaveShiftNew(List<Leave> leaves, AttendanceRecord attendanceRecord, DayTableCalculation dayTableCalculation, ShiftTimeManagement shiftTime, List<AttendanceItemForComputDTO> AttendanceItemDTO)
        {
            if (shiftTime.StartRestTime != null)
            {
                shiftTime.StartRestTime = attendanceRecord.AttendanceDate.AddTime((DateTime)shiftTime.StartRestTime);
            }
            if (shiftTime.EndRestTime != null)
            {
                shiftTime.EndRestTime = attendanceRecord.AttendanceDate.AddTime((DateTime)shiftTime.EndRestTime);
            }
            shiftTime.StartWorkTime = attendanceRecord.AttendanceDate.AddTime(shiftTime.StartWorkTime);
            shiftTime.EndWorkTime = attendanceRecord.AttendanceDate.AddTime(shiftTime.EndWorkTime);
            foreach (var leave in leaves)
            {
                ComPuteOut(leave, attendanceRecord, dayTableCalculation, shiftTime, AttendanceItemDTO);
            }
        }

        /// <summary>
        /// 计算弹性上班时间的弹性时间
        /// </summary>
        /// <param name="attendanceRecord"></param>
        /// <param name="shiftTime"></param>
        /// <param name="clock"></param>
        /// <param name="shift"></param>
        /// <param name="startWorkTime"></param>
        /// <param name="flexible"></param>
        /// <returns></returns>
        private void ComputeFlexibleNew(AttendanceRecord attendanceRecord, ClockInfo clockInfo, DayTableCalculation dayTableCalculation, ShiftTimeManagement shiftTime, ClockRecord clock, List<AttendanceItemForComputDTO> AttendanceItemDTO)
        {

            DateTime clocktime = clock.ClockTime.ConvertTime();
            clockInfo.ClockInTime = clock.ClockTime;
            clockInfo.StartLocation = clock.Location;
            if (dayTableCalculation.IsFlexible)
            {
                DateTime StartWorkTime = shiftTime.StartWorkTime;
                StartWorkTime = StartWorkTime.AddMinutes(dayTableCalculation.FlexibleMinutes);
                DateTime EndWorkTime = shiftTime.EndWorkTime;
                EndWorkTime = EndWorkTime.AddMinutes(dayTableCalculation.FlexibleMinutes);
                if (clocktime > StartWorkTime)//判断弹性迟到
                {
                    shiftTime.EndWorkTime = EndWorkTime;
                    shiftTime.StartWorkTime = StartWorkTime;
                    clockInfo.ClockInResult = ClockResultEnum.Late;
                    AttendanceItemDTO.Where(s => s.AttendanceItemCatagoryName == "基本考勤项").ToList().ForEach(s =>
                    {
                        if (s.AttendanceItemName == "迟到" && s.Unit == "分钟数")
                        {
                            s.AttendanceItemValue = (decimal)clocktime.Subtract(shiftTime.StartWorkTime.ConvertTime3()).TotalMinutes;
                        }
                        if (s.AttendanceItemName == "迟到" && s.Unit == "次数")
                        {
                            s.AttendanceItemValue++;
                        }
                    });
                }
                if (clocktime <= StartWorkTime)
                {
                    var time = clocktime.ConvertTime3() - shiftTime.StartWorkTime.ConvertTime3();
                    shiftTime.StartWorkTime = shiftTime.StartWorkTime.AddMinutes(time.TotalMinutes);
                    shiftTime.EndWorkTime = shiftTime.EndWorkTime.AddMinutes(time.TotalMinutes);
                    clockInfo.ClockInResult = ClockResultEnum.Normal;
                }
            }
            else
            {
                //迟到
                clockInfo.ClockInResult = ClockResultEnum.Late;
                AttendanceItemDTO.Where(s => s.AttendanceItemCatagoryName == "基本考勤项").ToList().ForEach(s =>
                {
                    if (s.AttendanceItemName == "迟到" && s.Unit == "分钟数")
                    {
                        s.AttendanceItemValue = (decimal)clocktime.Subtract(shiftTime.StartWorkTime.ConvertTime3()).TotalMinutes;
                    }
                    if (s.AttendanceItemName == "迟到" && s.Unit == "次数")
                    {
                        s.AttendanceItemValue++;
                    }
                });
            }
            return;
        }



        //public void AutoBreakOff(DateTime time)
        //{
        //    var workingBreakOff = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.Breakoff == BreakoffEnum.Working && s.AttendanceDate == time.Date).GroupBy(s => s.CompanyID);
        //    foreach (var item in workingBreakOff)
        //    {
        //        if (workingBreakOff.Any())
        //        {
        //            var workingBreakOffList = item.ToList();
        //            var breakoffList = _breakoffRepository.EntityQueryable<Breakoff>(s => s.CompanyID == workingBreakOffList.FirstOrDefault().CompanyID).ToList();
        //            for (int i = 0; i < workingBreakOffList.Count; i++)
        //            {
        //                workingBreakOffList[i].Breakoff = BreakoffEnum.Processed;
        //                var itemvalue = JsonConvert.DeserializeObject<List<AttendanceItemForComputDTO>>(workingBreakOffList[i].AttendanceItemDTOJson).Where(s => s.AttendanceItemName == "工作日加班" && s.Unit == "小时").FirstOrDefault().AttendanceItemValue/;
        //                var breakitem = breakoffList.Where(s => s.IDCard == workingBreakOffList[i].IDCard).FirstOrDefault();
        //                if (breakitem == null)
        //                {
        //                    breakitem = new Breakoff
        //                    {
        //                        IDCard = workingBreakOffList[i].IDCard,
        //                        CompanyID = workingBreakOffList[i].CompanyID,
        //                        SurplusAmount = 0,
        //                        TotalSettlement = 0,
        //                    };
        //                }
        //                breakitem.SurplusAmount = breakitem.SurplusAmount+itemvalue
        //                BreakoffDetail breakoffDetail = new BreakoffDetail
        //                {
        //                    AttendanceRecordID = workingBreakOffList[i].AttendanceRecordID,
        //                    ChangeTime = itemvalue,
        //                    ChangeType = ChangeTypeEnum.Add,
        //                    CompanyID = workingBreakOffList[i].CompanyID,
        //                    CurrentQuota = 0,
        //                    IDCard = workingBreakOffList[i].IDCard,
        //                    Remark = "系统自动增加",
        //                };
        //            }
        //            _attendanceUnitOfWork.BatchUpdate<AttendanceRecord>(s => new AttendanceRecord { Breakoff = BreakoffEnum.Processed }, s => s.Breakoff == BreakoffEnum.Working && s.AttendanceDate == time.Date);
        //        }
        //    }
        //    var restBreakOff = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.Breakoff == BreakoffEnum.Rest && s.AttendanceDate == time.Date).ToList();
        //    if (restBreakOff.Any())
        //    {
        //        var RestBreakOffList = restBreakOff.ToList();
        //        _attendanceUnitOfWork.BatchUpdate<AttendanceRecord>(s => new AttendanceRecord { Breakoff = BreakoffEnum.Processed }, s => s.Breakoff == BreakoffEnum.Rest && s.AttendanceDate == time.Date);
        //    }
        //    var holidayBreakOff = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.Breakoff == BreakoffEnum.Holidays && s.AttendanceDate == time.Date).ToList();
        //    if (holidayBreakOff.Any())
        //    {
        //        var holidayBreakOffList = holidayBreakOff.ToList();
        //        _attendanceUnitOfWork.BatchUpdate<AttendanceRecord>(s => new AttendanceRecord { Breakoff = BreakoffEnum.Processed }, s => s.Breakoff == BreakoffEnum.Holidays && s.AttendanceDate == time.Date);
        //    }
        //    try
        //    {
        //        var res = _attendanceUnitOfWork.Commit();
        //        if (!res)
        //        {
        //            _httpRequestHelp.RequestGet("https://sc.ftqq.com/SCU50608T4d004dff917249bfb7d7fd5f3000c9745ccd6ed0741a2.send?text=提交调休失败&&desp=");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _httpRequestHelp.RequestGet("https://sc.ftqq.com/SCU50608T4d004dff917249bfb7d7fd5f3000c9745ccd6ed0741a2.send?text=提交调休失败&&desp=" + ex.ToString());
        //    }
        //}
        #endregion

        public bool DataUpdate()
        {
            var time = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate == time.Date).Select(s => new { s.IDCard, s.AttendanceRecordID }).ToList();
                var recordgroup = record.GroupBy(s => s.IDCard).ToList();

                recordgroup.ForEach(s =>
                {
                    int num = s.Count();
                    if (num > 1)
                    {
                        var res = s.LastOrDefault().AttendanceRecordID;
                        _attendanceUnitOfWork.BatchDelete<AttendanceRecord>(j => j.AttendanceRecordID == res);
                    }
                });
                time = time.AddDays(-1);
            }
            return _attendanceUnitOfWork.Commit();
        }
        /// <summary>
        /// 更改组织数据错误
        /// </summary>
        /// <returns></returns>
        public bool DataUpdateOr(DateTime dateTime)
        {
            var person = _personRepository.EntityQueryable<Person>(s => true).ToDictionary(s => s.IDCard);
            var record = _attendanceRecordRepository.EntityQueryable<AttendanceRecord>(s => s.AttendanceDate == dateTime.Date).ToList();
            var clock = _clockRecordRepository.EntityQueryable<ClockRecord>(s => s.AttendanceDate == dateTime.Date).ToDictionary(s => s.IDCard);
            foreach (var item in record)
            {
                if (person.TryGetValue(item.IDCard, out Person pe))
                {
                    item.DepartmentID = pe.DepartmentID;
                    item.CustomerID = pe.CustomerID;
                    _attendanceUnitOfWork.Update(item);
                    if (clock.TryGetValue(item.IDCard, out ClockRecord co))
                    {
                        co.DepartmentID = pe.DepartmentID;
                        co.CustomerID = pe.CustomerID;
                        _attendanceUnitOfWork.Update(co);
                    }
                }
            }
            return _attendanceUnitOfWork.Commit();
        }
    }
}

