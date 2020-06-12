using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class SchedulingMap : IEntityTypeConfiguration<Scheduling>
    {
        public void Configure(EntityTypeBuilder<Scheduling> builder)
        {
            builder.ToTable("Scheduling");
            builder.HasKey(t => t.SchedulingId);
            builder.Property(t => t.SchedulingId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.ShiftId).IsRequired();
            builder.Property(t => t.IdCard).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Time).IsRequired().HasColumnType("datetime");
        }
    }
}


