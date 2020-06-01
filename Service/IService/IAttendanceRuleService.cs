using Domain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IAttendanceRuleService : IService
    {
        PageResult<AttendanceRuleDTO> GetAttendanceRulePage(int pageIndex, int pageSize, string ruleName, FyuUser user);
        (bool, string) DeleteAttendanceRule(string attendanceRuleID);
        (bool,string) AddAttendanceRule(AttendanceRuleParamDTO attendanceRuleJson, FyuUser user);
        (bool,string) UpdateAttendanceRule(AttendanceRuleUpdateDTO attendanceRuleJson);
        AttendanceRuleUpdateDTO GetUpdateDetail(string attendanceRuleID);
        List<AttendanceRuleDTO> GetAttendanceRules(FyuUser user);
    }
}
