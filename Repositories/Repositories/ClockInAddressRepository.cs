using Domain;
using Infrastructure;

namespace Repositories
{
    public class ClockInAddressRepository : EFRepository<ClockInAddress>, IClockInAddressRepository, IBaseRepository
    {
        public ClockInAddressRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
