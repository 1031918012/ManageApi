using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class ManageItemMap : IEntityTypeConfiguration<ManageItem>
    {
        public void Configure(EntityTypeBuilder<ManageItem> builder)
        {
            builder.ToTable("ManageItem");
            builder.HasKey(t => t.BookID);
            builder.Property(t => t.BookID).IsRequired().HasColumnName("ID");
            builder.Property(t => t.Name).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ModifyTime).IsRequired(false).HasColumnType("datetime");
            builder.Property(t => t.Isdelete).IsRequired();
        }
    }
}
