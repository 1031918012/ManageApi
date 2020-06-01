using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceRuleDetailRepository : EFRepository<AttendanceRuleDetail>, IAttendanceRuleDetailRepository, IBaseRepository
    {
        public AttendanceRuleDetailRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}