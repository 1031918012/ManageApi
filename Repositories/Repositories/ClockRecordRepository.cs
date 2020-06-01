using Domain;
using Infrastructure;

namespace Repositories
{
    public class ClockRecordRepository : EFRepository<ClockRecord>, IClockRecordRepository, IBaseRepository
    {
        public ClockRecordRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
