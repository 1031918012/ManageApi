using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AddressMap : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Address");
            builder.HasKey(t => t.AddressId);
            builder.Property(t => t.AddressId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.AddressName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AddressDetailName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceGroupId).IsRequired();
            builder.Property(t => t.Distance).IsRequired();
            builder.Property(t => t.Latitude).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.Longitude).IsRequired().HasColumnType("double(18,6)");
        }
    }
}


