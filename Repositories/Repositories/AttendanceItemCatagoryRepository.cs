using Domain;
using Infrastructure;

namespace Repositories
{
    public class AttendanceItemCatagoryRepository : EFRepository<AttendanceItemCatagory>, IAttendanceItemCatagoryRepository, IBaseRepository
    {
        public AttendanceItemCatagoryRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}