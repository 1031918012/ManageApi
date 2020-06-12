using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AttendanceGroupMap : IEntityTypeConfiguration<AttendanceGroup>
    {
        public void Configure(EntityTypeBuilder<AttendanceGroup> builder)
        {
            builder.ToTable("AttendanceGroup");
            builder.HasKey(t => t.AttendanceGroupId);
            builder.Property(t => t.AttendanceGroupId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceGroupName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceTypeEnum).IsRequired();
            builder.Property(t => t.AutomaticSchedule).IsRequired();
            builder.Property(t => t.CustomerId).HasMaxLength(36).IsRequired();
            builder.Property(t => t.FixedShift).IsRequired().HasColumnType("json");
            builder.Property(t => t.IsClockAddress).IsRequired();
            builder.Property(t => t.IsWifi).IsRequired();
            builder.Property(t => t.OvertimeRulesId).IsRequired();
            builder.Property(t => t.ScheduleShift).IsRequired().HasColumnType("json");
            builder.Property(t => t.TimeAcrossDays).IsRequired().HasColumnType("time");
        }
    }
}


