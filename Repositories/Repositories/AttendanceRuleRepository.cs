using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceRuleRepository : EFRepository<AttendanceRule>, IAttendanceRuleRepository, IBaseRepository
    {
        public AttendanceRuleRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}