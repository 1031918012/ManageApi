using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repositories
{
    public class AlarmSettingMap : IEntityTypeConfiguration<AlarmSetting>
    {
        public void Configure(EntityTypeBuilder<AlarmSetting> builder)
        {
            builder.ToTable("AlarmSetting");
            builder.HasKey(t => t.AlarmSettingID);
            builder.Property(t => t.AlarmSettingID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.Hour).IsRequired();
            builder.Property(t => t.Minutes).IsRequired();
            builder.Property(t => t.Week).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
        }
    }
}

