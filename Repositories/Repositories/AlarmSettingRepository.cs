using Domain;
using Infrastructure;

namespace Repositories
{
    public class AlarmSettingRepository : EFRepository<AlarmSetting>, IAlarmSettingRepository, IBaseRepository
    {
        public AlarmSettingRepository(DBContextBase dBContextBase) : base(dBContextBase)
        {
        }
    }
}
