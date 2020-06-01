using Domain;
using Infrastructure;

namespace Repositories
{
    public class PersonHolidayRepository : EFRepository<PersonHoliday>, IPersonHolidayRepository, IBaseRepository
    {
        public PersonHolidayRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
