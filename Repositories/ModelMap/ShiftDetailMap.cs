using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ShiftDetailMap : IEntityTypeConfiguration<ShiftDetail>
    {
        public void Configure(EntityTypeBuilder<ShiftDetail> builder)
        {
            builder.ToTable("ShiftDetail");
            builder.HasKey(t => t.ShiftDetailId);
            builder.Property(t => t.ShiftDetailId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.ShiftId).IsRequired();
            builder.Property(t => t.IsEnableRest).IsRequired();
            builder.Property(t => t.IsEnableTime).IsRequired();
            builder.Property(t => t.DownEndClockTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.DownStartClockTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.EndRestTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.EndWorkTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.StartRestTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.StartWorkTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.UpEndClockTime).IsRequired().HasColumnType("time");
            builder.Property(t => t.UpStartClockTime).IsRequired().HasColumnType("time");
        }
    }
}


