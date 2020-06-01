using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IEnterpriseSetService : IService
    {
        List<AttendanceItemCatagoryDTO> SelectAttendanceItem();
        PageResult<EnterpriseSetDTO> SelectEnterpriseSetList(int pageIndex, int pageSize, string companyID);
        List<AttendanceItemCatagoryDTO> GetEnterpriseSetByID(string companyID);
        bool CheckEnterpriseSet(string companyID);
        bool AddEnterpriseSet(string companyID, string companyName, List<AttendanceItemCatagoryAddDTO> addEnterpriseSetList, FyuUser user);
        bool UpdateEnterpriseSet(string companyID, List<AttendanceItemCatagoryDTO> EnterpriseSetDTO);
        SetRuleDTO GetSetRule(FyuUser user);
    }
}
