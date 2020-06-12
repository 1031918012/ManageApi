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
            builder.HasKey(t => t.HolidayManagementId);
            builder.Property(t => t.HolidayManagementId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.HolidayManagementName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.StartTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.EndTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Balance).IsRequired().HasColumnType("json");
            builder.Property(t => t.CustomerId).IsRequired().HasMaxLength(36);
        }
    }
}


