using Microsoft.EntityFrameworkCore;
using SupermarketPOS.Core;


namespace SupermarketPOS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }

        // Constructor used by EF tools / DI
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=supermarket.db");
            }
        }
    }
}