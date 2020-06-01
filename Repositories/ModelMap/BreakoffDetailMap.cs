using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class BreakoffDetailMap : IEntityTypeConfiguration<BreakoffDetail>
    {
        public void Configure(EntityTypeBuilder<BreakoffDetail> builder)
        {
            builder.ToTable("BreakoffDetail");
            builder.HasKey(t => t.BreakoffDetailID);
            builder.Property(t => t.BreakoffDetailID).IsRequired().HasColumnName("ID").HasMaxLength(36).ValueGeneratedOnAdd();
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Remark).IsRequired().HasMaxLength(20);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.ChangeTime).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(t => t.CurrentQuota).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(t => t.ChangeType).IsRequired();
            builder.Property(t => t.AttendanceRecordID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
        }
    }
}

