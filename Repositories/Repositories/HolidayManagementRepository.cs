using Domain;
using Infrastructure;

namespace Repositories
{
    public class HolidayManagementRepository : EFRepository<HolidayManagement>, IHolidayManagementRepository, IBaseRepository
    {
        public HolidayManagementRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
