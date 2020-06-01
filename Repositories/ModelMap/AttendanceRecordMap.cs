using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class AttendanceRecordMap : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            builder.ToTable("AttendanceRecord");
            builder.HasKey(t => t.AttendanceRecordID);
            builder.Property(t => t.AttendanceRecordID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.IDCard).IsRequired().IsRequired().HasMaxLength(18);
            builder.Property(t => t.Name).IsRequired().IsRequired().HasMaxLength(30);
            builder.Property(t => t.Department).IsRequired().HasMaxLength(50);
            builder.Property(t => t.DepartmentID).IsRequired();
            builder.Property(t => t.Position).IsRequired().HasMaxLength(50);
            builder.Property(t => t.EmployeeNo).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36); 
            builder.Property(t => t.CustomerID).IsRequired(); 
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceGroupName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceGroupID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.AttendanceItemDTOJson).IsRequired().HasColumnType("json");
            builder.Property(t => t.AttendanceDate).IsRequired().HasColumnType("datetime");
            builder.Property(t => t.Shift).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.WorkingTime).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.LateTimes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.LateMinutes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.EarlyLeaveTimes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.EarlyLeaveMinutes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.NotClockInTimes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.NotClockOutTimes).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.BusinessTripDuration).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.OutsideDuration).IsRequired().HasColumnType("double(18,6)");
            builder.Property(t => t.LeaveDuration).IsRequired().HasColumnType("double(18,6)");
        }
    }
}
