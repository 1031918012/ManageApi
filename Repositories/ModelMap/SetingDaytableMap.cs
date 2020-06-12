using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class SetingDaytableMap : IEntityTypeConfiguration<SetingDaytable>
    {
        public void Configure(EntityTypeBuilder<SetingDaytable> builder)
        {
            builder.ToTable("SetingDaytable");
            builder.HasKey(t => t.SetingDaytableId);
            builder.Property(t => t.SetingDaytableId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceGroupId).IsRequired();
            builder.Property(t => t.CustomerId).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CustomerName).IsRequired().HasMaxLength(36);
            builder.Property(t => t.EarlyFlexible).IsRequired();
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired();
            builder.Property(t => t.IdCard).IsRequired().HasMaxLength(36);
            builder.Property(t => t.IsExemption).IsRequired();
            builder.Property(t => t.IsFlexible).IsRequired();
            builder.Property(t => t.LateFlexible).IsRequired();
            builder.Property(t => t.LateMinutes).IsRequired();
            builder.Property(t => t.SettingTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ShiftDetails).IsRequired().HasColumnType("json");
            builder.Property(t => t.ShiftName).IsRequired().HasMaxLength(50);
        }
    }
}


