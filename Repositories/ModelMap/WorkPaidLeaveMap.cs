using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class WorkPaidLeaveMap : IEntityTypeConfiguration<WorkPaidLeave>
    {
        public void Configure(EntityTypeBuilder<WorkPaidLeave> builder)
        {
            builder.ToTable("WorkPaidLeave");
            builder.HasKey(t => t.WorkPaidLeaveID);
            builder.Property(t => t.WorkPaidLeaveID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.PaidLeaveTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Type).IsRequired();
            builder.Property(t => t.HolidayID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.HolidayName).IsRequired().HasMaxLength(50);
        }
    }
}


