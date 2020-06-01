using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AttendanceItemUnitMap : IEntityTypeConfiguration<AttendanceItemUnit>
    {
        public void Configure(EntityTypeBuilder<AttendanceItemUnit> builder)
        {
            builder.ToTable("AttendanceItemUnit");
            builder.HasKey(t => t.AttendanceItemUnitID);
            builder.Property(t => t.AttendanceItemUnitID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceItemUnitName).IsRequired().HasMaxLength(10);
            builder.Property(t => t.AttendanceItemID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
        }
    }
}
