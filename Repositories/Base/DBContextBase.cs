using System.Threading;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Repositories
{
    public class DBContextBase : DbContext
    {
        public DBContextBase(DbContextOptions<DBContextBase> options) : base(options)
        {
        }

        public DbSet<AttendanceGroup> AttendanceGroupList { get; set; }
        public DbSet<ClockRecord> ClockRecordList { get; set; }
        public DbSet<ClockStatistics> ClockStatisticsList { get; set; }
        public DbSet<SetingDaytable> SetingDaytableList { get; set; }
        public DbSet<Address> AddressList { get; set; }
        public DbSet<PersonGroup> PersonGroupList { get; set; }
        public DbSet<Scheduling> SchedulingList { get; set; }
        public DbSet<Shift> ShiftList { get; set; }
        public DbSet<ShiftDetail> ShiftDetailList { get; set; }
        public DbSet<Wifi> WifiList { get; set; }
        public DbSet<HolidayManagement> HolidayManagementList { get; set; }


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.ApplyConfiguration(new AttendanceGroupMap());
        //    base.OnModelCreating(modelBuilder);
        //}
    }
}
