using Domain;
using Infrastructure;

namespace Repositories
{
    public class ShiftManagementRepository : EFRepository<ShiftManagement>, IShiftManagementRepository, IBaseRepository
    {
        public ShiftManagementRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
