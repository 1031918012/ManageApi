using Domain;
using Infrastructure;

namespace Repositories
{
    public class PersonHolidayDetailRepository : EFRepository<PersonHolidayDetail>, IPersonHolidayDetailRepository, IBaseRepository
    {
        public PersonHolidayDetailRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
