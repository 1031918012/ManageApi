using Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service
{
    public interface IAttendanceMonthlyRecordService : IService
    {
        (bool,int) ComputeMonthlyRecord(FyuUser fyuUser,int year,int month);
        PageResult<AttendanceMonthlyRecordDTO> GetMonthlyRecordList(int year, int month, List<string> idCards, int pageIndex, int pageSize, FyuUser user);
        List<AttendanceItemForComputDTO> GetItemComputDTO(FyuUser user);
        bool ComputeMonthlyRecordByIdCard(FyuUser fyuUser, string idCard, int year, int month);
        byte[] GetMonthlyRecord(int year, int month, List<string> idCards, FyuUser user);

        (bool, int) ComputeMonthlyRecord(FyuUser fyuUser, int year, int month, string idCard);
    }
}
