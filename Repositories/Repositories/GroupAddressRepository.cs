using Domain;
using Infrastructure;

namespace Repositories
{
    public class GroupAddressRepository : EFRepository<GroupAddress>, IGroupAddressRepository, IBaseRepository
    {
        public GroupAddressRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}