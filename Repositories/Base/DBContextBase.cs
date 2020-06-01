using System.Threading;
using System.Threading.Tasks;
using Domain;
using Infrastructure;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Repositories
{
    public class DBContextBase : DbContext
    {
        public DBContextBase(DbContextOptions<DBContextBase> options) : base(options)
        {
        }

        public DbSet<WorkPaidLeave> WorkPaidLeaveList { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new WorkPaidLeaveMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
