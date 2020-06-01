using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ShiftTimeManagementMap : IEntityTypeConfiguration<ShiftTimeManagement>
    {
        public void Configure(EntityTypeBuilder<ShiftTimeManagement> builder)
        {
            builder.ToTable("ShiftTimeManagement");
            builder.HasKey(t => t.ShiftTimeID);
            builder.Property(t => t.ShiftTimeID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.ShiftID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.ShiftName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.ShiftTimeNumber).IsRequired();
            builder.Property(t => t.StartWorkTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.EndWorkTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.StartRestTime).HasColumnType("datetime");
            builder.Property(t => t.EndRestTime).HasColumnType("datetime");
            builder.Property(t => t.UpStartClockTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.UpEndClockTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.DownStartClockTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.DownEndClockTime).IsRequired().HasColumnType("datetime");
        }
    }
}

