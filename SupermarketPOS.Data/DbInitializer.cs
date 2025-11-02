using SupermarketPOS.Core;
using System.Linq;

namespace SupermarketPOS.Data
{
    public static class DbInitializer
    {
        public static void Initialize()
        {
            using var db = new AppDbContext();

            // Ensure database and tables exist
            db.Database.EnsureCreated();

            // Define the desired product list
            var seedProducts = new[]
            {
                new Product { 
                    Barcode = "1", Naam = "Melk 1 L", Prijs = 1.20m, Categorie = "voedsel", Eenheid = "stuk"},
                new Product 
                { Barcode = "2", Naam = "Brood", Prijs = 0.80m, Categorie = "voedsel", Eenheid = "stuk" },
                new Product 
                { Barcode = "3", Naam = "pen", Prijs = 2.50m, Categorie = "Divers", Eenheid = "stuk" },
                new Product 
                { Barcode = "4", Naam = "Appels 1kg", Prijs = 2.00m, Categorie = "voedsel", Eenheid = "kg" }
            };

            // Update existing products and add new ones
            foreach (var newProduct in seedProducts)
            {
                var existingProduct = db.Products.FirstOrDefault(p => p.Barcode == newProduct.Barcode);
                if (existingProduct != null)
                {
                    // Update properties
                    existingProduct.Naam = newProduct.Naam;
                    existingProduct.Prijs = newProduct.Prijs;
                }
                else
                {
                    // Add new product
                    db.Products.Add(newProduct);
                }
            }

            // Remove products no longer in the seed list
            var seedBarcodes = seedProducts.Select(p => p.Barcode).ToHashSet();
            var productsToRemove = db.Products.Where(p => !seedBarcodes.Contains(p.Barcode)).ToList();
            if (productsToRemove.Any())
            {
                db.Products.RemoveRange(productsToRemove);
            }

            // Save all changes at once
            db.SaveChanges();
        }
    }
}

