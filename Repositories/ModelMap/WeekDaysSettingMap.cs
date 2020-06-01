using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class WeekDaysSettingMap : IEntityTypeConfiguration<WeekDaysSetting>
    {
        public void Configure(EntityTypeBuilder<WeekDaysSetting> builder)
        {
            builder.ToTable("WeekDaysSetting");
            builder.HasKey(t => t.WeekDaysSettingID);
            builder.Property(t => t.WeekDaysSettingID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceGroupID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Week).IsRequired();
            builder.Property(t => t.ShiftID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.IsHolidayWork).IsRequired();
        }
    }
}
