using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class PersonGroupMap : IEntityTypeConfiguration<PersonGroup>
    {
        public void Configure(EntityTypeBuilder<PersonGroup> builder)
        {
            builder.ToTable("PersonGroup");
            builder.HasKey(t => t.PersonGroupId);
            builder.Property(t => t.PersonGroupId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(25);
            builder.Property(t => t.IdCard).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CustomerName).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CustomerId).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceGroupId).IsRequired();
        }
    }
}


