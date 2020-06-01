using Domain;
using Infrastructure;

namespace Repositories
{
    public class LeaveRepository : EFRepository<Leave>, ILeaveRepository, IBaseRepository
    {
        public LeaveRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
