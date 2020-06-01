using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class HolidayMap : IEntityTypeConfiguration<Holiday>
    {
        public void Configure(EntityTypeBuilder<Holiday> builder)
        {
            builder.ToTable("Holiday");
            builder.HasKey(t => t.HolidayID);
            builder.Property(t => t.HolidayID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.HolidayName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.HolidayYear).IsRequired();
            builder.Property(t => t.HolidayNumber).IsRequired();
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.IsDelete).IsRequired().HasColumnType("bit");
            builder.Property(t => t.StartHolidayTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.EndHolidayTime).IsRequired().HasColumnType("datetime");
        }
    }
}

