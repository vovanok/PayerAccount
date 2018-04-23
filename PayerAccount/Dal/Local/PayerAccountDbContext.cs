using Microsoft.EntityFrameworkCore;
using PayerAccount.Models.Local;

namespace PayerAccount.Dal.Local
{
    public class PayerAccountDbContext : DbContext
    {
        public DbSet<Region> Regions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        public PayerAccountDbContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
