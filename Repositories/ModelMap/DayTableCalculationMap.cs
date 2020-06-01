using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class DayTableCalculationMap : IEntityTypeConfiguration<DayTableCalculation>
    {
        public void Configure(EntityTypeBuilder<DayTableCalculation> builder)
        {
            builder.ToTable("DayTableCalculation");
            builder.HasKey(t => t.DayTableCalculationID);
            builder.Property(t => t.DayTableCalculationID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceRuleID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.ClockRule).IsRequired();
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired();
            builder.Property(t => t.FlexibleMinutes).IsRequired();
            builder.Property(t => t.IsDynamicRowHugh).IsRequired();
            builder.Property(t => t.IsExemption).IsRequired();
            builder.Property(t => t.IsFlexible).IsRequired();
            builder.Property(t => t.IsHoliday).IsRequired();
            builder.Property(t => t.IsHolidayWork).IsRequired();
            builder.Property(t => t.IsWorkPaidLeave).IsRequired();
            builder.Property(t => t.LateMinutes).IsRequired();
            builder.Property(t => t.WorkingCalculationMethod).IsRequired();
            builder.Property(t => t.RestCalculationMethod).IsRequired();
            builder.Property(t => t.HolidayCalculationMethod).IsRequired();
            builder.Property(t => t.WorkingCompensationMode).IsRequired();
            builder.Property(t => t.RestCompensationMode).IsRequired();
            builder.Property(t => t.HolidayCompensationMode).IsRequired();
            builder.Property(t => t.ExcludingOvertime).IsRequired();
            builder.Property(t => t.LongestOvertime).IsRequired();
            builder.Property(t => t.MinimumOvertime).IsRequired();
            builder.Property(t => t.ShiftTimeManagementList).IsRequired().HasColumnType("json");
            builder.Property(t => t.ShiftType).IsRequired();
            builder.Property(t => t.Week).IsRequired();
            builder.Property(t => t.WorkHours).IsRequired();
            builder.Property(t => t.SiteAttendance).IsRequired().HasColumnType("json");
            builder.Property(t => t.AttendanceRecordID).IsRequired().HasMaxLength(36);
        }
    }
}
