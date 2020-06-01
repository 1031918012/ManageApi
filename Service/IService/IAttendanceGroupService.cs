using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IAttendanceGroupService:IService
    {
        PageResult<AttendanceGroupDTO> GetAttendanceGrouPage(int pageIndex, int pageSize, string groupName, FyuUser user);
        JsonResponse DeleteAttendanceRule(string attendanceGroupID);
        JsonResponse AddAttendanceGroup(AttendanceGroupAddDTO attendanceGroupDTO, FyuUser fyuUser);
        AttendanceGroupDTO GetSingleAttendanceGroupDTO(string attendanceGroupID);
        JsonResponse UpdateAttendanceGroup(AttendanceGroupDTO attendanceGroupDTO, FyuUser fyuUser);
        List<GroupDTO> GetAttendanceGroupList(FyuUser user);

        TimeSpan aaa(FyuUser user);
        TimeSpan aaa2(FyuUser user);
    }
}
