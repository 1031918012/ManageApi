using Domain;
using Infrastructure;

namespace Repositories
{
    public class EnterpriseSetRepository : EFRepository<EnterpriseSet>, IEnterpriseSetRepository, IBaseRepository
    {
        public EnterpriseSetRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
