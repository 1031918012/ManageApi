using Domain;
using Infrastructure;

namespace Repositories
{
    public class VersionUpdateRepository : EFRepository<VersionUpdate>, IVersionUpdateRepository, IBaseRepository
    {
        public VersionUpdateRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
