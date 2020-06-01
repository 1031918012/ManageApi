using Domain;
using Infrastructure;

namespace Repositories
{
    public class WorkPaidLeaveRepository : EFRepository<WorkPaidLeave>, IWorkPaidLeaveRepository, IBaseRepository
    {
        public WorkPaidLeaveRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
