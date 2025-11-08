using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using SupermarketPOS.Core;
using SupermarketPOS.Data;

namespace SupermarketPOS.UI
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Product> Cart { get; set; } = [];

        public MainWindow()
        {
            InitializeComponent();
            try 
            {
                DbInitializer.Initialize();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Failed to initialize database:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            CartGrid.ItemsSource = Cart;
        }


        private void BarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;

            try
            {
                var code = BarcodeTextBox.Text.Trim();
                if (string.IsNullOrEmpty(code)) return;

                Product? product;

                using (var db = new AppDbContext())
                {
                    product = db.Products.FirstOrDefault(p => p.Barcode == code);
                }
                // The context is now disposed

                if (product != null)
                {
                    Cart.Add(product);
                    UpdateTotal();
                }
                else
                {
                    MessageBox.Show("Product niet gevonden!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                BarcodeTextBox.Clear();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"An error occurred:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateTotal()
        {
            decimal total = Cart.Sum(p => p.Prijs);

            TotalText.Text = total.ToString("C");
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Check if cart is empty
            if (!Cart.Any())
            {
                MessageBox.Show("Cart is empty. Nothing to save.");
                return;
            }

            // 2. Group the products to get quantities
            // This is the most important part!
            // It turns [Melk, Melk, Brood] into:
            //  - { Product: Melk, Quantity: 2 }
            //  - { Product: Brood, Quantity: 1 }
            var groupedItems = Cart
                .GroupBy(product => product.Barcode)
                .Select(group => new
                {
                    Barcode = group.Key,
                    Quantity = group.Count(),
                    PriceAtSale = group.First().Prijs // Get the price from the first item
                });

            // 3. Save to database using a *new context*
            try
            {
                // Use a new context for a "Unit of Work"
                // This is safer than using your long-lived `_context`
                using (var db = new AppDbContext())
                {
                    var newSale = new Sale
                    {
                        SaleDate = DateTime.UtcNow,
                        TotalAmount = Cart.Sum(p => p.Prijs) // Get total from the original cart
                    };

                    // 4. Create a SaleItem for each *grouped* item
                    foreach (var item in groupedItems)
                    {
                        var saleItem = new SaleItem
                        {
                            ProductBarcode = item.Barcode,
                            Quantity = item.Quantity,
                            PriceAtSale = item.PriceAtSale
                        };
                        // Add the line item to the sale
                        newSale.SaleItems.Add(saleItem);
                    }

                    // 5. Add the new Sale (and its items) to the database
                    db.Sales.Add(newSale);
                    db.SaveChanges();
                }

                MessageBox.Show($"Transaction saved! Total: {TotalText.Text}");

                // 6. Clear the screen (your code was correct here)
                Cart.Clear();
                UpdateTotal();
                BarcodeTextBox.Focus(); // Good to focus the textbox
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving transaction: {ex.Message}\n\n{ex.InnerException?.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
