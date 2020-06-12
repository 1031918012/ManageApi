using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ShiftMap : IEntityTypeConfiguration<Shift>
    {
        public void Configure(EntityTypeBuilder<Shift> builder)
        {
            builder.ToTable("Shift");
            builder.HasKey(t => t.ShiftId);
            builder.Property(t => t.ShiftId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.ShiftName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IsExemption).IsRequired();
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired();
            builder.Property(t => t.LateMinutes).IsRequired();
            builder.Property(t => t.IsFlexible).IsRequired();
            builder.Property(t => t.EarlyFlexible).IsRequired();
            builder.Property(t => t.LateFlexible).IsRequired();
            builder.Property(t => t.CustomerId).IsRequired().HasMaxLength(36);
        }
    }
}


