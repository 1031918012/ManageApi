using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ClockInAddressMap : IEntityTypeConfiguration<ClockInAddress>
    {
        public void Configure(EntityTypeBuilder<ClockInAddress> builder)
        {
            builder.ToTable("ClockInAddress");
            builder.HasKey(t => t.ClockInAddressID);
            builder.Property(t => t.ClockInAddressID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.ClockName).IsRequired().HasMaxLength(100);
            builder.Property(t => t.SiteName).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Distance).IsRequired();
            builder.Property(t => t.Latitude).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.Longitude).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.LatitudeBD).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.LongitudeBD).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
        }
    }
}

