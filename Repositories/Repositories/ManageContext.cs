using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories
{
    public class ManageContext : DbContext
    {
        public ManageContext(DbContextOptions<ManageContext> options) : base(options)
        {

        }
        public DbSet<ManageItem> ManageItems { get; set; }
        public DbSet<People> Peoples { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ManageItemMap());
            modelBuilder.ApplyConfiguration(new PeopleMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
