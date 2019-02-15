using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class PeopleMap : IEntityTypeConfiguration<People>
    {
        public void Configure(EntityTypeBuilder<People> builder)
        {
            builder.ToTable("People");
            builder.HasKey(t => t.PeopleID);
            builder.Property(t => t.PeopleID).IsRequired().HasColumnName("ID");
            builder.Property(t => t.Name).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.IDCard).IsRequired().HasColumnType("char(18)");
            builder.Property(t => t.BankCard).IsRequired(false).HasMaxLength(50);
        }
    }
}
