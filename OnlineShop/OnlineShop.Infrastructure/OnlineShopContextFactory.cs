using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OnlineShop.Infrastructure
{
    // Provides the SQLite context configuration to Entity Framework design-time tools.
    public sealed class OnlineShopContextFactory : IDesignTimeDbContextFactory<OnlineShopContext>
    {
        public OnlineShopContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<OnlineShopContext>()
                .UseSqlite(OnlineShopContext.CreateDefaultConnectionString())
                .Options;

            return new OnlineShopContext(options);
        }
    }
}
