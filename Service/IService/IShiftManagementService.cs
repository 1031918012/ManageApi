using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IShiftManagementService : IService
    {
        PageResult<ShiftManagementDTO> SelectShiftList(int pageIndex, int pageSize, string companyID, string shiftname);
        ShiftManagementDTO GetShiftManagementByID(string ShiftManagementID);
        bool CheckSameName(string name, string companyid, string id = "");
        bool AddShift(ShiftManagementAddDTO addShiftManagementDTO, FyuUser user);
        JsonResponse UpdateShiftEntity(string shiftManagementID);
        JsonResponse UpdateShift(ShiftManagementDTO ShiftManagementDTO);
        List<ShiftManagementDTO> GetCompanyShiftList(FyuUser fyuUser);
        bool IsCanClock(string shiftID);
    }
}
