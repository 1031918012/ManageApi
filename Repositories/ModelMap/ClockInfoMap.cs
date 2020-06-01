using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ClockInfoMap : IEntityTypeConfiguration<ClockInfo>
    {
        public void Configure(EntityTypeBuilder<ClockInfo> builder)
        {
            builder.ToTable("ClockInfo");
            builder.HasKey(t => t.ClockInfoID);
            builder.Property(t => t.ClockInfoID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceRecordID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.ClockInTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ClockInResult).IsRequired();
            builder.Property(t => t.ClockOutResult).IsRequired();
            builder.Property(t => t.ClockOutTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.EndLocation).IsRequired();
            builder.Property(t => t.StartLocation).IsRequired();
            builder.Property(t => t.ShiftTimeID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemDTOJson).IsRequired();
        }
    }
}
