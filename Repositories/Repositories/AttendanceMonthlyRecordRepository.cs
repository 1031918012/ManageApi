using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceMonthlyRecordRepository : EFRepository<AttendanceMonthlyRecord>, IAttendanceMonthlyRecordRepository, IBaseRepository
    {
        public AttendanceMonthlyRecordRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}