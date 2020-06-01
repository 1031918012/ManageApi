using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class FeedbackMap : IEntityTypeConfiguration<Feedback>
    {
        public void Configure(EntityTypeBuilder<Feedback> builder)
        {
            builder.ToTable("Feedback");
            builder.HasKey(t => t.FeedbackID);
            builder.Property(t => t.FeedbackID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(20);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.FeedbackType).IsRequired();
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Content).IsRequired().HasMaxLength(255);
            builder.Property(t => t.Path).IsRequired().HasColumnType("json");
        }
    }
}

