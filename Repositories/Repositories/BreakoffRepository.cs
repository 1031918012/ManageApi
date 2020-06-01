using Domain;
using Infrastructure;

namespace Repositories
{
    public class BreakoffRepository : EFRepository<Breakoff>, IBreakoffRepository, IBaseRepository
    {
        public BreakoffRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
