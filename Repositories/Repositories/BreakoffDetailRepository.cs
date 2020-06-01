using Domain;
using Infrastructure;

namespace Repositories
{
    public class BreakoffDetailRepository : EFRepository<BreakoffDetail>, IBreakoffDetailRepository, IBaseRepository
    {
        public BreakoffDetailRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
