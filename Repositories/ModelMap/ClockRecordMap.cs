using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class ClockRecordMap : IEntityTypeConfiguration<ClockRecord>
    {
        public void Configure(EntityTypeBuilder<ClockRecord> builder)
        {
            builder.ToTable("ClockRecord");
            builder.HasKey(t => t.ClockRecordID);
            builder.Property(t => t.ClockRecordID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.EmployeeNo).HasMaxLength(50);
            builder.Property(t => t.Department).HasMaxLength(50);
            builder.Property(t => t.DepartmentID).IsRequired();
            builder.Property(t => t.Position).HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CustomerID).IsRequired();
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceDate).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ClockTime).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.ClockType);
            builder.Property(t => t.ClockResult);
            builder.Property(t => t.IsFieldAudit);
            builder.Property(t => t.ClockWay);
            builder.Property(t => t.IsInRange).HasColumnType("bit");
            builder.Property(t => t.Location).HasMaxLength(255);
            builder.Property(t => t.Remark).HasMaxLength(255);
            builder.Property(t => t.AbnormalReason).HasMaxLength(255);
            builder.Property(t => t.ClockImage1).HasMaxLength(255);
            builder.Property(t => t.ClockImage2).HasMaxLength(255);
            builder.Property(t => t.ClockDevice).HasMaxLength(100);
            builder.Property(t => t.ShiftTimeID).HasMaxLength(36);
            builder.Property(t => t.Latitude).HasMaxLength(100);
            builder.Property(t => t.Longitude).HasMaxLength(100);
        }
    }
}
