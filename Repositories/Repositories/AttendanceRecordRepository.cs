using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceRecordRepository : EFRepository<AttendanceRecord>, IAttendanceRecordRepository, IBaseRepository
    {
        public AttendanceRecordRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
