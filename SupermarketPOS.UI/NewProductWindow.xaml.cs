using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SupermarketPOS.Core;
using SupermarketPOS.Data;

namespace SupermarketPOS.UI
{
    public partial class NewProductWindow : UserControl
    {
        public MainWindow MainWindow { get; set; }

        public NewProductWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow?.CloseOverlay();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var barcode = BarcodeTextBox.Text.Trim();
            var naam = NaamTextBox.Text.Trim();
            var prijsText = PrijsTextBox.Text.Trim();
            var btwText = BTWTextBox.Text.Trim();
            var eenheid = EenheidTextBox.Text.Trim();
            var categorie = CategorieTextBox.Text.Trim();
            var voorraadText = VoorraadTextBox.Text.Trim();

            if (string.IsNullOrEmpty(barcode))
            {
                MessageBox.Show("Barcode is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(naam))
            {
                MessageBox.Show("Name is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(prijsText, NumberStyles.Number, CultureInfo.InvariantCulture, out var prijs))
            {
                MessageBox.Show("Price is invalid. Use a numeric value (e.g. 1.20).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(btwText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var btw))
            {
                MessageBox.Show("BTW is invalid. Use an integer percent (e.g. 6).", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(voorraadText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var voorraad))
            {
                MessageBox.Show("Stock is invalid. Use an integer value.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using var db = new AppDbContext();

                if (db.Products.Any(p => p.Barcode == barcode))
                {
                    MessageBox.Show("A product with this barcode already exists.", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var product = new Product
                {
                    Barcode = barcode,
                    Naam = naam,
                    Prijs = prijs,
                    BTW = btw,
                    Eenheid = string.IsNullOrEmpty(eenheid) ? "stuk" : eenheid,
                    Categorie = string.IsNullOrEmpty(categorie) ? "Overig" : categorie,
                    Voorraad = voorraad
                };

                db.Products.Add(product);
                db.SaveChanges();

                MessageBox.Show("Product saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow?.CloseOverlay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}