using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AttendanceItemCatagoryMap : IEntityTypeConfiguration<AttendanceItemCatagory>
    {
        public void Configure(EntityTypeBuilder<AttendanceItemCatagory> builder)
        {
            builder.ToTable("AttendanceItemCatagory");
            builder.HasKey(t => t.AttendanceItemCatagoryID);
            builder.Property(t => t.AttendanceItemCatagoryID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceItemCatagoryName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
        }
    }
}

