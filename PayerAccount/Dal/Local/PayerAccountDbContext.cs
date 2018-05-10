using Microsoft.EntityFrameworkCore;
using PayerAccount.Dal.Local.Data;

namespace PayerAccount.Dal.Local
{
    public class PayerAccountDbContext : DbContext
    {
        public DbSet<Region> Regions { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }

        public PayerAccountDbContext()
            : base()
        {
        }
    }
}
