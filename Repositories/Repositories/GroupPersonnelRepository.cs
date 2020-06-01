using Domain;
using Infrastructure;

namespace Repositories
{
    public class GroupPersonnelRepository : EFRepository<GroupPersonnel>, IGroupPersonnelRepository, IBaseRepository
    {
        public GroupPersonnelRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}