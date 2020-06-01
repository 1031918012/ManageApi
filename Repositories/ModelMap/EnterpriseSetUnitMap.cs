using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class EnterpriseSetUnitMap : IEntityTypeConfiguration<EnterpriseSetUnit>
    {
        public void Configure(EntityTypeBuilder<EnterpriseSetUnit> builder)
        {
            builder.ToTable("EnterpriseSetUnit");
            builder.HasKey(t => t.EnterpriseSetUnitID);
            builder.Property(t => t.EnterpriseSetUnitID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.SortNumber).IsRequired();
            builder.Property(t => t.EnterpriseSetID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemUnitID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemUnitName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IsSelect).IsRequired().HasColumnType("bit");
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
        }
    }
}
