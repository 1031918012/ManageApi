using Domain;
using Infrastructure;

namespace Repositories
{
    public class HolidayRepository : EFRepository<Holiday>, IHolidayRepository, IBaseRepository
    {
        public HolidayRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
