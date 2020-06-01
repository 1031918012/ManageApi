using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class AttendanceMonthlyMap : IEntityTypeConfiguration<AttendanceMonthlyRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceMonthlyRecord> builder)
        {
            builder.ToTable("AttendanceMonthlyRecord");
            builder.HasKey(t => t.AttendanceMonthlyRecordID);
            builder.Property(t => t.AttendanceMonthlyRecordID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Department).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Position).IsRequired().HasMaxLength(50);
            builder.Property(t => t.EmployeeNo).IsRequired().HasMaxLength(50);
            builder.Property(t => t.AttendanceProjectsJson).IsRequired().HasColumnType("json");
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(100);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(20);
            builder.Property(t => t.AttendanceDate).IsRequired().HasColumnType("datetime");
        }
    }
}
