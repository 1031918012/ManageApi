using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AttendanceGroupMap : IEntityTypeConfiguration<AttendanceGroup>
    {
        public void Configure(EntityTypeBuilder<AttendanceGroup> builder)
        {
            builder.ToTable("AttendanceGroup");
            builder.HasKey(t => t.AttendanceGroupId);
            builder.Property(t => t.AttendanceGroupId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceGroupName).IsRequired().HasMaxLength(50);
        }
    }
}


