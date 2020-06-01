using Domain;
using Infrastructure;

namespace Repositories
{
    public class WeekDaysSettingRepository : EFRepository<WeekDaysSetting>, IWeekDaysSettingRepository, IBaseRepository
    {
        public WeekDaysSettingRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}