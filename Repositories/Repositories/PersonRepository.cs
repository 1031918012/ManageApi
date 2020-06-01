using Domain;
using Infrastructure;

namespace Repositories
{
    public class PersonRepository : EFRepository<Person>, IPersonRepository, IBaseRepository
    {
        public PersonRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}