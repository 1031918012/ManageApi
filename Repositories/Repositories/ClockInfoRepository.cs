using Domain;
using Infrastructure;

namespace Repositories
{
    public class ClockInfoRepository : EFRepository<ClockInfo>, IClockInfoRepository, IBaseRepository
    {
        public ClockInfoRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}