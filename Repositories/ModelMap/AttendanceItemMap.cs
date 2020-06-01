﻿using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AttendanceItemMap : IEntityTypeConfiguration<AttendanceItem>
    {
        public void Configure(EntityTypeBuilder<AttendanceItem> builder)
        {
            builder.ToTable("AttendanceItem");
            builder.HasKey(t => t.AttendanceItemID);
            builder.Property(t => t.AttendanceItemID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceItemName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceItemCatagoryID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemCatagoryName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
        }
    }
}
