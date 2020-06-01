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
        public DbSet<AttendanceItemUnit> AttendanceItemUnitList { get; set; }
        public DbSet<AttendanceItem> AttendanceItemList { get; set; }
        public DbSet<AttendanceItemCatagory> AttendanceItemCatagoryList { get; set; }
        public DbSet<AttendanceMonthlyRecord> AttendanceMonthlyRecordList { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecordList { get; set; }
        public DbSet<AttendanceRule> AttendanceRuleList { get; set; }
        public DbSet<AttendanceRuleDetail> AttendanceRuleDetailList { get; set; }
        public DbSet<ClockInAddress> ClockInAddressList { get; set; }
        public DbSet<ClockRecord> ClockRecordList { get; set; }
        public DbSet<EnterpriseSet> EnterpriseSetList { get; set; }
        public DbSet<GroupPersonnel> GroupPersonnelList { get; set; }
        public DbSet<Holiday> HolidayList { get; set; }
        public DbSet<Leave> LeaveList { get; set; }
        public DbSet<ShiftManagement> ShiftManagementList { get; set; }
        public DbSet<ShiftTimeManagement> ShiftTimeManagementList { get; set; }
        public DbSet<WeekDaysSetting> WeekDaysSettingList { get; set; }
        public DbSet<WorkPaidLeave> WorkPaidLeaveList { get; set; }
        public DbSet<Person> PersonList { get; set; }
        public DbSet<ClockInfo> ClockInfoList { get; set; }
        public DbSet<DayTableCalculation> DayTableCalculationList { get; set; }
        public DbSet<Overtime> OvertimeList { get; set; }
        public DbSet<Breakoff> BreakoffList { get; set; }
        public DbSet<BreakoffDetail> BreakoffDetailList { get; set; }
        public DbSet<HolidayManagement> HolidayManagementList { get; set; }
        public DbSet<PersonHolidayDetail> PersonHolidayDetailList { get; set; }
        public DbSet<PersonHoliday> PersonHolidayList { get; set; }
        public DbSet<AlarmSetting> AlarmSettingList { get; set; }
        public DbSet<VersionUpdate> VersionUpdateList { get; set; }
        public DbSet<Feedback> FeedbackList { get; set; }
        public DbSet<GroupAddress> GroupAddressList { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AttendanceGroupMap());
            modelBuilder.ApplyConfiguration(new AttendanceItemUnitMap());
            modelBuilder.ApplyConfiguration(new AttendanceItemMap());
            modelBuilder.ApplyConfiguration(new AttendanceItemCatagoryMap());
            modelBuilder.ApplyConfiguration(new AttendanceMonthlyMap());
            modelBuilder.ApplyConfiguration(new AttendanceRecordMap());
            modelBuilder.ApplyConfiguration(new AttendanceRuleDetailMap());
            modelBuilder.ApplyConfiguration(new AttendanceRuleMap());
            modelBuilder.ApplyConfiguration(new ClockInAddressMap());
            modelBuilder.ApplyConfiguration(new ClockRecordMap());
            modelBuilder.ApplyConfiguration(new EnterpriseSetMap());
            modelBuilder.ApplyConfiguration(new GroupPersonnelMap());
            modelBuilder.ApplyConfiguration(new HolidayMap());
            modelBuilder.ApplyConfiguration(new ShiftManagementMap());
            modelBuilder.ApplyConfiguration(new ShiftTimeManagementMap());
            modelBuilder.ApplyConfiguration(new WeekDaysSettingMap());
            modelBuilder.ApplyConfiguration(new WorkPaidLeaveMap());
            modelBuilder.ApplyConfiguration(new PersonMap());
            modelBuilder.ApplyConfiguration(new BreakoffMap());
            modelBuilder.ApplyConfiguration(new BreakoffDetailMap());
            modelBuilder.ApplyConfiguration(new OvertimeMap());
            modelBuilder.ApplyConfiguration(new LeaveMap());
            modelBuilder.ApplyConfiguration(new DayTableCalculationMap());
            modelBuilder.ApplyConfiguration(new ClockInfoMap());
            modelBuilder.ApplyConfiguration(new HolidayManagementMap());
            modelBuilder.ApplyConfiguration(new PersonHolidayDetailMap());
            modelBuilder.ApplyConfiguration(new PersonHolidayMap());
            modelBuilder.ApplyConfiguration(new AlarmSettingMap());
            modelBuilder.ApplyConfiguration(new VersionUpdateMap());
            modelBuilder.ApplyConfiguration(new FeedbackMap());
            modelBuilder.ApplyConfiguration(new GroupAddressMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
