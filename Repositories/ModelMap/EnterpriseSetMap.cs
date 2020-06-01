using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class EnterpriseSetMap : IEntityTypeConfiguration<EnterpriseSet>
    {
        public void Configure(EntityTypeBuilder<EnterpriseSet> builder)
        {
            builder.ToTable("EnterpriseSet");
            builder.HasKey(t => t.EnterpriseSetID);
            builder.Property(t => t.EnterpriseSetID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.SortNumber).IsRequired();
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IsEnable).IsRequired().HasColumnType("bit");
        }
    }
}
