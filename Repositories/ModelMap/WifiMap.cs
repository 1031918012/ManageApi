using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class WifiMap : IEntityTypeConfiguration<Wifi>
    {
        public void Configure(EntityTypeBuilder<Wifi> builder)
        {
            builder.ToTable("Wifi");
            builder.HasKey(t => t.WifiId);
            builder.Property(t => t.WifiId).IsRequired().HasColumnName("Id").ValueGeneratedOnAdd();
            builder.Property(t => t.Mac).IsRequired().HasMaxLength(30);
            builder.Property(t => t.WifiName).IsRequired().HasMaxLength(50);
        }
    }
}


