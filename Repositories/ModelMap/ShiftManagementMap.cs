using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ShiftManagementMap : IEntityTypeConfiguration<ShiftManagement>
    {
        public void Configure(EntityTypeBuilder<ShiftManagement> builder)
        {
            builder.ToTable("ShiftManagement");
            builder.HasKey(t => t.ShiftID);
            builder.Property(t => t.ShiftID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.ShiftName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceTime).IsRequired().HasMaxLength(100);
            builder.Property(t => t.WorkHours).IsRequired().HasColumnType("decimal(18,1)");
            builder.Property(t => t.ShiftRemark).IsRequired().HasMaxLength(200);
            builder.Property(t => t.ClockRule).IsRequired();
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.IsDelete).IsRequired().HasColumnType("bit");
            builder.Property(t => t.IsExemption).IsRequired().HasColumnType("bit");
            builder.Property(t => t.LateMinutes).IsRequired();
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired();
            builder.Property(t => t.IsFlexible).IsRequired().HasColumnType("bit");
            builder.Property(t => t.FlexibleMinutes).IsRequired();
        }
    }
}
