using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class GroupPersonnelMap : IEntityTypeConfiguration<GroupPersonnel>
    {
        public void Configure(EntityTypeBuilder<GroupPersonnel> builder)
        {
            builder.ToTable("GroupPersonnel");
            builder.HasKey(t => t.GroupPersonnelID);
            builder.Property(t => t.GroupPersonnelID).IsRequired().HasColumnName("ID").HasMaxLength(36);
            builder.Property(t => t.AttendanceGroupID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.IDCard).IsRequired().HasMaxLength(18);
            builder.Property(t => t.CompanyID).IsRequired().HasMaxLength(18);
        }
    }
}
