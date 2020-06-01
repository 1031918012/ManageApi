using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories
{
    public class GroupAddressMap : IEntityTypeConfiguration<GroupAddress>
    {
        public void Configure(EntityTypeBuilder<GroupAddress> builder)
        {
            builder.ToTable("GroupAddress");
            builder.HasKey(t => t.GroupAddressID);
            builder.Property(t => t.GroupAddressID).IsRequired().HasColumnName("ID").ValueGeneratedOnAdd();
            builder.Property(t => t.AttendanceGroupID).IsRequired().HasMaxLength(36);
            builder.Property(t => t.ClockInAddressID).IsRequired().HasMaxLength(36);
        }
    }
}
