using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class MonthStatisticsMap : IEntityTypeConfiguration<MonthStatistics>
    {
        public void Configure(EntityTypeBuilder<MonthStatistics> builder)
        {
            builder.ToTable("MonthStatistics");
            builder.HasKey(t => t.MonthStatisticsId);
            builder.Property(t => t.MonthStatisticsId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.ActualAttendanceDay).IsRequired();
            builder.Property(t => t.ActualAttendanceTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.AttendanceDay).IsRequired();
            builder.Property(t => t.AttendanceTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired().HasColumnType("time");
            builder.Property(t => t.EarlyLeaveTimes).IsRequired();
            builder.Property(t => t.GoOut).IsRequired().HasColumnType("time");
            builder.Property(t => t.HolidayOvertime).IsRequired().HasColumnType("time");
            builder.Property(t => t.IdCard).IsRequired().HasMaxLength(36);
            builder.Property(t => t.LateMinutes).IsRequired().HasColumnType("time");
            builder.Property(t => t.LateTime).IsRequired();
            builder.Property(t => t.RestOvertime).IsRequired().HasColumnType("time");
            builder.Property(t => t.SettingTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Travel).IsRequired().HasColumnType("time");
            builder.Property(t => t.WorkingOvertime).IsRequired().HasColumnType("time");
            builder.Property(t => t.NotClockInTimes).IsRequired();
            builder.Property(t => t.NotClockOutTimes).IsRequired();
            builder.Property(t => t.Holiday).IsRequired().HasColumnType("time");
        }
    }
}


