using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class AttendanceRuleMap : IEntityTypeConfiguration<AttendanceRule>
    {
        public void Configure(EntityTypeBuilder<AttendanceRule> builder)
        {
            builder.ToTable("AttendanceRule");
            builder.HasKey(t => t.AttendanceRuleID);
            builder.Property(t => t.AttendanceRuleID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.NotClockRule).IsRequired();
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.EarlyLeaveRule).IsRequired();
            builder.Property(t => t.LateRule).IsRequired();
            builder.Property(t => t.Remark).IsRequired().HasMaxLength(200);
            builder.Property(t => t.RuleName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(20);
        }
    }
}
