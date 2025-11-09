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
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = @"C:\Users\samee\Kassa\SupermarketPOS.Data\supermarket.db";
                optionsBuilder.UseSqlite($"Data Source={dbPath}");            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- UPDATED PRODUCT CONFIGURATION ---
            // 'Id' is the Primary Key by convention.
            // We just need to tell EF that 'Barcode' must be unique,
            // so it can be used as a reliable link (an "alternate key").
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Barcode)
                .IsUnique();
            // -------------------------------------

            // Configure the Product -> SaleItem relationship
            // This part is still correct. It tells EF to link
            // SaleItem.ProductBarcode to Product.Barcode (not Product.Id).
            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.Product)
                .WithMany() // A Product can be in many SaleItems
                .HasForeignKey(si => si.ProductBarcode)
                .HasPrincipalKey(p => p.Barcode); // Links to the Product.Barcode property

            // Configure the Sale -> SaleItem relationship (standard)
            // This links SaleItem.SaleId to Sale.Id
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleItems)
                .WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleId);
        }
    }
}