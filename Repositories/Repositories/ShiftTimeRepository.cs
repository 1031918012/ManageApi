using Domain;
using Infrastructure;

namespace Repositories
{
    public class ShiftTimeRepository : EFRepository<ShiftTimeManagement>, IShiftTimeRepository, IBaseRepository
    {
        public ShiftTimeRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {

        }
    }
}
