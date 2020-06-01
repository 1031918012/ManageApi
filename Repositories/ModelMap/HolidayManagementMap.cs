using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class HolidayManagementMap : IEntityTypeConfiguration<HolidayManagement>
    {
        public void Configure(EntityTypeBuilder<HolidayManagement> builder)
        {
            builder.ToTable("HolidayManagement");
            builder.HasKey(t => t.HolidayManagementID);
            builder.Property(t => t.HolidayManagementID).IsRequired().HasColumnName("ID").HasMaxLength(36).HasColumnType("char(36)");
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36).HasColumnType("char(36)");
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.DateOfIssue).IsRequired();
            builder.Property(t => t.DistributionMethod).IsRequired();
            builder.Property(t => t.EnableBalance).IsRequired();
            builder.Property(t => t.FixedData).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(t => t.HolidayName).IsRequired().HasMaxLength(20);
            builder.Property(t => t.IssuingCycle).IsRequired();
            builder.Property(t => t.QuotaRule).IsRequired();
            builder.Property(t => t.Seniority).IsRequired().HasColumnType("json");
            builder.Property(t => t.ValidityOfLimit).IsRequired();
            builder.Property(t => t.WorkingYears).IsRequired().HasColumnType("json");
        }
    }
}

