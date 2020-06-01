using AutoMapper;
using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Service
{
    public class AttendanceRuleService : BaseService, IAttendanceRuleService
    {
        private readonly IAttendanceGroupRepository _attendanceGroupRepository;
        private readonly IAttendanceRuleDetailRepository _attendanceRuleDetailRepository;
        private readonly IAttendanceRuleRepository _attendanceRuleRepository;
        public AttendanceRuleService(IAttendanceUnitOfWork salaryUnitOfWork, IAttendanceRuleRepository attendanceRuleRepository, IMapper mapper, IAttendanceGroupRepository attendanceGroupRepository, IAttendanceRuleDetailRepository attendanceRuleDetailRepository, ISerializer<string> serializer) : base(salaryUnitOfWork, mapper, serializer)
        {
            _attendanceRuleRepository = attendanceRuleRepository;
            _attendanceRuleDetailRepository = attendanceRuleDetailRepository;
            _attendanceGroupRepository = attendanceGroupRepository;
        }
        /// <summary>
        /// 新增考勤规则于详情
        /// </summary>
        /// <param name="attendanceRuleJson"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string) AddAttendanceRule(AttendanceRuleParamDTO attendanceRuleJson, FyuUser user)
        {
            var attendanceRule = new AttendanceRule
            {
                AttendanceRuleID = Guid.NewGuid().ToString(),
                CompanyID = user.customerId,
                CompanyName = user.customerName,
                CreateTime = DateTime.Now,
                Creator = user.realName,
                CreatorID = user.userId,
                Remark = attendanceRuleJson.Remark,
                RuleName = attendanceRuleJson.RuleName,
                NotClockRule = (NotClockEnum)attendanceRuleJson.NotClockRule,
                EarlyLeaveRule = (EarlyLeaveEnum)attendanceRuleJson.EarlyLeaveRule,
                LateRule = (LateEnum)attendanceRuleJson.LateRule,
            };
            _attendanceUnitOfWork.Add(attendanceRule);
            var attendanceRuleDetail = new List<AttendanceRuleDetail>();
            int i = 0;
            if (attendanceRuleJson.LateRule != 0)
            {
                attendanceRuleJson.Late.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.LateMaxJudge,
                        MaxTime = s.LateMaxTime,
                        MinJudge = (SymbolEnum)s.LateMinJudge,
                        MinTime = s.LateMinTime,
                        RuleType = RuleTypeEnum.Late,
                        Time = s.Time,
                        Unit = s.CallRuleType== 1? UnitEnum.Minute:UnitEnum.Hour,
                        Sort = i++,
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleJson.EarlyLeaveRule != 0)
            {
                attendanceRuleJson.EarlyLeave.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.EarlyLeaveMaxJudge,
                        MaxTime = s.EarlyLeaveMaxTime,
                        MinJudge = (SymbolEnum)s.EarlyLeaveMinJudge,
                        MinTime = s.EarlyLeaveMinTime,
                        RuleType = RuleTypeEnum.EarlyLeave,
                        Time = s.Time,
                        Unit = s.CallRuleType == 1 ? UnitEnum.Minute : UnitEnum.Hour,
                        Sort = i++
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleJson.NotClockRule != 0)
            {
                attendanceRuleJson.NotClock.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.NotClockMaxJudge,
                        MaxTime = s.NotClockMaxTime,
                        MinJudge = (SymbolEnum)s.NotClockMinJudge,
                        MinTime = s.NotClockMinTime,
                        RuleType = RuleTypeEnum.NotClock,
                        Time = s.Time,
                        Unit = s.CallRuleType == 1 ? UnitEnum.Minute : UnitEnum.Hour,
                        Sort = i++
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleDetail.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(attendanceRuleDetail);
            }
            return _attendanceUnitOfWork.Commit() ? (true, "添加成功") : (false, "保存失败,请联系管理员");
        }
        /// <summary>
        /// 删除考勤规则(考勤规则被使用次数大于0的时候无法被删除)
        /// </summary>
        /// <param name="attendanceRuleID"></param>
        /// <returns></returns>
        public (bool, string) DeleteAttendanceRule(string attendanceRuleID)
        {
            if (_attendanceGroupRepository.EntityQueryable<AttendanceGroup>(s => s.AttendanceRuleID == attendanceRuleID,true).Any())
            {
                return (false, "该考勤规则已被考勤组被使用，无法删除");
            }
            var Rule = _attendanceRuleRepository.EntityQueryable<AttendanceRule>(s => s.AttendanceRuleID == attendanceRuleID).FirstOrDefault();
            if (Rule == null)
            {
                return (false, "该考勤规则已不存在，刷新页面重试");
            }
            if (!Rule.IsUsed)
            {
                _attendanceUnitOfWork.BatchDelete<AttendanceRule>(s => s.AttendanceRuleID == attendanceRuleID);
                _attendanceUnitOfWork.BatchDelete<AttendanceRuleDetail>(s => s.AttendanceRuleID == attendanceRuleID);
            }
            _attendanceUnitOfWork.BatchUpdate<AttendanceRule>(s => new AttendanceRule() { IsDelete = true }, s => s.AttendanceRuleID == attendanceRuleID);
            return _attendanceUnitOfWork.Commit() ? (true, "删除成功") : (false, "服务器错误，删除失败");
        }
        /// <summary>
        /// 获取考勤规则分页数据
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="ruleName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public PageResult<AttendanceRuleDTO> GetAttendanceRulePage(int pageIndex, int pageSize, string ruleName, FyuUser user)
        {
            Expression<Func<AttendanceRule, bool>> exp = s => s.CompanyID == user.customerId&&!s.IsDelete;
            if (!string.IsNullOrEmpty(ruleName))
            {
                exp = s => s.RuleName.Contains(ruleName) && s.CompanyID == user.customerId;
            }
            var Rule = _attendanceRuleRepository.GetByPage(pageIndex, pageSize, exp, s => s.CreateTime, SortOrderEnum.Descending);
            return PageMap<AttendanceRuleDTO, AttendanceRule>(Rule);
        }
        public List<AttendanceRuleDTO> GetAttendanceRules(FyuUser user)
        {
            var rules = _attendanceRuleRepository.EntityQueryable<AttendanceRule>(a => a.CompanyID == user.customerId&&!a.IsDelete,true).ToList();
            return _mapper.Map<List<AttendanceRuleDTO>>(rules);
        }
        /// <summary>
        /// 修改考勤规则以及详情
        /// </summary>
        /// <param name="attendanceRuleJson"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public (bool, string) UpdateAttendanceRule(AttendanceRuleUpdateDTO attendanceRuleJson)
        {
            var attendanceRule = _attendanceRuleRepository.GetEntity(s => s.AttendanceRuleID == attendanceRuleJson.AttendanceRuleID);
            if (attendanceRule == null)
            {
                return (false, "状态已被更改,刷新页面重试");
            }
            attendanceRule.Remark = attendanceRuleJson.Remark;
            attendanceRule.RuleName = attendanceRuleJson.RuleName;
            attendanceRule.NotClockRule = (NotClockEnum)attendanceRuleJson.NotClockRule;
            attendanceRule.EarlyLeaveRule = (EarlyLeaveEnum)attendanceRuleJson.EarlyLeaveRule;
            attendanceRule.LateRule = (LateEnum)attendanceRuleJson.LateRule;
            _attendanceUnitOfWork.Update(attendanceRule);
            _attendanceUnitOfWork.BatchDelete<AttendanceRuleDetail>(s => s.AttendanceRuleID == attendanceRule.AttendanceRuleID);
            var attendanceRuleDetail = new List<AttendanceRuleDetail>();
            int i = 0;
            if (attendanceRuleJson.LateRule != 0)
            {
                attendanceRuleJson.Late.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.LateMaxJudge,
                        MaxTime = s.LateMaxTime,
                        MinJudge = (SymbolEnum)s.LateMinJudge,
                        MinTime = s.LateMinTime,
                        RuleType = RuleTypeEnum.Late,
                        Time = s.Time,
                        Unit = s.CallRuleType == 1 ? UnitEnum.Minute : UnitEnum.Hour,
                        Sort = i++,
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleJson.EarlyLeaveRule != 0)
            {
                attendanceRuleJson.EarlyLeave.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.EarlyLeaveMaxJudge,
                        MaxTime = s.EarlyLeaveMaxTime,
                        MinJudge = (SymbolEnum)s.EarlyLeaveMinJudge,
                        MinTime = s.EarlyLeaveMinTime,
                        RuleType = RuleTypeEnum.EarlyLeave,
                        Time = s.Time,
                        Unit = s.CallRuleType == 2 ? UnitEnum.Minute : UnitEnum.Hour,
                        Sort = i++,
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleJson.NotClockRule != 0)
            {
                attendanceRuleJson.NotClock.ForEach(s =>
                {
                    var detail = new AttendanceRuleDetail
                    {
                        AttendanceRuleDetailID = Guid.NewGuid().ToString(),
                        AttendanceRuleID = attendanceRule.AttendanceRuleID,
                        CallRuleType = (RuleTypeEnum)s.CallRuleType,
                        MaxJudge = (SymbolEnum)s.NotClockMaxJudge,
                        MaxTime = s.NotClockMaxTime,
                        MinJudge = (SymbolEnum)s.NotClockMinJudge,
                        MinTime = s.NotClockMinTime,
                        RuleType = RuleTypeEnum.NotClock,
                        Time = s.Time,
                        Unit = s.CallRuleType == 3 ? UnitEnum.times : UnitEnum.Hour,
                        Sort = i++,
                    };
                    attendanceRuleDetail.Add(detail);
                });
            };
            if (attendanceRuleDetail.Count > 0)
            {
                _attendanceUnitOfWork.BatchInsert(attendanceRuleDetail);
            }
            return _attendanceUnitOfWork.Commit() ? (true, "保存成功") : (false, "保存失败,请联系管理员");
        }
        public AttendanceRuleUpdateDTO GetUpdateDetail(string attendanceRuleID)
        {
            var rule = _attendanceRuleRepository.EntityQueryable<AttendanceRule>(s => s.AttendanceRuleID == attendanceRuleID,true).FirstOrDefault();
            var ruleDetail = _attendanceRuleDetailRepository.EntityQueryable<AttendanceRuleDetail>(s => s.AttendanceRuleID == attendanceRuleID,true).ToList();
            var groupDTO = _mapper.Map<AttendanceRuleUpdateDTO>(rule);
            if (rule.LateRule != LateEnum.NoDisposal)
            {
                groupDTO.Late = _mapper.Map<List<LateParam>>(ruleDetail.Where(s => s.RuleType == RuleTypeEnum.Late).OrderBy(s => s.Sort));
            }
            if (rule.EarlyLeaveRule != EarlyLeaveEnum.NoDisposal)
            {
                groupDTO.EarlyLeave = _mapper.Map<List<EarlyLeaveParam>>(ruleDetail.Where(s => s.RuleType == RuleTypeEnum.EarlyLeave).OrderBy(s => s.Sort));
            }
            if (rule.NotClockRule != NotClockEnum.NoDisposal)
            {
                groupDTO.NotClock = _mapper.Map<List<NotClockParam>>(ruleDetail.Where(s => s.RuleType == RuleTypeEnum.NotClock).OrderBy(s => s.Sort));
            }
            return groupDTO;
        }
    }
}
