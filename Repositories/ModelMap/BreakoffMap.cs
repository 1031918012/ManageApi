using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class BreakoffMap : IEntityTypeConfiguration<Breakoff>
    {
        public void Configure(EntityTypeBuilder<Breakoff> builder)
        {
            builder.ToTable("Breakoff");
            builder.HasKey(t => t.BreakoffID);
            builder.Property(t => t.BreakoffID).IsRequired().HasColumnName("ID").HasMaxLength(36).ValueGeneratedOnAdd();
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.SurplusAmount).IsRequired().HasColumnType("decimal(18,6)");
            builder.Property(t => t.TotalSettlement).IsRequired().HasColumnType("decimal(18,6)");
        }
    }
}

