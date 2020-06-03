using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceGroupRepository : EFRepository<AttendanceGroup>, IAttendanceGroupRepository, IBaseRepository
    {
        public AttendanceGroupRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
