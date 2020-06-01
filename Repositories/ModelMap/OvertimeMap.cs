using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class OvertimeMap : IEntityTypeConfiguration<Overtime>
    {
        public void Configure(EntityTypeBuilder<Overtime> builder)
        {
            builder.ToTable("Overtime");
            builder.HasKey(t => t.OvertimeID);
            builder.Property(t => t.OvertimeID).IsRequired().HasColumnName("ID").HasMaxLength(36).HasColumnType("char(36)");
            builder.Property(t => t.OvertimeName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.WorkingCalculationMethod).IsRequired();
            builder.Property(t => t.RestCalculationMethod).IsRequired();
            builder.Property(t => t.HolidayCalculationMethod).IsRequired();
            builder.Property(t => t.WorkingCompensationMode).IsRequired();
            builder.Property(t => t.RestCompensationMode).IsRequired();
            builder.Property(t => t.HolidayCompensationMode).IsRequired();
            builder.Property(t => t.ExcludingOvertime).IsRequired();
            builder.Property(t => t.LongestOvertime).IsRequired();
            builder.Property(t => t.MinimumOvertime).IsRequired();
            builder.Property(t => t.IsDelete).IsRequired();
            builder.Property(t => t.IsUsed).IsRequired();
        }
    }
}

