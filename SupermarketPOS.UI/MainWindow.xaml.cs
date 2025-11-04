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
        private readonly AppDbContext _context = new();
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
            if (e.Key == Key.Enter)
            {
                try
                {
                    var code = BarcodeTextBox.Text.Trim();
                    var product = _context.Products.FirstOrDefault(p => p.Barcode == code);

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
        }

        private void UpdateTotal()
        {
            TotalText.Text = $"{Cart.Sum(p => p.Prijs)}";
        }

        private void CashButton_Click(object sender, RoutedEventArgs e)
        {
            Cart.Clear();
            UpdateTotal();
        }
    }
}
