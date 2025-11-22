using SupermarketPOS.Core;
using SupermarketPOS.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace SupermarketPOS.UI
{
    public partial class SalesHistoryWindow : UserControl
    {

        public MainWindow MainWindow { get; set; }

        public SalesHistoryWindow()
        {
            InitializeComponent();
            LoadSales(); // Load sales when the window is created
        }

        private void LoadSales()
        {
            try
            {
                using var db = new AppDbContext();
                // Load all sales, show the newest first
                var sales = db.Sales
                              .OrderByDescending(s => s.SaleDate)
                              .ToList();
                SalesGrid.ItemsSource = sales;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sales: {ex.Message}", "Error");
            }
        }

        private void SalesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the sale that the user just clicked on
            Sale? selectedSale = SalesGrid.SelectedItem as Sale;
            if (selectedSale == null)
            {
                SaleItemsGrid.ItemsSource = null; // Clear bottom grid if nothing is selected
                return;
            }

            try
            {
                using var db = new AppDbContext();
                // Find all SaleItems for the selected SaleId
                // We use .Include() to also load the related Product data (like the Name)
                var items = db.SaleItems
                              .Include(si => si.Product) // <-- IMPORTANT!
                              .Where(si => si.SaleId == selectedSale.Id)
                              .ToList();

                SaleItemsGrid.ItemsSource = items;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load sale items: {ex.Message}", "Error");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow?.CloseOverlay();
        }
    }
}