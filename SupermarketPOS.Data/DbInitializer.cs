using SupermarketPOS.Core;

namespace SupermarketPOS.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            if (!db.Products.Any())
            {
                db.Products.AddRange(new[]
                {
                    new Product { barcode = "1234567890123", name = "Milk 1L", price = 1.20m },
                    new Product { barcode = "9876543210987", name = "Bread", price = 0.80m },
                    new Product { barcode = "1111111111111", name = "Apples 1kg", price = 2.50m, hasPromo = true }
                });
                db.SaveChanges();
            }
        }
    }
}
