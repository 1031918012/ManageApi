using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class PersonMap : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");
            builder.HasKey(t => t.PersonID);
            builder.Property(t => t.PersonID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Department).IsRequired().HasMaxLength(100);
            builder.Property(t => t.DepartmentID).IsRequired();
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.IDType).IsRequired().HasMaxLength(50);
            builder.Property(t => t.IsBindWechat).IsRequired();
            builder.Property(t => t.PhoneCode).IsRequired().HasMaxLength(15);
            builder.Property(t => t.Position).IsRequired().HasMaxLength(50);
            builder.Property(t => t.PositionID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.JobNumber).IsRequired().HasMaxLength(50);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.CustomerID).IsRequired();
            builder.Property(t => t.CompanyName).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Hiredate).IsRequired().HasColumnType("datetime");
            builder.HasIndex(a => new { a.IDCard, a.PersonID }).IsUnique();
        }
    }
}
