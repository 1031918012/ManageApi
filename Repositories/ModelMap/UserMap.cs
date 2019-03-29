using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            builder.HasKey(t => t.ID);
            builder.Property(t => t.ID).IsRequired().HasColumnName("ID");
            builder.Property(t => t.Password).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Phone).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Uname).IsRequired().HasMaxLength(100);
            builder.Property(t => t.UNickname).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Sub).IsRequired().HasMaxLength(100);
        }
    }
}
