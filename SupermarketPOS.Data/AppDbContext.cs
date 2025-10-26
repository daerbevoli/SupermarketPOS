using Microsoft.EntityFrameworkCore;
using SupermarketPOS.Core;

namespace SupermarketPOS.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=supermarket.db");
        }
    }
}
