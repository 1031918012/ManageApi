using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class VersionUpdateMap : IEntityTypeConfiguration<VersionUpdate>
    {
        public void Configure(EntityTypeBuilder<VersionUpdate> builder)
        {
            builder.ToTable("VersionUpdate");
            builder.HasKey(t => t.VersionUpdateID);
            builder.Property(t => t.VersionUpdateID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.UpdateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.VersionContent).IsRequired().HasColumnType("json");
            builder.Property(t => t.VersionNumber).IsRequired().HasMaxLength(20);
            builder.Property(t => t.VersionTitle).IsRequired().HasMaxLength(50);
        }
    }
}

