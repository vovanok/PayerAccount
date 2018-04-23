using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PayerAccount.Dal.Local
{
    public class PayerAccountDbContextFactory : IDesignTimeDbContextFactory<PayerAccountDbContext>
    {
        public PayerAccountDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PayerAccountDbContext>();
            optionsBuilder.UseSqlite("Data Source=PayerAccount.db");

            return new PayerAccountDbContext(optionsBuilder.Options);
        }
    }
}
