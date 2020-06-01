using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class AttendanceRuleDetailMap : IEntityTypeConfiguration<AttendanceRuleDetail>
    {
        public void Configure(EntityTypeBuilder<AttendanceRuleDetail> builder)
        {
            builder.ToTable("AttendanceRuleDetail");
            builder.HasKey(t => t.AttendanceRuleDetailID);
            builder.Property(t => t.AttendanceRuleDetailID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceRuleID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CallRuleType).IsRequired().HasMaxLength(10);
            builder.Property(t => t.MaxJudge).IsRequired();
            builder.Property(t => t.MaxTime).IsRequired();
            builder.Property(t => t.MinJudge).IsRequired();
            builder.Property(t => t.MinTime).IsRequired();
            builder.Property(t => t.RuleType).IsRequired().HasMaxLength(10);
            builder.Property(t => t.Time).IsRequired();
            builder.Property(t => t.Unit).IsRequired();
            builder.Property(t => t.Sort).IsRequired();
        }
    }
}
