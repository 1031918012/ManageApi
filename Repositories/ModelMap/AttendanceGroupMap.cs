using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class AttendanceGroupMap: IEntityTypeConfiguration<AttendanceGroup>
    {
        public void Configure(EntityTypeBuilder<AttendanceGroup> builder)
        {
            builder.ToTable("AttendanceGroup");
            builder.HasKey(t => t.AttendanceGroupID);
            builder.Property(t => t.AttendanceGroupID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.ShiftType).IsRequired();
            builder.Property(t => t.ClockInWay).IsRequired();
            builder.Property(t => t.IsDynamicRowHugh).IsRequired();
            builder.Property(t => t.AttendanceRuleID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CreateTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(20);
            builder.Property(t => t.CreatorID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.Creator).IsRequired().HasMaxLength(20);
            builder.Property(t => t.Range).IsRequired().HasMaxLength(100);
            builder.Property(t => t.OvertimeID).IsRequired().HasColumnType("char(36)"); ;
            
        }
    }
}
