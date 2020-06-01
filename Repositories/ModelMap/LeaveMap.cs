using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class LeaveMap : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.ToTable("Leave");
            builder.HasKey(t => t.LeaveID);
            builder.Property(t => t.LeaveID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Department).IsRequired().HasMaxLength(100);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.JobNumber).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.LeaveName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.StartTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.EndTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.AllDay).IsRequired().HasColumnType("double(18,2)");
            builder.Property(t => t.AllTime).IsRequired().HasColumnType("double(18,2)");
        }
    }
}

