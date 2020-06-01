using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class PersonHolidayMap : IEntityTypeConfiguration<PersonHoliday>
    {
        public void Configure(EntityTypeBuilder<PersonHoliday> builder)
        {
            builder.ToTable("PersonHoliday");
            builder.HasKey(t => t.PersonHolidayID);
            builder.Property(t => t.PersonHolidayID).IsRequired().HasColumnName("ID").HasMaxLength(36).ValueGeneratedOnAdd();
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.HolidayManagementID).IsRequired().HasMaxLength(36).HasColumnType("char(36)");
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.SurplusAmount).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(t => t.TotalSettlement).IsRequired().HasColumnType("decimal(18,6)");
        }
    }
}

