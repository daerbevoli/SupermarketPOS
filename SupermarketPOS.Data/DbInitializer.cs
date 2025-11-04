using System;
using System.Data.Common;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SupermarketPOS.Core;

namespace SupermarketPOS.Data
{
    public static class DbInitializer
    {
        /// <summary>
        /// Initialize database:
        /// - If migrations history exists => run migrations.
        /// - If no history and no tables => run migrations (creates DB).
        /// - If no history but tables exist => fail with clear message (user should create a baseline migration or delete DB).
        /// After schema is ensured, seed data in a transaction (idempotent).
        /// </summary>
        public static void Initialize()
        {
            using var db = new AppDbContext();

            try
            {
                EnsureSchema(db);
            }
            catch (Exception ex)
            {
                // Re-throw with context so caller can display a clear error
                throw new InvalidOperationException("Database schema initialization failed. See inner exception for details.", ex);
            }

            // Seed data idempotently inside a transaction
            using var transaction = db.Database.BeginTransaction();
            try
            {
                var seedProducts = new[]
                {
                    new Product { Barcode = "1", Naam = "Melk 1 L", Prijs = 1.20m, BTW = 6, Categorie = "voedsel", Eenheid = "stuk" },
                    new Product { Barcode = "2", Naam = "Brood", Prijs = 0.80m, BTW = 6, Categorie = "voedsel", Eenheid = "stuk" },
                    new Product { Barcode = "3", Naam = "pen", Prijs = 2.50m, BTW = 21, Categorie = "Divers", Eenheid = "stuk" },
                    new Product { Barcode = "4", Naam = "Appels 1kg", Prijs = 2.00m, BTW = 6, Categorie = "voedsel", Eenheid = "kg" }
                };

                foreach (var newProduct in seedProducts)
                {
                    var existing = db.Products.FirstOrDefault(p => p.Barcode == newProduct.Barcode);
                    if (existing != null)
                    {
                        // update only the fields you want to keep in sync
                        existing.Naam = newProduct.Naam;
                        existing.Prijs = newProduct.Prijs;
                        existing.BTW = newProduct.BTW;
                        existing.Categorie = newProduct.Categorie;
                        existing.Eenheid = newProduct.Eenheid;
                    }
                    else
                    {
                        db.Products.Add(newProduct);
                    }
                }

                // Remove products that are no longer in the seed list (optional)
                var seedBarcodes = seedProducts.Select(p => p.Barcode).ToHashSet();
                var toRemove = db.Products.Where(p => !seedBarcodes.Contains(p.Barcode)).ToList();
                if (toRemove.Count != 0)
                    db.Products.RemoveRange(toRemove);

                db.SaveChanges();
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private static void EnsureSchema(AppDbContext db)
        {
            // Inspect the underlying sqlite schema to make a safe decision
            var connection = db.Database.GetDbConnection();
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();

                using var cmd = connection.CreateCommand();

                // Check for migrations history table
                cmd.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory'";
                var historyCount = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);

                if (historyCount > 0)
                {
                    // DB was created/managed by EF Migrations -> apply pending migrations
                    db.Database.Migrate();
                    return;
                }

                // No migrations history -- check whether there are any user tables
                cmd.CommandText = "SELECT COUNT(1) FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'";
                var userTableCount = Convert.ToInt32(cmd.ExecuteScalar() ?? 0);

                if (userTableCount == 0)
                {
                    // Database empty: safe to apply migrations (creates schema)
                    db.Database.Migrate();
                    return;
                }

                // DB has user tables but no migrations history -> do NOT run Migrate (it will try to create tables and fail).
                // Provide a clear, actionable exception so the developer can choose a safe recovery:
                // - delete the DB if data can be lost
                // - or create a baseline migration in your project and mark the DB as up-to-date
                throw new InvalidOperationException(
                    "The database file already contains tables but was not created by EF Migrations (no __EFMigrationsHistory table). " +
                    "Options: delete the database file to recreate it, or create a baseline migration (use `Add-Migration InitialCreate -IgnoreChanges`) " +
                    "and then `Update-Database` so EF Migrations can be used safely.");
            }
            finally
            {
                // keep connection closed for caller
                if (connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
            }
        }
    }
}

