using Domain;
using Infrastructure;

namespace Repositories
{
    public class OverTimeRepository : EFRepository<Overtime>, IOvertimeRepository, IBaseRepository
    {
        public OverTimeRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
