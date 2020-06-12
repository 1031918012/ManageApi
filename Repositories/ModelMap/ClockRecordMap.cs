using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ClockRecordMap : IEntityTypeConfiguration<ClockRecord>
    {
        public void Configure(EntityTypeBuilder<ClockRecord> builder)
        {
            builder.ToTable("ClockRecord");
            builder.HasKey(t => t.ClockRecordId);
            builder.Property(t => t.ClockRecordId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.Address).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceDate).IsRequired().HasColumnType("date");
            builder.Property(t => t.ClockTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ShiftTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ClockType).IsRequired();
            builder.Property(t => t.IdCard).IsRequired().HasMaxLength(36);
            builder.Property(t => t.ImageUrl).IsRequired().HasColumnType("json");
            builder.Property(t => t.Latitude).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.Longitude).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.Remark).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Result).IsRequired();
        }
    }
}


